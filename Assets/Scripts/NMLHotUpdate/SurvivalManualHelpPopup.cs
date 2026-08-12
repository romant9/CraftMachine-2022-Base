using TWDModel;
using UnityEngine;

public class SurvivalManualHelpPopup : HUDElement
{
	public enum HelpType
	{
		MainTopHelp = 0,
		MainBottomHelp = 1,
		StoriesHelp = 2
	}

	[SerializeField]
	private GameObject Content_MainTopHelp;

	[SerializeField]
	private GameObject Content_MainBottomHelp;

	[SerializeField]
	private GameObject Content_StoriesHelp;

	[SerializeField]
	private UILabel Title;

	private HelpType helpType;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public void Open(HelpType helpType)
	{
		base.Open();
		this.helpType = helpType;
		UpdateUI();
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
		Helpers.GameObjectSetActive(Content_MainTopHelp, value: false);
		Helpers.GameObjectSetActive(Content_MainBottomHelp, value: false);
		Helpers.GameObjectSetActive(Content_StoriesHelp, value: false);
		base.UpdateUI();
		switch (helpType)
		{
		case HelpType.StoriesHelp:
			Title.text = LocalizationManager.GetText("SurvivalManual_Help_Title_31");
			Helpers.GameObjectSetActive(Content_StoriesHelp, value: true);
			break;
		case HelpType.MainBottomHelp:
			Title.text = LocalizationManager.GetText("SurvivalManual_Help_Title_21");
			Helpers.GameObjectSetActive(Content_MainBottomHelp, value: true);
			break;
		case HelpType.MainTopHelp:
			Title.text = LocalizationManager.GetText("SurvivalManual_Help_Title_11");
			Helpers.GameObjectSetActive(Content_MainTopHelp, value: true);
			break;
		}
	}
}
