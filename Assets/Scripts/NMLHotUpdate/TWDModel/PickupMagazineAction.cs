using BaseModel;

namespace TWDModel
{
	public class PickupMagazineAction : ModelAction
	{
		public ActorModel Actor { get; private set; }

		public MagazineArea Magazine { get; private set; }

		public PickupMagazineAction(ActorModel actor, MagazineArea magazine)
			: base(actor)
		{
			Actor = actor;
			Magazine = magazine;
		}

		public override bool Execute(ModelManager manager)
		{
			if (Actor == null || Actor.IsDead || Magazine == null)
			{
				return false;
			}
			if (!(manager is TWDModelManager { CombatModel: var combatModel }))
			{
				return false;
			}
			if (combatModel == null)
			{
				return false;
			}
			combatModel.RemoveModel(Magazine);
			combatModel.NotifyChange("MagazineAreasUpdate");
			Actor.TacticalResupplyMagazineNextDragLineCritPending = true;
			if (Actor.AbilityCompleted)
			{
				if (!Actor.IsInteractingWithObject)
				{
					Actor.TurnState = TurnState.Idle;
				}
				Actor.SecondMoveCompleted = false;
				Actor.AbilityCompleted = false;
			}
			else if (Actor.MoveCompleted)
			{
				if (Actor.SecondMoveCompleted)
				{
					Actor.SecondMoveCompleted = false;
				}
				else
				{
					Actor.MoveCompleted = false;
				}
			}
			Actor.NotifyChange("RefreshCommandSkill");
			Actor.NotifyChange("actorExtraAbilityAction");
			return true;
		}
	}
}
