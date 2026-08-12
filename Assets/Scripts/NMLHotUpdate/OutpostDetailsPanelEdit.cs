using TWDModel;
using UnityEngine;

public class OutpostDetailsPanelEdit : OutpostDetailsPanelStored
{
	[SerializeField]
	protected UILabel OutpostName;

	public override void UpdateUI()
	{
		bool flag = GameManager.Instance.playerModel.OutpostModel.EditLevelModel != null;
		if (flag)
		{
			if (OutpostName != null)
			{
				OutpostName.text = "";
			}
			if (ProductionTimerLabel != null)
			{
				ProductionTimerLabel.text = GameManager.Instance.playerModel.GetProductionPerHour(CurrencyType.Outpost) + " / h";
			}
			UpdateLevel(GameManager.Instance.playerModel.Camp.GetBuilding("Outpost"));
			UpdateDefendersIconsAndLevel(GameManager.Instance.playerModel.SurvivorContainer.OutpostDefendingSurvivors);
			TutorialView.Instance.ShowButtonSuggest("OutpostPublishButton", GameManager.Instance.playerModel.OutpostModel.OutpostRunLocation == null);
		}
		else
		{
			base.gameObject.SetActive(flag);
		}
	}

	public override void Update()
	{
	}
}
