using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class AnimateIapRewards : MonoBehaviour
{
	public const string CurrencyTypeSlots = "CurrencyTypeSlots";

	[SerializeField]
	private UISprite CurrencySprite;

	[SerializeField]
	private UISprite OutfitSprite;

	[SerializeField]
	private UILabel CurrencyLabel;

	[SerializeField]
	private EquipmentButton equipmentButton;

	private Dictionary<string, int> IapRewardsList = new Dictionary<string, int>();

	private Animator animator;

	private Callback CompletedCallback;

	private bool HideLastItem = true;

	private string lastCurrencyShown;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void AddSingleIAP(CurrencyType type, InAppPurchaseProductApple item)
	{
		if (equipmentButton != null && equipmentButton.gameObject.activeSelf)
		{
			equipmentButton.gameObject.SetActive(value: false);
		}
		if (type == CurrencyType.Diamonds && CurrencySprite != null && CurrencyLabel != null)
		{
			CurrencySprite.spriteName = HelpersGfx.GetCurrencyIconName(type);
		}
	}

	public void AddSingleIAP(BundleContentDefinition bundleContentDefinition)
	{
		if (equipmentButton != null && equipmentButton.gameObject.activeSelf)
		{
			equipmentButton.gameObject.SetActive(value: false);
		}
		int totalCurrencyRewardAmount = bundleContentDefinition.RewardEntries.GetTotalCurrencyRewardAmount(CurrencyType.Diamonds);
		if (totalCurrencyRewardAmount > 0)
		{
			CurrencySprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Diamonds);
			CurrencyLabel.text = totalCurrencyRewardAmount.ToString();
		}
	}

	public void StartPlaying(Callback completedCallback = null, bool hideLast = true)
	{
		HideLastItem = hideLast;
		CompletedCallback = completedCallback;
		AnimateNextResources();
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		CompletedCallback = null;
		base.gameObject.SetActive(value: false);
	}

	public void AddResource(string type, int amount)
	{
		if (equipmentButton != null && equipmentButton.gameObject.activeSelf)
		{
			equipmentButton.gameObject.SetActive(value: false);
		}
		if (!IapRewardsList.ContainsKey(type))
		{
			IapRewardsList.Add(type, amount);
		}
		else
		{
			Debug.LogWarning("Already added amount for type: " + type);
		}
	}

	public void AddEquipment(EquipmentItemModel equipment)
	{
		if (equipmentButton != null)
		{
			if (!equipmentButton.gameObject.activeSelf)
			{
				equipmentButton.gameObject.SetActive(value: true);
			}
			equipmentButton.Setup(equipment, null, null, "OnNewEquipmentCardSelected", showOwnerAndUpgradeIndicator: false);
		}
	}

	public void AddOutfits(OutfitDefinition outfit)
	{
		if (equipmentButton != null && equipmentButton.gameObject.activeSelf)
		{
			equipmentButton.gameObject.SetActive(value: false);
		}
		bool flag = !string.IsNullOrEmpty(outfit.BundleSprite);
		if (CurrencySprite != null)
		{
			CurrencySprite.gameObject.SetActive(!flag);
		}
		if (OutfitSprite != null)
		{
			OutfitSprite.gameObject.SetActive(flag);
			if (flag)
			{
				OutfitSprite.spriteName = outfit.BundleSprite;
			}
		}
		if (CurrencyLabel != null)
		{
			string text = LocalizationManager.GetText(outfit.TitleLocalizationKey);
			CurrencyLabel.text = LocalizationManager.GetText("Bundle.Outfit.Description{Parameter}", text);
		}
	}

	public void ShowAnimationDone()
	{
		if (!CanHideCurrentAnimated())
		{
			HideAnimationDone();
		}
		if (!string.IsNullOrEmpty(lastCurrencyShown) && IapRewardsList.ContainsKey(lastCurrencyShown))
		{
			IapRewardsList.Remove(lastCurrencyShown);
		}
	}

	private bool CanHideCurrentAnimated()
	{
		int num = ((!HideLastItem) ? 1 : 0);
		if (IapRewardsList.Count > num)
		{
			setAnimatorParam(show: false);
			return true;
		}
		return false;
	}

	public int HideAnimationDone()
	{
		AnimateNextResources();
		if (IapRewardsList.Count == 0 && CompletedCallback != null)
		{
			CompletedCallback();
		}
		return IapRewardsList.Count;
	}

	public bool AnimateNextResources()
	{
		using (Dictionary<string, int>.Enumerator enumerator = IapRewardsList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				KeyValuePair<string, int> current = enumerator.Current;
				lastCurrencyShown = current.Key;
				if (CurrencySprite != null)
				{
					if (current.Key == "CurrencyTypeSlots")
					{
						CurrencySprite.gameObject.SetActive(value: false);
					}
					else
					{
						try
						{
							CurrencyType currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), current.Key);
							CurrencySprite.spriteName = HelpersGfx.GetCurrencyIconName(currencyType);
							CurrencySprite.gameObject.SetActive(value: true);
						}
						catch (Exception)
						{
							Debug.LogError("Could not parse \"" + current.Key + "\" to Enum-> CurrencyType");
							CurrencySprite.gameObject.SetActive(value: true);
						}
					}
				}
				if (CurrencyLabel != null)
				{
					string text = "";
					if (current.Value == -1)
					{
						if (current.Key == CurrencyType.ReplayToken.ToString())
						{
							text = LocalizationManager.GetText("Bundle.Replay.Full");
						}
						else if (current.Key == CurrencyType.Supplies.ToString())
						{
							text = LocalizationManager.GetText("Bundle.Supplies.Full");
						}
					}
					if (current.Key == "CurrencyTypeSlots")
					{
						text = LocalizationManager.GetText("Bundle.Slots.Description{Parameter}", current.Value);
					}
					if (text != "")
					{
						CurrencyLabel.text = text;
					}
					else if (current.Key == CurrencyType.Diamonds.ToString())
					{
						CurrencyLabel.text = current.Value.ToString();
					}
					else
					{
						CurrencyLabel.text = Helpers.FormatNumber(current.Value);
					}
				}
				setAnimatorParam(show: true);
				return true;
			}
		}
		return false;
	}

	public int GetIapRewardsListCount()
	{
		if (IapRewardsList != null)
		{
			return IapRewardsList.Count;
		}
		return 0;
	}

	public void SkipCurrent()
	{
		if (animator != null && animator.GetBool("Show"))
		{
			CanHideCurrentAnimated();
			if (!string.IsNullOrEmpty(lastCurrencyShown) && IapRewardsList.ContainsKey(lastCurrencyShown))
			{
				IapRewardsList.Remove(lastCurrencyShown);
			}
		}
	}

	private void setAnimatorParam(bool show)
	{
		if (animator != null)
		{
			animator.SetBool("Show", show);
		}
	}
}
