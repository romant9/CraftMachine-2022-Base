using TWDModel;
using UnityEngine;

public class SetStateEqualsToGvGBattleOnGoing : MonoBehaviour
{
	[SerializeField]
	private UIButton[] buttons;

	private GuildWarModel guildWarModel;

	private void Awake()
	{
		guildWarModel = GuildWarHelper.GetGuildWarModel();
		Check();
	}

	private void OnEnable()
	{
		if (guildWarModel != null)
		{
			guildWarModel.Changed += GuildWarModelChangedEventHandler;
		}
	}

	private void OnDisable()
	{
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= GuildWarModelChangedEventHandler;
		}
	}

	private void GuildWarModelChangedEventHandler(TWDGroupModelChild model, string changed, object args)
	{
		if (changed == "GuildBattleStarted" || changed == "GuildBattleEnded")
		{
			Check();
		}
	}

	private void Check()
	{
		bool flag = GuildWarHelper.IsBattleOnGoing();
		UIButton[] array = buttons;
		foreach (UIButton uIButton in array)
		{
			if (flag)
			{
				if (uIButton.state == UIButtonColor.State.Disabled)
				{
					UpdateState(uIButton, isBattleOngoing: true);
				}
			}
			else if (uIButton.state != UIButtonColor.State.Disabled)
			{
				UpdateState(uIButton, isBattleOngoing: false);
			}
		}
	}

	private void UpdateState(UIButton button, bool isBattleOngoing)
	{
		HelpersUI.SetButtonState(button, (!isBattleOngoing) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
	}
}
