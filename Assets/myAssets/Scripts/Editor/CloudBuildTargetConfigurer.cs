using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CloudBuildTargetConfigurer
{
	// Этот метод мы укажем в настройках Android-конфигурации
	public static void ConfigureAndroidIL2CPP()
	{
		// 1. Включаем IL2CPP
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

		// 2. Включаем архитектуры ARMv7 и ARM64 (для Google Play)
		PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23; //AndroidApiLevel23 (6)
		PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34; //AndroidApiLevel33 (13)
		PlayerSettings.Android.minifyDebug = true;
		PlayerSettings.Android.minifyRelease = true;
#pragma warning disable 0618
		PlayerSettings.Android.minifyWithR8 = true;
#pragma warning restore 0618

		Debug.Log("[CloudBuildScript] Настройка Android успешно завершена.");
	}

	// Этот метод можно вызвать для Windows, если хотите принудительно вернуть Mono
	public static void ConfigureWindowsMono()
	{
		Debug.Log("[CloudBuildScript] Настройка Windows: переключение на Mono...");

		// Включаем Mono для Windows
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

		Debug.Log("[CloudBuildScript] Настройка Windows успешно завершена.");
	}

	/// <summary>
	/// Локальный запуск сборки Android в batch-режиме из консоли.
	/// & "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -quit -batchmode -logFile batch_build_android_log.txt -projectPath . -executeMethod CloudBuildTargetConfigurer.LocalAndroidBatchBuild
	/// </summary>
	public static void LocalAndroidBatchBuild()
	{
		Debug.Log("[BatchBuild] Начало процесса локальной сборки Android...");

		// 1. Автоматически собираем все сцены, которые включены в Build Settings
		string[] scenes = GetScenes();

		if (scenes.Length == 0)
		{
			Debug.LogError("[BatchBuild] Ошибка: В Build Settings не добавлено ни одной активной сцены!");
			EditorApplication.Exit(1);
			return;
		}

		// 2. Определяем путь для сохранения итогового APK/AAB
		//string buildFolder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Builds");
		string buildFolder = Application.dataPath + @"\..\..\Builds\";
		if (!Directory.Exists(buildFolder))
		{
			Directory.CreateDirectory(buildFolder);
		}

		// Определяем расширение в зависимости от настроек (APK или App Bundle)
		string extension = EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
		string buildPath = Path.Combine(buildFolder, $"{Application.productName}_{Application.version}{extension}");

		// 3. Формируем опции сборки (переключаем целевую платформу на Android)
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = buildPath,
			target = BuildTarget.Android,
			targetGroup = BuildTargetGroup.Android,
			options = BuildOptions.None
		};

		// 4. ПРИНУДИТЕЛЬНО вызываем ваш метод предобработки,
		// чтобы он сгенерировал манифесты, поправил EOS namespace и т.д.
		try
		{
			ConfigureAndroidIL2CPP();
			Debug.Log("[BatchBuild] Шаг предобработки ConfigureAndroidIL2CPP успешно выполнен.");
		}
		catch (Exception ex)
		{
			Debug.LogError($"[BatchBuild] КРИТИЧЕСКАЯ ОШИБКА в ConfigureAndroidIL2CPP: {ex.Message}");
			EditorApplication.Exit(2);
			return;
		}

		// 5. Запускаем компиляцию проекта
		Debug.Log($"[BatchBuild] Запуск BuildPipeline. Файл будет сохранен в: {buildPath}");
		var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
		var summary = report.summary;

		// 6. Проверяем статус и завершаем процесс с правильным статус-кодом
		if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
		{
			Debug.Log($"[BatchBuild] СБОРКА УСПЕШНО ЗАВЕРШЕНА! Время сборки: {summary.totalTime.TotalSeconds:F2} сек.");
			EditorApplication.Exit(0);
		}
		else if (summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
		{
			Debug.LogError($"[BatchBuild] СБОРКА ЗАВЕРШИЛАСЬ НЕУДАЧЕЙ! Количество ошибок: {summary.totalErrors}");
			EditorApplication.Exit(3);
		}
	}

	/// <summary>
	/// Локальный запуск сборки Windows (64-bit) в batch-режиме из консоли.
	/// & "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" -quit -batchmode -logFile windows_build_win_log.txt -projectPath . -executeMethod CloudBuildTargetConfigurer.LocalWindowsBatchBuild
	/// </summary>
	public static void LocalWindowsBatchBuild()
	{
		Debug.Log("[BatchBuild] Начало процесса локальной сборки Windows...");

		string[] scenes = GetScenes();

		if (scenes.Length == 0)
		{
			Debug.LogError("[BatchBuild] Ошибка: В Build Settings нет активных сцен!");
			EditorApplication.Exit(1);
			return;
		}

		string folderName = Application.productName + "_" + Application.version;
		string buildFolder = Application.dataPath + @"\..\..\Builds\" + folderName;
		//string buildFolder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Builds", folderName);
		if (!Directory.Exists(buildFolder))
		{
			Directory.CreateDirectory(buildFolder);
		}

		string exeName = PlayerSettings.productName;
		foreach (char c in Path.GetInvalidFileNameChars())
		{
			exeName = exeName.Replace(c, '_');
		}
		string buildPath = Path.Combine(buildFolder, $"{exeName}.exe");

		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = buildPath,
			target = BuildTarget.StandaloneWindows64,
			targetGroup = BuildTargetGroup.Standalone,
			options = BuildOptions.None
		};

		Debug.Log($"[BatchBuild] Запуск компиляции. Файл: {buildPath}");
		var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
		var summary = report.summary;

		if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
		{
			Debug.Log($"[BatchBuild] СБОРКА WINDOWS УСПЕШНО ЗАВЕРШЕНА! Время: {summary.totalTime.TotalSeconds:F2} сек.");
			EditorApplication.Exit(0);
		}
		else
		{
			Debug.LogError($"[BatchBuild] СБОРКА WINDOWS ЗАВЕРШИЛАСЬ НЕУДАЧЕЙ! Ошибок: {summary.totalErrors}");
			EditorApplication.Exit(3);
		}
	}

	private static string[] GetScenes()
	{
		return EditorBuildSettings.scenes
			.Where(s => s.enabled)
			.Select(s => s.path)
			.ToArray();
	}
}
