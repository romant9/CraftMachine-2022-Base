using NextGames.Sdk.AssetBundleManager;
using System;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class RadioCallButton : MonoBehaviourExtended
{
	[Header("References")]
	[SerializeField]
	private UILabel TitleLabel;

	[SerializeField]
	private UITexture BackgroundTexture;

	[SerializeField]
	private UITexture HeroTexture;

	[SerializeField]
	private UITexture RegularTexture;

	[SerializeField]
	private PayButton PayButton;

	[SerializeField]
	private TutorialArrowParent callButtonTutorialArrowParent;

	[SerializeField]
	private GameObject FreeCallButton;

	[SerializeField]
	private UIButtonExtended lockedButton;

	[SerializeField]
	private ThingsToDoIndicator FreeCallNumbers;

	[SerializeField]
	private UIButtonExtended ClickButton;

	[SerializeField]
	private UIButtonExtended ClickButtonFree;

	[SerializeField]
	private GameObject PayWithGoldParent;

	[SerializeField]
	private GameObject LockedParent;

	[SerializeField]
	private UILabel LockedLabel;

	[SerializeField]
	private GameObject[] StarsArray;

	[Header("Offers")]
	[SerializeField]
	private GameObject TimerContainer;

	[SerializeField]
	private UILabel TimerLabel;

	[SerializeField]
	private GameObject HeroTimerContainer;

	[SerializeField]
	private UILabel HeroTimerLabel;

	[SerializeField]
	private UISprite HeroTokenIcon;

	[SerializeField]
	private UILabel moreInfoTitleLabel;

	[SerializeField]
	private UILabel moreInfoTitleLabel2;

	[SerializeField]
	private UITexture cardSurvivorClassTexture;

	[Header("Tweens")]
	[SerializeField]
	private int PlayTweenGroupAtInit = 1;

	[Tooltip("Tween group will be called on the when the background images are loaded")]
	[SerializeField]
	private int BgImageLoadCompleteTweenGroup = 11;

	private UIWidget widget;

	private Cashier cashier;

	private DropType currentDropType = DropType.None;

	private bool PlayTweens = true;

	private SurvivorClass survivorClassNeeded = SurvivorClass.None;

	private bool hasRequiredSurvivorClass = true;

	private bool dropTypeUnlocked;

	private long timeToEndOffer;

	private DoubleBooleanState texturesLoaded;

	private readonly object readLock = new object();

	private bool loading;

	public int SlotNumber { get; set; }

	public PhoneCallDefinition PhoneCallDefinition { get; set; }

	public Vector2 localSize
	{
		get
		{
			if (widget != null)
			{
				return widget.localSize;
			}
			return Vector2.zero;
		}
	}

	public DropType dropType => currentDropType;

	public void Awake()
	{
		DebugIdString = "RadioCallButton";
		widget = GetComponent<UIWidget>();
	}

	public void OnEnable()
	{
		UpdateCashier();
	}

	public void SetData(int callSlotNumber, DropType dropType, PhoneCallDefinition phoneCallDefinition = null)
	{
		SlotNumber = callSlotNumber;
		if (IndexLabel) IndexLabel.text = SlotNumber.ToString();
		if (callButtonTutorialArrowParent != null)
		{
			callButtonTutorialArrowParent.Id = "Buy_Slot_" + SlotNumber;
		}
		currentDropType = dropType;
		PhoneCallDefinition = phoneCallDefinition;
		UpdateCashier();
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			GenerateRarityAmounts();
		}
	}

	public void OnDropRateClicked()
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.DropRatesInfoPopup))
		{
			return;
		}
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (modelManager == null)
		{
			return;
		}
		int radioTentLevel = GetRadioTentLevel();
		SpecialPhoneCallState callState = null;
		PhoneCallDefinition phoneCallDefinition = modelManager.GameEconomyData.GetPhoneCallDefinition(modelManager.Player.UtcTimeStamp, SlotNumber);
		if (phoneCallDefinition != null)
		{
			callState = modelManager.Player.LootManager.GetSpecialPhoneCallState(SlotNumber, phoneCallDefinition.EndTimeUtc);
		}
		RadioCallProbabilityData radioCallProbabilities = modelManager.GameEconomyData.GetRadioCallProbabilities(DropEventDefinition.DropEventType.RadioPhone, DropEventDefinition.DropEventContext.Normal, DropEventDefinition.DropEventTag.None, currentDropType, radioTentLevel, SlotNumber, modelManager.Player.UtcTimeStamp, callState);
		List<RadioCallTableItem> list = new List<RadioCallTableItem>();
		List<ItemAmountProbabilityData> probabilities = radioCallProbabilities.Probabilities;
		probabilities.Sort((ItemAmountProbabilityData a, ItemAmountProbabilityData b) => (a != null && b != null) ? (a.Rarity.CompareTo(b.Rarity) * -1) : 0);
		DropRatesNamesHelper.GetRadioCallNames(ref probabilities, DropEventDefinition.DropEventType.RadioPhone, currentDropType, DropEventDefinition.DropEventTag.None, radioTentLevel);
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && SurvivorRarityAmounts == null && HeroRarityAmounts == null)");
			GenerateRarityAmounts();
		}
		else
		{
			Dictionary<int, FixedPoint> equipmentAndSurvivorRarityProbabilities = modelManager.GameEconomyData.GetEquipmentAndSurvivorRarityProbabilities(currentDropType, DropRewardType.Survivor, radioTentLevel);
			List<ItemAmountProbabilityData> list2 = new List<ItemAmountProbabilityData>();
			foreach (KeyValuePair<int, FixedPoint> item in equipmentAndSurvivorRarityProbabilities)
			{
				list2.Add(new ItemAmountProbabilityData
				{
					Rarity = item.Key,
					Probability = item.Value
				});
			}
			List<ItemAmountProbabilityData> heroRarityAmounts = BuildHeroRarityAmountsForUi(modelManager.GameEconomyData, currentDropType, radioTentLevel, PhoneCallDefinition ?? radioCallProbabilities.CallDefinition);
			PhoneCallVisual data = GetData(PhoneCallDefinition);
			string text = LocalizationManager.GetText((data != null) ? data.LocalisationKey : "");
			if (PhoneCallDefinition != null && PhoneCallDefinition.SlotNumber > 2)
			{
				text = LocalizationManager.GetText("Droprate.Table.Name.SpecialCall");
			}
			list.Add(new RadioCallTableItem(radioCallProbabilities.CallDefinition)
			{
				DropName = text,
				Description = LocalizationManager.GetText("Droprate.Table.Description.RadioCall"),
				Probabilities = probabilities,
				SurvivorRarityAmounts = list2,
				HeroRarityAmounts = heroRarityAmounts,
				SpecialCallProbabilities = radioCallProbabilities.HighlightedProbabilities,
				GuarateedHero = radioCallProbabilities.GuaranteedHero
			});
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DropRatesInfoPopup) as DropRatesInfoPopup).TryOpenWithHeroData(list.ToArray());
		}
	}

	private int GetRadioTentLevel()
	{
		int result = 0;
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (modelManager != null && modelManager.CampModel != null)
		{
			BuildingModel building = modelManager.CampModel.GetBuilding("RadioTent");
			if (building != null)
			{
				result = building.Level;
			}
		}
		return result;
	}

	private List<ItemAmountProbabilityData> BuildHeroRarityAmountsForUi(GameEconomyData ged, DropType dropType, int controlLevel, PhoneCallDefinition callDefinition)
	{
		List<ItemAmountProbabilityData> list = new List<ItemAmountProbabilityData>();
		if (callDefinition != null && !string.IsNullOrEmpty(callDefinition.HeroTokensDropNumber))
		{
			bool parseError;
			List<int> hreoKensDropNumberValues = callDefinition.getHreoKensDropNumberValues(out parseError);
			if (!parseError && hreoKensDropNumberValues != null && hreoKensDropNumberValues.Count >= 3)
			{
				Dictionary<int, FixedPoint> equipmentAndSurvivorRarityProbabilities = ged.GetEquipmentAndSurvivorRarityProbabilities(dropType, DropRewardType.HeroToken, controlLevel);
				FixedPoint fixedPoint = 0L;
				FixedPoint fixedPoint2 = 0L;
				FixedPoint fixedPoint3 = 0L;
				foreach (KeyValuePair<int, FixedPoint> item in equipmentAndSurvivorRarityProbabilities)
				{
					if (item.Key == 2)
					{
						fixedPoint = item.Value;
					}
					else if (item.Key == 3)
					{
						fixedPoint2 = item.Value;
					}
					else if (item.Key >= 4)
					{
						fixedPoint3 += item.Value;
					}
				}
				if (fixedPoint > 0L)
				{
					list.Add(new ItemAmountProbabilityData
					{
						Amount = hreoKensDropNumberValues[0].ToString(),
						Probability = fixedPoint,
						Rarity = 2
					});
				}
				if (fixedPoint2 > 0L)
				{
					list.Add(new ItemAmountProbabilityData
					{
						Amount = hreoKensDropNumberValues[1].ToString(),
						Probability = fixedPoint2,
						Rarity = 3
					});
				}
				if (fixedPoint3 > 0L)
				{
					list.Add(new ItemAmountProbabilityData
					{
						Amount = hreoKensDropNumberValues[2].ToString(),
						Probability = fixedPoint3,
						Rarity = 4
					});
				}
				if (list.Count > 0)
				{
					return list;
				}
			}
		}
		foreach (KeyValuePair<int, FixedPoint> genericHeroRarityToTokenAmount in ged.GetGenericHeroRarityToTokenAmounts(dropType, controlLevel, DropEventDefinition.DropEventContext.Normal))
		{
			list.Add(new ItemAmountProbabilityData
			{
				Amount = genericHeroRarityToTokenAmount.Key.ToString(),
				Probability = genericHeroRarityToTokenAmount.Value
			});
		}
		return list;
	}

	private void UpdateCashier()
	{
		cashier = GameManager.Instance.playerModel.PhoneCall.GetCashier(currentDropType, SlotNumber);
	}

	public static bool IsSurvivorClassUnlockedAfter(GameEconomyData ged, SurvivorClass survClass, SurvivorClass after)
	{
		SurvivorClass[] array = ged.ConfigData.ParseSurvivorClassUnlockOrder();
		bool result = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == survClass)
			{
				return result;
			}
			if (array[i] == after)
			{
				result = true;
			}
		}
		DebugTWD.LogError("Called IsSurvivorClassUnlockedAfter for a survivor class parameter that was not in the unlock list, " + survClass.ToString() + ", " + after.ToString());
		return false;
	}

	private void LoadAndSetTextureLocal(UITexture texture, string name)
	{
		Texture mainTexture = Resources.Load<Texture>("Ui/" + name.Substring(name.IndexOf("LOCAL:") + "LOCAL:".Length));
		texture.mainTexture = mainTexture;
	}

	public void UpdateUI()
	{
		if (cashier == null)
		{
			return;
		}
		dropTypeUnlocked = GameManager.Instance.playerModel.PhoneCall.IsUnlocked(currentDropType);
		survivorClassNeeded = SurvivorClass.None;
		hasRequiredSurvivorClass = true;
		if (PhoneCallDefinition != null)
		{
			SurvivorContainerModel survivorContainer = GameManager.Instance.playerModel.SurvivorContainer;
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			CurrencyType[] parsedCurrencyTypeValues = PhoneCallDefinition.GetParsedCurrencyTypeValues();
			for (int i = 0; i < parsedCurrencyTypeValues.Length; i++)
			{
				if (parsedCurrencyTypeValues[i] != CurrencyType.None)
				{
					SurvivorClass heroSurvivorClass = survivorContainer.GetHeroSurvivorClass(parsedCurrencyTypeValues[i]);
					if (survivorClassNeeded == SurvivorClass.None || (heroSurvivorClass != SurvivorClass.None && IsSurvivorClassUnlockedAfter(gameEconomyData, heroSurvivorClass, survivorClassNeeded)))
					{
						survivorClassNeeded = heroSurvivorClass;
					}
				}
			}
			if (parsedCurrencyTypeValues.Length == 0 && PhoneCallDefinition.SurvivorClass != SurvivorClass.None && (survivorClassNeeded == SurvivorClass.None || IsSurvivorClassUnlockedAfter(gameEconomyData, PhoneCallDefinition.SurvivorClass, survivorClassNeeded)))
			{
				survivorClassNeeded = PhoneCallDefinition.SurvivorClass;
			}
			if (survivorClassNeeded != SurvivorClass.None && !survivorContainer.IsSurvivorClassUnlocked(survivorClassNeeded))
			{
				hasRequiredSurvivorClass = false;
			}
		}
		bool flag = GameManager.Instance.gameEconomyData.GetFeature("AllowLockedClassesOnRadio")?.Enabled ?? false;
		bool flag2 = dropTypeUnlocked && (hasRequiredSurvivorClass || flag);
		string text = "";
		bool flag3 = GameManager.Instance.playerModel.PhoneCall.HasFreeCall(SlotNumber);
		if (LockedParent != null && LockedLabel != null)
		{
			if (!flag2)
			{
				if (!dropTypeUnlocked)
				{
					int num = GameManager.Instance.gameEconomyData.ConfigData.PhoneSilverUnlockAtLevel;
					if (currentDropType == DropType.Gold)
					{
						num = GameManager.Instance.gameEconomyData.ConfigData.PhoneGoldUnlockAtLevel;
					}
					text = ((num != 1) ? LocalizationManager.GetText("Popup.StartPhoneCall.LockedCall{RadioLevel}", num) : LocalizationManager.GetText("Popup.StartPhoneCall.LockedCallBuildRadio"));
				}
				else if (!hasRequiredSurvivorClass && PhoneCallDefinition != null)
				{
					text = LocalizationManager.GetText("Popup.Quest.UnlockClass{ClassName}", HelpersLocalization.GetSurvivorClassName(survivorClassNeeded));
				}
				LockedLabel.text = text;
			}
			Helpers.GameObjectSetActive(LockedLabel.gameObject, !flag2);
			Helpers.GameObjectSetActive(LockedParent.gameObject, !flag2);
		}
		if (PayButton != null)
		{
			if (flag2)
			{
				PayButton.UpdateUI(cashier, LocalizationManager.GetText("Popup.StartPhoneCall.Button"));
			}
			Helpers.GameObjectSetActive(PayButton.gameObject, flag2 && !flag3);
		}
		if (FreeCallButton != null)
		{
			Helpers.GameObjectSetActive(FreeCallButton.gameObject, flag2 && flag3);
			if (flag3)
			{
				int freeCallStacked = GameManager.Instance.playerModel.PhoneCall.GetFreeCallStacked(SlotNumber);
				if (freeCallStacked > 1)
				{
					FreeCallNumbers.SetNumber(freeCallStacked);
				}
				else
				{
					FreeCallNumbers.SetNumber(0);
				}
			}
		}
		if (PayWithGoldParent != null)
		{
			bool active = !cashier.CanPay(CurrencyType.Phone);
			PayWithGoldParent.SetActive(active);
		}
		int num2 = -1;
		PhoneCallVisual data = GetData(PhoneCallDefinition);
		if (data != null)
		{
			DebugTWD.Log("Begin Setup Calls", DebugType.Call);
			if (BackgroundTexture != null && HeroTexture != null && !loading)
			{
				loading = true;
				BackgroundTexture.gameObject.SetActive(value: false);
				HeroTexture.gameObject.SetActive(value: false);
				RegularTexture.gameObject.SetActive(value: false);
				if (PhoneCallDefinition.SlotNumber < 3)
				{
					SetupRegularCall(PhoneCallDefinition.SlotNumber);
				}
				else
				{
					SetupDynamicCall(data);
				}
			}
			HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText(data.LocalisationKey));
			if (TitleLabel != null && ColorUtility.TryParseHtmlString(data.TitleColor, out var color))
			{
				TitleLabel.color = color;
			}
			if (moreInfoTitleLabel != null && ColorUtility.TryParseHtmlString(data.Title2Color, out var color2))
			{
				moreInfoTitleLabel.color = color2;
			}
			if (moreInfoTitleLabel2 != null && ColorUtility.TryParseHtmlString(data.Title2Color, out var color3))
			{
				moreInfoTitleLabel2.color = color3;
			}
			num2 = (int)currentDropType;
		}
		if (StarsArray != null)
		{
			for (int j = 0; j < StarsArray.Length; j++)
			{
				if (StarsArray[j] != null)
				{
					if (j == num2)
					{
						Helpers.GameObjectSetActive(StarsArray[j], value: true);
					}
					else
					{
						Helpers.GameObjectSetActive(StarsArray[j], value: false);
					}
				}
			}
		}
		if (PlayTweens && PlayTweenGroupAtInit > -1)
		{
			PlayTweens = false;
			TweenManager.PlayTweenGroup(base.gameObject, PlayTweenGroupAtInit);
		}
		if (TimerContainer != null && HeroTimerContainer != null)
		{
			if (PhoneCallDefinition == null)
			{
				long num3 = GameManager.Instance.playerModel.PhoneCall.MillisecondsTillFreeCall[SlotNumber];
				TimerContainer.SetActive(num3 > 0);
				HeroTimerContainer.SetActive(value: false);
				if (num3 > 0)
				{
					timeToEndOffer = num3;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(PhoneCallDefinition.EndTimeUtc))
				{
					timeToEndOffer = PhoneCallDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
				}
				if (PhoneCallDefinition.Type == PhoneCallDefinitionType.BetterChanceOfSurvivor)
				{
					TimerContainer.SetActive(value: true);
					HeroTimerContainer.SetActive(value: false);
					HelpersGfx.SetSurvivorClassMaterial(cardSurvivorClassTexture, PhoneCallDefinition.SurvivorClass);
				}
				if (PhoneCallDefinition.Type == PhoneCallDefinitionType.BetterChanceOfHero || PhoneCallDefinition.Type == PhoneCallDefinitionType.BetterChanceOfMultipleHeroes)
				{
					HeroTimerContainer.SetActive(value: true);
					TimerContainer.SetActive(value: false);
					if (HeroTokenIcon != null && PhoneCallDefinition.Type == PhoneCallDefinitionType.BetterChanceOfHero)
					{
						CurrencyType currencyType = CurrencyType.None;
						CurrencyType[] parsedCurrencyTypeValues2 = PhoneCallDefinition.GetParsedCurrencyTypeValues();
						if (parsedCurrencyTypeValues2.Length != 0)
						{
							currencyType = parsedCurrencyTypeValues2[0];
						}
						HeroTokenIcon.spriteName = HelpersGfx.GetTokenCurrencyIconName(currencyType);
					}
					else
					{
						HeroTokenIcon.spriteName = "";
					}
				}
				if (PhoneCallDefinition.Type == PhoneCallDefinitionType.GuaranteedHero)
				{
					HeroTimerContainer.SetActive(value: false);
					TimerContainer.SetActive(value: false);
					if (!string.IsNullOrEmpty(PhoneCallDefinition.EndTimeUtc))
					{
						TimerContainer.SetActive(value: true);
					}
				}
			}
		}
		if (moreInfoTitleLabel != null)
		{
			moreInfoTitleLabel.gameObject.SetActive(PhoneCallDefinition != null);
			if (PhoneCallDefinition != null)
			{
				if (PhoneCallDefinition.Type == PhoneCallDefinitionType.BetterChanceOfSurvivor)
				{
					moreInfoTitleLabel.text = LocalizationManager.GetText("Popup.StartPhoneCall.BetterChanceFor{SurvivorClass}", HelpersLocalization.GetSurvivorClassName(PhoneCallDefinition.SurvivorClass));
				}
				else if (PhoneCallDefinition.Type == PhoneCallDefinitionType.BetterChanceOfHero)
				{
					CurrencyType currencyType2 = CurrencyType.None;
					CurrencyType[] parsedCurrencyTypeValues3 = PhoneCallDefinition.GetParsedCurrencyTypeValues();
					if (parsedCurrencyTypeValues3.Length == 1)
					{
						currencyType2 = parsedCurrencyTypeValues3[0];
					}
					else if (parsedCurrencyTypeValues3.Length == 0)
					{
						Debug.LogError("Encountered a phone call definition of type BetterChanceOfHero, but the call has no hero token currency type.");
					}
					else
					{
						Debug.LogError("Encountered a phone call definition of type BetterChanceOfHero, but the call actually has multiple hero types.");
					}
					moreInfoTitleLabel.text = LocalizationManager.GetText("Popup.StartPhoneCall.BetterChanceFor{HeroToken}", HelpersLocalization.GetCurrencyName(currencyType2));
				}
				else if (PhoneCallDefinition.Type == PhoneCallDefinitionType.BetterChanceOfMultipleHeroes)
				{
					moreInfoTitleLabel.text = LocalizationManager.GetText("Popup.StartPhoneCall.MoreInfo." + data.Name);
				}
				else if (PhoneCallDefinition.Type == PhoneCallDefinitionType.GuaranteedHero)
				{
					moreInfoTitleLabel.text = LocalizationManager.GetText("Popup.StartPhoneCall.GuaranteedHeroTitle");
				}
			}
		}
		if (!(moreInfoTitleLabel2 != null))
		{
			return;
		}
		bool value = false;
		if (PhoneCallDefinition != null && PhoneCallDefinition.Type == PhoneCallDefinitionType.BetterChanceOfMultipleHeroes)
		{
			string textId = "Popup.StartPhoneCall.MoreInfo2." + data.Name;
			if (SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(textId))
			{
				moreInfoTitleLabel2.text = LocalizationManager.GetText(textId);
				value = true;
			}
		}
		Helpers.GameObjectSetActive(moreInfoTitleLabel2, value);
	}

	//SetupDynamicCall
	private void SetupDynamicCall(PhoneCallVisual data)
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("SetupDynamicCall " + data.BgImgPath, DebugType.Call);
			if (!GameManager.Instance.IsConnectedToServer)
			{
				LoadImageFromCdn.GetTextureFromCache(HeroTexture, data.OverlayImgPath);
				if (HeroTexture.mainTexture != null)
				{
					texturesLoaded.FirstState = true;
					texturesLoaded.SecondState = true;
					TweenTexture(null);
				}
			}
			else
			{
				LoadImageFromCdn.LoadImageAsync(data.OverlayImgPath, delegate (Texture texture)
				{
					HeroTexture.mainTexture = texture;
					texturesLoaded.FirstState = true;
					texturesLoaded.SecondState = true;
					TweenTexture(null);

				}, delegate
				{
					loading = false;
				});
			}
		}
		else
		{
			if (!GameManager.Instance.IsConnectedToServer)
			{
				Helpers.GameObjectSetActive(HeroTexture, value: true);
				Helpers.GameObjectSetActive(BackgroundTexture, value: true);
				TweenManager.PlayTweenGroup(base.gameObject, BgImageLoadCompleteTweenGroup);
				return;
			}
			LoadImageFromCdn.LoadImageAsync(data.BgImgPath, delegate (Texture texture)
			{
				BackgroundTexture.mainTexture = texture;
				texturesLoaded.FirstState = true;
				lock (readLock)
				{
					if ((bool)texturesLoaded)
					{
						HeroTexture.gameObject.SetActive(value: true);
						BackgroundTexture.gameObject.SetActive(value: true);
						if (BgImageLoadCompleteTweenGroup != -1)
						{
							TweenManager.PlayTweenGroup(base.gameObject, BgImageLoadCompleteTweenGroup);
						}
					}
				}
			}, delegate
			{
				loading = false;
			});
			LoadImageFromCdn.LoadImageAsync(data.OverlayImgPath, delegate (Texture texture)
			{
				HeroTexture.mainTexture = texture;
				texturesLoaded.SecondState = true;
				lock (readLock)
				{
					if ((bool)texturesLoaded)
					{
						HeroTexture.gameObject.SetActive(value: true);
						BackgroundTexture.gameObject.SetActive(value: true);
						if (BgImageLoadCompleteTweenGroup != -1)
						{
							TweenManager.PlayTweenGroup(base.gameObject, BgImageLoadCompleteTweenGroup);
						}
					}
				}
			}, delegate
			{
				loading = false;
			});
		}
	}

	private void SetupRegularCall(int callSlot)
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			RegularTexture.material = ui_materials[callSlot];
		}
		else
		{
			RegularTexture.material = AssetBundleManager.Instance.LoadAsset<Material>($"ui_radio_{callSlot + 1}", "uimaterials");
		}
		RegularTexture.gameObject.SetActive(value: true);
		if (BgImageLoadCompleteTweenGroup != -1)
		{
			TweenManager.PlayTweenGroup(base.gameObject, BgImageLoadCompleteTweenGroup);
		}
	}

	private void Update()
	{
		if (timeToEndOffer <= 0)
		{
			return;
		}
		timeToEndOffer -= (long)(Time.deltaTime * 1000f);
		if (PhoneCallDefinition == null)
		{
			TimerLabel.text = LocalizationManager.GetText("Popup.StartPhoneCall.FreeCallIn{Time}", Helpers.FormatTimeNoZero(timeToEndOffer));
			UpdateUI();
			return;
		}
		if (TimerLabel.gameObject.activeInHierarchy)
		{
			TimerLabel.text = LocalizationManager.GetText("BundlePopUp.OfferTimer", Helpers.FormatTimeNoZero(timeToEndOffer));
		}
		if (HeroTimerLabel.gameObject.activeInHierarchy)
		{
			HeroTimerLabel.text = LocalizationManager.GetText("BundlePopUp.OfferTimer", Helpers.FormatTimeNoZero(timeToEndOffer));
		}
		if (timeToEndOffer <= 0)
		{
			UIEvent.Send("OnRadioPopupCardExpired");
		}
	}

	public string GetTutorialArrowID()
	{
		if (!(callButtonTutorialArrowParent != null))
		{
			return "";
		}
		return callButtonTutorialArrowParent.Id;
	}

	public void SetPosition(Vector3 localPosition)
	{
		base.transform.localPosition = localPosition;
	}

	public void AddClickListener(UIButtonExtended.OnClickCallback callback, string buttonId)
	{
		if (ClickButton != null)
		{
			ClickButton.id = buttonId;
			ClickButton.SetClickCallback(callback);
		}
		if (ClickButtonFree != null)
		{
			ClickButtonFree.id = buttonId;
			ClickButtonFree.SetClickCallback(callback);
		}
		if (lockedButton != null)
		{
			lockedButton.id = buttonId;
			lockedButton.SetClickCallback(OnClickedLockedButton);
		}
	}

	public void RemoveListeners()
	{
		if (ClickButton != null)
		{
			ClickButton.Clear();
		}
		if (ClickButtonFree != null)
		{
			ClickButtonFree.Clear();
		}
		if (lockedButton != null)
		{
			lockedButton.Clear();
		}
	}

	public void SetIsEnabled(bool value)
	{
		if (ClickButton != null)
		{
			ClickButton.isEnabled = value;
		}
		if (ClickButtonFree != null)
		{
			ClickButtonFree.isEnabled = value;
		}
	}

	public override void Clear()
	{
		widget = null;
		cashier = null;
		RemoveListeners();
		PlayTweens = true;
		base.Clear();
	}

	private void OnClickedLockedButton(UIButtonExtended button)
	{
		if (dropTypeUnlocked && !hasRequiredSurvivorClass && survivorClassNeeded != SurvivorClass.None)
		{
			UnlockClassPopup.OpenInfoAboutClass(survivorClassNeeded);
		}
	}

	private PhoneCallVisual GetData(PhoneCallDefinition definition)
	{
		PhoneCallVisual phoneCallVisual = GameManager.Instance.gameEconomyData.GetPhoneCallVisual(definition);
		if (phoneCallVisual == null)
		{
			DebugLogError($"Could not find visualisation data for definition '{definition.VisualOverride}'");
		}
		return phoneCallVisual;
	}


	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;

	[SerializeField]
	private UILabel IndexLabel;

	public List<Material> ui_materials = new List<Material>();

	public UISprite GlowSprite;

	public List<ItemAmountProbabilityData> SurvivorRarityAmounts { get; set; }

	public List<ItemAmountProbabilityData> HeroRarityAmounts { get; set; }

	public List<int> OriginHeroRarityAmountsValues;

	public List<int> parsedHeroTokensDropNumberValues;
	#endregion

	#region mycode
	public string GetInfo()
	{
		if (moreInfoTitleLabel != null)
			return moreInfoTitleLabel.text;
		return string.Empty;
	}

	public void SetGlow(bool IsGlow)
	{
		GlowSprite.gameObject.SetActive(IsGlow);
	}

	public void GenerateRarityAmounts()
	{
		int radioTentLevel = GetRadioTentLevel();
		Dictionary<int, FixedPoint> equipmentAndSurvivorRarityProbabilities = DataManager.Instance.GameData.GetEquipmentAndSurvivorRarityProbabilities(currentDropType, DropRewardType.Survivor, radioTentLevel);
		List<ItemAmountProbabilityData> list2 = new List<ItemAmountProbabilityData>();
		foreach (KeyValuePair<int, FixedPoint> item in equipmentAndSurvivorRarityProbabilities)
		{
			list2.Add(new ItemAmountProbabilityData
			{
				Rarity = item.Key,
				Probability = item.Value
			});
		}
		PhoneCallDefinition phoneCallDefinition = DataManager.Instance.GameData.GetPhoneCallDefinition(DataManager.Instance.Player.UtcTimeStamp, SlotNumber);

		int[] newHeroRarities = phoneCallDefinition.ParseHeroTokensDropNumberValues(out bool parseError);
		if (!parseError)
		{
			parsedHeroTokensDropNumberValues = newHeroRarities.ToList();
			DebugTWD.Log("NewHeroRarities for call " + this.SlotNumber + ": {" + string.Join(",", newHeroRarities) + "}");
		}

		Dictionary<int, FixedPoint> genericHeroRarityToTokenAmounts = DataManager.Instance.GameData.GetGenericHeroRarityToTokenAmounts(currentDropType, radioTentLevel, DropEventDefinition.DropEventContext.Normal);
		List<ItemAmountProbabilityData> list3 = new List<ItemAmountProbabilityData>();
		OriginHeroRarityAmountsValues = new List<int>();

		int index = 0;
		foreach (KeyValuePair<int, FixedPoint> item2 in genericHeroRarityToTokenAmounts)
		{
			DebugTWD.Log("GenericHero: " + phoneCallDefinition.SurvivorClass + " | " + item2.Key.ToString() + " | " + Math.Round(((float)item2.Value) * 100, 2) + "%");

			OriginHeroRarityAmountsValues.Add(item2.Key);
			list3.Add(new ItemAmountProbabilityData
			{
				Amount = !parseError && newHeroRarities.Length == genericHeroRarityToTokenAmounts.Count ? newHeroRarities[index].ToString() : item2.Key.ToString(),
				Probability = item2.Value
			});
			index++;
		}

		SurvivorRarityAmounts = list2;
		HeroRarityAmounts = list3;
	}

	private void TweenTexture(EventDelegate.Callback callback)
	{
		lock (readLock)
		{
			if ((bool)texturesLoaded)
			{
				HeroTexture.gameObject.SetActive(value: true);
				BackgroundTexture.gameObject.SetActive(value: true);
				if (BgImageLoadCompleteTweenGroup != -1)
				{
					TweenManager.PlayTweenGroup(base.gameObject, BgImageLoadCompleteTweenGroup, true, callback);
				}
			}
		}
	}

	public int GetCallPrice()
	{
		int price = 0;
		if (PayButton != null)
		{
			price = PayButton.radioPrice;
		}
		return price;
	}
	#endregion
}
