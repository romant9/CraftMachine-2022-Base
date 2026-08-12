using TwdCustomMod;
using UnityEngine;

public class ShowTooltip : MonoBehaviour
{
	public string LocalizationKey;

	public string LocalizationParameter;

	private void OnClick()
	{
		if (!string.IsNullOrEmpty(LocalizationKey) && !IsCustomText)
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, LocalizationManager.GetText(LocalizationKey, LocalizationParameter));
		}
		else
		{
			string text = "";
			if (!string.IsNullOrEmpty(EnCustomText))
			{
				text = LocalizationManager.Instance.CurrentLanguage == "ru" ? !string.IsNullOrEmpty(RuCustomText) ? RuCustomText : EnCustomText : EnCustomText;
			}
			else
			{
				text = LocalizationManager.GetCustomText(LocalizationKey, LocalizationParameter);
			}
			TooltipManager.OpenTextBoxWithText(gameObject, text, Prefab != null ? Prefab : CraftSettings.Instance.tooltipPrefab);
			if (OfflineManager.IsSaveToClipboard) MyTools.CopyToClipboard(text);
		}
	}

	public void OnClickEventIcon()
	{
		OnClick();
	}


	#region myparams
	private UIButton bt;
	public bool IsCustomText;
	public GameObject Prefab;
	public bool ActivateOnHover = false;
	[Multiline]
	public string EnCustomText;
	[Multiline]
	public string RuCustomText;
    #endregion

    #region mycode

    private void Start()
    {
		bt = GetComponent<UIButton>();
    }

    private void LateUpdate()
	{
		if (bt == null) return;
		if (ActivateOnHover && bt.state == UIButtonColor.State.Hover || bt.state == UIButtonColor.State.Pressed)
		{
			OnClick();
		}
	}
	#endregion
}
