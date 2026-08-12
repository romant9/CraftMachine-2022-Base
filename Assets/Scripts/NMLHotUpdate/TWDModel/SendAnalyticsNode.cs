using System;

namespace TWDModel
{
	[Serializable]
	public class SendAnalyticsNode : NodeBase
	{
		[GraphItVariable("Analytics Event Id to be sent to omniata")]
		public StepAnalyticsEvent Event;

		[GraphItVariable("The type of the step performed")]
		public StepID Step;

		[GraphItVariable("Location ID where the event happened")]
		public LocationID Location;

		[GraphItVariable("The tutorial step number")]
		public int StepNumber;

		public SendAnalyticsNode()
		{
		}

		public SendAnalyticsNode(SendAnalyticsNode node)
			: base(node)
		{
			Event = node.Event;
			Step = node.Step;
			Location = node.Location;
			StepNumber = node.StepNumber;
		}

		public override NodeBase RecordValue()
		{
			return new SendAnalyticsNode(this);
		}

		[GraphItInput("Send", "Send the analytics.")]
		public void Send()
		{
		}
	}
}
