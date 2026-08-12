using System;
using Newtonsoft.Json;

namespace TWDModel
{
	public class Achievement
	{
		public string AchievementDefinitionID;

		[NonSerialized]
		[JsonIgnore]
		public PlayerModel Player;

		public AchievementViewState ViewState { get; set; }

		[JsonIgnore]
		public GameEconomyData GED => Player.manager.GameEconomyData;

		[JsonIgnore]
		public BlackboardModel Blackboard => Player.Blackboard;

		[JsonIgnore]
		public AchievementDefinition AchievementDefinition => GED.GetAchievementDefinition(AchievementDefinitionID);

		[JsonIgnore]
		public int ChallengeBonusStars => AchievementDefinition.BonusStars;

		[JsonIgnore]
		public bool Valid { get; protected set; }

		[JsonIgnore]
		public virtual bool RewardClaimed => Blackboard.IsToggleOn(AchievementDefinition.BlackboardRewardClaimedKey);

		[JsonIgnore]
		public bool IsCompleted
		{
			get
			{
				if (Valid)
				{
					return InternalIsCompleted;
				}
				return false;
			}
		}

		[JsonIgnore]
		protected virtual bool InternalIsCompleted => false;

		public Rewards GetRewards()
		{
			return new Rewards(AchievementDefinition.Reward);
		}

		public void Initialize()
		{
			Valid = Init();
		}

		protected virtual bool Init()
		{
			return false;
		}

		public int GetProgress()
		{
			int progressTarget = GetProgressTarget();
			int progressStep = GetProgressStep();
			if (progressTarget <= 0)
			{
				return 0;
			}
			return UtilsMath.Clamp(progressStep * 100 / progressTarget, 0, 100);
		}

		public virtual int GetProgressStep()
		{
			return 0;
		}

		public virtual int GetProgressTarget()
		{
			return 0;
		}
	}
}
