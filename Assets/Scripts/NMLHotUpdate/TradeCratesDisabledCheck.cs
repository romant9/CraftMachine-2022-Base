using UnityEngine;

public class TradeCratesDisabledCheck : MonoBehaviour
{
	private void Awake()
	{
		if (!GameManager.Instance.gameEconomyData.ConfigData.TradeCratesEnabled)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
