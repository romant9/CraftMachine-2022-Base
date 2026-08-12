using UnityEngine;

public class PlayerHubPersonalStatCard : UIListCard<PlayerHubPersonalStatCardItem>
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel valueLabel;

	public override void UpdateUI()
	{
		base.UpdateUI();
		nameLabel.text = LocalizationManager.GetText("Statistic." + base.Item.Type);
		valueLabel.text = base.Item.Value.ToString();
	}
}
