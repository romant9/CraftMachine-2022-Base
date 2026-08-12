using System;

namespace TWDModel
{
	[Serializable]
	public class EnableCoverNode : NodeBase
	{
		public EnableCoverNode()
		{
		}

		public EnableCoverNode(EnableCoverNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new EnableCoverNode(this);
		}

		[GraphItInput("Enable", "")]
		public void Enable()
		{
			foreach (CoverModel model in base.manager.CombatModel.GetModels<CoverModel>())
			{
				if (!model.IsActive)
				{
					model.FlipActiveState(null);
				}
			}
		}

		[GraphItInput("Disable", "")]
		public void Disable()
		{
			foreach (CoverModel model in base.manager.CombatModel.GetModels<CoverModel>())
			{
				if (model.IsActive)
				{
					model.FlipActiveState(null);
				}
			}
		}
	}
}
