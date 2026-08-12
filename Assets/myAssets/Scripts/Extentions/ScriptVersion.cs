#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ScriptVersion
{
    public static DefineScriptVerion scriptVersion = DefineScriptVerion.Custom;
    //#EFB1FF91
    //#B3ECFFA1

    public static void GetScriptsByVersion()
    {
        string[] monoScripts = AssetDatabase.FindAssets("t:Script");

        for (int i = 0; i < monoScripts.Length; i++)
        {
            string scriptGuid = monoScripts[i];

            string assetPath = AssetDatabase.GUIDToAssetPath(scriptGuid);

            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

            System.Type classType = monoScript.GetClass();

            if (classType == null || !classType.IsSubclassOf(typeof(Component)))
            {
                DebugTWD.Log("Assets folder not found.");
                continue;
            }

            List<DefineScriptVerion> v = monoScript.GetData<DefineScriptVerion>().ToList();
            string assemblyName = classType.Assembly.GetName().Name;
            string fullName = classType.FullName.Replace('.', '/');
            string className = classType.Name;
        }
    }
}
#endif
public enum DefineScriptVerion
{
    Origin, //не измененный
    Custom, //свой собственный
    Mod_621, //оригинальный, адаптированный к 6.21
    Mod_622, //оригинальный, адаптированный к 6.22
    Mod_624 //оригинальный, адаптированный к 6.24
}
