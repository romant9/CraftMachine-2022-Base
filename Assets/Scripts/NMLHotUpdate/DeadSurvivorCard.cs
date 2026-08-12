using TWDModel;
using UnityEngine;

public class DeadSurvivorCard : UIListCard<DeadSurvivorModel>
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel killsLabel;

	[SerializeField]
	private UILabel missionsLabel;

	[SerializeField]
	private UILabel numberDaysAliveLabel;

	[SerializeField]
	private UILabel deathDayLabel;

	public override void UpdateUI()
	{
		nameLabel.text = base.Item.Name;
		numberDaysAliveLabel.text = "Days";
		killsLabel.text = base.Item.Statistics.NumberWalkersKilled.ToString();
		missionsLabel.text = base.Item.Statistics.NumberMissionPlayed.ToString();
		numberDaysAliveLabel.text = base.Item.Statistics.GetNumberDaysAlive().ToString();
		deathDayLabel.text = base.Item.Statistics.DeathDate.ToString(LocalizationManager.GetText("DateFormat"));
	}
}
