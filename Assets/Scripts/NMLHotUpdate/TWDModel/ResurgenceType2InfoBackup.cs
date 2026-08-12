using System.Collections.Generic;

namespace TWDModel
{
	public sealed class ResurgenceType2InfoBackup
	{
		public ActorModel Source { get; set; }

		public bool ThisAbilityActionBearerTriggedRestoreAP { get; set; }

		public int TurnStartFactionActorNums { get; set; }

		public List<ActorModel> UsedChargeAttackActors { get; set; }

		public int NextCanTriggedRestoreAPTurn { get; set; }
	}
}
