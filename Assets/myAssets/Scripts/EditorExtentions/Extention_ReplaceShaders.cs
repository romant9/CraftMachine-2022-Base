using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Extention_Methods;

public class Extention_ReplaceShaders : Extention_ReplaceFiles
{
    //public string FileCompare_Json = "ShaderCompareList";
    //public string FileBackup_Json = "ShaderBackupList";
    //public string FileReplaced_Json = "ShaderReplacedList";
    private void Start()
    {      
    }

    [ContextMenu("1. Save Shader Compare File List")]
    public void SaveShaderFileList()
    {
        ItemReplaceTypes = new List<ItemReplaceType> { ItemReplaceType.Shader };
        shaderExtraCondition = true;
        base.SaveCompareFileList();
    }

    [ContextMenu("2. Backup Shader Files")]
    public void BackupShaderFiles()
    {
        base.BackupFiles();
    }

    [ContextMenu("3. Replace Shader Files")]
    public void ReplaceShaderFiles()
    {
        base.ReplaceFiles();
    }

    [ContextMenu("Restore Shader Files")]
    public void RestoreShaderFiles()
    {
        base.RestoreFiles();
    }

    [ContextMenu("Delete Shader Backup Files")]
    public void DeleteShaderBackupFiles()
    {
        base.DeleteBackupFiles();
    }
}
