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
        field.SetValue(this, text.StringToObject(type));
    }
    public virtual string GetField(FieldInfo field)
    {
        Type type = field.FieldType;
        System.Object obj = field.GetValue(this);
        
        return obj.ObjectToString(type);
    }
    #endregion
}