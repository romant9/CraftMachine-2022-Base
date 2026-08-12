namespace TWDModel
{
	public abstract class ParameterModifier : ModelModifier
	{
		public abstract bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor = null);

		public virtual string[] GetParameterNames()
		{
			return null;
		}
	}
}
