using TWDModel;

public class GraveyardListPanel : ScrollableListPanel<DeadSurvivorModel>
{
	private void Start()
	{
		SetCards(GameManager.Instance.playerModel.SurvivorContainer.DeadSurvivors);
	}
}
