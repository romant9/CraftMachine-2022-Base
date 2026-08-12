using TWDModel;

public class SurvivalManualPlotGuidePopup : HUDElement
{
	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
		Helpers.SetSurvivalManualPlotGuideOpened(on: true);
	}

	public override void Close()
	{
		base.Close();
	}

	private void OnUiEvent(string type, object parameter)
	{
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}

	public void OnclickNext()
	{
		SurvivalManualMainPopup survivalManualMainPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SurvivalManualMainPopup, HUDManager.Instance.UIContainerTopCameras) as SurvivalManualMainPopup;
		if (survivalManualMainPopup != null)
		{
			survivalManualMainPopup.Open();
		}
		Close();
	}
}
