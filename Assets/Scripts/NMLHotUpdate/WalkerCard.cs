using System;
using BaseModel;
using TWDModel;
using UnityEngine;

public class WalkerCard : UIListCard<OutpostWalkerModel>
{
	[Header("Main info")]
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UISprite portrait;

	[SerializeField]
	private UIButton infoButton;

	[SerializeField]
	private GameObject statsContainer;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UILabel maxLevelLabel;

	[Header("Upgrades")]
	[SerializeField]
	private GameObject upgradeBox;

	[SerializeField]
	private UILabel upgradeBoxLabel;

	[SerializeField]
	private GameObject indicatorContainer;

	[SerializeField]
	private UILabel indicatorLabel;

	[SerializeField]
	private UIProgressBar upgradeProgressBar;

	[SerializeField]
	private UIButton upgradeButton;

	[SerializeField]
	private UILabel upgradePriceLabel;

	[SerializeField]
	private UIButton fullyTraindeButton;

	[SerializeField]
	private UILabel fullyTrainedLabel;

	[SerializeField]
	private UISprite classIconSprite;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has enough currency.")]
	private Color availableCurrencyColor = Color.white;

	[SerializeField]
	[Tooltip("The tint color for labels when the user has NOT enough currency.")]
	private Color unavailableCurrencyColor = new Color(0.511f, 0.129f, 0.027f, 1f);

	[Header("Locked")]
	[SerializeField]
	private GameObject lockedContainer;

	[SerializeField]
	private UILabel lockedLabel;

	private void OnDisable()
	{
		OutpostWalkerModel item = base.Item;
		if (item != null)
		{
			item.Changed -= OnWalkerModelChanged;
		}
	}

	public override void UpdateUI()
	{
		bool num = base.Item != null;
		OutpostWalkerModel item = base.Item;
		ActorDefinition actorDefinition = item.ActorDefinition;
		if (!num)
		{
			return;
		}
		nameLabel.text = HelpersLocalization.GetActorClassName(Faction.Walker.ToString(), actorDefinition.Class);
		amountLabel.text = item.Amount.ToString();
		levelLabel.text = item.Level.ToString();
		maxLevelLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("SurvivorCard.Of") + " " + item.MaxUpgradeLevel;
		if (!item.HasReachedMaxLevel)
		{
			Cashier upgradeCashier = item.GetUpgradeCashier(instantUpgrade: false);
			upgradePriceLabel.text = Helpers.FormatNumber(upgradeCashier.GetTotalCost(CurrencyType.Outpost));
			upgradePriceLabel.color = (upgradeCashier.CanPay(CurrencyType.Outpost) ? availableCurrencyColor : unavailableCurrencyColor);
		}
		fullyTraindeButton.gameObject.SetActive(item.HasReachedMaxLevel);
		fullyTrainedLabel.text = LocalizationManager.GetText("SurvivorCard.Button.FullyTrained");
		HelpersGfx.SetWalkerPortaitTexture(portrait, item.Id);
		if (upgradeBox != null)
		{
			bool flag = item.IsUpgrading();
			upgradeBox.SetActive(flag);
			if (upgradeBoxLabel != null && flag)
			{
				item.Changed -= OnWalkerModelChanged;
				item.Changed += OnWalkerModelChanged;
				upgradeBoxLabel.text = Helpers.FormatTimeNoZero(item.TimedActionModel.MillisecondsTillCompletion);
			}
		}
		bool isLocked = base.Item.IsLocked;
		if (isLocked)
		{
			lockedLabel.text = LocalizationManager.GetText("Popup.UpgradeWalker.CompleteEpisode{Name}", HelpersLocalization.GetEpisodeTitle(GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(item.CurrentUpgradeDefinition.EpisodeLock)));
		}
		lockedContainer.SetActive(isLocked);
		statsContainer.SetActive(!isLocked);
		classIconSprite.spriteName = "Ui_Icon_Class_" + item.ActorDefinition.Class.ToString();
	}

	private void OnWalkerModelChanged(ModelObject model, string changed, object args)
	{
		if (args == base.Item && changed == "ActionFinishedEvent")
		{
			UpdateUI();
		}
	}

	public void OnLanguageChanged()
	{
		UpdateUI();
	}

	protected void LateUpdate()
	{
		if (base.Item != null && upgradeBox != null)
		{
			bool flag = base.Item.IsUpgrading();
			upgradeBox.SetActive(flag);
			if (upgradeBoxLabel != null && flag)
			{
				upgradeBoxLabel.text = Helpers.FormatTimeNoZero(base.Item.TimedActionModel.MillisecondsTillCompletion);
			}
		}
	}

	public void SetPicture(Texture picture)
	{
		if (portrait != null && picture != null)
		{
			portrait.mainTexture = picture;
		}
	}

	public override int GetSortValue()
	{
		if (base.Item == null)
		{
			return 0;
		}
		CageDefinition cafeDefinitionForLevel = base.Item.GetCafeDefinitionForLevel(0);
		if (cafeDefinitionForLevel != null && !string.IsNullOrEmpty(cafeDefinitionForLevel.EpisodeLock))
		{
			int num = 0;
			try
			{
				num = int.Parse(cafeDefinitionForLevel.EpisodeLock.Replace("Episode ", ""));
			}
			catch (Exception)
			{
				return 1;
			}
			return -num;
		}
		return 1;
	}

	public bool IsAnyWalkerUpgrading()
	{
		return false;
	}

	public void OnCardClicked()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_card_click");
		UIEvent.Send("OnNewWalkerSelected", base.Item);
		EventManager.NotifyClick("WalkerCard");
	}
}
