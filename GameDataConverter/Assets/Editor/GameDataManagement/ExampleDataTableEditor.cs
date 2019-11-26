using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExampleDataTable))]
public class LevelDataEditor : GameDataTableBaseEditor<ExampleData>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected override void SetParameters()
    {
        base.SetParameters();

        /*
        fileName = "Try a new file name";

        ImportToggle.GoogleSheet = false;
        ImportToggle.Excel = false;
        ExportToggle.GoogleSheet = false;
        ExportToggle.Excel = false;*/
    }
}