using TWDModel;
using UnityEngine;

public class GuildBattleRewardCurrencyBonus : MonoBehaviour
{
	public GuildBattleMapSectorModel Model;

	[SerializeField]
	private UISprite currencyIcon;

	[SerializeField]
	private UILabel currencyAmount;

	public void UpdateUI()
	{
		if (GuildWarHelper.GetCurrentBattle().GetPersonalGuildBattleSectorCompletionBonus(Model.SectorId) is RewardCurrency rewardCurrency)
		{
			HelpersUI.SetSprite(currencyIcon, HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType));
			HelpersUI.SetContentToLabel(currencyAmount, rewardCurrency.Amount.ToString());
			Helpers.GameObjectSetActive(base.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}
}
