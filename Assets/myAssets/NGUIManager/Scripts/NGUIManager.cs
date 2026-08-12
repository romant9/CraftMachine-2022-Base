using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[Serializable]
[CreateAssetMenu(fileName = "NGUIManager", menuName = "Custom/Create NGUIManager")]
public class NGUIManager : ScriptableObject
{
	private static NGUIManager instance;
	public static NGUIManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load<NGUIManager>("Config/NGUIManager");
				SetShaders();
			}
			return instance;
		}
	}

	public static string LocalizationName = "Localization";
	public static bool IsSingleLanguageLoad = true;
	public static bool IsLoadDataManager = true;
	public static bool IsSetDirty = false;
	public static float screenMult = 1;

	[SerializeField]
	private Shader transpColoredShader;
	[SerializeField]
	private Shader premultColoredShader;
	[SerializeField]
	private Shader transpMasked;

	public static bool IsEditor => Application.isEditor && !Application.isPlaying;
	public static bool IsEditorRuntime => Application.isEditor && Application.isPlaying;

	private static Shader _transpColoredShader;
	public static Shader TranspColoredShader 
	{ 
		get 
		{ 
			if (_transpColoredShader == null)
			{
				return Shader.Find("Unlit/Transparent Colored");
			}
			return _transpColoredShader; 
		}
		set { _transpColoredShader = value; }
	}

	private static Shader _premultColoredShader;
	public static Shader PremultColoredShader
	{
		get
		{
			if (_premultColoredShader == null)
			{
				return Shader.Find("Unlit/Transparent Colored");
			}
			return _premultColoredShader;
		}
		set { _premultColoredShader = value; }
	}

	private static Shader _transpMaskedShader;
	public static Shader TranspMaskedShader
	{
		get
		{
			if (_transpMaskedShader == null)
			{
				return Shader.Find("Unlit/Transparent Masked");
			}
			return _transpMaskedShader;
		}
		set { _transpMaskedShader = value; }
	}

	public static void SetShaders()
	{
		TranspColoredShader = Instance != null && Instance.transpColoredShader ? Instance.transpColoredShader : Shader.Find("Unlit/Transparent Colored");
		PremultColoredShader = Instance != null && Instance.premultColoredShader ? Instance.premultColoredShader : Shader.Find("Unlit/Premultiplied Colored");
		TranspMaskedShader = Instance != null && Instance.transpMasked ? Instance.transpMasked : Shader.Find("Unlit/Transparent Masked");
	}

	public static Shader GetWidgetShader(Material mat)
	{
		if (mat != null)
		{
			if (mat.shader.name.Contains("Traveling"))// == "TWD FX/FX Traveling Rainbow" || )
				return TranspMaskedShader; // "Drill/NGUI-Unlit/Transparent Colored Dual (SoftClip)");  //);
			return mat.shader;
		}
		return null;
	}

	public static bool IsShaderNameContains_Jump(Shader shader)
	{
		return shader.name.Contains("Traveling") || shader.name.Contains("Jumping");
	}

	public static bool IsShaderNameContains_Transp(Shader shader)
	{
		return shader.name.Contains("Transparent") && !shader.name.Contains("Masked");
	}

	public static Vector2 ScreenSize()
	{
		Vector2 vec = new Vector2(Screen.width, Screen.height) * screenMult;
		return vec;
	}

	public static void UIDrawCallHelper(bool hasManager, GameObject managerGO, GameObject dcGo)
	{
#if UNITY_EDITOR
		var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
		if (prefabStage != null && hasManager)
		{
			// If prefab stage exists and new daw call
			var stage = UnityEditor.SceneManagement.StageUtility.GetStageHandle(managerGO);
			if (stage == prefabStage.stageHandle)
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(dcGo, prefabStage.scene);
		}
#endif
	}

#if UNITY_EDITOR
	private static UnityEditor.SceneManagement.PrefabStage PrefabStage;
#endif

	public static bool IsPrefabStage(GameObject go)
	{
#if UNITY_EDITOR
		UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(go);
		PrefabStage = prefabStage;
		return prefabStage == null;
#else
		return true;
#endif
	}

	public static GameObject GetContainer()
	{
#if UNITY_EDITOR
		return UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("UIRoot (Environment)", HideFlags.DontSave);
#else
		return null;
#endif
	}

	public static void MoveGameObjectToScene(GameObject container)
	{
#if UNITY_EDITOR
		if (PrefabStage) UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(container, PrefabStage.scene);
#endif
	}

	public static int GetMHandles()
	{
#if UNITY_EDITOR
		return UnityEditor.EditorPrefs.GetInt("NGUI Handles", 1);
#else
		return -1;
#endif
	}

	public static void SetMHandles(int mHandles)
	{
#if UNITY_EDITOR
		UnityEditor.EditorPrefs.SetInt("NGUI Handles", mHandles);
#endif
	}

	public static bool IsShowHandles()
	{
#if UNITY_EDITOR
		return UnityEditor.Tools.current == UnityEditor.Tool.Rect;
#else
		return false;
#endif
	}

	public static void RegisterCreatedObjectUndo2D(BoxCollider2D box)
	{
#if UNITY_EDITOR
		UnityEditor.Undo.RegisterCreatedObjectUndo(box, "Add Collider");
#endif
	}

	public static void RegisterCreatedObjectUndo(BoxCollider box)
	{
#if UNITY_EDITOR
		UnityEditor.Undo.RegisterCreatedObjectUndo(box, "Add Collider");
#endif
	}

	public static void RegisterCreatedObjectUndo(bool undo, GameObject go)
	{
#if UNITY_EDITOR
		if (undo && !Application.isPlaying)
			UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
	}

	public static void SetSelection(GameObject go)
	{
#if UNITY_EDITOR
		UnityEditor.Selection.activeGameObject = go;
#endif
	}

	public static bool IsActiveGameobject(GameObject go)
	{
#if UNITY_EDITOR
		return UnityEditor.Selection.activeGameObject == go;
#else
		return false;
#endif
	}

	static int mSizeFrame = -1;
	static Func<Vector2> s_GetSizeOfMainGameView;
	[System.NonSerialized] static Vector2 mGameSize = Vector2.one;
	[System.NonSerialized] static bool mCheckedMainViewFunc = false;

	static public Vector2 screenSize
	{
		get
		{
			return IsEditor ? screenSizeEditor : ScreenSize();
		}
	}

	public static Vector2 screenSizeEditor
	{
		get
		{
			int frame = Time.frameCount;

			if (mSizeFrame != frame || !Application.isPlaying)
			{
				UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (NGUITools.screenSize)");

				mSizeFrame = frame;

				if (s_GetSizeOfMainGameView == null && !mCheckedMainViewFunc)
				{
					mCheckedMainViewFunc = true;
					System.Type type = System.Type.GetType("UnityEditor.GameView,UnityEditor");

					// Post-Unity 5.4
					var methodInfo = type.GetMethod("GetMainGameViewTargetSize",
						System.Reflection.BindingFlags.Public |
						System.Reflection.BindingFlags.NonPublic |
						System.Reflection.BindingFlags.Static);

					// Pre-Unity 5.4
					if (methodInfo == null)
						methodInfo = type.GetMethod("GetSizeOfMainGameView",
							System.Reflection.BindingFlags.Public |
							System.Reflection.BindingFlags.NonPublic |
							System.Reflection.BindingFlags.Static);

					// Create the delegate
					if (methodInfo != null)
					{
						s_GetSizeOfMainGameView = (Func<Vector2>)Delegate.CreateDelegate(typeof(Func<Vector2>), methodInfo);
					}
					else Debug.LogWarning("Unable to get the main game view size function");
				}

				if (s_GetSizeOfMainGameView != null)
				{
					mGameSize = s_GetSizeOfMainGameView();
				}
				else mGameSize = new Vector2(Screen.width, Screen.height);
				UnityEngine.Profiling.Profiler.EndSample();
			}
			return mGameSize;
		}
	}
}
