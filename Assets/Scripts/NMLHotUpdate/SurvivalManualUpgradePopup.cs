using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivalManualUpgradePopup : HUDElement
{
	[SerializeField]
	private UILabel StoryName;

	[SerializeField]
	private UILabel OldLevel;

	[SerializeField]
	private UILabel OldAttack;

	[SerializeField]
	private UILabel OldHP;

	[SerializeField]
	private UILabel NewLevel;

	[SerializeField]
	private UILabel NewAttack;

	[SerializeField]
	private UILabel NewHP;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject EntryContainer;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private int storyId = -1;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private SurvivalManualModel storyModel => playerModel.SurvivalManualManager.GetSurvivalManualModel(storyId);

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public void Open(int storyId)
	{
		base.Open();
		this.storyId = storyId;
		ClearBTLevelEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = EntryContainer.GetComponentInParent<UIScrollView>();
		FreshListData();
		component.Reposition();
		componentInParent.ResetPosition();
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SurvivalManualStoryUpgradeSelecteddEvent")
		{
			UpdateUI();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		StoryName.text = LocalizationManager.GetText(storyModel.SurvivalManualDefinition.StoryQueueName);
		OldLevel.text = LocalizationManager.GetText("SurvivalManual_SystemNotice_Lv") + storyModel.GetTotalActorsAllLevel();
		OldAttack.text = "+" + storyModel.GetSurvivalManualAttack().ToString();
		OldHP.text = "+" + storyModel.GetSurvivalManualHp().ToString();
		NewLevel.text = LocalizationManager.GetText("SurvivalManual_SystemNotice_Lv") + GetSumLevel();
		NewAttack.text = "+" + GetSumAttack();
		NewHP.text = "+" + GetSumHP();
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
		List<string> actorList = storyModel.SurvivalManualDefinition.ActorList;
		if (actorList == null || actorList.Count <= 0)
		{
			return;
		}
		foreach (string item in actorList)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<SurvivalManualActorUpItem>(out var component))
			{
				component.Setup(storyModel, item);
			}
			Entries.Add(gameObject);
		}
	}

	public void OnclickUpgrade()
	{
		if (Entries == null || Entries.Count <= 0)
		{
			return;
		}
		List<string> list = new List<string>();
		for (int i = 0; i < Entries.Count; i++)
		{
			SurvivalManualActorUpItem component = Entries[i].GetComponent<SurvivalManualActorUpItem>();
			if (component != null && component.GetSelectedState())
			{
				list.Add(component.GetSelectedStoryActorID());
			}
		}
		if (list != null && list.Count > 0)
		{
			if (Helpers.ExecuteCommand(new SurvivalManualActorUpgradeCommand(storyModel.ModelId, list, SurvivalManualActorUpgradeCommand.UpgradeType.OneClickUpgrade)) == TWDModelResult.OK)
			{
				HUDNotification.Info(LocalizationManager.GetText("Achievement.CooperationLvup.Title"));
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
			}
			UpdateUI();
			UIEvent.Send("SurvivalManualStoryHeroUpgrade");
		}
	}

	private int GetSumLevel()
	{
		if (Entries == null || Entries.Count <= 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < Entries.Count; i++)
		{
			SurvivalManualActorUpItem component = Entries[i].GetComponent<SurvivalManualActorUpItem>();
			num += component.GetSelectedLevel();
		}
		return num;
	}

	private int GetSumAttack()
	{
		if (Entries == null || Entries.Count <= 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < Entries.Count; i++)
		{
			SurvivalManualActorUpItem component = Entries[i].GetComponent<SurvivalManualActorUpItem>();
			num += component.GetSelectedAttack();
		}
		return num;
	}

	private int GetSumHP()
	{
		if (Entries == null || Entries.Count <= 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < Entries.Count; i++)
		{
			SurvivalManualActorUpItem component = Entries[i].GetComponent<SurvivalManualActorUpItem>();
			num += component.GetSelectedHP();
		}
		return num;
	}
}
