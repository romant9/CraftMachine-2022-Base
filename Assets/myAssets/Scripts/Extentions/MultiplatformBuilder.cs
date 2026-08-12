using UnityEditor;
using UnityEngine;
using System.IO;

public class MultiplatformBuilder
{
	[MenuItem("Build/Build Windows and Android")]
	public static void BuildWindowsAndAndroid()
	{
		// 1. Получаем список сцен, включенных в Build Settings
		string[] scenes = GetScenesForBuild();
		if (scenes.Length == 0)
		{
			Debug.LogError("Нет сцен для сборки! Добавьте сцены в Build Settings.");
			return;
		}

		// Путь для сохранения сборок в корне проекта
		string buildRoot = Path.Combine(Application.dataPath, "../Builds");

		// 2. Сборка для Windows
		BuildWindows(scenes, Path.Combine(buildRoot, "Windows"));

		// 3. Сборка для Android
		BuildAndroid(scenes, Path.Combine(buildRoot, "Android"));

		Debug.Log("--- Сборка на обе платформы успешно завершена! ---");
	}

	private static void BuildWindows(string[] scenes, string path)
	{
		Debug.Log("Начало сборки для Windows...");
		Directory.CreateDirectory(path);

		BuildPlayerOptions options = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = Path.Combine(path, Application.productName + ".exe"),
			target = BuildTarget.StandaloneWindows64,
			options = BuildOptions.None
		};

		var report = BuildPipeline.BuildPlayer(options);
		Debug.Log($"Результат Windows: {report.summary.result}");
	}

	private static void BuildAndroid(string[] scenes, string path)
	{
		Debug.Log("Начало сборки для Android...");
		Directory.CreateDirectory(path);

		BuildPlayerOptions options = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = Path.Combine(path, Application.productName + ".apk"),
			target = BuildTarget.Android,
			options = BuildOptions.None
		};

		var report = BuildPipeline.BuildPlayer(options);
		Debug.Log($"Результат Android: {report.summary.result}");
	}

	private static string[] GetScenesForBuild()
	{
		var scenes = EditorBuildSettings.scenes;
		System.Collections.Generic.List<string> activeScenes = new System.Collections.Generic.List<string>();

		foreach (var scene in scenes)
		{
			if (scene.enabled)
			{
				activeScenes.Add(scene.path);
			}
		}
		return activeScenes.ToArray();
	}
}
