using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class SurvivalGameModel : TWDModelObject
	{
		protected AbilityManagerModel abilityManager;

		protected CombatModel combatModel;

		[IgnoreModelProperty]
		public ActorModel LeaderActor { get; private set; }

		[IgnoreModelProperty]
		public ActorModel EnemyActor { get; private set; }

		public int LeftCount { get; private set; }

		public int LeftNoDeadCount { get; private set; }

		[IgnoreModelProperty]
		public TraitEntry LeadTrait => LeaderActor.TraitContainer.GetTraitAnyLevel("LeaderBuffSurvivalGame");

		[IgnoreModelProperty]
		public TraitDefinition LeadTraitDefinition
		{
			get
			{
				if (LeadTrait != null && combatModel != null)
				{
					return combatModel.gameEconomyData.GetTraitDefinition(LeadTrait.TraitIdentifier);
				}
				return null;
			}
		}

		[IgnoreModelProperty]
		public int LeadLevel
		{
			get
			{
				if (LeadTrait != null)
				{
					return UpgradeTraitsData.GetTraitLevelIdentifier(LeadTrait.TraitIdentifier);
				}
				return -1;
			}
		}

		public SurvivalGameModel()
		{
		}

		public SurvivalGameModel(SurvivalGameModelBackup modelbackup)
		{
			EnemyActor = modelbackup.EnemyActor;
			LeaderActor = modelbackup.LeaderActor;
			LeftCount = modelbackup.LeftCount;
			LeftNoDeadCount = modelbackup.LeftNoDeadCount;
		}

		public int GetLeftNoDeadCount()
		{
			FixedPoint value = 0.0;
			abilityManager.VisitParameter("LeaderBuffSurvivalGame_NoDeadLevel", ref value, LeaderActor);
			if (LeadLevel + 1 >= (int)value && LeftCount > 0)
			{
				return LeftNoDeadCount;
			}
			return 0;
		}

		public void ReduceLeftNoDeadCount()
		{
			if (LeftNoDeadCount > 0)
			{
				LeftNoDeadCount--;
				LeaderActor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffSurvivalGame", false });
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			abilityManager = base.manager.Player.AbilityManager;
			combatModel = base.manager.CombatModel;
		}

		public override void Start()
		{
			base.Start();
			abilityManager = base.manager.Player.AbilityManager;
			combatModel = base.manager.CombatModel;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void SetNewSelected(ActorModel leader, ActorModel enemy)
		{
			if (combatModel != null)
			{
				LeaderActor = leader;
				EnemyActor = enemy;
				if (LeadTrait != null)
				{
					FixedPoint value = 0.0;
					abilityManager.VisitParameter("LeaderBuffSurvivalGame_MaxTurns", ref value, leader);
					LeftCount = (int)value;
					value = 0.0;
					abilityManager.VisitParameter("LeaderBuffSurvivalGame_CDTurns", ref value, leader);
					leader.SurvivalGameLeftCD = (int)value;
					value = 0.0;
					abilityManager.VisitParameter("LeaderBuffSurvivalGame_NoDeadMaxCount", ref value, leader);
					LeftNoDeadCount = (int)value;
					SetUnLucky();
					enemy.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffSurvivalGame", false });
					enemy.NotifyChange("UpdateSurvivalGameEvent");
					leader.NotifyChange("UpdateSurvivalGameEvent");
					combatModel.NotifyChange("UpdateSurvivalGameEvent");
				}
			}
		}

		private void SetUnLucky()
		{
			if (LeadTraitDefinition == null)
			{
				return;
			}
			FixedPoint value = 0.0;
			abilityManager.VisitParameter("LeaderBuffSurvivalGame_MaxTurns", ref value, LeaderActor);
			string parameter = LeadTraitDefinition.GetParameter<string>(11);
			if (base.manager.GameEconomyData.GetTraitDefinition(parameter) != null)
			{
				EnemyActor.StartUnLucky((int)value, LeaderActor, parameter);
			}
			FixedPoint value2 = 0.0;
			abilityManager.VisitParameter("LeaderBuffSurvivalGame_MaxTurns", ref value2, LeaderActor);
			string parameter2 = LeadTraitDefinition.GetParameter<string>(14);
			if (base.manager.GameEconomyData.GetTraitDefinition(parameter2) == null)
			{
				return;
			}
			List<ActorModel> flagTeamByDistance = CombatHelpers.GetFlagTeamByDistance(EnemyActor, (int)value2);
			if (flagTeamByDistance != null && flagTeamByDistance.Count > 0)
			{
				for (int i = 0; i < flagTeamByDistance.Count; i++)
				{
					flagTeamByDistance[i].StartUnLucky((int)value, LeaderActor, parameter2);
				}
			}
		}

		public int GetEnemyNegativeCount()
		{
			if (LeadTraitDefinition == null)
			{
				return 0;
			}
			return EnemyActor.GetNegativeEffCount(LeadTraitDefinition.EffectIndex);
		}

		public void TurnChange()
		{
			LeftCount--;
		}

		public void End()
		{
			if (combatModel == null)
			{
				return;
			}
			if (!EnemyActor.IsDead)
			{
				FixedPoint value = 0.0;
				base.manager.Player.AbilityManager.VisitParameter("LeaderBuffSurvivalGame_ChanceStun", ref value, LeaderActor);
				if (base.manager.Player.RollDice(RollDiceType.SurvivalGame, value) != PlayerRandomChanceResult.Failed)
				{
					EnemyActor.Stun(1, LeaderActor);
				}
			}
			if (EnemyActor.IsDead && LeftCount > 0)
			{
				combatModel.HealSurvivalGameList(LeaderActor);
			}
			if (!EnemyActor.IsDead)
			{
				EnemyActor.NotifyChange("UpdateSurvivalGameEvent");
			}
			if (!LeaderActor.IsDead)
			{
				LeaderActor.NotifyChange("UpdateSurvivalGameEvent");
			}
			combatModel.NotifyChange("UpdateSurvivalGameEvent");
		}

		public void UpdateData()
		{
			if (LeftCount <= 0 || EnemyActor.IsDead)
			{
				End();
			}
		}
	}
}
