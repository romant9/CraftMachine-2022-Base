using System.Collections;
using BaseModel;
using TWDModel;
using UnityEngine;

public class MedicTentPopup : HUDElement
{
	[SerializeField]
	[Tooltip("The slot containing the survivor being cured.")]
	private GameObject[] curedSurvivorsSlots;

	[SerializeField]
	[Tooltip("The slot containing the survivor being cured that are locked.")]
	private GameObject[] curedSurvivorsLockedSlots;

	[SerializeField]
	[Tooltip("The label of the slot containing the survivor being cured that are locked.")]
	private UILabel[] curedSurvivorsLockedSlotsLabel;

	[SerializeField]
	private GameObject survivorCardPrefab;

	[SerializeField]
	private PayButton healAllButton;

	private MedicTentModel medicTentModel;

	private SurvivorCard[] survivorCards;

	private void Awake()
	{
		medicTentModel = GameManager.Instance.playerModel.Camp.GetBuilding("MedicTent") as MedicTentModel;
		survivorCards = new SurvivorCard[medicTentModel.MaxNumberSurvivorsCuredSlotsUnlockable];
	}

	public override void Open()
	{
		base.Open();
		for (int i = 0; i < medicTentModel.MaxNumberSurvivorsCured; i++)
		{
			curedSurvivorsSlots[i].SetActive(value: true);
			curedSurvivorsLockedSlots[i].SetActive(value: false);
		}
		for (int j = medicTentModel.MaxNumberSurvivorsCured; j < medicTentModel.MaxNumberSurvivorsCuredSlotsUnlockable; j++)
		{
			curedSurvivorsSlots[j].SetActive(value: false);
			curedSurvivorsLockedSlots[j].SetActive(value: true);
			curedSurvivorsLockedSlotsLabel[j].text = LocalizationManager.GetText("Popup.MedicTent.SlotUnlockAt{Level}", GetSlotUnlockLevel(j));
		}
		StartCoroutine(SetHealingSurvivors(instant: true));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_medictent");
	}

	private int GetSlotUnlockLevel(int slotIndex)
	{
		int maximumUpgradeLevel = medicTentModel.gameEconomyData.GetMaximumUpgradeLevel("MedicTent");
		for (int i = medicTentModel.Level; i <= maximumUpgradeLevel; i++)
		{
			BuildingUpgradeLevel buildingUpgradeLevel = medicTentModel.gameEconomyData.GetBuildingUpgradeLevel("MedicTent", i);
			if (buildingUpgradeLevel != null && buildingUpgradeLevel.MedicSlotsAmount == slotIndex + 1)
			{
				return i;
			}
		}
		return 99;
	}

	private void OnEnable()
	{
		medicTentModel.Changed += OnMedicTentChanged;
	}

	private void OnDisable()
	{
		medicTentModel.Changed -= OnMedicTentChanged;
	}

	private IEnumerator SetHealingSurvivors(bool instant)
	{
		if (!instant)
		{
			yield return new WaitForSeconds(0.5f);
		}
		SurvivorCard[] array = survivorCards;
		foreach (SurvivorCard survivorCard in array)
		{
			if (survivorCard != null)
			{
				survivorCard.gameObject.SetActive(value: false);
			}
		}
		int num = 0;
		foreach (TimedQueueItemModel item in medicTentModel.TimedQueueModel.Active)
		{
			if (num >= survivorCards.Length || survivorCards[num] == null)
			{
				survivorCards[num] = Helpers.InstantiateToParent(survivorCardPrefab, curedSurvivorsSlots[num]).GetComponent<SurvivorCard>();
			}
			survivorCards[num].gameObject.SetActive(value: true);
			survivorCards[num].Item = item.Item as SurvivorModel;
			survivorCards[num].ShowEquipmentContainers(show: false);
			survivorCards[num].UpdateUI();
			num++;
		}
		if (medicTentModel.HasPatients)
		{
			healAllButton.gameObject.SetActive(value: true);
			healAllButton.UpdateUI(medicTentModel.GetFinishAllCashier(), LocalizationManager.GetText("Popup.MedicTent.Button.CureAllSurvivors"));
		}
		else
		{
			healAllButton.gameObject.SetActive(value: false);
		}
		yield return null;
	}

	private void OnMedicTentChanged(ModelObject model, string changed, object args)
	{
		if (changed == "EventStatusUpdated")
		{
			StartCoroutine(SetHealingSurvivors(instant: false));
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_heal");
		}
	}

	public void OnSpeedUpCuringAllSurvivors()
	{
		ConsumeCurrencyCommandUtils.Execute(new SpeedUpCuringAllSurvivorsCommand
		{
			Cashier = medicTentModel.GetFinishAllCashier()
		});
	}
}
