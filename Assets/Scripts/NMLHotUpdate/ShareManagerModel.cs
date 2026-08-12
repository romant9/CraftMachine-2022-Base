using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TWDModel;

[Serializable]
public class ShareManagerModel : TWDModelObject
{
	[JsonIgnore]
	private PlayerModel PlayerModel => base.manager.Player;

	public Dictionary<ShareType, ShareModel> ObtainedRewards { get; set; }

	public override bool IsValid()
	{
		return true;
	}

	public override void Initialize()
	{
		base.Initialize();
		ObtainedRewards = new Dictionary<ShareType, ShareModel>();
	}

	public override void Start()
	{
		base.Start();
		if (ObtainedRewards == null)
		{
			ObtainedRewards = new Dictionary<ShareType, ShareModel>();
		}
	}

	public bool GiveShareReward(ShareType shareType)
	{
		if (ObtainedRewards.TryGetValue(shareType, out var value) && value.IsObtained)
		{
			base.Debug.LogError("Share to " + shareType.ToString() + " has been obtained");
			return false;
		}
		if (shareType != ShareType.Discord)
		{
			base.Debug.LogError("Share to " + shareType.ToString() + " type error");
			return false;
		}
		IReward reward = new Rewards(PlayerModel.gameEconomyData.ConfigData.ShareToDiscordReward)?.RewardsList[0];
		if (reward == null)
		{
			return false;
		}
		if (!(reward is RewardCurrency rewardCurrency))
		{
			base.Debug.LogError("Share to " + shareType.ToString() + " reward not recognized");
			return false;
		}
		LootEntry lootEntry = PlayerModel.LootManager.CreateCurrencyLoot(rewardCurrency.CurrencyType, rewardCurrency.Amount, DropType.None, DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency);
		if (lootEntry == null)
		{
			return false;
		}
		PlayerModel.LootManager.GiveLoot(lootEntry);
		PlayerModel.BundleManager.ShareRewardEntrys.Add(lootEntry);
		ObtainedRewards[shareType] = new ShareModel
		{
			IsObtained = true
		};
		base.manager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, rewardCurrency.Amount, rewardCurrency.Amount).Send();
		return true;
	}
}
