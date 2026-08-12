using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BadgeRecipesPopup : HUDElement
{
	[SerializeField]
	private GameObject RecipePrefab;

	[SerializeField]
	private UITable RecipesContainer;

	public override void Open()
	{
		base.Open();
		foreach (Transform item in RecipesContainer.transform)
		{
			Object.Destroy(item);
		}
		List<CurrencyType> compoentsAsList = Object.FindObjectOfType<ResidenceCraftBadgeTab>().GetCompoentsAsList(includeNone: true);
		BadgeRecipe[] badgeRecipes = GameManager.Instance.gameEconomyData.BadgeRecipes;
		foreach (BadgeRecipe badgeRecipe in badgeRecipes)
		{
			Object.Instantiate(RecipePrefab, RecipesContainer.transform).GetComponent<RecipeRowView>().Init(badgeRecipe, compoentsAsList);
		}
		RecipesContainer.Reposition();
	}
}
