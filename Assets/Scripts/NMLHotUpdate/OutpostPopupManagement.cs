using TWDModel;
using UnityEngine;

public class OutpostPopupManagement : HUDElement
{
	[SerializeField]
	private UIToggleContent LoadingDataParent;

	[SerializeField]
	private UIToggleContent LoadingDataCompleteParent;

	[Header("Loading Complete")]
	[SerializeField]
	private OutpostSliceEdit SlicePreview;

	[SerializeField]
	private UIButton EditButton;

	[SerializeField]
	private UIButton NewButton;

	[SerializeField]
	private UIButton TeamButton;

	[SerializeField]
	private UIButton LogButton;

	[SerializeField]
	private UIButton AbandonButton;

	[SerializeField]
	private UIButton HighScoreButton;

	[SerializeField]
	private UIButton AttackButton;

	[SerializeField]
	private UIButton TestOutpost;

	[Header("Outpost Details")]
	[SerializeField]
	private OutpostDetailsPanelStored OutpostDetails;

	private RunLocationModel CurrentRunLocationModel;

	private bool LoadingData;

	private const string LogDebug = "OutpostPopupManagement: ";

	public override void Open()
	{
		base.Open();
		OutpostLevelModel storedLevelModel = GameManager.Instance.playerModel.OutpostModel.StoredLevelModel;
		if (storedLevelModel != null && CurrentRunLocationModel == null)
		{
			LoadingData = true;
			UpdateUI();
			RunLocationLoader.LoadLocationModel(GameManager.Instance.gameEconomyData.GetOutpostTemplateDefinitionForMissionId(storedLevelModel.BaseRunLocationID), LoadingDone, LoadingError);
		}
		else
		{
			LoadingData = false;
			UpdateUI();
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_outpostmanagement");
	}

	public void OnDisable()
	{
		RemoveCallbacks();
	}

	private void LoadingDone(RunLocationModel model)
	{
		LoadingData = false;
		CurrentRunLocationModel = model;
		RemoveCallbacks();
		UpdateUI();
	}

	private void LoadingError(RunLocationModel model)
	{
		CurrentRunLocationModel = null;
		RemoveCallbacks();
		Close();
	}

	private void RemoveCallbacks()
	{
		RunLocationLoader.ClearCallbacks(LoadingDone, LoadingError);
	}

	public override void UpdateUI()
	{
		if (!(GameManager.Instance != null))
		{
			return;
		}
		base.UpdateUI();
		LoadingDataParent.gameObject.SetActive(LoadingData);
		LoadingDataCompleteParent.gameObject.SetActive(!LoadingData);
		bool active = GameManager.Instance.playerModel.OutpostModel.StoredLevelModel != null;
		if (SlicePreview != null && !RunLocationLoader.IsLoading)
		{
			if (GameManager.Instance.playerModel.OutpostModel.StoredLevelModel != null && CurrentRunLocationModel != null)
			{
				SlicePreview.gameObject.SetActive(value: true);
				if (SlicePreview.OutpostLevelModel == null)
				{
					SlicePreview.OutpostLevelModel = GameManager.Instance.playerModel.OutpostModel.StoredLevelModel;
					SlicePreview.OutpostTemplateModel = CurrentRunLocationModel;
				}
				SlicePreview.UpdateSlices();
			}
			else
			{
				SlicePreview.gameObject.SetActive(value: false);
			}
		}
		if (EditButton != null && NewButton != null && TeamButton != null)
		{
			EditButton.gameObject.SetActive(active);
			NewButton.gameObject.SetActive(value: true);
			TeamButton.gameObject.SetActive(value: true);
		}
		if (TestOutpost != null)
		{
			TestOutpost.gameObject.SetActive(active);
		}
		if (AbandonButton != null)
		{
			AbandonButton.gameObject.SetActive(active);
		}
		if (LogButton != null)
		{
			LogButton.gameObject.SetActive(active);
		}
		if (HighScoreButton != null)
		{
			HighScoreButton.gameObject.SetActive(value: true);
		}
		if (AttackButton != null)
		{
			AttackButton.gameObject.SetActive(active);
		}
		if (OutpostDetails != null)
		{
			OutpostDetails.UpdateUI();
		}
	}

	public void OnClickEditButton()
	{
		if (OutpostEditManager.CallStartEditingOutpost())
		{
			ContinueToEditPopup();
			return;
		}
		Debug.LogError("OutpostPopupManagement: Could not start edit outpost!");
		Close();
	}

	public void OnClickNewButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_outpost_create");
		ContinueToEditPopup(createNewOutpost: true);
	}

	public void OnClickTeam()
	{
		TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
		obj.SurvivorType = SurvivorContainerModel.SurvivorType.Outpost;
		obj.Open();
	}

	public void OnClicktTestOutpost()
	{
	}

	public void OnClickLog()
	{
		Close();
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopupBattleLog) as OutpostPopupBattleLog).Open();
	}

	public void OnClickHighScore()
	{
		Close();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostHighscorePopup).Open();
	}

	public void OnClickAttackOutpost()
	{
		Close();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MatchmakingPopup).Open();
	}

	public override void OnClickClose()
	{
		if (TutorialView.Allowed("Close"))
		{
			Close();
		}
	}

	private void ContinueToEditPopup(bool createNewOutpost = false)
	{
		Close();
		OutpostEditPopup outpostEditPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopupEdit) as OutpostEditPopup;
		if (createNewOutpost)
		{
			outpostEditPopup.State = OutpostManagementState.SelectBackground;
		}
		else
		{
			outpostEditPopup.State = OutpostManagementState.SliceEdit;
		}
		outpostEditPopup.Open();
	}
}
