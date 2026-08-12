namespace TWDModel
{
	public struct ModifierLog
	{
		public string ParamName;

		public FixedPoint OldValue;

		public FixedPoint NewValue;

		public ActorModel Actor;

		public AbilityModel Ability;

		public bool Passive;

		public override string ToString()
		{
			string text = "Modification(";
			text += (Passive ? "Passive" : "Active");
			text += ((Actor != null) ? (", " + Actor.Name) : "");
			text += ((Ability != null) ? (", " + Ability.GetType().Name) : "");
			text = text + ") Param = '" + ParamName + "'";
			string[] obj = new string[5] { text, " ", null, null, null };
			FixedPoint oldValue = OldValue;
			obj[2] = oldValue.ToString();
			obj[3] = " -> ";
			oldValue = NewValue;
			obj[4] = oldValue.ToString();
			return string.Concat(obj);
		}
	}
}
