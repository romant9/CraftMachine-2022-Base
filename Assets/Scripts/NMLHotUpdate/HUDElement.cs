using BaseModel;
using TWDModel;
using UnityEngine;

public class HUDElement : MonoBehaviour
{
	public delegate void HUDElementTransitionHandler(HUDElement element, HUDElementConfig hudElementConfig);

	public Callback OnCloseCallback;

	protected UIType UITypeOpenOnClose = UIType.None;

	protected object UIOpenDataOnClose;

	[SerializeField]
	[Tooltip("If true, it uses the default popup template and it will be added to the default pop-up when opening it.")]
	private bool useDefaultPopup;

	[SerializeField]
	[Tooltip("Determines the layer used for for defaultPopup. True == UiTopCameras, false == UI.")]
	protected bool useUiTopCamerasLayer;

	[Tooltip("If true, it uses the white background in the default popup.")]
	public bool UseWhiteBackground;

	[Tooltip("Sets the width of the default pop up.")]
	public int DefaultPopUpWidth = DefaultPopup.DefaultWidth;

	[Tooltip("Sets the width of the default pop up.")]
	public int DefaultPopUpHeight = DefaultPopup.DefaultHeightSmall;

	[Tooltip("Sets the y position of the popup & the default pop up. -1 means default position and 0 is top of the screen.")]
	public int ForcePositionY = -1;

	protected DefaultPopup defaultPopup;

	protected ModelObject model;

	protected TWDGroupModelChild groupModelChild;

	private bool isClosing;

	private GameObject target;

	private long viewTimestamp;

	protected string DebugClassString = "HUDElement";

	public UIType UIType { get; set; }

	public bool IsOpen => base.gameObject.activeInHierarchy;

	public bool IsClosing
	{
		get
		{
			if (IsOpen)
			{
				return isClosing;
			}
			return false;
		}
	}

	public event HUDElementTransitionHandler OnOpen;

	public event HUDElementTransitionHandler OnOpenAnimComplete;

	public event HUDElementTransitionHandler OnClose;

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void UpdateUI()
	{
	}

	protected T GetModel<T>() where T : ModelObject
	{
		if (model != null && model is T)
		{
			return model as T;
		}
		throw new UnityException(string.Format("The model type does not match that of the requested one! Should be \"{0}\" but was \"{1}\"!", typeof(T).Name, (model == null) ? "(null)" : model.GetType().Name));
	}

	protected bool IsModelRequestedType<T>() where T : ModelObject
	{
		return model is T;
	}

	protected T GetGroupModel<T>() where T : TWDGroupModelChild
	{
		if (groupModelChild != null && groupModelChild is T)
		{
			return groupModelChild as T;
		}
		throw new UnityException(string.Format("The model type does not match that of the requested one! Should be \"{0}\" but was \"{1}\"!", typeof(T).Name, (model == null) ? "(null)" : model.GetType().Name));
	}

	protected bool IsGroupModelRequestedType<T>() where T : TWDGroupModelChild
	{
		return groupModelChild is T;
	}

	protected bool IsOpenedWithGroupModel()
	{
		return groupModelChild != null;
	}

	public virtual void OpenForModel(ModelObject model)
	{
		if (model == null)
		{
			Debug.LogError("Cannot open HUD element for (null) model!");
		}
		else if (!base.gameObject.activeSelf || this.model != model)
		{
			this.model = model;
			Open();
		}
	}

	public virtual void OpenForModel(TWDGroupModelChild model)
	{
		if (model == null)
		{
			Debug.LogError("Cannot open HUD element for (null) model!");
		}
		else if (!base.gameObject.activeSelf || groupModelChild != model)
		{
			groupModelChild = model;
			Open();
		}
	}

	public virtual void OpenWithStateData(object data)
	{
		Open();
	}

	public virtual void Open()
	{
		if (base.gameObject.activeSelf && !isClosing)
		{
			return;
		}
		isClosing = false;
		if (useDefaultPopup)
		{
			defaultPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get((!useUiTopCamerasLayer) ? UIType.DefaultPopup : UIType.DefaultTopCamerasPopup) as DefaultPopup;
			defaultPopup.AddPopUp(this);
			defaultPopup.Open();
		}
		HUDElementConfig hudElementConfig = SingularityMonoBehaviour<HUDManager>.Instance.GetHudElementConfig(UIType);
		if (!OfflineManager.IsLoadDataManager && SingularityMonoBehaviour<HUDManager>.Instance.UsesVersionIncompatibleFeature(UIType, out bool showPopup))
		{
			if (showPopup)
			{
				OptionalUpdatePopup.OpenFeatureLockedContent();
			}
			return;
		}
		base.gameObject.SetActive(value: true);
		if (this.OnOpen != null)
		{
			this.OnOpen(this, hudElementConfig);
		}
		TweenManager.PlayTweenGroup(base.gameObject, 1, forward: true, OnOpenAnimationOver);
		UpdateParentCamera();
		if (hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Popup || hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Dialog)
		{
			UIEvent.Send("OnPopUpOpen", this);
		}
		else if (hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.ContextMenuBox)
		{
			UIEvent.Send("OnContextMenuBoxOpened", this);
		}
		CreateOpenedTimeStamp();
	}

	public virtual void OnOpenAnimationOver()
	{
		if (this.OnOpenAnimComplete != null)
		{
			this.OnOpenAnimComplete(this, SingularityMonoBehaviour<HUDManager>.Instance.GetHudElementConfig(UIType));
		}
	}

	public virtual void Close()
	{
		if (!base.gameObject || !base.gameObject.activeSelf || isClosing)
		{
			return;
		}
		isClosing = true;
		TweenManager.PlayTweenGroup(base.gameObject, 2, forward: true, OnCloseAnimOver);
		if (OnCloseCallback != null)
		{
			OnCloseCallback();
			OnCloseCallback = null;
		}
		if (UITypeOpenOnClose != UIType.None && UITypeOpenOnClose <= UIType.None)
		{
			HUDManager instance = SingularityMonoBehaviour<HUDManager>.Instance;
			HUDElement hUDElement = (instance ? instance.Get(UITypeOpenOnClose) : null);
			if ((bool)hUDElement && !hUDElement.IsOpen)
			{
				if (UIOpenDataOnClose != null)
				{
					hUDElement.OpenWithStateData(UIOpenDataOnClose);
				}
				else
				{
					hUDElement.Open();
				}
			}
		}
		SetUITypeOpenOnClose(UIType.None);
		ClearTimestamp();
	}

	public void SetUITypeOpenOnClose(UIType type, object data = null)
	{
		UITypeOpenOnClose = type;
		UIOpenDataOnClose = data;
	}

	protected virtual void OnCloseAnimOver()
	{
		HUDElementConfig hudElementConfig = SingularityMonoBehaviour<HUDManager>.Instance.GetHudElementConfig(UIType);
		if (isClosing)
		{
			if (hudElementConfig != null && (hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Popup || hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.Dialog))
			{
				UIEvent.Send("OnPopUpClose", this);
			}
			else if (hudElementConfig != null && (object)hudElementConfig != null && hudElementConfig.ElementType == HUDElementConfig.ElementTypeEnum.ContextMenuBox)
			{
				UIEvent.Send("OnContextMenuBoxClosed", this);
			}
			isClosing = false;
			base.gameObject.SetActive(value: false);
			if (useDefaultPopup)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists((!useUiTopCamerasLayer) ? UIType.DefaultPopup : UIType.DefaultTopCamerasPopup);
			}
		}
		UpdateParentCamera();
		if (this.OnClose != null)
		{
			this.OnClose(this, hudElementConfig);
		}
		EventManager.NotifyClick("Close");
	}

	public virtual void UpdateParentCamera()
	{
		if (base.transform != null)
		{
			UICameraParent component = base.transform.parent.GetComponent<UICameraParent>();
			if (component != null)
			{
				component.UpdateState();
			}
		}
	}

	public virtual void OnClickClose()
	{
		if (TutorialView.Allowed("Close"))
		{
			Close();
		}
	}

	public virtual void OnBackButtonClicked()
	{
		OnClickClose();
	}

	public void SetParent(Transform parent)
	{
		base.transform.parent = parent;
	}

	public void FollowTarget(GameObject target)
	{
		if (!(target == null) && !(GetComponent<UIWidget>() == null))
		{
			this.target = target;
			GetComponent<UIWidget>().SetAnchor(target);
		}
	}

	public void UpdateFollowTarget()
	{
		FollowTarget(target);
	}

	public int GetPopupOpenInSeconds()
	{
		if (GameManager.Instance != null && viewTimestamp != 0L)
		{
			return (int)((GameManager.Instance.playerModel.UtcTimeStamp - viewTimestamp) / 1000);
		}
		return 0;
	}

	public void ClearTimestamp()
	{
		viewTimestamp = 0L;
	}

	public void CreateOpenedTimeStamp()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			viewTimestamp = GameManager.Instance.playerModel.UtcTimeStamp;
		}
	}

	protected void DebugLog(string message)
	{
	}

	protected void DebugLogWarning(string message)
	{
		Debug.LogWarning(DebugClassString + ": " + message);
	}

	protected void DebugLogError(string message)
	{
		Debug.LogError(DebugClassString + ": " + message);
	}

	public DefaultPopup GetDefaultPopup()
	{
		if (!useDefaultPopup)
		{
			return null;
		}
		return defaultPopup;
	}


	#region mycode
	public void SetIsUiTopCameras(bool isTop)
	{
		useUiTopCamerasLayer = isTop;
	}

	public void SetUseDefaultPopup(bool use)
	{
		useDefaultPopup = use;
	}

	public static GameObject GetParent(GameObject refObject = null)
	{
		return OfflineManager.IsLoadDataManager ? refObject == null || refObject.layer == 5 ? HUDManager.Instance.UIContainer : HUDManager.Instance.UIContainerTopCameras : null;
	}
	#endregion
}
