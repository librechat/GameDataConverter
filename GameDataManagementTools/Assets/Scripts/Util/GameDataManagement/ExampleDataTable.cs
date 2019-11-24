using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "CustomScriptableObject/ExampleDataTable")]
public class ExampleDataTable : GameDataTableBase<ExampleData>
{
    public List<ExampleData> ExampleDataList
    {
        get
        {
            return DataList;
        }
    }
}
