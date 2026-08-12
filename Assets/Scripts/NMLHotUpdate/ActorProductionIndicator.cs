using BaseModel;
using TWDModel;
using UnityEngine;

public class ActorProductionIndicator : HUDElementFollowTarget
{
	public ActorView ParentView;

	public UISprite ResourceIcon;

	public BuildingView BuildingProducer { get; set; }

	private SurvivorModel survivor => ParentView.Model as SurvivorModel;

	private void Start()
	{
		UpdateVisualisation();
		GameManager.Instance.playerModel.Changed += OnPlayerChange;
		ParentView.Model.Changed += OnSurvivorChanged;
	}

	private void OnDestroy()
	{
		GameManager.Instance.playerModel.Changed -= OnPlayerChange;
		ParentView.Model.Changed -= OnSurvivorChanged;
	}

	private void UpdateVisualisation()
	{
		ProducerModel producer = survivor.Producer;
		if (producer == null)
		{
			producer = BuildingProducer.Model.Producer;
		}
		if (producer != null && ResourceIcon != null)
		{
			Vector3 localScale = ResourceIcon.transform.localScale;
			if (producer.CurrencyType == CurrencyType.Diamonds)
			{
				localScale *= 3f;
			}
			ResourceIcon.spriteName = HelpersGfx.GetCurrencyIconName(producer.CurrencyType);
			ResourceIcon.MakePixelPerfect();
			ResourceIcon.transform.localScale = localScale;
		}
		FollowTarget(ParentView.IndicatorParent);
	}

	private void OnPlayerChange(ModelObject m, string changed, object args)
	{
	}

	private void OnSurvivorChanged(ModelObject m, string changed, object args)
	{
	}

	public void OnClick()
	{
		TWDModelResult tWDModelResult = TWDModelResult.Error;
		ProducerModel producerModel = null;
		if (survivor.Producer != null && survivor.CanCollectProduction)
		{
			producerModel = survivor.Producer;
			tWDModelResult = Helpers.ExecuteCommand(new CollectProductionActorCommand(survivor));
			if (tWDModelResult == TWDModelResult.OK)
			{
				CampView.Instance.BuildingsHud.CreateCollectAnim(survivor.Producer.CurrencyType, ParentView.gameObject, survivor.Producer.LastCollectedAmount);
			}
		}
		else if (BuildingProducer != null)
		{
			producerModel = BuildingProducer.Model.Producer;
			if (BuildingProducer.Model.CanCollect)
			{
				tWDModelResult = BuildingProducer.Collect();
			}
		}
		if (producerModel != null && tWDModelResult != TWDModelResult.OK)
		{
			if (producerModel.Amount > 0)
			{
				if (!CampView.Instance.Model.HasMaximumStorages(producerModel.CurrencyType))
				{
					HUDNotification.Error(LocalizationManager.GetText("Error.NoStorage." + producerModel.CurrencyType));
				}
			}
			else
			{
				HUDNotification.Error(LocalizationManager.GetText("Error.CannotCollect"));
			}
		}
		UpdateVisualisation();
	}
}
