using System;

namespace TWDModel
{
	[Serializable]
	public struct CombatHUDStateInfo
	{
		public bool ShowObjectiveState;

		public bool ShowChargeState;

		public bool ShowFleeState;

		public bool ShowSkipTurnState;

		public bool ShowThreatState;

		public bool ShowKeysState;

		public bool ShowSpeedUpState;

		public void Reset()
		{
			ShowObjectiveState = true;
			ShowChargeState = true;
			ShowFleeState = true;
			ShowSkipTurnState = true;
			ShowThreatState = true;
			ShowKeysState = true;
			ShowSpeedUpState = true;
		}
	}
}
