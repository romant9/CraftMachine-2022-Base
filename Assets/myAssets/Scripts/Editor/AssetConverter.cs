#if UNITY_EDITOR
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetTools
{
	public class AssetConverter : MonoBehaviour
	{
		public List<DllType> DllTypesList;

		//Assets_New\
		public string DirToCopyNewAssets;

		//d:\Unity Projects\CM_win_2022_Base\Assets\
		public string PathToAssets_Base;

		//e:\Unity Projects\TWD\Projects\Origin_7.13\Assets\
		public string PathToAssets_Origin;
		//e:\Unity Projects\TWD\Projects\Origin_7.13\Assets\Scripts\NMLHotUpdate\
		public string PathToAssetsNew_Origin;

		//e:\Unity Projects\TWD\Projects\Origin_7.13\Assets\..
		public List<string> DirsToParseAssets_Base = new List<string>() { "Scripts\\", "Resources\\", "Shader\\", "AmplifyColor\\", "myAssets\\", "NGUI\\",
	"Font\\", "MonoBehaviour\\", "Texture2D\\"};

		//e:\Unity Projects\TWD\Projects\HotUpdateData\ConvertData\
		public string PathToSaveJsonData;

		public string PathToParseSingleAsset;
		public string PathToParseAssetsFolder;

		// парсить только .assets и .prefab атласов
		public bool DoOnlyAtlases;
		public bool DoSaveExtJson = true;
		public bool DoAtlasesBackup = true;

		private readonly string dllDataJsonName = "DllData.json";
		private readonly string AssetsJsonOriginName = "OriginAssetsData.json";
		private readonly string AssetsJsonBaseName = "BaseAssetsData.json";

		private int countFilesConvert = 0;
		private int countFiles = 0;

		public List<string> FixLog { get; private set; }

		// m_Script: {fileID: 11500000
		// material: {fileID: 2100000

		private Dictionary<string, Dictionary<long, ResourceItem>> DllDataIDDic
		{
			get
			{
				if (dllDataIDDic == null)
				{
					dllDataIDDic = new();
					foreach (var dllDic in DllData)
					{
						var dllDicGuid = dllDic.Key;
						var dllDataDic = new Dictionary<long, ResourceItem>();

						foreach (KeyValuePair<string, ResourceItem> dllIdDic in dllDic.Value)
						{
							var dllDicKey = dllIdDic.Value.DllFileId;

							if (!dllDataDic.ContainsKey(dllDicKey)) dllDataDic.Add(dllDicKey, dllIdDic.Value);
						}
						dllDataIDDic.Add(dllDicGuid, dllDataDic);
					}
				}
				return dllDataIDDic;
			}
			set { dllDataIDDic = value; }
		}

		private Dictionary<string, Dictionary<string, ResourceItem>> DllData
		{
			get
			{
				if (dllData == null)
				{
					if (!File.Exists(PathToSaveJsonData + dllDataJsonName)) return null;
					var dllDataJson = File.ReadAllText(PathToSaveJsonData + dllDataJsonName);

					dllData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ResourceItem>>>(dllDataJson);
				}
				return dllData;
			}
			set { dllData = value; }
		}
		private Dictionary<string, Dictionary<string, ResourceItem>> AssetsOriginData
		{
			get
			{
				if (assetsOriginData == null)
				{
					if (!File.Exists(PathToSaveJsonData + AssetsJsonOriginName)) return null;
					var assetDataJson = File.ReadAllText(PathToSaveJsonData + AssetsJsonOriginName);

					assetsOriginData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ResourceItem>>>(assetDataJson);
				}
				return assetsOriginData;
			}
			set { assetsOriginData = value; }
		}

		private Dictionary<string, Dictionary<string, ResourceItem>> AssetsBaseData
		{
			get
			{
				if (assetsBaseData == null)
				{
					if (!File.Exists(PathToSaveJsonData + AssetsJsonBaseName)) return null;
					var assetDataJson = File.ReadAllText(PathToSaveJsonData + AssetsJsonBaseName);

					assetsBaseData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ResourceItem>>>(assetDataJson);
				}
				return assetsBaseData;
			}
			set { assetsBaseData = value; }
		}

		private Dictionary<string, Dictionary<string, ResourceItem>> dllData; // NMLHotUpdate, NGUI
		private Dictionary<string, Dictionary<string, ResourceItem>> assetsOriginData; // Origin_7.13
		private Dictionary<string, Dictionary<string, ResourceItem>> assetsBaseData; // CM_win_2022_Base
		private Dictionary<string, Dictionary<long, ResourceItem>> dllDataIDDic;

		private void Start()
		{
			
		}

		[ContextMenu("1. Generate Dll-Scripts Dic")]
		public void GenerateDllDatasDic()
		{
			var dicsFileIDs = new Dictionary<string, Dictionary<string, ResourceItem>>();

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
						if (!dicFileIDs.ContainsKey(asset.name))
						{
							var item = new ResourceItem(dllFile.Guid, asset.name, fileID);
							dicFileIDs.Add(asset.name, item);
						}
					}
					else
					{
						Debug.LogWarning("Could't get guid from " + asset);
					}
				}

				dicsFileIDs.Add(dllFile.Guid, dicFileIDs);
			}

			var dicsFileIDsJson = JsonConvert.SerializeObject(dicsFileIDs, Formatting.Indented);
			File.WriteAllText(PathToSaveJsonData + dllDataJsonName, dicsFileIDsJson);
			Debug.LogWarning("Сохранили файл " + PathToSaveJsonData + dllDataJsonName);
		}

		[ContextMenu("2. Generate Assets Data Dictionary: Origin")]
		public void GenerateAssetsDataDic_Origin()
		{
			// все файлы кроме скриптов (они в dll) и .meta
			var assetOriginFiles = new DirectoryInfo(PathToAssets_Origin).GetFiles("*.*", SearchOption.AllDirectories).Where(x => x.Extension != ".meta"); //.Where(x => x.Extension != ".cs");
			GenerateAssetsDataDic(assetOriginFiles.ToList(), AssetsJsonOriginName, PathToAssets_Origin);
			Debug.Log("Dic Origin Saved");
		}

		[ContextMenu("2a. Add New Scripts: Origin")]
		public void AddNewScripts_Origin()
		{
			// все файлы кроме скриптов (они в dll) и .meta
			var assetOriginFiles = new DirectoryInfo(PathToAssetsNew_Origin).GetFiles("*.*", SearchOption.AllDirectories).Where(x => x.Extension != ".meta"); //.Where(x => x.Extension != ".cs");
			GenerateAssetsDataDic(assetOriginFiles.ToList(), AssetsJsonOriginName, PathToAssetsNew_Origin, true);
			Debug.Log("Dic Origin Modified");
		}

		[ContextMenu("3. Generate Assets Data Dictionary: Base")]
		public void GenerateAssetsDataDic_Base()
		{
			var assetBaseFiles = new List<FileInfo>();
			foreach (var dir in DirsToParseAssets_Base)
			{
				var pathBase = PathToAssets_Base + dir;
				var pathBaseFiles = new DirectoryInfo(pathBase).GetFiles("*.*", SearchOption.AllDirectories).Where(x => x.Extension != ".meta");
				foreach (var file in pathBaseFiles)
				{
					if (!assetBaseFiles.Contains(file))
					{
						assetBaseFiles.Add(file);
					}
				}
			}
			GenerateAssetsDataDic(assetBaseFiles, AssetsJsonBaseName, PathToAssets_Base);
			Debug.Log("Dic Base Saved");
		}

		public void GenerateAssetsDataDic(List<FileInfo> assetOriginFiles, string dicName, string pathToAssets, bool isModify = false)
		{
			var assetOriginExt = assetOriginFiles.Select(x => x.Extension).Distinct().ToList();

			var dicsFileIDs = new Dictionary<string, Dictionary<string, ResourceItem>>();

			foreach (var ext in assetOriginExt)
			{
				dicsFileIDs.Add(ext, new());
			}

			if (DoSaveExtJson)
			{
				var assetOriginExtJson = JsonConvert.SerializeObject(assetOriginExt, Formatting.Indented);
				File.WriteAllText(PathToSaveJsonData + dicName.Replace(".json", "Ext.json"), assetOriginExtJson);
				return;
			}

			foreach (var assetOriginFile in assetOriginFiles)
			{
				if (assetOriginFile.Name.EndsWith("_alpha.png")) continue;

				var fileGuidPath = assetOriginFile.FullName + ".meta";
				if (!File.Exists(fileGuidPath)) continue;

				var name = assetOriginFile.Name;
				var ext = assetOriginFile.Extension;
				var dic = dicsFileIDs[ext];
				if (!dic.ContainsKey(name))
				{
					var fileGuid = File.ReadAllLines(fileGuidPath)[1].Split(' ')[1];

					var path = assetOriginFile.FullName[pathToAssets.Length..];
					var item = new ResourceItem(name, ext, fileGuid, path);
					dic.Add(fileGuid, item);
				}
			}

			var dicPath = PathToSaveJsonData + dicName;
			if (isModify && File.Exists(dicPath))
			{
				var currentDicFile = File.ReadAllText(dicPath);
				var currentDic = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ResourceItem>>>(currentDicFile);
				foreach (var item in currentDic)
				{
					if (dicsFileIDs.ContainsKey(item.Key))
					{
						foreach (var asset in dicsFileIDs[item.Key])
						{
							if (!item.Value.ContainsKey(asset.Key)) item.Value.Add(asset.Key, asset.Value);
						}
					}
				}
				dicsFileIDs = currentDic;
			}
			var dicsFileIDsJson = JsonConvert.SerializeObject(dicsFileIDs, Formatting.Indented);
			File.WriteAllText(PathToSaveJsonData + dicName, dicsFileIDsJson);
		}

		[ContextMenu("Parse Single File")]
		public void ParseSingleFile()
		{
			FixLog = new();
			var file = new FileInfo(PathToParseSingleAsset);
			var fileTypes = new HashSet<string>(GetFileTypesList(FileTypesParse), StringComparer.OrdinalIgnoreCase);

			if (!fileTypes.Contains(file.Extension))
			{
				Debug.Log("Тип файла не соответствует вариантам конвертации");
				return;
			}
			ParseFile(file);

			Debug.Log(countFilesConvert > 0 ? "Файл изменен" : "Изменений нет");
		}

		[ContextMenu("Parse Assets in Folder")]
		public void ParseDirectory()
		{
			FixLog = new();

			var fileTypes = new HashSet<string>(GetFileTypesList(FileTypesParse), StringComparer.OrdinalIgnoreCase);
			var assetsFilesWhatFrom = new List<FileInfo>();

			var dir = new DirectoryInfo(PathToParseAssetsFolder);
			var files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(f => fileTypes.Contains(f.Extension)).ToList();
			if (files == null || files.Count == 0)
			{
				Debug.Log("Типы файлов не соответствует вариантам конвертации");
				return;
			}

			if (DoOnlyAtlases)
			{
				foreach (var file in files)
				{
					if ((file.Extension == ".asset" || file.Extension == ".prefab") && !file.Name.StartsWith("Atlas_")) continue;
					if (file.FullName.Contains("NGUI")) continue;

					if (!assetsFilesWhatFrom.Contains(file))
					{
						assetsFilesWhatFrom.Add(file);
					}
				}
			}
			else
			{
				assetsFilesWhatFrom = files;
			}

			countFiles = 0;

			foreach (var fileAsset in assetsFilesWhatFrom)
			{
				ParseFile(fileAsset, assetsFilesWhatFrom.Count);
			}

			EditorUtility.ClearProgressBar();
			Debug.Log("Конвертация завершена " + countFilesConvert + "/" + countFiles);
		}

		private void ParseFile(FileInfo fileAsset, int filesCount = 1)
		{
			countFiles++;

			var file = CopyFile(fileAsset);
			var fileData = File.ReadAllLines(file.FullName).ToList();
			var newData = FixData(fileData, out int countRedact);
			if (filesCount > 1) EditorUtility.DisplayProgressBar("Replace Guides", file.Name + " : " + countRedact, countFiles / filesCount * 100);

			if (FixLog.Count > 0)
			{
				var dir = PathToSaveJsonData + "Log\\";

				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
				var pathLog = dir + Path.GetFileNameWithoutExtension(file.Name) + "_log.txt";
				File.WriteAllLines(pathLog, FixLog);
				FixLog = new();
			}

			if (newData != null)
			{
				countFilesConvert++;
				File.WriteAllLines(file.FullName, newData);
			}
		}

		private FileInfo CopyFile(FileInfo file)
		{
			if (file.FullName.ToLower().Contains(DirToCopyNewAssets.ToLower())) return file;
			var pathOldAssetIndex = file.FullName.IndexOf("Assets\\") + 7;
			var pathRelativeOld = file.FullName[pathOldAssetIndex..];
			var pathNewAsset = PathToAssets_Base + DirToCopyNewAssets + pathRelativeOld;
			var dirNewAsset = pathNewAsset[..^file.Name.Length];

			if (!Directory.Exists(dirNewAsset))
			{
				Directory.CreateDirectory(dirNewAsset);
			}
			if (File.Exists(pathNewAsset))
			{
				File.Delete(pathNewAsset);
			}
			file.CopyTo(pathNewAsset);

			return new FileInfo(pathNewAsset);
		}

		[ContextMenu("Fix Atlases")]
		public void FixAtlases()
		{
			var baseAtlases = AssetsBaseData[".asset"].Values.Where(x => x.Name.ToLower().StartsWith("atlas_"));
			var originAtlases = AssetsOriginData[".asset"];//.Where(x=>x.Value.Name.ToLower().StartsWith("atlas_"));

			var count = 0;
			var atlasCount = baseAtlases.Count();
			foreach (var baseAtlas in baseAtlases)
			{
				count++;
				var atlasPath = PathToAssets_Base + baseAtlas.FilePath;
				if (DoAtlasesBackup)
				{
					var backupPath = PathToSaveJsonData + "Backup\\" + baseAtlas.FilePath;
					var backupPathDir = backupPath[..^baseAtlas.Name.Length];
					if (!Directory.Exists(backupPathDir))
					{
						Directory.CreateDirectory(backupPathDir);
					}
					if (File.Exists(backupPath))
					{
						File.Delete(backupPath);
					}
					File.Copy(atlasPath, backupPath);
				}

				var baseAtlasData = File.ReadAllLines(atlasPath).ToList();
				var originAtlasItem = originAtlases.Values.FirstOrDefault(x => x.Name == baseAtlas.Name);
				//if (originAtlases.TryGetValue(baseAtlas.Name, out ResourceItem originAtlasItem))
				if (originAtlasItem != null)
				{
					var originAtlasData = File.ReadAllLines(PathToAssets_Origin + originAtlasItem.FilePath).ToList();

					var spriteString = originAtlasData.FirstOrDefault(x => x.Trim().StartsWith("mSprites:"));
					if (string.IsNullOrEmpty(spriteString)) continue;
					var indexOrigin = originAtlasData.IndexOf(spriteString);

					var indexBase = baseAtlasData.IndexOf(baseAtlasData.FirstOrDefault(x => x.Trim().StartsWith("mSprites:")));

					var originRange = originAtlasData.GetRange(indexOrigin, originAtlasData.Count - indexOrigin - 2);
					var baseAtlasDataReplacement = baseAtlasData.GetRange(baseAtlasData.Count - 2, 2);
					baseAtlasData = baseAtlasData.GetRange(0, indexBase).Concat(originRange).Concat(baseAtlasDataReplacement).ToList();
					File.WriteAllLines(atlasPath, baseAtlasData);
				}
			}
			Debug.Log($"Завершили редиктирование атласов ({count}/{atlasCount})");
		}

		public List<FileType> FileTypesParse = new List<FileType>()
	{
		new FileType(true,".unity"),
		new FileType(true,".asset"),
		new FileType(true,".prefab"),
	};

		public List<string> GetFileTypesList(List<FileType> list)
		{
			return list.Where(x => x.IsActive).Select(x => x.Name).ToList();
		}

		private void AddToLog(string log)
		{
			FixLog.Add(log);
			Debug.Log(log);
		}

		private List<string> FixData(List<string> content, out int countRedact)
		{
			//var baseAtlases = AssetsBaseData[".asset"].Values.Where(x => x.Name.ToLower().StartsWith("atlas_"));

			int countLines = -1;
			countRedact = 0;
			var newContent = new List<string>();
			newContent.AddRange(content);
			foreach (var line in newContent)
			{
				countLines++;
				var trimLine = line.Trim();
				if (trimLine.Length > 0 && trimLine.Contains(", guid:"))
				{
					var fields = line.Split(',');

					var fileID = fields[0].Split(' ').Last();
					string fileGUID = fields.Length > 2 ? fields[1].Split(' ').Last() : "";

					if (string.IsNullOrEmpty(fileGUID) || fileID == "0") continue;

					if (trimLine.StartsWith("m_Script:"))
					{
						string ext = ".cs";
						//fileID: 11500000
						if (fileID == "11500000")
						{
							if (AssetsOriginData[ext].TryGetValue(fileGUID, out ResourceItem scriptsOrigin))
							{
								var scriptName = scriptsOrigin.Name;
								var baseScriptItem = AssetsBaseData[ext].Values.FirstOrDefault(x => x.Name == scriptName);
								if (baseScriptItem != null)
								{
									var newLine = line.Replace(fileID, baseScriptItem.ScriptFileId.ToString()).Replace(fileGUID, baseScriptItem.ScriptGuid);
									content[countLines] = newLine;
									countRedact++;
								}
								else
								{
									AddToLog("Base. Скрипт: " + scriptName + " не найден, строка " + countLines);
								}
							}
							else
							{
								AddToLog("Origin. Скрипт: " + fileGUID + " не найден, строка " + countLines);
							}
						}
						else
						{
							//if (DllDataDicList.TryGetValue(fileGUID, out ResourceItem originScriptItem))
							//{
							//    var scriptName = originScriptItem.Name;
							//    var baseScriptItem = AssetsBaseData[ext].Values.FirstOrDefault(x => x.Name == scriptName);
							//    if (baseScriptItem != null)
							//    {
							//        var newLine = line.Replace(fileID, baseScriptItem.ScriptFileId.ToString()).Replace(fileGUID, baseScriptItem.ScriptGuid);
							//        content[countLines] = newLine;
							//        countRedact++;
							//    }
							//    else
							//    {
							//        AddToLog("Base. Скрипт: " + scriptName + " не найден, строка " + countLines);
							//    }
							//}
							//else
							//{
							//    AddToLog("Origin. Скрипт: " + fileGUID + " не найден, строка " + countLines);
							//}

							if (DllDataIDDic.TryGetValue(fileGUID, out Dictionary<long, ResourceItem> originScriptItemDic))
							{
								//var dllDataDicIDList = new Dictionary<long, ResourceItem>();
								//foreach (var dllDic in originScriptItemDic)
								//{
								//    var fileOriginId = dllDic.Value.DllFileId;
								//    if (!dllDataDicIDList.ContainsKey(fileOriginId)) dllDataDicIDList.Add(fileOriginId, dllDic.Value);
								//}

								if (long.TryParse(fileID, out long fileIdResult) && originScriptItemDic.TryGetValue(fileIdResult, out ResourceItem originScriptItem))
								{
									var scriptName = originScriptItem.Name + ext;

									var baseScriptItem = AssetsBaseData[ext].Values.FirstOrDefault(x => x.Name == scriptName);
									if (baseScriptItem != null)
									{
										var newLine = line.Replace(fileID, baseScriptItem.ScriptFileId.ToString()).Replace(fileGUID, baseScriptItem.ScriptGuid);
										content[countLines] = newLine;
										countRedact++;
									}
									else
									{
										AddToLog("Base. Скрипт: " + scriptName + " не найден, строка " + countLines);
									}
								}
								else
								{
									AddToLog("Base. Скрипт: " + fileID + " не найден, строка " + countLines);
								}
							}
							else
							{
								AddToLog("Origin. Скрипт: " + fileGUID + " не найден, строка " + countLines);
							}
						}
					}
					else if (trimLine.StartsWith("mTrueTypeFont:"))
					{
						//fileID: 12800000
						string ext = ".ttf";
						var fontOrigin = AssetsOriginData[ext].Values.FirstOrDefault(x => x.ScriptGuid == fileGUID);
						if (fontOrigin == null)
						{
							ext = ".otf";
							fontOrigin = AssetsOriginData[ext].Values.FirstOrDefault(x => x.ScriptGuid == fileGUID);
						}
						if (fontOrigin != null)
						{
							var fontOriginName = fontOrigin.Name;
							var baseFontItem = AssetsBaseData[ext].Values.FirstOrDefault(x => x.Name == fontOriginName);
							if (baseFontItem != null)
							{
								var newLine = line.Replace(fileID, "12800000").Replace(fileGUID, baseFontItem.ScriptGuid);
								content[countLines] = newLine;
								countRedact++;
							}
							else
							{
								var originFile = new FileInfo(PathToAssets_Origin + fontOrigin.FilePath);
								CopyFile(originFile);
								var originFileMeta = new FileInfo(PathToAssets_Origin + fontOrigin.FilePath + ".meta");
								CopyFile(originFileMeta);
								Debug.Log("Base. Шрифт:" + fontOriginName + " не найден. Копирую его в Base");
							}
						}
						else
						{
							AddToLog("Origin. Шрифт: " + fileGUID + " не найден, строка " + countLines);
						}
					}
					else if (trimLine.StartsWith("mAtlas:"))
					{
						//fileID: 11400000 - .asset
						bool isAsset = fileID == "11400000";
						string ext = isAsset ? ".asset" : ".prefab";

						var originPrefab = AssetsOriginData[ext].Values.FirstOrDefault(x => x.ScriptGuid == fileGUID);
						if (originPrefab != null)
						{
							var prefabOriginName = originPrefab.Name;

							ResourceItem baseAssetItem = null;
							if (!isAsset)
							{
								var name = Path.GetFileNameWithoutExtension(prefabOriginName);
								baseAssetItem = AssetsBaseData[".asset"].Values.FirstOrDefault(x => x.Name == name + ".asset" || x.Name == name + "_HD.asset");
								if (baseAssetItem != null)
								{
									var newLine = line.Replace(fileID, "11400000").Replace(fileGUID, baseAssetItem.ScriptGuid);

									//FixStringType(fields, ref newLine);

									content[countLines] = newLine;
									countRedact++;
									continue;
								}
							}

							if (baseAssetItem == null)
							{
								var basePrefabItem = AssetsBaseData[ext].Values.FirstOrDefault(x => x.Name == prefabOriginName);
								if (basePrefabItem != null)
								{
									var newLine = line.Replace(fileGUID, basePrefabItem.ScriptGuid);

									if (isAsset)
									{
										newLine = newLine.Replace(fileID, "11400000");
									}
									content[countLines] = newLine;
									countRedact++;
								}
								else
								{
									var originFile = new FileInfo(PathToAssets_Origin + originPrefab.FilePath);
									CopyFile(originFile);
									var originFileMeta = new FileInfo(PathToAssets_Origin + originPrefab.FilePath + ".meta");
									CopyFile(originFileMeta);
									Debug.Log("Base. Prefab атласа:" + prefabOriginName + " не найден. Копирую его в Base");
								}
							}
						}
						else
						{
							AddToLog("Origin. Prefab атласа: " + fileGUID + " не найден, строка " + countLines);
						}
					}
					else if (GetExtByResourceType(trimLine, out string ext, out string newFileId))
					{
						var originPrefab = AssetsOriginData[ext].Values.FirstOrDefault(x => x.ScriptGuid == fileGUID);
						if (originPrefab != null)
						{
							var prefabOriginName = originPrefab.Name;
							var basePrefabItem = AssetsBaseData[ext].Values.FirstOrDefault(x => x.Name == prefabOriginName);
							if (basePrefabItem != null)
							{
								var newLine = line.Replace(fileID, !string.IsNullOrEmpty(newFileId) ? newFileId : fileID).Replace(fileGUID, basePrefabItem.ScriptGuid);
								content[countLines] = newLine;
								countRedact++;
							}
							else
							{
								var originFile = new FileInfo(PathToAssets_Origin + originPrefab.FilePath);
								CopyFile(originFile);
								var originFileMeta = new FileInfo(PathToAssets_Origin + originPrefab.FilePath + ".meta");
								CopyFile(originFileMeta);
								Debug.Log("Base. Ресурс:" + prefabOriginName + " не найден. Копирую его в Base");
							}
						}
						else
						{
							AddToLog("Origin. Ресурс: " + fileGUID + " (" + ext + ") не найден, строка " + countLines);
						}
					}

					//mMat: 2100000
					//mShader: 4800000
					//mTexture: 2800000
				}
			}
			return countRedact > 0 ? content : null;
		}

		private bool GetExtByResourceType(string trimLine, out string resType, out string fileID)
		{
			fileID = "";
			resType = "";
			bool isCondition = true;
			if (trimLine.ToLower().Contains("prefab:"))
			{
				resType = ".prefab";
			}
			else if (trimLine.StartsWith("mMat:"))
			{
				resType = ".mat";
				fileID = "2100000";
			}
			else if (trimLine.StartsWith("mShader:"))
			{
				resType = ".shader";
				fileID = "4800000";
			}
			else if (trimLine.StartsWith("mTexture:"))
			{
				resType = ".png";
				fileID = "2800000";
			}
			else
			{
				isCondition = false;
			}
			return isCondition;
		}


		//private void FixCollider(string[] file, int i)
		//{
		//	if (file[i].Contains("BoxCollider:") && file[i + 1].Contains("serializedVersion: 3"))
		//	{
		//		string str_after = file[i + 1];
		//		str_after = str_after.Replace("serializedVersion: 3", "serializedVersion: 2");
		//		file[i + 1] = str_after;
		//		if (i + 1 < file.Length) i++;
		//		count++;
		//	}
		//}

		//private void FixSingleLine(ResourceItem item, string ext, string line)
		//{
		//    var itemOriginName = item.Name;
		//    if (AssetsBaseData[ext].TryGetValue(itemOriginName, out ResourceItem baseScriptItem))
		//    {
		//        var newLine = line.Replace(fileID, baseScriptItem.ScriptFileId.ToString()).Replace(fileGUID, baseScriptItem.ScriptGuid);
		//        content[countLines] = newLine;
		//        countRedact++;
		//    }
		//    else
		//    {
		//        var originFile = new FileInfo(PathToAssets_Origin + item.FilePath);
		//        CopyFile(originFile);
		//        var originFileMeta = new FileInfo(PathToAssets_Origin + item.FilePath + ".meta");
		//        CopyFile(originFileMeta);
		//        Debug.Log("Base. Шрифт:" + itemOriginName + " не найден. Копирую его в Base");
		//    }
		//}

		private void FixLineType(string[] fields, ref string newLine)
		{
			if (fields.Length > 2)
			{
				var fileTypeField = fields[2];
				var fileType = fileTypeField.Split(' ').Last();
				if (fileType == "3")
				{
					newLine = newLine.Replace(fileTypeField, "type: 2");
				}
			}
		}

		// SPRemoldMainPopup и другие
		// UI Camp HUD
		// Activity_Popup
		// Popup_SurvivalManual_Main и другие
		// Survivor_Info_PopUp
	}

	[Serializable]
	public class ResourceItem
	{
		public string Name { get; set; }
		public string Extention { get; set; }

		//from
		public long DllFileId { get; set; }
		public long ScriptFileId { get; set; }

		//to
		public string DllGuid { get; set; }
		public string ScriptGuid { get; set; }

		public string FilePath { get; set; }

		public ResourceItem(string dllGuid, string name, long dllFileId)
		{
			Name = name;
			Extention = ".cs";
			DllFileId = dllFileId;
			DllGuid = dllGuid;
		}

		public ResourceItem(string name, string ext, string guid, string path)
		{
			Name = name;
			Extention = ext;
			ScriptFileId = 11500000;
			ScriptGuid = guid;
			FilePath = path;
		}
		public ResourceItem()
		{
		}
	}

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
}
#endif
