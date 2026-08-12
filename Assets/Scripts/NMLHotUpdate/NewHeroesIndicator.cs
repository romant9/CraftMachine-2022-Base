using BaseModel;
using TWDModel;
using UnityEngine;

public class NewHeroesIndicator : MonoBehaviour
{
	private void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed += OnPlayerChanged;
			UpdateUI();
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.Changed -= OnPlayerChanged;
		}
	}

	private void OnPlayerChanged(ModelObject modelObject, string changed, object args)
	{
		if (changed == "currencyChangedEvent" && args != null && args.GetType() == typeof(CurrencyModel))
		{
			string heroId = SurvivorToken.GetHeroId(((CurrencyModel)args).Type);
			GameManager instance = GameManager.Instance;
			if (!string.IsNullOrEmpty(heroId) && instance.gameEconomyData.GetActorDefinition(heroId) != null)
			{
				UpdateUI();
			}
		}
	}

	public void UpdateUI()
	{
		GameManager instance = GameManager.Instance;
		bool flag = false;
		if (instance.playerModel != null)
		{
			for (int i = 0; i < instance.playerModel.Currencies.Count; i++)
			{
				CurrencyModel currencyModel = instance.playerModel.Currencies[i];
				string heroId = SurvivorToken.GetHeroId(currencyModel.Type);
				if (!string.IsNullOrEmpty(heroId) && instance.gameEconomyData.GetActorDefinition(heroId) != null)
				{
					Cashier heroUnlockCashier = instance.playerModel.SurvivorContainer.GetHeroUnlockCashier(currencyModel.Type);
					if (instance.playerModel.GetCurrency(currencyModel.Type).Value >= heroUnlockCashier.GetTotalCost(currencyModel.Type) && !GameManager.Instance.playerModel.SurvivorContainer.HasHero(heroId))
					{
						flag = true;
						break;
					}
				}
			}
		}
		if (flag)
		{
			NGUITools.SetActiveChildren(base.gameObject, state: true);
		}
		else
		{
			NGUITools.SetActiveChildren(base.gameObject, state: false);
		}
	}
}
