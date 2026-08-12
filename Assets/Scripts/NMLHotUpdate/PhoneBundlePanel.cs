using TWDModel;
using UnityEngine;

public class PhoneBundlePanel : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel NameLabel;

	[SerializeField]
	private UISprite IconSprite;

	[SerializeField]
	private UIButtonExtended Button;

	public BundleStoreDefinition CurrentDefinition { get; set; }

	public void UpdateUI(BundleStoreDefinition definition)
	{
		if (definition != null && NameLabel != null)
		{
			CurrentDefinition = definition;
			string text = LocalizationManager.GetText("IAPCard.ItemName." + definition.BundleIdentifier);
			string text2 = LocalizationManager.GetText("Popup.StartPhoneCall.BundleOffer.Description{BundleName}", text);
			NameLabel.text = text2;
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void SetClickCallback(UIButtonExtended.OnClickCallback callback)
	{
		if (Button != null)
		{
			Button.SetClickCallback(callback);
		}
	}

	public override void Clear()
	{
		if (Button != null)
		{
			Button.Clear();
		}
		base.Clear();
	}
}
