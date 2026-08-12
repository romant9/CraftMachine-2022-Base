using System;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class HUDManager : SingularityMonoBehaviour<HUDManager>
{
	private List<HUDElement> openPopups = new List<HUDElement>();

	private CampaignVisualConfig campaignVisualConfig;

	[SerializeField]
	[Tooltip("Container for all ui.")]
	public GameObject UIContainer;

	public GameObject UIContainerTopCameras;

	public GameObject UIContainerPerspective;

	[SerializeField]
	[Tooltip("Set this to true to make the game always use HD atlases.")]
	private bool forceHDAtlases;

	private Dictionary<int, HUDElement> hudElements = new Dictionary<int, HUDElement>(64);

	public Dictionary<int, HUDElementConfig> hudElementsConfig = new Dictionary<int, HUDElementConfig>();

	private int hasFullScreenPopupOldValue = -1;

	public int NumberDialogsOpen => GetOpenPopupsList().Count;

	public List<HUDElement> OpenPopups => openPopups;

	public CampaignVisualConfig CampaignVisualConfig
	{
		get
		{
			if (campaignVisualConfig == null)
			{
				CampaignModel campaignModel = GameManager.Instance.playerModel.CampaignModel;
				CampaignDefinition campaignDefinition = GameManager.Instance.gameEconomyData.GetCampaignDefinition(campaignModel.Id);
				if (campaignDefinition == null)
				{
					return null;
				}
				string text = "CampaignVisualConfigs/CampaignVisualConfig_" + campaignDefinition.VisualConfig;
				campaignVisualConfig = UnityUtils.LoadFromAssetBundle<CampaignVisualConfig>(text, "scriptableobjects");
			}
			return campaignVisualConfig;
		}
	}

	public Transform UIParent => UIContainer.transform;

	public event HUDElement.HUDElementTransitionHandler OnAnyHudElementClosed;

	public HUDElement Get(UIType uiType, GameObject parent = null, bool createIfNotExist = true, GameObject prefabVariant = null)
	{
		if (hudElements.ContainsKey((int)uiType))
		{
			return hudElements[(int)uiType];
		}
		if (createIfNotExist)
		{
			if (prefabVariant != null)
			{
				Load(uiType, prefabVariant);
			}
			else
			{
				Load(uiType);
			}
			return CreateHudElement(uiType, parent);
		}
		return null;
	}

	public HUDElement GetFromAssetBundle(UIType uiType, GameObject parent = null, bool createIfNotExist = true)
	{
		if (hudElements.ContainsKey((int)uiType))
		{
			return hudElements[(int)uiType];
		}
		if (createIfNotExist)
		{
			if (!hudElementsConfig.ContainsKey((int)uiType))
			{
				HUDElementConfig value = UnityUtils.LoadFromAssetBundle<HUDElementConfig>("UI/HudElementsConfig/" + uiType, "scriptableobjects");
				hudElementsConfig.Add((int)uiType, value);
			}
			return CreateHudElement(uiType, parent);
		}
		return null;
	}

	public HUDElement GetNoCreation(UIType uiType, GameObject parent = null)
	{
		return Get(uiType, parent, createIfNotExist: false);
	}

	public bool IsOpen(UIType uiType)
	{
		if (GetNoCreation(uiType) != null)
		{
			if (GetNoCreation(uiType).IsOpen)
			{
				return !GetNoCreation(uiType).IsClosing;
			}
			return false;
		}
		return false;
	}

	public bool IsActive(UIType uiType)
	{
		HUDElement noCreation = GetNoCreation(uiType);
		if (noCreation != null)
		{
			return noCreation.IsOpen;
		}
		return false;
	}

	public void Load(UIType uiType)
	{
		if (!hudElementsConfig.ContainsKey((int)uiType))
		{
			HUDElementConfig value = UnityUtils.LoadFromAssetBundle<HUDElementConfig>("UI/HudElementsConfig/" + uiType, "scriptableobjects");
			hudElementsConfig.Add((int)uiType, value);
		}
	}

	public void Load(UIType uiType, GameObject prefab)
	{
		if (!hudElementsConfig.ContainsKey((int)uiType))
		{
			HUDElementConfig hUDElementConfig = UnityUtils.LoadFromAssetBundle<HUDElementConfig>("UI/HudElementsConfig/" + uiType, "scriptableobjects");
			if (OfflineManager.IsLoadDataManager)
			{				
				hUDElementConfig.assetBundlePrefabOverride = prefab;
			}
			hUDElementConfig.PrefabName = prefab.name;
			hudElementsConfig.Add((int)uiType, hUDElementConfig);
		}
	}

	public void RemoveHudElementsConfig(UIType uiType)
	{
		if (hudElementsConfig.ContainsKey((int)uiType))
		{
			hudElementsConfig.Remove((int)uiType);
		}
	}

	public void ReleaseInactiveConfigs()
	{
		foreach (UIType value in Enum.GetValues(typeof(UIType)))
		{
			if (!hudElements.ContainsKey((int)value))
			{
				RemoveHudElementsConfig(value);
			}
		}
	}

	public HUDElement CloseIfExists(UIType uiType)
	{
		HUDElement hUDElement = Get(uiType, null, createIfNotExist: false);
		if ((bool)hUDElement)
		{
			if (OfflineManager.IsLoadDataManager) 
			{
				hudElements.Remove((int)uiType);
				Destroy(hUDElement.gameObject);
				return null;
			}
			else
			{
				hUDElement.Close();
			}
		}
		return hUDElement;
	}

	public HUDElement IfExists(UIType uiType, Action<HUDElement> callback)
	{
		HUDElement hUDElement = Get(uiType, null, createIfNotExist: false);
		if ((bool)hUDElement)
		{
			callback(hUDElement);
		}
		return hUDElement;
	}

	public HUDElement GetPopupOnTop()
	{
		if (openPopups == null || openPopups.Count == 0)
		{
			return null;
		}
		HUDElement hUDElement = openPopups[openPopups.Count - 1];
		for (int i = 0; i < openPopups.Count; i++)
		{
			if (hUDElement != null && hUDElement.gameObject.layer == 5 && openPopups[i] != null && openPopups[i].gameObject.layer == 20)
			{
				hUDElement = openPopups[i];
			}
		}
		return hUDElement;
	}

	public bool CanEnableCamp(UIType ignoreOpenPopupsOfType = UIType.None)
	{
		if (openPopups != null)
		{
			for (int i = 0; i < openPopups.Count; i++)
			{
				if (openPopups[i] != null && GetHudElementConfig(openPopups[i].UIType) != null && GetHudElementConfig(openPopups[i].UIType).DisableCampMode != DisableCampMode.None && (ignoreOpenPopupsOfType == UIType.None || openPopups[i].UIType != ignoreOpenPopupsOfType))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static HUDElement TryOpenPopup(UIType popupType, GameObject parent = null)
	{
		if (popupType == UIType.None)
		{
			return null;
		}
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(popupType, parent);
		if (hUDElement != null)
		{
			hUDElement.Open();
		}
		return hUDElement;
	}

	public static bool TryClosePopup(UIType popupType)
	{
		if (popupType == UIType.None)
		{
			return false;
		}
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(popupType);
		if (noCreation == null)
		{
			return false;
		}
		noCreation.OnClickClose();
		return true;
	}

	private HUDElement CreateHudElement(UIType uiType, GameObject parent)
	{
		HUDElementConfig hudElementConfig = GetHudElementConfig(uiType);
		if (hudElementConfig == null)
		{
			Debug.LogError("Failed to initialize hud element " + uiType.ToString() + ". HUDElementConfig was NULL!");
			return null;
		}
		if (string.IsNullOrEmpty(hudElementConfig.PrefabName))
		{
			Debug.LogError("Failed to initialize hud element " + uiType.ToString() + ". Missing Prefab refence!");
			return null;
		}
		HUDElement hUDElement = InitializeHUDElement(uiType, hudElementConfig, parent);
		if (hUDElement == null)
		{
			Debug.LogError("Failed to initialize hud element " + uiType);
			return null;
		}
		hUDElement.OnClose += OnHudClosed;
		hUDElement.OnOpen += OnHudOpened;
		hUDElement.OnOpenAnimComplete += OnHudOpenAnimComplete;
		if (!hudElementConfig.DeleteOnExitState && !hudElementConfig.DeleteOnClose)
		{
			UnityEngine.Object.DontDestroyOnLoad(hUDElement.transform.root.gameObject);
		}
		hudElements.Add((int)uiType, hUDElement);
		return hUDElement;
	}

	public HUDElementConfig GetHudElementConfig(UIType uiType)
	{
		if (hudElementsConfig.ContainsKey((int)uiType))
		{
			return hudElementsConfig[(int)uiType];
		}
		DebugTWD.Log("Error loading HudElement " + uiType, DebugType.UI);
		return null;
	}

	public HUDElementConfig GetHudElementConfig(HUDElement hudElement)
	{
		if (hudElement != null)
		{
			return GetHudElementConfig(hudElement.UIType);
		}
		return null;
	}

	public void OnStateChange(GameState newState, GameState oldState)
	{
		if (newState != oldState)
		{
			OnLeaveState(oldState);
			OnEnterState(newState);
		}
	}

	public bool UsesVersionIncompatibleFeature(UIType uiType, out bool showPopup)
	{
		showPopup = false;
		if (OfflineManager.IsLoadDataManager || OfflineManager.IsFreeAll) 
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager || OfflineManager.IsFreeAll)");
			return false;
		}
		
		HUDElementConfig hudElementConfig = GetHudElementConfig(uiType);
		if (hudElementConfig != null)
		{
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			if (gameEconomyData == null)
			{
				return false;
			}
			Feature feature = gameEconomyData.GetFeature(hudElementConfig.UsesFeature);
			showPopup = feature.ShowPopup;
			return !feature.Enabled;
		}
		return false;
	}

	private void OnEnterState(GameState state)
	{
		KeyValuePair<int, HUDElementConfig>[] array = hudElementsConfig.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<int, HUDElementConfig> keyValuePair = array[i];
			if (keyValuePair.Value.LoadOnState == state)
			{
				Get((UIType)keyValuePair.Key);
			}
		}
	}

	private void OnLeaveState(GameState state)
	{
		List<HUDElement> list = new List<HUDElement>();
		foreach (KeyValuePair<int, HUDElement> hudElement in hudElements)
		{
			HUDElementConfig hudElementConfig = GetHudElementConfig(hudElement.Value);
			if (hudElementConfig != null && hudElementConfig.DeleteOnExitState && (hudElementConfig.LoadOnState == GameState.None || hudElementConfig.LoadOnState == state))
			{
				list.Add(hudElement.Value);
			}
		}
		foreach (HUDElement item in list)
		{
			DeleteHudElement(item);
		}
		ReleaseInactiveConfigs();
		UIDrawCall.ReleaseInactive();
		Helpers.ClearUnusedMemory();
	}

	public void OnHudOpened(HUDElement hudElement, HUDElementConfig elementConfig)
	{
		HUDElementConfig hudElementConfig = GetHudElementConfig(hudElement);
		if (!(hudElementConfig == null))
		{
			if (hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Dialog && hudElement.UIType != UIType.DefaultPopup && hudElement.UIType != UIType.DefaultTopCamerasPopup && !openPopups.Contains(hudElement))
			{
				openPopups.Add(hudElement);
			}
			if (SingularityMonoBehaviour<SDKManager>.Instance != null && (int)hudElement.UIType < (int)UIType.None - 4)
			{
				SingularityMonoBehaviour<SDKManager>.Instance.InterfaceResult(hudElementConfig.PrefabName);
			}
		}
	}

	public void OnHudOpenAnimComplete(HUDElement hudElement, HUDElementConfig elementConfig)
	{
		if (CampManager.Instance != null && elementConfig != null && elementConfig.DisableCampMode != DisableCampMode.None)
		{
			CampManager.Instance.FullscreenPopupShowCamp(show: false, elementConfig.DisableCampMode);
		}
	}

	public List<HUDElement> GetOpenPopupsList()
	{
		return openPopups;
	}

	public void OnHudClosed(HUDElement hudElement, HUDElementConfig elementConfig)
	{
		HUDElementConfig hudElementConfig = GetHudElementConfig(hudElement);
		if (hudElementConfig == null)
		{
			return;
		}
		if (hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Dialog)
		{
			if (hudElement.UIType != UIType.DefaultPopup && hudElement.UIType != UIType.DefaultTopCamerasPopup && openPopups.Contains(hudElement))
			{
				openPopups.Remove(hudElement);
			}
			if (hudElementConfig.DeleteOnClose)
			{
				DeleteHudElement(hudElement);
			}
		}
		if (this.OnAnyHudElementClosed != null)
		{
			this.OnAnyHudElementClosed(hudElement, hudElementConfig);
		}
		if (CampManager.Instance != null && hudElementConfig.DisableCampMode != DisableCampMode.None)
		{
			bool show = CanEnableCamp();
			CampManager.Instance.FullscreenPopupShowCamp(show, hudElementConfig.DisableCampMode);
		}
	}

	private void DeleteHudElement(HUDElement hudElement)
	{
		if (!(hudElement == null))
		{
			if (hudElements.ContainsKey((int)hudElement.UIType))
			{
				hudElements.Remove((int)hudElement.UIType);
			}
			if (hudElement.gameObject != null)
			{
				UnityEngine.Object.Destroy(hudElement.gameObject);
				hudElement.OnClose -= OnHudClosed;
				hudElement.OnOpen -= OnHudOpened;
				hudElement.OnOpenAnimComplete -= OnHudOpenAnimComplete;
			}
			UnityEngine.Object.Destroy(hudElement);
			hudElement = null;
		}
	}

	public void CloseAllOpenPopupsAndDialogs(List<UIType> ignoreOpenPopupsOfType = null)
	{
		if (hudElements == null)
		{
			return;
		}
		foreach (UIType value in Enum.GetValues(typeof(UIType)))
		{
			if (!hudElements.ContainsKey((int)value) || (ignoreOpenPopupsOfType != null && ignoreOpenPopupsOfType.Contains(value)))
			{
				continue;
			}
			HUDElement hUDElement = hudElements[(int)value];
			if (hUDElement != null)
			{
				HUDElementConfig hudElementConfig = GetHudElementConfig(hUDElement);
				if (hudElementConfig != null && (hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Popup || hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Dialog))
				{
					hUDElement.SetUITypeOpenOnClose(UIType.None);
					hUDElement.Close();
				}
			}
		}
	}

	public void CloseAllElementsOfType(UIType type)
	{
		List<HUDElement> list = new List<HUDElement>();
		foreach (KeyValuePair<int, HUDElement> hudElement in hudElements)
		{
			if (hudElement.Key == (int)type && hudElement.Value != null)
			{
				list.Add(hudElement.Value);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Close();
		}
	}

	public void CloseAll()
	{
		foreach (UIType value in Enum.GetValues(typeof(UIType)))
		{
			if (hudElements.ContainsKey((int)value))
			{
				hudElements[(int)value].Close();
			}
		}
		Reset();
	}

	public void DeleteAll()
	{
		foreach (UIType value in Enum.GetValues(typeof(UIType)))
		{
			if (hudElements.ContainsKey((int)value))
			{
				HUDElement hudElement = hudElements[(int)value];
				DeleteHudElement(hudElement);
			}
		}
	}

	public AudioListener GetUIAudioListener()
	{
		if (UIContainer != null)
		{
			return UIContainer.GetComponent<AudioListener>();
		}
		return null;
	}

	private HUDElement InitializeHUDElement(UIType uiType, HUDElementConfig elementConfig, GameObject parent = null)
	{
		if (parent == null)
		{
			parent = elementConfig.ShowInPerspectiveCamera && UIContainerPerspective ? UIContainerPerspective : ((!elementConfig.ShowOnTopCameras) ? UIContainer : UIContainerTopCameras);
		}
		GameObject gameObject = parent.AddChild(elementConfig.GetPrefab());
		HUDElement component = gameObject.GetComponent<HUDElement>();
		if (!(component == null))
		{
			component.UIType = uiType;
		}
		if (UIContainerPerspective && parent == UIContainerPerspective)
		{
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.z = 800f;
			gameObject.transform.localPosition = localPosition;
		}
		gameObject.SetActive(value: false);
		return component;
	}

	public void UpdateAtlasQuality()
	{
		if (!forceHDAtlases && !(Helpers.GetPixelRatio() >= 2f))
		{
			return;
		}
		UIAtlas[] array = Resources.FindObjectsOfTypeAll<UIAtlas>();
		foreach (UIAtlas uIAtlas in array)
		{
			if (uIAtlas.gameObject.tag != "HDAtlas")
			{
				GameObject gameObject = UnityUtils.LoadAsset(uIAtlas.name + "_HD") as GameObject;
				if (gameObject != null)
				{
					uIAtlas.replacement = gameObject.GetComponent<UIAtlas>();
				}
			}
		}
	}

	public void OnDestroy()
	{
	}

	public void Reset()
	{
		if (openPopups != null)
		{
			openPopups.Clear();
		}
		hasFullScreenPopupOldValue = -1;
	}

	private void Update()
	{
		bool flag = false;
		if (NumberDialogsOpen > 0 && openPopups != null && openPopups.Count > 0)
		{
			foreach (HUDElement openPopup in openPopups)
			{
				if ((bool)openPopup && (bool)openPopup.gameObject && openPopup.gameObject.activeInHierarchy)
				{
					HUDElementConfig hudElementConfig = SingularityMonoBehaviour<HUDManager>.Instance.GetHudElementConfig(openPopup.UIType);
					if (hudElementConfig != null && hudElementConfig.colorClear)
					{
						flag = true;
						break;
					}
				}
			}
		}
		if ((bool)SingularityMonoBehaviour<LoadingScreenHUD>.Instance && SingularityMonoBehaviour<LoadingScreenHUD>.Instance.gameObject.activeInHierarchy && (SingularityMonoBehaviour<AssetLoaderRoot>.Instance == null || !SingularityMonoBehaviour<AssetLoaderRoot>.Instance.IsFading))
		{
			flag = true;
		}
		if (hasFullScreenPopupOldValue == (flag ? 1 : 0))
		{
			return;
		}
		if ((bool)UIContainer)
		{
			Camera component = UIContainer.GetComponent<Camera>();
			if ((bool)component)
			{
				component.clearFlags = (flag ? CameraClearFlags.Color : CameraClearFlags.Depth);
			}
		}
		CampManager.Toggle(!flag);
		hasFullScreenPopupOldValue = (flag ? 1 : 0);
	}

	public bool HasFullScreenPopup()
	{
		if (NumberDialogsOpen > 0 && openPopups != null)
		{
			foreach (HUDElement openPopup in openPopups)
			{
				if ((bool)openPopup && (bool)openPopup.gameObject && openPopup.gameObject.activeInHierarchy)
				{
					HUDElementConfig hudElementConfig = GetHudElementConfig(openPopup.UIType);
					if (hudElementConfig != null && hudElementConfig.colorClear)
					{
						return true;
					}
				}
			}
		}
		return false;
	}
}
