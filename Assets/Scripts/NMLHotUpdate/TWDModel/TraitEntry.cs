using System.Collections.Generic;

namespace TWDModel
{
	public class TraitEntry
	{
		public string TraitIdentifier;

		public long TraitDuration;

		public bool IsTemporary;

		public string Tag;

		public FixedPoint ConstructionParametersMultiplier;

		public List<int> RemodeValues;

		public List<int> RemodeParamIndex;

		public TraitEntry()
		{
			ConstructionParametersMultiplier = 1.0;
		}

		public TraitEntry(string traitIdentifier, long duration)
		{
			TraitIdentifier = traitIdentifier;
			TraitDuration = duration;
			ConstructionParametersMultiplier = 1.0;
		}

		public TraitEntry(string traitIdentifier, long duration, FixedPoint constructionParametersMultiplier)
		{
			TraitIdentifier = traitIdentifier;
			TraitDuration = duration;
			ConstructionParametersMultiplier = constructionParametersMultiplier;
		}

		public TraitDefinition RemodeTraitDefinition(TraitDefinition definition)
		{
			if (RemodeParamIndex == null || RemodeValues == null)
			{
				return definition;
			}
			TraitDefinition traitDefinition = definition.DeepClone();
			for (int i = 0; i < RemodeValues.Count && i < RemodeParamIndex.Count; i++)
			{
				traitDefinition.ConstructionParameters[RemodeParamIndex[i]] = RemodeValues[i].ToString();
			}
			return traitDefinition;
		}

		public TraitEntry(TraitEntry entry)
		{
			TraitIdentifier = entry.TraitIdentifier;
			TraitDuration = entry.TraitDuration;
			IsTemporary = entry.IsTemporary;
			Tag = entry.Tag;
			ConstructionParametersMultiplier = entry.ConstructionParametersMultiplier;
			RemodeValues = ((entry.RemodeValues == null) ? null : new List<int>(entry.RemodeValues));
			RemodeParamIndex = ((entry.RemodeParamIndex == null) ? null : new List<int>(entry.RemodeParamIndex));
		}
	}
}
