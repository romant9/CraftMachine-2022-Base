using TWDModel;
using UnityEngine;

public class UIGuildBattleParticipants : MonoBehaviour
{
	[Header("Optional. Auto set at Awake()")]
	[SerializeField]
	private UILabel battleParticipantsLabel;

	private void Awake()
	{
		if (battleParticipantsLabel == null)
		{
			battleParticipantsLabel = GetComponent<UILabel>();
		}
	}

	private void OnEnable()
	{
		UpdateUI();
		AddListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void AddListeners()
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnModelChange;
			guildWarModel.Changed += OnModelChange;
		}
	}

	private void RemoveListeners()
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnModelChange;
		}
	}

	private void OnModelChange(TWDGroupModelChild model, string changed, object args)
	{
		if (changed == "GuildBattlePlayerRegistered" || changed == "GuildBattlePlayerResigned")
		{
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		int registeredPlayersCountForBattleTimeSlot = GuildWarHelper.GetRegisteredPlayersCountForBattleTimeSlot();
		int maxPlayerCountInBattle = gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
		bool flag = registeredPlayersCountForBattleTimeSlot >= gameEconomyData.GuildWarConfig.MinPlayersToStartBattle;
		battleParticipantsLabel.color = (flag ? SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.ValidColor : SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NotValidColor);
		HelpersUI.SetContentToLabel(battleParticipantsLabel, $"{registeredPlayersCountForBattleTimeSlot}/{maxPlayerCountInBattle}");
	}
}
