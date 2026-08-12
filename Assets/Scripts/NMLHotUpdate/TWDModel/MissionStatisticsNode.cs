using System;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class MissionStatisticsNode : NodeBase
	{
		[JsonIgnore]
		[GraphItExportData("Walkers Killed", "How many walkers have been killed.")]
		public int WalkersKilled => base.manager.Player.Combat.MissionStatistics.WalkersKilled;

		[JsonIgnore]
		[GraphItExportData("Boxes Opened", "How many loot boxes opened.")]
		public int BoxesOpened => base.manager.Player.Combat.MissionStatistics.CollectedLoot;

		[JsonIgnore]
		[GraphItExportData("Keys", "How many keys found.")]
		public int Keys => base.manager.Player.Combat.MissionStatistics.CollectedLoot;

		public MissionStatisticsNode()
		{
		}

		public MissionStatisticsNode(MissionStatisticsNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new MissionStatisticsNode(this);
		}

		public override void Start()
		{
			base.Start();
			base.manager.RegisterDelayedEventListener(base.manager.Player.Combat.MissionStatistics, MissionStatistics_OnChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			base.manager.UnregisterDelayedEventListener(base.manager.Player.Combat.MissionStatistics, MissionStatistics_OnChanged);
		}

		private void MissionStatistics_OnChanged(ModelObject model, string changed, object args)
		{
			Modified();
		}

		[GraphItOutput("Modified", "Whenever mission statistics change this pin will fire.")]
		public void Modified()
		{
			Fire("Modified");
		}
	}
}
