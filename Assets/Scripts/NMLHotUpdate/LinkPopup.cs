using UnityEngine;

public class LinkPopup : HUDElement
{
	[SerializeField]
	protected UILabel titleLabel;

	[SerializeField]
	protected UILabel infoLabel;

	[SerializeField]
	protected UIScrollView descScrollView;

	public void SetContent(string key)
	{
		if (key != null && titleLabel != null)
		{
			titleLabel.text = LocalizationManager.GetText(key + ".Name");
		}
		if (key != null && infoLabel != null)
		{
			infoLabel.text = LocalizationManager.GetText(key + ".Desc");
		}
		descScrollView.UpdatePosition();
	}

	public override void Update()
	{
		if (!Input.GetMouseButtonDown(0))
		{
			Input.GetKeyUp(KeyCode.Escape);
		}
	}

	public void ShowPopup(string key)
	{
		LinkPopup linkPopup = null;
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			linkPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.LinkPopup) as LinkPopup;
		}
		if (linkPopup == null)
		{
			Debug.LogWarning("Alert popup not found!");
			return;
		}
		linkPopup.SetContent(key);
		linkPopup.Open();
	}
}
