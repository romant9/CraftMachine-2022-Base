using System;

namespace BaseModel
{
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreModelPropertyAttribute : Attribute
	{
	}
}
