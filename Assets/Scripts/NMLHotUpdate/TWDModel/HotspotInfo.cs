namespace TWDModel
{
	public class HotspotInfo
	{
		public string SliceViewId { get; set; }

		public string HotspotViewId { get; set; }

		public HotspotState State { get; set; }

		public WalkerType WalkerType { get; set; }

		public int Count { get; set; }

		public AIMode DefensiveMode { get; set; }

		public bool IsDefenderSpawn
		{
			get
			{
				if (State != HotspotState.DefenderSpawn_0 && State != HotspotState.DefenderSpawn_1)
				{
					return State == HotspotState.DefenderSpawn_2;
				}
				return true;
			}
		}

		public bool IsWalkerSpawn => State == HotspotState.Walker;

		public bool IsGoal
		{
			get
			{
				if (State != HotspotState.Flag)
				{
					return State == HotspotState.ResourceContainer;
				}
				return true;
			}
		}

		public int GetDefenderIndex()
		{
			if (State == HotspotState.DefenderSpawn_0)
			{
				return 0;
			}
			if (State == HotspotState.DefenderSpawn_1)
			{
				return 1;
			}
			if (State == HotspotState.DefenderSpawn_2)
			{
				return 2;
			}
			return -1;
		}
	}
}
