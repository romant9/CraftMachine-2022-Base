using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class ActorVariableNode : NodeBase
	{
		[IgnoreModelProperty]
		public List<ActorModel> actors { get; set; }

		[GraphItExportData("Current Actors", "")]
		public List<ActorModel> CurrentActors
		{
			get
			{
				return actors;
			}
			set
			{
				if (value != actors)
				{
					actors = value;
					ValueChanged();
				}
			}
		}

		public ActorVariableNode()
		{
		}

		public ActorVariableNode(ActorVariableNode node)
			: base(node)
		{
			actors = ((node.actors == null) ? null : new List<ActorModel>(node.actors));
		}

		public override NodeBase RecordValue()
		{
			return new ActorVariableNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			actors = null;
		}

		[GraphItOutput("Value Changed", "")]
		public void ValueChanged()
		{
			Fire("Value Changed");
		}
	}
}
