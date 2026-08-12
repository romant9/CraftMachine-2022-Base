using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class BundleRotationSetupDefinition
	{
		public string RotationIdentifier;

		public bool IsRestartingPoint;

		public bool IgnoreHighestUnlockedClassForBundles;

		public List<string> SpenderTiers;

		public string RequiredRotation;

		public List<string> BundlesToRandomize;
	}
}
