using TWDModel;

public class OutpostAttackNotificationListPanel : ScrollableListPanel<OutpostAttackNotificationModel>
{
	private void OnEnable()
	{
		SetNotificationsCards();
	}

	public void SetNotificationsCards()
	{
		SetCards(GameManager.Instance.playerModel.OutpostModel.OutpostAttackNotificationModels);
	}
}
