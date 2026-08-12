namespace TWDModel
{
	public abstract class DebuffParameterAbstract<T> : DebuffParameterBase
	{
		public T ParameterValue { get; private set; }

		public DebuffParameterAbstract(DebuffParameterAbstract<T> debuffDebuffParameterAbstract)
			: base(debuffDebuffParameterAbstract)
		{
			ParameterValue = debuffDebuffParameterAbstract.ParameterValue;
		}

		public DebuffParameterAbstract()
		{
		}

		public DebuffParameterAbstract(string parameterKey, T parameterValue)
			: base(parameterKey)
		{
			ParameterValue = parameterValue;
		}

		public DebuffParameterAbstract(string parameterKey, T parameterValue, int expiryTurn)
			: base(parameterKey, expiryTurn)
		{
			ParameterValue = parameterValue;
		}
	}
}
