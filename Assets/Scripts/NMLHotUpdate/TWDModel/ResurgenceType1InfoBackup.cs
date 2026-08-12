using System.Collections.Generic;

namespace TWDModel
{
	public sealed class ResurgenceType1InfoBackup
	{
		public ActorModel Source { get; set; }

		public bool ThisAbilityActionBearerTriggedRestoreAP { get; set; }

		public int TurnStartFactionActorNums { get; set; }

		public List<ActorModel> UsedChargeAttackActors { get; set; }

		public int ThisTurnAlreadyTiggerTimes { get; set; }
	}
}
