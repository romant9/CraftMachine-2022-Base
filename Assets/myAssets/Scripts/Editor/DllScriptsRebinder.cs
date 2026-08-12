#if UNITY_EDITOR
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static DllScriptRebinder.BackupProject;
using Object = UnityEngine.Object;

public class DllScriptRebinder : MonoBehaviour
{
    public List<DllType> DllTypesList;

    [Header("Папка Assets, полный путь, по которому ищем сцены и префабы")]
    public string ProjectAssetsDir = @"e:\Unity Projects\TWD\Projects\Origin_7.2\Assets\";

    [Header("Папка для сохранения файла словаря")]
    public string ProjectBackupDir = @"e:\Unity Projects\TWD\Projects\Origin_7.2\bak\";

    //словари, листы, классы
    private string BackupJsonDir
    {
        get
        {
            string dir = ProjectBackupDir + @"JsonData\";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }
    }

    private string BackupDataDir
    {
        get
        {
            string dir = ProjectBackupDir + @"ScenePrefab\";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }
    }

    [Header("Для прямой замены *.asset из оригинальной выгрузки")]
    public string ProjectOriginAssetsDir = @"e:\Unity Projects\TWD\Projects\Origin_7.2_re\Assets\";

    [Header("Меняем guids dll->scripts или наоборот")]
    public bool IsConvertDllToScripts = true;

    [Header("Сохраняем файл восстановления .-backup.json")]
    public bool IsDoBackup = true;

    [Header("Физически копируем в /ScenePrefab все изменяемые ресурсы")]
    public bool IsDoBackupCopyFiles = false;

    [Header("Сохраняем словарь сопоставления dll и скриптов .-guides.json")]
    public bool IsSaveJsonDic = true;

    [Serializable]
    public class FileType
    {
        public bool IsActive;
        public string Name;

        public FileType(bool isActive, string name)
        {
            IsActive = isActive;
            Name = name;
        }
    }

    public List<FileType> FileTypesList = new List<FileType>()
    {
        new FileType(true,".prefab"),
        new FileType(true,".unity"),
        new FileType(true,".asset")
    };

    [Serializable]
    public class DllType
    {
        public string Name;
        public bool IsActive;
        public string Guid;
        public string AssetPath => !string.IsNullOrEmpty(Guid) ? AssetDatabase.GUIDToAssetPath(Guid) : "";

        public string ScriptsDir;

        public DllType(bool isActive, string guid, string scriptDir)
        {
            IsActive = isActive;
            Guid = guid;
            ScriptsDir = scriptDir;
        }
    }

    //public Dictionary<string, ResourceItem> dicFileIDs { get; set; }
    public BackupProject backupProject { get; set; }

    void Start()
    {
    }

    [ContextMenu("RestoreAssetsTemp")]
    public void RestoreAssetsTemp()
    {
        var filesToFix = Directory.GetFiles(ProjectAssetsDir, "*.asset", SearchOption.AllDirectories);

        var filesOrigin = Directory.GetFiles(ProjectOriginAssetsDir, "*.asset", SearchOption.AllDirectories);
        int count = 0;
        int error = 0;
        foreach (var file in filesOrigin)
        {
            var fixPath = file.Replace("Origin_7.2_re", "Origin_7.2");
            if (File.Exists(fixPath))
            {
                try
                {
                    File.Copy(file, fixPath, true);
                    count++;
                }
                catch (Exception ex)
                {
                    error++;
                    continue;
                }
            }
        }
        Debug.Log("Обработали " + count + "/" + filesToFix.Length + ". Ошибок: " + error);
    }

    public List<string> GetFileTypesList()
    {
        return FileTypesList.Where(x => x.IsActive).Select(x => x.Name).ToList();
    }

    [ContextMenu("Generate Dll-Scripts Dic")]
    public void GenerateDllScriptsDic()
    {
        int count = 0;
        // 1. Сохраняем FileID всех скриптов из DLL (NMLHotUpdate.dll)
        foreach (var dllFile in DllTypesList.Where(x => x.IsActive))
        {
            count++;
            string assetPath = AssetDatabase.GUIDToAssetPath(dllFile.Guid);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            var dicFileIDs = new Dictionary<string, ResourceItem>();

            foreach (Object asset in assets)
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long fileID))
                {
                    //Debug.Log("Guid: " + asset.name + ", fileID: " + fileID);
                    if (!dicFileIDs.ContainsKey(asset.name))
                    {
                        var item = new ResourceItem(dllFile.Guid) { Name = asset.name, DllFileId = fileID };
                        dicFileIDs.Add(asset.name, item);
                    }
                }
                else
                {
                    Debug.LogWarning("Could't get guid from " + asset);
                }
            }

            // 2. Добавляем все guid скриптов из отдельно взятой папки NMLHotUpdate (в другом проекте)
            var scriptsPaths = Directory.GetFiles(dllFile.ScriptsDir, "*.cs.meta", SearchOption.AllDirectories);

            foreach (var scriptsPath in scriptsPaths)
            {
                var csFileName = Path.GetFileNameWithoutExtension(scriptsPath).Replace(".cs", "");
                var csGuid = File.ReadAllLines(scriptsPath)[1].Split(' ')[1];

                if (dicFileIDs.TryGetValue(csFileName, out ResourceItem item))
                {
                    item.ScriptGuid = csGuid;
                }
            }

            if (IsSaveJsonDic && dicFileIDs.Count > 0)
            {
                // 3. Сохраняем словарь в guides.json
                var jsonString = JsonConvert.SerializeObject(dicFileIDs, Formatting.Indented);
                var assetName = assetPath.Split('/').Last().Split('.').First();
                var path = BackupJsonDir + '\\' + assetName + "-guides.json";
                File.WriteAllText(path, jsonString);
                Debug.Log("Сохранили " + path);
            }
            Debug.Log("Генерация " + count + " закончена, " + dicFileIDs.Count + " элементов");
        }
    }

    [ContextMenu("Replace Scripts Guides and FileID")]
    public void ChangeAssetGuides()
    {
        FileInfo[] jsonFiles = new DirectoryInfo(BackupJsonDir).GetFiles("*-guides.json");

        if (jsonFiles == null || jsonFiles.Length == 0) return;

        List<string> backupName = new List<string>();

        //string assetPath = AssetDatabase.GUIDToAssetPath(dllGuid);
        //var assetName = assetPath.Split('/').Last().Split('.').First();
        //var path = BackupJsonDir + '\\' + assetName + "-guides.json";

        // 4. Меняем во всех сценах и префабах ссылки к скриптам
        //var nameList = new[] { ".prefab", ".unity" };
        var set = new HashSet<string>(GetFileTypesList(), StringComparer.OrdinalIgnoreCase);
        var dir = new DirectoryInfo(ProjectAssetsDir);
        var files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(f => set.Contains(f.Extension)).ToList();

        int countGen = 0;
        int filesCount = 0;
        if (files != null && files.Count > 0)
        {
            filesCount = files.Count;

            if (IsDoBackup)
            {
                backupProject = new BackupProject(ProjectAssetsDir);
            }

            var dicJsonAll = new Dictionary<string, ResourceItem>();
            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    var jsonString = File.ReadAllText(jsonFile.FullName);
                    var dicFileIDs = JsonConvert.DeserializeObject<Dictionary<string, ResourceItem>>(jsonString);
                    var fileName = Path.GetFileNameWithoutExtension(jsonFile.FullName);
                    backupName.Add(fileName.Substring(0, fileName.Length >= 6 ? 6 : fileName.Length));
                    foreach (var dicItem in dicFileIDs)
                    {
                        if (!dicJsonAll.ContainsKey(dicItem.Key)) dicJsonAll.Add(dicItem.Key, dicItem.Value);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Нет элементов в словаре " + jsonFile.Name);
                    Debug.LogException(ex);
                }
            }

            if (dicJsonAll.Count == 0)
            {
                Debug.LogWarning("Нет элементов в словарях");
                return;
            }

            foreach (var file in files)
            {
                var content = File.ReadAllLines(file.FullName);
                int countLines = -1;
                int countRedact = 0;

                ProjectItem projectItem = new ProjectItem(file);
                foreach (var line in content)
                {
                    countLines++;
                    var trimLine = line.Trim();
                    if (trimLine.Length > 0 && trimLine.StartsWith("m_Script"))
                    {
                        var fields = line.Split(',');
                        var fileId = fields[0].Split(' ').Last();

                        string fileGuid = "";
                        try
                        {
                            fileGuid = fields[1].Split(' ').Last();
                        }
                        catch
                        {
                            continue;
                        }

                        string newLine;
                        ResourceItem item;

                        if (IsConvertDllToScripts)
                        {
                            item = dicJsonAll.Values.FirstOrDefault(x => x.DllFileId.ToString() == fileId);
                            if (item == null || fileId == item.ScriptGuid || string.IsNullOrEmpty(item.ScriptGuid)) continue;
                            newLine = line.Replace(fileId, item.ScriptFileId.ToString()).Replace(fileGuid, item.ScriptGuid);
                        }
                        else
                        {
                            item = dicJsonAll.Values.FirstOrDefault(x => x.ScriptGuid == fileGuid);
                            if (item == null || fileGuid == item.DllGuid) continue;
                            newLine = line.Replace(fileId, item.DllFileId.ToString()).Replace(fileGuid, item.DllGuid);
                        }

                        countRedact++;
                        if (IsDoBackup) projectItem.Content.Add(countLines, line + '|' + item.Name);
                        content[countLines] = newLine;
                        EditorUtility.DisplayProgressBar("Replace Guides", file.Name + " : " + countRedact, countGen / filesCount * 100);
                    }
                }
                if (countRedact > 0)
                {
                    if (IsDoBackup)
                    {
                        backupProject.ProjectItemsList.Add(projectItem);

                        if (IsDoBackupCopyFiles)
                        {
                            try
                            {
                                var index = file.FullName.IndexOf("\\Assets\\");
                                var backupPath = BackupDataDir + file.FullName.Substring(index);
                                if (!File.Exists(backupPath))
                                {
                                    var backupAbsDir = backupPath.Substring(0, backupPath.Length - file.Name.Length);
                                    if (!Directory.Exists(backupAbsDir))
                                    {
                                        Directory.CreateDirectory(backupAbsDir);
                                    }
                                    File.Copy(file.FullName, backupPath);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogException(ex);
                                continue;
                            }
                        }
                    }

                    countGen++;
                    File.WriteAllLines(file.FullName, content);
                }
            }
        }

        EditorUtility.ClearProgressBar();
        if (IsDoBackup && backupName.Count > 0)
        {
            if (backupProject.ProjectItemsList.Count > 0)
            {
                string jsonBackupProject = JsonConvert.SerializeObject(backupProject, Formatting.Indented);
                string jsonBackupFilePath = BackupJsonDir + '\\' + string.Join('_', backupName) + "-backup.json";
                File.WriteAllText(jsonBackupFilePath, jsonBackupProject);
            }
        }
        Debug.Log("Конветрация закончена, обработано " + countGen + "/" + filesCount + " файлов");
    }

    [ContextMenu("Restore Scripts Guides and FileID")]
    public void RestoreAssetGuides()
    {
        //string assetPath = AssetDatabase.GUIDToAssetPath(dllGuid);
        //var assetName = assetPath.Split('/').Last().Split('.').First();
        //string jsonBackupFilePath = BackupJsonDir + '\\' + assetName + "-backup.json";

        var backupFile = new DirectoryInfo(BackupJsonDir).GetFiles("*-backup.json").FirstOrDefault();

        if (backupFile == null || !backupFile.Exists)
        {
            Debug.LogWarning("Отсутствует файл восстановления " + backupFile.Name);
            return;
        }

        string content = File.ReadAllText(backupFile.FullName);
        try
        {
            backupProject = JsonConvert.DeserializeObject<BackupProject>(content);
        }
        catch
        {
            backupProject = null;
            return;
        }

        var projectItems = backupProject.ProjectItemsList;
        int count = -1;
        foreach (var item in projectItems)
        {
            EditorUtility.DisplayProgressBar("Replace Guides", item.Name + " : ", count / projectItems.Count * 100);
            if (item.Content.Count == 0) continue;
            var contentLines = File.ReadAllLines(item.Path);
            foreach (var line in item.Content)
            {
                contentLines[line.Key] = line.Value.Split('|').First();
            }
            File.WriteAllLines(backupFile.FullName, contentLines);
        }
        EditorUtility.ClearProgressBar();
    }

    string GetGUIDFromString(string content)
    {
        Regex guidRegex = new Regex(@"guid: ([0-9a-fA-F]{32})");
        Match match = guidRegex.Match(content);
        string missingGuid = match.Groups[1].Value;
        return missingGuid;
    }

    [Serializable]
    public class ResourceItem
    {
        public string Name { get; set; }
        public long ScriptFileId { get; set; }
        public long DllFileId { get; set; }
        public string ScriptGuid { get; set; }
        public string DllGuid { get; set; }

        public ResourceItem(string dllGuid)
        {
            Name = "";
            ScriptFileId = 11500000;
            DllFileId = -1;
            ScriptGuid = "";
            DllGuid = dllGuid;
        }
    }

    public class BackupProject
    {
        public string Name { get; set; }
        public string Dir { get; set; }
        public List<ProjectItem> ProjectItemsList { get; set; }

        public BackupProject(string dir)
        {
            Dir = dir;
            Name = new DirectoryInfo(dir).Parent.Name;
            ProjectItemsList = new List<ProjectItem>();
        }

        public class ProjectItem
        {
            public string Name { get; set; }
            public string Ext { get; set; }
            public string Path { get; set; }
            public Dictionary<int, string> Content { get; set; }

            public ProjectItem(FileInfo file)
            {
                Name = file.Name;
                Ext = file.Extension;
                Path = file.FullName;
                Content = new Dictionary<int, string>();
            }
        }
    }
}
#endif