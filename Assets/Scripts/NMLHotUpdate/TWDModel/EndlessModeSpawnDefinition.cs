using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class EndlessModeSpawnDefinition
	{
		public string SpawnSetupID;

		public string WaveID;

		public string SpawnCompositionID;

		public int WaveDuration;

		public int WaveSurviveRewardPoints;

		public string WavePointsIncrement;

		public int LevelOffSet;

		[JsonIgnore]
		public int[] GetWaveIncreamentCosts => (from item in WavePointsIncrement.Split(',')
			select int.Parse(item)).ToArray();

		[JsonIgnore]
		public List<string> GetSpawnCompositionIDs => SpawnCompositionID.Split(';').ToList();
	}
}
