using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class WalkerExplosionDefinition
	{
		public string TraitIdentifier;

		public bool ExplodeOnKill;

		public List<Faction> EffectedFactions;

		public string EffectClass;

		public List<string> Parameters;

		public T GetParameter<T>(int index)
		{
			string text = Parameters[index];
			if (typeof(T).IsEnum)
			{
				return (T)Enum.Parse(typeof(T), text);
			}
			if (typeof(T) == typeof(FixedPoint))
			{
				return (T)(object)new FixedPoint(text.ToString());
			}
			return (T)Convert.ChangeType(text, typeof(T));
		}
	}
}
