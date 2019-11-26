using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ExampleData : GameDataBase
{
    public enum ExampleDataType
    {
        TypeA = 0,
        TypeB,
        TypeC
    }
    public string Name;
    public bool Available;
    public ExampleDataType Type;
    public Vector3 Position;    
    public float Speed;
    public List<int> FriendList;

    public Sprite Icon;
    public Transform Prefab;

    public override List<string> GetFieldNames()
    {
        List<string> s = base.GetFieldNames();
        string[] addition = 
        { 
            "Name", "Available", "Type", "Position", "Speed", "FriendList"
        };

        s.AddRange(addition);
        return s;
    }
    public override string GetField(System.Reflection.FieldInfo field)
    {
        return base.GetField(field);
    }
    public override void SetField(System.Reflection.FieldInfo field, string text)
    {
        base.SetField(field, text);
    } 
}