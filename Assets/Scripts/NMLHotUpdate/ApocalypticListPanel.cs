using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ApocalypticListPanel : MonoBehaviour
{
	[SerializeField]
	private List<ApocalypticCard> apocalypticCards;

	public void Init(List<WeeklyChallengeApocalypseBuff> buffs)
	{
		for (int i = 0; i < apocalypticCards.Count; i++)
		{
			Helpers.GameObjectSetActive(apocalypticCards[i], value: false);
		}
		if (buffs == null)
		{
			return;
		}
		for (int j = 0; j < buffs.Count; j++)
		{
			ApocalypticCard apocalypticCard = apocalypticCards[j];
			if (apocalypticCard != null)
			{
				Helpers.GameObjectSetActive(apocalypticCard, value: true);
				apocalypticCard.UpdateUI(buffs[j]);
			}
		}
	}
}
