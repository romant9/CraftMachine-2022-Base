#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Finds all gameobjects in the scene that say "Missing Prefab with GUID" and allows you 
/// to update their .meta files to have the GUID to match the one in the gameobject in the scene
/// </summary>
public class FixMissingGUIDReferences : EditorWindow
{
    #region Fields

    static List<string> fileTypes = new List<string>() { "prefab", "Model" };
    List<GameObject> gameObjectsWithMissingReferences = new List<GameObject>();
    Color defaultGuiColor;
    Color oliveGreenColor = new Color(0.6f, 0.8f, 0.6f);
    bool searched = false;
    
    // Init static fields/variables, events, and delegates for asset database reload
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitStatics()
    {
        fileTypes = new List<string>() { "prefab", "Model" };
    }

    #endregion

    #region Main

    [MenuItem("Tools/Repair Missing GUID References")]
    public static void ShowWindow()
    {
        GetWindow<FixMissingGUIDReferences>("Missing GUID References");
    }

    void OnGUI()
    {
        /* // A text description of the tool
        string description =
            "This tool finds all gameobjects in the active scene(s) that have missing prefab references "
            + "and allows you to fix the corresponding asset's meta to have the correct prefab reference " 
            + "(done per-object by clicking 'Change Meta File GUID').";
        
        // Set height to 0 so that the height is automatically calculated based on the text
        // Get the width of the window and subtract 20 to account for the 
        EditorGUILayout.LabelField(description, GUILayout.ExpandHeight(true), EditorStyles.label.wordWrap ? GUILayout.ExpandWidth(true) : GUILayout.Width(500));
        EditorGUILayout.Space(5); */

        defaultGuiColor = GUI.color;
        
        // STEP 1: Find all gameobjects with missing references
        GUI.color = oliveGreenColor;
        if (GUILayout.Button("Find Missing References"))
        {
            FindMissingReferences();
            searched = true;
        }
        GUI.color = defaultGuiColor;
        EditorGUILayout.Space(5);

        // STEP 2: Display all gameobjects with missing references
        foreach (GameObject go in gameObjectsWithMissingReferences)
        {
            EditorGUILayout.ObjectField(go, typeof(GameObject), true);
            if (GUILayout.Button("Change Meta File GUID"))
            {
                if(ChangeMetaFileGUID(go)) gameObjectsWithMissingReferences.Remove(go);
            }
        }
        
        // If there are no missing references, display a message
        if (gameObjectsWithMissingReferences.Count == 0 && searched)
        {
            EditorGUILayout.LabelField("No missing references found!");
        }
    }

    // STEP 1: Find all gameobjects in active scenes that have missing prefab references (prefab references)
    void FindMissingReferences()
    {
        gameObjectsWithMissingReferences.Clear();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name.ToLower().Contains("missing prefab with guid"))
            {
                gameObjectsWithMissingReferences.Add(go);
                continue;
            }
        }
    }

    // STEP 3: Find the prefab that the gameobject is referencing and replace its guid with the missing guid, if exists
    bool ChangeMetaFileGUID(GameObject go)
    {
        string log = "";
        string metaFilePath;
        string prefabPath;

        // 1. Removes string in parenthesis from go name - returns original name
        string origGoName = GetOriginalGameObjectName(go.name);

        // 2. Parse the guid from the broken gameobject name
        string missingGuid = GetGUIDFromString(go.name);
        log += "Missing GUID: " + missingGuid;

        // 3. Search asset database for GUID for any asset that matches the orig GO name (for types listed in fileTypes)
        string assetGuid = SearchForMatchingAsset(origGoName);

        if (assetGuid == null)
        {
            DebugTWD.LogError("No matching asset found for: " + origGoName + " with guid: " + missingGuid);
            return false;
        }

        log += "\nFound matching asset for: " + origGoName + " with guid: " + assetGuid;

        // 4. Get the asset path and meta from the prefab's guid reference (broken ref)
        // 4.1
        try { prefabPath = AssetDatabase.GUIDToAssetPath(assetGuid); }
        catch (Exception e)
        {
            DebugTWD.LogError(log + "\nError getting prefab path.\n" + e.Message);
            return false;
        }

        // 4.2: Set the meta file path
        metaFilePath = prefabPath + ".meta";

        // Successfully found the prefab and its meta file!!
        log += "\nFound asset!! Meta path: " + metaFilePath;
        DebugTWD.Log(log);

        // 5. Read the meta file and replace its guid with the missing guid
        if (File.Exists(metaFilePath))
        {
            // The raw text of the meta file
            string metaContents = File.ReadAllText(metaFilePath);
            
            // Get the guid from the meta file
            string currentGuid = GetGUIDFromString(metaContents);
            
            // Replace the guid in the meta file with the missing guid
            string newMetaContents = metaContents.Replace(currentGuid, missingGuid);
            
            // Save the changes to the meta file
            File.WriteAllText(metaFilePath, newMetaContents);

            // Refresh asset database
            AssetDatabase.Refresh();

            DebugTWD.Log("Updated GUID for: " + go.name);

            return true;
        }

        DebugTWD.LogError("Meta file not found for asset: " + origGoName + " in path " + metaFilePath +
                       "\nLog History:\n" + log);
        
        return false;
    }

    #endregion

    #region Helpers

    string GetOriginalGameObjectName(string name)
    {
        // Remove string in parenthesis from go name
        string cleanedName = null;

        int index = name.IndexOf(" (");

        // Get all text up to but not including the string that starts with " ("
        if (index != -1)
        {
            cleanedName = name.Substring(0, index);
        }
        else
        {
            DebugTWD.LogError("No parenthesis found in gameobject name: " + name);
        }

        return cleanedName;
    }

    string SearchForMatchingAsset(string objectName)
    {
        // Iterate through fileTypes and see if any of them match the objectName
        foreach (string fileType in fileTypes)
        {
            string[] guids = AssetDatabase.FindAssets(objectName + " t:" + fileType);

            if (guids.Length > 0)
            {
                return guids[0];
            }
        }

        return null;
    }

    string GetGUIDFromString(string content)
    {
        // get the guid from the GO name with the missing reference
        Regex guidRegex = new Regex(@"guid: ([0-9a-fA-F]{32})");
        Match match = guidRegex.Match(content);
        string missingGuid = match.Groups[1].Value;
        return missingGuid;
    }

    #endregion
}
#endif

/*

    void DisplayMissingReferences()
    {
        EditorGUILayout.LabelField("Gameobjects with missing references: " + gameObjectsWithMissingReferences.Count);

        foreach (GameObject go in gameObjectsWithMissingReferences)
        {
            EditorGUILayout.ObjectField(go, typeof(GameObject), true);
        }
    }

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                continue;

            string assetPath = AssetDatabase.GetAssetPath(go.transform.root.gameObject);
            if (!string.IsNullOrEmpty(assetPath))
                continue;

            allGameObjects.Add(go);
        }
        */