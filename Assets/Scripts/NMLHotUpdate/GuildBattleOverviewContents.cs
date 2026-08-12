using UnityEngine;

public class GuildBattleOverviewContents : UIToggleContent
{
	[Header("Set at Awake() if null")]
	public UIToggleMenu OverviewToggleMenu;

	[SerializeField]
	private GameObject warTimer;

	private void Awake()
	{
		if (OverviewToggleMenu == null)
		{
			OverviewToggleMenu = GetComponent<UIToggleMenu>();
		}
		Activate();
	}

	public override void Activate()
	{
		base.Activate();
		if (OverviewToggleMenu != null)
		{
			if (!GameManager.Instance.playerModel.IsGuildMember)
			{
				OverviewToggleMenu.OpenContentByIndex(0);
			}
			else if (GuildWarHelper.IsSeasonOngoing() && !GuildWarHelper.IsWarOngoing() && !GuildWarHelper.HasSeenGvGSeasonStart() && !GuildWarHelper.IsLockedByCouncilLevelOrTutorial())
			{
				OverviewToggleMenu.OpenContentByIndex(4);
				GuildWarHelper.SetHasSeenGvGSeasonStartFlagAndGiveSeasonStartRewards();
			}
			else if (GuildWarHelper.IsWarOngoing())
			{
				OverviewToggleMenu.OpenContentByIndex(1);
			}
			else if (!GuildWarHelper.IsSeasonOngoing())
			{
				OverviewToggleMenu.OpenContentByIndex(2);
			}
			else if (!GuildWarHelper.IsWarOngoing())
			{
				OverviewToggleMenu.OpenContentByIndex(3);
				Helpers.GameObjectSetActive(warTimer, GuildWarHelper.GetTimeLeftToNextWar() != 0);
			}
		}
	}
}
