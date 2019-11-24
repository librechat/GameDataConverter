using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class WebTool
{
    public static void DownloadXLSX(string docId, Action<string> callback, string fileName = "tmpXLSX")
    {
        string url =
            "https://docs.google.com/spreadsheets/d/" + docId + "/export?format=xlsx";
        UnityWebRequest www = UnityWebRequest.Get(url);

        string savePath = string.Format("{0}/{1}.xlsx", GameConstant.ExcelSourceFolder, fileName);
        var dh = new DownloadHandlerFile(savePath);
        dh.removeFileOnAbort = true;
        www.downloadHandler = dh;

        UnityWebRequestAsyncOperation asyncOperation = www.SendWebRequest();
        asyncOperation.completed += (async) =>
        {
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Download success! File at:" + savePath);
                dh.Dispose();
                callback(savePath);
                www.Dispose();
            }
        };
    }

    private static void End_CreateFileCallBack(IAsyncResult result)
    {
        var stream = result.AsyncState as FileStream;
        stream.EndWrite(result);
        stream.Close();
    }

}

