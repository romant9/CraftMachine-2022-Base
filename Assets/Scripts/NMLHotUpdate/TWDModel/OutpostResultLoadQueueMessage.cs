using System;

namespace TWDModel
{
	public class OutpostResultLoadQueueMessage : LoadQueueMessage
	{
		public OutpostVisitEntry VisitEntry { get; set; }

		public OutpostResultLoadQueueMessage()
		{
		}

		public OutpostResultLoadQueueMessage(OutpostVisitEntry entry)
		{
			VisitEntry = entry;
		}

		public override bool Execute(TWDModelManager manager)
		{
			if (VisitEntry.UtcTime < manager.Player.ShieldTimeStamp)
			{
				manager.Debug.LogWarning("Ignoring OutpostResultLoadQueueMessage for " + manager.Player.HashedId + ", entry time " + VisitEntry.UtcTime + " shield end " + manager.Player.ShieldTimeStamp);
				return true;
			}
			manager.Player.UpdateOutpostSeasonAtAttackTime(VisitEntry.UtcTime);
			manager.Player.AddDefenseOutpostVisitLog(VisitEntry);
			if (VisitEntry.CombatResult == ECombatResult.Successful)
			{
				manager.Player.SetRankingScore(manager.Player.RankingScore + VisitEntry.RankingScoreChange);
				CurrencyModel currency = manager.Player.GetCurrency(CurrencyType.Outpost);
				if (currency != null)
				{
					int value = currency.Value;
					currency.SetValue(Math.Max(0, value - VisitEntry.ResourcesStolen));
				}
			}
			else
			{
				manager.Player.SetRankingScore(manager.Player.RankingScore + VisitEntry.RankingScoreChange);
			}
			if (VisitEntry.RequiresShield())
			{
				manager.Debug.Log("activating shield for player=" + manager.Player.HashedId);
				manager.Player.SetOutpostShield(VisitEntry.UtcTime);
			}
			if (manager.ServerService != null)
			{
				manager.Debug.Log("OutpostResultLoadQueueMessage::Execute() -> Player hash = " + manager.Player.HashedId + " Result = " + VisitEntry.CombatResult.ToString() + " Shield time left = " + manager.Player.GetShieldTimeMillisLeft(manager.Player.UtcTimeStamp));
			}
			return true;
		}
	}
}
