using BaseModel;

namespace TWDModel
{
	public class AbilityAfterAddActiveTraitAction : ModelAction
	{
		public GridCoordinate TargetCell { get; private set; }

		public ActorModel Source { get; private set; }

		public AbilityAfterAddActiveTraitAction(ActorModel source, GridCoordinate targetCell)
			: base(source)
		{
			Source = source;
			TargetCell = targetCell;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((Source != null) ? Source.DebugInfo : "null") + " TargetCell = " + TargetCell.ToString();
		}
	}
}
