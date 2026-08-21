using TWDModel;
using UnityEngine;

public class WorldBossRecallPopup : HUDElement
{
	[Header("Bundle Items List")]
	[SerializeField]
	private UILabel descLabel;

	[SerializeField]
	private UILabel costLabel;

	[SerializeField]
	private UIButton getButton;

	[SerializeField]
	private UIButton getGrayButton;

	private WorldBossReturningTeamView _returningTeam;

	private int _goldCost;

	public void SetReturningTeam(WorldBossReturningTeamView returningTeam)
	{
		_returningTeam = returningTeam;
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (_returningTeam != null && worldBossModelManager != null)
		{
			_goldCost = worldBossModelManager.GetInstantReturnGoldCost(_returningTeam.CapturePoint, _returningTeam.ReturningTeamId);
			if (_goldCost <= 0)
			{
				_goldCost = _returningTeam.InstantReturnGoldCost;
			}
			HelpersUI.SetContentToLabel(descLabel, LocalizationManager.GetText("World.Boss.PVP.QuickReturnAsk", _goldCost));
			HelpersUI.SetContentToLabel(costLabel, _goldCost.ToString());
			bool flag = (GameManager.Instance?.playerModel?.GetCurrency(CurrencyType.Diamonds)?.Value).GetValueOrDefault() >= _goldCost;
			Helpers.GameObjectSetActive(getButton?.gameObject, flag);
			Helpers.GameObjectSetActive(getGrayButton?.gameObject, !flag);
		}
	}

	public void OnClickGetButton()
	{
		if (_returningTeam == null)
		{
			return;
		}
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager != null && worldBossModelManager.WorldBossGuildFullSnapshot != null)
		{
			int currentSeasonId = worldBossModelManager.GetCurrentSeasonId();
			int currentCycleId = worldBossModelManager.GetCurrentCycleId();
			if (Helpers.ExecuteCommand(new WorldBossInstantReturnCommand(currentSeasonId, currentCycleId, _returningTeam.CapturePoint, _returningTeam.ReturningTeamId, _goldCost)) == TWDModelResult.OK)
			{
				Close();
			}
		}
	}
}
