using BaseModel;

namespace TWDModel
{
	public class WorldBossInstantReturnCommand : TWDWorldBossInternalCommand
	{
		public string CapturePoint { get; set; }

		public string ReturningTeamId { get; set; }

		public int GoldCost { get; set; }

		public WorldBossInstantReturnCommand()
		{
		}

		public WorldBossInstantReturnCommand(int seasonId, int cycleId, string capturePoint, string returningTeamId, int goldCost)
			: base(seasonId, cycleId)
		{
			CapturePoint = capturePoint;
			ReturningTeamId = returningTeamId;
			GoldCost = goldCost;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			if (WorldBossGuildFullSnapshot == null)
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: Guild has no WorldBossGuildMatchSnapshot");
				return TWDModelResult.Error;
			}
			if (string.IsNullOrEmpty(CapturePoint))
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: CapturePoint is empty");
				return TWDModelResult.Error;
			}
			if (string.IsNullOrEmpty(ReturningTeamId))
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: ReturningTeamId is empty");
				return TWDModelResult.Error;
			}
			if ((WorldBossGuildFullSnapshot.Match?.CycleId ?? 0) > 0 && (WorldBossGuildFullSnapshot.Match?.CycleId ?? 0) != base.CycleId)
			{
				manager.Debug.LogError($"WorldBossInstantReturnCommand: CycleId mismatch. Command={base.CycleId}, Model={WorldBossGuildFullSnapshot.Match?.CycleId ?? 0}");
				return TWDModelResult.Error;
			}
			if ((WorldBossGuildFullSnapshot.Match?.SeasonId ?? 0) > 0 && (WorldBossGuildFullSnapshot.Match?.SeasonId ?? 0) != base.SeasonId)
			{
				manager.Debug.LogError($"WorldBossInstantReturnCommand: SeasonId mismatch. Command={base.SeasonId}, Model={WorldBossGuildFullSnapshot.Match?.SeasonId ?? 0}");
				return TWDModelResult.Error;
			}
			WorldBossSeasonDefinition worldBossSeasonDefinition = manager.GameEconomyData.FindWorldBossSeasonDefinition(base.SeasonId);
			if (worldBossSeasonDefinition == null || !worldBossSeasonDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: Season not found or not open: " + base.SeasonId);
				return TWDModelResult.Error;
			}
			WorldBossCycleDefinition worldBossCycleDefinition = manager.GameEconomyData.FindWorldBossCycleDefinition(base.SeasonId, base.CycleId);
			if (worldBossCycleDefinition == null || !worldBossCycleDefinition.IsOpen(manager.Player.UtcTimeStamp))
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: Cycle not found or combat window not open: " + base.CycleId);
				return TWDModelResult.Error;
			}
			WorldBossModelManager worldBossModelManager = manager.Player.WorldBossModelManager;
			WorldBossReturningTeamModel worldBossReturningTeamModel = worldBossModelManager?.FindMyReturningTeam(CapturePoint, ReturningTeamId);
			if (worldBossReturningTeamModel == null)
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: returning team not found. CapturePoint=" + CapturePoint + ", Id=" + ReturningTeamId);
				return TWDModelResult.Error;
			}
			if (worldBossModelManager.GetReturningTeamRemainingMs(worldBossReturningTeamModel) <= 0)
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: returning team already finished. CapturePoint=" + CapturePoint + ", Id=" + ReturningTeamId);
				return TWDModelResult.Error;
			}
			if (GoldCost <= 0)
			{
				manager.Debug.LogError($"WorldBossInstantReturnCommand: invalid GoldCost from client. CapturePoint={CapturePoint}, Id={ReturningTeamId}, GoldCost={GoldCost}");
				return TWDModelResult.Error;
			}
			if (manager.Player.GetCurrency(CurrencyType.Diamonds).Value < GoldCost)
			{
				manager.Debug.LogError($"WorldBossInstantReturnCommand: not enough Diamonds. Need={GoldCost}, Have={manager.Player.GetCurrency(CurrencyType.Diamonds).Value}");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			IModelCommandRespond modelCommandRespond = base.Execute(modelManager);
			if (modelCommandRespond != null && modelCommandRespond.Code == 0 && modelManager is TWDModelManager tWDModelManager)
			{
				int goldCost = GoldCost;
				Cashier cashier = new Cashier(tWDModelManager);
				CashierItem cashierItem = new CashierItem(PurchaseType.WorldBossInstantReturn);
				cashierItem.SetCost(CurrencyType.Diamonds, goldCost);
				cashier.AddItem(cashierItem);
				if (cashier.Pay() != TWDModelResult.OK)
				{
					tWDModelManager.Debug.LogError($"WorldBossInstantReturnCommand: Cashier payment failed after team cleared (free return). Cost={goldCost}");
				}
			}
			return modelCommandRespond;
		}

		protected override TWDModelResult ExecuteOnServer(TWDModelManager manager)
		{
			WorldBossModelManager worldBossModelManager = manager.Player.WorldBossModelManager;
			WorldBossReturningTeamModel worldBossReturningTeamModel = worldBossModelManager?.FindMyReturningTeam(CapturePoint, ReturningTeamId);
			if (worldBossReturningTeamModel == null || worldBossModelManager.GetReturningTeamRemainingMs(worldBossReturningTeamModel) <= 0)
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: returning team gone before execute");
				return TWDModelResult.Error;
			}
			WorldBossOperationResult worldBossOperationResult = manager.ServerService.WorldBossInstantReturn(new WorldBossInstantReturnOperationRequest
			{
				GroupId = manager.Player.GuildId,
				PlayerHashedId = manager.Player.HashedId,
				SeasonId = base.SeasonId,
				CycleId = base.CycleId,
				CapturePoint = CapturePoint,
				ReturningTeamId = ReturningTeamId
			});
			if (worldBossOperationResult == null || !worldBossOperationResult.Success)
			{
				manager.Debug.LogError("WorldBossInstantReturnCommand: IServerService.WorldBossInstantReturn failed, skip charge");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}
	}
}
