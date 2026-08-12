using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyUseClass : DailyQuest
	{
		[JsonIgnore]
		private SurvivorClass survivorClass = SurvivorClass.None;

		[JsonIgnore]
		private int targetCount = -1;

		public int InitialValue { get; set; }

		[JsonIgnore]
		public override bool IsValidForBonusStars
		{
			get
			{
				CombatModel combat = Player.Combat;
				if (combat != null)
				{
					for (int i = 0; i < combat.MissionRoster.Count; i++)
					{
						if (combat.MissionRoster[i].SurvivorClass != SurvivorClass)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public SurvivorClass SurvivorClass
		{
			get
			{
				if (survivorClass == SurvivorClass.None)
				{
					survivorClass = (SurvivorClass)Enum.Parse(typeof(SurvivorClass), base.AchievementDefinition.Params, ignoreCase: true);
				}
				return survivorClass;
			}
		}

		[JsonIgnore]
		public override bool CanComplete
		{
			get
			{
				if (Player.SurvivorContainer.IsSurvivorClassUnlocked(SurvivorClass))
				{
					List<SurvivorModel> survivorsOfClass = Player.SurvivorContainer.GetSurvivorsOfClass(SurvivorClass);
					if (survivorsOfClass != null)
					{
						return survivorsOfClass.Count >= 3;
					}
					return false;
				}
				return false;
			}
		}

		[JsonIgnore]
		public int TargetCount
		{
			get
			{
				if (targetCount == -1)
				{
					int.TryParse(base.AchievementDefinition.ExtParams, out targetCount);
				}
				return targetCount;
			}
		}

		[JsonIgnore]
		protected override bool InternalIsCompleted => GetProgressStep() >= TargetCount;

		protected override bool Init()
		{
			if (SurvivorClass != SurvivorClass.None && TargetCount >= 0)
			{
				InitialValue = Player.Blackboard.GetCounter(BlackboardModel.GetSameClassMissionCompleteKey(SurvivorClass));
				return true;
			}
			return false;
		}

		public override int GetProgressStep()
		{
			return Player.Blackboard.GetCounter(BlackboardModel.GetSameClassMissionCompleteKey(SurvivorClass)) - InitialValue;
		}

		public override int GetProgressTarget()
		{
			return TargetCount;
		}
	}
}
