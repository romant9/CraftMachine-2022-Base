using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class EnableExitNode : NodeBase
	{
		public EnableExitNode()
		{
		}

		public EnableExitNode(EnableExitNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new EnableExitNode(this);
		}

		[GraphItInput("Enable", "")]
		public void EnableExit()
		{
			List<TWDModelObject> models = base.manager.Player.Combat.GetModels<CombatExitModel>();
			if (models != null)
			{
				for (int i = 0; i < models.Count; i++)
				{
					(models[i] as CombatExitModel).Enabled = true;
				}
			}
		}

		[GraphItInput("Disable", "")]
		public void DisableExit()
		{
			List<TWDModelObject> models = base.manager.Player.Combat.GetModels<CombatExitModel>();
			if (models != null)
			{
				for (int i = 0; i < models.Count; i++)
				{
					(models[i] as CombatExitModel).Enabled = false;
				}
			}
		}
	}
}
