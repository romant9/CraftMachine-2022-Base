using UnityEngine;

public class ActorEffectInfoItem : MonoBehaviour
{
	private const string DefaultBgSpriteName = "Ui_Equipment_Rarity_Common";

	[SerializeField]
	private UISprite bgSprite;

	[SerializeField]
	private UISprite iconSprite;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel descLabel;

	[SerializeField]
	private GameObject timeGo;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private UIProgressBar cdProgressBar;

	[SerializeField]
	private UILabel cdLabel;

	public void UpdateUI(ActorEffectInfoData data)
	{
		ApplyBg(data.Bg);
		data.Icon.ApplyTo(iconSprite);
		if (nameLabel != null)
		{
			HelpersUI.SetContentToLabel(nameLabel, LocalizationManager.GetText(data.NameLocKey));
		}
		if (descLabel != null)
		{
			HelpersUI.SetContentToLabel(descLabel, LocalizationManager.GetText(data.DescLocKey));
		}
		ApplyTurnCount(data.TurnCount);
	}

	public void UpdateUI(string iconSpriteName, string nameLocKey, string descLocKey, int turnCount = 1)
	{
		ApplyBg(default(ActorEffectSprite));
		if (iconSprite != null)
		{
			if (!string.IsNullOrEmpty(iconSpriteName))
			{
				HelpersUI.SetSprite(iconSprite, iconSpriteName);
			}
			else
			{
				Helpers.GameObjectSetActive(iconSprite.gameObject, value: false);
			}
		}
		if (nameLabel != null)
		{
			HelpersUI.SetContentToLabel(nameLabel, LocalizationManager.GetText(nameLocKey));
		}
		if (descLabel != null)
		{
			HelpersUI.SetContentToLabel(descLabel, LocalizationManager.GetText(descLocKey));
		}
		ApplyTurnCount(turnCount);
	}

	private void ApplyTurnCount(int turnCount)
	{
		if (timeLabel != null)
		{
			bool flag = turnCount > 0;
			Helpers.GameObjectSetActive(timeGo.gameObject, flag);
			if (flag)
			{
				timeLabel.text = turnCount.ToString();
			}
		}
	}

	private void ApplyBg(ActorEffectSprite bg)
	{
		if (!(bgSprite == null))
		{
			Helpers.GameObjectSetActive(bgSprite.gameObject, value: true);
			if (bg.IsValid && bg.Atlas != null)
			{
				bg.ApplyTo(bgSprite);
			}
			else
			{
				HelpersUI.SetSprite(bgSprite, "Ui_Equipment_Rarity_Common");
			}
		}
	}
}
