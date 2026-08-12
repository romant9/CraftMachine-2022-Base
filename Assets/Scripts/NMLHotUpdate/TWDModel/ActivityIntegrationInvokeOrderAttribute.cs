using System;

namespace TWDModel
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ActivityIntegrationInvokeOrderAttribute : Attribute
	{
		public int InvokeOrder { get; set; }

		public ActivityIntegrationInvokeOrderAttribute(int invokeOrder = int.MaxValue)
		{
			InvokeOrder = invokeOrder;
		}
	}
}
