using UnityEngine;

public class OutpostLeaderboardListCard : UIListCard<OutpostLeaderboardScoreEntry>
{
	[SerializeField]
	private Color normalColor;

	[SerializeField]
	private Color highlightColor;

	[SerializeField]
	private UISprite highlightSpriteTarget;

	[SerializeField]
	private UILabel rankLabel;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UILabel memberTypeLabel;

	[SerializeField]
	private UISprite tierEmblem;

	[Header("Score")]
	[SerializeField]
	private GameObject scoreContainer;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UISprite defaultPortrait;

	private GameObject defaultPortraitGameObject;

	private void OnEnable()
	{
		if (defaultPortrait != null)
		{
			defaultPortraitGameObject = defaultPortrait.gameObject;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		nameLabel.text = GameManager.Instance.GetFilteredText(base.Item.Name);
		scoreLabel.text = base.Item.Score.ToString();
		levelLabel.text = base.Item.Level.ToString();
		scoreContainer.SetActive(value: true);
		if (defaultPortraitGameObject != null)
		{
			defaultPortraitGameObject.SetActive(value: true);
		}
		if (memberTypeLabel != null)
		{
			memberTypeLabel.text = "";
		}
		if (highlightSpriteTarget != null)
		{
			highlightSpriteTarget.color = (base.Item.IsOwnPlayer ? highlightColor : normalColor);
		}
		if (tierEmblem != null)
		{
			string tierEmblemIconName = HelpersGfx.GetTierEmblemIconName(base.Item.OutpostTierId);
			if (!string.IsNullOrEmpty(tierEmblemIconName))
			{
				tierEmblem.spriteName = tierEmblemIconName;
				tierEmblem.gameObject.SetActive(value: true);
			}
			else
			{
				tierEmblem.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetRank(int rank)
	{
		rankLabel.text = rank + ".";
	}

	public override int GetSortValue()
	{
		return -(int)base.Item.Score;
	}

	public void OnClick()
	{
	}
}
