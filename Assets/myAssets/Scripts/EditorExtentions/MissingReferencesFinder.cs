#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

public static class MissingReferencesFinder
{
	private static CustomAssetsData configData;

	public static void SetConfigData(CustomAssetsData data)
	{
		configData = data;
	}

	public struct MissingErrorInfo
	{
		public string ComponentName;
		public string PropertyName;
	}

	private static readonly Regex GuidRegex = new Regex(
		@"guid:\s*([a-fA-E0-0-9]{32})",
		RegexOptions.Compiled | RegexOptions.IgnoreCase
	);
	private static readonly Regex ComponentNameRegex = new Regex(
		@"m_Name:\s*(.+)",
		RegexOptions.Compiled
	);
	private static readonly Regex ScriptGuidRegex = new Regex(
		@"m_Script:\s*\{fileID:[^,]+,\s*guid:\s*([a-fA-E0-0-9]{32})",
		RegexOptions.Compiled | RegexOptions.IgnoreCase
	);

	/// <summary>
	/// Универсальный метод поиска битых ссылок по списку путей к ассетам из AssetDependencyTracker
	/// </summary>
	public static Dictionary<string, List<MissingErrorInfo>> FindMissingReferences(
		string[] assetPaths,
		Action<string, float> onProgress
	)
	{
		var results = new Dictionary<string, List<MissingErrorInfo>>();
		if (assetPaths == null || assetPaths.Length == 0)
			return results;

		int total = assetPaths.Length;

		for (int i = 0; i < total; i++)
		{
			string assetPath = assetPaths[i];

			onProgress?.Invoke(Path.GetFileName(assetPath), (float)i / total);

			if (!File.Exists(assetPath))
				continue;

			var missingErrors = ScanFileForMissingReferences(assetPath);
			if (missingErrors.Count > 0)
			{
				results[assetPath] = missingErrors;
			}
		}

		return results;
	}

	private static List<MissingErrorInfo> ScanFileForMissingReferences(string filePath)
	{
		var errors = new List<MissingErrorInfo>();
		string[] lines = File.ReadAllLines(filePath);

		string currentComponent = "Unknown Component/Object";
		var scriptNameCache = new Dictionary<string, string>();

		for (int i = 0; i < lines.Length; i++)
		{
			string line = lines[i];

			// 1. Определяем имя текущего компонента
			string trimmedLine = line.TrimStart();
			if (
				trimmedLine.StartsWith("MonoBehaviour:")
				|| trimmedLine.StartsWith("GameObject:")
				|| trimmedLine.StartsWith("Transform:")
				|| trimmedLine.StartsWith("Animator:")
			)
			{
				int colonIndex = trimmedLine.IndexOf(':');
				if (colonIndex > 0)
				{
					currentComponent = trimmedLine.Substring(0, colonIndex);
				}
			}

			// 2. Ищем GUID и проверяем его валидность
			Match scriptMatch = ScriptGuidRegex.Match(line);
			if (scriptMatch.Success)
			{
				string scriptGuid = scriptMatch.Groups[1].Value; // Используем индекс 1 для получения группы с самим GUID
				if (!scriptNameCache.TryGetValue(scriptGuid, out string scriptName))
				{
					string scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuid);
					scriptName = !string.IsNullOrEmpty(scriptPath)
						? Path.GetFileNameWithoutExtension(scriptPath)
						: "Missing Script";
					scriptNameCache[scriptGuid] = scriptName;
				}
				currentComponent = $"MonoBehaviour ({scriptName})";
			}

			Match nameMatch = ComponentNameRegex.Match(line);
			if (nameMatch.Success)
			{
				string nameValue = nameMatch.Groups[1].Value.Trim();
				if (!string.IsNullOrEmpty(nameValue))
				{
					currentComponent += $" [{nameValue}]";
				}
			}

			Match guidMatch = GuidRegex.Match(line);
			if (guidMatch.Success)
			{
				string guid = guidMatch.Groups[1].Value;

				if (guid != "00000000000000000000000000000000")
				{
					string realPath = AssetDatabase.GUIDToAssetPath(guid);

					if (string.IsNullOrEmpty(realPath))
					{
						string propertyName = "Unknown Property";
						int colonIndex = line.IndexOf(':');
						if (colonIndex > 0)
						{
							propertyName = line.Substring(0, colonIndex).Trim();
						}

						errors.Add(
							new MissingErrorInfo
							{
								ComponentName = currentComponent,
								PropertyName = propertyName,
							}
						);
					}
				}
			}
		}

		return errors;
	}

	// Новый блок методов

	public struct MissingReferenceInfo
	{
		public Object targetObject;
		public Object OverrideRefObject;

		public string targetObjectName;
		public string componentName;
		public string propertyPath;
		public string expectedTypeName;

		public string objectGuid;
		public long fileID;
		public string assetPath;
		public int lineIndex;

		public int componentIndex;
		public string localFileID;
	}

	/// <summary>
	/// Универсальный метод поиска битых ссылок по списку путей к ассетам из CustomAssetsReplacer
	/// </summary>
	public static List<MissingReferenceInfo> FindMissingReferences(
		Object obj, 
		string assetPath, 
		string[] lines, 
		IProgress<(int current, string text)> progressHandler,
		CancellationToken token)
	{
		List<MissingReferenceInfo> missingList = new ();
		if (obj == null) return missingList;

		token.ThrowIfCancellationRequested();

		bool isParseMethod = true; // Всегда парсить сам файл

		if (obj is SceneAsset || isParseMethod)
		{
			missingList = FindMissingReferencesScene(obj, assetPath, lines, progressHandler, token);
		}
		else if (obj is GameObject go)
		{
			// Сначала ищем целиком потерянные скрипты через сериализацию самого GameObject
			CheckMissingScriptsOnGameObject(go, assetPath, missingList);

			// Затем проверяем внутренние поля всех «живых» компонентов
			Component[] components = go.GetComponents<Component>();
			int compIndex = 0;
			foreach (var comp in components)
			{
				// Пропускаем null, так как мы их обработали в методе выше
				if (comp == null) continue;

				CheckSerializedObject(comp, comp.GetType().Name, missingList, assetPath, compIndex, lines);
				compIndex++;
			}

			// Рекурсивный обход иерархии сцены или префаба
			foreach (Transform child in go.transform)
			{
				missingList.AddRange(FindMissingReferences(child.gameObject, assetPath, lines, progressHandler, token));
			}
		}
		else
		{
			// Если это ScriptableObject или другой тип ассета
			CheckSerializedObject(obj, obj.GetType().Name, missingList, assetPath, 0, lines);
		}

		return missingList;
	}

	private static List<MissingReferenceInfo> FindMissingReferencesScene(
		Object scene, 
		string assetPath, 
		string[] lines, 
		IProgress<(int current, string text)> progressHandler, 
		CancellationToken token)
	{
		List<MissingReferenceInfo> missingList = new();

		int currentIndex = -1;
		int total = lines.Length;
		string currentLine = string.Empty;
		var searchPattern = @"^\s*([^:]+):\s*\{fileID:\s*(-?\d+),\s*guid:\s*([a-fA-F0-9]{32}),\s*type:\s*(\d+)\}";

		if (scene is Material mat)
		{
			string compName = string.Empty;
			int texCount = -1;
			while (currentIndex < total - 1)
			{
				currentIndex++;
				currentLine = lines[currentIndex];

				token.ThrowIfCancellationRequested();
				if (currentLine.Contains("m_Name:"))
				{
					// нашли имя компонента (TreeLabel)
					compName = currentLine.Split(' ').Last();
				}
				var addToMissing = AddToMissing(currentLine, searchPattern, missingList);

				if (addToMissing.addToMissing)
				{
					if (addToMissing.propType == "m_Texture")
					{
						texCount++;
					}
					missingList.Add(new MissingReferenceInfo
					{
						targetObject = mat,
						targetObjectName = "mat",
						componentName = compName,
						propertyPath = addToMissing.propType,
						expectedTypeName = "Material",

						objectGuid = addToMissing.guid,
						fileID = addToMissing.fileID,
						assetPath = assetPath,
						lineIndex = currentIndex,
						componentIndex = texCount
					});
				}
			}
		}
		else
		{
			while (currentIndex < total - 1)
			{
				currentIndex++;
				currentLine = lines[currentIndex];
				token.ThrowIfCancellationRequested();
				if (currentLine == "MonoBehaviour:")
				{
					// Нашли targetLocalId (1995444365)
					string targetLocalId = lines[currentIndex - 1].Split('&').Last();
					string compName = string.Empty;
					string localIdLine = string.Empty;
					int compIndex = -1;
					int localIdIndex = currentIndex - 1;
					//string compPattern = @"^\s*-\s*component:\s*\{fileID:\s*(-?\d+)\}";
					while (localIdLine != "GameObject:")
					{
						localIdIndex--;
						localIdLine = lines[localIdIndex];
						if (localIdLine.TrimStart() == "m_Component:")
						{
							int compStartIndex = -1;
							while (!localIdLine.StartsWith("--- "))
							{
								localIdIndex++;
								compStartIndex++;
								localIdLine = lines[localIdIndex];
								if (localIdLine.TrimStart() == "- component: {fileID: " + targetLocalId + "}")
								{
									// нашли индекс компонента
									compIndex = compStartIndex;
									if (!string.IsNullOrEmpty(compName)) break;
								}
								if (localIdLine.Contains("m_Name:"))
								{
									// нашли имя компонента (TreeLabel)
									compName = localIdLine.Split(' ').Last();
									if (compIndex != -1) break;
								}
							}
							break;
						}
					}
					while (!currentLine.StartsWith("--- ") && currentIndex < total - 1)
					{
						currentIndex++;
						currentLine = lines[currentIndex];
						token.ThrowIfCancellationRequested();

						var addToMissing = AddToMissing(currentLine, searchPattern, missingList);
						if (addToMissing.addToMissing)
						{
							if (compIndex == -1) compIndex = 0;
							missingList.Add(new MissingReferenceInfo
							{
								targetObject = scene,
								targetObjectName = "scene",
								componentName = compName,
								propertyPath = addToMissing.propType,
								expectedTypeName = "MonoBehaviour",

								objectGuid = addToMissing.guid,
								fileID = addToMissing.fileID,
								assetPath = assetPath,
								lineIndex = currentIndex,
								componentIndex = compIndex,
								localFileID = targetLocalId
							});
						}
					}
				}
				// каждые 500 строк
				if (currentIndex % 500 == 0 || currentIndex == total - 1)
				{
					progressHandler?.Report((currentIndex + 1, $"Строка {currentIndex + 1} из {total}"));
				}
			}
		}

		return missingList;
	}

	private static (bool addToMissing, string propType, long fileID, string guid, string type) AddToMissing(string currentLine, string searchPattern, List<MissingReferenceInfo> missingList)
	{
		string propType = "";
		long fileID = 0;
		string guid = "";
		string type = "";
		bool addToMissing = false;
		Match match = Regex.Match(currentLine, searchPattern);
		if (match.Success)
		{
			// нашли тип объекта (mTrueTypeFont)
			propType = match.Groups[1].Value;
			if (!long.TryParse(match.Groups[2].Value, out fileID)) fileID = 0;
			guid = match.Groups[3].Value;
			type = match.Groups[4].Value;

			var missingAddedItem = missingList.FirstOrDefault(x => x.objectGuid == guid);
			if (string.IsNullOrEmpty(missingAddedItem.objectGuid))
			{
				//var missingAssetPath = AssetDatabase.GUIDToAssetPath(guid);
				var missingAssetMapping = configData.baseAssetsDataMapping.Find(m => m.guid == guid);
				if (string.IsNullOrEmpty(missingAssetMapping.guid))
				{
					addToMissing = true;
				}
			}
			else
			{
				addToMissing = true;
			}
		}
		return (addToMissing, propType, fileID, guid, type);
	}

	/// <summary>
	/// Находит пропавшие скрипты (Missing MonoBehaviour) и вытаскивает их оригинальный GUID
	/// </summary>
	private static void CheckMissingScriptsOnGameObject(GameObject go, string assetPathBase, List<MissingReferenceInfo> list, string[] lines = null)
	{
		SerializedObject goSerialized = new (go);
		SerializedProperty componentArray = goSerialized.FindProperty("m_Component"); // Список всех компонентов в инспекторе

		if (componentArray == null) return;

		// Получаем массив всех живых компонентов для сопоставления индексов
		Component[] components = go.GetComponents<Component>();
		int aliveComponentIndex = 0;
		for (int i = 0; i < componentArray.arraySize; i++)
		{
			SerializedProperty componentPair = componentArray.GetArrayElementAtIndex(i);
			SerializedProperty componentRef = componentPair.FindPropertyRelative("component");

			if (componentRef == null) continue;

			// Если ссылка на сам компонент в массиве Unity пустая (равна null), 
			// либо если на этой позиции в реальном GameObject находится null — это Missing Script
			bool isMissingInArray = componentRef.objectReferenceValue == null;
			bool isMissingInComponents = aliveComponentIndex < components.Length && components[aliveComponentIndex] == null;

			if (isMissingInArray || isMissingInComponents)
			{
				string guid = string.Empty;
				long fileID = 0;
				int index = 0;

				// Достаем объект самого сломанного компонента
				Object missingComponentTarget = componentRef.objectReferenceValue;

				if (missingComponentTarget != null)
				{
					// Создаем SerializedObject напрямую из системного инстанса битого компонента
					SerializedObject missingCompSerialized = new (missingComponentTarget);

					// У любого MonoBehaviour скрытая ссылка на файл скрипта лежит в поле m_Script
					SerializedProperty scriptProp = missingCompSerialized.FindProperty("m_Script");
					if (scriptProp != null)
					{
						SerializedProperty guidProp = scriptProp.FindPropertyRelative("m_Guid");
						if (guidProp != null)
						{
							guid = guidProp.stringValue;
							if (!string.IsNullOrEmpty(guid))
							{
								SerializedProperty fileIDProp = scriptProp.FindPropertyRelative("m_FileID");
								if (fileIDProp != null)
								{
									long.TryParse(fileIDProp.stringValue, out fileID);
								}
							}
							else
							{
								lines ??= File.ReadAllLines(assetPathBase);
								var (parsedFileID, parsedGuid, assetPath, parsedIndex) = guidProp.GetMissingReferenceFromAnyYaml(assetPathBase, lines);
								guid = parsedGuid;
								fileID = parsedFileID;
								assetPathBase = assetPath;
								index = parsedIndex;
							}
						}
					}
				}

				list.Add(new MissingReferenceInfo
				{
					targetObject = go,
					targetObjectName = go.name,
					componentName = $"Missing Script (Index: {i})",
					propertyPath = $"m_Component.Array.data[{i}]",
					expectedTypeName = "MonoBehaviour",
					objectGuid = guid,
					fileID = fileID,
					assetPath = assetPathBase,
					lineIndex = index,
					componentIndex = i
				});
			}

			// Инкрементируем индекс живых компонентов только если текущий элемент в сериализации был валидным
			if (componentRef.objectReferenceValue != null)
			{
				aliveComponentIndex++;
			}
		}
	}

	private static void CheckSerializedObject(Object obj, string componentName, List<MissingReferenceInfo> list, string assetPathBase, int compIndex, string[] lines = null)
	{
		SerializedObject so = new SerializedObject (obj);
		SerializedProperty sp = so.GetIterator();
		while (sp.Next(true))
		{
			if (sp.propertyType == SerializedPropertyType.ObjectReference)
			{
				//Missing Reference
				if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
				{
					var (fileID, guid, assetPath, index) = sp.GetMissingReferenceFromAnyYaml(assetPathBase, lines);

					list.Add(new MissingReferenceInfo
					{
						targetObject = obj,
						targetObjectName = obj.name,
						componentName = componentName,
						propertyPath = sp.propertyPath,
						expectedTypeName = sp.type,

						objectGuid = guid,
						fileID = fileID,
						assetPath = assetPath,
						lineIndex = index,
						componentIndex = compIndex
					});
				}
			}
		}
	}

	public static string GetAssetPath(this Object targetObject)
	{
		if (targetObject == null) return null;

		string assetPath = string.Empty;

		// 1. Проверяем, открыт ли сейчас Prefab Mode (Режим редактирования префаба)
		PrefabStage currentStage = PrefabStageUtility.GetCurrentPrefabStage();

		// 3. Если мы находимся в РЕЖИМЕ РЕДАКТИРОВАНИЯ ПРЕФАБА
		if (currentStage != null)// && targetObject is Component comp && currentStage.IsPartOfPrefabContents(comp.gameObject))
		{
			// Напрямую забираем путь к редактируемому .prefab файлу на диске
			assetPath = currentStage.assetPath;
		}
		// 4. Обычный режим (Сцена или выбор префаба в окне Project)
		else if (targetObject is Component || targetObject is GameObject)
		{
			if (PrefabUtility.IsPartOfAnyPrefab(targetObject))
			{
				Object sourceObject = PrefabUtility.GetCorrespondingObjectFromSource(targetObject);
				if (sourceObject != null)
				{
					assetPath = AssetDatabase.GetAssetPath(sourceObject);
				}
			}

			if (string.IsNullOrEmpty(assetPath))
			{
				assetPath = AssetDatabase.GetAssetPath(targetObject);
				if (string.IsNullOrEmpty(assetPath) && targetObject is Component component)
				{
					assetPath = component.gameObject.scene.path;
				}
			}
		}
		else
		{
			assetPath = AssetDatabase.GetAssetPath(targetObject);
		}
		return assetPath;
	}

	private static (long targetLocalId, string assetPath) GetAssetPathData(this SerializedObject so, string propertyPath)
	{
		Object targetObject = so.targetObject;
		if (targetObject == null) return (0, string.Empty);

		string assetPath = string.Empty;
		long targetLocalId = 0;
		bool isPrefabChildWithoutOverride = false;

		// 1. Проверяем, открыт ли сейчас Prefab Mode (Режим редактирования префаба)
		PrefabStage currentStage = PrefabStageUtility.GetCurrentPrefabStage();

		// 2. Определяем, с каким типом файла мы работаем (.mat, .prefab, .unity)
		if (targetObject is Material material)
		{
			// Если это файл материала на диске
			assetPath = AssetDatabase.GetAssetPath(material);
			targetLocalId = 2100000; // У всех материалов в Unity фиксированный LocalID
		}
		else
		{
			// 3. Если мы находимся в РЕЖИМЕ РЕДАКТИРОВАНИЯ ПРЕФАБА
			if (currentStage != null && targetObject is Component comp && currentStage.IsPartOfPrefabContents(comp.gameObject))
			{
				// Напрямую забираем путь к редактируемому .prefab файлу на диске
				assetPath = currentStage.assetPath;
#pragma warning disable 0618
				targetLocalId = Unsupported.GetLocalIdentifierInFile(targetObject.GetInstanceID());
#pragma warning restore 0618
				if (targetLocalId == 0)
				{
					targetLocalId = GetLocalIdViaDebugMode(targetObject);
				}
			}
			// 4. Обычный режим (Сцена или выбор префаба в окне Project)
			else if (targetObject is Component || targetObject is GameObject)
			{
				if (PrefabUtility.IsPartOfAnyPrefab(targetObject))
				{
					bool isOverridden = IsLegacyPropertyOverridden(targetObject, propertyPath);

					if (!isOverridden)
					{
						Object sourceObject = PrefabUtility.GetCorrespondingObjectFromSource(targetObject);
						if (sourceObject != null)
						{
							assetPath = AssetDatabase.GetAssetPath(sourceObject);
							targetLocalId = (long)Unsupported.GetLocalIdentifierInFileForPersistentObject(sourceObject);
							isPrefabChildWithoutOverride = true;
						}
					}
				}

				if (!isPrefabChildWithoutOverride)
				{
					assetPath = AssetDatabase.GetAssetPath(targetObject);
					if (string.IsNullOrEmpty(assetPath) && targetObject is Component component)
					{
						assetPath = component.gameObject.scene.path;
					}
#pragma warning disable 0618
					targetLocalId = Unsupported.GetLocalIdentifierInFile(targetObject.GetInstanceID());
#pragma warning restore 0618
					if (targetLocalId == 0)
					{
						targetLocalId = GetLocalIdViaDebugMode(targetObject);
					}
					//targetLocalId = targetObject.GetInstanceID();
				}
			}
		}

		if (string.IsNullOrEmpty(assetPath) || targetLocalId == 0)
		{
			Debug.LogError("Не удалось определить путь к файлу или LocalID объекта.");
		}
		return (targetLocalId, assetPath);
	}

	public static (long fileID, string guid, string assetPath, int index) GetMissingReferenceFromAnyYaml(this SerializedProperty property, string assetPathBase, string[] lines = null)
	{
		var nullValues = (0, string.Empty, string.Empty, 0);
		(long fileID, string guid) guidData = (0, string.Empty);
		if (property.propertyType != SerializedPropertyType.ObjectReference) return nullValues;

		// 1. Получаем объект-владелец свойства
		Object targetObject = property.serializedObject.targetObject;
		if (targetObject == null) return nullValues;

		var assetPathData = GetAssetPathData(property.serializedObject, property.propertyPath);

		string assetPath = assetPathData.assetPath;
		long targetLocalId = assetPathData.targetLocalId;

		if (string.IsNullOrEmpty(assetPath) || targetLocalId == 0)
		{
			Debug.LogError("Не удалось определить путь к файлу или LocalID объекта.");
			return nullValues;
		}

		// 3. Читаем файл построчно
		if (assetPath != assetPathBase || lines == null)
		{
			lines = File.ReadAllLines(assetPath);
		}

		string headerPattern = $@"&{targetLocalId}\s*$";
		string propertyName = property.name;

		// Корректируем имя для шейдера, если оно пришло в дефолтном виде
		if (targetObject is Material && (propertyName == "m_Shader" || propertyName == "shader"))
		{
			propertyName = "m_Shader";
		}

		var startLine = lines.FirstOrDefault(x => x.StartsWith("--- ") && Regex.IsMatch(x, headerPattern));
		if (!string.IsNullOrEmpty(startLine))
		{
			var i = lines.IndexOf(startLine);
			int guidLineIndex = i;

			// СЦЕНА: Обработка модификаций префабов (Overrides) на сцене
			if (assetPath.EndsWith(".unity") && startLine.Contains("propertyPath: " + propertyName))
			{
				for (int j = 1; j <= 3 && (i + j) < lines.Length; j++)
				{
					if (lines[i + j].TrimStart().StartsWith("value:"))
					{
						guidLineIndex = i + j;
						guidData = ParseIdAndGuid(lines[guidLineIndex]);
						return new (guidData.fileID, guidData.guid, assetPath, guidLineIndex);
					}
				}
			}

			// МАТЕРИАЛ: Особая обработка для Текстур (m_TexEnvs)
			if (targetObject is Material && propertyName != "m_Shader")
			{
				// Текстуры лежат в блоке вида:
				// - _MainTex:
				//     m_Texture: {fileID: 0, guid: ...}
				if (startLine.TrimStart().StartsWith("- " + propertyName + ":"))
				{
					// Ищем строку m_Texture на следующих 3 строках
					for (int j = 1; j <= 3 && (i + j) < lines.Length; j++)
					{
						string subLine = lines[i + j];
						if (subLine.Contains("m_Texture:"))
						{
							guidLineIndex = i + j;
							guidData = ParseIdAndGuid(subLine);
							return new(guidData.fileID, guidData.guid, assetPath, guidLineIndex);
						}
					}
				}
			}

			// ОБЩЕЕ: Прямой поиск свойства (для m_Shader или обычных скриптов)
			int startIndex = 0;
			string yamlContext = startLine;
			while (!yamlContext.TrimStart().StartsWith(propertyName + ":"))
			{
				startIndex++;
				guidLineIndex = i + startIndex;
				if (guidLineIndex >= lines.Length)
				{
					yamlContext = string.Empty;
					break;
				}
				yamlContext = lines[guidLineIndex];
				if (yamlContext.StartsWith("--- "))
				{
					yamlContext = string.Empty;
					break;
				}
			}
			if (!string.IsNullOrEmpty(yamlContext))
			{
				guidData = ParseIdAndGuid(yamlContext);
				return new(guidData.fileID, guidData.guid, assetPath, guidLineIndex);
			}		
		}
		return nullValues;
	}

	public static (long fileID, string guid) GetMissingGuidFromAnyYaml(string lineObjName, int componentIndex, string propertyPath, string[] linesCustom)
	{
		var nullValues = (0, string.Empty);

		int lineObjNameIndex = linesCustom.IndexOf(lineObjName);
		string m_Component = lineObjName;
		while (m_Component.TrimStart() != "m_Component:")
		{
			lineObjNameIndex--;
			m_Component = linesCustom[lineObjNameIndex];
			if (m_Component.StartsWith("--- "))
			{
				m_Component = string.Empty;
				break;
			}
		}
		if (!string.IsNullOrEmpty(m_Component))
		{
			var componentLine = linesCustom[lineObjNameIndex + 1 + componentIndex];
			var pattern = @"fileID:\s*(\d+)";
			Match match = Regex.Match(componentLine, pattern);
			long targetLocalId = 0;
			if (match.Success)
			{
				long.TryParse(match.Groups[1].Value, out targetLocalId);
			}
			if (targetLocalId == 0)
			{
				return nullValues;
			}
			string headerPattern = $@"&{targetLocalId}\s*$";
			var startLine = linesCustom.FirstOrDefault(x => x.StartsWith("--- ") && Regex.IsMatch(x, headerPattern));
			if (!string.IsNullOrEmpty(startLine))
			{
				var i = linesCustom.IndexOf(startLine);
				int guidLineIndex = i;

				int startIndex = 0;
				string yamlContext = startLine;
				while (!yamlContext.TrimStart().StartsWith(propertyPath + ":"))
				{
					startIndex++;
					guidLineIndex = i + startIndex;
					if (guidLineIndex >= linesCustom.Length)
					{
						yamlContext = string.Empty;
						break;
					}
					yamlContext = linesCustom[guidLineIndex];
					if (yamlContext.StartsWith("--- "))
					{
						yamlContext = string.Empty;
						break;
					}
				}
				if (!string.IsNullOrEmpty(yamlContext))
				{
					var (customFileID, customGuid) = ParseIdAndGuid(yamlContext);
					if (!string.IsNullOrEmpty(customGuid))
					{
						return (customFileID, customGuid);
					}
				}
			}
		}
		return nullValues;
	}

	private static PropertyModification[] GetPrefabModifications(Object targetObject) 
	{
		GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(targetObject);
		if (root == null) return null;

		// Получаем все модификации свойств этого префаба на сцене
		return PrefabUtility.GetPropertyModifications(root);
	}

	private static bool IsLegacyPropertyOverridden(Object targetObject, string propertyPath)
	{
		var modifications = GetPrefabModifications(targetObject);
		if (modifications == null) return false;
		foreach (var mod in modifications)
		{
			// Если цель модификации — наш объект, и путь к свойству совпадает
			if (mod.target == targetObject && mod.propertyPath == propertyPath)
			{
				return true;
			}
		}
		return false;
	}

	private static long GetLocalIdViaDebugMode(Object obj)
	{
		if (obj == null) return 0;

		SerializedObject so = new SerializedObject(obj);
		var inspectorModeProperty = typeof(SerializedObject).GetProperty(
			"inspectorMode",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
		);

		if (inspectorModeProperty != null)
		{
			inspectorModeProperty.SetValue(so, InspectorMode.Debug, null);
			SerializedProperty localIdProp = so.FindProperty("m_LocalIdentfierInFile"); // Опечатка Unity!
			if (localIdProp != null)
			{
				return localIdProp.longValue;
			}
		}
		return 0;
	}

	public static (long fileID, string guid) ParseIdAndGuid(string yamlLine)
	{
		Match fileIdMatch = Regex.Match(yamlLine, @"fileID:\s*(-?[0-9]+)", RegexOptions.IgnoreCase);
		Match guidMatch = Regex.Match(yamlLine, @"guid:\s*([a-fA-F0-9]{32})", RegexOptions.IgnoreCase);

		long fileID = 0;
		string guid = string.Empty;

		if (fileIdMatch.Success) long.TryParse(fileIdMatch.Groups[1].Value, out fileID);
		if (guidMatch.Success) guid = guidMatch.Groups[1].Value;

		return (fileID, guid);
	}

	// Вспомогательный метод для старых версий Unity, если objectReferenceObjectValue пустой
	private static Object GetObjectFromPropertyID(SerializedProperty prop)
	{
		if (prop == null) return null;
		var field = typeof(SerializedProperty).GetField("m_SerializedObject",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (field != null)
		{
			var so = field.GetValue(prop) as SerializedObject;
			if (so != null) return so.targetObject;
		}
		return null;
	}
}
#endif