namespace TWDModel
{
	public class ModSkillRewardResult
	{
		public ModSkillRewardType RewardType { get; private set; }

		public ModSkillMode ModSkillMode { get; private set; }

		public Rewards DuplicateRewards { get; private set; }

		public bool IsNewAcquisition => RewardType == ModSkillRewardType.NewAcquisition;

		public bool IsDuplicate => RewardType == ModSkillRewardType.Duplicate;

		public static ModSkillRewardResult NoneResult()
		{
			return new ModSkillRewardResult
			{
				RewardType = ModSkillRewardType.None
			};
		}

		public static ModSkillRewardResult NewAcquisitionResult(ModSkillMode modSkillMode)
		{
			return new ModSkillRewardResult
			{
				RewardType = ModSkillRewardType.NewAcquisition,
				ModSkillMode = modSkillMode
			};
		}

		public static ModSkillRewardResult DuplicateResult(Rewards duplicateRewards)
		{
			return new ModSkillRewardResult
			{
				RewardType = ModSkillRewardType.Duplicate,
				DuplicateRewards = duplicateRewards
			};
		}
	}
}
