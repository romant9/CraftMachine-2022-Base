using BaseModel;

namespace TWDModel
{
	public class StartOutpostAttackCommand : ModelCommand
	{
		private const int OutpostTemporaryShieldDuration = 1800000;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager.Player.Combat == null)
			{
				tWDModelManager.Debug.LogError("StartOutpostAttack issued outside of combat!");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (tWDModelManager.Player.Combat.OutpostCombat == null)
			{
				tWDModelManager.Debug.LogError("StartOutpostAttack issued outside of outpost combat!");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			string defenderHashedId = tWDModelManager.Player.Combat.OutpostCombat.DefenderHashedId;
			MatchMakingInfo matchMakingInfo = tWDModelManager.GetMatchMakingInfo(defenderHashedId);
			if (matchMakingInfo == null)
			{
				tWDModelManager.Debug.LogError("No match info for " + defenderHashedId);
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (matchMakingInfo.Availability > tWDModelManager.Player.UtcTimeStamp / 1000)
			{
				tWDModelManager.Player.Combat.OutpostCombat.SetFake();
			}
			else
			{
				tWDModelManager.UpdateMatchMakingAvailability(defenderHashedId, tWDModelManager.Player.UtcTimeStamp + 1800000);
			}
			new Cashier(tWDModelManager).FakeSendPurchaseAnalyticsEvent();
			tWDModelManager.Player.Combat.OutpostCombat.CombatStarted = true;
			tWDModelManager.Player.LastVisitDebugInfo = "";
			tWDModelResult = TWDModelResult.OK;
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
