using System.Collections.Generic;
using UnityEngine;

public class ScavengeListBase : MonoBehaviourExtended
{
	private UIGrid grid;

	private UIPanel uiPanel;

	private List<ScavengeMissionButton> buttonList = new List<ScavengeMissionButton>();

	public virtual void Awake()
	{
		DebugIdString = "ScavengeListBase";
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
		for (int i = 0; i < buttonList.Count; i++)
		{
			if (buttonList[i] != null)
			{
				buttonList[i].UpdateUI();
			}
		}
	}

	public List<ScavengeMissionButton> GetButtonList()
	{
		return buttonList;
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

	public ScavengeMissionButton InstantiateButton(GameObject prefab, HUDElement parentPopup)
	{
		if (prefab != null)
		{
			return Helpers.InstantiateToList(prefab, base.gameObject, buttonList);
		}
		return null;
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < buttonList.Count; i++)
		{
			if (buttonList[i] != null)
			{
				buttonList[i].Clear();
				Object.Destroy(buttonList[i].gameObject);
			}
		}
		buttonList = new List<ScavengeMissionButton>();
	}
}
