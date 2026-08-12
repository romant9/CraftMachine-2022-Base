namespace TWDModel
{
	public struct TargetActionLog
	{
		public string Name;

		public ActorModel Source;

		public ActorModel Target;

		public override string ToString()
		{
			return Name + "(" + ((Source != null) ? Source.Name : "null") + ", " + ((Target != null) ? Target.Name : "null") + ")";
		}
	}
}
