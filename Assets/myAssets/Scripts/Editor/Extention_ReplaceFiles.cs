using System.IO;
using System.Linq;
using TWDModel;
using System.Collections.Generic;
using UnityEngine;
using static Extention_Methods;

public class Extention_ReplaceFiles : MonoBehaviour
{
    [Header("Replace Files")]
    public string AssetPath_Old; //оставляем '/' на конце строки пути
    public string AssetPath_New;
    public string Path_JSON;
    public string FileCompare_Json = "FileCompareList";
    public string FileBackup_Json = "FileBackupList";
    public string FileReplaced_Json = "FileReplacedList";

    public List<ResourceItem> FileCompareList { get; set; }
    //замещать в новом проекте только meta
    public bool IsReplaceOnlyMeta;
    //сохранять старые meta в новом проекте
    public bool IsSaveNewMeta;
    //в новом проект создать структуру папок, как в старом проекте 
    public bool IsSaveOldFileStructure;
    //в новый проект копировать все файлы из старого проекта, в том числе те, которых нет в новом проекте
    public bool IsCopyAllOldFiles;

    public List<ItemReplaceType> ItemReplaceTypes; //ItemReplaceType.Script

    public bool shaderExtraCondition = false;

    private MessageSerializer jsonSerializer;

    public MessageSerializer messageSerializer
    {
        get
        {
            if (jsonSerializer == null)
            {
                jsonSerializer = new MessageSerializer();
            }
            return jsonSerializer;
        }
    }

    //[ContextMenu("1. Save Compare File List")]
    public virtual void SaveCompareFileList()
    {
        FileCompareList = null;
        string path_fileCompareList = Path_JSON + FileCompare_Json + ".json";
        string path_newFilesBackupList = Path_JSON + FileBackup_Json + ".json";

        //AssetPath_Old - NGUI папка
        if (string.IsNullOrEmpty(AssetPath_Old) || string.IsNullOrEmpty(AssetPath_New)) { DebugTWD.Log("Пути не заданы"); return; }

        DirectoryInfo directory_old = new DirectoryInfo(AssetPath_Old);
        var files_old = new List<ResourceItem>();
        GetFiles(directory_old, ref files_old, ItemReplaceTypes);
        if (files_old.Count == 0) { DebugTWD.Log("Old files has no items"); return; }
        DebugTWD.Log("Старых файлов: " + files_old.Count);

        DirectoryInfo directory_new = new DirectoryInfo(AssetPath_New);
        var files_new = new List<ResourceItem>();
        GetFiles(directory_new, ref files_new, ItemReplaceTypes);
        if (files_new.Count == 0) { DebugTWD.Log("New files has no items"); return; }
        DebugTWD.Log("Новых файлов: " + files_new.Count);

        var file_to_save = new List<ResourceItem>();
        var file_to_backup = new List<ResourceItem>();

        foreach (var file_old in files_old)
        {
            if (shaderExtraCondition)
            {
                var content = File.ReadAllLines(file_old.LocalPath_Old);
                if (content != null && content.FirstOrDefault(x=>x.Contains("DummyShaderTextExporter")) != null)
                {
                    continue;
                }
            }
            bool isFindCompare = false;
            foreach (var file_new in files_new)
            {
                if (file_old.Name_Old.ToLower() == file_new.Name_Old.ToLower())
                {
                    var item = new ResourceItem(file_old);
                    item.LocalPath_New = file_new.LocalPath_Old;
                    file_to_save.Add(item);
                    file_to_backup.Add(file_new);
                    isFindCompare = true;
                }
            }

            if (!isFindCompare && IsCopyAllOldFiles)
            {
                var index_Old = file_old.LocalPath_Old.IndexOf("Assets") + 7;
                string pathDiff = file_old.LocalPath_Old.Substring(index_Old);
                var index_New = AssetPath_New.IndexOf("Assets") + 7;
                string newPathAsset = AssetPath_New.Substring(0, index_New);
                file_old.LocalPath_New = Path.Combine(newPathAsset, pathDiff).Replace("\\", "/");
                file_old.IsAbsentInNew = true;
                file_to_save.Add(file_old);
            }
        }
        FileCompareList = file_to_save.Where(x => !string.IsNullOrEmpty(x.LocalPath_New))?.ToList();
        if (FileCompareList == null) { DebugTWD.Log("Файлы для копирования не определены"); return; }

        DebugTWD.Log("Записываем карту файлов " + FileCompareList.Count + " из " + file_to_save.Count);
        var fileCompareListSer = messageSerializer.Serialize(FileCompareList, true);
        File.WriteAllText(path_fileCompareList, fileCompareListSer);

        var newFilesBackupSer = messageSerializer.Serialize(file_to_backup, true);
        File.WriteAllText(path_newFilesBackupList, newFilesBackupSer);
    }

    //[ContextMenu("2. Backup Files")]
    public virtual void BackupFiles()
    {
        string path_newFilesBackupList = Path_JSON + FileBackup_Json + ".json";
        if (!File.Exists(path_newFilesBackupList))
        {
            DebugTWD.LogError("Путь " + path_newFilesBackupList + " не существует");
            return;
        }
        var newFilesBackupStr = File.ReadAllText(path_newFilesBackupList);
        var newFilesBackupList = messageSerializer.Deserialize<List<ResourceItem>>(newFilesBackupStr);
        string path_BackupDir = Path_JSON + "Resources_Backup";
        if (!Directory.Exists(path_BackupDir)) Directory.CreateDirectory(path_BackupDir);

        var count = 0;
        foreach (var file in newFilesBackupList)
        {
            string pathDiff = file.LocalPath_Old.Substring(file.LocalPath_Old.IndexOf("Assets") + 7);
            string path_file_Backup = path_BackupDir.Replace("\\", "/") + "/" + pathDiff;
            string dirNew = path_file_Backup.Substring(0, path_file_Backup.LastIndexOf("/"));

            if (!Directory.Exists(dirNew)) Directory.CreateDirectory(dirNew);

            File.Copy(file.LocalPath_Old, path_file_Backup, true);
            count++;
            if (File.Exists(file.LocalPath_Old + ".meta"))
            {
                File.Copy(file.LocalPath_Old + ".meta", path_file_Backup + ".meta", true);
                count++;
            }
        }
        DebugTWD.Log("Backup of " + count + " files is complete");
    }

    //[ContextMenu("3. Replace Files")]
    public void ReplaceFiles()
    {
        var replacedFiles = new List<ResourceItem>();

        string path_NGUIScriptList = Path_JSON + FileCompare_Json + ".json";
        string path_replacedList = Path_JSON + FileReplaced_Json + ".json";

        if (!File.Exists(path_NGUIScriptList))
        {
            DebugTWD.LogError("Путь " + path_NGUIScriptList + " не существует");
            return;
        }
        var NGUIScriptListSer = File.ReadAllText(path_NGUIScriptList);
        FileCompareList = messageSerializer.Deserialize<List<ResourceItem>>(NGUIScriptListSer);

        int originCount = 0;
        int metaCount = 0;
        for (int i = 0; i < FileCompareList.Count; i++)
        {
            ResourceItem shaderItem = FileCompareList[i];
            if (shaderItem.LocalPath_New != null)
            {
                string path_new = shaderItem.LocalPath_New;
                string path_old = shaderItem.LocalPath_Old;
                string dirNew;

                if (shaderItem.IsAbsentInNew && !IsReplaceOnlyMeta)
                {
                    dirNew = path_new.Substring(0, path_new.LastIndexOf("/"));
                    if (!Directory.Exists(dirNew))
                    {
                        Directory.CreateDirectory(dirNew);
                    }
                    File.Copy(path_old, path_new, true);
                    originCount++;
                    replacedFiles.Add(new ResourceItem(shaderItem.Name_Old, path_new));
                    if (File.Exists(path_old + ".meta"))
                    {
                        File.Copy(path_old + ".meta", path_new + ".meta", true);
                        metaCount++;
                        replacedFiles.Add(new ResourceItem(shaderItem.Name_Old + ".meta", path_new + ".meta"));
                    }
                }
                else
                {
                    string path_meta = path_new + ".meta";

                    //if (File.Exists(path_new) && !IsReplaceOnlyMeta)
                    //{
                    //    File.Delete(path_new);
                    //}
                    //if (File.Exists(path_meta) && !IsSaveNewMeta)
                    //{
                    //    File.Delete(path_meta);
                    //}

                    if (IsSaveOldFileStructure)
                    {
                        string pathDiff = path_old.Substring(path_old.IndexOf("Assets") + 7);
                        string newPathAsset = AssetPath_New.Substring(0, AssetPath_New.IndexOf("Assets") + 7);
                        path_new = newPathAsset.Replace("\\", "/") + "/" + pathDiff;

                        dirNew = path_new.Substring(0, path_new.LastIndexOf("/"));
                        if (!Directory.Exists(dirNew))
                        {
                            Directory.CreateDirectory(dirNew);
                        }
                    }

                    if (!IsReplaceOnlyMeta)
                    {
                        File.Copy(path_old, path_new, true);
                        originCount++;
                        replacedFiles.Add(new ResourceItem(shaderItem.Name_Old, path_new));
                    }
                    if (File.Exists(path_old + ".meta"))
                    {
                        if (IsSaveNewMeta)
                        {
                            if (File.Exists(path_meta))
                            {
                                string path_meta_new = path_new + ".meta";
                                if (path_meta != path_meta_new)
                                {
                                    File.Copy(path_meta, path_meta_new, true);
                                    metaCount++;
                                    replacedFiles.Add(new ResourceItem(shaderItem.Name_Old + ".meta", path_meta_new));
                                    File.Delete(path_meta);
                                }
                            }
                        }
                        else
                        {
                            File.Copy(path_old + ".meta", path_new + ".meta", true);
                            metaCount++;
                            replacedFiles.Add(new ResourceItem(shaderItem.Name_Old + ".meta", path_new + ".meta"));
                        }
                    }
                }
            }
        }
        DebugTWD.Log("Заменено " + originCount + " скриптов и " + metaCount + " .meta из " + FileCompareList.Count);

        var replacedFilesSer = messageSerializer.Serialize(replacedFiles, true);
        File.WriteAllText(path_replacedList, replacedFilesSer);
    }

    //[ContextMenu("Restore Files")]
    public void RestoreFiles()
    {
        string path_newFilesBackupList = Path_JSON + FileBackup_Json + ".json";
        if (!File.Exists(path_newFilesBackupList))
        {
            DebugTWD.LogError("Путь " + path_newFilesBackupList + " не существует");
            return;
        }
        var newFilesBackupStr = File.ReadAllText(path_newFilesBackupList);
        var newFilesBackupList = messageSerializer.Deserialize<List<ResourceItem>>(newFilesBackupStr);

        string path_replacedList = Path_JSON + FileReplaced_Json + ".json";
        if (!File.Exists(path_replacedList))
        {
            DebugTWD.LogError("Путь " + path_replacedList + " не существует");
            return;
        }
        var fileReplacedStr = File.ReadAllText(path_replacedList);
        var fileReplacedList = messageSerializer.Deserialize<List<ResourceItem>>(fileReplacedStr);

        int deleteCount = 0;
        for (int i = 0; i < fileReplacedList.Count; i++)
        {
            ResourceItem shaderItem = fileReplacedList[i];
            if (File.Exists(shaderItem.LocalPath_Old))
            {
                File.Delete(shaderItem.LocalPath_Old);
                deleteCount++;
            }
        }

        int restoreCount = 0;
        string path_BackupDir = Path_JSON + "Resources_Backup";

        for (int i = 0; i < newFilesBackupList.Count; i++)
        {
            ResourceItem file = newFilesBackupList[i];

            string pathDiff = file.LocalPath_Old.Substring(file.LocalPath_Old.IndexOf("Assets") + 7);
            string path_file_Backup = path_BackupDir.Replace("\\", "/") + "/" + pathDiff;

            if (File.Exists(path_file_Backup))
            {
                File.Copy(path_file_Backup, file.LocalPath_Old, true);
                restoreCount++;
                if (File.Exists(path_file_Backup + ".meta"))
                {
                    File.Copy(path_file_Backup + ".meta", file.LocalPath_Old + ".meta", true);
                    restoreCount++;
                }
            }
        }

        DebugTWD.Log("Удалено " + deleteCount + " новых файлов из " + fileReplacedList.Count);
        DebugTWD.Log("Восстановлено " + restoreCount + " исходных файлов из " + newFilesBackupList.Count);
    }

    //[ContextMenu("Delete Backup Files")]
    public void DeleteBackupFiles()
    {
        string path_newFilesBackupList = Path_JSON + FileBackup_Json + ".json";
        if (!File.Exists(path_newFilesBackupList))
        {
            DebugTWD.LogError("Путь " + path_newFilesBackupList + " не существует");
            return;
        }
        var newFilesBackupStr = File.ReadAllText(path_newFilesBackupList);
        var newFilesBackupList = messageSerializer.Deserialize<List<ResourceItem>>(newFilesBackupStr);

        foreach (var file in newFilesBackupList)
        {
            if (File.Exists(file.LocalPath_Old))
            {
                File.Delete(file.LocalPath_Old);
            }
            if (File.Exists(file.LocalPath_Old + ".meta"))
            {
                File.Delete(file.LocalPath_Old + ".meta");
            }
        }

        string path_BackupDir = Path_JSON + "Resources_Backup";

        RemoveEmptyDirectory(path_BackupDir);
    }

    private static void RemoveEmptyDirectory(string startLocation)
    {
        foreach (var directory in Directory.GetDirectories(startLocation))
        {
            RemoveEmptyDirectory(directory);
            if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
            {
                Directory.Delete(directory, false);
            }
        }
    }
}
