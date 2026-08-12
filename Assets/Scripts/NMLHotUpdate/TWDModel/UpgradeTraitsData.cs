using System.Collections.Generic;

namespace TWDModel
{
	public class UpgradeTraitsData
	{
		private Dictionary<string, List<int>> _thisRemodeValues;

		private Dictionary<string, List<int>> _thisRemodeParamIndex;

		public string Identifier { get; set; }

		public int UnlockingLevel { get; set; }

		public FixedPoint ConstructionMultiplier { get; set; }

		public TraitBucketsDefinition.BucketType BucketType { get; set; }

		public int RarityLevel { get; set; }

		public bool IsLocked { get; set; }

		public bool IsTactical { get; set; }

		public bool IsBreakthroughUnlockTrait { get; set; }

		public bool RemodelEd { get; set; }

		public bool RemodelIng { get; set; }

		public List<string> ThisRemodeIds { get; set; }

		public List<int> RemodeValues { get; set; }

		public Dictionary<string, List<int>> ThisRemodeValues
		{
			get
			{
				if (_thisRemodeValues == null)
				{
					_thisRemodeValues = new Dictionary<string, List<int>>();
				}
				return _thisRemodeValues;
			}
			set
			{
				_thisRemodeValues = value;
			}
		}

		public Dictionary<string, List<int>> ThisRemodeParamIndex
		{
			get
			{
				if (_thisRemodeParamIndex == null)
				{
					_thisRemodeParamIndex = new Dictionary<string, List<int>>();
				}
				return _thisRemodeParamIndex;
			}
			set
			{
				_thisRemodeParamIndex = value;
			}
		}

		public void ResetRemodel()
		{
			RemodelEd = false;
			RemodelIng = false;
			ThisRemodeIds = new List<string>();
			_thisRemodeValues = new Dictionary<string, List<int>>();
			_thisRemodeParamIndex = new Dictionary<string, List<int>>();
		}

		public static string StripTraitLevelIdentifier(string traitIdentifier)
		{
			int num = traitIdentifier.LastIndexOf(".");
			if (num >= 0 && traitIdentifier.Substring(num + 1).StartsWith(TraitDefinition.TRAIT_TAG_RARITY_LEVEL))
			{
				return traitIdentifier.Substring(0, num);
			}
			return traitIdentifier;
		}

		public static string StripEquipmentLabel(string traitIdentifier)
		{
			if (traitIdentifier.StartsWith(TraitDefinition.TRAIT_TAG_EQUIPMENT))
			{
				traitIdentifier = traitIdentifier.Substring(traitIdentifier.IndexOf(".") + 1);
				if (traitIdentifier.StartsWith(TraitDefinition.TRAIT_TAG_ARMOR))
				{
					traitIdentifier = traitIdentifier.Substring(traitIdentifier.IndexOf(".") + 1);
				}
			}
			return traitIdentifier;
		}

		public static int GetTraitLevelIdentifier(string traitIdentifier)
		{
			int num = traitIdentifier.LastIndexOf(TraitDefinition.TRAIT_TAG_RARITY_LEVEL);
			int result = 0;
			if (num >= 0)
			{
				int.TryParse(traitIdentifier.Substring(num + TraitDefinition.TRAIT_TAG_RARITY_LEVEL.Length), out result);
			}
			return result;
		}

		public static string CompileUpgradeTraitIdentifier(string traitIdentifier, int traitLevel, bool isLocked)
		{
			return StripTraitLevelIdentifier(traitIdentifier) + "." + TraitDefinition.TRAIT_TAG_RARITY_LEVEL + ((!isLocked) ? traitLevel : 0);
		}
	}
}
