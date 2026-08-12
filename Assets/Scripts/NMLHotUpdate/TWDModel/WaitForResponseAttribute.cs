using System;

namespace TWDModel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class WaitForResponseAttribute : Attribute
	{
	}
}
