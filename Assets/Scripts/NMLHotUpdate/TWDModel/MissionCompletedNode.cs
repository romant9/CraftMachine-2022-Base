using System;

namespace TWDModel
{
	[Serializable]
	public class MissionCompletedNode : NodeBase
	{
		public MissionCompletedNode()
		{
		}

		public MissionCompletedNode(MissionCompletedNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new MissionCompletedNode(this);
		}

		[GraphItInput("Victory", "")]
		public void Victory()
		{
			base.manager.Player.Combat.ForceEndMissionVictory();
		}

		[GraphItInput("Failure", "")]
		public void Failure()
		{
			base.manager.Player.Combat.ForceEndMissionFailure();
		}
	}
}
