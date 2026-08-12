using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class SurvivalManualRankPopup : HUDElement
{
	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject MyRankContainer;

	[SerializeField]
	private UILabel rankNum;

	[SerializeField]
	private PlayerEmblemIcon playerEmblem;

	[SerializeField]
	private UILabel playerName;

	[SerializeField]
	private UISprite[] medalIcons;

	[SerializeField]
	private UILabel level;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private List<ScoreDataEntry> datas = new List<ScoreDataEntry>();

	protected SurvivalManualRankDataProvider provider;

	protected LeaderboardPosition MyRankPosition;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
		provider = new SurvivalManualRankDataProvider();
		provider.OnDataReceived += OnDataReceived;
		provider.RequestData();
		var leaderboardName = Leaderboards.GetPlayerSurvivalManualLeaderboardName();
		SignalRClient.Instance.RequestCommand("GetLeaderboardPosition", leaderboardName, playerModel.HashedId, OnDataMyRank, null, waitForResponse: true);
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		provider.OnDataReceived -= OnDataReceived;
	}

	public override void Open()
	{
		base.Open();
		SetInitMyRank();
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
		UpdateRankList();
		UpdateMyRank();
	}

	private void UpdateRankList()
	{
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
		if (datas == null || datas.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < datas.Count; i++)
		{
			if (datas[i] != null && !string.IsNullOrEmpty(datas[i].Id))
			{
				GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
				NGUITools.SetActive(gameObject, state: true);
				if (gameObject.TryGetComponent<SurvivalManualRankItem>(out var component) && datas[i] is SurvivalManualScoreDataEntry)
				{
					component.Setup(i, (SurvivalManualScoreDataEntry)datas[i]);
					Entries.Add(gameObject);
				}
			}
		}
	}

	private void OnDataMyRank(string result)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(result))
		{
			Debug.LogError("OnDataMyRank failed");
			SignalRClient.Instance.ClearError();
			return;
		}
		LeaderboardPosition leaderboardPosition = GameManager.Instance.jsonSerializer.DeserializeObject<LeaderboardPosition>(result);
		if (leaderboardPosition != null)
		{
			leaderboardPosition.Position++;
			MyRankPosition = leaderboardPosition;
			UpdateMyRank();
		}
	}

	private void OnDataReceived(ScoreDataProvider scoreDataProvider, List<ScoreDataEntry> dataEntries)
	{
		if (dataEntries != null && dataEntries.Count > 0)
		{
			int num = 100;
			if (dataEntries.Count > num)
			{
				dataEntries.RemoveRange(num, dataEntries.Count - num - 1);
			}
			datas = dataEntries;
			UpdateRankList();
		}
	}

	private void UpdateMyRank()
	{
		Helpers.GameObjectSetActive(MyRankContainer, value: false);
		if (MyRankPosition == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(MyRankContainer, value: true);
		string text = "100+";
		if (MyRankPosition != null)
		{
			text = MyRankPosition.Position.ToString();
			rankNum.text = text + ".";
		}
		else
		{
			rankNum.text = text;
		}
		playerName.text = playerModel.Name;
		playerEmblem.SetEmblem(playerModel.PlayerEmblem);
		ModelList<SurvivalManualModel> survivalManualModels = playerModel.SurvivalManualManager.SurvivalManualModels;
		int num = 0;
		if (survivalManualModels != null && survivalManualModels.Count > 0)
		{
			int num2 = 0;
			while (num < 3 && num2 < survivalManualModels.Count)
			{
				if (survivalManualModels[num2].SurvivalManualEmblesState)
				{
					medalIcons[num].spriteName = survivalManualModels[num2].SurvivalManualDefinition.SouvenirMedalIcon;
					num++;
				}
				num2++;
			}
		}
		level.text = "Lv." + playerModel.SurvivalManualManager.GetSystemLV();
	}

	private void SetInitMyRank()
	{
		rankNum.text = "100+";
		playerEmblem.SetEmblem(playerModel.PlayerEmblem);
		playerName.text = playerModel.Name;
		level.text = "Lv." + playerModel.SurvivalManualManager.GetSystemLV();
	}
}
