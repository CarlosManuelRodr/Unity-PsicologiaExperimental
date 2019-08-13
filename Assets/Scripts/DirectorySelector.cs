﻿using System;
using System.IO;
using UnityEngine;
using TMPro;
using SFB;

/// <summary>
/// Selector de directorio en Unity usando Windows Forms.
/// </summary>
public class DirectorySelector : MonoBehaviour
{
    public GameObject inputObject;
    private TMP_InputField inputField;
    private string defaultPath;

    public static string GetSaveDirectory()
    {
        string path;
        path = PlayerPrefs.GetString("Path", "");
        if (path == "")
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "HuertosLog");
        return path;
    }

    void Start()
    {
        inputField = inputObject.GetComponent<TMP_InputField>();
        defaultPath = PlayerPrefs.GetString("Path", "");
        if (defaultPath == "")
            defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "HuertosLog");
        inputField.text = defaultPath;
    }

    public void ChangeDirectory()
    {
        var path = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);
        if (path.Length != 0)
        {
            inputField.text = path[0];
            PlayerPrefs.SetString("Path", path[0]);
        }
    }
}
