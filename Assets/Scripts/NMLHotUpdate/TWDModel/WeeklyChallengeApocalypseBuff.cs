using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WeeklyChallengeApocalypseBuff
	{
		public string Identifier;

		public ChallengeApocalypseBuffType BuffType;

		public string Level;

		public string Name;

		public List<string> Group;

		public List<FixedPoint> ConstructionParameters;

		public string Description;

		public int Weight;

		public List<string> Conflict;

		[JsonIgnore]
		private string[] _params;

		[JsonIgnore]
		public string[] GetConstructionParameters
		{
			get
			{
				if (_params == null)
				{
					_params = new string[ConstructionParameters.Count];
					for (int i = 0; i < ConstructionParameters.Count; i++)
					{
						_params[i] = ConstructionParameters[i].ToString();
					}
				}
				return _params;
			}
		}
	}
}
