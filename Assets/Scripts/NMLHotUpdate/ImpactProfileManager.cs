using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ImpactProfileManager : MonoBehaviour
{
	private static ImpactProfileManager instance;

	private ImpactProfileData impactProfileData;

	private QuickHitProfileData quickHitProfileData;

	public static ImpactProfileManager Instance => instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		impactProfileData = UnityUtils.LoadFromAssetBundle<ImpactProfileData>("ImpactProfileData", "scriptableobjects");
		quickHitProfileData = UnityUtils.LoadFromAssetBundle<QuickHitProfileData>("QuickHitProfileData", "scriptableobjects");
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public ImpactProfile GetExplosionImpactProfile()
	{
		List<ImpactProfile> list = new List<ImpactProfile>();
		foreach (ImpactProfile impactProfile in impactProfileData.ImpactProfiles)
		{
			if (impactProfile.isExplosive)
			{
				list.Add(impactProfile);
			}
		}
		if (list.Count > 0)
		{
			return list[Random.Range(0, list.Count)];
		}
		return null;
	}

	public ImpactProfile GetImpactProfile(EquipmentType equipmentType, string subCategory = null, bool isCritical = false)
	{
		for (int i = 0; i < ((impactProfileData.ImpactProfiles != null) ? impactProfileData.ImpactProfiles.Count : 0); i++)
		{
			ImpactProfile impactProfile = impactProfileData.ImpactProfiles[i];
			if (impactProfile.EquipmentType == equipmentType && impactProfile.isCriticalOnly == isCritical)
			{
				if (subCategory != null && impactProfile.SubCategory == subCategory)
				{
					return impactProfile;
				}
				if (subCategory == null)
				{
					return impactProfile;
				}
			}
		}
		for (int j = 0; j < ((impactProfileData.ImpactProfiles != null) ? impactProfileData.ImpactProfiles.Count : 0); j++)
		{
			ImpactProfile impactProfile2 = impactProfileData.ImpactProfiles[j];
			if (impactProfile2.EquipmentType == equipmentType)
			{
				if (subCategory != null && impactProfile2.SubCategory == subCategory)
				{
					return impactProfile2;
				}
				if (subCategory == null)
				{
					return impactProfile2;
				}
			}
		}
		for (int k = 0; k < ((impactProfileData.ImpactProfiles != null) ? impactProfileData.ImpactProfiles.Count : 0); k++)
		{
			ImpactProfile impactProfile3 = impactProfileData.ImpactProfiles[k];
			if (impactProfile3.useAsDefault)
			{
				return impactProfile3;
			}
		}
		if (impactProfileData.ImpactProfiles.Count > 0)
		{
			return impactProfileData.ImpactProfiles[0];
		}
		return null;
	}

	public QuickHitProfile GetQuickHitProfile(EquipmentType equipmentType, string subCategory = null)
	{
		foreach (QuickHitProfile quickHitProfile in quickHitProfileData.QuickHitProfiles)
		{
			if (quickHitProfile.EquipmentType == equipmentType)
			{
				if (subCategory != null && quickHitProfile.SubCategory == subCategory)
				{
					return quickHitProfile;
				}
				if (subCategory == null)
				{
					return quickHitProfile;
				}
			}
		}
		foreach (QuickHitProfile quickHitProfile2 in quickHitProfileData.QuickHitProfiles)
		{
			if (quickHitProfile2.useAsDefault)
			{
				return quickHitProfile2;
			}
		}
		if (quickHitProfileData.QuickHitProfiles.Count > 0)
		{
			return quickHitProfileData.QuickHitProfiles[0];
		}
		return null;
	}
}
