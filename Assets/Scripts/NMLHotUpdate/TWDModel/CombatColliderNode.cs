using System;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class CombatColliderNode : NodeBase
	{
		[IgnoreModelProperty]
		public CombatColliderModel CombatColliderModel { get; set; }

		public CombatColliderNode()
		{
		}

		public CombatColliderNode(CombatColliderNode node)
			: base(node)
		{
			CombatColliderModel = node.CombatColliderModel;
		}

		public override NodeBase RecordValue()
		{
			return new CombatColliderNode(this);
		}

		public override void Start()
		{
			CombatColliderModel combatColliderModel = CombatColliderModel;
			CombatColliderModel = null;
			base.Start();
			CombatColliderModel = combatColliderModel;
			base.manager.RegisterDelayedEventListener(CombatColliderModel, OnColliderChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			base.manager.UnregisterDelayedEventListener(CombatColliderModel, OnColliderChanged);
		}

		private void OnColliderChanged(ModelObject model, string changed, object args)
		{
			if (changed == "IsEnabled")
			{
				if (CombatColliderModel.IsEnabled)
				{
					Enabled();
				}
				else
				{
					Disabled();
				}
				ChangedEnabled();
			}
		}

		[GraphItInput("Enable", "Enable collider.")]
		public void Enable()
		{
			CombatColliderModel.SetEnabled(enabled: true);
		}

		[GraphItInput("Disable", "Disable collider.")]
		public void Disable()
		{
			CombatColliderModel.SetEnabled(enabled: false);
		}

		[GraphItOutput("Disabled", "Fired when combat collider is disabled.")]
		public void Disabled()
		{
			Fire("Disabled");
		}

		[GraphItOutput("Enabled", "Fired when combat collider is enabled.")]
		public void Enabled()
		{
			Fire("Enabled");
		}

		[GraphItOutput("ChangedEnabled", "Fired when combat collider enable / disable status changed.")]
		public void ChangedEnabled()
		{
			Fire("ChangedEnabled");
		}
	}
}
