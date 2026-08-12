using System.Collections.Generic;
using BestHTTP.Extensions;
using TWDModel;
using UnityEngine;

public class PlightConsumableHero : UIListCard<DifficultyIncrementalDebuff>
{
	[SerializeField]
	private List<GameObject> icons;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item == null)
		{
			return;
		}
		int num = base.Item.Name.ToInt32();
		int num2 = base.Item.Identifier.ToInt32();
		List<DifficultyIncrementalDebuff> lTDebuffs = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().CurrentCircleDefinition.LTDebuffs;
		for (int i = 0; i < icons.Count; i++)
		{
			int index = num * 6 + i;
			if (i < num2)
			{
				icons[i].SetActive(value: true);
				if (lTDebuffs[index] != null)
				{
					string spriteName = "Ui_Icon_" + lTDebuffs[index].LTTokenIcon;
					icons[i].GetComponent<UISprite>().spriteName = spriteName;
				}
			}
			else
			{
				icons[i].SetActive(value: false);
			}
		}
	}

	public void oncliek1()
	{
	}

	public void OnButtonClick(GameObject go)
	{
		string text = go.name;
		int num = 0;
		int num2 = base.Item.Name.ToInt32() * 6 + text switch
		{
			"icon0" => 0, 
			"icon1" => 1, 
			"icon2" => 2, 
			"icon3" => 3, 
			"icon4" => 4, 
			"icon5" => 5, 
			_ => 0, 
		} + 1;
		List<DifficultyIncrementalDebuff> lTDebuffs = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel().CurrentCircleDefinition.LTDebuffs;
		if (lTDebuffs[num2 - 1] != null)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ChallengeInfoPopup) as ChallengeInfoPopup).OpenForTrait(lTDebuffs[num2 - 1]);
		}
	}
}
