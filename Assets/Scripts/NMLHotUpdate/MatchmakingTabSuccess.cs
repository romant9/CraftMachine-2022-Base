using TWDModel;
using UnityEngine;

public class MatchmakingTabSuccess : MatchmakingTabBase
{
	[Header("Buttons")]
	[SerializeField]
	private UIButton AttackButton;

	[SerializeField]
	private PayButton NextMatchButton;

	[Header("Match Details")]
	[SerializeField]
	private UILabel PlayerNameLabel;

	[SerializeField]
	private UILabel TotalInfluenceAmountLabel;

	[SerializeField]
	private UILabel OutpostNameLabel;

	[SerializeField]
	private UILabel OutpostLevelLabel;

	[SerializeField]
	private UISprite OutpostTierEmblem;

	[SerializeField]
	private GameObject OutpostLevelParent;

	[SerializeField]
	private OutpostDetailsPanelMatchMaking outpostDetails;

	[SerializeField]
	private UISprite BackgroundSprite;

	[SerializeField]
	private DefenderCard[] DefendersCardsList;

	[Header("Currently not used")]
	[SerializeField]
	private OutpostSliceEdit OutpostPreview;

	public override void Activate()
	{
		base.Activate();
		NextMatchButton.UpdateUI(GameManager.Instance.playerModel.OutpostModel.GetNextMatchCashier());
		if (OutpostPreview != null)
		{
			OutpostPreview.gameObject.SetActive(value: false);
		}
		if (PlayerNameLabel != null)
		{
			PlayerNameLabel.text = GameManager.Instance.GetFilteredText(base.ParentPopup.GetCurrentMatchInfoSurvivorName());
		}
		if (TotalInfluenceAmountLabel != null)
		{
			TotalInfluenceAmountLabel.text = base.ParentPopup.GetOpponentTotalInfluence().ToString();
		}
		if (OutpostLevelLabel != null)
		{
			OutpostLevelLabel.text = base.ParentPopup.GetCurrentMatchInfo().DefendingOutpostWalkerPower.ToString();
		}
		if (OutpostLevelParent != null)
		{
			OutpostLevelParent.SetActive(value: true);
		}
		if (OutpostNameLabel != null)
		{
			OutpostTemplateDefinition outpostTemplateDefinitionForMissionId = GameManager.Instance.gameEconomyData.GetOutpostTemplateDefinitionForMissionId(base.ParentPopup.GetCurrentMatchInfo().OutpostLevelModel.BaseRunLocationID);
			OutpostNameLabel.text = outpostTemplateDefinitionForMissionId.Id;
		}
		if (OutpostTierEmblem != null)
		{
			string tierEmblemIconName = HelpersGfx.GetTierEmblemIconName(base.ParentPopup.GetOpponentOutpostTierId());
			if (!string.IsNullOrEmpty(tierEmblemIconName))
			{
				OutpostTierEmblem.spriteName = tierEmblemIconName;
				OutpostTierEmblem.gameObject.SetActive(value: true);
			}
			else
			{
				OutpostTierEmblem.gameObject.SetActive(value: false);
			}
		}
		if (outpostDetails != null)
		{
			outpostDetails.CurrentMatchInfo = base.ParentPopup.GetCurrentMatchInfo();
			outpostDetails.CurrentMatchSurviviorName = base.ParentPopup.GetCurrentMatchInfoSurvivorName();
			outpostDetails.CurrentMatchPlayerHashedId = base.ParentPopup.GetCurrentMatchPlayerHashedId();
			outpostDetails.UpdateUI();
		}
		if (BackgroundSprite != null)
		{
			OutpostTemplateDefinition outpostTemplateDefinitionForMissionId2 = GameManager.Instance.gameEconomyData.GetOutpostTemplateDefinitionForMissionId(base.ParentPopup.GetCurrentMatchInfo().OutpostLevelModel.BaseRunLocationID);
			BackgroundSprite.spriteName = HelpersGfx.GetOutpostBackgroundSpriteName(outpostTemplateDefinitionForMissionId2);
		}
		if (DefendersCardsList == null || DefendersCardsList.Length == 0 || base.ParentPopup.GetCurrentMatchInfo() == null)
		{
			return;
		}
		MatchInfo currentMatchInfo = base.ParentPopup.GetCurrentMatchInfo();
		for (int i = 0; i < currentMatchInfo.DefendingSurvivorClasses.Count; i++)
		{
			SurvivorModel survivorModel = new SurvivorModel();
			survivorModel.SurvivorClass = currentMatchInfo.DefendingSurvivorClasses[i];
			survivorModel.SurvivorRarityLevel = currentMatchInfo.DefendingSurvivorRarityLevels[i];
			survivorModel.SurvivorName = currentMatchInfo.DefendingSurvivorNames[i];
			survivorModel.Level = currentMatchInfo.DefendingSurvivorLevels[i];
			if (DefendersCardsList.Length > i && DefendersCardsList[i] != null)
			{
				DefendersCardsList[i].LimitedSurvivorModel = survivorModel;
				DefendersCardsList[i].UpdateUI();
			}
		}
	}

	public void OnClickAttack()
	{
		if (base.ParentPopup != null)
		{
			base.ParentPopup.AttackCurrentMatch();
		}
	}

	public void OnClickNext()
	{
		base.ParentPopup.OnNextMatch();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
	}
}
