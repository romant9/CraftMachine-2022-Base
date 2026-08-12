using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TraitDefinition : UtilsList.IDeepClonable<TraitDefinition>
	{
		public string Identifier;

		public string ClassName;

		public string DisplayName;

		public List<string> Tags;

		[NonSerialized]
		private HashSet<string> _tagsLookup;

		public int BuffPriority;

		public bool IsApocalypticTrait;

		public List<string> OwnerFilters;

		public bool CanBeDuplicate;

		public FixedPoint ProbabilityWeight;

		public List<string> ConstructionParameters;

		public List<string> EffectIndex;

		public List<string> DependsOnTraits;

		[JsonIgnore]
		public bool HasSurvivorClassFilter;

		public static string TRAIT_TAG_TACTICAL = "Tactical";

		public static string TRAIT_TAG_LOCKED = "Locked";

		public static string TRAIT_TAG_RARITY_LEVEL = "Level";

		public static string TRAIT_TAG_EQUIPMENT = "Equipment";

		public static string TRAIT_TAG_ARMOR = "Armor";

		public string GetTraitClassName()
		{
			if (ClassName == null || ClassName.Length <= 0)
			{
				return Identifier + "Trait";
			}
			return ClassName;
		}

		public bool HasTag(string inTag)
		{
			if (Tags == null || Tags.Count == 0)
			{
				return false;
			}
			if (_tagsLookup == null)
			{
				_tagsLookup = new HashSet<string>(Tags, StringComparer.OrdinalIgnoreCase);
			}
			return _tagsLookup.Contains(inTag);
		}

		public int GetParameterCount()
		{
			return ConstructionParameters.Count;
		}

		public T GetParameter<T>(int index)
		{
			string text = ConstructionParameters[index];
			if (typeof(T).IsEnum)
			{
				return (T)Enum.Parse(typeof(T), text);
			}
			if (typeof(T) == typeof(FixedPoint))
			{
				return (T)(object)new FixedPoint(text.ToString());
			}
			return (T)Convert.ChangeType(text, typeof(T));
		}

		public TraitDefinition()
		{
			Tags = new List<string>();
		}

		public TraitDefinition DeepClone()
		{
			return new TraitDefinition
			{
				Identifier = Identifier,
				ClassName = ClassName,
				DisplayName = DisplayName,
				Tags = ((Tags == null) ? null : new List<string>(Tags)),
				OwnerFilters = ((OwnerFilters == null) ? null : new List<string>(OwnerFilters)),
				CanBeDuplicate = CanBeDuplicate,
				ProbabilityWeight = ProbabilityWeight,
				ConstructionParameters = ((ConstructionParameters == null) ? null : new List<string>(ConstructionParameters)),
				DependsOnTraits = ((DependsOnTraits == null) ? null : new List<string>(DependsOnTraits)),
				EffectIndex = ((EffectIndex == null) ? null : new List<string>(EffectIndex)),
				HasSurvivorClassFilter = HasSurvivorClassFilter
			};
		}
	}
}
