using System;

namespace TWDModel
{
	[Serializable]
	public class InitialCampBuildingData
	{
		public string TypeName;

		public FixedVec2 Position;

		public float RotationAngle;

		public int Level;

		public int DestroyAtCouncilLevel;

		public int SpawnAtCouncilLevel;

		public int RepairDependencyLevelRequired;

		public int CutDependencyLevelRequired;
	}
}
