using UnityEngine;

public class GuildShopResetIndicator : MonoBehaviour
{
	[SerializeField]
	public GameObject disableOverlappingIndicator;

	private void OnEnable()
	{
		bool flag = GuildWarHelper.CheckForGuildShopResetWarning();
		Helpers.GameObjectSetActive(base.gameObject, flag);
		if (flag)
		{
			Helpers.GameObjectSetActive(disableOverlappingIndicator, value: false);
		}
	}
}
