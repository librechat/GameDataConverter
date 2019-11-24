using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

[System.Serializable]
public class GameDataBase
{
    public int Id;

    #region FieldFunctions
    public virtual List<string> GetFieldNames()
    {
        List<string> names = new List<string>();
        names.Add("Id");
        return names;
    }
    public virtual void SetField(FieldInfo field, string text)
    {
        Type type = field.FieldType;
        if(type == typeof(string)) field.SetValue(this, text);

        if (type == typeof(Vector3))
        {
            field.SetValue(this, text.ToVector3());
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type[] argTypes = type.GetGenericArguments();
            if (argTypes[0] == typeof(string)) field.SetValue(this, text.ToListExt<string>());
            else if (argTypes[0] == typeof(int)) field.SetValue(this, text.ToListExt<int>());
            else if (argTypes[0] == typeof(float)) field.SetValue(this, text.ToListExt<float>());
            else if (argTypes[0] == typeof(bool)) field.SetValue(this, text.ToListExt<bool>());
        }
        else
        {
            if (type == typeof(int)) field.SetValue(this, Int32.Parse(text));
            else if (type == typeof(float)) field.SetValue(this, float.Parse(text));
            else if (type == typeof(bool)) field.SetValue(this, bool.Parse(text));
        }       
    }
    public virtual string GetField(FieldInfo field)
    {
        Type type = field.FieldType;
        if (type == typeof(Vector3))
        {
            Vector3 value = (Vector3)field.GetValue(this);
            return value.x.ToString() + "," + value.y.ToString() + "," + value.z.ToString();
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type[] argTypes = type.GetGenericArguments();
            System.Object listObj = field.GetValue(this);
            if (argTypes[0] == typeof(string)) return ((List<string>)listObj).ToStringExt<string>();
            else if (argTypes[0] == typeof(int)) return ((List<int>)listObj).ToStringExt<int>();
            else if (argTypes[0] == typeof(float)) return ((List<float>)listObj).ToStringExt<float>();
            else if (argTypes[0] == typeof(bool)) return ((List<bool>)listObj).ToStringExt<bool>();
            else return "";
        }
        else return field.GetValue(this).ToString();
    }
    #endregion
}