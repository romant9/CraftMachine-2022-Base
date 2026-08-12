namespace TWDModel
{
	public class PushEffect
	{
		public DamageAction DamageAction;

		public GridCoordinate OriginalCoordinate;

		public GridCoordinate PushCoordinate;

		public bool Handled;

		public PushEffect DependsOn;

		public PushEffect Dependant;
	}
}
