using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SetObjectiveNode : NodeBase
	{
		public string ObjectiveText;

		public string CustomText1;

		public string CustomText2;

		[GraphItVariable("Should show mission objectives popup")]
		public bool ShowPopup = true;

		[JsonIgnore]
		[GraphItImportData("Custom Value 1", "")]
		public object CustomValue1 => Import("Custom Value 1");

		[JsonIgnore]
		[GraphItImportData("Custom Value 2", "")]
		public object CustomValue2 => Import("Custom Value 2");

		public SetObjectiveNode()
		{
		}

		public SetObjectiveNode(SetObjectiveNode node)
			: base(node)
		{
			ObjectiveText = node.ObjectiveText;
			CustomText1 = node.CustomText1;
			CustomText2 = node.CustomText2;
			ShowPopup = node.ShowPopup;
		}

		public override NodeBase RecordValue()
		{
			return new SetObjectiveNode(this);
		}

		[GraphItInput("Set Objective", "Set objective to the text specified.")]
		public void SetObjective()
		{
			string customText = ((CustomValue1 != null) ? Convert.ToString(CustomValue1) : CustomText1);
			string customText2 = ((CustomValue2 != null) ? Convert.ToString(CustomValue2) : CustomText2);
			base.manager.Player.Combat.CurrentMissionObjective.SetDescription(ObjectiveText, customText, customText2, ShowPopup);
		}
	}
}
