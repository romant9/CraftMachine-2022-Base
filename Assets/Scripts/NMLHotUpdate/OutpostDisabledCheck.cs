using UnityEngine;

public class OutpostDisabledCheck : MonoBehaviour
{
	private void Awake()
	{
		if (!GameManager.Instance.gameEconomyData.ConfigData.OutpostEnabled)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
