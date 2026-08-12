using System;

namespace TWDModel
{
	[Serializable]
	public class GEDType : Attribute
	{
		public GEDSpecialType GedType;

		public GEDType(GEDSpecialType gt)
		{
			GedType = gt;
		}
	}
}
