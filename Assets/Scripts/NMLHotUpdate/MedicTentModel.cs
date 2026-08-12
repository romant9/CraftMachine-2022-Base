using System;
using BaseModel;
using Newtonsoft.Json;
using TWDModel;

public class MedicTentModel : BuildingModel
{
	public const string EventStatusUpdated = "EventStatusUpdated";

	public TimedQueueModel TimedQueueModel { get; set; }

	[JsonIgnore]
	public bool HasPatients => TimedQueueModel.TotalTime > 0;

	[JsonIgnore]
	public override bool UpgradeInside
	{
		get
		{
			if (HasPatients)
			{
				return true;
			}
			return false;
		}
	}

	[JsonIgnore]
	public int MaxNumberSurvivorsCured => GetCurrentUpgradeLevel().MedicSlotsAmount;

	[JsonIgnore]
	public int MaxNumberSurvivorsCuredSlotsUnlockable
	{
		get
		{
			int maximumUpgradeLevel = base.gameEconomyData.GetMaximumUpgradeLevel("MedicTent");
			return base.gameEconomyData.GetBuildingUpgradeLevel("MedicTent", maximumUpgradeLevel).MedicSlotsAmount;
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		TimedQueueModel = new TimedQueueModel();
		TimedQueueModel.SetManager(base.manager);
		TimedQueueModel.Initialize();
	}

	public override void Start()
	{
		base.Start();
		TimedQueueModel.Changed += OnTimedQueueChanged;
		TimedQueueModel.UpdateNumberSlots(MaxNumberSurvivorsCured);
	}

	public Cashier GetFinishOneCashier(TWDModelObject item)
	{
		return TimedQueueModel.GetFinishOneCashier(item, PurchaseType.SpeedUpCuringSurvivor);
	}

	public Cashier GetFinishAllCashier()
	{
		return TimedQueueModel.GetFinishAllCashier(PurchaseType.SpeedUpCuringAllSurvivors);
	}

	public TWDModelResult FinishOne(TWDModelObject item)
	{
		return TimedQueueModel.FinishOne(item, PurchaseType.SpeedUpCuringSurvivorForFree);
	}

	public TWDModelResult FinishAll()
	{
		return TimedQueueModel.FinishAll(PurchaseType.SpeedUpCuringAllSurvivors);
	}

	private void OnTimedQueueChanged(ModelObject model, string changed, object args)
	{
		if (changed == "ActionFinishedEvent" && args is SurvivorModel)
		{
			SurvivorModel survivorModel = args as SurvivorModel;
			if (survivorModel.InjuryType != InjuryType.None)
			{
				survivorModel.InjuryType = InjuryType.None;
				survivorModel.MinHitpoints = survivorModel.MaxHitPoints;
				survivorModel.OnInjuryCured();
				NotifyChange("ActionFinishedEvent", survivorModel);
				NotifyChange("EventStatusUpdated");
			}
		}
		if (changed == "ActionUpdatedEvent" && args is SurvivorModel)
		{
			SurvivorModel args2 = args as SurvivorModel;
			NotifyChange("ActionUpdatedEvent", args2);
			NotifyChange("EventStatusUpdated");
		}
	}

	protected override void CompleteUpgrade(Metrics.UpgradeTypes upgradeType)
	{
		base.CompleteUpgrade(upgradeType);
		TimedQueueModel.UpdateNumberSlots(MaxNumberSurvivorsCured);
	}

	public void NewSurvivorInjured(SurvivorModel survivor, int missionLevel, FixedPoint healingTimeModifier)
	{
		if (!TimedQueueModel.Exists(survivor))
		{
			TimedQueueModel.Add(survivor, GetInjuryTime(survivor, missionLevel, healingTimeModifier));
			NotifyChange("EventStatusUpdated");
		}
	}

	private int GetInjuryTime(SurvivorModel survivor, int missionLevel, FixedPoint healingTimeModifier)
	{
		int val = missionLevel;
		if (!base.manager.Player.Tutorial.HasCompletedPart("Phone"))
		{
			val = 1;
		}
		MissionGenerationData missionGenerationData = base.gameEconomyData.GetMissionGenerationData(Math.Max(1, val));
		int num = 0;
		if (survivor.InjuryType == InjuryType.Minor)
		{
			num = missionGenerationData.CuringTimeMinor;
		}
		else if (survivor.InjuryType == InjuryType.Major)
		{
			num = missionGenerationData.CuringTimeMajor;
		}
		else if (survivor.InjuryType == InjuryType.Critical)
		{
			num = missionGenerationData.CuringTimeCritical;
		}
		num -= GetCurrentUpgradeLevel().MedicInjuryTimeBonus;
		num = (int)(num * healingTimeModifier);
		if (base.manager.Player.ActivityManager.TryGetActivityParam(ActivityType.ReducedHealingTimes, out var activityParams))
		{
			num = (int)((FixedPoint)num * (FixedPoint)(100 - int.Parse(activityParams[0])) / 100L);
		}
		else if (base.gameEconomyData.ConfigData.WeeklyEventClassHealTimeReduction == survivor.SurvivorClass)
		{
			num = (int)((FixedPoint)num * (FixedPoint)(100 - base.gameEconomyData.ConfigData.WeeklyEventClassHealTimeReductionPercentage) / 100L);
		}
		if (base.manager.Player.SubscriptionManager.IsSubscriptionActive)
		{
			num = (int)((FixedPoint)num * (FixedPoint)base.manager.GameEconomyData.SubscriptionConfig.RecoveryFactor);
		}
		return Math.Max(1, num);
	}

	public TWDModelResult CureAllSurvivors()
	{
		TWDModelResult num = FinishAll();
		if (num == TWDModelResult.OK)
		{
			NotifyChange("EventStatusUpdated");
		}
		return num;
	}
}
