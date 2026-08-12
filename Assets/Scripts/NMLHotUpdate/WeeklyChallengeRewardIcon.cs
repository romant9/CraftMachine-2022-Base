using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeRewardIcon : WaypointIconBase
{
	[SerializeField]
	private UISprite iconSprite;

	[SerializeField]
	private UIButtonExtended iconButton;

	[SerializeField]
	private UIAtlas monochromeAtlas;

	[SerializeField]
	private UITexture weaponIcon;

	[SerializeField]
	private UITexture avatarIcon;

	[SerializeField]
	private UITexture borderIcon;

	[SerializeField]
	private UIWidget rewardContainer;

	[SerializeField]
	private int playTweenGroupOnClaim = 8;

	[SerializeField]
	private int ImageLoadCompleteTweenGroup = 10;

	private WeeklyChallengeReward rewardReference;

	public void SetReward(WeeklyChallengeReward reward, int currentStarCount)
	{
		if (IsNotNull(reward))
		{
			if (iconSprite != null)
			{
				iconSprite.alpha = 1f;
			}
			rewardReference = reward;
			UpdateUI();
		}
		else
		{
			Clear();
		}
	}

	public void UpdateUI()
	{
		if (rewardReference == null || rewardReference.RewardEntries == null)
		{
			return;
		}
		IReward rewardAt = rewardReference.RewardEntries.GetRewardAt(0);
		if (rewardAt == null)
		{
			return;
		}
		if (rewardAt is RewardTradeCrate)
		{
			string spriteName = "";
			HelpersGfx.GetIconNameForIReward(rewardAt, out spriteName, null, null, null);
			HelpersUI.SetSprite(iconSprite, spriteName);
		}
		else if (rewardAt is RewardCurrency || rewardAt is RewardSkipChallange)
		{
			string spriteName2 = "";
			HelpersGfx.GetIconNameForIReward(rewardAt, out spriteName2, null, null, null, GameManager.Instance.playerModel);
			HelpersUI.SetSpriteAndAtlas(iconSprite, spriteName2, monochromeAtlas);
		}
		else if (rewardAt is RewardEquipment)
		{
			weaponIcon.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardAt as RewardEquipment);
		}
		else if (rewardAt is RewardAvatars rewardAvatars)
		{
			Helpers.GameObjectSetActive(avatarIcon, value: false);
			Helpers.GameObjectSetActive(borderIcon, value: false);
			if (rewardAvatars.Avatar >= 0 && avatarIcon != null)
			{
				AvatarsDefinition avatarsDefinition = GameManager.Instance.gameEconomyData.GetAvatarsDefinition(rewardAvatars.Avatar);
				LoadImageFromCdn.LoadImageToTarget(avatarIcon, avatarsDefinition?.Image, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
			}
			else if (rewardAvatars.Border >= 0 && borderIcon != null)
			{
				BordersDefinition bordersDefinition = GameManager.Instance.gameEconomyData.GetBordersDefinition(rewardAvatars.Border);
				LoadImageFromCdn.LoadImageToTarget(borderIcon, bordersDefinition?.Image, clearLocalCachedUrls: false, ImageLoadCompleteTweenGroup);
			}
		}
	}

	public override void Show()
	{
		base.Show();
		Helpers.GameObjectSetActive(rewardContainer, value: true);
		if (rewardReference.RewardEntries.GetRewardAt(0) is RewardEquipment)
		{
			Helpers.GameObjectSetActive(avatarIcon, value: false);
			Helpers.GameObjectSetActive(borderIcon, value: false);
			Helpers.GameObjectSetActive(iconSprite, value: false);
			Helpers.GameObjectSetActive(weaponIcon, value: true);
		}
		else if (rewardReference.RewardEntries.GetRewardAt(0) is RewardAvatars)
		{
			Helpers.GameObjectSetActive(weaponIcon, value: false);
			Helpers.GameObjectSetActive(iconSprite, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(avatarIcon, value: false);
			Helpers.GameObjectSetActive(borderIcon, value: false);
			Helpers.GameObjectSetActive(weaponIcon, value: false);
			Helpers.GameObjectSetActive(iconSprite, value: true);
		}
	}

	public override void Hide()
	{
		base.Hide();
		Helpers.GameObjectSetActive(rewardContainer, value: false);
	}

	public override void CompleteTrigger()
	{
		base.CompleteTrigger();
		Show();
		TweenManager.PlayTweenGroup(base.gameObject, playTweenGroupOnClaim, forward: true, TweenClaimDone);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		AddListeners();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		RemoveListeners();
	}

	public override void Clear()
	{
		base.Clear();
		rewardReference = null;
		RemoveListeners();
	}

	private void TweenClaimDone()
	{
		Helpers.GameObjectSetActive(rewardContainer, value: false);
	}

	private void OnClickIcon(UIButtonExtended button)
	{
		if (button != null && button.gameObject != null && rewardReference != null)
		{
			int overSpeedConvertedAmount = GetOverSpeedConvertedAmount();
			if (overSpeedConvertedAmount <= 0)
			{
				TooltipManager.OpenForChallengeReward(base.gameObject, rewardReference, rewardReference.Control);
			}
			else
			{
				TooltipManager.OpenForChallengeReward_sp(base.gameObject, rewardReference, rewardReference.Control, overSpeedConvertedAmount);
			}
		}
	}

	private int GetOverSpeedConvertedAmount()
	{
		int num = 0;
		if (rewardReference == null || rewardReference.RewardEntries == null || rewardReference.RewardEntries.RewardsList == null)
		{
			return 0;
		}
		List<IReward> rewardsList = rewardReference.RewardEntries.RewardsList;
		if (rewardsList.Count <= 0)
		{
			return 0;
		}
		for (int i = 0; i < rewardsList.Count; i++)
		{
			if (!(rewardsList[i] is RewardCurrency))
			{
				continue;
			}
			RewardCurrency rewardCurrency = rewardsList[i] as RewardCurrency;
			if (GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(rewardCurrency.CurrencyType))
			{
				PlayerModel playerModel = GameManager.Instance.playerModel;
				int currencyAmount = playerModel.GetCurrencyAmount(rewardCurrency.CurrencyType);
				int max = playerModel.GetCurrency(rewardCurrency.CurrencyType).Max;
				if (currencyAmount + rewardCurrency.Amount > max)
				{
					num += GameManager.Instance.modelManager.GameEconomyData.CurrencyToDiamonds(rewardCurrency.CurrencyType, currencyAmount + rewardCurrency.Amount - max, GameManager.Instance.modelManager.Player);
				}
			}
		}
		return num;
	}

	private void AddListeners()
	{
		if (iconButton != null)
		{
			iconButton.SetClickCallback(OnClickIcon);
		}
	}

	private void RemoveListeners()
	{
		if (iconButton != null)
		{
			iconButton.Clear();
		}
	}
}
