using System.Linq;
using TWDModel;
using UnityEngine;

public class RecycleWeaponPopupRewardPicItem : MonoBehaviour
{
	[Header("Star Card Mode")]
	[SerializeField]
	private GameObject starCardRoot;

	[SerializeField]
	private UISprite iconSprite1;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private UISprite BgMax;

	[Header("Card Mode")]
	[SerializeField]
	private GameObject CardRoot;

	[SerializeField]
	private UISprite iconSprite2;

	public void Setup(RewardPicEntry reward)
	{
		Helpers.GameObjectSetActive(starCardRoot, value: false);
		Helpers.GameObjectSetActive(CardRoot, value: false);
		if (reward.Count > 0)
		{
			SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions = GameManager.Instance.gameEconomyData?.GetSPTraitsRemodeDefinitions(null, null, reward.Count)?.FirstOrDefault();
			if (sPTraitsRemoldDefinitions != null)
			{
				BgMax.color = Helpers.HexToColor(sPTraitsRemoldDefinitions.Color);
			}
			Helpers.GameObjectSetActive(starCardRoot, value: true);
			iconSprite1.spriteName = reward.SpriteName;
			starList.Setup(reward.Count);
		}
		else
		{
			Helpers.GameObjectSetActive(CardRoot, value: true);
			iconSprite2.spriteName = reward.SpriteName;
		}
	}

	public void Setup(RewardShowPicEntry reward)
	{
		Helpers.GameObjectSetActive(starCardRoot, value: false);
		Helpers.GameObjectSetActive(CardRoot, value: false);
		if (reward.Count > 0)
		{
			SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions = GameManager.Instance.gameEconomyData?.GetSPTraitsRemodeDefinitions(null, null, reward.Star)?.FirstOrDefault();
			if (sPTraitsRemoldDefinitions != null)
			{
				BgMax.color = Helpers.HexToColor(sPTraitsRemoldDefinitions.Color);
			}
			Helpers.GameObjectSetActive(starCardRoot, value: true);
			iconSprite1.spriteName = reward.PicId;
			starList.Setup(reward.Star);
		}
		else
		{
			Helpers.GameObjectSetActive(CardRoot, value: true);
			iconSprite2.spriteName = reward.PicId;
		}
	}
}
