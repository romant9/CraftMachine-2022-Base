using BaseModel;

namespace TWDModel
{
	public class AbilityBeforeRemoveActiveTraitAction : ModelAction
	{
		public GridCoordinate TargetCell { get; private set; }

		public ActorModel Source { get; private set; }

		public AbilityAction AbilityAction { get; private set; }

		public AbilityBeforeRemoveActiveTraitAction(ActorModel source, GridCoordinate targetCell, AbilityAction abilityAction = null)
			: base(source)
		{
			Source = source;
			TargetCell = targetCell;
			AbilityAction = abilityAction;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((Source != null) ? Source.DebugInfo : "null") + ", AbilityID = " + AbilityAction.Ability.DefinitionID + " TargetCell = " + TargetCell.ToString();
		}
	}
}
