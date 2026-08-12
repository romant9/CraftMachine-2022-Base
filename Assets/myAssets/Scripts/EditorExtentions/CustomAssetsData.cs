#if UNITY_EDITOR
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomAssetsData", menuName = "Custom/Custom Assets Data")]
public class CustomAssetsData : ScriptableObject
{
	public const string externalFileName = "ExternalMapping.json";
	public const string baseFileName = "BaseMapping.json";
	public const string dllDataJsonName = "DllData.json";

	[Tooltip("Имя внешней папки, находящейся на одном уровне с проектом (напр. g:\\Unity Projects\\TWD\\EpicGames\\HotUpdateData\\json\\)")]
	public string jsonFolderPath;

	[Tooltip("Путь внешней папки с ассетами (напр. g:\\Unity Projects\\TWD\\EpicGames\\CM_win_2022.3_Base\\Assets\\)")]
	public string externalFolderPath;

	//[Tooltip("Сгенерированная карта ассетов из внешней папки")]
	public List<AssetMapping> customAssetsDataMapping { get; private set; } = new ();

	//[Tooltip("Карта ассетов текущего проекта Unity")]
	public List<AssetMapping> baseAssetsDataMapping { get; private set; } = new ();

	public Dictionary<string, Dictionary<long, ResourceItemNew>> DllDataIDDict { get; private set; } = new();

	/// <summary>
	/// Возвращает абсолютный путь к внешней папке на основе ее имени рядом с проектом.
	/// </summary>
	public string GetAbsoluteExternalPath()
	{
		return externalFolderPath;
		//if (string.IsNullOrEmpty(externalFolderPath)) return string.Empty;

		// Корневая папка проекта (там где Assets)
		//string projectRoot = Directory.GetParent(Application.dataPath).FullName;
		// Папка на один уровень выше корня проекта
		//string parentDirectory = Directory.GetParent(projectRoot).FullName;

		//return Path.Combine(parentDirectory, externalFolderPath).Replace("\\", "/");
	}

	public void LoadDllMapping()
	{
		LoadDllMappingInternal();
		EditorUtility.SetDirty(this);
	}

	public void LoadBaseMapping()
	{
		LoadBaseMappingInternal();
		EditorUtility.SetDirty(this);
	}

	public void LoadExternalMapping()
	{
		LoadExternalMappingInternal();
		EditorUtility.SetDirty(this);
	}

	public void RefreshBaseMapping()
	{
		RefreshBaseMappingInternal();
		EditorUtility.SetDirty(this);
	}

	public void RefreshExternalMapping()
	{
		RefreshExternalMappingInternal();
		EditorUtility.SetDirty(this);
	}

	private void LoadDllMappingInternal()
	{
		var dllDataPath = jsonFolderPath + dllDataJsonName;
		if (!File.Exists(dllDataPath))
		{
			Debug.LogWarning($"[CustomAssetsData] Файл '{baseFileName}' не найден. Выполните RefreshBaseMapping");
			return;
		}
		var dllDataJson = File.ReadAllText(dllDataPath);
		var dllData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ResourceItemNew>>>(dllDataJson);
		if (dllData != null)
		{
			DllDataIDDict = new();
			foreach (var dllDic in dllData)
			{
				var dllDicGuid = dllDic.Key;
				var dllDataDic = new Dictionary<long, ResourceItemNew>();

				foreach (KeyValuePair<string, ResourceItemNew> dllIdDic in dllDic.Value)
				{
					var dllDicKey = dllIdDic.Value.DllFileId;

					if (!dllDataDic.ContainsKey(dllDicKey)) dllDataDic.Add(dllDicKey, dllIdDic.Value);
				}
				DllDataIDDict.Add(dllDicGuid, dllDataDic);
			}
			Debug.Log($"[CustomAssetsData] DllDataIDDic загружена из файла '{dllDataJsonName}'");
		}
		else
		{
			Debug.LogWarning($"[CustomAssetsData] Ошибка десериализации DllDataIDDic из '{dllDataJsonName}'");
		}
	}

	private void LoadExternalMappingInternal()
	{
		var externalJsonData = jsonFolderPath + externalFileName;
		if (!File.Exists(externalJsonData))
		{
			Debug.LogWarning($"[CustomAssetsData] Файл '{externalFileName}' не найден. Выполните RefreshExternalMapping");
			return;
		}
		var fileContent = File.ReadAllText(externalJsonData);
		customAssetsDataMapping = JsonConvert.DeserializeObject<List<AssetMapping>>(fileContent);
		if (customAssetsDataMapping != null && customAssetsDataMapping.Count > 0)
		{
			Debug.Log($"[CustomAssetsData] customAssetsDataMapping загружена из файла '{externalFileName}'");
		}
		else
		{
			Debug.LogWarning($"[CustomAssetsData] Ошибка десериализации '{externalFileName}'. Выполните RefreshExternalMapping");
		}
	}

	private void LoadBaseMappingInternal()
	{
		var baseJsonData = jsonFolderPath + baseFileName;
		if (!File.Exists(baseJsonData))
		{
			Debug.LogWarning($"[CustomAssetsData] Файл '{baseFileName}' не найден. Выполните RefreshBaseMapping");
			return;
		}
		var fileContent = File.ReadAllText(baseJsonData);
		baseAssetsDataMapping = JsonConvert.DeserializeObject<List<AssetMapping>>(fileContent);
		if (baseAssetsDataMapping != null && baseAssetsDataMapping.Count > 0)
		{
			Debug.Log($"[CustomAssetsData] baseAssetsDataMapping загружена из файла '{baseFileName}'");
		}
		else
		{
			Debug.LogWarning($"[CustomAssetsData] Ошибка десериализации '{baseFileName}'. Выполните RefreshBaseMapping");
		}
	}

	private void RefreshExternalMappingInternal()
	{
		customAssetsDataMapping.Clear();
		string absoluteExternalPath = GetAbsoluteExternalPath();

		if (string.IsNullOrEmpty(absoluteExternalPath) || !Directory.Exists(absoluteExternalPath))
		{
			Debug.LogWarning($"[CustomAssetsData] Внешняя папка по пути '{absoluteExternalPath}' не найдена.");
			return;
		}

		string[] allFiles = Directory.GetFiles(absoluteExternalPath, "*.*", SearchOption.AllDirectories);

		foreach (string filePath in allFiles)
		{
			if (filePath.EndsWith(".meta")) continue;

			string metaPath = filePath + ".meta";
			if (File.Exists(metaPath))
			{
				string guid = ExtractGuidFromMeta(metaPath);
				if (!string.IsNullOrEmpty(guid))
				{
					// Сохраняем путь относительно самой внешней папки
					string relativeToExternal = Path.GetRelativePath(absoluteExternalPath, filePath);

					customAssetsDataMapping.Add(new AssetMapping
					{
						guid = guid,
						fileName = Path.GetFileName(filePath),
						externalRelativePath = relativeToExternal.Replace("\\", "/")
					});
				}
			}
		}
		var externalJsonDataPath = jsonFolderPath + externalFileName;
		var externalJsonDataSer = JsonConvert.SerializeObject(customAssetsDataMapping, Formatting.Indented);
		File.WriteAllText(externalJsonDataPath, externalJsonDataSer);
		Debug.Log($"[CustomAssetsData] Смаппировано внешних файлов: {customAssetsDataMapping.Count}");
	}

	private void RefreshBaseMappingInternal()
	{
		baseAssetsDataMapping.Clear();
		string[] allProjectFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);

		foreach (string filePath in allProjectFiles)
		{
			if (filePath.EndsWith(".meta")) continue;

			string relativePath = "Assets" + filePath.Substring(Application.dataPath.Length).Replace("\\", "/");
			string guid = AssetDatabase.AssetPathToGUID(relativePath);

			if (!string.IsNullOrEmpty(guid))
			{
				baseAssetsDataMapping.Add(new AssetMapping
				{
					guid = guid,
					fileName = Path.GetFileName(filePath),
					externalRelativePath = relativePath
				});
			}
		}

		var baseJsonDataPath = jsonFolderPath + baseFileName;
		var baseJsonDataSer = JsonConvert.SerializeObject(baseAssetsDataMapping, Formatting.Indented);
		File.WriteAllText(baseJsonDataPath, baseJsonDataSer);
		Debug.Log($"[CustomAssetsData] Смаппировано файлов проекта: {baseAssetsDataMapping.Count}");
	}

	private string ExtractGuidFromMeta(string metaPath)
	{
		try
		{
			string[] lines = File.ReadAllLines(metaPath);
			foreach (string line in lines)
			{
				if (line.Trim().StartsWith("guid:"))
				{
					string[] parts = line.Split(':');
					if (parts.Length > 1)
					{
						return parts[1].Trim();
					}
				}
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError($"Ошибка при чтении мета-файла {metaPath}: {e.Message}");
		}
		return null;
	}
}

[System.Serializable]
public struct AssetMapping
{
	public string guid;
	public string fileName;
	public string externalRelativePath; // Относительный путь
}
#endif