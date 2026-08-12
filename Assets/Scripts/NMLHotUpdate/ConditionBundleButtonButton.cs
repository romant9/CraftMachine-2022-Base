using TWDModel;
using UnityEngine;

public class ConditionBundleButtonButton : MonoBehaviour
{
	[SerializeField]
	private GameObject button;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UISprite buttonSprite;

	[SerializeField]
	private Metrics.BundleSource bundleSource;

	private ConditionBundleDefinition bundleOffered;

	private void OnEnable()
	{
		NoOffer();
		SetOffer(Helpers.GetFirstConditionBundle());
		UIEvent.Send("CampBottomLeftFreshEvent");
	}

	private void OnDisable()
	{
		NoOffer();
	}

	protected void NoOffer()
	{
		bundleOffered = null;
		Helpers.GameObjectSetActive(button, value: false);
	}

	protected void SetOffer(ConditionBundleDefinition bundle)
	{
		bundleOffered = bundle;
		Helpers.GameObjectSetActive(button, bundle != null);
	}

	private void Update()
	{
		bool activeSelf = button.activeSelf;
		ConditionBundleDefinition firstConditionBundle = Helpers.GetFirstConditionBundle();
		if (firstConditionBundle == null)
		{
			Helpers.GameObjectSetActive(button, value: false);
			RefreshBottomLeftTable(activeSelf);
			return;
		}
		SetOffer(firstConditionBundle);
		if (bundleOffered != null)
		{
			long giftLeftTime = GameManager.Instance.playerModel.RFMGiftManager.GetGiftLeftTime(bundleOffered.BundleIdentifier);
			timerLabel.text = Helpers.FormatTimeNoZero(giftLeftTime);
			if (giftLeftTime <= 0)
			{
				NoOffer();
			}
		}
		if (TutorialView.Instance.RunningButNotSuggesting)
		{
			Helpers.GameObjectSetActive(button, value: false);
		}
		RefreshBottomLeftTable(activeSelf);
	}

	public void OnClick()
	{
		if (bundleOffered != null && bundleOffered.BundleIdentifier != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			GameManager.Instance.BundleSource = bundleSource;
			BundleCardPopup.OpenBundle(bundleOffered.BundleIdentifier);
		}
	}

	private void RefreshBottomLeftTable(bool isActive)
	{
		if (isActive != button.activeSelf)
		{
			UIEvent.Send("CampBottomLeftFreshEvent");
		}
	}
}
