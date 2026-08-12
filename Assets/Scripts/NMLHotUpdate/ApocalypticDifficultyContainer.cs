using TWDModel;
using UnityEngine;

public class ApocalypticDifficultyContainer : MonoBehaviour
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

	private int selectIndex;

	public void Init(WeeklyChallengeApocalypseBuff model, int index)
	{
		selectIndex = index;
		if (nameLabel != null)
		{
			nameLabel.text = LocalizationManager.GetText(model.Name);
		}
		if (contentLabel != null)
		{
			contentLabel.text = HelpersLocalization.GetApocalypticDescription(model);
		}
		if (bg != null)
		{
			switch (model.Level)
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

	public void ClickSelectButton()
	{
		if (selectIndex >= 0 && selectIndex <= 2 && Helpers.ExecuteCommand(new SelectApocalypseBuffCommand(selectIndex)) == TWDModelResult.OK)
		{
			EventManager.NotifyClick("SelectApocalyptic");
		}
	}

	private void OnDestroy()
	{
		nameLabel = null;
		contentLabel = null;
	}
}
