using System.Collections.Generic;
using UnityEngine;

public class ImpactProfileData : ScriptableObject
{
	[Tooltip("Impact profiles for different equipment types.")]
	public List<ImpactProfile> ImpactProfiles = new List<ImpactProfile>();
}
