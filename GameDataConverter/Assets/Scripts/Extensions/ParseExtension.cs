using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

public static class ParseExtension
{
    /*
     * Primitive types here includes:
     * int, float, bool, string, Vector3, enums
     */
    private static List<Type> primitiveTypeList = new List<Type>() { 
        typeof(int), typeof(float), typeof(bool), typeof(string), typeof(Vector3)
    };
    
    public static string ObjectToString(this System.Object value, Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type[] argTypes = type.GetGenericArguments();
            
            if (argTypes[0].IsEnum || primitiveTypeList.Find(x => x == argTypes[0]) != null)
            {
                return (string)typeof(ParseExtension).GetMethod("ListToString").MakeGenericMethod(argTypes[0])
                .Invoke(null, new object[] { value });
            }
            else
            {
                printInvalidTypeWarning(argTypes[0], true);
                return "";
            }
        }
        else
        {
            return value.PrimitivesToString(type);
        }       
    }
    public static System.Object StringToObject(this string value, Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type[] argTypes = type.GetGenericArguments();

            if (argTypes[0].IsEnum || primitiveTypeList.Find(x => x == argTypes[0]) != null)
            {
                return typeof(ParseExtension).GetMethod("StringToList").MakeGenericMethod(argTypes[0])
                    .Invoke(null, new object[] { value, argTypes[0] });
            }
            else
            {
                printInvalidTypeWarning(argTypes[0], true);
                return null;
            }
        }
        else
        {
            return value.StringToPrimitives(type);
        }
    }
    
    public static string ListToString<T>(this List<T> value)
    {
        string s = "";
        Type argType = typeof(T);

        for (int i = 0; i < value.Count; i++)
        {
            if (i == 0) s += value[i].PrimitivesToString(argType);
            else s += "," + value[i].PrimitivesToString(argType);
        }
        return s;
    }
    public static List<T> StringToList<T>(this string value, Type argType)
    {
        List<T> list = new List<T>();
        if (value == "") return list;

        string[] strs = value.Split(',');
        if (argType == typeof(Vector3))
        {
            strs = System.Text.RegularExpressions.Regex.Split(value, @"(?:\))(,)(?:\()");
        }

        for (int i = 0; i < strs.Length; i++)
        {
            list.Add((T)strs[i].StringToPrimitives(argType));
        }
        return list;
    }

    private static string PrimitivesToString(this System.Object value, Type type)
    {
        if (type.IsEnum || type == typeof(string) || type == typeof(int)
            || type == typeof(float) || type == typeof(bool)) return value.ToString();
        else if (type == typeof(Vector3))
        {
            Vector3 vec = (Vector3)Convert.ChangeType(value, typeof(Vector3));
            return vec.Vector3ToString();
        }
        else
        {
            printInvalidTypeWarning(type);
            return "";
        }
    }
    private static System.Object StringToPrimitives(this string value, Type type) // including Vector3 and string
    {       
        if (type == typeof(string))
        {
            return Convert.ChangeType(value, type);
        }
        else if (type == typeof(Vector3))
        {
            return Convert.ChangeType(value.StringToVector3(), type);
        }        
        else if (type == typeof(int))
        {
            int result = 0;
            bool success = Int32.TryParse(value, out result);
            if (!success) printInvalidInputWarning(value);

            return Convert.ChangeType(result, type);
        }
        else if (type == typeof(float))
        {
            float result = 0.0f;
            bool success = float.TryParse(value, out result);
            if (!success) printInvalidInputWarning(value);

            return Convert.ChangeType(result, type);
        }
        else if (type == typeof(bool))
        {
            bool result = false;
            if (value == "0") result = false;
            else if (value == "1") result = true;
            else
            {
                bool success = bool.TryParse(value, out result);
                if (!success) printInvalidInputWarning(value);
            }       

            return Convert.ChangeType(result, type);
        }
        else if (type.IsEnum)
        {
            var result = Activator.CreateInstance(type);

            var allMethods = typeof(Enum).GetMethods(BindingFlags.Public | BindingFlags.Static);
            MethodInfo method = allMethods.FirstOrDefault(
                mi => mi.Name == "TryParse" && mi.GetParameters().Length == 2);

            var parameters = new object[] { value, result };
            bool success = (bool) method.MakeGenericMethod(type).Invoke(null, parameters);
            if (!success) printInvalidInputWarning(value);

            return parameters[1];
        }
        else
        {
            printInvalidTypeWarning(type);
            return null;
        }
    }

    public static string Vector3ToString(this Vector3 value)
    {
        return "(" + value.x + "," + value.y + "," + value.z + ")";
    }
    public static Vector3 StringToVector3(this string value)
    {
        if (value == "")
        {
            printInvalidInputWarning(value);
            return Vector3.zero;
        }

        string str = System.Text.RegularExpressions.Regex.Replace(value, @"[\(\)]", "");
        string[] strs = str.Split(',');

        if (strs.Length != 3)
        {
            printInvalidInputWarning(value);
            return Vector3.zero;
        }

        Vector3 vec = new Vector3();
        float result = 0.0f;

        bool success = float.TryParse(strs[0], out result);
        if (!success)
        {
            printInvalidInputWarning(value);
            return Vector3.zero;
        }
        vec.x = result;

        success = float.TryParse(strs[1], out result);
        if (!success)
        {
            printInvalidInputWarning(value);
            return Vector3.zero;
        }
        vec.y = result;

        success = float.TryParse(strs[2], out result);
        if (!success)
        {
            printInvalidInputWarning(value);
            return Vector3.zero;
        }
        vec.z = result;
        return vec;
    }

    private static void printInvalidInputWarning(string input)
    {
        Debug.LogError("Invalid input for this type! Use default value instead. / Input string:" + input);
    }
    private static void printInvalidTypeWarning(Type type, bool isArgument = false)
    {
        string opt = (isArgument) ? "argument" : "data";
        Debug.LogError("Invalid "+ opt +" type! Use null or empty string instead. / Type:" + type);
    }
}