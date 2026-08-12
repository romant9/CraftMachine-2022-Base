using BaseModel;
using TWDModel;
using UnityEngine;

public class BattlePassPopup : HUDElement
{
	[SerializeField]
	private UILabel seasonTitleLabel;

	[SerializeField]
	private UIWidget backgroundColorWidget;

	[SerializeField]
	private GameObject[] regularOnlyObjects;

	public const string BattlePassInfoShown = "BattlePassInfoShown";

	public const string BeginnerBattlePassInfoShown = "BeginnerBattlePassInfoShown";

	public override void Start()
	{
		base.Start();
		GameManager.Instance.playerModel.BattlePass.Changed += BattlePassOnChanged;
		EventManager.OnClick += OnClick;
		UIEvent.OnUIEvent += OnUiEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void OnDestroy()
	{
		GameManager.Instance.playerModel.BattlePass.Changed -= BattlePassOnChanged;
		EventManager.OnClick -= OnClick;
		UIEvent.OnUIEvent -= OnUiEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void BattlePassOnChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "SeasonChanged" && base.gameObject.activeSelf)
		{
			Close();
		}
	}

	public override void Open()
	{
		Helpers.ExecuteCommand(new BattlePassTierRefreshCommand());
		base.Open();
		UpdateUI();
		ShowBattlePassInstructionsIfNecessary();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		BattlePassModel battlePass = GameManager.Instance.playerModel.BattlePass;
		HelpersUI.SetContentToLabel(seasonTitleLabel, LocalizationManager.GetText(battlePass.IsBeginnerBattlePass ? "Popup.BattlePass.BeginnerBattlePassTitle" : $"Popup.BattlePass.Season{battlePass.CurrentSeasonId}.Title"));
		HelpersUI.SetColor(seasonTitleLabel, NGUIText.ParseColor(battlePass.TitleColor));
		HelpersUI.SetColor(backgroundColorWidget, NGUIText.ParseColor(battlePass.BackgroundColor));
		GameObject[] array = regularOnlyObjects;
		for (int i = 0; i < array.Length; i++)
		{
			Helpers.GameObjectSetActive(array[i], !battlePass.IsBeginnerBattlePass);
		}
	}

	private void ShowBattlePassInstructionsIfNecessary()
	{
		BattlePassModel battlePass = GameManager.Instance.playerModel.BattlePass;
		if (TWDPlayerPrefs.GetInt(battlePass.IsBeginnerBattlePass ? "BeginnerBattlePassInfoShown" : "BattlePassInfoShown") == 0 && battlePass.ReachedTier <= 0)
		{
			ShowBattlePassInstructions();
		}
	}

	private void ShowBattlePassInstructions()
	{
		BattlePassModel battlePass = GameManager.Instance.playerModel.BattlePass;
		UIType uiType = (battlePass.IsBeginnerBattlePass ? UIType.BeginnerBattlePassInstructionPopup : UIType.BattlePassInstructionPopup);
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(uiType);
		if (hUDElement != null)
		{
			string key = (battlePass.IsBeginnerBattlePass ? "BeginnerBattlePassInfoShown" : "BattlePassInfoShown");
			hUDElement.Open();
			TWDPlayerPrefs.SetInt(key, 1);
		}
	}

	private void OnUiEvent(string type, object parameter)
	{
		if ((parameter is ShopPopup || parameter is QuestsPopup) && type == "OnPopUpOpen")
		{
			Close();
		}
	}

	private void OnClick(string clickType)
	{
		if (clickType == EventManager.EventTypeClick.MissionHub.ToString())
		{
			Close();
		}
	}

	public void OnKillsProgressBarClicked(GameObject clickedObject)
	{
		TooltipManager.OpenTextBoxWithText(clickedObject, LocalizationManager.GetText("Tooltip.BattlePass.DailyKills.LinearBar"));
	}

	public void OnNextTierClicked(GameObject clickedObject)
	{
		TooltipManager.OpenTextBoxWithText(clickedObject, LocalizationManager.GetText("Tooltip.BattlePass.ProgressBar.NextTier"));
	}

	public void OnMainProgressBarClicked(GameObject clickedObject)
	{
		TooltipManager.OpenTextBoxWithText(clickedObject, LocalizationManager.GetText("Tooltip.BattlePass.ProgressBar.Main"));
	}

	public void OnBattlePassInfoClicked()
	{
		ShowBattlePassInstructions();
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateUI();
	}
}
