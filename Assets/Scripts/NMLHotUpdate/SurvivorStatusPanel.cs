using TWDModel;
using UnityEngine;

public class SurvivorStatusPanel : MonoBehaviour
{
	[SerializeField]
	private UISprite backgroundSprite;

	[SerializeField]
	private UILabel statusLabel;

	[SerializeField]
	private Color HealingBgColor;

	[SerializeField]
	private Color HealingWaitBgColor;

	[SerializeField]
	private Color TrainigBgColor;

	private TimedQueueItemModel injuryTimer;

	private MedicTentModel medicTentModel;

	private SurvivorModel survivorModel;

	private string localisationHeailing = "";

	private string localisationHeailingWait = "";

	private string localisationTraining = "";

	public void SetInfo(SurvivorModel survivor)
	{
		localisationHeailing = LocalizationManager.GetText("Popup.SurvivorInfoPopup.Healing");
		localisationHeailingWait = LocalizationManager.GetText("Popup.SurvivorInfoPopup.Waiting");
		localisationTraining = LocalizationManager.GetText("Popup.SurvivorInfoPopup.Training");
		if (survivor != null)
		{
			medicTentModel = GameManager.Instance.playerModel.Camp.GetBuilding("MedicTent") as MedicTentModel;
			if (medicTentModel != null)
			{
				injuryTimer = medicTentModel.TimedQueueModel.GetQueueItemFromItem(survivor);
				survivorModel = survivor;
				Update();
			}
		}
	}

	private void Update()
	{
		if (survivorModel != null && medicTentModel != null && statusLabel != null && injuryTimer != null)
		{
			if (survivorModel.InjuryType != InjuryType.None)
			{
				if (medicTentModel.TimedQueueModel.IsActive(injuryTimer))
				{
					statusLabel.text = localisationHeailing + " " + Helpers.FormatTimeNoZero(injuryTimer.MillisecondsTillCompletion);
					backgroundSprite.color = HealingBgColor;
				}
				else if (medicTentModel.TimedQueueModel.IsQueued(injuryTimer))
				{
					statusLabel.text = localisationHeailingWait;
					backgroundSprite.color = HealingWaitBgColor;
				}
				setActiveTrue();
			}
			else if (survivorModel.IsUpgrading())
			{
				statusLabel.text = localisationTraining + " " + Helpers.FormatTimeNoZero(survivorModel.TimedActionModel.MillisecondsTillCompletion);
				backgroundSprite.color = TrainigBgColor;
				setActiveTrue();
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void setActiveTrue()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
	}
}
