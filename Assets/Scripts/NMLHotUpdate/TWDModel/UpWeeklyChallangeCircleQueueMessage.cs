namespace TWDModel
{
	public class UpWeeklyChallangeCircleQueueMessage : SupportLoadQueueMessage
	{
		public int Circle { get; set; }

		public bool IsAppolytic { get; set; }

		public UpWeeklyChallangeCircleQueueMessage()
		{
		}

		public UpWeeklyChallangeCircleQueueMessage(int circle, bool isAppolytic)
		{
			Circle = circle;
			IsAppolytic = isAppolytic;
		}

		public override bool Execute(TWDModelManager manager)
		{
			manager.Metrics.AddResetCombat(manager.Player.Combat != null).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			if (manager.Player.Combat != null)
			{
				manager.Player.DeleteCombatModel(notify: false);
			}
			if (manager.Player != null && manager.Player.WeeklyChallenge != null)
			{
				if (IsAppolytic)
				{
					manager.Player.WeeklyChallenge.OpenedApocalypseWeeklyChallenge = true;
					manager.Player.ApocalypseWeeklyChallenge.SkipToCircle(Circle);
				}
				else if (manager.Player.WeeklyChallenge.CurrentCycle + 1 < Circle)
				{
					manager.Player.WeeklyChallenge.SkipToCircle(Circle);
				}
				LootEntry lootEntry;
				do
				{
					lootEntry = manager.Player.WeeklyChallenge.GiveReward();
				}
				while (lootEntry != null);
				return true;
			}
			manager.Debug.LogError("Bundle reward failed, missing bundle id or invalid player. SupportEntityGUID: '" + base.SupportEntityGUID + "'");
			return true;
		}
	}
}
