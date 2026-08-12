using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SetActorVariableNode : NodeBase
	{
		[JsonIgnore]
		[GraphItImportData("New Actors", "New list of actors to set, will replace the current set of Actors.")]
		public List<ActorModel> NewActors => Import("New Actors") as List<ActorModel>;

		[JsonIgnore]
		[GraphItImportData("Target Actors", "Target list of actors to set.")]
		public List<ActorModel> TargetActors
		{
			set
			{
				Export("Target Actors", NewActors);
			}
		}

		public SetActorVariableNode(SetActorVariableNode node)
			: base(node)
		{
		}

		public SetActorVariableNode()
		{
		}

		public override NodeBase RecordValue()
		{
			return new SetActorVariableNode(this);
		}

		[GraphItInput("Set", "")]
		public void Set()
		{
			TargetActors = NewActors;
		}
	}
}
