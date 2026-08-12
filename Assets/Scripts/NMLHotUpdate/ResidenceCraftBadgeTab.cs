using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class ResidenceCraftBadgeTab : MonoBehaviour
{
	[SerializeField]
	private UIButtonWithLabelAndIcon craftButton;

	[SerializeField]
	private PayButton craftPayButton;

	[SerializeField]
	private UIButtonExtended autoFillButton;

	[SerializeField]
	private List<RecipeComponentView> componentButtonList;

	[SerializeField]
	private List<RecipeComponentView> slotButtonList;

	[SerializeField]
	private UILabel rarityLabel;

	[SerializeField]
	private UILabel effectChanceLabel;

	[SerializeField]
	private float showCraftResultDelay;

	[SerializeField]
	private UIButton dropRateButton;

	[SerializeField]
	private BadgeInfoButton badgeInfoButton;

	[SerializeField]
	private string Color50;

	[SerializeField]
	private string Color75;

	[SerializeField]
	private string Color99;

	[SerializeField]
	private string Color100;

	public CurrencyType[] currentSelection { get; private set; } = new CurrencyType[5];

	private Cashier costCashier;

	private TweenScale tweenScale;

	private void Awake()
	{
		tweenScale = effectChanceLabel.GetComponent<TweenScale>();
	}

	public void OnEnable()
	{
		UpdateUI();
		HelpersUI.SetButtonState(craftButton, UIButtonColor.State.Normal);
		UIEvent.OnUIEvent += OnUIEvent;
		if (craftButton != null)
		{
			craftButton.SetClickCallback(OnClickCraft);
		}
		if (autoFillButton != null)
		{
			autoFillButton.SetClickCallback(OnClickAutofill);
		}
	}

	public void OnDropRateClicked()
	{
		List<CurrencyType> compoentsAsList = GetCompoentsAsList();
		if (compoentsAsList != null && compoentsAsList.Count > 0 && GameManager.Instance.gameEconomyData != null)
		{
			List<ItemAmountProbabilityData> probabilities = GameManager.Instance.gameEconomyData.GetBadgeProbabilities(compoentsAsList);
			DropRatesNamesHelper.GetNamesForBadges(ref probabilities);
			DropRatesInfoPopup obj;

			if (IsLoadDataManager && DropRatesInfoPopup)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager && DropRatesInfoPopup)");
				DropRatesInfoPopup.SetActive(true);
				obj = DropRatesInfoPopup.GetComponent<DropRatesInfoPopup>();
			}
			else
			{
				obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DropRatesInfoPopup) as DropRatesInfoPopup;
			}
			DropTableItem dropTableItem = new DropTableItem
			{
				DropName = LocalizationManager.GetText("Droprate.Table.Name.Badges"),
				Description = LocalizationManager.GetText("Droprate.Table.Description.Badges"),
				Probabilities = probabilities
			};
			obj.TryOpenWithNormalData(dropTableItem);
		}
	}

	public void OnDisable()
	{
		Clear();
	}

	public void UpdateUI()
	{
		if (craftButton != null && currentSelection != null)
		{
			craftButton.IsVisuallyDisabled = !IsSelectionValid();
			craftButton.SetContentToLabelOne(LocalizationManager.GetText("Residence.CraftButton.Text"));
		}
		costCashier = GameManager.Instance.playerModel.LootManager.GetBadgeCraftCashier(GetCompoentsAsList());
		if (costCashier != null && Helpers.GameObjectSetActive(craftPayButton, costCashier.GetTotalCost(CurrencyType.Supplies) > 0))
		{
			craftPayButton.UpdateUI(costCashier, null, -1, new CurrencyType[1] { CurrencyType.Supplies });
		}
		if (!GameManager.Instance.Blackboard.IsToggleOn("Toggle.ResidenceSeen") && badgeInfoButton != null)
		{
			badgeInfoButton.OnBadgeInfoClicked(null);
		}
		if (componentButtonList != null && slotButtonList != null && componentButtonList.Count == slotButtonList.Count)
		{
			for (int i = 0; i < componentButtonList.Count; i++)
			{
				if (!(componentButtonList[i] != null) || !slotButtonList[i] || currentSelection == null || currentSelection.Length <= i)
				{
					continue;
				}
				int num = 0;
				bool flag = false;
				bool flag2 = currentSelection[i] != CurrencyType.None;
				int num2 = 0;
				List<CurrencyModel> listRef = new List<CurrencyModel>();
				if (i > 0)
				{
					TooltipComponentSelector.GetAllComponentModelsList(GameManager.Instance.playerModel, ref listRef, TooltipComponentSelector.FilterType.Exclude, CurrencyType.Badge0, 1);
				}
				else
				{
					TooltipComponentSelector.GetAllComponentModelsList(GameManager.Instance.playerModel, ref listRef, TooltipComponentSelector.FilterType.Only, CurrencyType.Badge0, 1);
				}
				Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
				foreach (CurrencyModel item in listRef)
				{
					num2 += item.Value;
					dictionary[item.Type] = item.Value;
				}
				for (int j = 0; j < currentSelection.Length; j++)
				{
					if (currentSelection[j] != CurrencyType.None && dictionary.ContainsKey(currentSelection[j]))
					{
						dictionary[currentSelection[j]]--;
						num2--;
					}
				}
				if (flag2)
				{
					CurrencyType currencyType = currentSelection[i];
					int num3 = 1;
					for (int k = 1; k < i; k++)
					{
						if (currentSelection[k] == currencyType)
						{
							num3++;
						}
					}
					num = (HasTheCurrency(currencyType, num3) ? 1 : 0);
					flag = num == 0;
					int componentRarityLevel = ComponentHelper.GetComponentRarityLevel(currencyType);
					bool flag3 = false;
					foreach (KeyValuePair<CurrencyType, int> item2 in dictionary)
					{
						if (item2.Value > 0 && ComponentHelper.GetComponentRarityLevel(item2.Key) > componentRarityLevel)
						{
							flag3 = true;
						}
					}
					flag = flag || flag3;
					Helpers.GameObjectSetActive(componentButtonList[i], value: true);
					Helpers.GameObjectSetActive(slotButtonList[i], value: false);
					componentButtonList[i].Initialize(currentSelection[i], num, allowed: true, i.ToString(), OnClickSlot, flag);
				}
				else
				{
					num = 0;
					flag = num2 > 0;
					Helpers.GameObjectSetActive(componentButtonList[i], value: false);
					Helpers.GameObjectSetActive(slotButtonList[i], value: true);
					if (i == 0)
					{
						slotButtonList[i].Initialize(CurrencyType.Badge0, 0, allowed: true, i.ToString(), OnClickSlot, flag);
					}
					else
					{
						slotButtonList[i].Initialize(currentSelection[i], 0, allowed: true, i.ToString(), OnClickSlot, flag);
					}
				}
			}
		}
		else
		{
			Debug.LogError("Check dragged refences to : ResidenceCraftBadgeTab or its children!");
		}
		if (IsSelectionValid())
		{
			List<CurrencyType> components = GetCompoentsAsList();
			BadgeRarityResult badgeRarityResult = GameManager.Instance.modelManager.GameEconomyData.CalculateBadgeRarityResult(components);
			if (badgeRarityResult == null)
			{
				Helpers.GameObjectSetActive(rarityLabel, value: false);
				Helpers.GameObjectSetActive(effectChanceLabel, value: false);
				Helpers.GameObjectSetActive(dropRateButton, value: true);
				Helpers.GameObjectSetActive(dropRateButton, value: false);
				return;
			}
			int minRarity = badgeRarityResult.MinRarity;
			int maxRarity = badgeRarityResult.MaxRarity;
			Color gradientColorBottom = GameManager.Instance.GetRarityColorData(minRarity).GradientColorBottom;
			Color gradientColorBottom2 = GameManager.Instance.GetRarityColorData(maxRarity).GradientColorBottom;
			string text = $"[{NGUIText.EncodeColor(gradientColorBottom)}]{HelpersLocalization.GetRarityLevel(minRarity)}[-]";
			string text2 = $"[{NGUIText.EncodeColor(gradientColorBottom2)}]{HelpersLocalization.GetRarityLevel(maxRarity)}[-]";
			HelpersUI.SetContentToLabel(rarityLabel, LocalizationManager.GetText("Residence.ResultRarity", text, text2));
			Helpers.GameObjectSetActive(dropRateButton, value: true);
			BadgeRecipe badgeRecipe = GameManager.Instance.gameEconomyData.BadgeRecipes.FirstOrDefault((BadgeRecipe recipe) => recipe.CanBeBuiltWith(components));
			if (badgeRecipe != null)
			{
				string localizationForResult = GetLocalizationForResult(badgeRecipe);
				int chanceToCraftRecipe = GameManager.Instance.playerModel.LootManager.GetChanceToCraftRecipe(components);
				string colorForChance = GetColorForChance(chanceToCraftRecipe);
				HelpersUI.SetContentToLabel(effectChanceLabel, LocalizationManager.GetText("Residence.ResultEffect{ChanceColor}{Percentage}{BadgeEffect}", colorForChance, chanceToCraftRecipe, localizationForResult));
				
				if (!OfflineManager.IsNoEffects)
				{
					tweenScale.ResetToBeginning();
					tweenScale.PlayForward();
				}
			}
			else
			{
				HelpersUI.SetContentToLabel(effectChanceLabel, LocalizationManager.GetText("Residence.ResultEffectNone"));
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(rarityLabel, LocalizationManager.GetText("Residence.FillAllSlots"));
			HelpersUI.SetContentToLabel(effectChanceLabel, LocalizationManager.GetText("Residence.ResultEffectNone"));
		}
	}

	public static string GetLocalizationForResult(BadgeRecipe badgeRecipe)
	{
		switch (badgeRecipe.Results.Split(',')[0])
		{
		case "CritChance":
			return LocalizationManager.GetText("BadgeEffects.CritChance.Title");
		case "CritDamage":
		case "FlatCritDamage":
			return LocalizationManager.GetText("BadgeEffects.CritDamage.Title");
		case "Damage":
		case "FlatDamage":
			return LocalizationManager.GetText("BadgeEffects.Damage.Title");
		case "Health":
		case "FlatHealth":
			return LocalizationManager.GetText("BadgeEffects.Health.Title");
		case "DamageReduction":
			return LocalizationManager.GetText("BadgeEffects.DamageReduction.Title");
		default:
			return string.Empty;
		}
	}

	private string GetColorForChance(int chance)
	{
		if (chance < 50)
		{
			return Color50;
		}
		if (chance < 75)
		{
			return Color75;
		}
		if (chance < 100)
		{
			return Color99;
		}
		return Color100;
	}

	public void Clear()
	{
		if (craftButton != null)
		{
			craftButton.Clear();
		}
		if (autoFillButton != null)
		{
			autoFillButton.Clear();
		}
		costCashier = null;
		UIEvent.OnUIEvent -= OnUIEvent;
		HelpersUI.TryClearListOf(ref componentButtonList);
		HelpersUI.TryClearListOf(ref slotButtonList);
	}

	private void OnClickCraft(UIButtonExtended button)
	{
		if (GetTotalBadgesOwned(GameManager.Instance.playerModel) >= GameManager.Instance.playerModel.SurvivorContainer.MaximumBadgeCount)
		{
			HUDNotification.Error(LocalizationManager.GetText("Error.BadgeInventoryFull"));
			return;
		}
		if (IsSelectionValid())
		{
			if (IsLoadDataManager && button != null)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager && button != null)");
				DebugTWD.Log("OnClickCraft");
				GameEconomyData data = GameManager.Instance.gameEconomyData;
				currentRecipe = data.BadgeRecipes.FirstOrDefault((BadgeRecipe recipe) => recipe.CanBeBuiltWith(currentSelection.ToList()));

				if (CraftSettings.Instance.IsRealPlayerData)
				{
					var Currency = GameManager.Instance.playerModel.Currencies;
					int count = Currency.Count;

					for (int i = 0; i < currentSelection.Length; i++)
					{
						var currency = Currency.First(x => x.Type == currentSelection[i]);
						if (currency.Value - 1 >= 0) currency.Subtract(1);
						else
						{
							DebugTWD.Log("Не хватает компонентов");
							return;
						}
					}
				}
				else
				{
					var Currency = CraftSettings.Instance.Currency;
					int count = Currency.Count;

					for (int i = 0; i < currentSelection.Length; i++)
					{
						var currency = Currency.First(x => x.Type == currentSelection[i]);
						if (currency.Value - 1 >= 0)
						{
							currency.ChangeValue(-1);
						}
						else
						{
							DebugTWD.Log("Не хватает компонентов");
							return;
						}
					}
				}
				BadgeCraft.Instance.OnClickCraft();
			}
			else
			{
				costCashier = GameManager.Instance.playerModel.LootManager.GetBadgeCraftCashier(GetCompoentsAsList());
				CraftBadgeCommand craftBadgeCommand = new CraftBadgeCommand();
				craftBadgeCommand.Currencies.AddRange(currentSelection);
				craftBadgeCommand.Cashier = costCashier;
				if (button != null)
				{
					StartCoroutine(DisableButton(1f, button));
					ConsumeCurrencyCommandUtils.Execute(craftBadgeCommand, CraftComplete);
				}
			}
		}
		else
		{
			TriggerEmptyEffect();
		}
		UpdateUI();
	}

	private static int GetTotalBadgesOwned(PlayerModel player)
	{
		return player.Equipment.Badges.Count + player.SurvivorContainer.Survivors.Sum((SurvivorModel x) => x.BadgeContainer.Badges.Count);
	}

	private IEnumerator DisableButton(float timeDisabled, UIButton button)
	{
		HelpersUI.SetButtonState(button, UIButtonColor.State.Disabled);
		yield return new WaitForSeconds(timeDisabled);
		HelpersUI.SetButtonState(button, UIButtonColor.State.Normal);
	}

	private void TriggerPostCraftEffect()
	{
		if (componentButtonList == null)
		{
			return;
		}
		for (int i = 0; i < componentButtonList.Count; i++)
		{
			if (componentButtonList[i] != null)
			{
				componentButtonList[i].TriggerPostCraftEffect();
			}
		}
	}

	private void TriggerEmptyEffect()
	{
		HUDNotification.Error(LocalizationManager.GetText("Warning.EmptySlots"));
		if (slotButtonList == null || componentButtonList == null || slotButtonList.Count != componentButtonList.Count || currentSelection == null)
		{
			return;
		}
		for (int i = 0; i < slotButtonList.Count; i++)
		{
			if (currentSelection.Length > i && !ValidateSlotCurrency(currentSelection[i]))
			{
				if (slotButtonList[i] != null)
				{
					slotButtonList[i].TriggerEmptyEffect();
				}
				if (componentButtonList[i] != null)
				{
					componentButtonList[i].TriggerEmptyEffect();
				}
			}
		}
	}

	private void CraftComplete(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			GameManager.Instance.CheckConnectionReachability(showPopup: true, "CraftBadgeCommand");
			TriggerPostCraftEffect();
			Invoke("ShowCraftedBadge", showCraftResultDelay);
		}
		UpdateUI();
	}

	private void ShowCraftedBadge()
	{
		if (GameManager.Instance.playerModel.LastCraftedBadge != null)
		{
			BadgeReceivePopup.OpenForBadge(GameManager.Instance.playerModel.LastCraftedBadge);
		}
	}

	private void OnClickSlot(UIButtonExtended button)
	{
		if (button != null && button.gameObject != null)
		{
			int result = -1;
			int.TryParse(button.id, out result);
			TooltipManager.Prefabs prefabEnum = ((result == 0) ? TooltipManager.Prefabs.TooltipComponentSelectorSmall : TooltipManager.Prefabs.TooltipComponentSelectorLarge);
			if (IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				GameObject tooltip = prefabEnum == TooltipManager.Prefabs.TooltipComponentSelectorSmall ? ToolTipSmall : ToolTipLarge;
				tooltip.SetActive(true);
				TooltipManager.OpenForComponentSlot(button.gameObject, result, tooltip, currentSelection.ToList());
			}
			else
			{
				TooltipManager.OpenForComponentSlot(button.gameObject, result, prefabEnum, currentSelection.ToList());
			}
			UpdateUI();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
		}
	}

	private void SortComponents(List<CurrencyModel> list, Dictionary<CurrencyType, int> currencyAllocated)
	{
		Helpers.RandomShuffle(list);
		list.StableSort(delegate(CurrencyModel a, CurrencyModel b)
		{
			int componentRarityLevel = ComponentHelper.GetComponentRarityLevel(a.Type);
			int num = ComponentHelper.GetComponentRarityLevel(b.Type) - componentRarityLevel;
			if (num == 0)
			{
				int num2 = a.Value;
				if (currencyAllocated != null && currencyAllocated.ContainsKey(a.Type))
				{
					num2 -= currencyAllocated[a.Type];
				}
				int num3 = b.Value;
				if (currencyAllocated != null && currencyAllocated.ContainsKey(b.Type))
				{
					num3 -= currencyAllocated[b.Type];
				}
				num = num3 - num2;
			}
			return num;
		});
	}

	private void OnClickAutofill(UIButtonExtended button)
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("OnClickAutofill. Ignore");
			CraftSettings.Instance.SetComponents();
			return;
		}
		List<CurrencyModel> listRef = new List<CurrencyModel>();
		TooltipComponentSelector.GetAllComponentModelsList(GameManager.Instance.playerModel, ref listRef, TooltipComponentSelector.FilterType.Exclude, CurrencyType.Badge0, 1);
		Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
		for (int i = 1; i < currentSelection.Length; i++)
		{
			if (listRef.Count > 0)
			{
				SortComponents(listRef, dictionary);
				CurrencyType type = listRef[0].Type;
				currentSelection[i] = type;
				if (dictionary.ContainsKey(type))
				{
					dictionary[type]++;
				}
				else
				{
					dictionary[type] = 1;
				}
				if (listRef[0].Value <= dictionary[type])
				{
					listRef.RemoveAt(0);
				}
			}
		}
		TooltipComponentSelector.GetAllComponentModelsList(GameManager.Instance.playerModel, ref listRef, TooltipComponentSelector.FilterType.Only, CurrencyType.Badge0, 1);
		SortComponents(listRef, null);
		if (listRef.Count > 0)
		{
			currentSelection[0] = listRef[0].Type;
		}
		UpdateUI();
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnClickComponentSelected" && parameter != null && parameter is SelectComponentEvent selectComponentEvent)
		{
			if (selectComponentEvent.index < currentSelection.Length && selectComponentEvent.model != null && HasTheCurrency(selectComponentEvent.model.Type, 1))
			{
				currentSelection[selectComponentEvent.index] = selectComponentEvent.model.Type;
			}
			UpdateUI();
		}
		else if (type == "OnResidenceClosed" && currentSelection != null)
		{
			for (int i = 0; i < currentSelection.Length; i++)
			{
				currentSelection[i] = CurrencyType.None;
			}
		}
	}

	private bool HasTheCurrency(CurrencyType currencyType, int amount)
	{
		if (IsLoadDataManager && !CraftSettings.Instance.IsRealPlayerData)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && !CraftSettings.Instance.IsRealPlayerData)");
			var currencyModel = CraftSettings.Instance.Currency.FirstOrDefault(x => x.Type == currencyType);
			if (currencyModel != null) return currencyModel.Value >= amount;
			return false;
		}
		else
		{
			return GameManager.Instance.playerModel.GetCurrency(currencyType).Value >= amount;
		}
	}

	private bool ValidateSlotCurrency(CurrencyType currencyType)
	{
		int amount = currentSelection.Where((CurrencyType x) => x == currencyType).Count();
		if (currencyType == CurrencyType.None || !HasTheCurrency(currencyType, amount))
		{
			return false;
		}
		return true;
	}

	private bool IsSelectionValid()
	{
		if (currentSelection != null)
		{
			for (int i = 0; i < currentSelection.Length; i++)
			{
				if (!ValidateSlotCurrency(currentSelection[i]))
				{
					return false;
				}
			}
		}
		return true;
	}

	public List<CurrencyType> GetCompoentsAsList(bool includeNone = false)
	{
		List<CurrencyType> list = new List<CurrencyType>();
		for (int i = 0; i < currentSelection.Length; i++)
		{
			if (includeNone || currentSelection[i] != CurrencyType.None)
			{
				list.Add(currentSelection[i]);
			}
		}
		return list;
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public GameObject ToolTipSmall;
	public GameObject ToolTipLarge;
	public GameObject DropRatesInfoPopup;
	public GameObject ComponentsPanel;

	[SerializeField]
	private UIButtonWithLabelAndIcon DeleteButton;
	private BadgeRecipe currentRecipe;

	public string GetPlannedRecipeResult()
	{
		return currentRecipe != null ? currentRecipe.Results.Split(',').FirstOrDefault() : string.Empty;
	}
	#endregion

	#region mycode
	private IEnumerator WaitForPlayer()
	{
		float startTime = Time.realtimeSinceStartup;
		while (!OfflineManager.Instance.IsPlayerLoaded)
		{
			if (Time.realtimeSinceStartup - startTime > 20f)
			{
				DebugTWD.LogWarning("Can't load player");
				yield break;
			}
			yield return null;
		}
		UpdateUI();
		HelpersUI.SetButtonState(craftButton, UIButtonColor.State.Normal);
		UIEvent.OnUIEvent += OnUIEvent;
		if (craftButton != null)
		{
			craftButton.SetClickCallback(OnClickCraft);
		}
		if (autoFillButton != null)
		{
			autoFillButton.SetClickCallback(OnClickAutofill);
		}
	}

	public void SetCraftComponents(List<CurrencyType> currency)
	{
		for (int i = 0; i < currentSelection.Length; i++)
		{
			currentSelection[i] = currency[i];
		}
		UpdateUI();
	}

	public void SetContentToCraftButton(string text)
	{
		craftButton.SetContentToLabelOne(text);
	}

	public void ShowComponentsTab(UIButtonToggle tg)
	{
		ComponentsPanel.SetActive(tg.IsToggled);
	}
	#endregion
}
