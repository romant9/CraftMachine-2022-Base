using Newtonsoft.Json;

namespace TWDModel
{
	public class FindEquipmentAchievement : Achievement
	{
		public int TargetLevel;

		[JsonIgnore]
		private bool IsCompletedValue;

		protected override bool InternalIsCompleted
		{
			get
			{
				if (!IsCompletedValue)
				{
					IsCompletedValue = Player.Equipment != null && Player.Equipment.GetHighestEquipmentRarity() >= TargetLevel;
				}
				return IsCompletedValue;
			}
		}

		protected override bool Init()
		{
			TargetLevel = 0;
			if (!int.TryParse(base.AchievementDefinition.Params, out TargetLevel))
			{
				return false;
			}
			return true;
		}

		public override int GetProgressStep()
		{
			if (!InternalIsCompleted)
			{
				return 0;
			}
			return 1;
		}

		public override int GetProgressTarget()
		{
			return 1;
		}
	}
}
