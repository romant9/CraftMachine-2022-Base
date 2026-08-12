using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class StartDialogNode : NodeBase
	{
		public CombatDialogPlayerModel DialogPlayerModel;

		[JsonIgnore]
		[GraphItImportData("Instigator", "")]
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

		public StartDialogNode()
		{
		}

		public StartDialogNode(StartDialogNode node)
			: base(node)
		{
			DialogPlayerModel = node.DialogPlayerModel;
		}

		public override NodeBase RecordValue()
		{
			return new StartDialogNode(this);
		}

		[GraphItInput("Start Dialog", "")]
		public void StartDialog()
		{
			if (DialogPlayerModel != null)
			{
				DialogPlayerModel.StartDialog(Instigator);
				Started();
			}
		}

		[GraphItOutput("Started", "")]
		public void Started()
		{
			Fire("Started");
		}
	}
}
