using TWDModel;

[GraphItNode(NodeType.Action)]
public class GuildBossAnimatePopupClientNode : ClientNodeBase
{
	[GraphItVariable("Boss texture name")]
	public string BossTextureName;

	[GraphItVariable("Localization key")]
	public string LabelLocalizationKey;

	private bool popupPending;

	private bool popupTriggered;

	private bool seenLoadingScreen;

	private void OnEnable()
	{
		EventManager.OnEvent += OnGameEvent;
	}

	private void OnDisable()
	{
		EventManager.OnEvent -= OnGameEvent;
	}

	public override void OnNodeBind()
	{
		if (base.NodeGuidHash != 0)
		{
			guidHash = base.NodeGuidHash;
		}
		base.OnNodeBind();
	}

	private void LateUpdate()
	{
		if (LoadingScreenCombat.Active)
		{
			seenLoadingScreen = true;
		}
		TryShowPopup();
	}

	private void OnGameEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.CombatStart)
		{
			TryShowPopup();
		}
	}

	private bool IsCombatHudVisible()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD, null, createIfNotExist: false);
		if (hUDElement != null)
		{
			return hUDElement.gameObject.activeInHierarchy;
		}
		return false;
	}

	private bool IsReadyToShowPopup()
	{
		if (seenLoadingScreen)
		{
			return IsCombatHudVisible();
		}
		return false;
	}

	private void TryShowPopup()
	{
		if (!popupPending || popupTriggered || !IsReadyToShowPopup())
		{
			return;
		}
		popupTriggered = true;
		popupPending = false;
		string textureName = BossTextureName;
		string labelKey = LabelLocalizationKey;
		VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
		{
			GuildBossAnimatePopup guildBossAnimatePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBossAnimatePopup) as GuildBossAnimatePopup;
			if (!(guildBossAnimatePopup == null))
			{
				GuildBossAnimatePopupData guildBossAnimatePopupData = new GuildBossAnimatePopupData
				{
					TextureName = textureName
				};
				if (!string.IsNullOrEmpty(labelKey))
				{
					guildBossAnimatePopupData.LabelText = LocalizationManager.GetText(labelKey);
				}
				guildBossAnimatePopup.OpenWithBoss(guildBossAnimatePopupData);
			}
		}));
	}

	[GraphItInput("Activate", "")]
	public void Activate()
	{
		if (!popupTriggered)
		{
			popupPending = true;
			TryShowPopup();
		}
	}
}
