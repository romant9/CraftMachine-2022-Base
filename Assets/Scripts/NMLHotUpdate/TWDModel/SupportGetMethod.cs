using System;

namespace TWDModel
{
	public class SupportGetMethod : Attribute
	{
		public string UpdateMethodName { get; set; }

		public SupportGetMethod()
		{
		}

		public SupportGetMethod(string updateMethodName)
		{
			UpdateMethodName = updateMethodName;
		}
	}
}
