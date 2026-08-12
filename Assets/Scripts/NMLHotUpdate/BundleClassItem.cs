using TWDModel;
using UnityEngine;

public class BundleClassItem : MonoBehaviour
{
	[SerializeField]
	private UILabel BundleClassText;

	private ShopPopup popup;

	private BundleClassification BundleClass;

	public void SetKey(ShopPopup popup, BundleClassification key)
	{
		this.popup = popup;
		BundleClass = key;
		BundleClassText.text = LocalizationManager.GetText("BundleClass." + BundleClass);
	}

	public void OnClick()
	{
		popup.SetBundleClassFilter(BundleClass);
	}

	private void OnEnable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
		if (BundleClassText != null)
		{
			BundleClassText.text = LocalizationManager.GetText("BundleClass." + BundleClass);
		}
	}

	private void OnDisable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		if (BundleClassText != null)
		{
			BundleClassText.text = LocalizationManager.GetText("BundleClass." + BundleClass);
		}
	}
}
