using System;

namespace TWDModel
{
	[Serializable]
	public class GameUpdateNode : NodeBase
	{
		public GameUpdateNode()
		{
		}

		public GameUpdateNode(GameUpdateNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new GameUpdateNode(this);
		}

		public override void Update()
		{
			CommandExecuted();
		}

		[GraphItOutput("Command Executed", "")]
		public void CommandExecuted()
		{
			Fire("Command Executed");
		}
	}
}
