using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class ExitEnabledNode : NodeBase
	{
		public ExitEnabledNode()
		{
		}

		public ExitEnabledNode(ExitEnabledNode node)
			: base(node)
		{
		}

		public override NodeBase RecordValue()
		{
			return new ExitEnabledNode(this);
		}

		[GraphItInput("Compare", "")]
		public void Compare()
		{
			List<TWDModelObject> models = base.manager.Player.Combat.GetModels<CombatExitModel>();
			if (models != null)
			{
				for (int i = 0; i < models.Count; i++)
				{
					if ((models[i] as CombatExitModel).Enabled)
					{
						FireTrue();
						return;
					}
				}
			}
			FireFalse();
		}

		[GraphItOutput("True", "")]
		public void FireTrue()
		{
			Fire("True");
		}

		[GraphItOutput("False", "")]
		public void FireFalse()
		{
			Fire("False");
		}
	}
}
