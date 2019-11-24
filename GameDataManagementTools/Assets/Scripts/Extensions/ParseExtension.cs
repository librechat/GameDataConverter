using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ParseExtension
{    
    public static string ToStringExt<T>(this List<T> value)
    {
        string s = "";

        for (int i = 0; i < value.Count; i++)
        {
            if (i == 0) s += value[i].ToString();
            else s += "," + value[i].ToString();
        }
        return s;
    }

    public static Vector3 ToVector3(this string value)
    {
        if (value == "") return Vector3.zero;
        string[] strs = value.Split(',');
        Vector3 vec = new Vector3();
        vec.x = float.Parse(strs[0]);
        vec.y = float.Parse(strs[1]);
        vec.z = float.Parse(strs[2]);
        return vec;
    }
    public static List<T> ToListExt<T>(this string value)
    {
        List<T> list = new List<T>();
        if (value == "") return list;

        string[] strs = value.Split(',');
        for (int i = 0; i < strs.Length; i++)
        {
            if (typeof(T) == typeof(string))
            {
                list.Add((T)Convert.ChangeType(strs[i], typeof(T)));
            }
            else if (typeof(T) == typeof(int))
            {
                int num = Int32.Parse(strs[i]);
                list.Add((T)Convert.ChangeType(num, typeof(T)));
            }
            else if (typeof(T) == typeof(float))
            {
                float f = float.Parse(strs[i]);
                list.Add((T)Convert.ChangeType(f, typeof(T)));
            }
            else if (typeof(T) == typeof(bool))
            {
                bool b = bool.Parse(strs[i]);
                list.Add((T)Convert.ChangeType(b, typeof(T)));
            }
        }
        return list;
    }
}