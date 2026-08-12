using Newtonsoft.Json;

namespace TWDModel
{
	public class GvGSeasonModelPlayer : TWDModelObject
	{
		public const string GvGSeasonStarted = "GvGSeasonStarted";

		[JsonIgnore]
		public bool HasSeenClaimSeasonRewardsPopup;

		public GuildWarModelPlayer GuildWarModelPlayer { get; set; }

		public int StartedGvGSeasonId { get; set; }

		[JsonIgnore]
		public int LastStartedSeasonId
		{
			get
			{
				if (base.gameEconomyData == null)
				{
					base.Debug.LogError("GameEconomyData is null");
					return -1;
				}
				if (base.manager == null)
				{
					base.Debug.LogError("ManagerModel is null");
					return -1;
				}
				if (base.manager.Player == null)
				{
					base.Debug.LogError("PlayerModel is null");
					return -1;
				}
				return base.gameEconomyData.FindLastStartedSeason(base.manager.Player.UtcTimeStamp)?.Identifier ?? (-1);
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			StartedGvGSeasonId = -1;
			GuildWarModelPlayer = new GuildWarModelPlayer();
			GuildWarModelPlayer.SetManager(base.manager);
			GuildWarModelPlayer.Initialize();
		}

		public void StartSeason(int gvGSeasonId)
		{
			ResetSeason();
			StartedGvGSeasonId = gvGSeasonId;
			if (base.manager == null)
			{
				return;
			}
			if (base.manager.Player == null)
			{
				base.Debug.LogError("PlayerModel is null");
				return;
			}
			if (base.manager.Player.Blackboard == null)
			{
				base.Debug.LogError("Blackboard in PlayerModel is null");
				return;
			}
			base.manager.Player.Blackboard.ClearToggle("HasSeenSeasonStart");
			base.manager.Player.Blackboard.ClearToggle("HasSeenSeasonEnd");
			if (base.manager.Player.GuildShopModel == null)
			{
				base.Debug.LogError("GuildShopModel in PlayerModel is null");
				return;
			}
			base.manager.Player.GuildShopModel.StartForNewSeason();
			NotifyChange("GvGSeasonStarted");
		}

		public bool IsCurrentSeasonEnded()
		{
			if (base.gameEconomyData == null)
			{
				base.Debug.LogError("GameEconomyData is null");
				return false;
			}
			GvGSeasonDefinition gvGSeasonDefinition = base.gameEconomyData.FindGvGSeasonDefinition(StartedGvGSeasonId);
			if (gvGSeasonDefinition == null)
			{
				return false;
			}
			if (base.manager == null)
			{
				return false;
			}
			if (base.manager.Player == null)
			{
				base.Debug.LogError("PlayerModel is null");
				return false;
			}
			return !gvGSeasonDefinition.IsOpen(base.manager.Player.UtcTimeStamp);
		}

		public bool HasGvGSeasonStarted()
		{
			if (base.manager == null)
			{
				return false;
			}
			if (base.manager.Player == null)
			{
				base.Debug.LogError("PlayerModel is null");
				return false;
			}
			if (base.manager.Player.GuildWarModel == null)
			{
				base.Debug.LogError("GuildWarModel in PlayerModel is null");
				return false;
			}
			if (base.manager.Player.GvGSeasonModel == null)
			{
				base.Debug.LogError("GvGSeasonModel in PlayerModel is null");
				return false;
			}
			if (base.manager.Player.IsGuildMember && base.manager.Player.GvGSeasonModel.IsCurrentSeasonOpen(base.manager.Player.UtcTimeStamp))
			{
				return base.manager.Player.GvGSeasonModel.SeasonDefinitionId == StartedGvGSeasonId;
			}
			return false;
		}

		private void ResetSeason()
		{
			if (StartedGvGSeasonId == -1)
			{
				return;
			}
			if (base.manager == null)
			{
				base.Debug.LogError("Manager is null");
				return;
			}
			if (base.manager.Player == null)
			{
				base.Debug.LogError("PlayerModel is null");
				return;
			}
			CurrencyModel currency = base.manager.Player.GetCurrency(CurrencyType.GuildBattleRP);
			if (currency == null)
			{
				base.Debug.LogWarning("Reward points is null");
				return;
			}
			if (base.manager.Metrics == null)
			{
				base.Debug.LogError("Metrics is null");
				return;
			}
			base.manager.Metrics.PushResource(CurrencyType.GuildBattleRP, -currency.Value);
			base.manager.Metrics.AddSpend().AddResources().AddEnd()
				.AddGvG()
				.AddGvGSeason()
				.Send();
			currency.SetValue(0);
			CurrencyModel currency2 = base.manager.Player.GetCurrency(CurrencyType.BattlePass);
			if (currency2 == null)
			{
				base.Debug.LogWarning("Battle passes is null");
				return;
			}
			currency2.SetValue(0);
			if (GuildWarModelPlayer == null)
			{
				base.Debug.LogWarning("GuildWarModelPlayer is null");
			}
			else
			{
				GuildWarModelPlayer.SeasonReset();
			}
		}
	}
}
