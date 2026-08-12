using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class MedicTentSurvivorsQueue : ScrollableListPanel<SurvivorModel>
{
	private MedicTentModel medicTentModel;

	protected override void Awake()
	{
		base.Awake();
		medicTentModel = GameManager.Instance.playerModel.Camp.GetBuilding("MedicTent") as MedicTentModel;
	}

	private void OnEnable()
	{
		medicTentModel.Changed += OnMedicTentChanged;
		Init();
	}

	private void OnDisable()
	{
		medicTentModel.Changed -= OnMedicTentChanged;
	}

	private void Init()
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		foreach (TimedQueueItemModel item in medicTentModel.TimedQueueModel.Queued)
		{
			list.Add(item.Item as SurvivorModel);
		}
		SetCards(list);
	}

	protected override void SetCard(UIListCard<SurvivorModel> card)
	{
		base.SetCard(card);
		((SurvivorCard)card).ShowEquipmentContainers(show: false);
	}

	private void OnMedicTentChanged(ModelObject model, string changed, object args)
	{
		if (changed == "EventStatusUpdated")
		{
			Init();
		}
	}
}
