using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class MissionHubListBase : MonoBehaviourExtended
{
	private UIGrid grid;

	private UIPanel uiPanel;

	private List<MissionHubPanelBase> PanelsList = new List<MissionHubPanelBase>();

	private UIScrollView scrollView;

	public virtual void Awake()
	{
		DebugIdString = "MissionHubListBase";
		scrollView = GetComponent<UIScrollView>();
	}

	public virtual void SetGridSize(float cellWidth, float cellHeight)
	{
		grid = base.gameObject.GetComponent<UIGrid>();
		if (grid != null)
		{
			grid.cellHeight = cellHeight;
			grid.cellWidth = cellWidth;
		}
	}

	public void UpdateUI()
	{
		for (int i = 0; i < PanelsList.Count; i++)
		{
			if (PanelsList[i] != null)
			{
				PanelsList[i].UpdateUI();
			}
		}
	}

	public void SetScrollViewEnabled(bool enabled)
	{
		if (scrollView != null)
		{
			scrollView.enabled = enabled;
		}
	}

	public void RepositionNow()
	{
		if (uiPanel == null)
		{
			uiPanel = GetComponent<UIPanel>();
		}
		if (uiPanel != null)
		{
			uiPanel.ResetAndUpdateAnchors();
		}
		if (grid != null)
		{
			grid.repositionNow = true;
		}
	}

	public MissionHubPanelBase InstantiatePanel(GameObject obj, MissionHubContent content, HUDElement parentPopup)
	{
		MissionHubPanelBase missionHubPanelBase = InitPanelToParent(obj, base.gameObject, content, parentPopup);
		if (missionHubPanelBase != null)
		{
			PanelsList.Add(missionHubPanelBase);
		}
		return missionHubPanelBase;
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < PanelsList.Count; i++)
		{
			if (PanelsList[i] != null)
			{
				PanelsList[i].Clear();
			}
		}
		PanelsList = new List<MissionHubPanelBase>();
	}

	private MissionHubPanelBase InitPanelToParent(GameObject obj, GameObject parent, MissionHubContent content, HUDElement parentPopup)
	{
		MissionHubPanelBase missionHubPanelBase = null;
		if (obj != null && parent != null)
		{
			GameObject gameObject = Helpers.InstantiateToParentAndLayer(obj, parent);
			if (gameObject != null)
			{
				missionHubPanelBase = gameObject.GetComponent<MissionHubPanelBase>();
				if (missionHubPanelBase != null)
				{
					missionHubPanelBase.Init(content, parentPopup);
				}
				else
				{
					Debug.LogError("Could not Init() " + missionHubPanelBase.name + ". Could not find component: MissionHubPanelBase");
				}
			}
		}
		else
		{
			Debug.LogError("Could not instantiate obj or parent null!");
		}
		return missionHubPanelBase;
	}
}
