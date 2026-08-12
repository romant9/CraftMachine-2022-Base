using BaseModel;
using TWDModel;
using UnityEngine;

public class CollectIndicator : BuildingIndicator
{
	public UISprite GlowSprite;

	[SerializeField]
	private Color cannotCollectColor = Color.red;

	private Color originalColor;

	private bool shown;

	public bool IsVisible => shown;

	private void Start()
	{
		originalColor = GlowSprite.color;
		shown = false;
		UpdateVisualisation();
		GameManager.Instance.playerModel.Changed += OnPlayerChange;
		base.Building.Model.Changed += OnBuildingChange;
	}

	private void OnDestroy()
	{
		GameManager.Instance.playerModel.Changed -= OnPlayerChange;
		base.Building.Model.Changed -= OnBuildingChange;
	}

	private void UpdateVisualisation()
	{
		bool flag = true;
		if (shown && flag)
		{
			setActive(base.transform.GetChild(0).gameObject, value: true);
			GlowSprite.color = (base.Building.Model.CanCollect ? originalColor : cannotCollectColor);
		}
		else
		{
			setActive(base.transform.GetChild(0).gameObject, value: false);
		}
	}

	private void setActive(GameObject obj, bool value)
	{
		if (obj != null)
		{
			if (value && !obj.activeSelf)
			{
				obj.SetActive(value);
			}
			else if (!value && obj.activeSelf)
			{
				obj.SetActive(value);
			}
		}
	}

	private void Update()
	{
		shown = false;
		bool flag = base.Building.Model.Camp.HasMaximumStorages(base.Building.Model.Producer.CurrencyType) && !base.Building.Model.CanCollect;
		if (!shown && !flag && base.Building.Model.Producer.HasEnoughToCollect)
		{
			shown = true;
		}
		UpdateVisualisation();
	}

	private void ProducerCarCameBack(GameObject buildingGameObject)
	{
		if (!shown && buildingGameObject == base.Building.gameObject && base.Building.Model.Producer.HasEnoughToCollect)
		{
			shown = true;
			UpdateVisualisation();
		}
	}

	private void OnPlayerChange(ModelObject m, string changed, object args)
	{
		if (changed == "currencyChangedEvent")
		{
			UpdateVisualisation();
		}
	}

	private void OnBuildingChange(ModelObject m, string changed, object args)
	{
		if (changed == "collected" && shown)
		{
			shown = false;
			UpdateVisualisation();
		}
	}

	public void OnClick()
	{
		TWDModelResult result = TWDModelResult.Error;
		if (base.Building.Model.CanCollect)
		{
			shown = false;
			result = base.Building.Collect();
			UpdateVisualisation();
		}
		ShowCollectError(base.Building.Model.Producer, result);
		EventManager.NotifyClick("Collect_" + base.Building.Model.Producer.CurrencyType.ToString() + "_Indicator");
	}

	public static void ShowCollectError(ProducerModel producerModel, TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			return;
		}
		if (producerModel.Amount > 0)
		{
			if (!CampView.Instance.Model.HasMaximumStorages(producerModel.CurrencyType))
			{
				HUDNotification.Error(LocalizationManager.GetText("Error.NoStorage." + producerModel.CurrencyType));
			}
		}
		else if (producerModel.LastCollectedAmount == 0)
		{
			HUDNotification.Error("Nothing to collect");
		}
		else
		{
			HUDNotification.Error("Unable to collect");
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/invalid_action");
	}
}
