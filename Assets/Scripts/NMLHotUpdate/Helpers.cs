using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class Helpers
{
	public delegate void ComponentTypeCallback(Type type, Component component);

	public enum TimeFormat
	{
		SecondsOnly = 0,
		MinutesOnly = 1
	}

	public static readonly Vector3 staticVector3One = new Vector3(1f, 1f, 1f);

	public static readonly Vector3 staticVector3Zero = new Vector3(0f, 0f, 0f);

	public static readonly Vector3 staticVector4Zero = new Vector4(0f, 0f, 0f, 0f);

	public static int HideNameHash = Animator.StringToHash("hide");

	public static int ShowNameHash = Animator.StringToHash("show");

	public const float ClickAreaWidth = 23f;

	public static int numberInstantiations = 0;

	public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

	public static string ConditionOpenedPlayerPrefs = "ConditionOpenedPlayerPrefs";

	public static string ItemListOpenedPlayerPrefs = "ItemListOpenedPlayerPrefs";

	public static string SurvivalManualPlotGuidePopupPlayerPrefs = "SurvivalManualPlotGuidePopupPlayerPrefs";

	public const int sp7StarRaityLevel = 6;

	public const int sp6StarRaityLevel = 5;

	public static string SPRemoldNotFirstOpenPlayerPrefs = "SPRemoldNotFirstOpenPlayerPrefs";

	public static string SPRemoldEasyPlayerPrefs = "SPRemoldEasyPlayerPrefs";

	public static string SPRemold24ComfirmPlayerPrefs = "SPRemold24ComfirmPlayerPrefs";

	public static string SPRemold24ComfirmTimePlayerPrefs = "SPRemold24ComfirmTimePlayerPrefs";

	public static string SPRemold24UpgradeComfirmPlayerPrefs = "SPRemold24UpgradeComfirmPlayerPrefs";

	public static string SPRemold24UpgradeComfirmTimePlayerPrefs = "SPRemold24UpgradeComfirmTimePlayerPrefs";

	public static string SkillWeaponBagOpenedPlayerPrefs = "SkillWeaponBagOpenedPlayerPrefs";

	public static string SkillBagOpenedPlayerPrefs = "SkillBagOpenedPlayerPrefs";

	public static bool IsInEditor
	{
		get
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				return Application.platform == RuntimePlatform.WindowsEditor;
			}
			return true;
		}
	}

	public static float InterpolationTime => 5f;

	public static float GetPixelRatio()
	{
		return (float)Screen.height / 640f;
	}

	public static OSVersion GetIOSOSVersion()
	{
		return "0.0";
	}

	public static Vector2 CalculateNguiScreenSize(GameObject obj)
	{
		float num = (float)NGUITools.FindInParents<UIRoot>(obj).activeHeight / (float)Screen.height;
		return new Vector2(Mathf.Ceil((float)Screen.width * num), Mathf.Ceil((float)Screen.height * num));
	}

	public static Transform FindChildWithTag(Transform currentTr, string tag)
	{
		if (currentTr.tag == tag)
		{
			return currentTr;
		}
		foreach (Transform item in currentTr)
		{
			Transform transform = FindChildWithTag(item, tag);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static GameObject GetParentWithComponent<T>(GameObject obj) where T : Component
	{
		if (obj.GetComponent(typeof(T)) != null)
		{
			return obj;
		}
		if (obj.transform.parent != null)
		{
			return GetParentWithComponent<T>(obj.transform.parent.gameObject);
		}
		return null;
	}

	public static T AddComponent<T>(GameObject obj) where T : Component
	{
		if (obj != null)
		{
			if (obj.GetComponent(typeof(T)) != null)
			{
				return obj.GetComponent<T>();
			}
			return obj.AddComponent<T>();
		}
		return null;
	}

	public static List<GameObject> GetAllChildren(GameObject obj)
	{
		List<GameObject> list = new List<GameObject>();
		GetAllChildrenInternal(obj, list, add: false);
		return list;
	}

	private static void GetAllChildrenInternal(GameObject obj, List<GameObject> outList, bool add)
	{
		if (add)
		{
			outList.Add(obj);
		}
		foreach (Transform item in obj.transform)
		{
			GetAllChildrenInternal(item.gameObject, outList, add: true);
		}
	}

	public static void FormatTimeSpecialTimer(long milliSeconds, out string seconds, out string minutes, out string hours, out string days)
	{
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 86400;
		num -= num2 * 24 * 60 * 60;
		int num3 = num / 3600;
		num -= num3 * 60 * 60;
		int num4 = num / 60;
		num -= num4 * 60;
		seconds = DoubleDigitStringFormat(num);
		minutes = DoubleDigitStringFormat(num4);
		hours = DoubleDigitStringFormat(num3);
		days = DoubleDigitStringFormat(num2);
	}

	private static string DoubleDigitStringFormat(int timeValue, string digitPrefix = "0", int threshold = 10)
	{
		timeValue = Mathf.Max(timeValue, 0);
		return ((timeValue < threshold) ? digitPrefix : "") + timeValue;
	}

	public static string FormatTime(long milliSeconds, TimeFormat format)
	{
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 86400;
		num -= num2 * 24 * 60 * 60;
		int num3 = num / 3600;
		num -= num3 * 60 * 60;
		int num4 = num / 60;
		num -= num4 * 60;
		return format switch
		{
			TimeFormat.SecondsOnly => ((num < 10) ? "0" : "") + num,
			TimeFormat.MinutesOnly => ((num4 < 10) ? "0" : "") + num4,
			_ => "00",
		};
	}

	public static string FormatTime(long milliSeconds)
	{
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 86400;
		num -= num2 * 24 * 60 * 60;
		int num3 = num / 3600;
		num -= num3 * 60 * 60;
		int num4 = num / 60;
		num -= num4 * 60;
		string text = "";
		if (num2 > 0)
		{
			text = num2 + LocalizationManager.GetText("Generic.Time.DaySmall") + " ";
		}
		if (num3 > 0)
		{
			text = text + num3 + LocalizationManager.GetText("Generic.Time.HourSmall") + " ";
		}
		if (num4 > 0 && num2 == 0)
		{
			text = text + num4 + LocalizationManager.GetText("Generic.Time.MinuteSmall") + " ";
		}
		if (num > 0 && num2 == 0 && num3 == 0)
		{
			text = text + num + LocalizationManager.GetText("Generic.Time.SecondSmall");
		}
		text.Trim();
		return text;
	}

	public static string FormatTimeWithDoubleDigits(long milliSeconds, bool doubleHours = false, bool doubleMinues = false, bool doubleSeconds = true)
	{
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 86400;
		num -= num2 * 24 * 60 * 60;
		int num3 = num / 3600;
		num -= num3 * 60 * 60;
		int num4 = num / 60;
		num -= num4 * 60;
		string text = "";
		if (num2 > 0)
		{
			text = string.Format("{0}{1} ", num2, LocalizationManager.GetText("Generic.Time.DaySmall"));
		}
		if (num3 > 0)
		{
			text += string.Format("{0}{1} ", doubleHours ? DoubleDigitStringFormat(num3) : num3.ToString(), LocalizationManager.GetText("Generic.Time.HourSmall"));
		}
		if (num2 == 0)
		{
			text += string.Format("{0}{1} ", doubleMinues ? DoubleDigitStringFormat(num4) : num4.ToString(), LocalizationManager.GetText("Generic.Time.MinuteSmall"));
		}
		if (num2 == 0 && num3 == 0)
		{
			text += string.Format("{0}{1}", doubleSeconds ? DoubleDigitStringFormat(num) : num.ToString(), LocalizationManager.GetText("Generic.Time.SecondSmall"));
		}
		text.Trim();
		return text;
	}

	public static string FormatTimeWithoutSeconds(long milliSeconds)
	{
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 86400;
		int num3 = num - num2 * 24 * 60 * 60;
		int num4 = num3 / 3600;
		int num5 = (num3 - num4 * 60 * 60) / 60 + 1;
		string text = "";
		if (num2 > 0)
		{
			text = num2 + LocalizationManager.GetText("Generic.Time.DaySmall") + " ";
		}
		if (num4 > 0)
		{
			text = text + num4 + LocalizationManager.GetText("Generic.Time.HourSmall") + " ";
		}
		if (num5 > 0 && num2 == 0)
		{
			text = text + num5 + LocalizationManager.GetText("Generic.Time.MinuteSmall") + " ";
		}
		text.Trim();
		return text;
	}

	public static string FormatTimeDayOrMin(long milliSeconds)
	{
		int num = (int)(milliSeconds / 1000);
		int num2 = num / 86400;
		int num3 = num - num2 * 24 * 60 * 60;
		int num4 = num3 / 3600;
		int num5 = (num3 - num4 * 60 * 60) / 60 + 1;
		string text = "";
		if (num2 > 0)
		{
			text = num2 + LocalizationManager.GetText("Generic.Time.DaySmall") + " ";
		}
		else
		{
			if (num4 > 0)
			{
				text = text + num4 + LocalizationManager.GetText("Generic.Time.HourSmall") + " ";
			}
			if (num5 > 0)
			{
				text = text + num5 + LocalizationManager.GetText("Generic.Time.MinuteSmall") + " ";
			}
		}
		text.Trim();
		return text;
	}

	public static string FormatTimeNoZero(long milliSeconds)
	{
		return FormatTime(milliSeconds + 999);
	}

	public static string FormatTimeAsHms(long milliSeconds)
	{
		long num = ((milliSeconds > 0) ? ((milliSeconds + 999) / 1000) : 0);
		long num2 = num / 3600;
		long num3 = num % 3600 / 60;
		long num4 = num % 60;
		return $"{num2:00}:{num3:00}:{num4:00}";
	}

	public static int ConvertToSecondsNoZero(long milliseconds)
	{
		return (int)((milliseconds + 999) / 1000);
	}

	public static string FormatTimeAgo(long referenceTime, long timeInPast)
	{
		string text = "";
		text = FormatTime(referenceTime - timeInPast);
		string text2 = "";
		if (text == "")
		{
			return LocalizationManager.GetText("Generic.Time.PostedNow");
		}
		return LocalizationManager.GetText("Generic.Time.PostedAgo{TimeAgo}", text);
	}

	public static string FormatNumber(long value, int formatingMinThreshold = 0, int decimalDigits = 2)
	{
		string text = decimalDigits switch
		{
			0 => "#",
			1 => "#.#",
			3 => "#.###",
			_ => "#.##",
		};
		string result = value.ToString();
		if (value >= formatingMinThreshold)
		{
			if (SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage == "ja")
			{
				if (value >= 100000000)
				{
					result = ((float)value / 100000000f).ToString(text, CultureInfo.InvariantCulture) + LocalizationManager.GetText("Generic.NumberFormat100M");
				}
				else if (value >= 10000)
				{
					result = ((float)value / 10000f).ToString(text, CultureInfo.InvariantCulture) + LocalizationManager.GetText("Generic.NumberFormat10K");
				}
			}
			else if (value >= 1000000)
			{
				result = ((float)value / 1000000f).ToString(text, CultureInfo.InvariantCulture) + LocalizationManager.GetText("Generic.NumberFormatM");
			}
			else if (value >= 10000)
			{
				result = ((float)value / 1000f).ToString(text, CultureInfo.InvariantCulture) + LocalizationManager.GetText("Generic.NumberFormatK");
			}
		}
		return result;
	}

	public static GameObject InstantiateToParent(GameObject prefab, GameObject parent)
	{
		if (!parent)
		{
			return null;
		}
		Quaternion localRotation = prefab.transform.localRotation;
		Vector3 localScale = prefab.transform.localScale;
		GameObject gameObject = null;
		if (prefab.GetComponent<CacheableObject>() == null || SingularityMonoBehaviour<ObjectPoolManager>.Instance == null)
		{
			try
			{
				gameObject = UnityEngine.Object.Instantiate(prefab, parent.transform.position, localRotation);
			}
			catch(Exception ex)
			{
				DebugTWD.LogError(ex.Message + '\n' + ex.StackTrace);
			}
			if (gameObject != null)
			{
				gameObject.transform.SetParent(parent.transform);
			}
		}
		else
		{
			gameObject = SingularityMonoBehaviour<ObjectPoolManager>.Instance.FetchObject(prefab, parent.transform);
		}
		if ((bool)gameObject)
		{
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = localRotation;
			gameObject.transform.localScale = localScale;
		}
		return gameObject;
	}

	public static GameObject InstantiateToParentAndLayer(GameObject prefab, GameObject parent)
	{
		GameObject gameObject = InstantiateToParent(prefab, parent);
		NGUITools.SetLayer(gameObject, parent.layer);
		return gameObject;
	}

	public static T InstantiateToList<T>(GameObject prefab, GameObject parent, List<T> list, bool addComponent = true) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		T val = InstantiateWithComponent<T>(prefab, parent, addComponent);
		if (val != null)
		{
			list.Add(val);
			return val;
		}
		return null;
	}

	public static T InstantiateWithComponent<T>(GameObject prefab, GameObject parent, bool addComponent = true) where T : Component
	{
		if (prefab != null)
		{
			if (parent != null)
			{
				GameObject gameObject = InstantiateToParentAndLayer(prefab, parent);
				if (gameObject != null)
				{
					if (gameObject.GetComponent<T>() != null)
					{
						return gameObject.GetComponent<T>();
					}
					if (addComponent)
					{
						return gameObject.AddComponent<T>();
					}
					Debug.LogError($"InstantiatePrefabWithComponent: Could not find Component: {typeof(T).ToString()} in GameObject: {gameObject}");
				}
				else
				{
					Debug.LogError($"InstantiatePrefabWithComponent: Could not InstantiateToParentAndLayer: {prefab} to parent: {parent}");
				}
			}
			else
			{
				Debug.LogError($"InstantiatePrefabWithComponent: Could not instantiate Prefab: {prefab.name} to NULL parent");
			}
		}
		else
		{
			Debug.LogError("InstantiatePrefabWithComponent: Could not instantiate NULL Prefab!");
		}
		return null;
	}

	public static T InstantiateFromResourcesToParent<T>(string resourceSrc, GameObject parent) where T : Component
	{
		if (parent == null)
		{
			Debug.LogError("Could not instantiate to null parent, src: " + resourceSrc);
			return null;
		}
		GameObject gameObject = UnityUtils.LoadAsset(resourceSrc) as GameObject;
		if (gameObject == null)
		{
			Debug.LogError("Could not load and instantiate prefab from src: " + resourceSrc);
			return null;
		}
		return InstantiateWithComponent<T>(gameObject, parent);
	}

	public static GameObject InstantiateFromResourcesToParent(string resourceSrc, GameObject parent)
	{
		if (parent == null)
		{
			Debug.LogError("Could not instantiate to null parent, src: " + resourceSrc);
			return null;
		}
		GameObject gameObject = UnityUtils.LoadAsset(resourceSrc) as GameObject;
		if (gameObject == null)
		{
			Debug.LogError("Could not load and instantiate prefab from src: " + resourceSrc);
			return null;
		}
		return InstantiateToParentAndLayer(gameObject, parent);
	}

	public static T InstantiateFromAssetBundleToParent<T>(string assetName, string bundleName, GameObject parent) where T : Component
	{
		if (parent == null)
		{
			Debug.LogError("Could not instantiate to null parent, src: " + assetName);
			return null;
		}
		GameObject gameObject = UnityUtils.LoadFromAssetBundle(assetName, bundleName) as GameObject;
		if (gameObject == null)
		{
			Debug.LogError("Could not load and instantiate prefab from src: " + assetName);
			return null;
		}
		return InstantiateWithComponent<T>(gameObject, parent);
	}

	public static GameObject InstantiateFromAssetBundleToParent(string assetName, string bundleName, GameObject parent)
	{
		if (parent == null)
		{
			Debug.LogError("Could not instantiate to null parent, src: " + assetName);
			return null;
		}
		GameObject gameObject = UnityUtils.LoadFromAssetBundle(assetName, bundleName) as GameObject;
		if (gameObject == null)
		{
			Debug.LogError("Could not load and instantiate prefab from src: " + assetName);
			return null;
		}
		return InstantiateToParentAndLayer(gameObject, parent);
	}

	public static void ChangeParent(GameObject gameobject, GameObject parent)
	{
		Quaternion localRotation = gameobject.transform.localRotation;
		Vector3 localScale = gameobject.transform.localScale;
		gameobject.transform.parent = parent.transform;
		gameobject.transform.localPosition = Vector3.zero;
		gameobject.transform.localRotation = localRotation;
		gameobject.transform.localScale = localScale;
	}

	public static void RandomShuffle<T>(List<T> aList)
	{
		int num = aList.Count;
		while (num > 1)
		{
			num--;
			int index = UnityEngine.Random.Range(0, num);
			T value = aList[index];
			aList[index] = aList[num];
			aList[num] = value;
		}
	}

	public static void DestroyOrCache(Component component)
	{
		if (component != null && component.gameObject != null)
		{
			DestroyOrCache(component.gameObject);
		}
	}

	public static void DestroyOrCache(GameObject gameObject)
	{
		if (gameObject != null)
		{
			if (gameObject.GetComponent<CacheableObject>() != null)
			{
				gameObject.GetComponent<CacheableObject>().Destroy();
			}
			else
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	public static void DestroyAllChildren(GameObject parent)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in parent.transform)
		{
			list.Add(item.gameObject);
		}
		foreach (GameObject item2 in list)
		{
			UnityEngine.Object.Destroy(item2);
		}
	}

	public static TWDModelResult ExecuteCommand(ModelCommand command)
	{
		if (DebugTWD.IsDebugBuild)
		{
			DebugTWD.LogWarning("Command is: " + command.GetType().Name, DebugType.Command);
		}
		if (GameManager.Instance == null || GameManager.Instance.modelManager == null)
		{
			Debug.LogError("Manager is NULL");
			return TWDModelResult.Error;
		}
		int debugModelsCount = GameManager.Instance.modelManager.GetDebugModelsCount();
		int callCount = GameManager.Instance.playerModel.PlayerRandom.CallCount;
		ModelCommandRespond modelCommandRespond = GameManager.Instance.modelManager.ExecuteCommand(command) as ModelCommandRespond;
		if (DebugTWD.IsDebugBuild && GameManager.Instance.modelManager.GameEconomyData.ConfigData.DebugPostLevel >= 2)
		{
			DebugTWD.LogWarning("StorePlayerJsonState: " + command.SequenceId, DebugType.System);
			GameManager.Instance.StorePlayerJsonState(command.SequenceId);
		}
		if (modelCommandRespond == null)
		{
			Debug.LogError("Null response");
			return TWDModelResult.Error;
		}
		if (modelCommandRespond.Code != 37 && modelCommandRespond.Code != 42 && modelCommandRespond.Code != -2 && modelCommandRespond.Code != 0)
		{
			int debugModelsCount2 = GameManager.Instance.modelManager.GetDebugModelsCount();
			int callCount2 = GameManager.Instance.playerModel.PlayerRandom.CallCount;
			string text = command.GetType()?.ToString() + " Failed: " + (TWDModelResult)modelCommandRespond.Code/*cast due to .constrained prefix*/;
			if (debugModelsCount != debugModelsCount2)
			{
				text = text + ", MODEL DESYNC: " + debugModelsCount + "=>" + debugModelsCount2;
			}
			if (callCount != callCount2)
			{
				text = text + ", RANDOM DESYNC: " + callCount + "=> " + callCount2;
			}
			GameManager.Instance?.Show_Command_Error(modelCommandRespond.Code);

			text = text + ", Command:" + GameManager.Instance.jsonSerializer.Serialize(command);

			if (OfflineManager.IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)
			{
				DebugTWD.LogError(text, DebugType.CommandError);
				if (CallCountBase.Instance)
				{
					CallCountBase.Instance.Show_Command_Error(modelCommandRespond);
				}
				modelCommandRespond.Code = 0;
			}
			else
			{
				ReportClientErrorCommand reportClientErrorCommand = new ReportClientErrorCommand();
				reportClientErrorCommand.Level = ReportClientErrorCommand.LogLevel.Error;
				reportClientErrorCommand.Message = text;
				GameManager.Instance.modelManager.ExecuteCommand(reportClientErrorCommand);
			}
		}
		if (modelCommandRespond.Code != 0 && modelCommandRespond.Code != 42 && modelCommandRespond.Code != -2 && GameManager.Instance.modelManager.CommandLog != null)
		{
			if (OfflineManager.IsLoadDataManager || OfflineManager.IsIgnoreResponseNotOK)
			{
				DebugTWD.LogError(command.GetType(), DebugType.CommandError);
				if (CallCountBase.Instance)
				{
					CallCountBase.Instance.Show_Command_Error(modelCommandRespond);
				}
				modelCommandRespond.Code = 0;
			}
			else
			{
				GameManager.Instance.modelManager.CommandLog.EndCommandExecution(success: false);
			}
		}

		return (TWDModelResult)modelCommandRespond.Code;
	}

	public static void ExecuteCommandDelayed(ModelCommand command, Action<bool> doneCallback = null)
	{
		if (GameManager.Instance == null || GameManager.Instance.modelManager == null)
		{
			Debug.LogError("Manager is NULL");
			return;
		}
		if (GameManager.Instance.modelManager.IsExecutingCommand)
		{
			GameManager.Instance.StartCoroutine(ExecuteCommandDelayedCoroutine(command, doneCallback));
			return;
		}
		TWDModelResult tWDModelResult = ExecuteCommand(command);
		if (doneCallback != null)
		{
			bool obj = tWDModelResult == TWDModelResult.OK;
			doneCallback(obj);
		}
	}

	private static IEnumerator ExecuteCommandDelayedCoroutine(ModelCommand command, Action<bool> doneCallback)
	{
		yield return null;
		TWDModelResult tWDModelResult = ExecuteCommand(command);
		if (doneCallback != null)
		{
			bool obj = tWDModelResult == TWDModelResult.OK;
			doneCallback(obj);
		}
	}

	public static Vector3 ToVector3(Vector2 a)
	{
		return new Vector3(a.x, 0f, a.y);
	}

	public static Vector3 ToVector3(Vector2 a, float y)
	{
		return new Vector3(a.x, y, a.y);
	}

	public static bool GameObjectSetActive(GameObject obj, bool value)
	{
		if (obj != null)
		{
			if (obj.activeSelf != value)
			{
				obj.SetActive(value);
			}
			return obj.activeSelf;
		}
		return false;
	}

	public static bool GameObjectSetActive(MonoBehaviour obj, bool value)
	{
		if (obj != null && obj.gameObject != null)
		{
			return GameObjectSetActive(obj.gameObject, value);
		}
		return false;
	}

	public static GameObject GameObjectChildItem(GameObject obj)
	{
		if (obj != null && obj.gameObject != null && obj.transform.childCount > 0)
		{
			GameObject gameObject = obj.transform.GetChild(0).gameObject;
			GameObjectSetActive(gameObject, value: false);
			return gameObject;
		}
		return null;
	}

	public static long DateTimeToUnixTime(DateTime dateTime)
	{
		return (long)(dateTime - UnixEpoch).TotalSeconds;
	}

	public static void StartCoroutine(MonoBehaviour go, IEnumerator methodToCall, ref IEnumerator container)
	{
		if (container != null)
		{
			go.StopCoroutine(container);
		}
		container = methodToCall;
		go.StartCoroutine(container);
	}

	public static void StopCoroutine(MonoBehaviour go, ref IEnumerator container)
	{
		if (container != null)
		{
			go.StopCoroutine(container);
			container = null;
		}
	}

	public static void ClearUnusedMemory(bool gcCollect = false)
	{
		if (gcCollect)
		{
			GC.Collect();
		}
		Resources.UnloadUnusedAssets();
	}

	public static T GetCopyOf<T>(Component component, T other) where T : Component
	{
		Type type = component.GetType();
		if (type != other.GetType())
		{
			return null;
		}
		BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		PropertyInfo[] properties = type.GetProperties(bindingAttr);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.CanWrite)
			{
				try
				{
					propertyInfo.SetValue(component, propertyInfo.GetValue(other, null), null);
				}
				catch
				{
				}
			}
		}
		FieldInfo[] fields = type.GetFields(bindingAttr);
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(component, fieldInfo.GetValue(other));
		}
		return component as T;
	}

	public static T CopyComponent<T>(GameObject go, T to) where T : Component
	{
		return GetCopyOf(go.AddComponent(to.GetType()), to);
	}

	public static void CopyTransform(GameObject from, GameObject to)
	{
		from.transform.position = to.transform.position;
		from.transform.rotation = to.transform.rotation;
		from.transform.localScale = to.transform.localScale;
	}

	public static void CopyShader(Renderer from, Renderer to)
	{
		from.materials = to.materials;
		from.material.shader = to.material.shader;
		from.sharedMaterials = to.sharedMaterials;
	}

	public static void IterateByRenderType(GameObject go, Type[] renderTypes, ComponentTypeCallback callback)
	{
		foreach (Type type in renderTypes)
		{
			Component component = go.GetComponent(type);
			if (!(component == null))
			{
				callback?.Invoke(type, component);
			}
		}
	}

	public static string FormatReadableTime(TimeSpan duration)
	{
		int num;
		string textId;
		if (duration.TotalMinutes < 1.0)
		{
			num = duration.Seconds;
			textId = "Text.General.Timer.Seconds";
		}
		else if (duration.TotalHours < 1.0)
		{
			num = duration.Minutes;
			textId = "Text.General.Timer.Minutes";
		}
		else if (duration.TotalDays < 1.0)
		{
			num = duration.Hours;
			textId = "Text.General.Timer.Hours";
		}
		else
		{
			num = duration.Days;
			textId = "Text.General.Timer.Days";
		}
		return $"{num} {LocalizationManager.GetText(textId)}";
	}

	public static bool CanShowBreakthroughBtn(EquipmentItemModel equipment)
	{
		if (equipment != null)
		{
			if (equipment.CanBreakthrough)
			{
				return true;
			}
			if (equipment.BreakthroughLevel > 0 && equipment.GetMaxBreakThroughLevel() == equipment.BreakthroughLevel)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsBreakthroughMaxed(EquipmentItemModel equipment)
	{
		if (equipment != null && equipment.BreakthroughLevel > 0 && equipment.GetMaxBreakThroughLevel() == equipment.BreakthroughLevel)
		{
			return true;
		}
		return false;
	}

	public static bool IsApocalyptic(RewardEquipToken rewardEquipToken)
	{
		string equipTokenId = rewardEquipToken.EquipTokenId;
		if (GameManager.Instance.playerModel.gameEconomyData.GetEquipTokenDefinition(equipTokenId) != null)
		{
			return true;
		}
		return false;
	}

	public static bool GetBundleButtonSwitch()
	{
		bool result = false;
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.BundleButtonSwitchIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.BundleButtonSwitch;
	}

	public static bool GetIngameBanana()
	{
		bool result = false;
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.IngameBananaIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.IngameBanana;
	}

	public static bool GetBananaButtonSwitch()
	{
		bool result = false;
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.BananaButtonSwitchIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.BananaButtonSwitch;
	}

	public static bool GetOpenBananaButtonOnApp()
	{
		bool result = false;
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.OpenBananaButtonOnAppIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.OpenBananaButtonOnApp;
	}

	public static string GetShopFairBannerUrl()
	{
		string result = "";
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.IngameBananaImageIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.IngameBananaImage;
	}

	public static string GetShopFairBannerUrlINPACK()
	{
		string text = "";
		text = ((GetShopRoleType() != ShopRoleType.IOS) ? GameManager.Instance.gameEconomyData.ConfigData.IngameBananaImageINPACK : GameManager.Instance.gameEconomyData.ConfigData.IngameBananaImageIOSINPACK);
		if (string.IsNullOrEmpty(text))
		{
			text = "CommonAd";
		}
		return text;
	}

	public static string GetShopBundleBannerUrl()
	{
		string text = "";
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.BundleBananaImageIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.BundleBananaImage;
	}

	public static string GetShopBundleBannerUrlINPACK()
	{
		string text = "";
		text = ((GetShopRoleType() != ShopRoleType.IOS) ? GameManager.Instance.gameEconomyData.ConfigData.BundleBananaImageINPACK : GameManager.Instance.gameEconomyData.ConfigData.BundleBananaImageIOSINPACK);
		if (string.IsNullOrEmpty(text))
		{
			text = "CommonAd";
		}
		return text;
	}

	public static string GetBananaEnterButtonIcon()
	{
		string result = "";
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.BananaEnterButtonIconIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.BananaEnterButtonIcon;
	}

	public static string GetBananaPopupImage()
	{
		string result = "";
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.BananaPopupImageIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.BananaPopupImage;
	}

	public static bool IsNewShopVersion()
	{
		bool result = false;
		if (GameManager.Instance == null || GameManager.Instance.playerModel == null || GameManager.Instance.gameEconomyData == null || GameManager.Instance.gameEconomyData.ConfigData == null)
		{
			return result;
		}
		string switchBackOldVersion = GameManager.Instance.gameEconomyData.ConfigData.SwitchBackOldVersion;
		if (!string.IsNullOrEmpty(switchBackOldVersion) && switchBackOldVersion.ToLower() == "New".ToLower())
		{
			result = true;
		}
		return result;
	}

	public static bool GetInBananaTime()
	{
		bool result = false;
		if (!CheckBaseBanana())
		{
			return result;
		}
		DateTime utcTime = GameManager.Instance.playerModel.UtcTime;
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.InBananaTimeIOS(utcTime);
		}
		return GameManager.Instance.gameEconomyData.ConfigData.InBananaTime(utcTime);
	}

	public static bool CheckBaseBanana()
	{
		bool result = false;
		if (GameManager.Instance == null || GameManager.Instance.playerModel == null || GameManager.Instance.gameEconomyData == null || GameManager.Instance.gameEconomyData.ConfigData == null)
		{
			return result;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		ConfigData configData = GameManager.Instance.gameEconomyData.ConfigData;
		bool flag = false;
		bool flag2 = false;
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			flag = playerModel.CouncilLevel >= configData.BananaPopupLimitCouncilLevelIOS;
			flag2 = playerModel.TotalUSDSpent >= configData.RechargeLimitWBIOS;
		}
		else
		{
			flag = playerModel.CouncilLevel >= configData.BananaPopupLimitCouncilLevel;
			flag2 = playerModel.TotalUSDSpent >= configData.RechargeLimitWB;
		}
		return flag && flag2;
	}

	public static bool GetClickInternal()
	{
		bool result = false;
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.ClickInternalIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.ClickInternal;
	}

	public static bool GetFirstPic()
	{
		bool result = false;
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.FirstPicIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.FirstPic;
	}

	public static bool GetBundleBannerSwitch()
	{
		bool flag = false;
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.NewBannerSwitchBundleIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.NewBannerSwitchBundle;
	}

	public static bool GetFairBannerSwitch()
	{
		bool result = false;
		if (!CheckBaseBanana())
		{
			return result;
		}
		if (GetShopRoleType() == ShopRoleType.IOS)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.NewBannerSwitchTFIOS;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.NewBannerSwitchTF;
	}

	public static ShopRoleType GetShopRoleType()
	{
		ShopRoleType result = ShopRoleType.DefaultType;
		if (GameManager.Instance == null || GameManager.Instance.gameEconomyData == null || GameManager.Instance.gameEconomyData.ConfigData == null)
		{
			return result;
		}
		string countryCode = GameManager.GetCountryCode();
		GameManager.Instance.modelManager.GameEconomyData.ConfigData.IsInCountryControlIOS(countryCode);
		return ShopRoleType.DefaultType;
	}

	public static bool IsBackUpPassConfig(string missionID)
	{
		CombatRevertConfig[] combatRevertConfigs = GameManager.Instance.gameEconomyData.CombatRevertConfigs;
		if (combatRevertConfigs == null)
		{
			return true;
		}
		for (int i = 0; i < combatRevertConfigs.Length; i++)
		{
			if (combatRevertConfigs[i] != null && combatRevertConfigs[i].missionID == missionID && !combatRevertConfigs[i].isActive)
			{
				return false;
			}
		}
		return true;
	}

	public static bool CanEnterActiveFoundation()
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return false;
		}
		ActiveFoundationManager activeFoundationManager = playerModel.ActiveFoundationManager;
		if (activeFoundationManager == null || activeFoundationManager.CurrentPeriodId <= 0)
		{
			return false;
		}
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (modelManager == null || playerModel.CouncilLevel < modelManager.GameEconomyData.ActiveFoundationConfig.CouncilLockLevel)
		{
			return false;
		}
		return true;
	}

	public static void BackupEndUIEvent()
	{
		if (!(GameManager.Instance == null))
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			if (playerModel != null && playerModel.Combat != null)
			{
				if (OfflineManager.IsLoadDataManager)
				{
					DebugTWD.LogWarning("BackupEndUIEvent. Проверить что делается.");
				}
				UIEvent.Send("BackupEndEvent");
			}
		}
	}

	public static List<TypeDefinition> GetItemTypes()
	{
		TypeDefinition[] typeDefinitions = GameManager.Instance.gameEconomyData.TypeDefinitions;
		if (typeDefinitions == null || typeDefinitions.Length == 0)
		{
			return null;
		}
		return typeDefinitions.OrderBy((TypeDefinition t) => t.Order).ToList();
	}

	public static ItemDefinition GetDefaultItemDefinition()
	{
		List<TypeDefinition> itemTypes = GetItemTypes();
		if (itemTypes == null || itemTypes.Count <= 0)
		{
			return null;
		}
		TypeDefinition typeDefinition = itemTypes[0];
		if (typeDefinition.ItemDefinitions == null || typeDefinition.ItemDefinitions.Count <= 1)
		{
			return null;
		}
		return typeDefinition.ItemDefinitions.First((ItemDefinition t) => !t.IsSubType);
	}

	public static TypeDefinition GetTypeDefinition(ItemDefinition itemDefinition)
	{
		TypeDefinition[] typeDefinitions = GameManager.Instance.gameEconomyData.TypeDefinitions;
		if (typeDefinitions == null || typeDefinitions.Length == 0)
		{
			return null;
		}
		for (int i = 0; i < typeDefinitions.Length; i++)
		{
			if (typeDefinitions[i] != null)
			{
				TypeDefinition typeDefinition = typeDefinitions[i];
				if (typeDefinition.ItemDefinitions.Contains(itemDefinition))
				{
					return typeDefinition;
				}
			}
		}
		return null;
	}

	public static bool CanEnterThreeDay()
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return false;
		}
		ThreeDayModel threeDayModel = playerModel.ThreeDayModel;
		if (threeDayModel == null)
		{
			return false;
		}
		if (threeDayModel.CanShowThreeDay)
		{
			return true;
		}
		return false;
	}

	public static void ReturnCamp()
	{
		if (!(GameManager.Instance == null) && GameManager.Instance.playerModel != null)
		{
			if (!OfflineManager.IsLoadDataManager && GameManager.Instance.IsConnectedToServer)
			{
				GameManager.Instance.SendWebShopRequest();
			}
			DebugTWD.Log("ReturnCamp. ExecuteCommand new RFMEventCommand", DebugType.Load);
			ExecuteCommand(new RFMEventCommand(RFMEvent.ReturnCamp));
			if (!IsConditionOpened())
			{
				TryOpenConditionBundle();
			}
		}
	}

	public static void OpenMiniShopEvent(CurrencyType currencyType)
	{
		if (!(GameManager.Instance == null) && GameManager.Instance.playerModel != null && currencyType == CurrencyType.Diamonds)
		{
			ExecuteCommand(new RFMEventCommand(RFMEvent.insufficientGold));
		}
	}

	private static bool IsConditionOpened()
	{
		if (TWDPlayerPrefs.GetInt(ConditionOpenedPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetConditionOpened(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(ConditionOpenedPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(ConditionOpenedPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static void TryOpenConditionBundle()
	{
		ConditionBundleDefinition firstConditionBundle = GetFirstConditionBundle();
		if (firstConditionBundle != null)
		{
			GameManager.Instance.BundleSource = Metrics.BundleSource.ConditionBundle;
			BundleCardPopup.OpenBundle(firstConditionBundle.BundleIdentifier);
			SetConditionOpened(on: true);
		}
	}

	public static ConditionBundleDefinition GetFirstConditionBundle()
	{
		List<string> currentGift = GameManager.Instance.playerModel.RFMGiftManager.CurrentGift;
		if (currentGift == null || currentGift.Count <= 0)
		{
			return null;
		}
		return GameManager.Instance.gameEconomyData.GetConditionBundleDefinition(currentGift[0]);
	}

	public static bool IsItemListOpened()
	{
		if (TWDPlayerPrefs.GetInt(ItemListOpenedPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetItemListOpened(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(ItemListOpenedPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(ItemListOpenedPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static bool CanEnterItemList()
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null)
		{
			return false;
		}
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		if (gameEconomyData == null)
		{
			return false;
		}
		if (!gameEconomyData.ConfigData.ItemListSwitch)
		{
			return false;
		}
		return gameEconomyData.IsInSpenderTier(playerModel, gameEconomyData.ConfigData.ItemListUnlockLimit);
	}

	public static bool IsSeasonMapAllCompleted()
	{
		SeasonDefinition[] seasonDefinitions = GameManager.Instance.gameEconomyData.SeasonDefinitions;
		if (seasonDefinitions == null || seasonDefinitions.Length == 0)
		{
			return true;
		}
		StoryTellerModel storyTeller = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller;
		if (storyTeller.GetCurrentUncompletedQuestDefinition() == null || storyTeller.GetCurrentUncompletedQuestDefinition().Order <= 0)
		{
			return false;
		}
		for (int i = 0; i < seasonDefinitions.Length; i++)
		{
			if (seasonDefinitions[i] != null && !string.IsNullOrEmpty(seasonDefinitions[i].Id))
			{
				MapMissionGroupModel seasonCurrentMapMissionGroup = DetailMapPopUp.GetSeasonCurrentMapMissionGroup(GameManager.Instance.gameEconomyData.GetSeasonDefinition(seasonDefinitions[i].Id));
				if (seasonCurrentMapMissionGroup != null && (seasonCurrentMapMissionGroup.GetNonCompletedMissionsCount() != 0 || !seasonCurrentMapMissionGroup.AreAllStoryMissionsCompleted()))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool IsStoryMapAllCompleted()
	{
		MapContainerModel mapContainerModel = GameManager.Instance.playerModel.MapContainerModel;
		if (mapContainerModel == null)
		{
			return true;
		}
		foreach (MissionSpawnPointGroup mapDefinition in GameManager.Instance.gameEconomyData.MapDefinitions)
		{
			if (mapDefinition == null || mapDefinition.Category != MapCategory.Story)
			{
				continue;
			}
			foreach (MissionSpawnPoint missionSpawnPoint in mapDefinition.MissionSpawnPoints)
			{
				if (missionSpawnPoint == null)
				{
					continue;
				}
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = mapContainerModel.GetMissionGroupModelForSpawnPointGroup(missionSpawnPoint.OwningGroup);
				if (missionGroupModelForSpawnPointGroup != null && !missionGroupModelForSpawnPointGroup.IsDisabledOnGED)
				{
					MapMissionGroupModel currentEpisodeDifficultyGroupModel = missionGroupModelForSpawnPointGroup.GetCurrentEpisodeDifficultyGroupModel();
					bool num = missionGroupModelForSpawnPointGroup.AreAllStoryMissionsCompleted();
					bool flag = GameManager.Instance.playerModel.MapContainerModel.GetHarderVersion(currentEpisodeDifficultyGroupModel) == null;
					if (!num)
					{
						return false;
					}
					if (missionGroupModelForSpawnPointGroup.GetNonCompletedMissionsCount() != 0)
					{
						return false;
					}
					if (!flag)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public static int GetNumsForIReward(IReward reward)
	{
		if (reward == null)
		{
			return -1;
		}
		int result = -1;
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (!(reward is RewardCurrency rewardCurrency))
		{
			if (!(reward is RewardEquipment rewardEquipment))
			{
				if (reward is RewardEquipToken rewardEquipToken)
				{
					result = rewardEquipToken.RewardAmount;
				}
			}
			else if (rewardEquipment.IsConsumableReward(modelManager))
			{
				result = rewardEquipment.Amount;
			}
			else
			{
				RewardEquipment rewardEquipment2 = rewardEquipment;
				if (!rewardEquipment2.IsConsumableReward(modelManager))
				{
					result = rewardEquipment2.Amount;
				}
			}
		}
		else
		{
			result = rewardCurrency.Amount;
		}
		return result;
	}

	public static bool IsCombatSkillSelectableStatus()
	{
		bool result = false;
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (combat != null && combatHUD != null)
		{
			return combatHUD.IsSkillSelectableStatus;
		}
		return result;
	}

	public static string GetPlatformName(RuntimePlatform platform)
	{
		string result = platform.ToString();
		if (platform == RuntimePlatform.IPhonePlayer)
		{
			result = "iOS";
		}
		return result;
	}

	public static string GetBananaURL()
	{
		if (GameManager.ActiveBranch.Contains("staging"))
		{
			return GameManager.Instance.gameEconomyData.ConfigData.BananaStagingURL;
		}
		if (GameManager.ActiveBranch.Contains("test"))
		{
			return GameManager.Instance.gameEconomyData.ConfigData.BananaTestURL;
		}
		if (GameManager.ActiveBranch.Contains("develop"))
		{
			return GameManager.Instance.gameEconomyData.ConfigData.BananaDevURL;
		}
		return GameManager.Instance.gameEconomyData.ConfigData.BananaURL;
	}

	public static bool IsSurvivalManualPlotGuidenOpened()
	{
		if (TWDPlayerPrefs.GetInt(SurvivalManualPlotGuidePopupPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetSurvivalManualPlotGuideOpened(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(SurvivalManualPlotGuidePopupPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(SurvivalManualPlotGuidePopupPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static bool IsSurvivalManualShow()
	{
		PlayerModel player = GameManager.Instance.modelManager.Player;
		SystemOpen systemOpenById = player.gameEconomyData.GetSystemOpenById("SystemBase.Survival_Manual");
		if (systemOpenById != null && player.CouncilLevel >= systemOpenById.ShowCampLv)
		{
			return true;
		}
		return false;
	}

	public static bool IsSurvivalManualOpen()
	{
		return GameManager.Instance.modelManager.Player.SurvivalManualManager.IsSystemBaseSurvivalManualOpen();
	}

	public static bool IsActorSheetSurvivalManualOpen()
	{
		return GameManager.Instance.modelManager.Player.SurvivalManualManager.IsSystemBaseActorSheetSurvivalManualOpen();
	}

	public static string GetSurvivalManualNotOpenTips()
	{
		SystemOpen systemOpenById = GameManager.Instance.modelManager.Player.gameEconomyData.GetSystemOpenById("SystemBase.ActorSheet.Survival_Manual");
		if (systemOpenById == null)
		{
			return "";
		}
		return LocalizationManager.GetText(systemOpenById.UnOpenedTips, systemOpenById.OpenCampLv);
	}

	public static List<int> GetSurvivalManualStorySkillList(string actorId)
	{
		PlayerModel player = GameManager.Instance.modelManager.Player;
		if (!player.SurvivalManualManager.ActorSurvivalManualStorySkillList.ContainsKey(actorId))
		{
			return null;
		}
		List<int> list = new List<int>();
		List<SurvivalManualActorStoryLockTrait> list2 = player.SurvivalManualManager.ActorSurvivalManualStorySkillList[actorId];
		if (list2 != null && list2.Count > 0)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				if (list2[i] != null)
				{
					list.Add(list2[i].SurvivalManualID);
				}
			}
		}
		return list;
	}

	public static int GetRedSurvivalManualNum()
	{
		int num = 0;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return 0;
		}
		if (CanUpgradeSurvivalManualAttribute())
		{
			num++;
		}
		ModelList<SurvivalManualModel> survivalManualModels = survivalManualManager.SurvivalManualModels;
		if (survivalManualModels != null && survivalManualModels.Count > 0)
		{
			for (int i = 0; i < survivalManualModels.Count; i++)
			{
				if (survivalManualModels[i] != null && (IsRedSurvivalManual_StoryId(survivalManualModels[i].ID) || CanSurvivalManualStorySkillUpgrade(survivalManualModels[i].ID)))
				{
					num++;
				}
			}
		}
		return num;
	}

	public static bool IsRedSurvivalManual_stories()
	{
		bool result = false;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return false;
		}
		ModelList<SurvivalManualModel> survivalManualModels = survivalManualManager.SurvivalManualModels;
		if (survivalManualModels != null && survivalManualModels.Count > 0)
		{
			for (int i = 0; i < survivalManualModels.Count; i++)
			{
				if (survivalManualModels[i] != null && (IsRedSurvivalManual_StoryId(survivalManualModels[i].ID) || CanSurvivalManualStorySkillUpgrade(survivalManualModels[i].ID)))
				{
					return true;
				}
			}
		}
		return result;
	}

	public static bool IsRedSurvivalManual_Hero(int survivalManualDefinitionId, string storyActorID)
	{
		bool result = false;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return false;
		}
		if (survivalManualManager.GetSurvivalManualModel(survivalManualDefinitionId) == null)
		{
			return false;
		}
		if (CanSurvivalManualHeroUpgrade(survivalManualDefinitionId, storyActorID))
		{
			return true;
		}
		List<SurvivalManualActorStory> survivalManualActorStories = GameManager.Instance.playerModel.gameEconomyData.GetSurvivalManualActorStories(storyActorID);
		if (survivalManualActorStories != null && survivalManualActorStories.Count > 0)
		{
			for (int i = 0; i < survivalManualActorStories.Count; i++)
			{
				if (CanSurvivalManualStoryUnlock(survivalManualDefinitionId, storyActorID, survivalManualActorStories[i].MemoryID))
				{
					return true;
				}
			}
		}
		return result;
	}

	public static bool IsRedSurvivalManual_Hero(SurvivorModel survivorModel)
	{
		if (survivorModel == null)
		{
			return false;
		}
		bool result = false;
		List<int> survivalManualStorySkillList = GetSurvivalManualStorySkillList(survivorModel.ActorDefinitionID);
		if (survivalManualStorySkillList == null || survivalManualStorySkillList.Count <= 0)
		{
			return false;
		}
		survivalManualStorySkillList.RemoveAll((int t) => t <= 0);
		string survivalManualStoryId = GameManager.Instance.playerModel.gameEconomyData.GetSurvivalManualStoryId(survivorModel.ActorDefinitionID);
		if (string.IsNullOrEmpty(survivalManualStoryId))
		{
			return false;
		}
		if (survivalManualStorySkillList != null && survivalManualStorySkillList.Count == 1)
		{
			result = IsRedSurvivalManual_Hero(survivalManualStorySkillList[0], survivalManualStoryId);
		}
		if (survivalManualStorySkillList != null && survivalManualStorySkillList.Count == 2)
		{
			result = IsRedSurvivalManual_Hero(survivalManualStorySkillList[0], survivalManualStoryId) || IsRedSurvivalManual_Hero(survivalManualStorySkillList[1], survivalManualStoryId);
		}
		return result;
	}

	public static bool IsRedSurvivalManual_StoryId(int survivalManualDefinitionId)
	{
		bool result = false;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return false;
		}
		SurvivalManualModel survivalManualModel = survivalManualManager.GetSurvivalManualModel(survivalManualDefinitionId);
		if (survivalManualModel == null)
		{
			return false;
		}
		List<string> actorList = survivalManualModel.SurvivalManualDefinition.ActorList;
		if (actorList != null && actorList.Count > 0)
		{
			for (int i = 0; i < actorList.Count; i++)
			{
				if (IsRedSurvivalManual_Hero(survivalManualDefinitionId, actorList[i]))
				{
					return true;
				}
			}
			return false;
		}
		return result;
	}

	public static bool IsRedSurvivalManual_StoryUpgradeLevel(int survivalManualDefinitionId)
	{
		bool result = false;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return false;
		}
		SurvivalManualModel survivalManualModel = survivalManualManager.GetSurvivalManualModel(survivalManualDefinitionId);
		if (survivalManualModel == null)
		{
			return false;
		}
		List<string> actorList = survivalManualModel.SurvivalManualDefinition.ActorList;
		if (actorList != null && actorList.Count > 0)
		{
			for (int i = 0; i < actorList.Count; i++)
			{
				if (CanSurvivalManualHeroUpgrade(survivalManualDefinitionId, actorList[i]))
				{
					return true;
				}
			}
			return false;
		}
		return result;
	}

	public static bool CanUpgradeSurvivalManualAttribute()
	{
		bool result = false;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return false;
		}
		if (survivalManualManager.CanUpgradeSurvivalManualAttributeLeve() != SurvivalManualType.UpgradeCondition)
		{
			return false;
		}
		Cashier cashier = new Cashier(GameManager.Instance.modelManager);
		foreach (KeyValuePair<CurrencyType, int> item in survivalManualManager.SkillDefinition.GetUpgradCostInfo())
		{
			CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSurvivalManualSkill);
			CurrencyType key = item.Key;
			int value = item.Value;
			cashierItem.SetCost(key, value);
			cashier.AddItem(cashierItem);
		}
		if (cashier.CanAfford())
		{
			result = true;
		}
		return result;
	}

	public static bool CanSurvivalManualStorySkillUpgrade(int survivalManualDefinitionId)
	{
		bool result = false;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return false;
		}
		SurvivalManualModel survivalManualModel = survivalManualManager.GetSurvivalManualModel(survivalManualDefinitionId);
		if (survivalManualModel == null)
		{
			return false;
		}
		if (survivalManualManager.GetSurvivalManualStorySkillCanUpgradeState(survivalManualDefinitionId) != SurvivalManualType.UpgradeCondition)
		{
			return false;
		}
		Cashier cashier = new Cashier(GameManager.Instance.modelManager);
		foreach (KeyValuePair<CurrencyType, int> item in survivalManualModel.SkillDefinition.GetUpgradCostInfo())
		{
			CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSurvivalManualStorySkill);
			cashierItem.SetCost(item.Key, item.Value);
			cashier.AddItem(cashierItem);
		}
		if (cashier.CanAfford())
		{
			result = true;
		}
		return result;
	}

	public static bool CanSurvivalManualHeroUpgrade(int survivalManualDefinitionId, string storyActorID)
	{
		bool result = false;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return false;
		}
		SurvivalManualModel survivalManualModel = survivalManualManager.GetSurvivalManualModel(survivalManualDefinitionId);
		if (survivalManualModel == null)
		{
			return false;
		}
		if (survivalManualModel.GetStoryActorCanUpgradeState(storyActorID) == StoryActorType.Upgradable)
		{
			result = true;
		}
		return result;
	}

	public static bool CanSurvivalManualStoryUnlock(int survivalManualDefinitionId, string storyActorID, int memoryId)
	{
		bool result = false;
		SurvivalManualManager survivalManualManager = GameManager.Instance.playerModel.SurvivalManualManager;
		if (survivalManualManager == null)
		{
			return false;
		}
		SurvivalManualModel survivalManualModel = survivalManualManager.GetSurvivalManualModel(survivalManualDefinitionId);
		if (survivalManualModel == null)
		{
			return false;
		}
		if (survivalManualModel.GetSurvivalManualStoryUnlockStatus(storyActorID, memoryId) == StoryUnlockStatus.Unlockable)
		{
			result = true;
		}
		return result;
	}

	public static bool IsChallengeRewardTipsOpen()
	{
		PlayerModel player = GameManager.Instance.modelManager.Player;
		SystemOpen systemOpenById = player.gameEconomyData.GetSystemOpenById("SystemBase.ChallengeRewardTips");
		if (systemOpenById == null)
		{
			return false;
		}
		if (player.UtcTimeStamp >= systemOpenById.StartTimeMilliseconds && player.UtcTimeStamp <= systemOpenById.EndTimeMilliseconds)
		{
			return true;
		}
		return false;
	}

	public static bool IsPCPlatform()
	{
#if UNITY_ANDROID
		return false;
#else
		return true;
#endif
	}

	public static void GoBanana()
	{
		if (GameManager.Instance.gameEconomyData?.ConfigData == null)
		{
			return;
		}
		if (GetClickInternal())
		{
			if (GameManager.Instance.IsConnectedToServer)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
				SignalRClient.Instance.RequestCommand("GetBananaLoginCode", OnGetTransferCode, waitForResponse: true);
			}
		}
		else
		{
			ShopPopupHelper.OpenWithIndex(2);
		}
	}

	private static void OnGetTransferCode(string message)
	{
		if (CheckError(message))
		{
			return;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		TransferCode transferCode = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<TransferCode>(message);
		if (transferCode != null && !string.IsNullOrEmpty(transferCode.Code))
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			string bananaURL = GetBananaURL();
			if (playerModel != null && playerModel.HashedId != null)
			{
				string text = Convert.ToBase64String(Encoding.UTF8.GetBytes("ydldeca" + playerModel.HashedId + "twd"));
				string deviceId = GameManager.Instance.LoginRequest.Device.DeviceId;
				bananaURL = bananaURL + "?id=" + text + "&code=" + transferCode.Code + "&DeviceId=" + deviceId + "&OS=" + GetPlatformName(Application.platform);
				Application.OpenURL(bananaURL);
			}
		}
		else
		{
			CheckError("");
		}
	}

	private static bool CheckError(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			AlertPopup.ShowPopupGetText("Error.Error", "Error.ErrorGeneric", "Button.Ok", null);
			return true;
		}
		return false;
	}

	public static bool IsSPRemoldNotFirstOpen()
	{
		if (TWDPlayerPrefs.GetInt(SPRemoldNotFirstOpenPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetSPRemoldNotFirstOpen(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(SPRemoldNotFirstOpenPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(SPRemoldNotFirstOpenPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static bool IsSPRemoldEasy()
	{
		if (TWDPlayerPrefs.GetInt(SPRemoldEasyPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetSPRemoldEasy(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(SPRemoldEasyPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(SPRemoldEasyPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static bool IsSPRemold24Comfirm()
	{
		if (TWDPlayerPrefs.GetInt(SPRemold24ComfirmPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetSPRemold24Comfirm(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(SPRemold24ComfirmPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(SPRemold24ComfirmPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static void SetSPRemold24ComfirmTimePlayerPrefs()
	{
		TWDPlayerPrefs.SetString(SPRemold24ComfirmTimePlayerPrefs, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
		TWDPlayerPrefs.Save();
	}

	public static bool IsSPRemold24ComfirmTimeOver()
	{
		string text = TWDPlayerPrefs.GetString(SPRemold24ComfirmTimePlayerPrefs);
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(text));
		return (DateTimeOffset.UtcNow - dateTimeOffset).TotalHours >= 24.0;
	}

	public static bool IsSPRemold24UpgradeComfirm()
	{
		if (TWDPlayerPrefs.GetInt(SPRemold24UpgradeComfirmPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetSPRemold24UpgradeComfirm(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(SPRemold24UpgradeComfirmPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(SPRemold24UpgradeComfirmPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static void SetSPRemold24UpgradeComfirmTimePlayerPrefs()
	{
		TWDPlayerPrefs.SetString(SPRemold24UpgradeComfirmTimePlayerPrefs, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
		TWDPlayerPrefs.Save();
	}

	public static bool IsSPRemold24UpgradeComfirmTimeOver()
	{
		string text = TWDPlayerPrefs.GetString(SPRemold24UpgradeComfirmTimePlayerPrefs);
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(text));
		return (DateTimeOffset.UtcNow - dateTimeOffset).TotalHours >= 24.0;
	}

	public static Color HexToColor(string hex)
	{
		if (ColorUtility.TryParseHtmlString(hex, out var color))
		{
			return color;
		}
		return Color.white;
	}

	public static string GetRateStrForPreviewMin(string equipIdentifier)
	{
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (modelManager == null || modelManager.Player == null)
		{
			return null;
		}
		GameEconomyData gameEconomyData = modelManager.GameEconomyData;
		if (gameEconomyData == null)
		{
			return null;
		}
		EquipmentDefinition equipmentDefinition = gameEconomyData.GetEquipmentDefinition(equipIdentifier);
		if (equipmentDefinition == null)
		{
			return null;
		}
		int num = 0;
		if (equipmentDefinition.SPTraitsRemoldType != null && equipmentDefinition.SPTraitsRemoldType.Count > 0)
		{
			foreach (string item in equipmentDefinition.SPTraitsRemoldType)
			{
				if (string.IsNullOrEmpty(item))
				{
					continue;
				}
				List<SPTraitsRemoldDefinitions> sPTraitsRemodeDefinitionByType = gameEconomyData.GetSPTraitsRemodeDefinitionByType(item);
				if (sPTraitsRemodeDefinitionByType != null && sPTraitsRemodeDefinitionByType.Count != 0)
				{
					int minLevel = sPTraitsRemodeDefinitionByType.Min((SPTraitsRemoldDefinitions t) => t.Level);
					SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions = sPTraitsRemodeDefinitionByType.FirstOrDefault((SPTraitsRemoldDefinitions t) => t.Level == minLevel);
					num += ((sPTraitsRemoldDefinitions.Star == 0) ? 1 : sPTraitsRemoldDefinitions.Star) * ((sPTraitsRemoldDefinitions.Value == 0) ? 1 : sPTraitsRemoldDefinitions.Value) * ((sPTraitsRemoldDefinitions.Level == 0) ? 1 : sPTraitsRemoldDefinitions.Level);
				}
			}
		}
		return gameEconomyData.SPTraitsRemoldConfigs.GetRatingByScore(num);
	}

	public static string GetRateStrForPreviewMax(string equipIdentifier)
	{
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (modelManager == null || modelManager.Player == null)
		{
			return null;
		}
		GameEconomyData gameEconomyData = modelManager.GameEconomyData;
		if (gameEconomyData == null)
		{
			return null;
		}
		EquipmentDefinition equipmentDefinition = gameEconomyData.GetEquipmentDefinition(equipIdentifier);
		if (equipmentDefinition == null)
		{
			return null;
		}
		int num = 0;
		if (equipmentDefinition.SPTraitsRemoldType != null && equipmentDefinition.SPTraitsRemoldType.Count > 0)
		{
			foreach (string item in equipmentDefinition.SPTraitsRemoldType)
			{
				if (string.IsNullOrEmpty(item))
				{
					continue;
				}
				List<SPTraitsRemoldDefinitions> sPTraitsRemodeDefinitionByType = gameEconomyData.GetSPTraitsRemodeDefinitionByType(item);
				if (sPTraitsRemodeDefinitionByType != null && sPTraitsRemodeDefinitionByType.Count != 0)
				{
					int maxLevel = sPTraitsRemodeDefinitionByType.Max((SPTraitsRemoldDefinitions t) => t.Level);
					SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions = sPTraitsRemodeDefinitionByType.FirstOrDefault((SPTraitsRemoldDefinitions t) => t.Level == maxLevel);
					num += ((sPTraitsRemoldDefinitions.Star == 0) ? 1 : sPTraitsRemoldDefinitions.Star) * ((sPTraitsRemoldDefinitions.Value == 0) ? 1 : sPTraitsRemoldDefinitions.Value) * ((sPTraitsRemoldDefinitions.Level == 0) ? 1 : sPTraitsRemoldDefinitions.Level);
				}
			}
		}
		return gameEconomyData.SPTraitsRemoldConfigs.GetRatingByScore(num);
	}

	public static SPTraitsRemoldDefinitions GetMaxLevelSPTraitsRemodeDefinition(string currentTraitId)
	{
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (modelManager == null || modelManager.Player == null)
		{
			return null;
		}
		GameEconomyData gameEconomyData = modelManager.GameEconomyData;
		if (gameEconomyData == null)
		{
			return null;
		}
		SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = gameEconomyData.GetSPTraitsRemodeDefinition(currentTraitId);
		if (sPTraitsRemodeDefinition == null)
		{
			return null;
		}
		SPTraitsRemoldDefinitions[] sPTraitsRemodeDefinition2 = gameEconomyData.SPTraitsRemodeDefinition;
		if (sPTraitsRemodeDefinition2 == null)
		{
			return null;
		}
		string id = null;
		int num = 0;
		SPTraitsRemoldDefinitions[] array = sPTraitsRemodeDefinition2;
		foreach (SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions in array)
		{
			if (!(sPTraitsRemoldDefinitions.Type != sPTraitsRemodeDefinition.Type) && sPTraitsRemoldDefinitions.Level > num)
			{
				num = sPTraitsRemoldDefinitions.Level;
				id = sPTraitsRemoldDefinitions.ID;
			}
		}
		return gameEconomyData.GetSPTraitsRemodeDefinition(id);
	}

	public static List<SPTraitSlot> GetSPTraitSlotsForPreview(string equipIdentifier)
	{
		List<SPTraitSlot> list = new List<SPTraitSlot>();
		EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(equipIdentifier);
		if (equipmentDefinition.SPTraitsRemoldType != null && equipmentDefinition.SPTraitsRemoldType.Count > 0)
		{
			foreach (string item in equipmentDefinition.SPTraitsRemoldType)
			{
				if (string.IsNullOrEmpty(item))
				{
					continue;
				}
				List<SPTraitsRemoldDefinitions> sPTraitsRemodeDefinitionByType = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinitionByType(item);
				if (sPTraitsRemodeDefinitionByType != null && sPTraitsRemodeDefinitionByType.Count != 0)
				{
					int minLevel = sPTraitsRemodeDefinitionByType.Min((SPTraitsRemoldDefinitions t) => t.Level);
					SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions = sPTraitsRemodeDefinitionByType.FirstOrDefault((SPTraitsRemoldDefinitions t) => t.Level == minLevel);
					list.Add(new SPTraitSlot(sPTraitsRemoldDefinitions.ID));
					if (list.Count >= 6)
					{
						break;
					}
				}
			}
		}
		if (list.Count > 6)
		{
			list = list.Take(6).ToList();
		}
		foreach (SPTraitSlot item2 in list)
		{
			if (item2 == null)
			{
				continue;
			}
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(item2.ID);
			if (sPTraitsRemodeDefinition != null)
			{
				if (sPTraitsRemodeDefinition.Locked)
				{
					item2.LockState = SPTraitsLockState.ForceLocked;
				}
				else
				{
					item2.LockState = SPTraitsLockState.Unlocked;
				}
				item2.Level = sPTraitsRemodeDefinition.Level;
				item2.MaxLevel = sPTraitsRemodeDefinition.MaxLevel;
				item2.CanUpgrade = sPTraitsRemodeDefinition.UpgradeType == 1;
			}
		}
		return list;
	}

	public static bool IsSpEquipmentRemoldAllLocked(EquipmentItemModel equipmentItemModel)
	{
		if (equipmentItemModel == null || equipmentItemModel.SpEquipmentRemoldModel == null)
		{
			return false;
		}
		SpEquipmentRemoldModel spEquipmentRemoldModel = equipmentItemModel.SpEquipmentRemoldModel;
		List<SPTraitSlot> list = (spEquipmentRemoldModel.HasPendingRemold ? spEquipmentRemoldModel.PendingSPTraitSlots : spEquipmentRemoldModel.SPTraitSlots);
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		int num = 0;
		foreach (SPTraitSlot item in list)
		{
			if (item != null && (item.LockState == SPTraitsLockState.Locked || item.LockState == SPTraitsLockState.ForceLocked))
			{
				num++;
			}
		}
		return num >= spEquipmentRemoldModel.SPTraitSlots.Count;
	}

	public static bool IsInSpenderTier(string spenderTierId)
	{
		PlayerModel player = GameManager.Instance.modelManager.Player;
		if (player == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(spenderTierId))
		{
			return true;
		}
		long secondsSinceLastPurchaseThatCostMoney = player.BundleManager.GetSecondsSinceLastPurchaseThatCostMoney();
		return GameManager.Instance.gameEconomyData.IsInSpenderTier(player, spenderTierId, player.TotalUSDSpent, (int)player.LifeTimeInDays, player.GetTotalPurchases(), secondsSinceLastPurchaseThatCostMoney, player.CreationTimeStamp, player.CouncilLevel);
	}

	public static bool IsInSpenderTier(List<string> spenderTierIds)
	{
		if (spenderTierIds == null || spenderTierIds.Count <= 0)
		{
			return true;
		}
		return spenderTierIds.All((string spenderTierId) => IsInSpenderTier(spenderTierId));
	}

	public static bool IsSkillKitNotice()
	{
		foreach (SurvivorClass value in Enum.GetValues(typeof(SurvivorClass)))
		{
			if (IsSkillKitNotice(value))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsSkillKitNotice(SurvivorClass filterSurvivorClass)
	{
		List<ModSkillMode> acquiredModSkillsByClass = GameManager.Instance.playerModel.ModSkillManager.GetAcquiredModSkillsByClass(filterSurvivorClass);
		List<ModSkillMode> unlockableModSkills = GameManager.Instance.playerModel.ModSkillManager.GetUnlockableModSkills(filterSurvivorClass);
		bool flag = false;
		if (acquiredModSkillsByClass.Count > 0)
		{
			flag = acquiredModSkillsByClass.Any((ModSkillMode skill) => skill.ModSkillUpState == ModSkillUpState.Upgraded && !skill.IsMaxLevel());
		}
		bool flag2 = false;
		if (unlockableModSkills.Count > 0)
		{
			flag2 = unlockableModSkills.Any((ModSkillMode skill) => skill.ModSkillLockState == ModSkillLockState.CanUnlock);
		}
		return flag || flag2;
	}

	public static bool IsSystemOpenById(string systemId)
	{
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (modelManager == null)
		{
			return false;
		}
		GameEconomyData gameEconomyData = modelManager.GameEconomyData;
		if (gameEconomyData == null)
		{
			return false;
		}
		SystemOpen systemOpenById = gameEconomyData.GetSystemOpenById(systemId);
		if (systemOpenById == null)
		{
			return false;
		}
		if (modelManager.Player.CouncilLevel < systemOpenById.OpenCampLv)
		{
			return false;
		}
		if (systemOpenById.HasDateLimit)
		{
			long utcTimeStamp = modelManager.Player.UtcTimeStamp;
			if (utcTimeStamp < systemOpenById.StartTimeMilliseconds || utcTimeStamp > systemOpenById.EndTimeMilliseconds)
			{
				return false;
			}
		}
		return true;
	}

	public static TraitDefinition GetApocalypticTraitDefinitionByEquipmentDefinitionId(string equipmentDefinitionIdentifier)
	{
		EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(equipmentDefinitionIdentifier);
		if (equipmentDefinition == null)
		{
			return null;
		}
		EquipmentItemModel equipmentItemModel = GameManager.Instance.modelManager.Player.Equipment.GetAllEquipments().FirstOrDefault((EquipmentItemModel e) => e.Definition.ID == equipmentDefinitionIdentifier);
		List<string> list = equipmentItemModel?.GetEquipmentActiveTraits();
		List<string> list2 = equipmentItemModel?.GetEquipmentPassiveTraits();
		List<string> list3 = new List<string>();
		if (list != null)
		{
			list3.AddRange(list);
		}
		else if (equipmentDefinition?.ActiveTraits != null)
		{
			list3.AddRange(equipmentDefinition.ActiveTraits);
		}
		if (list2 != null)
		{
			list3.AddRange(list2);
		}
		else if (equipmentDefinition?.PassiveTraits != null)
		{
			list3.AddRange(equipmentDefinition.PassiveTraits);
		}
		for (int num = 0; num < list3.Count; num++)
		{
			string text = list3[num];
			if (text.Contains("Equipment_Apocalyptic_DMG") || text.Contains("Equipment_Apocalyptic_BS") || text.Contains("Equipment_Apocalyptic_DEF"))
			{
				TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(text);
				if (traitDefinition != null)
				{
					return traitDefinition;
				}
			}
		}
		return null;
	}

	public static string GetApocalypticIconNameByTraitIdentifier(string traitIdentifier)
	{
		if (traitIdentifier.Contains("Equipment_Apocalyptic_DMG"))
		{
			return "Equipment_Apocalyptic_DMG";
		}
		if (traitIdentifier.Contains("Equipment_Apocalyptic_BS"))
		{
			return "Equipment_Apocalyptic_BS";
		}
		if (traitIdentifier.Contains("Equipment_Apocalyptic_DEF"))
		{
			return "Equipment_Apocalyptic_DEF";
		}
		return "";
	}

	public static bool IsSkillWeaponBagOpened()
	{
		if (TWDPlayerPrefs.GetInt(SkillWeaponBagOpenedPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetSkillWeaponBagOpened(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(SkillWeaponBagOpenedPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(SkillWeaponBagOpenedPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static bool IsSkillBagOpened()
	{
		if (TWDPlayerPrefs.GetInt(SkillBagOpenedPlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public static void SetSkillBagOpened(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(SkillBagOpenedPlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(SkillBagOpenedPlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public static CurrencyType GetSurvivorClassCurrencyType(SurvivorClass survivorClass)
	{
		return survivorClass switch
		{
			SurvivorClass.Warrior => CurrencyType.CBPWarrior,
			SurvivorClass.Scout => CurrencyType.CBPScout,
			SurvivorClass.Bruiser => CurrencyType.CBPBruiser,
			SurvivorClass.Shooter => CurrencyType.CBPShooter,
			SurvivorClass.Hunter => CurrencyType.CBPHunter,
			SurvivorClass.Assault => CurrencyType.CBPAssault,
			_ => CurrencyType.None,
		};
	}

	public static EquipmentUpgradePopup OpenEquipmentUpgradePopup(EquipmentItemModel equipmentItemModel)
	{
		if (equipmentItemModel == null)
		{
			return null;
		}
		EquipmentUpgradePopup equipmentUpgradePopup;
		if (equipmentItemModel.Definition != null && equipmentItemModel.Definition.SwitchRemoldMode)
		{
			equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopupNew) as EquipmentUpgradePopup;
		}
		else
		{
			equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
		}
		if (equipmentUpgradePopup != null)
		{
			equipmentUpgradePopup.ShowNextLevel = true;
			equipmentUpgradePopup.OpenForModel(equipmentItemModel);
		}
		return equipmentUpgradePopup;
	}

	public static EquipmentUpgradePopup OpenEquipmentUpgradePopupPreview(EquipmentDefinition definition, int rarityLevel)
	{
		if (definition == null)
		{
			return null;
		}
		EquipmentUpgradePopup equipmentUpgradePopup;
		if (definition.SwitchRemoldMode)
		{
			equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopupNew) as EquipmentUpgradePopup;
		}
		else
		{
			equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
		}
		if (equipmentUpgradePopup != null)
		{
			equipmentUpgradePopup.OpenForPreview(definition.ID, rarityLevel);
		}
		return equipmentUpgradePopup;
	}

	public static List<SPTraitsRemoldDefinitions> GetSPTraitsRemoldSkillList(CurrencyType currencyType)
	{
		if (!HelpersGfx.IsSkillTonkenCurrencyType(currencyType))
		{
			return new List<SPTraitsRemoldDefinitions>();
		}
		SPTraitsSkillKitTokenSet skillKitTokenSetDefinition = HelpersGfx.GetSkillKitTokenSetDefinition(currencyType);
		if (skillKitTokenSetDefinition == null)
		{
			return new List<SPTraitsRemoldDefinitions>();
		}
		List<SPTraitsRemoldDefinitions> list = new List<SPTraitsRemoldDefinitions>();
		SPTraitsRemoldDefinitions[] sPTraitsRemodeDefinition = GameManager.Instance.gameEconomyData.SPTraitsRemodeDefinition;
		if (sPTraitsRemodeDefinition == null)
		{
			return new List<SPTraitsRemoldDefinitions>();
		}
		SPTraitsRemoldDefinitions[] array = sPTraitsRemodeDefinition;
		foreach (SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions in array)
		{
			if (sPTraitsRemoldDefinitions.MakingCost.ToLower().Contains(skillKitTokenSetDefinition.ID.ToLower()))
			{
				list.Add(sPTraitsRemoldDefinitions);
			}
		}
		return list;
	}

	public static SPTraitsRemoldDefinitions GetMinRemoldDefinitionForGroup(string spTraitsGroupId)
	{
		if (string.IsNullOrEmpty(spTraitsGroupId) || GameManager.Instance == null || GameManager.Instance.gameEconomyData == null)
		{
			return null;
		}
		List<SPTraitsRemoldDefinitions> sPTraitsRemodeDefinitionByType = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinitionByType(spTraitsGroupId);
		if (sPTraitsRemodeDefinitionByType == null || sPTraitsRemodeDefinitionByType.Count == 0)
		{
			return null;
		}
		return sPTraitsRemodeDefinitionByType.OrderBy((SPTraitsRemoldDefinitions x) => x.Level).FirstOrDefault();
	}


	#region mycode
	public static GameObject GetUIParent(GameObject refObject, bool isReverse = false)
	{
		GameObject parentUI = isReverse ? HUDManager.Instance.UIContainerTopCameras : HUDManager.Instance.UIContainer;
		GameObject parentTop = isReverse ? HUDManager.Instance.UIContainer : HUDManager.Instance.UIContainerTopCameras;

		return OfflineManager.IsLoadDataManager ? refObject.layer == 5 ? parentUI : parentTop : null;
	}
	#endregion
}
