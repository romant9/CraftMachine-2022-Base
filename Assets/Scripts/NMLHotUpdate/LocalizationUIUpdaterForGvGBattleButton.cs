using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class LocalizationUIUpdaterForGvGBattleButton : LocalizationUIUpdater
{
	[SerializeField]
	private string spectate;

	public override string localizationKey
	{
		get
		{
			if (!GuildWarHelper.IsBattleOnGoing())
			{
				return base.localizationKey;
			}
			if (GuildWarHelper.IsBattleOngoingAndPlayerRegistered())
			{
				return base.localizationKey;
			}
			return spectate;
		}
	}
}
