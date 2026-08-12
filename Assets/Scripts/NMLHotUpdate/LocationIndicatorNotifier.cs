using UnityEngine;

public class LocationIndicatorNotifier : MonoBehaviour
{
	public IndicatorType indicatorType;

	private void Start()
	{
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD).ShowLocationIndicator(base.gameObject, indicatorType);
	}

	private void Stop()
	{
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD).HideLocationIndicator(base.gameObject);
	}
}
