#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateAssetBundles : Editor
{
	public static string AssetBundlesRootPath = "AssetBundles";

	[MenuItem("Assets/Build AssetBundles")]
	public static void BuildAssetBundles()
	{
		BuildPipeline.BuildAssetBundles(AssetBundlesRootPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
	}

	[MenuItem("Assets/Build AssetBundles Atlases")]
	public static void CreateAssetBundlesWindow()
	{
		CreateAssetBundlesMenu menu = (CreateAssetBundlesMenu)EditorWindow.GetWindow(typeof(CreateAssetBundlesMenu));
		menu.OpenWindow();
	}

	public class CreateAssetBundlesMenu: EditorWindow
	{
		string Name = "scene_indoors";

		public void OpenWindow()
		{
			position = new Rect(Screen.width / 2, Screen.height / 2, 300, 200);
			titleContent = new GUIContent("Build Asset Bundles");
			Show();
		}

		private void OnGUI()
		{
			GUILayout.Label("Название бандла:");
			Name = GUILayout.TextField(Name, 200);

			if (GUILayout.Button(new GUIContent("СОЗДАТЬ")))
			{
				if (string.IsNullOrEmpty(Name)) return;
				BuildAssetBundlesByName(Name);
			}
		}


		public static void BuildAssetBundlesByName(string name)//string[] assetBundleNames)
		{
			string[] assetBundleNames = new[] { name };
			//Argument validation
			if (assetBundleNames == null || assetBundleNames.Length == 0)
			{
				return;
			}

			//Remove duplicates from the input set of asset bundle names to build.
			//assetBundleNames = assetBundleNames.Distinct().ToArray();

			List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

			foreach (string assetBundle in assetBundleNames)
			{
				var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundle);

				AssetBundleBuild build = new AssetBundleBuild();
				build.assetBundleName = assetBundle;
				build.assetNames = assetPaths;

				builds.Add(build);
				Debug.Log("assetBundle to build:" + build.assetBundleName);
			}

			BuildPipeline.BuildAssetBundles(AssetBundlesRootPath, builds.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
		}
	}


}
#endif