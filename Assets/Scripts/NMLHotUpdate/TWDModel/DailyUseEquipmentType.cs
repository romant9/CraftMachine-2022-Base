using System;
using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyUseEquipmentType : DailyQuest
	{
		[JsonIgnore]
		private EquipmentCategory equipmentCategory = EquipmentCategory.None;

		[JsonIgnore]
		private int targetCount = -1;

		public int InitialValue { get; set; }

		[JsonIgnore]
		public EquipmentCategory EquipmentCategory
		{
			get
			{
				if (equipmentCategory == EquipmentCategory.None)
				{
					equipmentCategory = (EquipmentCategory)Enum.Parse(typeof(EquipmentCategory), base.AchievementDefinition.Params, ignoreCase: true);
				}
				return equipmentCategory;
			}
		}

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
						SurvivorModel survivorModel = combat.MissionRoster[i];
						if (survivorModel == null)
						{
							return false;
						}
						if (survivorModel.GetWeaponEquipment() == null)
						{
							if (EquipmentCategory == EquipmentCategory.MeleeWeapon && !survivorModel.IsMeleeClass)
							{
								return false;
							}
							if (EquipmentCategory == EquipmentCategory.RangeWeapon && !survivorModel.IsRangedClass)
							{
								return false;
							}
						}
						else if (survivorModel.GetWeaponEquipment().Definition.Category != EquipmentCategory)
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
		public override bool CanComplete
		{
			get
			{
				int num = 0;
				for (int i = 0; i < Player.SurvivorContainer.Survivors.Count; i++)
				{
					if (Player.SurvivorContainer.Survivors[i].GetWeaponEquipment().Definition.Category == EquipmentCategory)
					{
						num++;
					}
				}
				return num >= 3;
			}
		}

		[JsonIgnore]
		protected override bool InternalIsCompleted => GetProgressStep() >= TargetCount;

		protected override bool Init()
		{
			if (EquipmentCategory != EquipmentCategory.None && TargetCount >= 0)
			{
				InitialValue = Player.Blackboard.GetCounter(BlackboardModel.GetSameEquipmentTypeMissionCompleteKey(EquipmentCategory));
				return true;
			}
			return false;
		}

		public override int GetProgressStep()
		{
			return Player.Blackboard.GetCounter(BlackboardModel.GetSameEquipmentTypeMissionCompleteKey(EquipmentCategory)) - InitialValue;
		}

		public override int GetProgressTarget()
		{
			return TargetCount;
		}
	}
}
