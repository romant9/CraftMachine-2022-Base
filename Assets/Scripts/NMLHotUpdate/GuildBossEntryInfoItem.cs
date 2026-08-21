using UnityEngine;

public class GuildBossEntryInfoItem : MonoBehaviour
{
	[SerializeField]
	private UISprite bgBorder;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel valueLabel;

	public void UpdateUI(string name, string color)
	{
		HelpersUI.SetContentToLabel(nameLabel, LocalizationManager.GetText(name));
		nameLabel.color = Helpers.HexToColor(color);
		bgBorder.color = Helpers.HexToColor(color);
		if (valueLabel != null)
		{
			HelpersUI.SetContentToLabel(valueLabel, LocalizationManager.GetText(name + ".Desc"));
		}
	}
}
