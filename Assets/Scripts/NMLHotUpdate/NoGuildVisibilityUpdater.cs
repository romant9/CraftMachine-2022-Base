using UnityEngine;

public class NoGuildVisibilityUpdater : MonoBehaviour
{
	[SerializeField]
	private UILabel noGuildLabel;

	private void OnEnable()
	{
		if (GameManager.Instance.playerModel != null && !GameManager.Instance.playerModel.IsGuildMember)
		{
			if (noGuildLabel != null)
			{
				Helpers.GameObjectSetActive(noGuildLabel, value: true);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(noGuildLabel, value: false);
		}
	}
}
