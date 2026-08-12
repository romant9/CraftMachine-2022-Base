using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Camp;
using TWDModel;
using UnityEngine;

public class TeamPresetEditPanel : MonoBehaviour, ISurvivorSlotProvider, IInterceptor
{
	private class Preset : ITeamPresetData
	{
		public SurvivorModel[] Survivors { get; set; }

		public string[] Supports { get; set; }

		public bool Modified { get; set; }
	}

	[Serializable]
	private class PresetButton
	{
		public UIButtonToggle ButtonToggle;

		public GameObject UnsavedChangeObject;

		public GameObject LockObject;
	}

	[Serializable]
	private class SurvivorSlot
	{
		public SurvivorCard SurvivorCard;

		public TeamSelectionEmptyCard EmptyCard;
	}

	[SerializeField]
	private GameObject teamSelectionSurvivorsListPanelPrefab;

	[SerializeField]
	private PresetButton[] presetButtons;

	[SerializeField]
	private SurvivorSlot[] survivorSlots;

	[SerializeField]
	private SmallSupportCard[] supportCards;

	[SerializeField]
	private UIInterceptableTabs interceptedTabs;

	[SerializeField]
	private SurvivorManagementPopUp interceptedSurvivorManagementPopUp;

	[SerializeField]
	private GameObject changesSavedContainer;

	[SerializeField]
	private GameObject unsavedChangesContainer;

	[SerializeField]
	private GameObject saveResetButtonsContainer;

	[SerializeField]
	private GameObject invalidTeamContainer;

	[SerializeField]
	private GameObject supportsContainer;

	private IList<Preset> presets;

	private PlayerModel player;

	private TeamPresetsManager presetsManager;

	private int currentPresetIndex;

	private SupportSelectionPanel supportSelectionPanel;

	private TeamSelectionSurvivorsListPanel teamSelectionSurvivorsListPanel;

	private int selectedSurvivorSlot;

	private Preset CurrentPreset => presets[currentPresetIndex];

	private bool ModifiedPresetsExist => presets.Any((Preset preset) => preset.Modified);

	public Transform SelectedSlotPosition => survivorSlots[selectedSurvivorSlot].EmptyCard.transform;

	public Transform FirstSlotPosition => survivorSlots[0].EmptyCard.transform;

	private void Awake()
	{
		player = GameManager.Instance.playerModel;
		presetsManager = player.TeamPresetsManager;
		TeamPresetData[] presetData = GameManager.Instance.gameEconomyData.TeamPresets;
		presets = new List<Preset>();
		for (int i = 0; i < presetButtons.Length; i++)
		{
			int index = i;
			EventDelegate.Add(presetButtons[i].ButtonToggle.onClick, delegate
			{
				SelectPreset(index);
			});
			EventDelegate.Add(presetButtons[i].LockObject.GetComponent<UIButton>().onClick, delegate
			{
				TeamPresetClientHelpers.LockTooltip(presetData, presetButtons[index].LockObject, index);
			});
		}
		for (int num = 0; num < survivorSlots.Length; num++)
		{
			survivorSlots[num].EmptyCard.SlotIndex = num;
		}
		supportSelectionPanel = Helpers.InstantiateToParent(teamSelectionSurvivorsListPanelPrefab, base.gameObject).GetComponent<SupportSelectionPanel>();
		Helpers.GameObjectSetActive(supportSelectionPanel, value: false);
		teamSelectionSurvivorsListPanel = supportSelectionPanel.GetComponent<TeamSelectionSurvivorsListPanel>();
		teamSelectionSurvivorsListPanel.IncludeTeamSurvivors = true;
		teamSelectionSurvivorsListPanel.SetFilterOffset(new Vector3(0f, 60f, 0f));
		teamSelectionSurvivorsListPanel.SurvivorSlotProvider = this;
		if ((bool)interceptedTabs)
		{
			interceptedTabs.SetInterceptor(this);
		}
		if ((bool)interceptedSurvivorManagementPopUp)
		{
			interceptedSurvivorManagementPopUp.SetInterceptor(this);
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		ResetPresetChanges();
		SelectPreset(0);
		for (int i = 0; i < presetButtons.Length; i++)
		{
			bool flag = TeamPresetHelpers.IsPresetSlotUnlocked(player, i);
			Helpers.GameObjectSetActive(presetButtons[i].LockObject, !flag);
			presetButtons[i].ButtonToggle.enabled = flag;
		}
		Helpers.GameObjectSetActive(supportsContainer, player.SupportModels.Any((SupportModel support) => support.Unlocked));
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void ResetPresetChanges()
	{
		PresetButton[] array = presetButtons;
		for (int i = 0; i < array.Length; i++)
		{
			Helpers.GameObjectSetActive(array[i].UnsavedChangeObject, value: false);
		}
		presets.Clear();
		foreach (TeamTeamPreset preset in presetsManager.Presets)
		{
			presets.Add(new Preset
			{
				Survivors = preset.Survivors.ToArray(),
				Supports = preset.Supports.ToArray()
			});
		}
	}

	private void SelectPreset(int index)
	{
		currentPresetIndex = index;
		for (int i = 0; i < presetButtons.Length; i++)
		{
			presetButtons[i].ButtonToggle.SetToggled(i == index);
		}
		RefreshSurvivorSlots();
		RefreshSupports();
		RefreshModified();
	}

	private void RefreshModified()
	{
		bool flag = !presetsManager.Presets[currentPresetIndex].AreEquivalent(CurrentPreset) && CurrentPreset.IsValid();
		CurrentPreset.Modified = flag;
		Helpers.GameObjectSetActive(presetButtons[currentPresetIndex].UnsavedChangeObject, flag);
		Helpers.GameObjectSetActive(unsavedChangesContainer, flag);
		Helpers.GameObjectSetActive(saveResetButtonsContainer, flag);
		Helpers.GameObjectSetActive(changesSavedContainer, value: false);
	}

	public void SaveClick()
	{
		Save(currentPresetIndex);
	}

	private void Save(int index)
	{
		Preset preset = presets[index];
		if (preset.Modified)
		{
			TeamPresetClientHelpers.SavePreset(index, preset);
			if (index == currentPresetIndex)
			{
				RefreshModified();
				Helpers.GameObjectSetActive(changesSavedContainer, value: true);
			}
		}
	}

	private void SaveAll()
	{
		for (int i = 0; i < presets.Count; i++)
		{
			Save(i);
		}
	}

	public async void Reset()
	{
		if (await HelpersUI.ConfirmationPopupAsync("Popup.TeamSelection.TeamPreset.ResetWarningTitle", "Popup.TeamSelection.TeamPreset.ResetWarningText", "Button.Reset"))
		{
			ResetPresetChanges();
			SelectPreset(currentPresetIndex);
		}
	}

	public void InfoClick()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.TeamPresetsInfoPopup)?.Open();
	}

	private void RefreshSurvivorSlots()
	{
		for (int i = 0; i < CurrentPreset.Survivors.Length; i++)
		{
			SurvivorSlot survivorSlot = survivorSlots[i];
			SurvivorModel survivorModel = CurrentPreset.Survivors[i];
			Helpers.GameObjectSetActive(survivorSlot.EmptyCard, survivorModel == null);
			Helpers.GameObjectSetActive(survivorSlot.SurvivorCard, survivorModel != null);
			if (survivorModel != null)
			{
				survivorSlot.SurvivorCard.Item = survivorModel;
				survivorSlot.SurvivorCard.UpdateUI();
				survivorSlot.SurvivorCard.SetInfoButtonActive(active: true);
				survivorSlot.SurvivorCard.Type = SurvivorCard.CardType.TeamSelect;
			}
		}
		Helpers.GameObjectSetActive(invalidTeamContainer, !CurrentPreset.IsValid());
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (!(type == "OnNewSurvivorSelected"))
		{
			return;
		}
		if (parameter is SurvivorModel survivorModel)
		{
			int num = -1;
			for (int i = 0; i < CurrentPreset.Survivors.Length; i++)
			{
				if (CurrentPreset.Survivors[i] == survivorModel)
				{
					num = i;
					break;
				}
			}
			if (num >= 0 && !teamSelectionSurvivorsListPanel.gameObject.activeSelf)
			{
				OpenSurvivorSelectionPanel(num);
				return;
			}
			teamSelectionSurvivorsListPanel.ClosePanel();
			if (num >= 0)
			{
				CurrentPreset.Survivors[num] = CurrentPreset.Survivors[selectedSurvivorSlot];
			}
			CurrentPreset.Survivors[selectedSurvivorSlot] = survivorModel;
			RefreshSurvivorSlots();
			RefreshModified();
		}
		else
		{
			OpenSurvivorSelectionPanel((int)parameter);
		}
	}

	private void OpenSurvivorSelectionPanel(int slotIndex)
	{
		selectedSurvivorSlot = slotIndex;
		Helpers.GameObjectSetActive(teamSelectionSurvivorsListPanel, value: true);
		teamSelectionSurvivorsListPanel.OpenPanel();
	}

	private void OnSupportClick(int index)
	{
		string tempSupport = null;
		int tempIndex = -1;
		supportSelectionPanel.Show(index, CurrentPreset.Supports, MapCategory.None, delegate(SupportModel supportModel, int slot)
		{
			for (int i = 0; i < CurrentPreset.Supports.Length; i++)
			{
				if (CurrentPreset.Supports[i] == supportModel.SupportId)
				{
					tempSupport = CurrentPreset.Supports[slot];
					tempIndex = i;
					CurrentPreset.Supports[i] = null;
					break;
				}
			}
			CurrentPreset.Supports[slot] = supportModel.SupportId;
			if (tempIndex != -1 && tempSupport != null)
			{
				CurrentPreset.Supports[tempIndex] = tempSupport;
			}
			RefreshSupports();
			RefreshModified();
		});
	}

	private void OnSupportInfoClick(SupportModel supportModel, SupportCard card)
	{
		((SupportDetailsPopup)HUDManager.TryOpenPopup(UIType.SupportDetailsPopup)).Show(supportModel, canUpgrade: true, card.Refresh);
	}

	private void OnSupportRemoveClick(int index)
	{
		presets[currentPresetIndex].Supports[index] = null;
		supportCards[index].Initialize(null, delegate
		{
			OnSupportClick(index);
		});
		RefreshModified();
	}

	private void RefreshSupports()
	{
		for (int i = 0; i < CurrentPreset.Supports.Length; i++)
		{
			int supportIndex = i;
			SmallSupportCard supportCard = supportCards[i];
			SupportModel supportModel = player.GetSupportModel(CurrentPreset.Supports[i]);
			supportCard.Initialize(supportModel, delegate
			{
				OnSupportClick(supportIndex);
			}, delegate
			{
				OnSupportInfoClick(supportModel, supportCard);
			}, delegate
			{
				OnSupportRemoveClick(supportIndex);
			});
		}
	}

	public async Task<bool> Intercept()
	{
		if (base.gameObject.activeInHierarchy && presets.Any((Preset preset) => preset.Modified))
		{
			return await HelpersUI.ConfirmationPopupAsync("Popup.TeamSelection.TeamPreset.SaveWarningTitle", "Popup.TeamSelection.TeamPreset.SaveWarningText", "Button.Continue");
		}
		return true;
	}
}
