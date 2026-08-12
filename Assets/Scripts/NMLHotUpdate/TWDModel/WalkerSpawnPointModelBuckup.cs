using System.Collections.Generic;

namespace TWDModel
{
	public class WalkerSpawnPointModelBuckup : ActorSpawnPointModelBackup
	{
		public bool UseOverrideWalkerType { get; set; }

		public WalkerType OverrideWalkerType { get; set; }

		public List<WalkerType> OverrideWalkerTypes { get; set; }

		public DormantType DormantType { get; set; }

		public bool IsBoss { get; set; }

		public bool AllowSpawningToAdjacent { get; set; }

		public int OverrideWalkerLevel { get; set; }

		public List<int> WalkerVisualizationChances { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void RecordStatus(ActorSpawnPointModel model)
		{
			base.RecordStatus(model);
			UseOverrideWalkerType = (base.Model as WalkerSpawnPointModel).UseOverrideWalkerType;
			OverrideWalkerType = (base.Model as WalkerSpawnPointModel).OverrideWalkerType;
			OverrideWalkerTypes = (((base.Model as WalkerSpawnPointModel).OverrideWalkerTypes == null) ? null : new List<WalkerType>((base.Model as WalkerSpawnPointModel).OverrideWalkerTypes));
			DormantType = (base.Model as WalkerSpawnPointModel).DormantType;
			IsBoss = (base.Model as WalkerSpawnPointModel).IsBoss;
			AllowSpawningToAdjacent = (base.Model as WalkerSpawnPointModel).AllowSpawningToAdjacent;
			OverrideWalkerLevel = (base.Model as WalkerSpawnPointModel).OverrideWalkerLevel;
			WalkerVisualizationChances = (((base.Model as WalkerSpawnPointModel).WalkerVisualizationChances == null) ? null : new List<int>((base.Model as WalkerSpawnPointModel).WalkerVisualizationChances));
		}

		public override void BackUp()
		{
			base.BackUp();
			(base.Model as WalkerSpawnPointModel).UseOverrideWalkerType = UseOverrideWalkerType;
			(base.Model as WalkerSpawnPointModel).OverrideWalkerType = OverrideWalkerType;
			(base.Model as WalkerSpawnPointModel).OverrideWalkerTypes = ((OverrideWalkerTypes == null) ? null : new List<WalkerType>(OverrideWalkerTypes));
			(base.Model as WalkerSpawnPointModel).DormantType = DormantType;
			(base.Model as WalkerSpawnPointModel).IsBoss = IsBoss;
			(base.Model as WalkerSpawnPointModel).AllowSpawningToAdjacent = AllowSpawningToAdjacent;
			(base.Model as WalkerSpawnPointModel).OverrideWalkerLevel = OverrideWalkerLevel;
			(base.Model as WalkerSpawnPointModel).WalkerVisualizationChances = ((WalkerVisualizationChances == null) ? null : new List<int>(WalkerVisualizationChances));
		}
	}
}
