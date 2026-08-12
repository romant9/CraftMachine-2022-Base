using TWDModel;
using UnityEngine;

public class MissionHubPanelBase : NUIGridItem
{
	[Header("Fullscreen BG")]
	[SerializeField]
	protected UIWidget[] fullscreenWidgets;

	private MissionHubContent panelContent;

	public bool isLocked { get; set; }

	public virtual MissionHubContent missionHubContent => panelContent;

	public virtual void Awake()
	{
		DebugIdString = "MissionHubPanelBase";
	}

	public virtual void Update()
	{
	}

	public virtual void UpdateUI()
	{
		UpdateFullscreenWidgets();
	}

	public virtual void Init(MissionHubContent content, HUDElement parent)
	{
		MissionHubGameModePanel missionHubGameModePanel = this as MissionHubGameModePanel;
		string text = "";
		if (missionHubGameModePanel != null)
		{
			missionHubGameModePanel.CheckLockedState();
			if (missionHubGameModePanel.isLocked && (!(missionHubGameModePanel.GetType() == typeof(MissionHubPanelGuildBattle)) || !GuildWarHelper.ShowWarIsOnOnMissionHub()))
			{
				text = "Locked_";
			}
		}
		panelContent = content;
		base.name = text + content.SortInt + "_" + content.PrefabName + "(Prefab)";
		Init();
	}

	public override bool Init()
	{
		return base.Init();
	}

	public override void Clear()
	{
		base.Clear();
	}

	public virtual void UpdateFullscreenWidgets()
	{
		if (fullscreenWidgets == null)
		{
			return;
		}
		GameObject gameObject = null;
		for (int i = 0; i < fullscreenWidgets.Length; i++)
		{
			if (fullscreenWidgets[i] != null)
			{
				if (gameObject == null)
				{
					gameObject = fullscreenWidgets[i].gameObject.FindInParents<UIPanel>();
				}
				fullscreenWidgets[i].keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
				fullscreenWidgets[i].SetAnchor(gameObject, 0, 0, 0, 0);
			}
		}
	}
}
