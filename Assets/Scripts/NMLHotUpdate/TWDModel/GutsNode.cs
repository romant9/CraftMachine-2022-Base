using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GutsNode : NodeBase
	{
		[GraphItVariable("")]
		public int Turns;

		[JsonIgnore]
		[GraphItExportData("Last Instigator", "Actor that performed the last operation.")]
		public ActorModel LastInstigator { get; set; }

		[JsonIgnore]
		[GraphItImportData("Instigator", "Actor that performs the operation.")]
		public ActorModel Instigator
		{
			get
			{
				object obj = Import("Instigator");
				if (obj is ActorModel result)
				{
					return result;
				}
				if (obj is List<ActorModel> { Count: >0 } list)
				{
					return list[0];
				}
				return null;
			}
		}

		public GutsNode()
		{
		}

		public GutsNode(GutsNode node)
			: base(node)
		{
			Turns = node.Turns;
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new GutsNode(this);
		}

		[GraphItInput("Smear", "Actor gets smeared with guts")]
		public void Smear()
		{
			ActorModel instigator = Instigator;
			if (instigator != null)
			{
				instigator.ClearInvisibility();
				instigator.SetInvisible(Turns, instigator);
				instigator.IsInteractingWithGuts = true;
				instigator.MoveCompleted = false;
				instigator.SecondMoveCompleted = false;
				instigator.EnsureExtraAP = false;
				instigator.AdditionalMoveRange = 0;
				instigator.AllowSecondMoveAfterAbility = true;
				instigator.NotifyChange("actorExtraAbilityAction", new object[2] { "", false });
			}
		}
	}
}
