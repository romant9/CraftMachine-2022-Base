using System;
using System.Collections.Generic;
using System.Globalization;
using TWDModel;

public static class WorldBossBuildingBuffDescHelper
{
	private sealed class BuildingBuffTier
	{
		public double ThresholdHours;

		public string EffectValue;
	}

	private const string TowerACapturePoint = "TOWER-A";

	private const string TowerBCapturePoint = "TOWER-B";

	private const string DepotCapturePoint = "DEPOT";

	private const string NextTierLocalizationKey = "World.Boss.PVP.BuildingEff.Next";

	public static string FormatBuffDescNow(WorldBossBattlegroundDefinition definition, WorldBossBuildingBuffView buff)
	{
		if (buff == null)
		{
			return string.Empty;
		}
		if (!TryResolveDisplayTier(buff, 0, out var thresholdHours, out var effectValue))
		{
			return string.Empty;
		}
		return FormatBuffDescTier(definition, buff.CapturePoint, thresholdHours, effectValue, buff.ExtraBossBattleTimes);
	}

	public static bool ShouldShowDescNext(WorldBossBuildingBuffView buff)
	{
		double thresholdHours;
		string effectValue;
		return TryResolveDisplayTier(buff, 1, out thresholdHours, out effectValue);
	}

	public static string FormatBuffDescNext(WorldBossBattlegroundDefinition definition, WorldBossBuildingBuffView buff)
	{
		if (buff == null)
		{
			return string.Empty;
		}
		if (!TryResolveDisplayTier(buff, 1, out var thresholdHours, out var effectValue))
		{
			return string.Empty;
		}
		string text = FormatBuffDescTier(definition, buff.CapturePoint, thresholdHours, effectValue, buff.ExtraBossBattleTimes);
		return LocalizationManager.GetText("World.Boss.PVP.BuildingEff.Next", text);
	}

	private static bool TryResolveDisplayTier(WorldBossBuildingBuffView buff, int offsetFromDisplayedTier, out double thresholdHours, out string effectValue)
	{
		thresholdHours = 0.0;
		effectValue = "0";
		if (buff == null || !TryGetConfiguredTiers(buff.CapturePoint, out var tiers))
		{
			return false;
		}
		int num = FindCurrentTierIndex(buff, tiers);
		int num2 = ((num >= 0) ? num : 0) + offsetFromDisplayedTier;
		if (num2 < 0 || num2 >= tiers.Count)
		{
			return false;
		}
		thresholdHours = tiers[num2].ThresholdHours;
		effectValue = tiers[num2].EffectValue;
		return true;
	}

	private static int FindCurrentTierIndex(WorldBossBuildingBuffView buff, List<BuildingBuffTier> tiers)
	{
		if (!buff.IsOccupiedByMe)
		{
			return -1;
		}
		for (int i = 0; i < tiers.Count; i++)
		{
			if (Math.Abs(tiers[i].ThresholdHours - buff.CurrentThresholdHours) < double.Epsilon && string.Equals(tiers[i].EffectValue, buff.CurrentValue, StringComparison.Ordinal))
			{
				return i;
			}
		}
		return -1;
	}

	private static bool TryGetConfiguredTiers(string capturePoint, out List<BuildingBuffTier> tiers)
	{
		tiers = new List<BuildingBuffTier>();
		WorldBossConfig worldBossConfig = GameManager.Instance?.gameEconomyData?.WorldBossConfig;
		if (worldBossConfig == null)
		{
			return false;
		}
		string text;
		string text2;
		if (IsTowerA(capturePoint))
		{
			text = worldBossConfig.TowerA;
			text2 = worldBossConfig.TowerAEff;
		}
		else if (string.Equals(capturePoint, "TOWER-B", StringComparison.OrdinalIgnoreCase))
		{
			text = worldBossConfig.TowerB;
			text2 = worldBossConfig.TowerBEff;
		}
		else
		{
			if (!IsDepot(capturePoint))
			{
				return false;
			}
			text = worldBossConfig.Depot;
			text2 = worldBossConfig.DepotEff;
		}
		string[] array = (text ?? string.Empty).Split(';');
		string[] array2 = (text2 ?? string.Empty).Split(';');
		int num = Math.Min(array.Length, array2.Length);
		for (int i = 0; i < num; i++)
		{
			if (double.TryParse(array[i]?.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
			{
				tiers.Add(new BuildingBuffTier
				{
					ThresholdHours = result,
					EffectValue = (string.IsNullOrEmpty(array2[i]?.Trim()) ? "0" : array2[i].Trim())
				});
			}
		}
		tiers.Sort((BuildingBuffTier left, BuildingBuffTier right) => left.ThresholdHours.CompareTo(right.ThresholdHours));
		return tiers.Count > 0;
	}

	private static string FormatBuffDescTier(WorldBossBattlegroundDefinition definition, string capturePoint, double thresholdHours, string effectValue, long extraBossBattleTimes)
	{
		if (definition == null || string.IsNullOrEmpty(definition.BuildingEffDesc))
		{
			return thresholdHours.ToString(CultureInfo.InvariantCulture) + (effectValue ?? "0");
		}
		string capturePoint2 = ((!string.IsNullOrEmpty(capturePoint)) ? capturePoint : definition.CapturePoint);
		if (IsDepot(capturePoint2))
		{
			return LocalizationManager.GetText(definition.BuildingEffDesc, thresholdHours, effectValue ?? "0", extraBossBattleTimes);
		}
		if (IsTowerA(capturePoint2))
		{
			return LocalizationManager.GetText(definition.BuildingEffDesc, thresholdHours, FormatTowerAScorePerMinuteDisplay(effectValue));
		}
		return LocalizationManager.GetText(definition.BuildingEffDesc, thresholdHours, effectValue ?? "0");
	}

	private static string FormatTowerAScorePerMinuteDisplay(string effectValue)
	{
		if (string.IsNullOrEmpty(effectValue))
		{
			return "0";
		}
		if (double.TryParse(effectValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return ((long)Math.Floor(result)).ToString(CultureInfo.InvariantCulture);
		}
		return effectValue;
	}

	private static bool IsTowerA(string capturePoint)
	{
		return string.Equals(capturePoint, "TOWER-A", StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsDepot(string capturePoint)
	{
		return string.Equals(capturePoint, "DEPOT", StringComparison.OrdinalIgnoreCase);
	}
}
