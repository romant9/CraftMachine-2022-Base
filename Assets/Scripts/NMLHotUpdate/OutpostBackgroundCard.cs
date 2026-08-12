using TWDModel;
using UnityEngine;

public class OutpostBackgroundCard : UIListCard<OutpostTemplateDefinition>
{
	[SerializeField]
	private UISprite BackgroundSprite;

	[SerializeField]
	private UILabel TypeLabel;

	[SerializeField]
	private UILabel DeploymentPointsLabel;

	[SerializeField]
	private UILabel UnlockDescriptionLabel;

	[SerializeField]
	private GameObject LockedParent;

	[SerializeField]
	private PayButton PayUnlockButton;

	[SerializeField]
	private GameObject ButtonSelectArtParent;

	private ButtonWithLabel ButtonInternal;

	public ButtonWithLabel Button
	{
		get
		{
			if (ButtonInternal == null)
			{
				ButtonInternal = GetComponent<ButtonWithLabel>();
			}
			return ButtonInternal;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GetDefinition() == null)
		{
			return;
		}
		OutpostTemplateDefinition definition = GetDefinition();
		if (Button != null)
		{
			Button.text = definition.Id;
		}
		if (IsBackgroundUnlocked())
		{
			if (LockedParent != null && ButtonSelectArtParent != null && Button != null && BackgroundSprite != null)
			{
				BackgroundSprite.spriteName = HelpersGfx.GetOutpostBackgroundSpriteName(definition);
				BackgroundSprite.alpha = 1f;
				LockedParent.gameObject.SetActive(value: false);
				ButtonSelectArtParent.SetActive(value: true);
				if (GameManager.Instance.playerModel.OutpostModel.OutpostRunLocation == null)
				{
					TutorialView.Instance.ShowButtonSuggest("OutpostBackgroundButton", show: true);
				}
			}
		}
		else
		{
			if (LockedParent != null && ButtonSelectArtParent != null && Button != null && BackgroundSprite != null)
			{
				BackgroundSprite.spriteName = HelpersGfx.GetOutpostBackgroundSpriteName(definition);
				BackgroundSprite.alpha = 0.5f;
				LockedParent.gameObject.SetActive(value: true);
				ButtonSelectArtParent.SetActive(value: false);
			}
			if (PayUnlockButton != null)
			{
				Cashier backgroundCashier = GetBackgroundCashier();
				if (backgroundCashier != null)
				{
					PayUnlockButton.UpdateUI(backgroundCashier);
					PayUnlockButton.gameObject.SetActive(value: true);
				}
				else
				{
					PayUnlockButton.gameObject.SetActive(value: false);
				}
			}
		}
		if (UnlockDescriptionLabel != null)
		{
			UnlockDescriptionLabel.text = LocalizationManager.GetText("Outpost.Background.Unlock.Description{Parameter}", definition.OutpostLevelRequirement);
		}
		if (TypeLabel != null)
		{
			TypeLabel.gameObject.SetActive(value: false);
		}
		if (DeploymentPointsLabel != null)
		{
			DeploymentPointsLabel.text = definition.FirstSliceDeploymentPoints + definition.SecondSliceDeploymentPoints + definition.ThirdSliceDeploymentPoints + " Deployment points";
		}
	}

	public bool IsBackgroundUnlocked()
	{
		if (GetDefinition() != null && GameManager.Instance != null && GameManager.Instance.modelManager.Player.OutpostModel != null)
		{
			OutpostTemplateDefinition definition = GetDefinition();
			return GameManager.Instance.modelManager.Player.OutpostModel.IsBackgroundUnlocked(definition.Id);
		}
		return true;
	}

	public Cashier GetBackgroundCashier()
	{
		if (GetDefinition() != null)
		{
			OutpostTemplateDefinition definition = GetDefinition();
			if (definition.GetCostAmount() <= 0)
			{
				return null;
			}
			return Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.OutpostBackground, definition.GetCostCurrencyType(), definition.GetCostAmount());
		}
		return null;
	}

	public void BuyBackgroundUnlock()
	{
		if (GetDefinition() != null && GetBackgroundCashier() != null)
		{
			ConsumeCurrencyCommandUtils.Execute(new BuyOutpostBackgroundCommand(GetDefinition().Id), GetBackgroundCashier(), BuyUnlockCallback);
		}
		else
		{
			Debug.LogError("OutpostBackgroundCard: Cant buy background! Definition or cashier is NULL");
		}
	}

	public void BuyUnlockCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			UpdateUI();
		}
	}

	public OutpostTemplateDefinition GetDefinition()
	{
		if (base.Item != null && base.Item != null)
		{
			return base.Item;
		}
		return null;
	}

	public void Clear()
	{
		if (ButtonInternal == null)
		{
			ButtonInternal.Clear();
		}
	}
}
