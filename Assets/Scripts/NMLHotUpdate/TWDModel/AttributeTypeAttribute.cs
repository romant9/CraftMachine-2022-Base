using System;

namespace TWDModel
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class AttributeTypeAttribute : Attribute
	{
		public AttributeType AttributeType { get; set; }

		public AttributeTypeAttribute(AttributeType attributeType)
		{
			AttributeType = attributeType;
		}
	}
}
