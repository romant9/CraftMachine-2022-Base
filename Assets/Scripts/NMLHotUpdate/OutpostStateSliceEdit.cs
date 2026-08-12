using System;
using TWDModel;
using UnityEngine;

public class OutpostStateSliceEdit : OutpostStateBase
{
	[SerializeField]
	private OutpostSliceEdit SliceEdit;

	[SerializeField]
	private GameObject DetailsEditParent;

	[SerializeField]
	private GameObject DetailsPublishParent;

	[SerializeField]
	private OutpostPlacedDetails PlacedDetails;

	[SerializeField]
	private OutpostDetailsPanelEdit DetailsPanel;

	[SerializeField]
	private UIButton AutoFillButton;

	[SerializeField]
	private OutpostHelpPopup outpostHelpPopup;

	[SerializeField]
	private OutpostHelpPopup outpostHelpPopupCanEdit;

	public SlicePosition SelectedSlice { get; private set; }

	public override GameObject GetTutorialPanel
	{
		get
		{
			if (OutpostPopup.HasBuildingAndCorrectLevelToEdit())
			{
				return outpostHelpPopupCanEdit.gameObject;
			}
			return outpostHelpPopup.gameObject;
		}
	}

	private void Awake()
	{
		outpostHelpPopup.gameObject.SetActive(!GameManager.Instance.playerModel.HasPublishedOutpost);
		outpostHelpPopupCanEdit.gameObject.SetActive(value: false);
	}

	public void OnEnable()
	{
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (GameManager.Instance.playerModel.OutpostModel == null || GameManager.Instance.playerModel.OutpostModel.EditLevelModel == null)
		{
			return;
		}
		OutpostLevelModel editLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
		RunLocationModel outpostTemplate = GameManager.Instance.playerModel.GetOutpostTemplate(editLevelModel.BaseRunLocationID);
		if (SliceEdit != null && GameManager.Instance != null && outpostTemplate != null && editLevelModel != null)
		{
			if (SliceEdit.OutpostLevelModel == null)
			{
				SliceEdit.OutpostLevelModel = editLevelModel;
				SliceEdit.OutpostTemplateModel = outpostTemplate;
				SliceEdit.Callback = OnClickedSlice;
				SliceEdit.UpdateParentCallback = UpdateDetails;
			}
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Close();
			HUDNotification.Hide();
			SliceEdit.UpdateSlices(enableEdit: true);
			UpdateDetails();
			if (DetailsPanel != null)
			{
				DetailsPanel.UpdateUI();
			}
			TutorialView.Instance.ShowButtonSuggest("OutpostPlaceDefendersButton", GameManager.Instance.playerModel.OutpostModel.OutpostRunLocation == null && !DetailsPublishParent.activeSelf);
		}
	}

	private void UpdateDetails()
	{
		if (DetailsEditParent != null && DetailsPublishParent != null)
		{
			bool flag = ValidateEditOutpostModel(GameManager.Instance.playerModel.OutpostModel.EditLevelModel);
			DetailsEditParent.SetActive(!flag);
			DetailsPublishParent.SetActive(flag);
		}
		if (PlacedDetails != null)
		{
			PlacedDetails.UpdateUI();
		}
	}

	private void OnClickedSlice(ButtonBase button)
	{
		if (button != null && SliceEdit != null)
		{
			if (!OutpostPopup.HasBuildingAndCorrectLevelToEdit())
			{
				FeatureLockedPopup.Open(FeatureLockedPopup.FeatureType.OutpostEdit, locked: true);
				return;
			}
			SelectedSlice = (SlicePosition)Enum.Parse(typeof(SlicePosition), button.id);
			RequestStateChange(StateChangeDirection.Next);
		}
	}

	private void PerformRandomize()
	{
		OutpostEditManager.AutoFill();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_autofill");
		base.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: true);
	}

	public void OnClickAutoFill()
	{
		PerformRandomize();
	}

	private bool ValidateEditOutpostModel(OutpostLevelModel editModel)
	{
		if (editModel != null && editModel.GetDefenderCount() > 2)
		{
			return true;
		}
		return false;
	}
}
