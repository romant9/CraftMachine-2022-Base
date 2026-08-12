using System.Collections.Generic;
using UnityEngine;
using static Extention_Methods;

public class Extention_ReplaceNGUI : Extention_ReplaceFiles
{
    //public string FileCompare_Json = "NGUIScriptCompareList";
    //public string FileBackup_Json = "NGUIScriptBackupList";
    //public string FileReplaced_Json = "NGUIScriptReplacedList";
    private void Start()
    {
    }

    [ContextMenu("1. Save NGUI Compare File List")]
    public void SaveNGUIFileList()
    {
        ItemReplaceTypes = new List<ItemReplaceType> { ItemReplaceType.Script };
        base.SaveCompareFileList();
    }

    [ContextMenu("2. Backup NGUI Files")]
    public void BackupNGUIFiles()
    {
        base.BackupFiles();
    }

    [ContextMenu("3. Replace NGUI Files")]
    public void ReplaceNGUIFiles()
    {
        base.ReplaceFiles();
    }

    [ContextMenu("Restore NGUI Files")]
    public void RestoreNGUIFiles()
    {
        base.RestoreFiles();       
    }

    [ContextMenu("Delete NGUI Backup Files")]
    public void DeleteNGUIBackupFiles()
    {
        base.DeleteBackupFiles();       
    }
}
