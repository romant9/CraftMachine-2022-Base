using System.Collections.Generic;

namespace TWDModel
{
	public class BundleRotationDefinition
	{
		public string RotationIdentifier { get; set; }

		public List<string> SpenderTiers { get; set; }

		public string RequiredRotation { get; set; }

		public int RestartingPoint { get; set; }

		public int RotationNumber { get; set; }

		public List<List<string>> BundlesToRandomizeOnSteps { get; set; }

		public List<bool> BundlesToRandomizeIgnoresHighesUnlockClass { get; set; }

		public int TotalSteps
		{
			get
			{
				if (BundlesToRandomizeOnSteps != null)
				{
					return BundlesToRandomizeOnSteps.Count;
				}
				return 0;
			}
		}
	}
}
