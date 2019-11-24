using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;

// [CustomEditor(typeof(GameDataTableBase<T>))]
public class GameDataTableBaseEditor<T> : Editor where T : GameDataBase, new()
{
    bool fold = false;

    protected class TargetToggle {
        public bool GoogleSheet = true;
        public bool Excel = true;
        public bool Json = true;
    };
    protected TargetToggle ImportToggle = new TargetToggle();
    protected TargetToggle ExportToggle = new TargetToggle();

    protected string fileName = "GameDataFile";
    protected string sheetName = "Sheet";

    void OnEnable()
    {
        SetParameters();
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Import and Export", EditorStyles.boldLabel);
        string hint = 
            "Google Sheet and Excel DO NOT support object references (prefab, image, ...). Json supports.";
        EditorGUILayout.HelpBox(hint, MessageType.None);
        drawImportExportGUI();

        fold = EditorGUILayout.Foldout(fold, "File Options");
        if (fold)
        {
            fileName = EditorGUILayout.TextField("File Name: ", fileName, GUI.skin.textField);
            sheetName = EditorGUILayout.TextField("Sheet Name: ", sheetName, GUI.skin.textField);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Open destination: ", EditorStyles.label);
            drawBtn("Google Sheet", GUI.skin.button, true, () =>
            {
                Application.OpenURL("https://docs.google.com/spreadsheets/d/" + GameConstant.GoogleSheetId);
            });
            drawBtn("Excel Folder", GUI.skin.button, true, () => {
                System.Diagnostics.Process.Start(GameConstant.ExcelSourceFolder);
            });
            drawBtn("Json Folder", GUI.skin.button, true, () =>
            {
                System.Diagnostics.Process.Start(GameConstant.JsonSourceFolder);
            });
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Label("Table Content", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Edit the ScriptableObject directly.", MessageType.None);
        base.OnInspectorGUI();
    }
    protected virtual void SetParameters()
    {
        string assetPath = AssetDatabase.GetAssetPath(target.GetInstanceID());
        fileName = GameConstant.GameDataFileName;
        sheetName = Path.GetFileNameWithoutExtension(assetPath);
        
        ExportToggle.GoogleSheet = false;        
    }
    private void drawImportExportGUI()
    {
        GUIStyle btnStyle = new GUIStyle(GUI.skin.GetStyle("Button"))
        {
            fixedWidth = 100,
            alignment = TextAnchor.MiddleCenter
        };
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Import from: ", EditorStyles.label);
        drawBtn("Google Sheet", btnStyle, ImportToggle.GoogleSheet, () =>
        {
            // download excel from google sheet
            
            Action<string> callback = (string path) =>
            {
                // parse excel file
                ImportFromExcel(path);
            };

            WebTool.DownloadXLSX(GameConstant.GoogleSheetId, callback, fileName);
        });
        drawBtn("Excel", btnStyle, ImportToggle.Excel, () => {
            string path = EditorUtility.OpenFilePanel("Open Excel File", GameConstant.ExcelSourceFolder, "xlsx");
            if (path.Length != 0)
            {
                ImportFromExcel(path);
            }
        });
        drawBtn("Json", btnStyle, ImportToggle.Json, () =>
        {
            string path = EditorUtility.OpenFilePanel("Open Json File", GameConstant.JsonSourceFolder, "json");
            if (path.Length != 0)
            {
                ImportFromJson(path);
            }
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Export to: ", EditorStyles.label);
        drawBtn("Google Sheet", btnStyle, ExportToggle.GoogleSheet);
        drawBtn("Excel", btnStyle, ExportToggle.Excel, () => {
            var path = EditorUtility.SaveFilePanel("Save Excel File", GameConstant.ExcelSourceFolder, string.Format("{0}.{1}", fileName, "xlsx"), "xlsx");
            if (path.Length != 0)
            {
                ExportToExcel(path);
            }   
        });
        drawBtn("Json", btnStyle, ExportToggle.Json, () =>
        {
            var path = EditorUtility.SaveFilePanel("Save Json File", GameConstant.JsonSourceFolder, string.Format("{0}.{1}", fileName, "json"), "json");
            if (path.Length != 0)
            {
                ExportToJson(path);
            }
        });
        EditorGUILayout.EndHorizontal();
    }

    private void drawBtn(string text, GUIStyle btnStyle, bool active = true, Action onPressed = null)
    {
        if (active)
        {
            if (GUILayout.Button(text, btnStyle))
            {
                if(onPressed != null) onPressed();
            }
        }
        else
        {
            GUI.enabled = false;
            GUILayout.Button(text, btnStyle);
            GUI.enabled = true;
        }
    }

    public virtual void ImportFromExcel(string filePath, bool withHeader = true)
    {
        GameDataTableBase<T> table = target as GameDataTableBase<T>;
        
        FileStream fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        ExcelPackage ep = new ExcelPackage(fstream);

        ExcelWorksheet sheet = ep.Workbook.Worksheets[sheetName];

        T dataSample = new T();
        List<string> fieldNames = dataSample.GetFieldNames();

        int i = (withHeader) ? 1 : 0;
        int dataIndex = 0;
        while (true)
        {
            ExcelRange range = sheet.GetExcelRange(i, 0, i, fieldNames.Count - 1);
            if (range.Any(c => !string.IsNullOrEmpty(c.Text)) == false) //all empty row
            {
                break;
            }

            if (table.DataList.Count <= dataIndex) table.DataList.Add(new T());
            for (int j = 0; j < fieldNames.Count; j++)
            {
                string text = sheet.GetValueInCell(i, j);
                table.DataList[dataIndex].SetField(typeof(T).GetField(fieldNames[j]), text);
            }

            i++;
            dataIndex++;
        }

        fstream.Close();
        ep.Dispose();

        Debug.Log("Import from Excel Process Completed. Source: " + filePath);
    }
    public virtual void ExportToExcel(string filePath, bool withHeader = true)
    {
        GameDataTableBase<T> table = target as GameDataTableBase<T>;

        ExcelPackage ep;
        FileStream fstream;

        if (File.Exists(filePath))
        {
            fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            ep = new ExcelPackage(fstream);
            fstream.Close();
        }
        else ep = new ExcelPackage();

        T dataSample = new T();
        List<string> fieldNames = dataSample.GetFieldNames();

        if (ep.Workbook.Worksheets[sheetName] != null)
        {
            ep.Workbook.Worksheets.Delete(sheetName);
        }
        ep.Workbook.Worksheets.Add(sheetName);
        ExcelWorksheet sheet = ep.Workbook.Worksheets[sheetName];

        for (int i = 0; i < fieldNames.Count; i++)
        {
            sheet.SetValueInCell(0, i, fieldNames[i]);
        }
        for (int i = 0; i < table.DataList.Count; i++)
        {
            for (int j = 0; j < fieldNames.Count; j++)
            {
                string value = table.DataList[i].GetField(typeof(T).GetField(fieldNames[j]));
                sheet.SetValueInCell(i + 1, j, value);
            }
        }

        fstream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        ep.SaveAs(fstream);
        fstream.Close();
        ep.Dispose();

        Debug.Log("Export to Excel Process Completed. Destination: " + filePath);
    }

    public virtual void ImportFromJson(string filePath)
    {
        // read json strings from filePath
        StreamReader file = new StreamReader(filePath);
        string jsonString = file.ReadToEnd();
        file.Close();

        GameDataTableBase<T> table = target as GameDataTableBase<T>;
        JsonUtility.FromJsonOverwrite(jsonString, table);

        Debug.Log("Import from Json Process Completed. Source: " + filePath);
    }
    public virtual void ExportToJson(string filePath)
    {
        GameDataTableBase<T> table = target as GameDataTableBase<T>;
        string jsonString = JsonUtility.ToJson(table);

        System.IO.File.WriteAllText(filePath, jsonString);
        Debug.Log("Export to Json Process Completed. Destination: " + filePath);
    }
}
