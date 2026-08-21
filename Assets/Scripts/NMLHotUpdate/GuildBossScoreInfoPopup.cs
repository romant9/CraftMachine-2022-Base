using System;
using TWDModel;
using UnityEngine;

public class GuildBossScoreInfoPopup : HUDElement
{
	[SerializeField]
	private UILabel[] scoreLabels;

	[SerializeField]
	private UILabel scoreMultiplierValueLabel;

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		CombatModel combatModel = GameManager.Instance?.playerModel?.Combat;
		if (combatModel == null || !combatModel.IsGuildBossMission)
		{
			return;
		}
		int value = (int)Math.Round(combatModel.GuildBossPoint);
		UIRollingNumberUtil.SetValue(scoreLabels, value);
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager != null)
		{
			double myTowerBBossScoreMultiplier = worldBossModelManager.GetMyTowerBBossScoreMultiplier();
			int num = (int)Math.Max(0.0, Math.Round((myTowerBBossScoreMultiplier - 1.0) * 100.0));
			if (scoreMultiplierValueLabel != null)
			{
				scoreMultiplierValueLabel.text = "+" + num + "%";
			}
		}
	}
}
