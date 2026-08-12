using System.Linq;
using BaseModel;
using Newtonsoft.Json;
using TWDModel;

public class DailyLoginCampaignModel : TWDModelObject
{
	[JsonIgnore]
	public long RefreshRate;

	[JsonIgnore]
	public int MaxPlayerCouncilLevel;

	[JsonProperty]
	public int ActiveDay { get; private set; }

	[JsonProperty]
	public ModelList<DailyLoginCampaignRewardModelItem> Rewards { get; private set; }

	[JsonProperty]
	public long NextRewardTime { get; private set; }

	[JsonProperty]
	public bool IsCompleted { get; private set; }

	[JsonProperty]
	private long CreationDate { get; set; }

	[JsonIgnore]
	public bool IsInitialized { get; private set; }

	public override bool IsValid()
	{
		return true;
	}

	public void InitializeCampaign()
	{
		if (base.gameEconomyData.GetFeature("DailyLoginCalendar").Enabled && !IsInitialized && !IsCompleted && (CreationDate != 0L || base.manager.Player.CouncilLevel <= MaxPlayerCouncilLevel))
		{
			if (CreationDate == 0L)
			{
				CreationDate = base.manager.Player.UtcTimeStamp;
			}
			CreateRewardCollection();
			IsInitialized = true;
		}
	}

	public override void Start()
	{
		base.Start();
		MaxPlayerCouncilLevel = base.manager.GameEconomyData.ConfigData.DailyLoginCalendarMaxCouncilLevel;
		RefreshRate = base.manager.GameEconomyData.ConfigData.DailyLoginCalendarRefreshRate;
		base.Changed += base.manager.Player.SevenDayLoginManager.OnDailyLoginCampaignCompleted;
		if (!base.gameEconomyData.GetFeature("DailyLoginCalendar").Enabled || IsCompleted || (CreationDate == 0L && base.manager.Player.CouncilLevel > MaxPlayerCouncilLevel))
		{
			NotifyChange("DailyLoginCampaignCompleted");
			base.Changed -= base.manager.Player.SevenDayLoginManager.OnDailyLoginCampaignCompleted;
		}
	}

	private void CreateRewardCollection()
	{
		if (Rewards != null)
		{
			for (int i = 0; i < Rewards.Count; i++)
			{
				DailyLoginRewardsDefinition definition = base.gameEconomyData.DailyLoginRewardsDefinitions[i];
				DailyLoginCampaignRewardModelItem dailyLoginCampaignRewardModelItem = Rewards[i];
				dailyLoginCampaignRewardModelItem.SetManager(base.manager);
				dailyLoginCampaignRewardModelItem.Initialize();
				dailyLoginCampaignRewardModelItem.Start();
				dailyLoginCampaignRewardModelItem.GenerateRewards(definition);
			}
			return;
		}
		Rewards = new ModelList<DailyLoginCampaignRewardModelItem>();
		for (int j = 0; j < base.gameEconomyData.DailyLoginRewardsDefinitions?.Length; j++)
		{
			DailyLoginRewardsDefinition definition2 = base.gameEconomyData.DailyLoginRewardsDefinitions[j];
			DailyLoginCampaignRewardModelItem dailyLoginCampaignRewardModelItem2 = new DailyLoginCampaignRewardModelItem();
			dailyLoginCampaignRewardModelItem2.SetManager(base.manager);
			dailyLoginCampaignRewardModelItem2.Initialize();
			dailyLoginCampaignRewardModelItem2.Start();
			dailyLoginCampaignRewardModelItem2.GenerateRewards(definition2);
			Rewards.Add(dailyLoginCampaignRewardModelItem2);
		}
		NotifyChange("");
	}

	public override void Tick(long deltaTime)
	{
		base.Tick(deltaTime);
		if (IsInitialized && !IsCompleted)
		{
			UpdateActiveDay();
		}
	}

	private void UpdateActiveDay()
	{
		if (base.manager.Player.UtcTimeStamp > NextRewardTime && Rewards[ActiveDay].Claimed)
		{
			ActiveDay++;
			NotifyChange("");
		}
	}

	public bool TryClaimReward(int modelId)
	{
		DailyLoginCampaignRewardModelItem dailyLoginCampaignRewardModelItem = Rewards.First((DailyLoginCampaignRewardModelItem reward) => reward.ModelId == modelId);
		if (dailyLoginCampaignRewardModelItem == null)
		{
			base.Debug.LogError($"Trying to claim reward '{modelId}' not in the reward list.");
			return false;
		}
		if (dailyLoginCampaignRewardModelItem.Claimed)
		{
			base.Debug.LogError($"Trying to claim reward '{modelId}' already claimed.");
			return true;
		}
		if (!dailyLoginCampaignRewardModelItem.ClaimReward())
		{
			base.Debug.LogError($"Failed claiming reward '{modelId}'.");
			return false;
		}
		ModelList<DailyLoginCampaignRewardModelItem> rewards = Rewards;
		if (rewards != null && rewards.Count == 7 && Rewards[6].Claimed)
		{
			IsCompleted = true;
			NotifyChange("DailyLoginCampaignCompleted");
			base.Changed -= base.manager.Player.SevenDayLoginManager.OnDailyLoginCampaignCompleted;
		}
		NextRewardTime = base.manager.Player.UtcTimeStamp + RefreshRate;
		return true;
	}

	public bool CanClaimRewardForActiveDay()
	{
		ModelList<DailyLoginCampaignRewardModelItem> rewards = Rewards;
		bool? obj;
		if (rewards == null)
		{
			obj = null;
		}
		else
		{
			DailyLoginCampaignRewardModelItem dailyLoginCampaignRewardModelItem = rewards[ActiveDay];
			obj = ((dailyLoginCampaignRewardModelItem != null) ? new bool?(!dailyLoginCampaignRewardModelItem.Claimed) : ((bool?)null));
		}
		bool? flag = obj;
		return flag == true;
	}

	public long GetCreationDate()
	{
		return CreationDate;
	}

	public void DebugSetCompleted()
	{
		IsCompleted = true;
		NotifyChange("DailyLoginCampaignCompleted");
	}
}
