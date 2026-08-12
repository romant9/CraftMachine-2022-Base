#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class CustomAssetsReplacer : EditorWindow
{
	private CustomAssetsData configData;
	private List<MissingReferencesFinder.MissingReferenceInfo> missingReferences = new List<MissingReferencesFinder.MissingReferenceInfo>();
	private Dictionary<string, List<int>> missingReferencesIndexes = new();
	private Vector2 scrollPosition;

	private int customAssetsDataMappingCount;
	private int baseAssetsDataMappingCount;
	private int dllDataIDDictCount;
	private string absPath;
	private bool isJoinMissingRefs;
	private Object sourceObject;

	private bool _isProcessing = false;
	private bool _isProcessingFindObjectsStart = false;
	private bool _isProcessingFindObjects = false;

	private float _progress = 0f;
	private string _progressText = "";
	private bool doUpdateText;

	private CancellationTokenSource _cts;

	private enum OperationAsync
	{
		Parse,
		Fix
	}

	[MenuItem("Tools/Missing References Finder")]
	public static void ShowWindow()
	{
		GetWindow<CustomAssetsReplacer>("Missing Finder");
	}

	private void OnGUI()
	{
		GUILayout.Label("Поиск и исправление битых ссылок (по GUID)", EditorStyles.boldLabel);

		EditorGUI.BeginChangeCheck();

		configData = Resources.Load<CustomAssetsData>("Config/CustomAssetsData");

		configData = (CustomAssetsData)EditorGUILayout.ObjectField("Конфиг данных (Data)", configData, typeof(CustomAssetsData), false);

		if (configData != null)
		{
			if (GUILayout.Button("Обновить карты ассетов другого проекта (External Mapping)"))
			{
				configData.RefreshExternalMapping();
			}
			if (GUILayout.Button("Обновить карты ассетов этого проекта (Base Mapping)"))
			{
				configData.RefreshBaseMapping();
			}
			EditorGUILayout.Space();

			if (GUILayout.Button("Загрузить карты ассетов другого проекта (External Mapping)"))
			{
				configData.LoadExternalMapping();
			}
			if (GUILayout.Button("Загрузить карты ассетов этого проекта (Base Mapping)"))
			{
				configData.LoadBaseMapping();
			}
			if (GUILayout.Button("Загрузить карты dllData для скриптов (Dll Mapping)"))
			{
				configData.LoadDllMapping();
			}

			if (EditorGUI.EndChangeCheck() || doUpdateText)
			{
				doUpdateText = false;
				absPath = configData.GetAbsoluteExternalPath();
				customAssetsDataMappingCount = configData.customAssetsDataMapping.Count;
				baseAssetsDataMappingCount = configData.baseAssetsDataMapping.Count;
				dllDataIDDictCount = configData.DllDataIDDict.Count;
				//EditorGUILayout.HelpBox($"Внешних файлов: {configData.customAssetsDataMapping.Count}\n Локальных файлов проекта: {configData.baseAssetsDataMapping.Count}\nБиблиотек: {configData.DllDataIDDict.Count}", MessageType.Info);
			}
			EditorGUILayout.LabelField($"Внешний путь: {absPath}", EditorStyles.miniLabel);
			EditorGUILayout.LabelField($"Внешних файлов: {customAssetsDataMappingCount}", EditorStyles.miniLabel);
			EditorGUILayout.LabelField($"Локальных файлов проекта: {baseAssetsDataMappingCount}", EditorStyles.miniLabel);
			EditorGUILayout.LabelField($"Библиотек: {dllDataIDDictCount}", EditorStyles.miniLabel);
			EditorGUILayout.LabelField($"Битых ссылок: {missingReferences.Count}", EditorStyles.miniLabel);
		}

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Выбрать исходник", GUILayout.Height(30), GUILayout.Width(120)))
		{
			if (sourceObject != null)
			{
				NavigateToObject(sourceObject);
				Debug.Log($"Объект {sourceObject.name} выбран!");
			}
			else
			{
				EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, выберите объект в проекте или на сцене.", "ОК");
			}
		}

		if (GUILayout.Button("Get Object Info", GUILayout.Height(30), GUILayout.Width(120)))
		{
			Object activeObject = Selection.activeObject;
			bool isSingle = true;
			
			if (activeObject != null)
			{
				if (activeObject is SceneAsset)
				{
					isSingle = true;				
				}
				else
				{
					var activeObjectPath = AssetDatabase.GetAssetPath(activeObject);
					isSingle = !string.IsNullOrEmpty(activeObjectPath);
				}

				if (isSingle)
				{
					GlobalObjectId id = GlobalObjectId.GetGlobalObjectIdSlow(activeObject);

					string sceneGuid = id.assetGUID.ToString();
					ulong localFileID = id.targetObjectId;

					//AssetDatabase.TryGetGUIDAndLocalFileIdentifier(activeObject, out sceneGuid, out long fileID);
					//long currentFileId = Unsupported.GetLocalIdentifierInFile(activeObject.GetInstanceID());

					Debug.Log($"Guid {sceneGuid} | LocalFileID: {localFileID}");
				}
				else if (activeObject is GameObject go)
				{
					Component[] components = go.GetComponents<Component>();
					int compIndex = 0;
					foreach (var comp in components)
					{
						if (comp == null || comp is Transform) continue;

						GlobalObjectId id = GlobalObjectId.GetGlobalObjectIdSlow(comp);

						string sceneGuid = id.assetGUID.ToString();
						ulong localFileID = id.targetObjectId;
						long fileID = activeObject.GetInstanceID();
						Debug.Log($"Type: {comp.GetType().Name} | Guid {sceneGuid} | LocalFileID: {localFileID}");

						compIndex++;
					}
				}
			}
			else
			{
				EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, выберите объект в проекте или на сцене.", "ОК");
			}
		}
		GUILayout.FlexibleSpace();

		// Блокируем кнопку во время работы
		GUI.enabled = !_isProcessing;
		if (GUILayout.Button($"Найти битые ссылки в {(Selection.activeObject ? Selection.activeObject.name : "")}", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
		{
			Object activeObject = Selection.activeObject;
			if (activeObject != null)
			{
				isJoinMissingRefs = false;
				missingReferencesIndexes.Clear();
				sourceObject = activeObject;

				MissingReferencesFinder.SetConfigData(configData);
				string assetPath = MissingReferencesFinder.GetAssetPath(activeObject);

				if (!string.IsNullOrEmpty(assetPath) && File.Exists(assetPath))
				{
					_ = StartOperationAsync(OperationAsync.Parse, activeObject, assetPath);
				}
				else
				{
					EditorUtility.DisplayDialog("Ошибка", "Не удалось найти assetPath объекта.", "ОК");
				}
			}
			else
			{
				EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, выберите объект в проекте или на сцене.", "ОК");
			}
		}
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();
		// Отрисовка прогрессбара прямо в окне EditorWindow
		if (_isProcessing)
		{
			if (GUILayout.Button("Отмена", GUILayout.Height(30), GUILayout.Width(100)))
			{
				_cts?.Cancel();
			}
			Rect rect = GUILayoutUtility.GetRect(18, 18, GUILayout.ExpandWidth(true));
			EditorGUI.ProgressBar(rect, _progress, _progressText);

			// Принудительно перерисовываем окно, чтобы прогресс обновлялся плавно
			Repaint();
		}
		// Отрисовка отдельно
		// EditorUtility.DisplayProgressBar("Заголовок", _progressText, _progress);

		EditorGUILayout.Space();

		//if (_isProcessingFindObjectsStart && !_isProcessing)
		//{
		//	_isProcessingFindObjectsStart = false;
		//	if (sourceObject is SceneAsset)
		//	{
		//		FindGameObjectsByFileIds(sourceObject);
		//	}
		//}

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
		if (missingReferences.Count > 0 && !_isProcessingFindObjects)
		{
			if (isJoinMissingRefs)
			{
				if (missingReferencesIndexes.Count == 0)
				{
					missingReferencesIndexes = new();
					foreach (var refItem in missingReferences)
					{
						var refIndex = missingReferences.IndexOf(refItem);
						if (!missingReferencesIndexes.ContainsKey(refItem.objectGuid))
						{
							var listIndexes = new List<int> { refIndex };
							missingReferencesIndexes.Add(refItem.objectGuid, listIndexes);
						}
						else
						{
							missingReferencesIndexes[refItem.objectGuid].Add(refIndex);
						}
					}
				}
				foreach (var keyValue in missingReferencesIndexes)
				{
					var indexCount = keyValue.Value.Count;

					var refItems = missingReferences.Where(x => x.objectGuid == keyValue.Key).ToList();
					var refItem = refItems[0];

					EditorGUILayout.BeginVertical(EditorStyles.helpBox);
					refItem.OverrideRefObject = (Object)EditorGUILayout.ObjectField("Переназначить", refItem.OverrideRefObject, typeof(Object), false);

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField($"Объект: {refItem.targetObject.name}", EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Select", GUILayout.Width(50)))
					{
						NavigateToObject(refItem.targetObject);
						Debug.Log($"Объект {refItem.targetObject.name} выбран!");
					}
					if (GUILayout.Button("Copy", GUILayout.Width(40)))
					{
						CopyToClipboard(string.Join(", ", refItems.Select(x => x.targetObject.name)));
						Debug.Log("Имя объекта скопировано!");
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.LabelField($"Компонент: {refItem.componentName}");
					EditorGUILayout.LabelField($"Свойство: {refItem.propertyPath}");

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField($"GUID ссылка: {refItem.objectGuid}", EditorStyles.miniLabel, GUILayout.ExpandWidth(true));
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Copy", GUILayout.Width(40)))
					{
						CopyToClipboard(refItem.objectGuid);
						Debug.Log("GUID скопирован!");
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.LabelField($"FileID ссылка: {refItem.fileID}", EditorStyles.miniLabel);
					EditorGUILayout.LabelField($"Line indexes: {string.Join(", ", refItems.Select(x => x.lineIndex))}", EditorStyles.miniLabel);
					EditorGUILayout.LabelField($"Всего элементов: {indexCount}", EditorStyles.miniLabel);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space(2);
				}
			}
			else
			{
				for (int i = 0; i < missingReferences.Count; i++)
				{
					var refItem = missingReferences[i];
					EditorGUILayout.BeginVertical(EditorStyles.helpBox);
					refItem.OverrideRefObject = (Object)EditorGUILayout.ObjectField("Переназначить", refItem.OverrideRefObject, typeof(Object), false);

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField($"Объект: {refItem.targetObject.name}", EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Select", GUILayout.Width(50)))
					{
						NavigateToObject(refItem.targetObject);
						Debug.Log($"Объект {refItem.targetObject.name} выбран!");
					}
					if (GUILayout.Button("Copy", GUILayout.Width(40)))
					{
						CopyToClipboard(refItem.targetObject.name);
						Debug.Log("Имя объекта скопировано!");
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.LabelField($"Компонент: {refItem.componentName}");
					EditorGUILayout.LabelField($"Свойство: {refItem.propertyPath}");

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField($"GUID ссылка: {refItem.objectGuid}", EditorStyles.miniLabel, GUILayout.ExpandWidth(true));
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Copy", GUILayout.Width(40)))
					{
						CopyToClipboard(refItem.objectGuid);
						Debug.Log("GUID скопирован!");
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.LabelField($"FileID ссылка: {refItem.fileID}", EditorStyles.miniLabel);
					EditorGUILayout.LabelField($"Line index: {refItem.lineIndex}", EditorStyles.miniLabel);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space(2);
				}
			}		
		}
		else
		{
			GUILayout.Label("Битых ссылок не обнаружено или сканирование не проводилось.");
		}
		EditorGUILayout.EndScrollView();

		// Кнопка фикса
		// EditorGUI.BeginDisabledGroup(configData == null || missingReferences.Count == 0 || configData.customAssetsDataMapping.Count == 0);
		EditorGUILayout.BeginHorizontal();
		GUI.enabled = !(configData == null || missingReferences.Count == 0 || configData.customAssetsDataMapping.Count == 0);
		isJoinMissingRefs = GUILayout.Toggle(isJoinMissingRefs, "Объединить по guid", "Button", GUILayout.ExpandWidth(true), GUILayout.Height(30));
		GUI.enabled = true;
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Свернуть иерархию", GUILayout.Height(30)))
		{
			CollapseHierarchy();
		}
		GUILayout.FlexibleSpace();
		GUI.enabled = !_isProcessing;
		if (GUILayout.Button("Fix Missing", GUILayout.Height(30)))
		{
			string assetPath = MissingReferencesFinder.GetAssetPath(sourceObject);
			_ = StartOperationAsync(OperationAsync.Fix, sourceObject, assetPath);
		}
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();
		// EditorGUI.EndDisabledGroup();
	}

	private async Task StartOperationAsync(OperationAsync operation, Object activeObject, string assetPath)
	{
		_isProcessing = true;
		_progress = 0f;
		_progressText = "Инициализация...";
		_cts = new CancellationTokenSource();
		(bool success, string message) result = (false, "");

		Repaint();
		try
		{
			if (operation == OperationAsync.Parse)
			{
				missingReferences = await FindReferencesAsync(activeObject, assetPath, _cts.Token);

				Debug.Log("Поиск завершен успешно!");
				Debug.Log($"Найдено битых ссылок: {missingReferences.Count}");
			}
			else
			{
				result = await FixReferenceAsync(assetPath, _cts.Token);
				Debug.Log("Фикс завершен успешно!");
			}
		}
		catch (OperationCanceledException)
		{
			Debug.LogWarning("Операция была отменена пользователем.");
		}
		catch (Exception ex)
		{
			Debug.LogError($"Ошибка при парсинге: {ex.Message}\n{ex.StackTrace}");
		}
		finally
		{
			_cts?.Dispose();
			_cts = null;
			_progress = 0f;
			_progressText = "";
			_isProcessing = false;
			if (operation == OperationAsync.Parse)
			{
				FindGameObjectsByFileIds(sourceObject);

				//_isProcessingFindObjectsStart = true;
			}
			else
			{
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog(result.success ? "Успех" : "Неудача", result.message, "ОК");
			}
			doUpdateText = true;
			EditorUtility.ClearProgressBar();
			Repaint();
		}
	}

	private async Task<List<MissingReferencesFinder.MissingReferenceInfo>> FindReferencesAsync(Object activeObject, string assetPath, CancellationToken token)
	{
		// Читаем файл асинхронно
		string[] lines = !string.IsNullOrEmpty(assetPath) && File.Exists(assetPath) ? await Task.Run(() => File.ReadAllLines(assetPath)) : null;
		int totalLines = lines != null ? lines.Length : 1;

		// Создаем прогресс-индикатор в панели Progress Unity (в правом нижнем углу)
		int progressId = Progress.Start("Поиск битых ссылок", "Анализ файла...");

		// Настраиваем callback для обновления прогресса из фонового потока
		IProgress<(int current, string text)> progressHandler = new Progress<(int current, string text)>(data =>
		{
			// Этот код выполняется в Главном Потоке (Main Thread)
			_progress = (float)data.current / totalLines;
			_progressText = data.text;

			// Обновляем глобальный прогрессбар Unity внизу экрана
			Progress.Report(progressId, _progress, data.text);
			Repaint();
		});

		// Запускаем парсинг в фоновом потоке
		List<MissingReferencesFinder.MissingReferenceInfo> missingReferences = await Task.Run(() =>
		
			MissingReferencesFinder.FindMissingReferences(activeObject, assetPath, lines, progressHandler, token)
		);

		Progress.Remove(progressId);
		Repaint();
		return missingReferences;
	}

	private void OnDestroy()
	{
		// Если окно закрыли, а процесс идет — отменяем его
		if (_isProcessing)
		{
			_cts?.Cancel();
		}
	}

	// Принудительно фокусируем Hierarchy и камеру сцены на объекте
	//EditorApplication.delayCall += () =>
	//{
	//	SceneView.FrameLastActiveSceneView();
	//};

	private static void CollapseHierarchyInternal()
	{
		// Выполняет встроенную команду редактора для сворачивания всех строк
		// 1. Находим окно Hierarchy
		Type windowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
		EditorWindow hierarchyWindow = EditorWindow.GetWindow(windowType, false, null, false);

		if (hierarchyWindow == null) return;

		// 2. Добираемся до свойства sceneHierarchy (современный API Unity)
		PropertyInfo hierarchyProperty = windowType.GetProperty("sceneHierarchy",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		if (hierarchyProperty != null)
		{
			object sceneHierarchyValue = hierarchyProperty.GetValue(hierarchyWindow);
			if (sceneHierarchyValue != null)
			{
				// В Unity 2022+ у объекта sceneHierarchy появился прямой метод CollapseAll() без аргументов
				MethodInfo collapseMethod = sceneHierarchyValue.GetType().GetMethod("CollapseAll",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

				if (collapseMethod != null)
				{
					collapseMethod.Invoke(sceneHierarchyValue, null);
					hierarchyWindow.Repaint();
					return;
				}

				// Резервный вариант для Unity 2020/2021 (если метод требует bool)
				MethodInfo setExpandedAllMethod = sceneHierarchyValue.GetType().GetMethod("SetExpandedAll",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(bool) }, null);

				if (setExpandedAllMethod != null)
				{
					setExpandedAllMethod.Invoke(sceneHierarchyValue, new object[] { false });
					hierarchyWindow.Repaint();
					return;
				}
			}
		}

		// 3. Если старая версия Unity: дергаем CollapseAll напрямую из самого окна
		MethodInfo oldCollapse = windowType.GetMethod("CollapseAll", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		if (oldCollapse != null)
		{
			oldCollapse.Invoke(hierarchyWindow, null);
			hierarchyWindow.Repaint();
		}
	}

	private static void NavigateToObject(Object obj)
	{
		// Проверяем, является ли объект компонентом (сводим к GameObject)
		if (obj is Component component)
		{
			obj = component.gameObject;
		}

		if (obj is GameObject gameObject)
		{
			// Получаем путь к ассету, на который кликнул пользователь
			string targetAssetPath = AssetDatabase.GetAssetPath(gameObject);

			// Получаем текущую открытую сцену префаба (если она открыта)
			PrefabStage currentStage = PrefabStageUtility.GetCurrentPrefabStage();

			// ПРОВЕРКА 1: Объект находится на ОБЫЧНОЙ сцене (не префаб)
			if (gameObject.scene.IsValid() && string.IsNullOrEmpty(targetAssetPath))
			{
				Selection.activeGameObject = gameObject;
				EditorGUIUtility.PingObject(gameObject);
				FocusSceneAndHierarchy(gameObject);
				return;
			}

			// ПРОВЕРКА 2: Пользователь кликнул по префабу, И какой-то префаб сейчас уже открыт
			if (currentStage != null && !string.IsNullOrEmpty(targetAssetPath))
			{
				// Сравниваем путь открытого префаба и целевого префаба
				if (currentStage.assetPath == targetAssetPath)
				{
					// Находим точную копию этого объекта внутри открытой сцены префаба
					GameObject objectInStage = FindMatchingObjectInStage(currentStage.prefabContentsRoot, gameObject);

					if (objectInStage != null)
					{
						Selection.activeGameObject = objectInStage;
						EditorGUIUtility.PingObject(objectInStage);
						FocusSceneAndHierarchy(objectInStage);
						return;
					}
				}
			}

			// ПРОВЕРКА 3: Префаб закрыт, либо открыт совершенно другой префаб
			if (!string.IsNullOrEmpty(targetAssetPath))
			{
				if (currentStage == null && targetAssetPath.EndsWith(".prefab"))
				{
					var assetNameWithExt = targetAssetPath.Split('/').Last();
					if (assetNameWithExt == gameObject.name + ".prefab")
					{
						Selection.activeObject = obj;
						EditorGUIUtility.PingObject(obj);
						return;
					}
				}
				// Открываем префаб
				AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(targetAssetPath));

				// Ждем один кадр, пока Unity загрузит Prefab Mode
				EditorApplication.delayCall += () =>
				{
					PrefabStage newStage = PrefabStageUtility.GetCurrentPrefabStage();
					if (newStage != null)
					{
						GameObject objectInStage = FindMatchingObjectInStage(newStage.prefabContentsRoot, gameObject);
						if (objectInStage != null)
						{
							Selection.activeGameObject = objectInStage;
							EditorGUIUtility.PingObject(objectInStage);
							FocusSceneAndHierarchy(objectInStage);
						}
					}
				};
			}
		}
		// ПРОВЕРКА 4: Любой другой тип ассета в проекте (Материал, Текстура)
		else
		{
			Selection.activeObject = obj;
			EditorGUIUtility.PingObject(obj);
		}
	}

	/// <summary>
	/// Находит копию объекта ассета внутри временно созданной сцены префаба,
	/// используя относительный путь трансформаций.
	/// </summary>
	private static GameObject FindMatchingObjectInStage(GameObject stageRoot, GameObject assetTarget)
	{
		// Строим путь от целевого объекта до его корня в ассете
		System.Collections.Generic.List<string> pathParts = new System.Collections.Generic.List<string>();
		Transform current = assetTarget.transform;

		while (current != null && current.parent != null) // Идем до самого верхнего родителя в префабе
		{
			pathParts.Insert(0, current.name);
			current = current.parent;
		}

		// Если объект сам является корнем префаба
		if (pathParts.Count == 0) return stageRoot;

		// Спускаемся по такому же пути внутри открытого Prefab Stage
		Transform foundTransform = stageRoot.transform.Find(string.Join("/", pathParts));
		return foundTransform != null ? foundTransform.gameObject : null;
	}

	/// <summary>
	/// Объединенный метод для раскрытия Hierarchy и фокусировки камеры
	/// </summary>
	private static void FocusSceneAndHierarchy(GameObject target)
	{
		EditorApplication.delayCall += () =>
		{
			ExpandParentsInHierarchy(target);
			if (SceneView.lastActiveSceneView != null)
			{
				SceneView.lastActiveSceneView.FrameSelected();
			}
		};
	}

	private static void CollapseHierarchy()
	{
		EditorApplication.delayCall += () =>
		{
			CollapseHierarchyInternal();
		};
	}

	/// <summary>
	/// Рефлексивный метод для раскрытия дерева Hierarchy до нужного объекта
	/// </summary>
	private static void ExpandParentsInHierarchy(GameObject target)
	{
		if (target == null) return;

		// Находим внутренний тип окна Hierarchy в Unity
		var hierarchyWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
		if (hierarchyWindowType == null) return;

		// Получаем метод раскрытия нод
		MethodInfo setExpandedMethod = hierarchyWindowType.GetMethod("SetExpandedRecursive",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		if (setExpandedMethod == null) return;

		// Ищем все открытые окна Hierarchy в редакторе
		UnityEngine.Object[] windows = Resources.FindObjectsOfTypeAll(hierarchyWindowType);

		foreach (var window in windows)
		{
			Transform currentParent = target.transform.parent;

			// Идем вверх по дереву трансформаций и раскрываем каждого родителя
			while (currentParent != null)
			{
				try
				{
					// Вызываем: window.SetExpandedRecursive(instanceID, true)
					setExpandedMethod.Invoke(window, new object[] { currentParent.gameObject.GetInstanceID(), true });
				}
				catch
				{
					// Игнорируем ошибки вызова внутренней кухни Unity
				}
				currentParent = currentParent.parent;
			}
		}
	}

	private async Task<(bool, string)> FixReferenceAsync(string assetPath, CancellationToken token)
	{
		// Читаем файл асинхронно
		string[] lines = await Task.Run(() => File.ReadAllLines(assetPath));
		int totalLines = lines.Length;

		// Создаем прогресс-индикатор в панели Progress Unity (в правом нижнем углу)
		int progressId = Progress.Start("Поиск битых ссылок", "Анализ файла...");

		// Настраиваем callback для обновления прогресса из фонового потока
		IProgress<(int current, string text)> progressHandler = new Progress<(int current, string text)>(data =>
		{
			// Этот код выполняется в Главном Потоке (Main Thread)
			_progress = (float)data.current / totalLines;
			_progressText = data.text;

			// Обновляем глобальный прогрессбар Unity внизу экрана
			Progress.Report(progressId, _progress, data.text);
			Repaint();
		});

		// Запускаем парсинг в фоновом потоке
		(bool success, string message) result = await Task.Run(() => FixMissingAssets(progressHandler, token));

		Progress.Remove(progressId);
		return result;
	}

	private (bool success, string message) FixMissingAssets(IProgress<(int current, string text)> progressHandler, CancellationToken token)
	{
		int reassignedCountCurrentPath = 0;
		int refIndex = -1;
		int total = missingReferences.Count;
		int copiedCount = 0;
		int reassignedCount = 0;
		int errorCount = 0;
		string targetBaseDir = "Assets/RecoveredAssets/";
		string absoluteExternalPath = configData.GetAbsoluteExternalPath();

		if (!Directory.Exists(targetBaseDir))
		{
			Directory.CreateDirectory(targetBaseDir);
		}

		var assetPath = missingReferences.First().assetPath;
		var lines = File.ReadAllLines(assetPath);

		foreach (var missingRef in missingReferences)
		{
			refIndex++;
			token.ThrowIfCancellationRequested();

			bool isPossible = false;
			string overrideRefObjectAssetPath = string.Empty;
			string overrideRefObjectGuid = string.Empty;
			long overrideRefObjectFileID = 0;
			Object overrideRefObject = missingRef.OverrideRefObject;

			if (isJoinMissingRefs && missingRef.OverrideRefObject == null)
			{
				var refItems = missingReferences.Where(x => x.objectGuid == missingRef.objectGuid).ToList();
				if (refItems.Count > 0)
				{
					var refItem = refItems.FirstOrDefault(x => x.OverrideRefObject != null);
					if (refItem.OverrideRefObject)
					{
						overrideRefObject = refItem.OverrideRefObject;
					}
				}			
			}

			// ToDo: Переписать
			if (overrideRefObject != null)
			{
				SerializedObject so = new SerializedObject(overrideRefObject);
				SerializedProperty baseProp = so.GetIterator();

				if (baseProp != null)
				{
					bool isScript = baseProp.type == "m_Script";

					isPossible = isScript ? missingRef.expectedTypeName == "MonoBehaviour" : baseProp.type == missingRef.expectedTypeName;

					if (isPossible)
					{
						SerializedProperty guidProp = baseProp.FindPropertyRelative("m_Guid");
						if (guidProp != null)
						{
							overrideRefObjectGuid = guidProp.stringValue;
						}
						SerializedProperty fileIDProp = baseProp.FindPropertyRelative("m_FileID");
						if (fileIDProp != null)
						{
							overrideRefObjectFileID = fileIDProp.longValue;
						}
						if (assetPath != missingRef.assetPath)
						{
							assetPath = missingRef.assetPath;
							lines = File.ReadAllLines(assetPath);
						}
						var propertyLine = lines[missingRef.lineIndex];
						var (fileID, guid) = MissingReferencesFinder.ParseIdAndGuid(propertyLine);

						if (!string.IsNullOrEmpty(overrideRefObjectGuid))
						{
							propertyLine = propertyLine.Replace(guid, overrideRefObjectGuid);
						}
						if (overrideRefObjectFileID != 0 && overrideRefObjectFileID != fileID)
						{
							propertyLine = propertyLine.Replace(fileID.ToString(), overrideRefObjectFileID.ToString());
						}

						lines[missingRef.lineIndex] = propertyLine;
						reassignedCount++;
						token.ThrowIfCancellationRequested();

						File.WriteAllLines(assetPath, lines);
					}
				}
			}		
			else
			{
				if (string.IsNullOrEmpty(missingRef.objectGuid)) continue;

				AssetMapping match = new();
				string fileName;
				AssetMapping fileAwailable = new();
				if (missingRef.propertyPath == "m_Script" && configData.DllDataIDDict.Count > 0 && configData.DllDataIDDict.ContainsKey(missingRef.objectGuid))
				{
					var dict = configData.DllDataIDDict[missingRef.objectGuid];
					var resourceItem = dict.Values.ToList().Find(x => x.DllFileId == missingRef.fileID);
					if (resourceItem != null)
					{
						fileName = resourceItem.Name + resourceItem.Extention;
						fileAwailable = configData.baseAssetsDataMapping.Find(n => n.fileName == fileName);
					}
				}
				else
				{
					match = configData.customAssetsDataMapping.Find(m => m.guid == missingRef.objectGuid);
					if (!string.IsNullOrEmpty(match.guid))
					{
						fileName = match.fileName;

						if (fileName.EndsWith(".prefab") && missingRef.propertyPath == "mAtlas")
						{
							fileName = fileName.Replace(".prefab", ".asset");
							fileAwailable = configData.baseAssetsDataMapping.Find(n => n.fileName == fileName);
							if (string.IsNullOrEmpty(fileAwailable.guid))
							{
								var fileNameExtract = Path.GetFileNameWithoutExtension(fileName);
								if (fileNameExtract.EndsWith("_HD"))
								{
									fileNameExtract = fileNameExtract.Replace("_HD", "");
								}
								else
								{
									fileNameExtract += "_HD";
								}
								fileName = fileNameExtract + ".asset";
								fileAwailable = configData.baseAssetsDataMapping.Find(n => n.fileName == fileName);
							}
						}
						else
						{
							fileAwailable = configData.baseAssetsDataMapping.Find(n => n.fileName == fileName);
						}
					}				
				}

				if (string.IsNullOrEmpty(fileAwailable.guid))
				{
					var baseAssetFileName = missingRef.assetPath.Split('/').Last();
					var customAssets = configData.customAssetsDataMapping.Where(m => m.fileName == baseAssetFileName);
					foreach (var customAsset in customAssets)
					{
						if (!string.IsNullOrEmpty(customAsset.fileName))
						{
							var customAssetAbsPath = configData.externalFolderPath + customAsset.externalRelativePath;
							var linesCustom = File.ReadAllLines(customAssetAbsPath);
							var customObjName = missingRef.targetObjectName;

							(long customFileID, string customGuid) customData = (0L, "");
							if (missingRef.expectedTypeName == "Material")
							{
								// Ищем шейдер
								if (missingRef.propertyPath == "m_Shader")
								{
									var lineShader = linesCustom.FirstOrDefault(x => x.TrimStart().StartsWith("m_Shader"));
									if (!string.IsNullOrEmpty(lineShader))
									{
										customData = MissingReferencesFinder.ParseIdAndGuid(lineShader);
									}
								}
								// Ищем текстуры
								else
								{
									var linesTex = linesCustom.Where(x => x.TrimStart().StartsWith("m_Texture:") && x.Contains("guid:"))?.ToList();
									if (linesTex != null && missingRef.componentIndex < linesTex.Count)
									{
										customData = MissingReferencesFinder.ParseIdAndGuid(linesTex[missingRef.componentIndex]);
									}
								}
								//if (missingRef.lineIndex < linesCustom.Length)
								//{
								//	customData = MissingReferencesFinder.ParseIdAndGuid(linesCustom[missingRef.lineIndex]);
								//}
							}
							else
							{
								var lineObjName = linesCustom.FirstOrDefault(x => x.TrimStart() == "m_Name: " + customObjName);
								if (!string.IsNullOrEmpty(lineObjName))
								{
									customData = MissingReferencesFinder.GetMissingGuidFromAnyYaml(lineObjName, missingRef.componentIndex, missingRef.propertyPath, linesCustom);
								}
							}
							if (!string.IsNullOrEmpty(customData.customGuid))
							{
								match = configData.customAssetsDataMapping.Find(m => m.guid == customData.customGuid);
								if (!string.IsNullOrEmpty(match.guid))
								{
									fileAwailable = configData.baseAssetsDataMapping.Find(n => n.fileName == match.fileName);
									break;
								}
							}
						}
					}				
				}

				// Случай 1: Файл с таким же именем уже есть в проекте (подменяем GUID)
				if (!string.IsNullOrEmpty(fileAwailable.guid))
				{
					token.ThrowIfCancellationRequested();

					if (assetPath != missingRef.assetPath)
					{
						if (reassignedCountCurrentPath != reassignedCount)
						{
							reassignedCountCurrentPath = reassignedCount;
							File.WriteAllLines(assetPath, lines);
						}
						assetPath = missingRef.assetPath;
						lines = File.ReadAllLines(assetPath);
					}

					var propertyLine = lines[missingRef.lineIndex];
					var (fileID, guid) = MissingReferencesFinder.ParseIdAndGuid(propertyLine);
					if (missingRef.propertyPath == "m_Script" && fileID.ToString() != "11500000")
					{
						propertyLine = propertyLine.Replace(fileID.ToString(), "11500000");
					}
					if (fileAwailable.fileName.EndsWith(".asset") && fileID.ToString() != "11400000")
					{
						propertyLine = propertyLine.Replace(fileID.ToString(), "11400000");
					}
					propertyLine = propertyLine.Replace(guid, fileAwailable.guid);
					lines[missingRef.lineIndex] = propertyLine;
					reassignedCount++;

					//File.WriteAllLines(assetPath, lines);
				}
				else
				{
					token.ThrowIfCancellationRequested();

					if (!string.IsNullOrEmpty(match.guid))
					{
						// Случай 2: Файла в проекте нет (копируем его из внешней папки)
						string fullSourcePath = Path.Combine(absoluteExternalPath, match.externalRelativePath);
						string targetFilePath = Path.Combine(targetBaseDir, match.externalRelativePath);
						string targetFileDir = Path.GetDirectoryName(targetFilePath);

						if (!Directory.Exists(targetFileDir))
						{
							Directory.CreateDirectory(targetFileDir);
						}

						if (File.Exists(fullSourcePath) && !File.Exists(targetFilePath))
						{
							File.Copy(fullSourcePath, targetFilePath, true);

							string sourceMeta = fullSourcePath + ".meta";
							string targetMeta = targetFilePath + ".meta";

							if (File.Exists(sourceMeta))
							{
								File.Copy(sourceMeta, targetMeta, true);
							}

							copiedCount++;
							Debug.Log($"[FixMissing] Файл восстановлен из внешней папки: {match.fileName} -> {targetFilePath}");
						}
					}
					else
					{
						Debug.LogWarning($"[FixMissing] Не найден внешний файл для GUID: {missingRef.objectGuid}");
						errorCount++;
					}
				}
			}
			// каждые 500 строк
			if (refIndex % 50 == 0 || refIndex == total - 1)
			{
				progressHandler?.Report((refIndex + 1, $"Строка {refIndex + 1} из {total}"));
			}
		}

		bool success = false;
		string resultMessage = "Никаких изменений не выполнено. Проверьте совпадения файлов.";
		if (copiedCount > 0 || reassignedCount > 0)
		{
			if (reassignedCount > 0)
			{
				File.WriteAllLines(assetPath, lines);
			}
			success = true;
			resultMessage = $"Переназначено локальных ссылок: {reassignedCount}.\nСкопировано новых файлов: {copiedCount}.\nНе найдено ссылок: {errorCount}";
			//EditorUtility.DisplayDialog("Успех", $"Переназначено локальных ссылок: {reassignedCount}.\nСкопировано новых файлов: {copiedCount}.\nНе найдено ссылок: {errorCount}", "ОК");
			missingReferences.Clear();
		}
		//else
		//{
		//	EditorUtility.DisplayDialog("Результат", "Никаких изменений не выполнено. Проверьте совпадения файлов.", "ОК");
		//}
		return (success, resultMessage);
	}

	private void FindGameObjectsByFileIds(Object scene)
	{
		_isProcessingFindObjects = true;

		var fileIds = missingReferences.Select(x => x.localFileID).ToList();
		if (fileIds == null || fileIds.Count == 0) 
		{
			_isProcessingFindObjects = false;
			return;
		}

		AssetDatabase.TryGetGUIDAndLocalFileIdentifier(scene, out string sceneGuid, out long fileID);
		if (string.IsNullOrEmpty(sceneGuid))
		{
			_isProcessingFindObjects = false;
			return;
		}
		int count = fileIds.Count;
		GlobalObjectId[] idsToFind = new GlobalObjectId[count];
		UnityEngine.Object[] outputObjects = new UnityEngine.Object[count];
		string typeIdentifier = scene is SceneAsset ? "2" : "1";

		// 1. Быстро формируем массив идентификаторов в памяти
		for (int i = 0; i < count; i++)
		{
			// Идентификатор типа 2 означает GameObject на сцене
			string assetIdString = $"GlobalObjectId_V1-{typeIdentifier}-{sceneGuid}-{fileIds[i]}-0";
			GlobalObjectId.TryParse(assetIdString, out idsToFind[i]);
		}

		// 2. ОДНИМ вызовом передаем весь массив в native-код Unity (это работает реактивно)
		GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(idsToFind, outputObjects);

		// 3. Собираем результат
		for (int i = 0; i < outputObjects.Length; i++)
		{
			var outputObject = outputObjects[i];
			if (outputObject == null) continue;
			var missingRef = missingReferences[i];
			missingRef.targetObject = outputObject;
			missingRef.targetObjectName = outputObject.name;
			missingRef.componentName = outputObject.GetType().Name;
			missingReferences[i] = missingRef;
		}
		_isProcessingFindObjects = false;
	}

	public static GameObject FindGameObjectByFileId(long fileId)
	{
		// Получаем активную сцену
		var activeScene = SceneManager.GetActiveScene();

		// Перебираем все корневые объекты на сцене
		foreach (GameObject rootGo in activeScene.GetRootGameObjects())
		{
			// Ищем нужный LocalID среди самого объекта и всех его дочерних элементов
			foreach (Transform t in rootGo.GetComponentsInChildren<Transform>(true))
			{
				GameObject go = t.gameObject;

				// Извлекаем внутренний Local Identifier объекта
				long currentFileId = Unsupported.GetLocalIdentifierInFile(go.GetInstanceID());

				if (currentFileId == fileId)
				{
					return go;
				}
			}
		}

		return null;
	}

	public static void CopyToClipboard(string text)
	{
		TextEditor te = new() { text = text.Trim() };
		te.SelectAll();
		te.Copy();
	}
}
#endif