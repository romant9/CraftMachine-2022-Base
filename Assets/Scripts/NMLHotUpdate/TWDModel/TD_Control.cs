using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TD_Control
	{
		public string Missionid;

		public string WaveTime;

		public string Actor;

		public long WaveGap;

		public int WalkerLevel;

		public string PathId;

		[JsonIgnore]
		public string WalkerType => Regex.Replace(Actor, "(.*)(\\(.*)", "$1");

		[JsonIgnore]
		public int MaxSpwanNum => int.Parse(Regex.Replace(Actor, "(.*\\()(.*)(\\).*)", "$2"));

		[JsonIgnore]
		public long WaveTimeMS => (long)TimeSpan.Parse(WaveTime).TotalMilliseconds;
	}
}
