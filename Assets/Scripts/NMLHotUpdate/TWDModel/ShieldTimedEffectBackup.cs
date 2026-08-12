namespace TWDModel
{
	public class ShieldTimedEffectBackup : TimeEffectBackup
	{
		public int Shield { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(ShieldTimedEffect shieldTimedEffect)
		{
			RecordStatus((TimedEffect)shieldTimedEffect);
			Shield = shieldTimedEffect.Shield;
		}

		public override void BackUp()
		{
			base.BackUp();
			(base.Model as ShieldTimedEffect).Shield = Shield;
		}
	}
}
