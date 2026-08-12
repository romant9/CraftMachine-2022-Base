using System;
using System.Collections.Generic;
using TWDModel;

[Serializable]
public class ImpactProfile
{
	[Tooltip("Should use this profile as default if proper impact profile cannot be found.")]
	public bool useAsDefault;

	[Tooltip("Equipment type for this profile.")]
	public EquipmentType EquipmentType;

	[Tooltip("Equipment sub category for this profile.")]
	public string SubCategory;

	[Tooltip("Is this profile for critical hits only.")]
	public bool isCriticalOnly;

	[Tooltip("Is this for explosion damage.")]
	public bool isExplosive;

	[Tooltip("Physics impact configurations defining this profile.")]
	public List<ImpactConfiguration> ImpactConfigurations = new List<ImpactConfiguration>();
}
