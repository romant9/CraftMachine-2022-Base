using System;

namespace TWDModel
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
	public class ModelAvailableTimerAttribute : Attribute
	{
	}
}
