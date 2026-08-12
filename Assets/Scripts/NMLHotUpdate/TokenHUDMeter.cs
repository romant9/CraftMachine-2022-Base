using TWDModel;
using UnityEngine;

public class TokenHUDMeter : MonoBehaviour
{
	[SerializeField]
	private CurrencyType tokenType;

	[SerializeField]
	private UISprite[] speedIcons;

	[SerializeField]
	private UISprite speedIconPlus;

	private CurrencyModel tokenCurrency;

	private CurrencyModel tokenCurrency1;

	private CurrencyModel tokenCurrency2;

	private CurrencyModel tokenCurrency3;

	private CurrencyModel tokenCurrency4;

	private CurrencyModel tokenCurrency5;

	private CurrencyModel tokenCurrency6;

	private CurrencyModel tokenCurrency7;

	private CurrencyModel tokenCurrency8;

	private CurrencyModel tokenCurrency9;

	private CurrencyModel tokenCurrencyPlus;

	public void Setup()
	{
		tokenCurrency = GameManager.Instance.modelManager.Player.GetCurrency(tokenType);
		UpdateTokenCurrencys();
	}

	private void UpdateTokenCurrencys()
	{
		if (tokenType == CurrencyType.EquipmentTokenBP)
		{
			tokenCurrency1 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken1min);
			tokenCurrency2 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken10min);
			tokenCurrency3 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken20min);
			tokenCurrency4 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken1h);
			tokenCurrency5 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken3h);
			tokenCurrency6 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken7h);
			tokenCurrency7 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken14h);
			tokenCurrency8 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentTokenBP);
			tokenCurrencyPlus = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.SuperEquipmentTokenBP);
		}
		else if (tokenType == CurrencyType.BuildingTokenBP)
		{
			tokenCurrency1 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken1min);
			tokenCurrency2 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken5min);
			tokenCurrency3 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken10min);
			tokenCurrency4 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken30min);
			tokenCurrency5 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken1h);
			tokenCurrency6 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken6h);
			tokenCurrency7 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken12h);
			tokenCurrency8 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken24h);
			tokenCurrency9 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingTokenBP);
			tokenCurrencyPlus = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.SuperBuildingTokenBP);
		}
		else if (tokenType == CurrencyType.TrainingTokenBP)
		{
			tokenCurrency1 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken5min);
			tokenCurrency2 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken20min);
			tokenCurrency3 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken1h);
			tokenCurrency4 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken3h);
			tokenCurrency5 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken8h);
			tokenCurrency6 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken16h);
			tokenCurrency7 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingTokenBP);
			tokenCurrencyPlus = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.SuperTrainingTokenBP);
		}
		else if (tokenType == CurrencyType.HealingTokenBP)
		{
			tokenCurrency1 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken1min);
			tokenCurrency2 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken5min);
			tokenCurrency3 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken10min);
			tokenCurrency4 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken1h);
			tokenCurrency5 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken2h);
			tokenCurrency6 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken4h);
			tokenCurrency7 = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingTokenBP);
		}
	}

	public void UpdateTokenHUDMeter()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		if (speedIcons == null || speedIcons.Length <= 8)
		{
			return;
		}
		Helpers.GameObjectSetActive(speedIcons[0], value: false);
		Helpers.GameObjectSetActive(speedIcons[1], value: false);
		Helpers.GameObjectSetActive(speedIcons[2], value: false);
		Helpers.GameObjectSetActive(speedIcons[3], value: false);
		Helpers.GameObjectSetActive(speedIcons[4], value: false);
		Helpers.GameObjectSetActive(speedIcons[5], value: false);
		Helpers.GameObjectSetActive(speedIcons[6], value: false);
		Helpers.GameObjectSetActive(speedIcons[7], value: false);
		Helpers.GameObjectSetActive(speedIcons[8], value: false);
		Helpers.GameObjectSetActive(speedIconPlus, value: false);
		if (tokenCurrency1 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[0], value: true);
			speedIcons[0].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency1.Value.ToString();
			speedIcons[0].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency1.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency1.Type.ToString());
			speedIcons[0].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency.Title;
			UIButton component = speedIcons[0].GetComponent<UIButton>();
			component.onClick.Clear();
			component.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency1);
			}));
		}
		if (tokenCurrency2 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[1], value: true);
			speedIcons[1].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency2.Value.ToString();
			speedIcons[1].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency2.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency2 = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency2.Type.ToString());
			speedIcons[1].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency2.Title;
			UIButton component2 = speedIcons[1].GetComponent<UIButton>();
			component2.onClick.Clear();
			component2.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency2);
			}));
		}
		if (tokenCurrency3 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[2], value: true);
			speedIcons[2].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency3.Value.ToString();
			speedIcons[2].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency3.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency3 = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency3.Type.ToString());
			speedIcons[2].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency3.Title;
			UIButton component3 = speedIcons[2].GetComponent<UIButton>();
			component3.onClick.Clear();
			component3.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency3);
			}));
		}
		if (tokenCurrency4 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[3], value: true);
			speedIcons[3].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency4.Value.ToString();
			speedIcons[3].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency4.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency4 = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency4.Type.ToString());
			speedIcons[3].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency4.Title;
			UIButton component4 = speedIcons[3].GetComponent<UIButton>();
			component4.onClick.Clear();
			component4.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency4);
			}));
		}
		if (tokenCurrency5 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[4], value: true);
			speedIcons[4].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency5.Value.ToString();
			speedIcons[4].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency5.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency5 = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency5.Type.ToString());
			speedIcons[4].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency5.Title;
			UIButton component5 = speedIcons[4].GetComponent<UIButton>();
			component5.onClick.Clear();
			component5.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency5);
			}));
		}
		if (tokenCurrency6 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[5], value: true);
			speedIcons[5].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency6.Value.ToString();
			speedIcons[5].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency6.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency6 = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency6.Type.ToString());
			speedIcons[5].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency6.Title;
			UIButton component6 = speedIcons[5].GetComponent<UIButton>();
			component6.onClick.Clear();
			component6.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency6);
			}));
		}
		if (tokenCurrency7 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[6], value: true);
			speedIcons[6].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency7.Value.ToString();
			speedIcons[6].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency7.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency7 = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency7.Type.ToString());
			speedIcons[6].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency7.Title;
			UIButton component7 = speedIcons[6].GetComponent<UIButton>();
			component7.onClick.Clear();
			component7.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency7);
			}));
		}
		if (tokenCurrency8 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[7], value: true);
			speedIcons[7].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency8.Value.ToString();
			speedIcons[7].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency8.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency8 = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency8.Type.ToString());
			speedIcons[7].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency8.Title;
			UIButton component8 = speedIcons[7].GetComponent<UIButton>();
			component8.onClick.Clear();
			component8.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency8);
			}));
		}
		if (tokenCurrency9 != null)
		{
			Helpers.GameObjectSetActive(speedIcons[8], value: true);
			speedIcons[8].transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrency9.Value.ToString();
			speedIcons[8].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrency9.Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency9 = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrency9.Type.ToString());
			speedIcons[8].transform.Find("time").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency9.Title;
			UIButton component9 = speedIcons[8].GetComponent<UIButton>();
			component9.onClick.Clear();
			component9.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrency9);
			}));
		}
		if (tokenCurrencyPlus != null)
		{
			Helpers.GameObjectSetActive(speedIconPlus, value: true);
			speedIconPlus.spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrencyPlus.Type);
			speedIconPlus.transform.Find("AmountLabel").GetComponent<UILabel>().text = tokenCurrencyPlus.Value.ToString();
			UIButton component10 = speedIconPlus.GetComponent<UIButton>();
			component10.onClick.Clear();
			component10.onClick.Add(new EventDelegate(delegate
			{
				UIEvent.Send("ConsumablesItemClickEvent", tokenCurrencyPlus);
			}));
		}
	}
}
