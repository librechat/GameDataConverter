using UnityEngine;

public class GameConstant
{
    public static string ExcelSourceFolder
    {
        get
        {
            return Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets")) + excelFolderName;
        }
    }
    public static string JsonSourceFolder
    {
        get
        {
            return Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets")) + jsonFolderName;
        }
    }
    public static string GameDataFileName = "GameData";
    public static string GoogleSheetId = "put-google-sheet-id";

    private static string excelFolderName = "GameData/Excel";
    private static string jsonFolderName = "GameData/Json";
}
