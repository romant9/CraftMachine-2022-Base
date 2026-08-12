using TWDModel;
using UnityEngine;

public class NewCampPopup : HUDElement
{
	[SerializeField]
	private UILabel campNameLabel;

	[SerializeField]
	private UILabel campDescriptionLabel;

	[SerializeField]
	private UISprite campSprite;

	[SerializeField]
	private UnlocksListPanel unlocksListPanel;

	public override void Open()
	{
		base.Open();
		defaultPopup.SetPayButtonClickCallback(OnMoveCamp);
		UpdateUI();
		if (BuildingPhotoManager.Instance.IsRendering)
		{
			base.gameObject.SetActive(value: false);
			EventManager.OnEvent += OnBuildingPhotoRendered;
		}
	}

	private void OnBuildingPhotoRendered(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.BuildingPhotoRendered)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public override void Close()
	{
		base.Close();
		EventManager.OnEvent -= OnBuildingPhotoRendered;
	}

	public override void UpdateUI()
	{
		CampMoverModel campMover = GameManager.Instance.playerModel.CampMover;
		CampType nextLevelCampType = campMover.GetNextLevelCampType();
		campSprite.spriteName = HelpersGfx.GetCampIconName(nextLevelCampType);
		campNameLabel.text = LocalizationManager.GetText("Camp.Name." + nextLevelCampType.Name);
		campDescriptionLabel.text = LocalizationManager.GetText("Camp.Description." + nextLevelCampType.Name);
		defaultPopup.SetPayButton(LocalizationManager.GetText("Popup.NewCamp.Button.Move"), GameManager.Instance.playerModel.CampMover.GetCashier());
		defaultPopup.ShowPayButtons();
		defaultPopup.HideInstantPayButton();
		unlocksListPanel.SetCampUnlocks(nextLevelCampType, campMover.GetCampSubtype(nextLevelCampType));
	}

	public void OnMoveCamp()
	{
		TwoOptionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.TwoOptionPopup) as TwoOptionPopup;
		obj.SetContent(LocalizationManager.GetText("Popup.MoweCampConfirmation.Title"), LocalizationManager.GetText("Popup.MoweCampConfirmation.Message"));
		obj.SetOption1ButtonLabel(LocalizationManager.GetText("Popup.MoweCampConfirmation.Button.Yes"));
		obj.SetOption2ButtonLabel(LocalizationManager.GetText("Popup.MoweCampConfirmation.Button.No"));
		obj.SetCallbacks(OnMoveCampConfirmed);
		obj.Open();
	}

	public void OnMoveCampConfirmed()
	{
		Close();
	}
}
