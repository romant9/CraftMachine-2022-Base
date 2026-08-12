using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GiveSPNode : NodeBase
	{
		[GraphItVariable("")]
		public int Amount;

		[GraphItVariable("")]
		public bool OneTimeOnly = true;

		public bool SPGiven;

		[JsonIgnore]
		[GraphItImportData("Instigator", "Actor that receives the reward.")]
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

		public GiveSPNode()
		{
		}

		public GiveSPNode(GiveSPNode node)
			: base(node)
		{
			Amount = node.Amount;
			OneTimeOnly = node.OneTimeOnly;
			SPGiven = node.SPGiven;
		}

		public override NodeBase RecordValue()
		{
			return new GiveSPNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			SPGiven = false;
		}

		[GraphItInput("Give", "Give reward.")]
		public void Give()
		{
			if (!SPGiven || !OneTimeOnly)
			{
				ActorModel instigator = Instigator;
				if (instigator != null)
				{
					instigator.GiveSP(Amount);
					SPGiven = true;
				}
			}
		}
	}
}
