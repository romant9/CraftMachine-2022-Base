using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivorCardBadgeElement : MonoBehaviour
{
	[SerializeField]
	private UISprite[] badgeSprites;

	[SerializeField]
	private Color32 emptyColor;

	[SerializeField]
	private Color32 equippedColor;

	[SerializeField]
	private Color32 bonusColor;

	private BadgeContainerModel badgeContainer;

	private BadgeContext badgeContext;

	public void SetDataForSurvivor(SurvivorModel survivorModel)
	{
		List<ActorModel> list = new List<ActorModel>();
		bool flag = false;
		for (int i = 0; i < GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors.Count; i++)
		{
			if (GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors[i] == survivorModel)
			{
				flag = true;
			}
			list.Add(GameManager.Instance.playerModel.SurvivorContainer.CombatSurvivors[i]);
		}
		BadgeContext context = new BadgeContext(survivorModel, flag ? list : null);
		SetData(survivorModel.BadgeContainer, context);
	}

	public void SetData(BadgeContainerModel container, BadgeContext context)
	{
		badgeContainer = container;
		badgeContext = context;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (badgeContainer == null || badgeContext == null)
		{
			for (int i = 0; i < ((badgeSprites != null) ? badgeSprites.Length : 0); i++)
			{
				badgeSprites[i].color = emptyColor;
			}
			return;
		}
		for (int j = 0; j < ((badgeSprites != null) ? badgeSprites.Length : 0); j++)
		{
			Color32 color = emptyColor;
			BadgeModel badge = badgeContainer.GetBadge(j);
			if (badge != null)
			{
				color = ((badge.BonusCondition == null || badge.BonusCondition is ConstantBonusCondition || !badge.BonusCondition.Evaluate(badgeContext)) ? equippedColor : bonusColor);
			}
			badgeSprites[j].color = color;
		}
	}
}
