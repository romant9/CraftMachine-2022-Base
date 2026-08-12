namespace TWDModel
{
	public class RewardRemoldSkill : IReward
	{
		public string SpRemoldSkillType { get; set; }

		public int Amount { get; set; }

		public ModSkillRewardResult GivenRewardResult { get; private set; }

		public RewardType Type => RewardType.RemoldSkill;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			return GivenRewardResult = manager.Player.ModSkillManager.AddRemoldSkill(SpRemoldSkillType, Amount);
		}


		#region mycode
		public void Remove(ModSkillManager manager)
		{
			var skill = GivenRewardResult.ModSkillMode;
			manager.ResetModSkill(skill);
			manager.ModSkillModes.Remove(skill);
			GivenRewardResult = null;
		}
		#endregion
	}
}
