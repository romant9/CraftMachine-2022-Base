using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class TD_BuffCheck
	{
		public enum TD_BuffEffect
		{
			None = 0,
			AttributeChange = 1,
			AttackAddHp = 2,
			AttackAddHpTeam = 3,
			AddDamageNextHit = 4,
			CastAddHp = 5,
			CastAddHpTeam = 6,
			SecAddHp = 7,
			AddBuffNextHit = 8,
			Stun = 9,
			Root = 10
		}

		public string Buffid;

		public TD_BuffEffect Identifier;

		public List<string> Paramter;

		public bool Duplicate;

		public string Description;

		public string EffectID;
	}
}
