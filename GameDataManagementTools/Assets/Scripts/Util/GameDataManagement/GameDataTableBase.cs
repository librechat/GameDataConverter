using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System;
using System.IO;

public class GameDataTableBase<T> : ScriptableObject where T:GameDataBase
{
    public List<T> DataList;

    public T GetDataById(int id)
    {
        for (int i = 0; i < DataList.Count; i++)
        {
            if (DataList[i].Id == id) return DataList[i];
        }
        return default(T);
    }

    public T GetDataBy(Func<T,bool> condition)
    {
        for (int i = 0; i < DataList.Count; i++)
        {
            if (condition(DataList[i])) return DataList[i];
        }
        return default(T);
    }

    public Dictionary<int, T> GetDataDicInt()
    {
        Dictionary<int, T> dic = new Dictionary<int, T>();
        for (int i = 0; i < DataList.Count; i++)
        {
            dic.Add(DataList[i].Id, DataList[i]);
        }
        return dic;
    }

    public Dictionary<string, T> GetDataDicName()
    {
        if (typeof(T).GetFields().Where(x => x.Name == "Name" && x.FieldType == typeof(string)).ToList().Count > 0)
        {
            Dictionary<string, T> dic = new Dictionary<string, T>();
            for (int i = 0; i < DataList.Count; i++)
            {
                string name = typeof(T).GetField("Name").GetValue(DataList[i]) as string;
                dic.Add(name, DataList[i]);
            }
            return dic;
        }
        return null;
    }
}
