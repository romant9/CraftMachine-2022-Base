using System;
using System.Threading.Tasks;
using TWDModel;
using UnityEngine;

public class TeamPresetSelectionPanel : MonoBehaviour, IInterceptor
{
	private class CurrentTeamProvider : ITeamPresetData
	{
		public SurvivorModel[] Survivors => GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors.ToArray();

		public string[] Supports => GameManager.Instance.playerModel.EquippedSupportIds;
	}

	[Serializable]
	private class PresetButton
	{
		public UIButtonToggle ButtonToggle;

		public GameObject LockObject;

		public GameObject ModifiedObject;
	}

	[SerializeField]
	private PresetButton[] buttons;

	[SerializeField]
	private GameObject[] unsavedChangeObjects;

	[SerializeField]
	private GameObject changesSavedObject;

	private SurvivorContainerModel.SurvivorType survivorType;

	private TeamPresetsManager presetsManager;

	private CurrentTeamProvider currentTeam;

	private PlayerModel player;

	private int currentPresetIndex;

	private bool hasUnsavedChanges;

	private void Awake()
	{
		player = GameManager.Instance.playerModel;
		currentTeam = new CurrentTeamProvider();
		presetsManager = GameManager.Instance.playerModel.TeamPresetsManager;
		TeamPresetData[] presetData = GameManager.Instance.gameEconomyData.TeamPresets;
		for (int i = 0; i < buttons.Length; i++)
		{
			int index = i;
			EventDelegate.Add(buttons[i].ButtonToggle.onClick, delegate
			{
				OnButtonClick(index);
			});
			EventDelegate.Add(buttons[i].LockObject.GetComponent<UIButton>().onClick, delegate
			{
				TeamPresetClientHelpers.LockTooltip(presetData, buttons[index].LockObject, index);
			});
		}
	}

	public void RefreshShowState(SurvivorContainerModel.SurvivorType type, MapMissionModel mapMissionModel)
	{
		survivorType = type;
		if (type != SurvivorContainerModel.SurvivorType.Outpost && type != SurvivorContainerModel.SurvivorType.GvGDefenders && type != SurvivorContainerModel.SurvivorType.CombatSurvival && (mapMissionModel == null || !mapMissionModel.IsFixedSurvivorSeasonMission) && (mapMissionModel?.MaxTeamSize ?? 3) == 3 && (mapMissionModel == null || !mapMissionModel.IsEndlessMission || !EndlessModeHelpers.IsEndlessExpertMode()) && TeamPresetHelpers.IsFeatureUnlocked(player))
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (!(type == "ReloadSurvivorList"))
		{
			switch (type)
			{
			default:
				return;
			case "ReloadSurvivorList":
			case "NewSupportEquipped":
			case "OnNewSurvivorSelected":
				break;
			}
		}
		RefreshModified();
	}

	private void Show()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		for (int i = 0; i < buttons.Length; i++)
		{
			bool flag = TeamPresetHelpers.IsPresetSlotUnlocked(player, i);
			Helpers.GameObjectSetActive(buttons[i].LockObject, !flag);
			buttons[i].ButtonToggle.enabled = flag;
			Helpers.GameObjectSetActive(buttons[i].ModifiedObject, value: false);
		}
		hasUnsavedChanges = false;
		SetPreset(-1);
	}

	private void Hide()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnButtonClick(int index)
	{
		SetPreset((index != currentPresetIndex) ? index : (-1));
	}

	private async void SetPreset(int index)
	{
		bool flag = index != currentPresetIndex && hasUnsavedChanges;
		if (flag)
		{
			flag = !(await ShowUnsavedChangesPopup());
		}
		if (flag)
		{
			RefreshToggleState();
			return;
		}
		currentPresetIndex = index;
		if (index >= 0)
		{
			TeamTeamPreset teamTeamPreset = presetsManager.Presets[index];
			Helpers.ExecuteCommand(new SetCombatSurvivorsCommand(survivorType, teamTeamPreset.Survivors));
			for (int i = 0; i < teamTeamPreset.Supports.Length; i++)
			{
				Helpers.ExecuteCommand(new EquipSupportCommand(i, teamTeamPreset.Supports[i]));
			}
			UIEvent.Send("ReloadSurvivorList");
			UIEvent.Send("ReloadSurvivorList");
		}
		RefreshToggleState();
		ResetModifiedIndicators();
		RefreshModified();
	}

	private void RefreshToggleState()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].ButtonToggle.SetToggled(currentPresetIndex == i);
		}
	}

	public void InfoClick()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.TeamPresetsInfoPopup)?.Open();
	}

	private void SetUnsavedChangeObjectsActive(bool active)
	{
		GameObject[] array = unsavedChangeObjects;
		for (int i = 0; i < array.Length; i++)
		{
			Helpers.GameObjectSetActive(array[i], active);
		}
	}

	private void ResetModifiedIndicators()
	{
		PresetButton[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			Helpers.GameObjectSetActive(array[i].ModifiedObject, value: false);
		}
	}

	private void RefreshModified()
	{
		if (currentPresetIndex >= 0)
		{
			TeamTeamPreset teamPresetData = presetsManager.Presets[currentPresetIndex];
			hasUnsavedChanges = currentTeam.IsValid() && !teamPresetData.AreEquivalent(currentTeam);
			SetUnsavedChangeObjectsActive(hasUnsavedChanges);
			Helpers.GameObjectSetActive(buttons[currentPresetIndex].ModifiedObject, hasUnsavedChanges);
		}
		else
		{
			SetUnsavedChangeObjectsActive(active: false);
		}
		Helpers.GameObjectSetActive(changesSavedObject, value: false);
	}

	public async void Save()
	{
		if (currentPresetIndex >= 0 && hasUnsavedChanges && await HelpersUI.ConfirmationPopupAsync("Popup.TeamSelection.TeamPreset.SaveConfirmationWarningTitle", "Popup.TeamSelection.TeamPreset.SaveConfirmationWarningText", "Button.Save"))
		{
			TeamPresetClientHelpers.SavePreset(currentPresetIndex, currentTeam);
			RefreshModified();
			Helpers.GameObjectSetActive(changesSavedObject, value: true);
		}
	}

	public async Task<bool> Intercept()
	{
		if (base.gameObject.activeInHierarchy && hasUnsavedChanges)
		{
			return await HelpersUI.ConfirmationPopupAsync("Popup.TeamSelection.TeamPreset.SaveWarningTitle", "Popup.TeamSelection.TeamPreset.PlayWarningText", "Button.Continue");
		}
		return true;
	}

	private Task<bool> ShowUnsavedChangesPopup()
	{
		return HelpersUI.ConfirmationPopupAsync("Popup.TeamSelection.TeamPreset.SaveWarningTitle", "Popup.TeamSelection.TeamPreset.SaveWarningText", "Button.Continue");
	}
}
