using UnityEngine;

public class OutpostPopupBattleLog : HUDElement
{
	public GameObject HeaderContainer;

	public UILabel HeaderLabel;

	public UILabel InfluenceLabel;

	public UISprite InflucenIcon;

	public bool BackClose { get; set; }

	public void OnEnable()
	{
		UpdateUI();
		BackClose = false;
	}

	public override void OnClickClose()
	{
		if (TutorialView.Allowed("Close"))
		{
			Close();
			if (!BackClose)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopupManagement).Open();
			}
		}
	}

	public override void UpdateUI()
	{
		if (GameManager.Instance.playerModel.OutpostModel.StoredLevelModel != null)
		{
			InfluenceLabel.text = GameManager.Instance.playerModel.RankingScore.ToString();
		}
		else
		{
			InfluenceLabel.text = "";
		}
	}
}
