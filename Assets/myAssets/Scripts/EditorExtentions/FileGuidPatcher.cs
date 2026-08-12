#if UNITY_EDITOR
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TWDModel;
using UnityEditor;
using UnityEngine;
using static Extention_Methods;
using Object = UnityEngine.Object;

public class FileGuidPatcher : MonoBehaviour
{
    //полный путь к папке, заканчивая "\"
    //public string objectToPatchPathFolder;
    //prefab без расширения
    //public string objectToPatchName;
    //альтернативно сам префаб
    //public GameObject GameObjectToPatch;
    //public Scene SceneToPatch;
    //t:scriptableobject - assets

    //префаб или сцена для изменения guid
    public Object ObjectToPatch;
    public string PathToChangeContent;

    //если включено, то ObjectToPatch будет выбранной группой 
    public bool IsUseSelectedObjects;

    //если включено, то ObjectToPatch будут файлы из директории 
    public bool IsUseObjectsByPath;

    //Only for downgrade version
    public bool fixCollider;
    public bool fixGuid;

    public bool StartProcessInPlayMode;

    public enum guidVersion
    {
        ver_614,
        ver_615,
        ver_616,
    }
    public guidVersion GuidVersion;

    public enum guidSource
    {
        File,
        Folder,
        All
    }

    public guidSource GuidSource;

    [Header("Replace Shaders")]

    public string AssetPath_Old;
    public string AssetPath_New;
    public string ResourcesPath_New;
    public string AtlasMapName = "Atlases_Map_Formatted_Full_24";
    public List<string> BlackListOld;
    public List<string> BlackListNew;

    public List<ResourceItem> ShaderList { get; set; }
    public List<ResourceItem> NGUIScriptList { get; set; }

    public bool FixOnlyMeta;

    [Header("Shader : Find Dependencies!")]
    public Shader ShaderToFindDependencies;
    public List<Material> MaterialsWithShader;
    public List<Material> MaterialsWithShaderName;

    [Header("Compare : Compare Resources")]
    public string AssetsPathProject1;
    public string AssetsPathProject2;
    public string ReplaceString1; //Origin_6.20
    public string ReplaceString2; //Origin_6.21
    public string ResourcesPath; //Resources
    public string JsonResourcesPath; //Resources/JSON/
    public string CompareJsonName; //CompareAssets_21-22

    private int ItemCounter;

    void Start()
    {
    }

    void Update()
    {
        if (StartProcessInPlayMode)
        {
            StartProcessInPlayMode = false;
        }
    }

    [ContextMenu("Generate Atlas Map File")]
    public void GenerateAtlasMapList()
    {
        //FileID 11400000
        var replaceTypes = new ItemReplaceType[6] { ItemReplaceType.Asset, ItemReplaceType.Prefab, ItemReplaceType.Font, ItemReplaceType.Script, ItemReplaceType.Image, ItemReplaceType.Material };
        var masks = new string[3] { "atlas_", "atlas_", "" };

        DirectoryInfo originDirectory = new DirectoryInfo(AssetPath_Old);
        ItemCounter = 0;
        List<ResourceItem> atlases_Old = GetShaderFiles(originDirectory, replaceTypes, BlackListOld);
        EditorUtility.DisplayProgressBar("", "Atlases_Old Generated with : " + atlases_Old.Count + " elements", .2f);
        if (atlases_Old.Count == 0) { DebugTWD.Log("atlases_Old no items"); return; }

        DirectoryInfo newDirectory = new DirectoryInfo(AssetPath_New);
        ItemCounter = 0;
        List<ResourceItem> atlases_New = GetShaderFiles(newDirectory, replaceTypes, BlackListNew);
        EditorUtility.DisplayProgressBar("", "Atlases_New Generated with : " + atlases_Old.Count + " elements", .4f);

        if (atlases_New.Count == 0) { DebugTWD.Log("atlases_New no items"); return; }

        var atlases_Collect = new List<ResourceItem>();
        int total = atlases_Old.Count;
        int added = 0;
        int progess = 0;
        foreach (var item in atlases_Old)
        {
            var itemNew = atlases_New.FirstOrDefault(x => x.Name_Old == item.Name_Old);// || x.Name_Old.Replace("_HD", "") == item.Name_Old);
            if (itemNew != null)
            {
                item.Name_New = itemNew.Name_Old;
                item.LocalPath_New = itemNew.LocalPath_Old;
                item.FileGuid_New = itemNew.FileGuid_Old;
                item.typeNew = itemNew.typeOld;

                if (!string.IsNullOrEmpty(itemNew.FileId_Old)) item.FileId_New = itemNew.FileId_Old;
                atlases_Collect.Add(item);
                added++;
            }
            progess++;
            EditorUtility.DisplayProgressBar("", "Добавлено : " + added + "  |  Прогресс : " + progess + "/" + total, ((float)progess) / total);
        }

        if (atlases_Collect.Count > 0)
        {
            //MessageSerializer messageSerializer = new MessageSerializer();
            //var shadersList = messageSerializer.Serialize(atlases_Collect);
            var shadersListConvert = JsonConvert.SerializeObject(atlases_Collect, Formatting.Indented);

            //File.WriteAllText(ResourcesPath_New + "/Atlases_Map.json", shadersList);
            File.WriteAllText(ResourcesPath_New + "/" + AtlasMapName + ".json", shadersListConvert);

            DebugTWD.Log("Atlases map wrote");
        }
        else
        {
            DebugTWD.Log("Atlases is null");
        }

        EditorUtility.ClearProgressBar();
    }

    

    [ContextMenu("Change Guides For Prefabs, Scenes")]
    public void ChangeGuidNew()
    {
        if (IsUseSelectedObjects)
        {
            Object[] objs = Selection.objects ?? null;

            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    ChangeGuidNew(objs[i]);
                }
            }
        }
        else if (IsUseObjectsByPath)
        {
            filesTemp = new();

            //@"!*.config, !*.js"
            //string mask = @"*.prefab, *.unity";
            var set = new HashSet<string>( new[] { ".prefab", ".unity" }, StringComparer.OrdinalIgnoreCase);
            var dir = new DirectoryInfo(PathToChangeContent);
            var files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(f => set.Contains(f.Extension)).ToList();
            filesTemp = files.Select(x => x.Name)?.ToList();
            if (files != null && files.Count > 0)
            {
                var count = files.Count;
                for (int i = 0; i < count; i++)
                {
                    ChangeGuidNew(null, files[i].FullName);
                }
            }
        }
        else
        {
            ChangeGuidNew(ObjectToPatch);
        }
    }

    public List<string> filesTemp;

    public void ChangeGuidNew(Object obj, string objPath = "")
    {
        PrefabReplaceType[] types = new PrefabReplaceType[4] { PrefabReplaceType.mAtlas, PrefabReplaceType.mTrueTypeFont, PrefabReplaceType.m_Script, PrefabReplaceType.mTexture };
        string path;
        if (!IsUseObjectsByPath && obj == null) return;
        if (IsUseObjectsByPath && !string.IsNullOrEmpty(objPath) && File.Exists(objPath))
        {
            path = objPath;
        }
        else
        {
            string ext = "";

            DebugTWD.Log("тип объекта : " + obj.GetType().ToString());

            if (obj.GetType() == typeof(GameObject)) ext = ".prefab";
            if (obj.GetType() == typeof(SceneAsset)) ext = ".unity";
            if (string.IsNullOrEmpty(ext))
            {
                DebugTWD.LogError("неверный тип объекта");
                return;
            }

            path = AssetDatabase.GetAssetOrScenePath(obj);
            DebugTWD.Log("path to objectToPatch : " + path);
            if (!File.Exists(path))
            {
                DebugTWD.LogError("не могу найти файл " + path);
                return;
            }
        }
        
        var file = File.ReadAllLines(path);
        List<ResourceItem> guidesList = null;
        if (fixGuid)
        {
            var guides = File.ReadAllText(ResourcesPath_New + "/" + AtlasMapName + ".json");
            guidesList = JsonConvert.DeserializeObject<List<ResourceItem>>(guides);
        }
        int count = 0;
        for (int i = 0; i < file.Length; i++)
        {
            if (fixGuid)
            {
                var str = file[i];

                var JsonSubstringOld = GetJsonSubstring(str, types);

                if (!string.IsNullOrEmpty(JsonSubstringOld))
                {
                    DebugTWD.Log(JsonSubstringOld);
                    List<string> stringItemFields = JsonSubstringOld.Split(", ").ToList();
                    if (stringItemFields.Count > 2)
                    {
                        var oldGuid = stringItemFields[1].Split(": ")[1];
                        var oldId = stringItemFields[0].Split(": ")[1];
                        var oldType = stringItemFields[2].Split(": ")[1];

                        var atlasItem = guidesList.FirstOrDefault(x => x.FileGuid_Old == oldGuid);
                        if (atlasItem != null)
                        {
                            str = str.Replace(oldGuid, atlasItem.FileGuid_New);
                            if (!string.IsNullOrEmpty(atlasItem.FileId_New))
                                str = str.Replace(oldId, atlasItem.FileId_New);

                            if (atlasItem.typeOld == ItemReplaceType.Prefab && atlasItem.typeNew == ItemReplaceType.Asset)
                            {
                                str = str.Replace("type: 3", "type: 2");
                            }

                            file[i] = str;
                            count++;
                        }
                    }
                }             
            }

            if (fixCollider)
            {
                if (file[i].Contains("BoxCollider:") && file[i + 1].Contains("serializedVersion: 3"))
                {
                    string str_after = file[i + 1];
                    str_after = str_after.Replace("serializedVersion: 3", "serializedVersion: 2");
                    file[i + 1] = str_after;
                    if (i + 1 < file.Length) i++;
                    count++;
                }
            }
        }
        if (count > 0) File.WriteAllLines(path, file);
        DebugTWD.Log("All Guides changed");
    }

    public string GetJsonSubstring(string original, PrefabReplaceType[] types)
    {
        for (int i = 0; i < types.Count(); i++)
        {
            if (original.Contains(types[i].ToString()))
            {
                int index = types[i].ToString().Length + 3;
                return original.Substring(index);
            }
        }
        
        return null;
    }

    [ContextMenu("Replace Shaders")]
    public void ReplaceShaders()
    {
        if (string.IsNullOrEmpty(AssetPath_Old) || string.IsNullOrEmpty(AssetPath_New)) { DebugTWD.Log("Пути не заданы"); return; }
        ShaderList = new List<ResourceItem>();

        DirectoryInfo oldDirectory = new DirectoryInfo(AssetPath_Old);
        List<ResourceItem> oldShaders = GetShaderFiles(oldDirectory, new ItemReplaceType[1] { ItemReplaceType.Shader });

        if (oldShaders.Count == 0) { DebugTWD.Log("shadersOrigin no items"); return; }

        DirectoryInfo newDirectory = new DirectoryInfo(AssetPath_New);
        List<ResourceItem> newShaders = GetShaderFiles(newDirectory, new ItemReplaceType[1] { ItemReplaceType.Shader });
        
        if (newShaders.Count == 0) { DebugTWD.Log("shadersBase no items"); return; }

        foreach (var oldShader in oldShaders)
        {
            foreach (var newShader in newShaders)
            {
                if (newShader.Name_Old.ToLower() == oldShader.Name_Old.ToLower())
                {
                    oldShader.LocalPath_New = newShader.LocalPath_Old;
                    oldShader.Name_New = newShader.Name_Old;
                }
            }
        }
        var list = oldShaders.Where(x => !string.IsNullOrEmpty(x.Name_New));
        if (list == null) { DebugTWD.Log("Шейдеры не найдены"); return; }
        ShaderList.AddRange(list.ToList());

        MessageSerializer messageSerializer = new MessageSerializer();

        var shadersList = messageSerializer.Serialize(ShaderList);
        File.WriteAllText(ResourcesPath_New + "\\ShaderList_21-24.json", shadersList);

        //Debug.Log(ShaderList.Count + '\n' +
        //    ShaderList.First().LocalPathOrigin + '\n' +
        //    ShaderList.First().NameOrigin + '\n' +
        //    ShaderList.First().LocalPathBase + '\n' +
        //    ShaderList.First().NameBase);

        //return;
        int originCount = 0;
        bool isDoAll = false;
        for (int i=0; i < ShaderList.Count; i++)
        {
            ResourceItem shaderItem = ShaderList[i];
            if (shaderItem.LocalPath_New != null)
            {
                string oldPath = shaderItem.LocalPath_New;
                string newPath = shaderItem.LocalPath_Old;

                string oldPathMeta = oldPath + ".meta";
                string newPathMeta = newPath + ".meta";

                if (FixOnlyMeta)
                {
                    if (!isDoAll)
                    {
                        if (!EditorUtility.DisplayDialog("", $"Меняем:\n{oldPathMeta}\n{newPathMeta}", "YES", "NO"))
                        {
                            DebugTWD.Log("Отмена");
                            return;
                        }
                        else
                        {
                            isDoAll = true;
                        }
                    }

                    if (File.Exists(oldPathMeta))
                    {
                        File.Copy(oldPathMeta, newPathMeta, true);
                    }
                }
                else
                {                  
                    if (File.Exists(oldPath))
                    {
                        File.Copy(oldPath, newPath, true);
                    }

                    if (File.Exists(oldPathMeta))
                    {
                        File.Copy(oldPathMeta, newPathMeta, true);
                    }
                }

                originCount++;
            }
        }
        DebugTWD.Log("Заменено " + originCount + " шейдеров из " + ShaderList.Count);
    }

    [ContextMenu("Replace NGUI scripts")]
    public void ReplaceScripts()
    {
        MessageSerializer messageSerializer = new MessageSerializer();
        string baseResourcesPathFull = ResourcesPath_New + "\\NGUIScriptList.json";
        if (!FixOnlyMeta)
        {
            //base - NGUI папка
            if (string.IsNullOrEmpty(AssetPath_Old) || string.IsNullOrEmpty(AssetPath_New)) { DebugTWD.Log("Пути не заданы"); return; }
            NGUIScriptList = new List<ResourceItem>();

            DirectoryInfo originDirectory = new DirectoryInfo(AssetPath_Old);
            List<ResourceItem> shadersOrigin = GetShaderFiles(originDirectory, new ItemReplaceType[1] { ItemReplaceType.Script });

            if (shadersOrigin.Count == 0) { DebugTWD.Log("shadersOrigin no items"); return; }

            DirectoryInfo baseDirectory = new DirectoryInfo(AssetPath_New);
            List<ResourceItem> shadersBase = GetShaderFiles(baseDirectory, new ItemReplaceType[1] { ItemReplaceType.Script });

            if (shadersBase.Count == 0) { DebugTWD.Log("shadersBase no items"); return; }

            foreach (var originShader in shadersOrigin)
            {
                foreach (var baseShader in shadersBase)
                {
                    if (baseShader.Name_Old.ToLower() == originShader.Name_Old.ToLower())
                    {
                        originShader.LocalPath_New = baseShader.LocalPath_Old;
                        originShader.Name_New = baseShader.Name_Old;
                    }
                }
            }
            var list = shadersOrigin.Where(x => !string.IsNullOrEmpty(x.Name_New));
            if (list == null) { DebugTWD.Log("Шейдеры не найдены"); return; }
            NGUIScriptList.AddRange(list.ToList());
       
            var nGUIScriptList = messageSerializer.Serialize(NGUIScriptList);
            File.WriteAllText(baseResourcesPathFull, nGUIScriptList);
        }
        else
        {
            if (!File.Exists(baseResourcesPathFull))
            {
                DebugTWD.LogError("Путь " + baseResourcesPathFull + " не существует");
                return;
            }

            var content = File.ReadAllText(baseResourcesPathFull);
            NGUIScriptList = messageSerializer.Deserialize<List<ResourceItem>>(content);
        }

        int originCount = 0;
        for (int i = 0; i < NGUIScriptList.Count; i++)
        {
            ResourceItem shaderItem = NGUIScriptList[i];
            if (shaderItem.LocalPath_New != null)
            {              
                if (!FixOnlyMeta)
                {
                    string basePath = shaderItem.LocalPath_New;
                    string originPath = shaderItem.LocalPath_Old;
                    File.Delete(originPath);
                    try
                    {
                        File.Copy(originPath + ".meta", basePath + ".meta", true);
                        File.Delete(originPath + ".meta");
                    }
                    catch
                    {
                        DebugTWD.Log("Файла " + originPath + ".meta" + " не существует");
                    }
                }
                else
                {
                    string baseMetaPath = shaderItem.LocalPath_Old.Replace("Modified", "Origin") + ".meta";
                    string originMetaPath = shaderItem.LocalPath_New + ".meta";

                    try
                    {
                        File.Copy(baseMetaPath, originMetaPath, true);
                    }
                    catch
                    {
                        DebugTWD.Log("Файла " + baseMetaPath + " не существует");
                    }
                }

                originCount++;
            }
        }
        if (!FixOnlyMeta)
            DebugTWD.Log("Удалено " + originCount + " скриптов из " + NGUIScriptList.Count);
        else
            DebugTWD.Log("Заменено " + originCount + " .meta для скриптов из " + NGUIScriptList.Count);
    }

    public string SetExtention(ItemReplaceType type)
    {
        switch (type)
        {
            case ItemReplaceType.Shader: return ".shader";
            case ItemReplaceType.Script: return ".cs";
            case ItemReplaceType.Image: return ".png";
            case ItemReplaceType.Asset: return ".asset";
            case ItemReplaceType.Prefab: return ".prefab";
            case ItemReplaceType.Font: return ".ttf";

            default: return null;
        }
    }

    public string SetFileID(ItemReplaceType type)
    {
        switch (type)
        {
            case ItemReplaceType.Shader: return "0";
            case ItemReplaceType.Script: return "11500000";
            case ItemReplaceType.Image: return "0";
            case ItemReplaceType.Asset: return "11400000";
            case ItemReplaceType.Font: return "12800000";

            default: return null;
        }
    }

    private List<ResourceItem> GetShaderFiles(DirectoryInfo root, ItemReplaceType[] type, List<string> blackList = null, string[] mask = null)
    {
        if (blackList != null)
        {
            for (int i = 0; i < blackList.Count; i++)
            {
                blackList[i] = blackList[i].ToLower().Replace("\\", "/").TrimEnd('/');
            }          
        }
        List<ResourceItem> files = new List<ResourceItem>();
        for (int i = 0; i < type.Count(); i++)
        {
            string ext = SetExtention(type[i]);
            //get the files in the current directory
            //var shaders = root.GetFiles("*"+ext,se).Where(x => x.Extension == ext && (blackList != null ? !blackList.Contains(x.FullName.ToLower().Replace("\\", "/").Substring(0, x.FullName.LastIndexOf('/'))) : true) 
            //&& (mask != null && !string.IsNullOrEmpty(mask[i]) ? x.Name.ToLower().Contains(mask[i]) : true));
            var shaders = root.GetFiles("*" + ext, SearchOption.AllDirectories).Where(x => (blackList == null || !blackList.Contains(x.FullName.ToLower().Replace("\\", "/").Substring(0, x.FullName.LastIndexOf('/')))));//
                       // && (mask == null || string.IsNullOrEmpty(mask[i]) || x.Name.ToLower().Contains(mask[i])));

            if (shaders != null && shaders.Count() > 0)
            {
                int countAll = shaders.Count();
                int count = 0;
                foreach (var s in shaders)
                {
                    count++;
                    EditorUtility.DisplayProgressBar("", $"Atlases of {type[i].ToString()} Generated with : {count}/{countAll} elements", (float)count/countAll);

                    var name = Path.GetFileNameWithoutExtension(s.FullName);
                    //var pathLocal = s.FullName.Substring(s.FullName.LastIndexOf("Assets")).Replace("Assets\\","");
                    var pathGlobal = s.FullName.Replace("\\","/");
                    ResourceItem item = new ResourceItem(name, pathGlobal);
                    string guid = "";
                    if (File.Exists(s.FullName + ".meta"))
                    {
                        guid = File.ReadAllLines(s.FullName + ".meta")[1].Split(":")[1].TrimStart(' ');
                    }
                    item.FileId_Old = SetFileID(type[i]);
                    item.FileGuid_Old = guid;
                    item.typeOld = type[i];

                    files.Add(item);
                }
            }

            // scan subdirectories
            //DirectoryInfo[] dirs = root.GetDirectories();
            //foreach (DirectoryInfo dir in dirs)
            //{
            //    var filesDeep = await GetShaderFiles(dir, type);
            //    files.AddRange(filesDeep);
            //}
        }
        return files;
    }

    [ContextMenu("Find Materials by Shader")]
    public void FindMaterialsByShader()
    {
        if (ShaderToFindDependencies == null) 
        {
            DebugTWD.LogWarning("ShaderToFindDependencies is null");
            return;
        }
        var MatlistGUID = AssetDatabase.FindAssets("t:Material");
        MaterialsWithShader = new List<Material>();
        MaterialsWithShaderName = new List<Material>();

        for (int i = 0; i < MatlistGUID.Length; i++)
        {
            string matGuid = MatlistGUID[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(matGuid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (mat.shader == ShaderToFindDependencies && !MaterialsWithShader.Contains(mat)) MaterialsWithShader.Add(mat);
            if (mat.shader.name.ToLower() == ShaderToFindDependencies.name.ToLower() && !MaterialsWithShaderName.Contains(mat)) MaterialsWithShaderName.Add(mat);
        }
    }

    [ContextMenu("Collect New Resources")]
    public void CollectNewResources()
    {
        MessageSerializer ser = new MessageSerializer();
        var path = JsonResourcesPath + "/" + CompareJsonName + "json";
        if (!File.Exists(path)) return;
        var jsonValue = File.ReadAllText(path);
        List<ResourceCompareItem> jsonItems = ser.Deserialize<List<ResourceCompareItem>>(jsonValue);
        int index = 0;
        int itemsCount = jsonItems.Count;
        int index1t = 0;
        foreach (var res in jsonItems)
        {
            if (Input.GetKey(KeyCode.Escape))// || index > 20)
            {
                EditorUtility.ClearProgressBar();
                break;
            }
            index++;
            index1t++;

            if (index1t > (float)index / 10f)
            {
                index1t = 0;
                var progress = (float)index / (float)itemsCount;
                EditorUtility.DisplayProgressBar("", $"Обрабатываем ресурсы 1: {index}/{itemsCount}", progress);
            }
            var path1 = GetSourcePath(res);
            var path2 = PathToSave(res);
            if (!Directory.Exists(path2)) Directory.CreateDirectory(Path.GetDirectoryName(path2));
            File.Copy(path1, path2, true);

            var meta1 = path1 + ".meta";
            if (File.Exists(meta1))
            {
                File.Copy(meta1, path2 + ".meta", true);
            }

            //try
            //{
            //    File.Copy(path1, path2, true);
            //}
            //catch (Exception ex)
            //{
            //    DebugTWD.Log(res.compareType + "\n" + path1 + "\n" + path2 + "\n");
            //}
            if (res.compareType == CompareType.NotEqual)
            {
                path1 = GetSourcePath(res, false);
                path2 = PathToSave(res, false);
                if (!Directory.Exists(path2)) Directory.CreateDirectory(Path.GetDirectoryName(path2));

                File.Copy(path1, path2, true);

                meta1 = path1 + ".meta";
                if (File.Exists(meta1))
                {
                    File.Copy(meta1, path2 + ".meta", true);
                }
            }
        }
        DebugTWD.Log("Copy Assets finished");

        EditorUtility.ClearProgressBar();
    }

    public string PathToSave(ResourceCompareItem item, bool isNotEqual1 = true)
    {
        var type = item.compareType;
        switch (type)
        {
            case CompareType.NotInPlace1:
                return Path.Combine(ResourcesPath, "Compare", "To1", LocalPath(item.LocalPath1).TrimStart('\\'));
            case CompareType.NotInPlace2:
                return Path.Combine(ResourcesPath, "Compare", "To2", LocalPath(item.LocalPath1).TrimStart('\\'));
            case CompareType.NotEqual:
                if (isNotEqual1)
                {
                    //string localPath = LocalPath(item.LocalPath1);
                    //if (localPath.StartsWith("\\")) localPath = localPath.TrimStart('\\');
                    var path = Path.Combine(ResourcesPath, "Compare", "NotEqual", "1", LocalPath(item.LocalPath1).TrimStart('\\'));
                    return path;                
                }
                else
                {
                    var path = Path.Combine(ResourcesPath, "Compare", "NotEqual", "2", LocalPath(item.LocalPath2).TrimStart('\\'));
                    return path;
                }
            default:
                return null;
        }
    }

    public string GetSourcePath(ResourceCompareItem item, bool isNotEqual1 = true)
    {
        var type = item.compareType;
        switch (type)
        {
            case CompareType.NotInPlace1:
                return item.LocalPath1;
            case CompareType.NotInPlace2:
                return item.LocalPath1;
            case CompareType.NotEqual:
                if (isNotEqual1)
                    return item.LocalPath1;
                else
                    return item.LocalPath2;
            default:
                return null;
        }
    }

    //Указываем путь до Assets для обоих проектов
    [ContextMenu("Save New Resources Json")]
    public void SaveNewResourcesJson()
    {
        var list = new List<ResourceCompareItem>();

        List<string> AllFilesProj1 = new List<string>();
        List<string> AllFilesProj2 = new List<string>();

        string[] fileProj1 = Directory.GetFiles(AssetsPathProject1, "*.*", SearchOption.AllDirectories);
        if (fileProj1 != null)
        {
            AllFilesProj1 = fileProj1.Where(x => !x.Contains(".meta")).ToList();
        }
        string[] fileProj2 = Directory.GetFiles(AssetsPathProject2, "*.*", SearchOption.AllDirectories);
        if (fileProj2 != null)
        {
            AllFilesProj2 = fileProj2.Where(x => !x.Contains(".meta")).ToList();
        }

        int count1 = AllFilesProj1.Count();
        int count2 = AllFilesProj2.Count();
        int index1 = 0;
        int index2 = 0;

        int index1t = 0;
        int index2t = 0;

        DebugTWD.Log($"Файлов1: {count1}, файлов2: {count2}");
        foreach (var filePath1 in AllFilesProj1)
        {
            if (Input.GetKey(KeyCode.Escape)) 
            {
                EditorUtility.ClearProgressBar();
                break;
            }
            index1++;
            index1t++;

            var filePath2 = filePath1.Replace(ReplaceString1, ReplaceString2);
            var file1 = new FileInfo(filePath1);

            if (index1t > (float)count1 / 100f) 
            {
                index1t = 0;
                var progress = (float)index1 / (float)count1;
                EditorUtility.DisplayProgressBar("", $"Обрабатываем ресурсы 1: {index1}/{count1}", progress);
            }

            if (AllFilesProj2.Contains(filePath2))
            {
                var file2 = new FileInfo(filePath2);
                if (file1.Length != file2.Length)
                {
                    var item = new ResourceCompareItem(file1.Name, filePath1, filePath2, CompareType.NotEqual);
                    list.Add(item);
                }
                else continue;
            }
            else
            {
                var item = new ResourceCompareItem(file1.Name, filePath1, filePath2, CompareType.NotInPlace2);
                list.Add(item);
            }
        }

        foreach (var filePath2 in AllFilesProj2)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                EditorUtility.ClearProgressBar();
                break;
            }
            index2++;
            index2t++;

            var filePath1 = filePath2.Replace(ReplaceString2, ReplaceString1);
            var file2 = new FileInfo(filePath2);

            if (index2t > (float)count2 / 100f)
            {
                index2t = 0;
                var progress = (float)index2 / (float)count2;
                EditorUtility.DisplayProgressBar("", $"Обрабатываем ресурсы 1: {index2}/{count2}", progress);
            }

            if (AllFilesProj1.Contains(filePath1))
            {
                continue;
            }
            else
            {
                var item = new ResourceCompareItem(file2.Name, filePath2, filePath1, CompareType.NotInPlace1);
                list.Add(item);
            }
        }

        MessageSerializer ser = new MessageSerializer();

        string json = ser.Serialize(list, true);

        File.WriteAllText(JsonResourcesPath + "/" + CompareJsonName + "json", json);
        DebugTWD.Log("CompareAssets finished");

        EditorUtility.ClearProgressBar();
        //var list10 = string.Join("\n", fileProj1.ToList().GetRange(0, 100));
        //DebugTWD.Log(list10);
    }

    public string LocalPath(string globalPath)
    {
        var asset = @"\Assets";
        if (globalPath.Contains(asset))
        {
            var index = globalPath.LastIndexOf(asset) + 7;
            var str = globalPath.Substring(index);
            return str;
        }
        return globalPath;
    }


    static List<string> fileTypes = new List<string>() { "prefab", "Model", "scriptableobject" };

    bool ChangeMetaFileGUID(ScriptableObject go)
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

    [ContextMenu("Fix UIUpdater")]
    public void FixUIUpdater()
    {
        var uiUpdaters = FindObjectsOfType<LocalizationUIUpdater>(true).Where(x => !string.IsNullOrEmpty(x.EnCustomText));
        if (uiUpdaters != null && uiUpdaters.Count() > 0)
        {
            foreach (var updater in uiUpdaters)
            {
                updater.IsCustomTranslate = true;
            }
        }
    }
}


public class PrefabStringItem
{
    public string fileID { get; set; }
    public string guid { get; set; }
    public string type { get; set; }

}


public class PrefabGuidReplacer : EditorWindow
{
    private string prefabPathInProject2 = "Assets/Prefabs/YourPrefab.prefab";
    private string prefabSavePathInProject1 = "Assets/UpdatedPrefabs/YourPrefab.prefab";

    // словарь GUID из проекта 2 и GUID из проекта 1
    private Dictionary<string, string> guidMap = new Dictionary<string, string>();

    [MenuItem("Tools/Replace GUIDs in Prefab")]
    public static void ShowWindow()
    {
        GetWindow<PrefabGuidReplacer>("GUID Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Настройка путей", EditorStyles.boldLabel);

        prefabPathInProject2 = EditorGUILayout.TextField("Путь префаба из проекта 2", prefabPathInProject2);
        prefabSavePathInProject1 = EditorGUILayout.TextField("Путь сохранения в проекте 1", prefabSavePathInProject1);

        if (GUILayout.Button("Загрузить GUIDs из файла (или вручную)"))
        {
            // Здесь можно реализовать чтение файла с GUIDs или оставить для ручного заполнения
            // Для примера можно вручную заполнить guidMap
            // guidMap["GUID_из_проекта2"] = "GUID_из_проекта1";
        }

        if (GUILayout.Button("Заменить GUIDs"))
        {
            ReplaceGuidsInPrefab();
        }
    }

    private void ReplaceGuidsInPrefab()
    {
        // Загрузка префаба из проекта 2
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPathInProject2);
        if (prefab == null)
        {
            Debug.LogError("Не удалось загрузить префаб по пути: " + prefabPathInProject2);
            return;
        }

        // Создаем временную копию префаба для изменений
        string tempPath = "Assets/TempPrefabToEdit.prefab";
        PrefabUtility.SaveAsPrefabAsset(prefab, tempPath);

        // Загружаем его обратно
        GameObject prefabInstance = AssetDatabase.LoadAssetAtPath<GameObject>(tempPath);

        // Обход всех компонентов и объектов внутри префаба
        foreach (var component in prefabInstance.GetComponentsInChildren<Component>(true))
        {
            SerializedObject so = new SerializedObject(component);
            SerializedProperty prop = so.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference)
                {
                    Object objRef = prop.objectReferenceValue;
                    if (objRef != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(objRef);
                        string guid = AssetDatabase.AssetPathToGUID(assetPath);

                        // Если GUID есть в словаре и отличается
                        if (guidMap.ContainsKey(guid))
                        {
                            string newGuid = guidMap[guid];
                            string newAssetPath = AssetDatabase.GUIDToAssetPath(newGuid);

                            Object newAsset = AssetDatabase.LoadAssetAtPath<Object>(newAssetPath);
                            if (newAsset != null)
                            {
                                prop.objectReferenceValue = newAsset;
                                Debug.Log($"Заменен ассет {assetPath} на {newAssetPath}");
                            }
                        }
                    }
                }
            }

            so.ApplyModifiedProperties();
        }

        // Сохраняем измененный префаб по новому пути
        string finalPath = prefabSavePathInProject1;
        PrefabUtility.SaveAsPrefabAsset(prefabInstance, finalPath);

        // Удаляем временный файл
        AssetDatabase.DeleteAsset(tempPath);

        Debug.Log("GUIDs в префабе успешно заменены и сохранены в " + finalPath);
    }
}
#endif
