using TWDModel;
using UnityEngine;

public class WalkerStatisticsPanel : MonoBehaviour
{
	[SerializeField]
	private GameObject upgradingContainer;

	[SerializeField]
	private UILabel upgradingLabel;

	[SerializeField]
	private UILabel labelLevel;

	[SerializeField]
	private UILabel walkerAmountLabel;

	[SerializeField]
	[Tooltip("Damage Prefab")]
	private SurvivorDamageHealthPanel damagePanel;

	[SerializeField]
	[Tooltip("Health Prefab")]
	private SurvivorDamageHealthPanel healthPanel;

	[Header("Equipment Card")]
	[SerializeField]
	private GameObject equipmentPrefab;

	[SerializeField]
	private GameObject weaponPosition;

	[SerializeField]
	private GameObject armorPosition;

	private EquipmentButton weaponCard;

	private EquipmentButton armorCard;

	private OutpostWalkerModel outpostWalkerModel;

	private string localisationTraining = "";

	public void SetInfo(OutpostWalkerModel outpostWalkerModel)
	{
		this.outpostWalkerModel = outpostWalkerModel;
		localisationTraining = LocalizationManager.GetText("Popup.WalkerInfoPopup.Training");
		labelLevel.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.LevelOfSurvivor{Start}{Max}", outpostWalkerModel.Level, outpostWalkerModel.MaxUpgradeLevel);
		walkerAmountLabel.text = outpostWalkerModel.Amount.ToString();
		if (damagePanel != null && outpostWalkerModel != null)
		{
			string amount = outpostWalkerModel.GetDamageForLevel(outpostWalkerModel.Level).ToString() ?? "";
			string baseAmount = outpostWalkerModel.GetDamageForLevel(outpostWalkerModel.Level).ToString() ?? "";
			damagePanel.setInfo(LocalizationManager.GetText("Statistic.Damage"), amount, LocalizationManager.GetText("Popup.SurvivorInfoPopup.Base"), baseAmount);
		}
		if (healthPanel != null && outpostWalkerModel != null)
		{
			string amount2 = outpostWalkerModel.GetHitpointsForLevel(outpostWalkerModel.Level).ToString() ?? "";
			string baseAmount2 = outpostWalkerModel.GetHitpointsForLevel(outpostWalkerModel.Level).ToString() ?? "";
			healthPanel.setInfo(LocalizationManager.GetText("Statistic.Health"), amount2, LocalizationManager.GetText("Popup.SurvivorInfoPopup.Base"), baseAmount2);
		}
	}

	private void Update()
	{
		if (outpostWalkerModel != null && outpostWalkerModel.IsUpgrading())
		{
			upgradingContainer.SetActive(value: true);
			upgradingLabel.text = localisationTraining + " " + Helpers.FormatTimeNoZero(outpostWalkerModel.TimedActionModel.MillisecondsTillCompletion);
		}
		else
		{
			upgradingContainer.SetActive(value: false);
		}
	}
}
