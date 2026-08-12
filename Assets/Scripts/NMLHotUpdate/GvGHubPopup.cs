using System.Collections.Generic;
using System.Linq;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class GvGHubPopup : HUDElement
{
	[SerializeField]
	private UIButtonExtended warPlannerButton;

	[SerializeField]
	private GvGCalendarGrid gvgCalendarGrid;

	public override void Open()
	{
		base.Open();
		Init();
	}

	public override void Close()
	{
		base.Close();
		warPlannerButton.RemoveClickCallback(OpenWarPlanner);
	}

	private void OpenWarPlanner(UIButtonExtended button)
	{
		GvGCalendarPopup gvGCalendarPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GvGCalendarPopup) as GvGCalendarPopup;
		if (gvGCalendarPopup != null)
		{
			gvGCalendarPopup.Open();
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;

		var back = GetComponentsInChildren<UITexture>(true).FirstOrDefault(x => x.name == "Background");
		if (back)
		{
			DebugTWD.Log("Fix Background");
			var rext = back.uvRect;
			rext.Set(0f, .02f, 1f, .49f);
			back.uvRect = rext;
			back.applyGradient = false;
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (!(type == "OnPopUpClose") || !(parameter is HUDElement hUDElement))
		{
			return;
		}
		if (hUDElement.UIType == UIType.GvGBetaInfoPopup)
		{
			if (!ShowSeasonStartPopup())
			{
				ShowWhatsNewPopUp();
			}
		}
		else if (hUDElement.UIType == UIType.GvGSeasonResetPopup)
		{
			ShowWhatsNewPopUp();
		}
	}

	private void Init()
	{
		warPlannerButton.SetClickCallback(OpenWarPlanner);
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			for (int i = 0; i < guildWarModel.RegisteredPlayersForBattleSlot.Count; i++)
			{
				int i2 = i;
				gvgCalendarGrid.SetButtonsClickCallback(i, delegate
				{
					CalendarGridButtonsCallback(i2);
				});
			}
		}
		if (!ShowGvGBetaInfoPopup() && !ShowSeasonStartPopup())
		{
			ShowWhatsNewPopUp();
		}
	}

	private static bool ShowGvGBetaInfoPopup()
	{
		return BlackboardUISeenToggle.TryToOpen(UIType.GvGBetaInfoPopup, "HasSeenGvGBetaNotice", new UIType[1] { UIType.GuildBattleEndPopup });
	}

	private static bool ShowSeasonStartPopup()
	{
		int currentSeasonDefinitionId = GuildWarHelper.GetCurrentSeasonDefinitionId();
		if (currentSeasonDefinitionId == -1)
		{
			return false;
		}
		return BlackboardUISeenToggle.TryToOpen(UIType.GvGSeasonResetPopup, "HasSeenGvGSeasonReset" + currentSeasonDefinitionId, new UIType[1] { UIType.GuildBattleEndPopup });
	}

	private static void ShowWhatsNewPopUp()
	{
		BlackboardUISeenToggle.TryToOpen(UIType.NewInGuildWarsPopup, "HasSeenWhatsNewInGuildWars", new UIType[1] { UIType.GuildBattleEndPopup });
	}

	private void CalendarGridButtonsCallback(int index)
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel == null)
		{
			return;
		}
		List<long> list = guildWarModel.RegisteredPlayersForBattleSlot.Keys.OrderBy((long x) => x).ToList();
		if (list != null && list.Count > index)
		{
			long num = list[index];
			if (guildWarModel.GuildBattleResults.ContainsKey(num))
			{
				ShowBattleResult(guildWarModel.GuildBattleResults[num]);
			}
			else
			{
				GuildWarDateSelected(num);
			}
		}
	}

	private void GuildWarDateSelected(long timeSlot)
	{
		GvGCalendarPopup gvGCalendarPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GvGCalendarPopup) as GvGCalendarPopup;
		if (gvGCalendarPopup != null)
		{
			gvGCalendarPopup.OpenWithStateData(timeSlot);
		}
	}

	private void ShowBattleResult(GuildBattleResultInfo guildBattleResult)
	{
		GuildBattleResultPopup guildBattleResultPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleResultPopup) as GuildBattleResultPopup;
		if (guildBattleResultPopup != null)
		{
			guildBattleResultPopup.OpenWithStateData(guildBattleResult);
		}
	}

	public override void OnBackButtonClicked()
	{
		if (DeepLinkNavigation.HandleDeepLink("MISSION_HUB"))
		{
			HUDManager.TryClosePopup(UIType.GuildBattleMapPopup);
		}
	}
}
