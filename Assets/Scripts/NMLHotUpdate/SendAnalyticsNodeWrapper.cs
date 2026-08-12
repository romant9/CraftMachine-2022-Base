using TWDModel;
using UnityEngine;

public class SendAnalyticsNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public SendAnalyticsNode NodeBaseInternal = new SendAnalyticsNode();
}
