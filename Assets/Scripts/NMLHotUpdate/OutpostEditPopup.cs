using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OutpostEditPopup : HUDElement
{
	public GameObject HelpButton;

	public OutpostManagementState State;

	public GameObject HeaderContainer;

	public UILabel HeaderLabel;

	public GameObject NextButton;

	public GameObject PrevButton;

	public UILabel NextLabel;

	public UILabel PrevLabel;

	public List<StatePanel> StatePanels;

	public static StatePanel CurrentActiveStatePanel;

	public override void Open()
	{
		for (int i = 0; i < StatePanels.Count; i++)
		{
			StatePanel statePanel = StatePanels[i];
			statePanel.OutpostStateBase.gameObject.SetActive(value: false);
			statePanel.OutpostStateBase.OutpostLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
			if (statePanel.OutpostStateBase.OutpostLevelModel != null)
			{
				statePanel.OutpostStateBase.OutpostTemplateModel = GameManager.Instance.playerModel.GetOutpostTemplate(GameManager.Instance.playerModel.OutpostModel.EditLevelModel.BaseRunLocationID);
			}
			statePanel.OutpostStateBase.OnRequestStateChange -= OnRequestStateChange;
			statePanel.OutpostStateBase.OnRequestStateChange += OnRequestStateChange;
		}
		OutpostLevelModel editLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
		if (editLevelModel != null)
		{
			OutpostTemplateDefinition outpostTemplateDefinitionForMissionId = GameManager.Instance.gameEconomyData.GetOutpostTemplateDefinitionForMissionId(editLevelModel.BaseRunLocationID);
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
			RunLocationLoader.LoadLocationModel(outpostTemplateDefinitionForMissionId, LoadingDone, LoadingError);
		}
		CampHUD.CurrencyHudSetActive(enable: false);
		if (State == OutpostManagementState.SelectBackground)
		{
			LoadingDone(null);
		}
		base.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		if (OutpostPopup.HasBuildingAndCorrectLevelToEdit())
		{
			TutorialView.Instance.StartPart("OutpostEditUnlocked");
		}
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
		for (int i = 0; i < StatePanels.Count; i++)
		{
			StatePanels[i].OutpostStateBase.OnRequestStateChange -= OnRequestStateChange;
		}
		CampHUD.CurrencyHudSetActive(enable: true);
	}

	private void LoadingDone(RunLocationModel model)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Close();
		UpdateUI();
	}

	private void LoadingError(RunLocationModel model)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Close();
		Close();
	}

	public override void OnClickClose()
	{
		if (TutorialView.Allowed("Close"))
		{
			OutpostEditManager.CallStopEditingOutpost();
			Close();
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopup).Open();
		}
	}

	private void OnRequestStateChange(StateChangeDirection direction)
	{
		switch (direction)
		{
		case StateChangeDirection.Prev:
			OnPrevStep();
			break;
		case StateChangeDirection.Next:
			OnNextStep();
			break;
		}
	}

	public void PublishOutpost()
	{
		OutpostEditManager.CallStopEditingOutpost(publishCurrentEdit: true);
		OnClickClose();
	}

	public OutpostStateBase GetStatePanel(OutpostManagementState state)
	{
		for (int i = 0; i < StatePanels.Count; i++)
		{
			StatePanel statePanel = StatePanels[i];
			if (statePanel.State == state)
			{
				return statePanel.OutpostStateBase;
			}
		}
		return null;
	}

	public void OnNextStep()
	{
		OutpostManagementState state = OutpostManagementState.MainMenu;
		switch (State)
		{
		case OutpostManagementState.SelectBackground:
			OutpostEditManager.CallCreateOutpostCommand();
			state = OutpostManagementState.SliceEdit;
			break;
		case OutpostManagementState.SliceEdit:
		{
			OutpostStateSlicePlaceItems obj = GetStatePanel(OutpostManagementState.SlicePlaceItems) as OutpostStateSlicePlaceItems;
			OutpostStateSliceEdit outpostStateSliceEdit = GetStatePanel(OutpostManagementState.SliceEdit) as OutpostStateSliceEdit;
			obj.SelectedSlice = outpostStateSliceEdit.SelectedSlice;
			state = OutpostManagementState.SlicePlaceItems;
			break;
		}
		}
		State = state;
		UpdateUI();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
	}

	public void OnPrevStep()
	{
		OutpostManagementState state = OutpostManagementState.MainMenu;
		switch (State)
		{
		case OutpostManagementState.SliceEdit:
			state = OutpostManagementState.SelectBackground;
			break;
		case OutpostManagementState.SlicePlaceItems:
			state = OutpostManagementState.SliceEdit;
			break;
		}
		State = state;
		UpdateUI();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
	}

	public override void UpdateUI()
	{
		for (int i = 0; i < StatePanels.Count; i++)
		{
			StatePanel statePanel = StatePanels[i];
			if (!(statePanel.OutpostStateBase != null))
			{
				continue;
			}
			statePanel.OutpostStateBase.OutpostLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
			if (statePanel.OutpostStateBase.OutpostLevelModel != null)
			{
				statePanel.OutpostStateBase.OutpostTemplateModel = GameManager.Instance.playerModel.GetOutpostTemplate(GameManager.Instance.playerModel.OutpostModel.EditLevelModel.BaseRunLocationID);
			}
			statePanel.OutpostStateBase.gameObject.SetActive(State == statePanel.State);
			if (State == statePanel.State)
			{
				if (statePanel.OutpostStateBase.ShowHeader && HeaderLabel != null)
				{
					HeaderLabel.text = statePanel.OutpostStateBase.GetTitle();
				}
				if (HeaderContainer != null)
				{
					HeaderContainer.SetActive(statePanel.OutpostStateBase.ShowHeader);
				}
				CurrentActiveStatePanel = statePanel;
			}
		}
		if (PrevButton != null)
		{
			PrevButton.SetActive(value: false);
		}
		if (NextButton != null)
		{
			NextButton.SetActive(value: false);
		}
	}

	public void ShowOutpostTutorialFromClick()
	{
		for (int i = 0; i < StatePanels.Count; i++)
		{
			if (StatePanels[i].OutpostStateBase.gameObject.activeSelf)
			{
				GameObject getTutorialPanel = StatePanels[i].OutpostStateBase.GetTutorialPanel;
				if (getTutorialPanel != null)
				{
					getTutorialPanel.SetActive(value: true);
				}
			}
		}
	}
}
