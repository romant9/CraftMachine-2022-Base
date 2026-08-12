public class PlayerHubNewsListPanel : ScrollableListPanel<PlayerHubNewsItem>
{
	protected override bool LastEntryAtTop => false;

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
		UpdateUI();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "NewsUpdated")
		{
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		SetCards(GameManager.Instance.PlayerHubManager.News);
	}
}
