using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OutpostSliceEdit : OutpostStateBase
{
	public GameObject SlicePrefab;

	public float SliceScale = 0.5f;

	public float SliceMarginScale;

	public GameObject SliceContainer;

	private List<SlicePanel> Slices = new List<SlicePanel>();

	private SlicePosition CurrentSlicePosition;

	public SlicePosition SelectedSlice { get; private set; }

	public ButtonBase.ButtonBaseCallback Callback { get; set; }

	public Callback UpdateParentCallback { get; set; }

	public bool EditSelectSlicesMode { get; private set; }

	public bool HasContent
	{
		get
		{
			if (Slices != null && Slices.Count > 0)
			{
				for (int i = 0; i < Slices.Count; i++)
				{
					if (base.OutpostLevelModel.GetTotalUsedDeploymentForSlice(Slices[i].SliceViewId) > 0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public void CreateSlices()
	{
		CreateChosenSlice(SlicePosition.First);
		CreateChosenSlice(SlicePosition.Second);
		CreateChosenSlice(SlicePosition.Third);
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
		DestroySlices();
	}

	private void ChangeSlice(SlicePosition slicePosition, int indexModifier)
	{
		SlicePanel slicePanel = null;
		for (int i = 0; i < Slices.Count; i++)
		{
			if (Slices[i].Position == slicePosition)
			{
				slicePanel = Slices[i];
				break;
			}
		}
		string text = base.OutpostLevelModel.GetChosenSliceViewId(slicePosition);
		if (text == null)
		{
			text = base.OutpostTemplateModel.GetSliceViewIds(slicePosition)[0];
		}
		int index = 0;
		int count = 0;
		base.OutpostTemplateModel.GetSliceIndexAndCount(text, out index, out count);
		index += indexModifier;
		if (index >= count)
		{
			index = 0;
		}
		else if (index < 0)
		{
			index = count - 1;
		}
		string sliceViewId = base.OutpostTemplateModel.GetSliceViewIds(slicePosition)[index];
		Helpers.ExecuteCommand(new SetOutpostSliceCommand(slicePosition, sliceViewId, clearPrevious: true));
		DeleteSlicePanel(slicePanel);
		CreateChosenSlice(slicePosition);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_change_slice");
	}

	private void OnMainSliceInteraction(SlicePanel slicePanel, OutpostSliceHotspot hotspot, string eventId)
	{
		CurrentSlicePosition = slicePanel.Position;
		string chosenSliceViewId = base.OutpostLevelModel.GetChosenSliceViewId(CurrentSlicePosition);
		int totalUsedDeploymentForSlice = base.OutpostLevelModel.GetTotalUsedDeploymentForSlice(chosenSliceViewId);
		switch (eventId)
		{
		case "SliceClicked":
			if (slicePanel != null)
			{
				SelectedSlice = slicePanel.Position;
				RequestStateChange(StateChangeDirection.Next);
			}
			break;
		case "NextClicked":
			if (totalUsedDeploymentForSlice > 0)
			{
				ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.Outpost.Edit.ClearSlice.Title"), LocalizationManager.GetText("Popup.Outpost.Edit.ClearSlice.Message"), "", NextConfirmed);
			}
			else
			{
				NextConfirmed();
			}
			break;
		case "PrevClicked":
			if (totalUsedDeploymentForSlice > 0)
			{
				ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.Outpost.Edit.ClearSlice.Title"), LocalizationManager.GetText("Popup.Outpost.Edit.ClearSlice.Message"), "", PrevConfirmed);
			}
			else
			{
				PrevConfirmed();
			}
			break;
		}
	}

	private void NextConfirmed()
	{
		ChangeSlice(CurrentSlicePosition, 1);
		if (UpdateParentCallback != null)
		{
			UpdateParentCallback();
		}
	}

	private void PrevConfirmed()
	{
		ChangeSlice(CurrentSlicePosition, -1);
		if (UpdateParentCallback != null)
		{
			UpdateParentCallback();
		}
	}

	private void CreateChosenSlice(SlicePosition slicePosition)
	{
		if (SliceContainer != null && SlicePrefab != null)
		{
			string text = base.OutpostLevelModel.GetChosenSliceViewId(slicePosition);
			if (text == null)
			{
				text = base.OutpostTemplateModel.GetSliceViewIds(slicePosition)[0];
			}
			int index = 0;
			int count = 0;
			base.OutpostTemplateModel.GetSliceIndexAndCount(text, out index, out count);
			SlicePanel slicePanel = SlicePanel.CreateSlicePanel(base.OutpostTemplateModel, SlicePrefab, SliceContainer, SliceScale, SliceMarginScale, (int)slicePosition, 3, slicePosition, text, base.OutpostLevelModel);
			if (slicePanel != null)
			{
				string label = (EditSelectSlicesMode ? (index + 1 + " / " + count) : "");
				Slices.Add(slicePanel);
				slicePanel.SetLabel(label);
				slicePanel.EnableSliceSelectClick = EditSelectSlicesMode;
				slicePanel.ShowArrows(EditSelectSlicesMode);
				slicePanel.ShowSidewaysArrows(enabled: false);
				slicePanel.OnSliceInteraction -= OnMainSliceInteraction;
				slicePanel.OnSliceInteraction += OnMainSliceInteraction;
				slicePanel.id = slicePosition.ToString();
				slicePanel.SetCallback(Callback);
			}
		}
	}

	public void UpdateSlices(bool enableEdit = false)
	{
		EditSelectSlicesMode = enableEdit;
		if (Slices == null || Slices.Count == 0)
		{
			CreateSlices();
			return;
		}
		for (int i = 0; i < Slices.Count; i++)
		{
			Slices[i].UpdateSlice();
		}
	}

	private void DestroySlices()
	{
		Slices.Clear();
		if (!(SliceContainer != null))
		{
			return;
		}
		foreach (Transform item in SliceContainer.transform)
		{
			DeleteSlicePanel(item.gameObject.GetComponent<SlicePanel>());
		}
	}

	private void DeleteSlicePanel(SlicePanel slicePanel)
	{
		if (slicePanel != null)
		{
			if (Slices != null)
			{
				Slices.Remove(slicePanel);
			}
			slicePanel.OnSliceInteraction -= OnMainSliceInteraction;
			Object.Destroy(slicePanel.gameObject);
		}
	}
}
