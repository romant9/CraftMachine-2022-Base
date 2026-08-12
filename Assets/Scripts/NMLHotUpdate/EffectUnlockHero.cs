using UnityEngine;

public class EffectUnlockHero : MonoBehaviour
{
	private GameObject effectPrefab;

	private GameObject effectParentGO;

	private GameObject effectGO;

	private static string croatEffectPrefabPath = "FX_Animated_Bleed";

	public void PlayCroatEffect(int trigger)
	{
		if (base.transform != null && !GameManager.Instance.IsGoreDisabled)
		{
			if (trigger == 0 && effectGO != null)
			{
				Object.Destroy(effectGO);
			}
			else if (trigger == 1)
			{
				effectParentGO = base.transform.FindInChildren("Bind_LeftForeArm")?.gameObject;
				effectPrefab = UnityUtils.LoadFromAssetBundle<PrefabResource>(croatEffectPrefabPath, "scriptableobjects").GetPrefab();
				effectGO = Helpers.InstantiateToParent(effectPrefab, effectParentGO);
			}
		}
	}
}
