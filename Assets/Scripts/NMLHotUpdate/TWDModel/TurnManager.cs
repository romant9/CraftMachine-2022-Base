using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;
using UnityEngine;

namespace TWDModel
{
	public class TurnManager : TWDModelObject
	{
		public const string TurnCountChanged = "TurnCountChanged";

		private ActorModel nextActorOverride;

		[JsonIgnore]
		private ActorModel activeActor;

		private List<ActorModel> activeActors;

		public int TurnCount { get; set; }

		public Faction ActiveFaction { get; set; }

		public ActorModel NextActorOverride
		{
			get
			{
				return nextActorOverride;
			}
			set
			{
				if (value != null && ActiveFaction != value.Faction && value.manager != null)
				{
					value.NewTurn();
					value.IsTurnConsumedOutOfFaction = true;
				}
				nextActorOverride = value;
			}
		}

		[JsonIgnore]
		public bool CanSwitchActiveActor { get; set; }

		[IgnoreModelProperty]
		public ActorModel ActiveActor
		{
			get
			{
				return activeActor;
			}
			set
			{
				if (CanSwitchActiveActor && activeActor != value)
				{
					ActorModel previousActor = activeActor;
					activeActor = value;
					NotifyActorChanged(activeActor);
					if (base.manager != null)
					{
						base.manager.ExecuteAction(new ActiveActorChangedAction(previousActor, value));
					}
				}
			}
		}

		public bool Paused { get; set; }

		[JsonIgnore]
		public bool AllActorsTurnCompleted
		{
			get
			{
				for (int i = 0; i < activeActors.Count; i++)
				{
					if (!activeActors[i].TurnComplete)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool AITurn
		{
			get
			{
				if (ActiveFaction != Faction.Walker && ActiveFaction != Faction.Dormant && ActiveFaction != Faction.Raider)
				{
					return ActiveFaction == Faction.Civilian;
				}
				return true;
			}
		}

		public event ActorChangedHandler ActorChanged;

		public event FactionChangingHandler FactionChanging;

		public event FactionChangingHandler FactionChanged;

		public event FactionChangingHandler FactionPostChanged;

		public IEnumerator ExecuteNPCTurnForClient()
		{
			if (ActiveFaction == Faction.Survivor)
			{
				ChangeFaction();
				yield return null;
			}
			while (ActiveFaction != Faction.Survivor && !Paused)
			{
				CanSwitchActiveActor = true;
				List<ActorModel> activeActorsCopy = new List<ActorModel>(activeActors);
				for (int i = 0; i < activeActorsCopy.Count; i++)
				{
					if (NextActorOverride != null)
					{
						ActiveActor = NextActorOverride;
						NextActorOverride = null;
						i--;
					}
					else
					{
						ActiveActor = activeActorsCopy[i];
					}
					yield return null;
					if (!ActiveActor.TurnComplete && ActiveActor.IsAIControlled)
					{
						ActiveActor.AIController.ExecuteTurn();
						yield return null;
					}
				}
				ChangeFaction();
				yield return null;
			}
		}

		public void ExecuteNPCTurn()
		{
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.Log("ExecuteNPCTurn");
				CommandHelper.Instance.StartCoroutine(ExecuteNPCTurnC());
			}
			else
			{
				if (ActiveFaction == Faction.Survivor)
				{
					ChangeFaction();
				}
				while (ActiveFaction != Faction.Survivor && !Paused)
				{
					CanSwitchActiveActor = true;
					List<ActorModel> list = new List<ActorModel>(activeActors);
					for (int i = 0; i < list.Count; i++)
					{
						if (NextActorOverride != null)
						{
							ActiveActor = NextActorOverride;
							NextActorOverride = null;
							i--;
						}
						else
						{
							ActiveActor = list[i];
						}
						if (!ActiveActor.TurnComplete && ActiveActor.IsAIControlled)
						{
							ActiveActor.AIController.ExecuteTurn();
						}
					}
					ChangeFaction();
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			ActiveFaction = Faction.Survivor;
			TurnCount = 0;
			Paused = false;
			CanSwitchActiveActor = true;
		}

		public override void Start()
		{
			base.Start();
			ResetActiveActor();
		}

		public void ResetActiveActor()
		{
			CanSwitchActiveActor = true;
			if (base.manager == null || base.manager.CombatModel == null)
			{
				return;
			}
			ActorModel actorModel = null;
			activeActors = base.manager.CombatModel.GetFactionActors(ActiveFaction);
			for (int i = 0; i < activeActors.Count; i++)
			{
				ActorModel actorModel2 = activeActors[i];
				if (!actorModel2.TurnComplete)
				{
					actorModel = actorModel2;
					break;
				}
			}
			ActiveActor = actorModel;
			NotifyActorChanged(ActiveActor);
		}

		public override bool IsValid()
		{
			return true;
		}

		private void ChangeFaction()
		{
			if (Paused)
			{
				return;
			}
			Faction faction = Faction.Any;
			Faction activeFaction = ActiveFaction;
			switch (ActiveFaction)
			{
			case Faction.Survivor:
				faction = Faction.Walker;
				break;
			case Faction.Walker:
				faction = Faction.Dormant;
				break;
			case Faction.Dormant:
				faction = Faction.Raider;
				break;
			case Faction.Raider:
				faction = Faction.Civilian;
				break;
			case Faction.Civilian:
				faction = Faction.Tutorial;
				break;
			case Faction.Tutorial:
				faction = Faction.Survivor;
				break;
			}
			foreach (ActorModel allActor in base.manager.CombatModel.GetAllActors())
			{
				allActor.AttackChainStaus = null;
				if (allActor.AsTargetAttackChainSlots != null && allActor.AsTargetAttackChainSlots.Count > 0)
				{
					allActor.AsTargetAttackChainSlots.Clear();
					if (!allActor.IsDead)
					{
						allActor.NotifyChange("ActorAttackChainUpdate");
					}
				}
				allActor.AttackChainGainExtraActionPoint = false;
			}
			base.manager.CombatModel.AttackChainContainer?.AttackChainSourceInfoRecords.Clear();
			base.manager.ExecuteAction(new PreChangeTurnAction(activeFaction, faction));
			NotifyFactionChanging(activeFaction, faction);
			ActiveFaction = faction;
			ActorModel[] array = new ActorModel[base.manager.CombatModel.GetAllActors().Count];
			base.manager.CombatModel.GetAllActors().CopyTo(array);
			foreach (ActorModel actorModel in array)
			{
				if (actorModel != null)
				{
					actorModel.CheckTimedEffectsEndByTraits(ActiveFaction);
					actorModel.IncrementTimedEffects(ActiveFaction);
					actorModel.UpdateEffectDuration(activeFaction);
					actorModel.UpdateTurnFaction(activeFaction, ActiveFaction);
				}
			}
			base.manager.CombatModel.ClearAttackedTargets(ActiveFaction);
			activeActors = base.manager.CombatModel.GetFactionActors(ActiveFaction);
			base.manager.ExecuteAction(new ChangeTurnAction());
			if (ActiveFaction == Faction.Survivor)
			{
				TurnCount++;
				IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider != null)
				{
					List<DifficultyIncrementalDebuff> challengeDebuffs = challengeDebuffProvider.GetChallengeDebuffs();
					List<List<FixedPoint>> debufAllParam = ChallengeDebufHelps.GetDebufAllParam(challengeDebuffs, ChallengeDebuffType.DebuffStunRemove);
					ModelRandom playerRandom = base.manager.Player.PlayerRandom;
					foreach (List<FixedPoint> item in debufAllParam)
					{
						if (TurnCount % (int)item[0] == 0)
						{
							List<ActorModel> stunnedWalkers = base.manager.CombatModel.GetStunnedWalkers();
							for (int j = 0; j < item[1] && j < stunnedWalkers.Count; j++)
							{
								playerRandom.GetRandomElement(stunnedWalkers, remove: true).IncrementStunnedTimedEffects();
							}
						}
					}
					foreach (List<FixedPoint> item2 in ChallengeDebufHelps.GetDebufAllParam(challengeDebuffs, ChallengeDebuffType.DebuffStunRemoveRaider))
					{
						if (TurnCount % (int)item2[0] == 0)
						{
							List<ActorModel> stunnedRaiders = base.manager.CombatModel.GetStunnedRaiders();
							for (int k = 0; k < item2[1] && k < stunnedRaiders.Count; k++)
							{
								playerRandom.GetRandomElement(stunnedRaiders, remove: true).IncrementStunnedTimedEffects();
							}
						}
					}
					foreach (List<FixedPoint> item3 in ChallengeDebufHelps.GetDebufAllParam(challengeDebuffs, ChallengeDebuffType.DebuffReduceRecovery))
					{
						int num = (int)item3[0];
						int param = (int)item3[1];
						if (TurnCount != num)
						{
							continue;
						}
						foreach (ActorModel survivor in base.manager.CombatModel.Survivors)
						{
							base.manager.ExecuteAction(new DebuffReduceRecoveryAction(survivor, num, param));
						}
					}
					foreach (List<FixedPoint> item4 in ChallengeDebufHelps.GetDebufAllParam(challengeDebuffs, ChallengeDebuffType.DebuffDamagePerRound))
					{
						int num2 = (int)item4[0];
						int param2 = (int)item4[1];
						int param3 = (int)item4[2];
						FixedPoint param4 = item4[3] / 100.0;
						if (TurnCount != num2)
						{
							continue;
						}
						base.manager.CombatModel.NotifyChange("DebuffDamagePerRound");
						foreach (ActorModel survivor2 in base.manager.CombatModel.Survivors)
						{
							base.manager.ExecuteAction(new DebuffDamagePerRoundAction(survivor2, num2, param2, param3, param4));
						}
					}
				}
				if (base.manager.CombatModel.IsEndlessBattleMission)
				{
					base.manager.CombatModel.EndlessModeCombatModel.CurrentTurnCount++;
				}
				NotifyChange("TurnCountChanged");
				for (int l = 0; l < array.Length; l++)
				{
					array[l]?.OnTurnCountChanged();
				}
			}
			List<ActorModel> list = new List<ActorModel>(base.manager.CombatModel.GetFactionActors(ActiveFaction));
			for (int m = 0; m < list.Count; m++)
			{
				list[m].NewTurn();
			}
			base.manager.CombatModel.RefreshCitadelTraits();
			if (ActiveFaction == Faction.Survivor)
			{
				if (base.manager.CombatModel.IsEndlessBattleMission)
				{
					EndlessModeCombatModel endlessModeCombatModel = base.manager.CombatModel.EndlessModeCombatModel;
					if (endlessModeCombatModel.KilledWalkersInSurvivorTurn.Count > 0)
					{
						endlessModeCombatModel.HandleKillScoreIncrease();
						base.manager.CombatModel.NotifyChange("EndlessModeScoreChanged");
					}
					if (endlessModeCombatModel.CanReduceMultiplier)
					{
						endlessModeCombatModel.HandleReducingKillScoreMultiplier();
						base.manager.CombatModel.NotifyChange("EndlessModeMultiplierReduced");
					}
					endlessModeCombatModel.KilledEnemyInTurn = false;
				}
				base.manager.CombatModel.CheckForSpawnpointTrigger();
				base.manager.CombatModel.NewTurn();
			}
			else if (activeFaction == Faction.Survivor)
			{
				base.manager.CombatModel.SurvivorTurnEnd();
			}
			if (ActiveFaction != Faction.Survivor)
			{
				SortActiveActors();
			}
			NotifyFactionChanged(activeFaction, ActiveFaction);
			base.manager.ExecuteAction(new PostChangeTurnAction());
			NotifyFactionPostChanged(activeFaction, ActiveFaction);
			for (int n = 0; n < activeActors.Count; n++)
			{
				ActorModel actorModel2 = activeActors[n];
				if ((ActiveFaction != Faction.Survivor || actorModel2.UserCanControl) && !actorModel2.AIController.IsActorIncapacitated && actorModel2.ExclusiveTimedEffect == null)
				{
					ActiveActor = actorModel2;
					break;
				}
			}
			if (ActiveActor == null)
			{
				ActiveActor = ((activeActors.Count > 0) ? activeActors[0] : null);
			}
		}

		public void SelectActor(ActorModel actorToSelect)
		{
			if (activeActors == null)
			{
				return;
			}
			foreach (ActorModel activeActor in activeActors)
			{
				if (!activeActor.TurnComplete && activeActor == actorToSelect)
				{
					ActiveActor = actorToSelect;
					break;
				}
			}
		}

		private ActorModel GetNextFreeActor()
		{
			ActorModel result = null;
			if (activeActors != null)
			{
				foreach (ActorModel activeActor in activeActors)
				{
					if (!activeActor.TurnComplete)
					{
						result = activeActor;
						break;
					}
				}
			}
			return result;
		}

		private void NextActor()
		{
			CanSwitchActiveActor = true;
			ActorModel nextFreeActor = GetNextFreeActor();
			if (nextFreeActor == null)
			{
				ChangeFaction();
				nextFreeActor = GetNextFreeActor();
			}
			ActiveActor = nextFreeActor;
		}

		private void NotifyActorChanged(ActorModel actor)
		{
			this.ActorChanged?.Invoke(actor);
		}

		private void NotifyFactionChanging(Faction currentFaction, Faction newFaction)
		{
			this.FactionChanging?.Invoke(currentFaction, newFaction);
		}

		private void NotifyFactionChanged(Faction currentFaction, Faction newFaction)
		{
			this.FactionChanged?.Invoke(currentFaction, newFaction);
		}

		private void NotifyFactionPostChanged(Faction currentFaction, Faction newFaction)
		{
			this.FactionPostChanged?.Invoke(currentFaction, newFaction);
		}

		private void SortActiveActors()
		{
			List<ActorModel> enemyFactionsActors = base.manager.CombatModel.GetEnemyFactionsActors(ActiveFaction);
			List<GridCoordinate> list = new List<GridCoordinate>();
			foreach (ActorModel item in enemyFactionsActors)
			{
				if (!item.IsStruggling && !item.IsBleedingOut)
				{
					list.Add(item.GridCoordinate);
				}
			}
			GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(base.manager.CombatModel, list, new DistanceFieldOptions(1f, null, null, 1000f));
			foreach (ActorModel activeActor in activeActors)
			{
				if (activeActor.AIDataModel != null)
				{
					FixedPoint fixedPoint = gridField[activeActor.GridCoordinate];
					FixedPoint fixedPoint2 = 0L;
					fixedPoint2 = ((!activeActor.AIController.IsActorIncapacitated) ? (fixedPoint + (long)(Enum.GetValues(typeof(AIAlertness)).Length - activeActor.AIDataModel.Alertness)) : FixedPoint.MaxValue);
					activeActor.AIDataModel.Initiative = fixedPoint2;
				}
			}
			activeActors.StableSort((ActorModel a, ActorModel b) => (a.AIDataModel == null) ? (-1) : ((float)a.AIDataModel.Initiative).CompareTo((b != null && b.AIDataModel != null) ? ((float)b.AIDataModel.Initiative) : (-1f)));
		}

		#region mycode
		private IEnumerator ExecuteNPCTurnC()
		{
			if (ActiveFaction == Faction.Survivor && !StartGWBattle.Instance.IsAIForSurvivors)
			{
				ChangeFaction();
			}
			while ((StartGWBattle.Instance.IsAIForSurvivors || StartGWBattle.Instance.IsSurvivorsPassTurns ? !base.manager.CombatModel.MissionCompleted : ActiveFaction != Faction.Survivor) && !Paused)
			{
				CanSwitchActiveActor = true;
				List<ActorModel> list = new List<ActorModel>(activeActors);
				for (int i = 0; i < list.Count; i++)
				{
					if (NextActorOverride != null)
					{
						ActiveActor = NextActorOverride;
						NextActorOverride = null;
						i--;
					}
					else
					{
						ActiveActor = list[i];
					}
					if (!ActiveActor.TurnComplete && ActiveActor.IsAIControlled)
					{
						if (StartGWBattle.Instance.IsAIForSurvivors && (ActiveActor.Faction == Faction.Survivor || ActiveActor.Faction == Faction.Raider))
						{
							if (StartGWBattle.Instance.IsWaitKeyEveryTurn)
								yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
							if (StartGWBattle.Instance.IsWaitTimeForNextTurn > 0)
								yield return new WaitForSeconds(StartGWBattle.Instance.IsWaitTimeForNextTurn);
						}
						ActiveActor.AIController.ExecuteTurn();
					}
					yield return null;
				}
				ChangeFaction();
				yield return null;
			}
			DebugTWD.Log("Finish NPCTurn");
		}
		#endregion
	}
}
