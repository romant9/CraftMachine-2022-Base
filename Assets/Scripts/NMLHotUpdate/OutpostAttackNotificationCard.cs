using TWDModel;
using UnityEngine;

public class OutpostAttackNotificationCard : UIListCard<OutpostAttackNotificationModel>
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel leveLabel;

	public override void UpdateUI()
	{
		base.UpdateUI();
		nameLabel.text = base.Item.PlayerName;
		leveLabel.text = base.Item.Level.ToString();
	}

	public void OnInfo()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnReplay()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnRevenge()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}
}
