using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RecipeRowView : MonoBehaviour
{
	[Header("Components")]
	[SerializeField]
	private string MetalSprite;

	[SerializeField]
	private string FoodSprite;

	[SerializeField]
	private string ClothSprite;

	[SerializeField]
	private string ChemicalsSprite;

	[Header("Effects")]
	[SerializeField]
	private string DamageSprite;

	[SerializeField]
	private string CriticalChanceSprite;

	[SerializeField]
	private string CriticalDamageSprite;

	[SerializeField]
	private string HealthSprite;

	[SerializeField]
	private string DamageReductionSprite;

	[SerializeField]
	private UISprite[] Components;

	[SerializeField]
	private GameObject[] Checkmarks;

	[SerializeField]
	private UILabel Title;

	[SerializeField]
	private UISprite TitleImage;

	private Dictionary<string, string> componentSprites;

	private Dictionary<string, string> effectSprites;

	private void Awake()
	{
		componentSprites = new Dictionary<string, string>
		{
			{
				CurrencyType.Metal0.ToString(),
				MetalSprite
			},
			{
				CurrencyType.Food0.ToString(),
				FoodSprite
			},
			{
				CurrencyType.Cloth0.ToString(),
				ClothSprite
			},
			{
				CurrencyType.Chemicals0.ToString(),
				ChemicalsSprite
			}
		};
		effectSprites = new Dictionary<string, string>
		{
			{ "Damage", DamageSprite },
			{ "FlatDamage", DamageSprite },
			{ "CritChance", CriticalChanceSprite },
			{ "CritDamage", CriticalDamageSprite },
			{ "FlatCritDamage", CriticalDamageSprite },
			{ "Health", HealthSprite },
			{ "FlatHealth", HealthSprite },
			{ "DamageReduction", DamageReductionSprite }
		};
	}

	public void Init(BadgeRecipe badgeRecipe, List<CurrencyType> selectedComponents)
	{
		Components[0].spriteName = componentSprites[badgeRecipe.Component1];
		Components[1].spriteName = componentSprites[badgeRecipe.Component2];
		Components[2].spriteName = componentSprites[badgeRecipe.Component3];
		Components[3].spriteName = componentSprites[badgeRecipe.Component4];
		List<string> list = new List<string>
		{
			ComponentHelper.GetComponentBaseCurrency(selectedComponents[1]).ToString(),
			ComponentHelper.GetComponentBaseCurrency(selectedComponents[2]).ToString(),
			ComponentHelper.GetComponentBaseCurrency(selectedComponents[3]).ToString(),
			ComponentHelper.GetComponentBaseCurrency(selectedComponents[4]).ToString()
		};
		Checkmarks[0].SetActive(list.Remove(badgeRecipe.Component1));
		Checkmarks[1].SetActive(list.Remove(badgeRecipe.Component2));
		Checkmarks[2].SetActive(list.Remove(badgeRecipe.Component3));
		Checkmarks[3].SetActive(list.Remove(badgeRecipe.Component4));
		Title.text = ResidenceCraftBadgeTab.GetLocalizationForResult(badgeRecipe);
		string[] array = badgeRecipe.Results.Split(',');
		TitleImage.spriteName = effectSprites[array[0]];
	}
}
