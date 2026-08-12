using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivorClassFilter : MonoBehaviour
{
	public delegate void OnClassFilterSelectedCallback(SurvivorClass selectedClass);

	[Tooltip("The survivor list panel.")]
	public SurvivorsListPanel SurvivorList;

	public SurvivorListFilter CurrentSelectedFilter = new SurvivorListFilter();

	[Tooltip("The buttons offsets.")]
	[SerializeField]
	private float buttonsOffset = -70f;

	private List<SurvivorClass> disabledClasses = new List<SurvivorClass>();

	private bool genericFilterButtonsEnabled = true;

	private bool heroFilterButtonsEnabled = true;

	private SurvivorButtonFilter[] allButtons;

	private UITabs uiTabs;

	private List<SurvivorButtonFilter> generalFilterButtons;

	public event OnClassFilterSelectedCallback OnClassFilterSelected;

	public void SetGenericFilterButtonsEnabled(bool active)
	{
		genericFilterButtonsEnabled = active;
	}

	public void SetHeroFilterButtonsEnabled(bool active)
	{
		heroFilterButtonsEnabled = active;
	}

	public void Start()
	{
		PlayerModel player = GameManager.Instance.modelManager.Player;
		allButtons = base.gameObject.GetComponentsInChildren<SurvivorButtonFilter>(includeInactive: true);
		int trainingGroundLevel = player.Camp.GetTrainingGroundLevel();
		for (int i = 0; i < 6; i++)
		{
			SurvivorClass survivorClass = (SurvivorClass)i;
			int minimumTrainingGroundLevelForClass = player.gameEconomyData.GetMinimumTrainingGroundLevelForClass(survivorClass);
			if (trainingGroundLevel < minimumTrainingGroundLevelForClass && player.SurvivorContainer.GetSurvivorsOfClass(survivorClass).Count == 0 && player.SurvivorContainer.IsSurvivorClassUnlocked(survivorClass))
			{
				disabledClasses.Add(survivorClass);
			}
		}
		UpdatePositionAndState();
		if (IsLoadDataManager)
		{
			var grid = allButtons[0].transform.parent.GetComponent<UIGrid>();
			if (grid)
			{
				grid.Reposition();
			}
		}
	}

	public void EnableButtonForClass(SurvivorClass survivorClass, bool enable)
	{
		if (enable && disabledClasses.Contains(survivorClass))
		{
			disabledClasses.Remove(survivorClass);
		}
		else if (!enable && !disabledClasses.Contains(survivorClass))
		{
			disabledClasses.Add(survivorClass);
		}
	}

	public SurvivorClass GetFirstAvailableClass()
	{
		SurvivorButtonFilter[] componentsInChildren = base.gameObject.GetComponentsInChildren<SurvivorButtonFilter>(includeInactive: true);
		foreach (SurvivorButtonFilter survivorButtonFilter in componentsInChildren)
		{
			if (survivorButtonFilter.SurvivorClass != SurvivorClass.None && !disabledClasses.Contains(survivorButtonFilter.SurvivorClass))
			{
				return survivorButtonFilter.SurvivorClass;
			}
		}
		return SurvivorClass.None;
	}

	public void UpdatePositionAndState()
	{
		if (allButtons == null)
		{
			return;
		}
		if (generalFilterButtons == null)
		{
			generalFilterButtons = new List<SurvivorButtonFilter>();
		}
		else
		{
			generalFilterButtons.Clear();
		}
		int num = 0;
		for (int i = 0; i < allButtons.Length; i++)
		{
			if (allButtons[i].SurvivorClass == SurvivorClass.None)
			{
				generalFilterButtons.Add(allButtons[i]);
				continue;
			}
			bool flag = !disabledClasses.Contains(allButtons[i].SurvivorClass);
			allButtons[i].gameObject.SetActive(flag);
			if (flag)
			{
				allButtons[i].transform.localPosition = new Vector3(buttonsOffset * (float)num, 0f, 0f);
				num++;
			}
		}
		bool flag2 = false;
		for (int j = 0; j < generalFilterButtons.Count; j++)
		{
			if ((generalFilterButtons[j].FilterType != SurvivorListFilter.FilterType.Hero || heroFilterButtonsEnabled) ? Helpers.GameObjectSetActive(generalFilterButtons[j], genericFilterButtonsEnabled) : Helpers.GameObjectSetActive(generalFilterButtons[j], value: false))
			{
				generalFilterButtons[j].transform.localPosition = new Vector3(buttonsOffset * (float)num, 0f, 0f);
				num++;
			}
		}
		generalFilterButtons.Clear();
	}

	public UITabs GetUITabs()
	{
		if (uiTabs == null)
		{
			uiTabs = GetComponent<UITabs>();
		}
		return uiTabs;
	}

	public void SetActiveButton(int index)
	{
		if (GetUITabs() != null)
		{
			GetUITabs().SelectTab(index);
		}
	}

	public GameObject GetButtonForClass(SurvivorClass survivorClass)
	{
		SurvivorButtonFilter[] componentsInChildren = base.gameObject.GetComponentsInChildren<SurvivorButtonFilter>(includeInactive: true);
		foreach (SurvivorButtonFilter survivorButtonFilter in componentsInChildren)
		{
			if (survivorButtonFilter.SurvivorClass == survivorClass)
			{
				return survivorButtonFilter.gameObject;
			}
		}
		return null;
	}

	public void SetSelectedClass(SurvivorClass survivorClass)
	{
		GameObject buttonForClass = GetButtonForClass(survivorClass);
		OnFilterClicked(buttonForClass);
	}

	public void SetSelectedClass(SurvivorListFilter filterSettings)
	{
		if (SurvivorList != null)
		{
			SurvivorList.FilterSettings = filterSettings;
			SurvivorList.SetupCardsByFiltering();
		}
		if (this.OnClassFilterSelected != null)
		{
			this.OnClassFilterSelected(filterSettings.ClassFilter);
		}
		GameObject buttonForClass = GetButtonForClass(filterSettings.ClassFilter);
		if (buttonForClass != null && !buttonForClass.GetComponent<UIToggle>().value)
		{
			buttonForClass.GetComponent<UIToggle>().value = true;
		}
		CurrentSelectedFilter = filterSettings;
		UIEvent.Send("OnSurvivorInfoClosed");
	}

	public void OnFilterClicked(GameObject filterButton)
	{
		if (filterButton != null)
		{
			SurvivorButtonFilter component = filterButton.GetComponent<SurvivorButtonFilter>();
			SetSelectedClass(component.FilterSettings);
			if (component.FilterType == SurvivorListFilter.FilterType.Hero)
			{
				EventManager.NotifyClick("HeroTab");
			}
			UIEvent.Send("OnClickSurvivorFilter");
		}
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion
}