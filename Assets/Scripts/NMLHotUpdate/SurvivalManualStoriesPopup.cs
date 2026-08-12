using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class SurvivalManualStoriesPopup : HUDElement
{
	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject EntryContainer;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
	}

	private void OnUiEvent(string type, object parameter)
	{
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		ClearBTLevelEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = EntryContainer.GetComponentInParent<UIScrollView>();
		FreshListData();
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void ClearBTLevelEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	private void FreshListData()
	{
		ModelList<SurvivalManualModel> survivalManualModels = playerModel.SurvivalManualManager.SurvivalManualModels;
		if (survivalManualModels == null || survivalManualModels.Count <= 0)
		{
			return;
		}
		foreach (SurvivalManualModel item in survivalManualModels)
		{
			if (item != null && (!item.SurvivalManualDefinition.HasDateLimit || item.Timer > 0))
			{
				GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
				NGUITools.SetActive(gameObject, state: true);
				if (gameObject.TryGetComponent<SurvivalManualStoryItem>(out var component))
				{
					component.Setup(item.ID);
				}
				Entries.Add(gameObject);
			}
		}
	}

	public void OnclickTips()
	{
		SurvivalManualHelpPopup survivalManualHelpPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualHelpPopup) as SurvivalManualHelpPopup;
		if (survivalManualHelpPopup != null)
		{
			survivalManualHelpPopup.Open(SurvivalManualHelpPopup.HelpType.StoriesHelp);
		}
	}

	public void OnClickStoryDetails(int storyId)
	{
		SurvivalManualStoryChapterPopup survivalManualStoryChapterPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualStoryChapterPopup) as SurvivalManualStoryChapterPopup;
		if (survivalManualStoryChapterPopup != null)
		{
			survivalManualStoryChapterPopup.Open();
			survivalManualStoryChapterPopup.InitSelect(storyId);
		}
	}
}
