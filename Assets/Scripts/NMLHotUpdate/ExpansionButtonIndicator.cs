using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class ExpansionButtonIndicator : HUDElementFollowTarget
{
	[SerializeField]
	[Tooltip("按钮在 gated/ungated 之间切换时启停的根 GameObject。可选 —— 不填则回退到 indicator 自身这个 GameObject。")]
	private GameObject buttonRoot;

	private readonly List<VegetationModel> group = new List<VegetationModel>();

	private bool initialized;

	private bool wasVisible;

	public void Init(List<VegetationModel> expansionGroup)
	{
		if (expansionGroup == null || expansionGroup.Count == 0)
		{
			Debug.LogWarning("ExpansionButtonIndicator.Init 传入了空的 group。");
			Object.Destroy(base.gameObject);
			return;
		}
		group.Clear();
		group.AddRange(expansionGroup);
		for (int i = 0; i < group.Count; i++)
		{
			VegetationModel vegetationModel = group[i];
			if (vegetationModel != null)
			{
				vegetationModel.Changed += OnVegetationChanged;
			}
		}
		CampModel camp = GameManager.Instance.playerModel.Camp;
		if (camp != null)
		{
			camp.Changed += OnCampChanged;
		}
		AnchorToGroupLeader();
		initialized = true;
		RefreshVisibility(forceRefresh: true);
	}

	private void OnDestroy()
	{
		for (int i = 0; i < group.Count; i++)
		{
			VegetationModel vegetationModel = group[i];
			if (vegetationModel != null)
			{
				vegetationModel.Changed -= OnVegetationChanged;
			}
		}
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			CampModel camp = GameManager.Instance.playerModel.Camp;
			if (camp != null)
			{
				camp.Changed -= OnCampChanged;
			}
		}
	}

	private void OnVegetationChanged(ModelObject model, string changed, object args)
	{
		RefreshVisibility();
	}

	private void OnCampChanged(ModelObject model, string changed, object args)
	{
		switch (changed)
		{
		case "EventLevelUpBuilding":
		case "RemoveBuilding":
		case "EventAddBuilding":
			RefreshVisibility();
			break;
		}
	}

	private void AnchorToGroupLeader()
	{
		if (CampView.Instance == null || CampView.Instance.CampViewBuildings == null)
		{
			return;
		}
		List<VegetationModel> list = new List<VegetationModel>(group);
		list.Sort((VegetationModel a, VegetationModel b) => GetFootprintArea(b).CompareTo(GetFootprintArea(a)));
		for (int num = 0; num < list.Count; num++)
		{
			VegetationModel vegetationModel = list[num];
			if (vegetationModel != null)
			{
				BuildingView buildingView = CampView.Instance.CampViewBuildings.FindBuildingView(vegetationModel);
				if (buildingView != null && buildingView.BuildingGameObject != null)
				{
					FollowTarget(buildingView.BuildingGameObject);
					break;
				}
			}
		}
	}

	private static int GetFootprintArea(VegetationModel veg)
	{
		if (veg == null)
		{
			return 0;
		}
		return veg.Size.X * veg.Size.Y;
	}

	private void RefreshVisibility(bool forceRefresh = false)
	{
		if (!initialized)
		{
			return;
		}
		group.RemoveAll((VegetationModel v) => v?.MarkedToBeDeleted ?? true);
		if (group.Count == 0)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		int councilLevel = GameManager.Instance.playerModel.Camp.GetCouncilLevel();
		bool flag = false;
		bool flag2 = true;
		for (int num = 0; num < group.Count; num++)
		{
			VegetationModel vegetationModel = group[num];
			if (vegetationModel.IsBeingCut)
			{
				flag = true;
			}
			if (!vegetationModel.CanBeCutAt(councilLevel))
			{
				flag2 = false;
			}
		}
		if (flag)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		bool flag3 = flag2;
		if (forceRefresh || flag3 != wasVisible)
		{
			Helpers.GameObjectSetActive((buttonRoot != null) ? buttonRoot : base.gameObject, flag3);
			wasVisible = flag3;
		}
	}

	public void OnClick()
	{
		if (!initialized || group.Count == 0)
		{
			return;
		}
		int councilLevel = GameManager.Instance.playerModel.Camp.GetCouncilLevel();
		for (int i = 0; i < group.Count; i++)
		{
			VegetationModel vegetationModel = group[i];
			if (vegetationModel == null || !vegetationModel.CanBeCutAt(councilLevel))
			{
				RefreshVisibility(forceRefresh: true);
				return;
			}
			if (vegetationModel.IsBeingCut)
			{
				return;
			}
		}
		Cashier currencies = BuildCombinedCashier();
		ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		if (confirmationPopup == null)
		{
			Debug.LogError("ExpansionButtonIndicator:ConfirmationPopup 未注册。");
			return;
		}
		confirmationPopup.SetContent(LocalizationManager.GetText("Popup.ExpandCamp.Title"), LocalizationManager.GetText("Popup.ExpandCamp.Message"));
		confirmationPopup.SetCurrencies(currencies);
		confirmationPopup.SetCallbacks(OnConfirm);
		confirmationPopup.Open();
	}

	private Cashier BuildCombinedCashier()
	{
		Cashier cashier = new Cashier((TWDModelManager)GameManager.Instance.playerModel.Manager);
		for (int i = 0; i < group.Count; i++)
		{
			List<CashierItem> cashierItems = group[i].GetCutCashier.GetCashierItems();
			for (int j = 0; j < cashierItems.Count; j++)
			{
				cashier.AddItem(cashierItems[j]);
			}
		}
		return cashier;
	}

	private void OnConfirm()
	{
		Cashier cashier = BuildCombinedCashier();
		if (cashier.CanAfford())
		{
			ExecuteAllCutCommands();
			return;
		}
		int num = 0;
		for (int i = 0; i < (int)CurrencyType.Count; i++)
		{
			CurrencyType currencyType = (CurrencyType)i;
			int missing = cashier.GetMissing(currencyType);
			if (missing > 0)
			{
				if (!GameManager.Instance.gameEconomyData.CanConvertToDiamonds(currencyType))
				{
					HUDNotification.Error(LocalizationManager.GetText("Popup.NotEnoughResources"));
					return;
				}
				num += GameManager.Instance.gameEconomyData.CurrencyToDiamonds(currencyType, missing, GameManager.Instance.playerModel);
			}
		}
		if (num <= 0)
		{
			return;
		}
		int value = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Diamonds).Value;
		if (num > value)
		{
			ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(num);
			return;
		}
		BuyResourcesPopup buyResourcesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup) as BuyResourcesPopup;
		if (!(buyResourcesPopup == null))
		{
			string text = LocalizationManager.GetText("Popup.BuyResources.NotEnoughCurrency");
			string text2 = LocalizationManager.GetText("Popup.BuyResources.BuyMissingResources");
			buyResourcesPopup.SetContent(text, text2, num);
			buyResourcesPopup.SetMissingCurrencies(cashier, showDiamonds: false);
			buyResourcesPopup.SetCallbacks(delegate
			{
				ExecuteAllCutCommands();
			}, delegate
			{
			});
			buyResourcesPopup.Open();
		}
	}

	private void ExecuteAllCutCommands()
	{
		List<VegetationModel> list = new List<VegetationModel>(group);
		for (int i = 0; i < list.Count; i++)
		{
			VegetationModel vegetationModel = list[i];
			if (vegetationModel != null && !vegetationModel.IsBeingCut)
			{
				Helpers.ExecuteCommand(new CutVegetationCommand(vegetationModel));
			}
		}
	}
}
