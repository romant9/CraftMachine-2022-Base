using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class CampDefenseModel : TWDModelObject
	{
		public const string CampDefenseWalkerKilled = "CampDefenseWalkerKilled";

		public ModelList<CampDefenseWalkerModel> Walkers { get; set; }

		public long AccumulatedTime { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			Walkers = new ModelList<CampDefenseWalkerModel>();
		}

		public override void Start()
		{
			base.Start();
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			TutorialModel tutorial = base.manager.Player.Tutorial;
			if (tutorial.Completed && tutorial.HasCompletedPart("WalkerTapping") && deltaTime > 0)
			{
				AccumulatedTime += deltaTime;
			}
		}

		public void Reset()
		{
			Walkers.Clear();
			AccumulatedTime = 0L;
		}

		public void CheckSpawns()
		{
			if (base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return;
			}
			ConfigData configData = base.manager.Player.gameEconomyData.ConfigData;
			long num = configData.CampDefenseSpawnDelay;
			long num2 = configData.CampDefenseWalkerCountPerWave;
			if (base.manager.Player.ActivityManager.TryGetActivityParam(ActivityType.TheHerd, out var activityParams))
			{
				num = (long)ActivityManager.ParseBuildTime(activityParams[0]) * 1000L;
				num2 = long.Parse(activityParams[1]);
			}
			if (AccumulatedTime >= num)
			{
				long num3 = AccumulatedTime / num;
				AccumulatedTime -= num3 * num;
				if (AccumulatedTime < 0)
				{
					AccumulatedTime = 0L;
				}
				num3 = Math.Min(num3, num2 - Walkers.Count);
				for (int i = 0; i < num3; i++)
				{
					CreateWalker();
				}
			}
		}

		public void KillWalker(ActorModel actor)
		{
			CampDefenseWalkerModel campDefenseWalkerModel = actor as CampDefenseWalkerModel;
			LootEntry lootEntry = null;
			if (campDefenseWalkerModel == null || !campDefenseWalkerModel.IsValid())
			{
				return;
			}
			LootManagerModel lootManager = base.manager.Player.LootManager;
			if (base.manager.Player.Tutorial.CurrentPartId == "WalkerTapping")
			{
				lootEntry = ((Walkers.Count != 3 && Walkers.Count != 1) ? base.manager.Player.LootManager.GiveForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies, CurrencyType.Supplies, 50) : base.manager.Player.LootManager.GiveForcedCurrencyLoot(DropCurrenciesProbabilitiesDefinition.DropCurrency.Phone, CurrencyType.Phone, 1));
				base.manager.Metrics.WalkerTapped(lootEntry);
			}
			else
			{
				DropEventDefinition.DropEventType eventType = (base.manager.Player.ActivityManager.IsActivityOpen(ActivityType.TheHerd) ? DropEventDefinition.DropEventType.EventWalkerTapping : DropEventDefinition.DropEventType.WalkerTapping);
				lootManager.ShuffleRewards(new LootEntryGenParams
				{
					eventType = eventType,
					targetLevel = base.manager.GetPlayer().Level
				});
				if (lootManager.CanOpenLootBox())
				{
					lootEntry = lootManager.OpenNextLoot(-1);
					if (lootEntry != null)
					{
						base.manager.Metrics.WalkerTapped(lootEntry);
					}
				}
			}
			base.manager.Player.Blackboard.IncreaseCounter("Counter.DefendWalkersKilled");
			RemoveWalker(campDefenseWalkerModel);
			List<object> args = new List<object> { campDefenseWalkerModel, lootEntry };
			NotifyChange("CampDefenseWalkerKilled", args);
			TWDModelManager tWDModelManager = base.manager;
			tWDModelManager.Player.DailyQuestManager.StartAction("Kill").TargetType = "CampWalker";
			tWDModelManager.Player.DailyQuestManager.CommitAction();
		}

		public void CreateWalker()
		{
			CampDefenseWalkerModel campDefenseWalkerModel = new CampDefenseWalkerModel();
			campDefenseWalkerModel.ActorDefinitionID = "WalkerNormal";
			campDefenseWalkerModel.SetManager(base.manager);
			campDefenseWalkerModel.Initialize();
			if (base.manager.IsStarted)
			{
				campDefenseWalkerModel.Start();
			}
			campDefenseWalkerModel.SetFaction(Faction.Walker);
			Walkers.Add(campDefenseWalkerModel);
			NotifyChange("CampDefenseWalkerAdded", campDefenseWalkerModel);
		}

		private void RemoveWalker(CampDefenseWalkerModel walker)
		{
			Walkers.Remove(walker);
		}
	}
}
