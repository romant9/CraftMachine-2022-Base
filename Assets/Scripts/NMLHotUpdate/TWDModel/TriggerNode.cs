using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TriggerNode : NodeBase, TriggerReceiver
	{
		[JsonIgnore]
		[GraphItExportData("Last Instigator", "")]
		public ActorModel LastInstigator { get; set; }

		public TriggerNode()
		{
		}

		public TriggerNode(TriggerNode node)
			: base(node)
		{
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new TriggerNode(this);
		}

		public override void Start()
		{
			base.Start();
			LastInstigator = null;
		}

		[GraphItOutput("Triggered", "")]
		public void Triggered(ActorModel instigator)
		{
			LastInstigator = instigator;
			Fire("Triggered");
		}

		public void OnTriggered(ActorModel instigator)
		{
			Triggered(instigator);
		}
	}
}
