using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public abstract class BaseCommandSkill : TWDModelObject
	{
		public int SkillID { get; private set; }

		public int LeftCooldownTurns { get; protected set; }

		[IgnoreModelProperty]
		public ActorModel OwnActorModel { get; private set; }

		[JsonIgnore]
		public CommandSkillDefinition Definition => base.manager.GameEconomyData.GetCommandSkillDefinition(SkillID);

		public abstract CommandSkillType Type { get; }

		public virtual Faction CooldownLeftTurnCheckFaction => Faction.Survivor;

		public BaseCommandSkill()
		{
		}

		public BaseCommandSkill(BaseCommandSkill baseCommandSkill)
		{
			SkillID = baseCommandSkill.SkillID;
			LeftCooldownTurns = baseCommandSkill.LeftCooldownTurns;
			OwnActorModel = baseCommandSkill.OwnActorModel;
		}

		public void SetSkillGEDParameter(int identifier)
		{
			SkillID = identifier;
		}

		public virtual bool CanExecute(GridCoordinate targetCell)
		{
			if (LeftCooldownTurns > 0)
			{
				return false;
			}
			if (!CanExecuteWhereAPEnough())
			{
				return false;
			}
			if (!CanExecuteTargetType(targetCell))
			{
				return false;
			}
			if (!CanExecuteRange(targetCell))
			{
				return false;
			}
			return true;
		}

		public bool ReleaseSkillToTargetCell(GridCoordinate targetCell)
		{
			if (!CanExecute(targetCell))
			{
				return false;
			}
			OnExecute(targetCell);
			PostExecute(targetCell);
			return true;
		}

		public void OnFactionChangeReduceCooldownLeftTurns()
		{
			if (LeftCooldownTurns > 0)
			{
				LeftCooldownTurns--;
				OwnActorModel.NotifyChange("CooldownLeftTurnUpdate");
			}
		}

		public abstract void OnExecute(GridCoordinate targetCell);

		public virtual void PostExecute(GridCoordinate targetCell)
		{
			if (Definition == null)
			{
				return;
			}
			LeftCooldownTurns = Definition.Cooldown;
			OwnActorModel.NotifyChange("CooldownLeftTurnUpdate");
			if (Definition.APCost == 1)
			{
				if (OwnActorModel.AbilityCompleted)
				{
					if (!OwnActorModel.MoveCompleted)
					{
						OwnActorModel.MoveCompleted = true;
						OwnActorModel.SecondMoveCompleted = true;
						OwnActorModel.NotifyChange("actorMoveCompleted");
						OwnActorModel.NotifyChange("actorSecondMoveCompleted");
					}
					else if (!OwnActorModel.SecondMoveCompleted)
					{
						OwnActorModel.SecondMoveCompleted = true;
						OwnActorModel.NotifyChange("actorSecondMoveCompleted");
					}
				}
				else if (!OwnActorModel.MoveCompleted)
				{
					OwnActorModel.MoveCompleted = true;
					OwnActorModel.NotifyChange("actorMoveCompleted");
				}
				else
				{
					OwnActorModel.AbilityCompleted = true;
					OwnActorModel.NotifyChange("actorAbilityCompleted");
				}
			}
			else if (Definition.APCost > 1)
			{
				OwnActorModel.MoveCompleted = true;
				OwnActorModel.SecondMoveCompleted = true;
				OwnActorModel.AbilityCompleted = true;
				OwnActorModel.NotifyChange("actorMoveCompleted");
				OwnActorModel.NotifyChange("actorSecondMoveCompleted");
				OwnActorModel.NotifyChange("actorAbilityCompleted");
			}
		}

		public bool CanExecuteWhereAPEnough()
		{
			if (Definition.APCost == 0)
			{
				return true;
			}
			if (Definition.APCost == 1)
			{
				if (!OwnActorModel.MoveCompleted || !OwnActorModel.SecondMoveCompleted)
				{
					return true;
				}
			}
			else if (!OwnActorModel.MoveCompleted && !OwnActorModel.SecondMoveCompleted)
			{
				return true;
			}
			return false;
		}

		private bool CanExecuteTargetType(GridCoordinate targetCell)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return false;
			}
			ActorModel occupier = combatModel.GetOccupier(targetCell);
			if (occupier == null)
			{
				if (Definition.TargetType.Contains(CommandSkillTargetType.Grid))
				{
					if (combatModel.Grid.IsCoordinateValid(targetCell))
					{
						return !combatModel.IsBlocked(targetCell);
					}
					return false;
				}
				return false;
			}
			if (OwnActorModel == occupier)
			{
				if (Definition.TargetType.Contains(CommandSkillTargetType.ActorItself))
				{
					return true;
				}
			}
			else if (OwnActorModel.Faction == occupier.Faction)
			{
				if (Definition.TargetType.Contains(CommandSkillTargetType.Friendly))
				{
					return true;
				}
			}
			else if (Definition.TargetType.Contains(CommandSkillTargetType.Enemy))
			{
				return true;
			}
			return false;
		}

		private bool CanExecuteRange(GridCoordinate targetCell)
		{
			if (Definition.Range < 0)
			{
				return true;
			}
			if (OwnActorModel.GridCoordinate.ChebyshevDistance(targetCell) <= Definition.Range)
			{
				return true;
			}
			return false;
		}

		public override void Initialize()
		{
			base.Initialize();
			LeftCooldownTurns = 0;
		}

		public void SetOwnActor(ActorModel ownActorModel)
		{
			OwnActorModel = ownActorModel;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
