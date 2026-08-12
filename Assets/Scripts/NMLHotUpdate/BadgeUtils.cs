using System;
using System.Collections.Generic;
using TWDModel;

public static class BadgeUtils
{
	public const string defaultBadgeResourcePath = "BadgeCard";

	public const string defaultBadgeEmptyResourcePath = "BadgeCardEmpty";

	public const string defaultBadgeSmallResourcePath = "BadgeCardSmall";

	public static List<BadgeInfo> GetAllBadges()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		List<BadgeInfo> list = new List<BadgeInfo>();
		for (int i = 0; i < playerModel.Equipment.Badges.Models.Count; i++)
		{
			list.Add(new BadgeInfo(playerModel.Equipment.Badges.Models[i]));
		}
		for (int j = 0; j < playerModel.SurvivorContainer.Survivors.Count; j++)
		{
			SurvivorModel survivorModel = playerModel.SurvivorContainer.Survivors[j];
			for (int k = 0; k < survivorModel.BadgeContainer.Badges.Models.Count; k++)
			{
				BadgeInfo item = new BadgeInfo(survivorModel.BadgeContainer.Badges.Models[k])
				{
					OwnerName = survivorModel.Name
				};
				list.Add(item);
			}
		}
		list.Sort(BadgesSortAlgorithm);
		return list;
	}

	public static Dictionary<string, BadgeInfo> GetAllBadgesAsDictionary()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		Dictionary<string, BadgeInfo> dictionary = new Dictionary<string, BadgeInfo>();
		for (int i = 0; i < playerModel.Equipment.Badges.Models.Count; i++)
		{
			dictionary.Add(playerModel.Equipment.Badges.Models[i].ModelId.ToString(), new BadgeInfo(playerModel.Equipment.Badges.Models[i]));
		}
		for (int j = 0; j < playerModel.SurvivorContainer.Survivors.Count; j++)
		{
			SurvivorModel survivorModel = playerModel.SurvivorContainer.Survivors[j];
			for (int k = 0; k < survivorModel.BadgeContainer.Badges.Models.Count; k++)
			{
				BadgeInfo badgeInfo = new BadgeInfo(survivorModel.BadgeContainer.Badges.Models[k])
				{
					OwnerName = survivorModel.Name
				};
				dictionary.Add(badgeInfo.ModelId, badgeInfo);
			}
		}
		return dictionary;
	}

	public static int BadgesSortAlgorithm(BadgeInfo a, BadgeInfo b)
	{
		int num = ((a.OwnerName != null) ? 1000 : 0) + a.Model.Rarity * 100 + a.Model.EffectRoll;
		return ((b.OwnerName != null) ? 1000 : 0) + b.Model.Rarity * 100 + b.Model.EffectRoll - num;
	}



	#region mycode
	public static List<BadgeInfo> GetAllBadgesCorrect()
	{
		int modelIDCounter = 0;

		PlayerModel player = GameManager.Instance.playerModel;

		List<BadgeInfo> listTempUnequip = new List<BadgeInfo>();
		foreach (var model in player.Equipment.Badges.Models)
		{
			model.SetID(modelIDCounter);
			BadgeBonusDefinition badgeBonusDefinition = GameManager.Instance.gameEconomyData.GetBadgeBonusDefinition(model.BonusId);
			model.BonusCondition = CreateBonusCondition(badgeBonusDefinition, model);
			listTempUnequip.Add(new BadgeInfo(model));
			modelIDCounter++;
		}

		//for (int i = 0; i < player.Equipment.Badges.Models.Count; i++)
		//{
		//	player.Equipment.Badges.Models[i].SetID(modelIDCounter);
		//	BadgeBonusDefinition badgeBonusDefinition = GameManager.Instance.gameEconomyData.GetBadgeBonusDefinition(player.Equipment.Badges.Models[i].BonusId);
		//	player.Equipment.Badges.Models[i].BonusCondition = CreateBonusCondition(badgeBonusDefinition, player.Equipment.Badges.Models[i]);
		//	listTempUnequip.Add(new BadgeInfo(player.Equipment.Badges.Models[i]));
		//	modelIDCounter++;
		//}

		List<BadgeInfo> listTempEquip = new List<BadgeInfo>();
		foreach (var survivor in player.SurvivorContainer.Survivors.Models)
		{
			foreach (var model in survivor.BadgeContainer.Badges.Models)
			{
				model.SetID(modelIDCounter);
				BadgeBonusDefinition badgeBonusDefinition = GameManager.Instance.gameEconomyData.GetBadgeBonusDefinition(model.BonusId);
				model.BonusCondition = CreateBonusCondition(badgeBonusDefinition, model);
				BadgeInfo item = new BadgeInfo(model)
				{
					OwnerName = survivor.SurvivorName
				};
				listTempEquip.Add(item);
				modelIDCounter++;
			}
		}
		//for (int j = 0; j < player.SurvivorContainer.Survivors.Count; j++)
		//{
		//	SurvivorModel survivorModel = player.SurvivorContainer.Survivors[j];
		//	for (int k = 0; k < survivorModel.BadgeContainer.Badges.Models.Count; k++)
		//	{
		//		survivorModel.BadgeContainer.Badges.Models[k].SetID(modelIDCounter);
		//		BadgeBonusDefinition badgeBonusDefinition = GameManager.Instance.gameEconomyData.GetBadgeBonusDefinition(survivorModel.BadgeContainer.Badges.Models[k].BonusId);
		//		survivorModel.BadgeContainer.Badges.Models[k].BonusCondition = CreateBonusCondition(badgeBonusDefinition, survivorModel.BadgeContainer.Badges.Models[k]);
		//		BadgeInfo item = new BadgeInfo(survivorModel.BadgeContainer.Badges.Models[k])
		//		{
		//			OwnerName = survivorModel.SurvivorName
		//		};
		//		listTempEquip.Add(item);
		//		modelIDCounter++;
		//	}
		//}

		listTempUnequip.AddRange(listTempEquip);

		foreach (BadgeInfo badge in listTempUnequip)
		{
			badge.ModelId = badge.Model.ModelId.ToString();
		}

		listTempUnequip.Sort(BadgesSortAlgorithm);
		return listTempUnequip;
	}

	public static BonusCondition CreateBonusCondition(BadgeBonusDefinition bonusDef, BadgeModel badgeModel)
	{
		Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), bonusDef.ConditionClassName);
		if (!string.IsNullOrEmpty(bonusDef.ConditionClassName) && type == null)
		{
			DebugTWD.Log("Failed to instantiate condition class " + bonusDef.ConditionClassName);
		}

		List<string> bonusParams = badgeModel.BonusParameters;
		if (bonusParams.Count > 1)
		{
			return badgeModel.BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, bonusParams) as BaseBonusCondition) : null);
		}
		else
		{
			return badgeModel.BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, bonusParams) as ConstantBonusCondition) : null);
		}
	}
	#endregion
}
