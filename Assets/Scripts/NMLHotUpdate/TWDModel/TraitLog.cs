namespace TWDModel
{
	public struct TraitLog
	{
		public string Trait;

		public FixedPoint Param;

		public override string ToString()
		{
			string trait = Trait;
			FixedPoint param = Param;
			return trait + "(" + param.ToString() + ")";
		}
	}
}
