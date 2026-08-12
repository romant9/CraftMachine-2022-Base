using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;
using static CombatHUD;
using static PhoneWeaponContainer;
using static TWDModel.EquipPrizeWheelModel;

public class RadioWeaponCard : MonoBehaviourExtended
{
	[SerializeField]
	private UIButtonExtended mainButton;

	[SerializeField]
	private UILabel rewardAmountLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UISprite timedRewardIcon;

	[SerializeField]
	private GameObject randomEquipmentButtonPrefab;

	[SerializeField]
	private GameObject equipmentButtonPrefab;

	[SerializeField]
	private GameObject equipmentTokenButtonPrefab;

	[SerializeField]
	private GameObject currencyParent;

	[SerializeField]
	private GameObject equipmentParent;

	[SerializeField]
	private GameObject equipmentTokenParent;

	[SerializeField]
	private GameObject consumableParent;

	[SerializeField]
	private GameObject timedRewardParent;

	[SerializeField]
	private GameObject skillParent;

	[SerializeField]
	private UISprite skillIcon;

	[SerializeField]
	private UISprite skillBgIcon;

	[SerializeField]
	private UISprite skillClassIcon;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private GameObject skillNewObj;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private UILabel consumableAmount;

	[SerializeField]
	private GameObject skillCurrencyParent;

	[SerializeField]
	private UISprite skillCurrencyBg;

	[SerializeField]
	private UISprite skillCurrencyIcon;

	[SerializeField]
	private UILabel skillCurrencyLabel;

	[SerializeField]
	public Vector3 equipmentCardScale = new Vector3(0.5f, 0.5f, 1f);

	private UIWidget widgetCached;

	private IReward _reward;

	public UIWidget widget => widgetCached;

	private void Awake()
	{
		DebugIdString = "WeaponCard";
		widgetCached = GetComponent<UIWidget>();
	}

	public void Init(IReward reward)
	{
		_reward = reward;
	}

	public IReward GetCurrentReward()
	{
		return _reward;
	}

	public void UpdateUI()
	{
		if (_reward != null)
		{
			if (equipmentParent != null)
			{
				Helpers.DestroyAllChildren(equipmentParent);
			}
			if (equipmentTokenParent != null)
			{
				Helpers.DestroyAllChildren(equipmentTokenParent);
			}
			Helpers.GameObjectSetActive(currencyParent, value: false);
			Helpers.GameObjectSetActive(equipmentParent, value: false);
			Helpers.GameObjectSetActive(equipmentTokenParent, value: false);
			Helpers.GameObjectSetActive(consumableParent, value: false);
			Helpers.GameObjectSetActive(timedRewardParent, value: false);
			if (skillParent != null)
			{
				Helpers.GameObjectSetActive(skillParent, value: false);
			}
			if (skillCurrencyParent != null)
			{
				Helpers.GameObjectSetActive(skillCurrencyParent, value: false);
			}
			CreateEquipmentCardsAndSetActive();
		}
	}

	public void ShowRevealTeaserOnly()
	{
		if (_reward == null)
		{
			return;
		}
		if (equipmentParent != null)
		{
			Helpers.DestroyAllChildren(equipmentParent);
		}
		if (equipmentTokenParent != null)
		{
			Helpers.DestroyAllChildren(equipmentTokenParent);
		}
		Helpers.GameObjectSetActive(currencyParent, value: false);
		Helpers.GameObjectSetActive(equipmentParent, value: false);
		Helpers.GameObjectSetActive(equipmentTokenParent, value: false);
		Helpers.GameObjectSetActive(consumableParent, value: false);
		Helpers.GameObjectSetActive(timedRewardParent, value: false);
		if (skillParent != null)
		{
			Helpers.GameObjectSetActive(skillParent, value: false);
		}
		if (skillCurrencyParent != null)
		{
			Helpers.GameObjectSetActive(skillCurrencyParent, value: false);
		}
		if (_reward is RewardCurrency { CurrencyType: var currencyType } rewardCurrency)
		{
			if (currencyType.ToString().Contains("SkillToken"))
			{
				SPTraitsSkillKitTokenSet sPTraitsSkillKitTokenSetByID = GameManager.Instance.playerModel.gameEconomyData.GetSPTraitsSkillKitTokenSetByID(rewardCurrency.CurrencyType.ToString());
				if (sPTraitsSkillKitTokenSetByID != null)
				{
					Helpers.GameObjectSetActive(skillCurrencyParent, value: true);
					HelpersUI.SetTraitsIconOnSprite(skillCurrencyIcon, sPTraitsSkillKitTokenSetByID.TopIcon, sPTraitsSkillKitTokenSetByID.TopIconOnCloud);
					HelpersUI.SetSprite(skillCurrencyBg, sPTraitsSkillKitTokenSetByID.BGIcon);
					HelpersUI.SetContentToLabel(skillCurrencyLabel, HelpersGfx.GetAmountForIReward(_reward).ToString());
				}
			}
			else
			{
				Helpers.GameObjectSetActive(currencyParent, value: true);
				HelpersGfx.GetIconNameForIReward(_reward, out var spriteName, null, null, null);
				HelpersUI.SetSprite(rewardIcon, spriteName);
			}
			if (rewardAmountLabel != null)
			{
				HelpersUI.SetContentToLabel(rewardAmountLabel, HelpersGfx.GetAmountForIReward(_reward).ToString());
			}
		}
		else if (_reward is RewardTimedBonus rewardTimedBonus)
		{
			Helpers.GameObjectSetActive(timedRewardParent, value: true);
			HelpersUI.SetSprite(timedRewardIcon, HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus));
		}
		else if (_reward is RewardRandomEquipment)
		{
			Helpers.GameObjectSetActive(currencyParent, value: true);
			HelpersGfx.GetIconNameForIReward(_reward, out var spriteName2, null, null, null);
			HelpersUI.SetSprite(rewardIcon, spriteName2);
			if (rewardAmountLabel != null)
			{
				rewardAmountLabel.text = "";
			}
		}
		else if (_reward is RewardEquipment rewardEquipment)
		{
			Helpers.GameObjectSetActive(consumableParent, value: true);
			consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
			bool flag = rewardEquipment.Amount > 1;
			if (consumableAmount != null)
			{
				consumableAmount.text = (flag ? rewardEquipment.Amount.ToString() : "");
				Helpers.GameObjectSetActive(consumableAmount.gameObject, flag);
			}
			UIButtonExtended component = consumableParent.GetComponent<UIButtonExtended>();
			if (component != null)
			{
				component.Clear();
			}
		}
		else if (_reward is RewardEquipToken rewardEquipToken)
		{
			EquipTokenDefinition equipTokenDefinition = GameManager.Instance.gameEconomyData.GetEquipTokenDefinition(rewardEquipToken.EquipTokenId);
			if (equipTokenDefinition != null && consumableTexture != null)
			{
				Helpers.GameObjectSetActive(consumableParent, value: true);
				consumableTexture.mainTexture = HelpersGfx.GetEquipmentTokenIconTexture(equipTokenDefinition);
				bool flag2 = rewardEquipToken.RewardAmount > 1;
				if (consumableAmount != null)
				{
					consumableAmount.text = (flag2 ? rewardEquipToken.RewardAmount.ToString() : "");
					Helpers.GameObjectSetActive(consumableAmount.gameObject, flag2);
				}
				UIButtonExtended component2 = consumableParent.GetComponent<UIButtonExtended>();
				if (component2 != null)
				{
					component2.Clear();
				}
			}
		}
		else if (_reward is RewardRemoldSkill rewardRemoldSkill)
		{
			ShowRemoldSkillTeaser(rewardRemoldSkill);
		}
	}

	private static bool ShouldShowRemoldSkillNewBadge(RewardRemoldSkill rewardRemoldSkill)
	{
		return (rewardRemoldSkill?.GivenRewardResult)?.IsNewAcquisition ?? false;
	}

	private void ApplyRemoldSkillNewBadgeFromResult(RewardRemoldSkill rewardRemoldSkill)
	{
		if (!(skillNewObj == null))
		{
			Helpers.GameObjectSetActive(skillNewObj, ShouldShowRemoldSkillNewBadge(rewardRemoldSkill));
		}
	}

	private void ShowRemoldSkillTeaser(RewardRemoldSkill rewardRemoldSkill)
	{
		SPTraitsRemoldDefinitions minRemoldDefinitionForGroup = Helpers.GetMinRemoldDefinitionForGroup(rewardRemoldSkill.SpRemoldSkillType);
		if (minRemoldDefinitionForGroup != null && !(skillParent == null) && !(skillIcon == null))
		{
			Helpers.GameObjectSetActive(skillParent, value: true);
			HelpersUI.SetTraitsIconOnSprite(skillIcon, minRemoldDefinitionForGroup.SPTraitsIcon, minRemoldDefinitionForGroup.SPTraitsIconOnCloud);
			skillClassIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(minRemoldDefinitionForGroup.AvailableClass);
			skillBgIcon.color = Helpers.HexToColor(minRemoldDefinitionForGroup.Color);
			starList.Setup(minRemoldDefinitionForGroup.Star);
			ApplyRemoldSkillNewBadgeFromResult(rewardRemoldSkill);
		}
	}

	private void CreateEquipmentCardsAndSetActive()
	{
		if (_reward == null)
		{
			return;
		}
		if (_reward is RewardCurrency { CurrencyType: var currencyType } rewardCurrency)
		{
			if (rewardCurrency.CurrencyType == CurrencyType.EquipTraitsRemodelToken)
			{
				currencyParent.transform.localScale = Vector3.one * 1.3f;
				ChangeCounterStatePhone(PrizeCounterType.RemodelPart);
			}

			if (rewardCurrency.CurrencyType == CurrencyType.ApocalypticEquipToken)
			{
				currencyParent.transform.localScale = Vector3.one * 1.3f;
				ChangeCounterStatePhone(PrizeCounterType.ApoPart);
			}

			if (rewardCurrency.CurrencyType == CurrencyType.BulePrintToken)
			{
				if (rewardCurrency.Amount < 50)
				{
					currencyParent.transform.localScale = Vector3.one * 1.2f;
					ChangeCounterStatePhone(PrizeCounterType.TokenPart); //
				}
				else
				{
					DebugTWD.Log("Reward is BulePrintToken : " + rewardCurrency.Amount, DebugType.Call);
					currencyParent.transform.localScale = Vector3.one * 1.4f;
					ChangeCounterStatePhone(PrizeCounterType.Token); //
				}
			}
			if (currencyType.ToString().Contains("SkillToken"))
			{
				SPTraitsSkillKitTokenSet sPTraitsSkillKitTokenSetByID = GameManager.Instance.playerModel.gameEconomyData.GetSPTraitsSkillKitTokenSetByID(rewardCurrency.CurrencyType.ToString());
				if (sPTraitsSkillKitTokenSetByID != null)
				{
					Helpers.GameObjectSetActive(skillCurrencyParent, value: true);
					HelpersUI.SetTraitsIconOnSprite(skillCurrencyIcon, sPTraitsSkillKitTokenSetByID.TopIcon, sPTraitsSkillKitTokenSetByID.TopIconOnCloud);
					HelpersUI.SetSprite(skillCurrencyBg, sPTraitsSkillKitTokenSetByID.BGIcon);
					HelpersUI.SetContentToLabel(skillCurrencyLabel, HelpersGfx.GetAmountForIReward(_reward).ToString());

					//часть skill token
					DebugTWD.Log("Reward is SkillToken : " + sPTraitsSkillKitTokenSetByID.ID, DebugType.Call);
					ChangeCounterStateGold(rewardCurrency.CurrencyType.ToString());
				}
			}
			else
			{
				Helpers.GameObjectSetActive(currencyParent, value: true);
				HelpersGfx.GetIconNameForIReward(_reward, out var spriteName, null, null, null);
				HelpersUI.SetSprite(rewardIcon, spriteName);
			}
			HelpersUI.SetContentToLabel(rewardAmountLabel, HelpersGfx.GetAmountForIReward(_reward).ToString());
		}
		else if (_reward is RewardTimedBonus rewardTimedBonus)
		{
			Helpers.GameObjectSetActive(timedRewardParent, value: true);
			HelpersUI.SetSprite(timedRewardIcon, HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus));
			UIButtonExtended component = timedRewardParent.GetComponent<UIButtonExtended>();
			component.Clear();
			component.SetClickCallback(delegate
			{
				TooltipManager.OpenTextBoxWithText(timedRewardParent, HelpersLocalization.GetShopTooltipForIReward(_reward));
			});
		}
		else if (_reward is RewardRandomEquipment && randomEquipmentButtonPrefab != null)
		{
			Helpers.GameObjectSetActive(equipmentParent, value: true);
			GameObject gameObject = Helpers.InstantiateToParent(randomEquipmentButtonPrefab, equipmentParent);
			if (gameObject != null)
			{
				gameObject.transform.localScale = equipmentCardScale;
				EquipmentRandomButton component2 = gameObject.GetComponent<EquipmentRandomButton>();
				if (component2 != null)
				{
					component2.Setup((RewardRandomEquipment)_reward);
				}
			}
		}
		else if (_reward is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
		{
			Helpers.GameObjectSetActive(consumableParent, value: true);
			consumableAmount.text = rewardEquipment.Amount.ToString();
			consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
			UIButtonExtended component3 = consumableParent.GetComponent<UIButtonExtended>();
			component3.Clear();
			component3.SetClickCallback(delegate
			{
				TooltipManager.OpenTextBoxWithText(consumableParent, HelpersLocalization.GetShopTooltipForIReward(_reward));
			});
		}
		else if (_reward is RewardEquipment && equipmentButtonPrefab != null)
		{
			Helpers.GameObjectSetActive(equipmentParent, value: true);
			GameObject gameObject2 = Helpers.InstantiateToParent(equipmentButtonPrefab, equipmentParent);
			if (gameObject2 != null)
			{
				gameObject2.transform.localScale = equipmentCardScale;
				EquipmentButton component4 = gameObject2.GetComponent<EquipmentButton>();
				if (component4 != null)
				{
					component4.Setup((RewardEquipment)_reward);

					if (IsLoadDataManager)
					{
						var name = ((RewardEquipment)_reward).EquipmentId;

						//Снаряжение
						//Weapon_ - оружие, Armor_ - броня
						//bool isWeapon = false;
						//if (name.StartsWith("Weapon_"))
						//{
						//	isWeapon = true;
						//}

                        if (!name.EndsWith("_chouka"))
						{
							DebugTWD.Log("Reward is RewardEquipment : " + name, DebugType.Call);
							equipmentParent.transform.localScale = Vector3.one * 1.3f;
							ChangeCounterStatePhone(PrizeCounterType.TokenPart);
						}
					}
				}
			}
		}
		else if (_reward is RewardEquipToken && equipmentTokenButtonPrefab != null)
		{
			Helpers.GameObjectSetActive(equipmentTokenParent, value: true);
			GameObject gameObject3 = Helpers.InstantiateToParent(equipmentTokenButtonPrefab, equipmentTokenParent);
			if (gameObject3 != null)
			{
				gameObject3.transform.localScale = equipmentCardScale;
				EquipmentTokenButton component5 = gameObject3.GetComponent<EquipmentTokenButton>();
				if (component5 != null)
				{
					if (IsLoadDataManager)
					{
						//токен оружия
						DebugTWD.Log("Reward is RewardEquipToken : " + ((RewardEquipToken)_reward).EquipTokenId, DebugType.Call);
						equipmentTokenParent.transform.localScale = Vector3.one * 1.5f;
						ChangeCounterStatePhone(PrizeCounterType.Token); //
					}
					component5.SetUpForCampaign((RewardEquipToken)_reward);
				}
			}
		}
		else if (_reward is RewardRemoldSkill rewardRemoldSkill)
		{
			CreateRemoldSkillRewardDisplay(rewardRemoldSkill);
		}
	}

	private void CreateRemoldSkillRewardDisplay(RewardRemoldSkill rewardRemoldSkill)
	{
		if (rewardRemoldSkill.GivenRewardResult == null)
		{
			rewardRemoldSkill.Give(GameManager.Instance.modelManager);
		}
		if (rewardRemoldSkill.GivenRewardResult is ModSkillRewardResult modSkillRewardResult)
		{
			if (modSkillRewardResult.RewardType == ModSkillRewardType.NewAcquisition && modSkillRewardResult.ModSkillMode != null)
			{
				ShowRemoldSkillNewAcquiredFullUI(modSkillRewardResult.ModSkillMode, rewardRemoldSkill);
			}
			else if (modSkillRewardResult.RewardType == ModSkillRewardType.Duplicate && modSkillRewardResult.DuplicateRewards != null)
			{
				ShowRemoldSkillDuplicateTokensUI(modSkillRewardResult.DuplicateRewards, rewardRemoldSkill);
			}

			//дорогой токен навыков переделки (часть)
			DebugTWD.Log("Reward is RewardEquipToken : " + rewardRemoldSkill.Type + " " + rewardRemoldSkill.SpRemoldSkillType, DebugType.Call); //RemoldSkill Hunter_4002
			//int remoldCounter = phoneWeaponContainer.remoldCounter;
			//int remoldCounterNearest = phoneWeaponContainer.remoldCounterNearest;
			//SetCounterValueForLabel(phoneWeaponContainer.remoldCounterLabel, ref remoldCounter, ref remoldCounterNearest, out int index);
			//phoneWeaponContainer.remoldCounterNearestList.Add(index);
			//phoneWeaponContainer.remoldCounter = remoldCounter;
			//phoneWeaponContainer.remoldCounterNearest = remoldCounterNearest;
			//UIEvent.Send("Reward_Remold");

			ChangeCounterStateGold(rewardRemoldSkill.SpRemoldSkillType);
		}
	}

	private void ShowRemoldSkillNewAcquiredFullUI(ModSkillMode modSkillMode, RewardRemoldSkill rewardRemoldSkill)
	{
		if (!(skillParent == null) && !(skillIcon == null))
		{
			Helpers.GameObjectSetActive(skillParent, value: true);
			SPTraitsRemoldDefinitions spTraitsDefaultTrait = modSkillMode.GetSpTraitsDefaultTrait();
			if (spTraitsDefaultTrait != null)
			{
				HelpersUI.SetTraitsIconOnSprite(skillIcon, spTraitsDefaultTrait.SPTraitsIcon, spTraitsDefaultTrait.SPTraitsIconOnCloud);
				skillClassIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(spTraitsDefaultTrait.AvailableClass);
				skillBgIcon.color = Helpers.HexToColor(spTraitsDefaultTrait.Color);
				starList.Setup(spTraitsDefaultTrait.Star);
			}
			ApplyRemoldSkillNewBadgeFromResult(rewardRemoldSkill);
		}
	}

	private void ShowRemoldSkillDuplicateTokensUI(Rewards rewards, RewardRemoldSkill rewardRemoldSkill)
	{
		if (rewards?.RewardsList == null || rewards.RewardsList.Count == 0)
		{
			return;
		}
		List<RewardCurrency> list = rewards.RewardsList.OfType<RewardCurrency>().ToList();
		if (list.Count == 0)
		{
			return;
		}
		ApplyRemoldSkillNewBadgeFromResult(rewardRemoldSkill);
		RewardCurrency rewardCurrency = list[0];
		if (rewardCurrency.CurrencyType.ToString().Contains("SkillToken"))
		{
			SPTraitsSkillKitTokenSet sPTraitsSkillKitTokenSetByID = GameManager.Instance.playerModel.gameEconomyData.GetSPTraitsSkillKitTokenSetByID(rewardCurrency.CurrencyType.ToString());
			if (sPTraitsSkillKitTokenSetByID != null)
			{
				Helpers.GameObjectSetActive(skillCurrencyParent, value: true);
				HelpersUI.SetTraitsIconOnSprite(skillCurrencyIcon, sPTraitsSkillKitTokenSetByID.TopIcon, sPTraitsSkillKitTokenSetByID.TopIconOnCloud);
				HelpersUI.SetSprite(skillCurrencyBg, sPTraitsSkillKitTokenSetByID.BGIcon);
				HelpersUI.SetContentToLabel(skillCurrencyLabel, rewardCurrency.Amount.ToString());
			}
		}
		else
		{
			Helpers.GameObjectSetActive(currencyParent, value: true);
			string currencyIconName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			HelpersUI.SetSprite(rewardIcon, currencyIconName);
		}
		if (rewardAmountLabel != null)
		{
			HelpersUI.SetContentToLabel(rewardAmountLabel, rewardCurrency.Amount.ToString());
		}
	}

	public void SetSeleted(bool value)
	{
	}

	public void CollectCard(bool allowShowingUnlockButton)
	{
	}

	public UIButtonExtended GetButton()
	{
		return mainButton;
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private PhoneWeaponContainer phoneWeaponContainer => NewPhonePopup.Instance != null ? NewPhonePopup.Instance.PhoneWeaponContainer : null;

	private GoldRadioWeaponContainer goldRadioWeaponContainer => NewPhonePopup.Instance != null ? NewPhonePopup.Instance.GoldRadioWeaponContainer : null;

	private SelectWeaponsPopup selectWeaponPopup 
	{
		get 
		{
			if (!NewPhonePopup.Instance) return null;

			if (NewPhonePopup.Instance.IsWeaponSkillMode)
			{
				return goldRadioWeaponContainer != null ? goldRadioWeaponContainer.selectWeaponsPopupCurrent : null;
			}
			else
			{
				return phoneWeaponContainer != null ? phoneWeaponContainer.SelectWeaponsPopupCurrent : null;
			}
		}
	}
	#endregion

	#region mycode
	public void SetCounterValueForLabel(UILabel label, ref int counterValue, ref int nearestvalue, out int index)
	{
		var prizeCounter = NewPhonePopup.Instance.IsWeaponSkillMode ? goldRadioWeaponContainer.playerPrizeSkillCounter : phoneWeaponContainer.playerPrizeCounter;
		var prizePass = NewPhonePopup.Instance.IsWeaponSkillMode ? goldRadioWeaponContainer.equipPrizeType == EquipPrizeType.One ? 0 : 9 : phoneWeaponContainer.EquipPrizeType == EquipPrizeType.One ? 0 : 9;
		index = selectWeaponPopup.CardsListCount + prizeCounter - prizePass;
		// странный костыль
		//if (phoneWeaponContainer != null && !phoneWeaponContainer.IsNoCountOffset)
		//{
		//	index -= 10;
		//}
		counterValue++;

		if (index > nearestvalue && nearestvalue == 0)
		{
			nearestvalue = index;
		}
		string secondIndex = "";
		if (label.name == "tokenCounterLabel")
		{
			if (nearestvalue != 0) secondIndex = ", " + nearestvalue;
		}

		label.text = counterValue + " (" + nearestvalue + secondIndex + ")";
	}

	public void ChangeCounterStatePhone(PrizeCounterType prizeType)
	{
		if (NewPhonePopup.Instance.IsWeaponSkillMode) return;
		var prizeIndex = (int)prizeType - 1;
		var counter = phoneWeaponContainer.AllCounters[prizeIndex];
		int counterValue = counter.CounterValue;
		int counterValueNearest = counter.CounterNearestValue;
		SetCounterValueForLabel(counter.CounterLabel, ref counterValue, ref counterValueNearest, out int index);
		counter.CounterNearestList.Add(index);
		counter.CounterValue = counterValue;
		counter.CounterNearestValue = counterValueNearest;
		phoneWeaponContainer.SetIsFineConstructFinded(prizeType);
	}

	private PrizeCounterType GetPrizeType(string prizeString, bool isSkill)
	{
		switch (prizeString)
		{
			case "Shooter":
				return isSkill ? PrizeCounterType.SkillShooter : PrizeCounterType.SkillShooterPart;
			case "Scout":
				return isSkill ? PrizeCounterType.SkillScout : PrizeCounterType.SkillScoutPart;
			case "Hunter":
				return isSkill ? PrizeCounterType.SkillHunter : PrizeCounterType.SkillHunterPart;
			case "Warrior":
				return isSkill ? PrizeCounterType.SkillWarrior : PrizeCounterType.SkillWarriorPart;
			case "Assault":
				return isSkill ? PrizeCounterType.SkillAssault : PrizeCounterType.SkillAssaultPart;
			case "Bruiser":
				return isSkill ? PrizeCounterType.SkillBruiser : PrizeCounterType.SkillBruiserPart;
			default:
				return PrizeCounterType.Any;
		}
	}

	public void ChangeCounterStateGold(string skillTypeString)
	{
		if (!NewPhonePopup.Instance.IsWeaponSkillMode) return;

		string classLevel;
		string classtwd;
		PrizeCounterType prizeType;
		var classString = skillTypeString.Split('_');
		bool isSkillPart = skillTypeString.StartsWith("SkillToken");

		if (isSkillPart)
		{
			classtwd = classString[1];
			classLevel = classString[2];
			prizeType = GetPrizeType(classtwd, false);
		}
		else
		{
			classtwd = classString[0];
			classLevel = classString[1];
			prizeType = GetPrizeType(classtwd, true);
		}

		var skillType = classtwd + "_" + classLevel;
		bool isBigStar = int.TryParse(classLevel[0].ToString(), out int levelResult) && levelResult >= goldRadioWeaponContainer.RewardStarValue;

		if (isBigStar)
		{
			var prizeIndex = (int)prizeType - 5;
			var counter = phoneWeaponContainer.AllCounters[prizeIndex];
			int counterValue = counter.CounterValue;
			int counterValueNearest = counter.CounterNearestValue;
			SetCounterValueForLabel(counter.CounterLabel, ref counterValue, ref counterValueNearest, out int index);
			counter.CounterNearestList.Add(index);
			counter.CounterValue = counterValue;
			counter.CounterNearestValue = counterValueNearest;
		}
		
		goldRadioWeaponContainer.SetIsFineConstructFinded(prizeType, skillType, isSkillPart, isBigStar);

		if (NewPhonePopup.Instance.FavoriteModSkillList.Contains(skillType))
		{
			var counterFav = isSkillPart ? goldRadioWeaponContainer.FavoriteCounterSkillPart : goldRadioWeaponContainer.FavoriteCounterSkill;
			int counterValueFav = counterFav.CounterValue;
			int counterValueNearestFav = counterFav.CounterNearestValue;
			SetCounterValueForLabel(counterFav.CounterLabel, ref counterValueFav, ref counterValueNearestFav, out int indexFav);
			counterFav.CounterNearestList.Add(indexFav);
			counterFav.CounterValue = counterValueFav;
			counterFav.CounterNearestValue = counterValueNearestFav;
		}
	}
	#endregion
}
