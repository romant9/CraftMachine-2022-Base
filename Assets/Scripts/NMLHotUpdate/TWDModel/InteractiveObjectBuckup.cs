using BaseModel;

namespace TWDModel
{
	public class InteractiveObjectBuckup : TWDModelObject
	{
		public int TurnsToComplete;

		[IgnoreModelProperty]
		public InteractiveObjectModel Model { get; set; }

		public int NPCAttackCount { get; set; }

		public bool InteractionDisabled { get; set; }

		public int lastTurnAttacked { get; set; }

		public bool HasBeenActivated { get; set; }

		public int UsedTurns { get; set; }

		[IgnoreModelProperty]
		public ActorModel Interactor { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(InteractiveObjectModel model)
		{
			Model = model;
			TurnsToComplete = model.TurnsToComplete;
			NPCAttackCount = model.NPCAttackCount;
			InteractionDisabled = model.InteractionDisabled;
			lastTurnAttacked = model.lastTurnAttacked;
			HasBeenActivated = model.HasBeenActivated;
			UsedTurns = model.UsedTurns;
			Interactor = model.Interactor;
		}

		public void BackUp()
		{
			Model.TurnsToComplete = TurnsToComplete;
			Model.NPCAttackCount = NPCAttackCount;
			Model.SetInteractionDisabled(InteractionDisabled);
			Model.UsedTurns = UsedTurns;
			Model.HasBeenActivated = HasBeenActivated;
			Model.lastTurnAttacked = lastTurnAttacked;
			Model.Interactor = Interactor;
		}
	}
}
