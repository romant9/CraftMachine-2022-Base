#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetDependencyTracker : EditorWindow
{
	private enum Mode
	{
		Single,
		Compare,
		MissingLinks,
	}

	private enum SearchScope
	{
		WholeProject,
		SelectedAsset,
		SelectedFolder,
	}

	private Mode currentMode = Mode.Single;
	private SearchScope currentScope = SearchScope.WholeProject;

	// Списки для Одиночного режима и Сравнения
	private Object selectedAssetA;
	private List<string> dependenciesA = new List<string>();
	private List<string> referencesA = new List<string>();

	private Object selectedAssetB;
	private List<string> dependenciesB = new List<string>();
	private List<string> referencesB = new List<string>();

	// Поля для режима битых ссылок
	private Object selectedMissingTarget; // Сюда можно перетащить файл или папку
	private Dictionary<string,List<MissingReferencesFinder.MissingErrorInfo>> missingReferencesResult = new();
	private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

	private Vector2 scrollPos;

	[MenuItem("Tools/Asset Dependency Tracker")]
	public static void ShowWindow()
	{
		GetWindow<AssetDependencyTracker>("Asset Tracker");
	}

	private void OnGUI()
	{
		GUILayout.Label("Анализатор связей ассетов", EditorStyles.boldLabel);

		currentMode = (Mode)
			GUILayout.Toolbar(
				(int)currentMode,
				new string[] { "Одиночный", "Сравнение", "Битые ссылки (Missing)" }
			);
		GUILayout.Space(10);

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		switch (currentMode)
		{
			case Mode.Single:
				DrawSingleMode();
				break;
			case Mode.Compare:
				DrawCompareMode();
				break;
			case Mode.MissingLinks:
				DrawMissingLinksMode();
				break;
		}

		EditorGUILayout.EndScrollView();
	}

	private void DrawSingleMode()
	{
		if (GUILayout.Button("Взять выделенный в Project элемент"))
		{
			selectedAssetA = Selection.activeObject;
			AnalyzeAsset(selectedAssetA, ref dependenciesA, ref referencesA);
		}

		EditorGUI.BeginChangeCheck();
		selectedAssetA = EditorGUILayout.ObjectField(
			"Целевой объект",
			selectedAssetA,
			typeof(Object),
			false
		);
		if (EditorGUI.EndChangeCheck())
		{
			AnalyzeAsset(selectedAssetA, ref dependenciesA, ref referencesA);
		}

		if (selectedAssetA == null)
		{
			EditorGUILayout.HelpBox("Выберите элемент для анализа.", MessageType.Info);
			return;
		}

		GUILayout.Space(10);
		GUILayout.Label($"Зависимости ({dependenciesA.Count}):", EditorStyles.boldLabel);
		RenderAssetList(dependenciesA);

		GUILayout.Space(10);
		GUILayout.Label($"Ссылки ({referencesA.Count}):", EditorStyles.boldLabel);
		RenderAssetList(referencesA);
	}

	private void DrawCompareMode()
	{
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Объект А из выделенного"))
		{
			selectedAssetA = Selection.activeObject;
			AnalyzeAllCompare();
		}
		if (GUILayout.Button("Объект Б из выделенного"))
		{
			selectedAssetB = Selection.activeObject;
			AnalyzeAllCompare();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUI.BeginChangeCheck();
		selectedAssetA = EditorGUILayout.ObjectField(
			"Объект А",
			selectedAssetA,
			typeof(Object),
			false
		);
		selectedAssetB = EditorGUILayout.ObjectField(
			"Объект Б",
			selectedAssetB,
			typeof(Object),
			false
		);
		if (EditorGUI.EndChangeCheck())
		{
			AnalyzeAllCompare();
		}

		if (selectedAssetA == null || selectedAssetB == null)
		{
			EditorGUILayout.HelpBox("Выберите оба объекта для сравнения.", MessageType.Info);
			return;
		}

		var commonDeps = dependenciesA.Intersect(dependenciesB).ToList();
		var uniqueDepsA = dependenciesA.Except(commonDeps).ToList();
		var uniqueDepsB = dependenciesB.Except(commonDeps).ToList();

		var commonRefs = referencesA.Intersect(referencesB).ToList();
		var uniqueRefsA = referencesA.Except(commonRefs).ToList();
		var uniqueRefsB = referencesB.Except(commonRefs).ToList();

		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
		GUILayout.Label("СРАВНЕНИЕ ЗАВИСИМОСТЕЙ", EditorStyles.boldLabel);

		GUI.color = new Color(0.8f, 1f, 0.8f);
		GUILayout.Label($"Общие зависимости ({commonDeps.Count}):", EditorStyles.boldLabel);
		GUI.color = Color.white;
		RenderAssetList(commonDeps);

		GUILayout.Space(5);
		GUILayout.Label($"Только у Объекта А ({uniqueDepsA.Count}):", EditorStyles.boldLabel);
		RenderAssetList(uniqueDepsA);

		GUILayout.Space(5);
		GUILayout.Label($"Только у Объекта Б ({uniqueDepsB.Count}):", EditorStyles.boldLabel);
		RenderAssetList(uniqueDepsB);

		GUILayout.Space(15);
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
		GUILayout.Label("СРАВНЕНИЕ ССЫЛОК", EditorStyles.boldLabel);

		GUI.color = new Color(0.8f, 1f, 0.8f);
		GUILayout.Label($"Общие места использования ({commonRefs.Count}):", EditorStyles.boldLabel);
		GUI.color = Color.white;
		RenderAssetList(commonRefs);

		GUILayout.Space(5);
		GUILayout.Label(
			$"Используется только Объект А ({uniqueRefsA.Count}):",
			EditorStyles.boldLabel
		);
		RenderAssetList(uniqueRefsA);

		GUILayout.Space(5);
		GUILayout.Label(
			$"Используется только Объект Б ({uniqueRefsB.Count}):",
			EditorStyles.boldLabel
		);
		RenderAssetList(uniqueRefsB);
	}

	private void DrawMissingLinksMode()
	{
		EditorGUILayout.HelpBox(
			"Настройте область сканирования битых ссылок. Скрипт покажет проблемные файлы, а также поврежденные свойства внутри них.",
			MessageType.Info
		);

		// Выбор области поиска
		currentScope = (SearchScope)EditorGUILayout.EnumPopup("Область поиска", currentScope);

		// Отображение полей ввода в зависимости от выбранной области
		if (currentScope == SearchScope.SelectedAsset || currentScope == SearchScope.SelectedFolder)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Взять выделенное в Project"))
			{
				selectedMissingTarget = Selection.activeObject;
			}
			selectedMissingTarget = EditorGUILayout.ObjectField(
				"Целевой элемент",
				selectedMissingTarget,
				typeof(Object),
				false
			);
			EditorGUILayout.EndHorizontal();

			if (selectedMissingTarget == null)
			{
				EditorGUILayout.HelpBox(
					currentScope == SearchScope.SelectedAsset
						? "Укажите ассет для проверки."
						: "Укажите папку для проверки.",
					MessageType.Warning
				);
				return;
			}

			// Дополнительная валидация на то, является ли объект папкой
			string path = AssetDatabase.GetAssetPath(selectedMissingTarget);
			bool isDirectory = Directory.Exists(path);

			if (currentScope == SearchScope.SelectedFolder && !isDirectory)
			{
				EditorGUILayout.HelpBox(
					"Выбранный объект не является папкой! Перетащите папку из окна Project.",
					MessageType.Error
				);
				return;
			}
			if (currentScope == SearchScope.SelectedAsset && isDirectory)
			{
				EditorGUILayout.HelpBox(
					"Выбрана папка вместо одиночного ассета! Измените область поиска или выберите файл.",
					MessageType.Error
				);
				return;
			}
		}

		GUILayout.Space(5);

		if (GUILayout.Button("Запустить поиск битых ссылок", GUILayout.Height(30)))
		{
			ExecuteMissingLinksSearch();
		}

		GUILayout.Space(15);

		if (missingReferencesResult.Count == 0)
		{
			GUILayout.Label("   Битых ссылок не обнаружено.", EditorStyles.miniLabel);
			return;
		}

		GUI.color = new Color(1f, 0.6f, 0.6f);
		GUILayout.Label(
			$"Найденные проблемные ассеты ({missingReferencesResult.Count}):",
			EditorStyles.boldLabel
		);
		GUI.color = Color.white;

		// Отрисовка структуры Foldout
		foreach (var pair in missingReferencesResult)
		{
			string path = pair.Key;
			List<MissingReferencesFinder.MissingErrorInfo> errors = pair.Value;

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.BeginHorizontal();

			if (!foldoutStates.ContainsKey(path))
				foldoutStates[path] = true;
			foldoutStates[path] = EditorGUILayout.Foldout(foldoutStates[path], "", true);

			Texture2D assetIcon = AssetDatabase.GetCachedIcon(path) as Texture2D;
			if (assetIcon != null)
				GUILayout.Label(assetIcon, GUILayout.Width(16), GUILayout.Height(16));

			if (GUILayout.Button(path, EditorStyles.linkLabel))
			{
				Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
				Selection.activeObject = obj;
				EditorGUIUtility.PingObject(obj);
			}

			GUILayout.Label($"({errors.Count})", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal();

			if (foldoutStates[path])
			{
				EditorGUI.indentLevel++;
				foreach (var error in errors)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(20);
					GUI.color = new Color(1f, 0.85f, 0.85f);
					GUILayout.Label(
						$"⚠ Компонент: ",
						EditorStyles.miniBoldLabel,
						GUILayout.Width(90)
					);
					GUI.color = Color.white;
					GUILayout.Label(
						error.ComponentName,
						EditorStyles.miniLabel,
						GUILayout.Width(200)
					);
					GUI.color = new Color(1f, 0.85f, 0.85f);
					GUILayout.Label("Свойство: ", EditorStyles.miniBoldLabel, GUILayout.Width(65));
					GUI.color = Color.white;
					GUILayout.Label(error.PropertyName, EditorStyles.miniLabel);
					EditorGUILayout.EndHorizontal();
				}
				EditorGUI.indentLevel--;
				GUILayout.Space(3);
			}
			EditorGUILayout.EndVertical();
			GUILayout.Space(2);
		}
	}

	private void ExecuteMissingLinksSearch()
	{
		missingReferencesResult.Clear();
		foldoutStates.Clear();
		string[] targetsToScan = null;
		string filter = "t:Prefab t:Scene t:ScriptableObject t:Material t:AnimatorController t:Font t:Shader t:Texture t:Sprite";
		switch (currentScope)
		{
			case SearchScope.WholeProject: // Ищем GUID всех поддерживаемых ассетов по всему проекту
				string[] guids = AssetDatabase.FindAssets(filter);
				targetsToScan = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
				break;
			case SearchScope.SelectedAsset: // Сканируем только один выбранный файл
				string assetPath = AssetDatabase.GetAssetPath(selectedMissingTarget);
				if (!string.IsNullOrEmpty(assetPath))
				{
					targetsToScan = new string[] { assetPath };
				}
				break;
			case SearchScope.SelectedFolder: // Ищем ассеты внутри указанной папки (и ее подпапок)
				string folderPath = AssetDatabase.GetAssetPath(selectedMissingTarget);
				if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
				{
					string[] folderGuids = AssetDatabase.FindAssets(
						filter,
						new string[] { folderPath }
					);
					targetsToScan = folderGuids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
				}
				break;
		}
		if (targetsToScan == null || targetsToScan.Length == 0)
		{
			EditorUtility.DisplayDialog(
				"Поиск битых ссылок",
				"Нет подходящих файлов для сканирования.",
				"ОК"
			);
			return;
		} 
		// Запуск логики из вынесенного класса
		missingReferencesResult = MissingReferencesFinder.FindMissingReferences(
			targetsToScan,
			(fileName, progress) =>
			{
				EditorUtility.DisplayProgressBar(
					"Поиск битых ссылок",
					$"Анализ: {fileName}",
					progress
				);
			}
		);
		EditorUtility.ClearProgressBar(); // Раскрываем списки для найденных ошибок по умолчанию
		foreach (var key in missingReferencesResult.Keys)
		{
			foldoutStates[key] = true;
		}
	}

	// Вспомогательные методы анализа для первой и второй вкладок
	private void AnalyzeAllCompare()
	{
		dependenciesA.Clear();
		referencesA.Clear();
		dependenciesB.Clear();
		referencesB.Clear();
		if (selectedAssetA == null && selectedAssetB == null)
			return;
		string pathA = selectedAssetA != null ? AssetDatabase.GetAssetPath(selectedAssetA) : "";
		string pathB = selectedAssetB != null ? AssetDatabase.GetAssetPath(selectedAssetB) : "";
		if (!string.IsNullOrEmpty(pathA))
			dependenciesA = AssetDatabase
				.GetDependencies(pathA, false)
				.Where(p => p != pathA)
				.ToList();
		if (!string.IsNullOrEmpty(pathB))
			dependenciesB = AssetDatabase
				.GetDependencies(pathB, false)
				.Where(p => p != pathB)
				.ToList();
		string[] allAssetGuids = AssetDatabase.FindAssets(
			"t:Prefab t:Scene t:ScriptableObject t:Material"
		);
		int total = allAssetGuids.Length;
		for (int i = 0; i < total; i++)
		{
			string currentAssetPath = AssetDatabase.GUIDToAssetPath(allAssetGuids[i]);
			if (i % 100 == 0)
				EditorUtility.DisplayProgressBar(
					"Сравнение ассетов",
					$"Анализ проекта: {Path.GetFileName(currentAssetPath)}",
					(float)i / total
				);
			string[] currentAssetDeps = AssetDatabase.GetDependencies(currentAssetPath, false);
			foreach (string dep in currentAssetDeps)
			{
				if (dep == pathA && currentAssetPath != pathA)
					referencesA.Add(currentAssetPath);
				if (dep == pathB && currentAssetPath != pathB)
					referencesB.Add(currentAssetPath);
			}
		}
		EditorUtility.ClearProgressBar();
	}

	private void AnalyzeAsset(Object target, ref List<string> deps, ref List<string> refs)
	{
		deps.Clear();refs.Clear();if (target == null)
			return;
		string assetPath = AssetDatabase.GetAssetPath(target);
		if (string.IsNullOrEmpty(assetPath))
			return;
		string[] rawDependencies = AssetDatabase.GetDependencies(assetPath, false);
		foreach (string path in rawDependencies)
		{
			if (path != assetPath)
				deps.Add(path);
		}
		string[] allAssetGuids = AssetDatabase.FindAssets(
			"t:Prefab t:Scene t:ScriptableObject t:Material"
		);
		int total = allAssetGuids.Length;
		for (int i = 0; i < total; i++)
		{
			string currentAssetPath = AssetDatabase.GUIDToAssetPath(allAssetGuids[i]);
			if (i % 100 == 0)
				EditorUtility.DisplayProgressBar(
					"Поиск ссылок",
					$"Проверка: {Path.GetFileName(currentAssetPath)}",
					(float)i / total
				);
			string[] currentAssetDeps = AssetDatabase.GetDependencies(currentAssetPath, false);
			foreach (string dep in currentAssetDeps)
			{
				if (dep == assetPath && currentAssetPath != assetPath)
				{
					refs.Add(currentAssetPath);
					break;
				}
			}
		}
		EditorUtility.ClearProgressBar();
	}

	private void RenderAssetList(List<string> paths)
	{
		if (paths.Count == 0)
		{
			GUILayout.Label("   (список пуст)", EditorStyles.miniLabel);
			return;
		}
		foreach (string path in paths)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10);
			Texture2D assetIcon = AssetDatabase.GetCachedIcon(path) as Texture2D;
			if (assetIcon != null)
				GUILayout.Label(assetIcon, GUILayout.Width(16), GUILayout.Height(16));
			if (GUILayout.Button(path, EditorStyles.linkLabel))
			{
				Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
				Selection.activeObject = obj;
				EditorGUIUtility.PingObject(obj);
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif