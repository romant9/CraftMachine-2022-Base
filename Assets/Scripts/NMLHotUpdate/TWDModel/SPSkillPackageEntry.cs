using System;

namespace TWDModel
{
	[Serializable]
	public class SPSkillPackageEntry
	{
		public string PackageId;

		public int Count;

		public SPSkillPackageEntry(string packageId, int count)
		{
			PackageId = packageId;
			Count = count;
		}
	}
}
