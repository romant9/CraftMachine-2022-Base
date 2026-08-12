using BaseModel;
using TWDModel;
using UnityEngine;

public class PopupCage : HUDElement
{
	[SerializeField]
	private WalkersListPanel walkersList;

	public override void Open()
	{
		base.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_trainingground");
		UpdateUI();
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		PlayerModel player = GameManager.Instance.modelManager.Player;
		if (player != null)
		{
			player.Changed += OnPlayerModelChanged;
		}
		UpdateUI();
	}

	public void OnPlayerModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "currencyChangedEvent" && args is CurrencyModel { Type: CurrencyType.Diamonds } && walkersList != null)
		{
			walkersList.RefreshCards();
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		PlayerModel player = GameManager.Instance.modelManager.Player;
		if (player != null)
		{
			player.Changed -= OnPlayerModelChanged;
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnNewWalkerSelected":
			if (parameter != null)
			{
				WalkerInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PopupWalkerInfo) as WalkerInfoPopup;
				obj.currentState = WalkerInfoPopup.WalkerInfoPopupStates.Info;
				obj.OpenForModel(parameter as OutpostWalkerModel);
				UpdateUI();
			}
			break;
		case "OnWalkerUpgraded":
			Close();
			break;
		case "OnSurvivorInstantUpgraded":
		case "OnSurvivorRenamed":
			if (walkersList != null)
			{
				walkersList.RefreshCards();
			}
			break;
		case "SurvivorExtraSlotBought":
			if (walkersList != null)
			{
				walkersList.RefreshCards();
			}
			break;
		case "SurvivorListRefreshed":
			UpdateUI();
			break;
		case "SurvivorPortraitUpdated":
			if (walkersList != null)
			{
				walkersList.RefreshCards();
			}
			break;
		}
	}

	public override void UpdateUI()
	{
		foreach (UIListCard<OutpostWalkerModel> card in walkersList.GetCards())
		{
			WalkerCard walkerCard = card as WalkerCard;
			if (walkerCard != null)
			{
				walkerCard.UpdateUI();
			}
		}
	}
}
