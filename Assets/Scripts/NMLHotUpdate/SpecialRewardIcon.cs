using UnityEngine;

public class SpecialRewardIcon : MonoBehaviour
{
	[Tooltip("Icon for gained currency icon.")]
	[SerializeField]
	private UISprite icon;

	public const string goldIconName = "Ui_Icon_Loot_Gold_Small";

	public const string silverIconName = "Ui_Icon_Loot_Silver_Small";

	public const string goldIconNameSmall = "Ui_Bg_Cardback_Gold_Small";

	public const string silverIconNameSmall = "Ui_Bg_Cardback_Silver_Small";

	private const string openedIconName = "Ui_Icon_Check";

	public SpecialRewardIconState State { get; set; }

	public void Awake()
	{
		SetIcon(SpecialRewardIconState.Empty);
	}

	public void Open()
	{
		TweenManager.PlayTweenGroup(base.gameObject, 0, forward: true, OnOpened);
		State = SpecialRewardIconState.Opened;
	}

	public void SetIcon(SpecialRewardIconState iconState)
	{
		UISprite component = GetComponent<UISprite>();
		switch (iconState)
		{
		case SpecialRewardIconState.Empty:
			icon.gameObject.SetActive(value: false);
			break;
		case SpecialRewardIconState.Silver:
			icon.gameObject.SetActive(value: false);
			HelpersGfx.UpdateSpriteAndKeepScale(component, "Ui_Bg_Cardback_Silver_Small");
			break;
		case SpecialRewardIconState.Gold:
			icon.gameObject.SetActive(value: false);
			HelpersGfx.UpdateSpriteAndKeepScale(component, "Ui_Bg_Cardback_Gold_Small");
			break;
		case SpecialRewardIconState.Opened:
			icon.gameObject.SetActive(value: true);
			break;
		}
		State = iconState;
	}

	public void OnOpened()
	{
		icon.gameObject.SetActive(value: true);
	}
}
