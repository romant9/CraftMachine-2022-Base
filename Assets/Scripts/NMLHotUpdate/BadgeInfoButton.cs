using TWDModel;
using UnityEngine;

public class BadgeInfoButton : MonoBehaviour
{
	[SerializeField]
	private UIButtonExtended badgeInfoButton;

	[SerializeField]
	private int stepToOpen = 2;

	private void OnEnable()
	{
		badgeInfoButton.SetClickCallback(OnBadgeInfoClicked);
	}

	private void OnDisable()
	{
		badgeInfoButton.Clear();
	}

	public void OnBadgeInfoClicked(UIButtonExtended button)
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeInfoPopup);
		if (!GameManager.Instance.Blackboard.IsToggleOn("Toggle.ResidenceSeen"))
		{
			if (hUDElement != null)
			{
				hUDElement.Open();
				Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.ResidenceSeen"));
			}
			return;
		}
		PopupQuickTip component = hUDElement.GetComponent<PopupQuickTip>();
		if (component != null)
		{
			component.Open();
			component.ShowStep(stepToOpen);
		}
	}
}
