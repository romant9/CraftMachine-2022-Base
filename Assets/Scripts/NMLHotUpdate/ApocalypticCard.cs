using TWDModel;
using UnityEngine;

public class ApocalypticCard : MonoBehaviour
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private UISprite bg;

	[SerializeField]
	private Color goldColor;

	[SerializeField]
	private Color silverColor;

	[SerializeField]
	private Color copperColor;

	public void UpdateUI(WeeklyChallengeApocalypseBuff item)
	{
		if (nameLabel != null)
		{
			nameLabel.text = LocalizationManager.GetText(item.Name);
		}
		if (contentLabel != null)
		{
			contentLabel.text = HelpersLocalization.GetApocalypticDescription(item);
		}
		if (bg != null)
		{
			switch (item.Level)
			{
			case "Level1":
				bg.color = copperColor;
				break;
			case "Level2":
				bg.color = silverColor;
				break;
			case "Level3":
				bg.color = goldColor;
				break;
			}
		}
	}

	private void OnDestroy()
	{
		nameLabel = null;
		contentLabel = null;
	}
}
