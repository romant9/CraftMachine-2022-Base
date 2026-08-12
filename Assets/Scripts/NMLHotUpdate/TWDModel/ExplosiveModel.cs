using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class ExplosiveModel : TWDSpatialModelObject, InteractionReceiver, TriggerReceiver
	{
		public const string ChangeExploded = "Exploded";

		public int Range;

		public bool AffectSurvivors;

		public int DamagePercentageFromHealth;

		public bool HasExploded { get; set; }

		public ExplosiveModel()
		{
		}

		public ExplosiveModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public override void Initialize()
		{
			base.Initialize();
			HasExploded = false;
		}

		public void OnInteractionStep(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnInteractionCanceled(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnAttacked(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnDestroyed(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnInteractionCompleted(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
			Explode(instigator);
		}

		public void OnTriggered(ActorModel instigator)
		{
			Explode(instigator);
		}

		public void Explode(ActorModel instigator)
		{
			if (HasExploded)
			{
				return;
			}
			HasExploded = true;
			List<ActorModel> actorsInRange = base.manager.CombatModel.GetActorsInRange(base.Location.Coordinate, Range);
			CombatModel combat = base.manager.Player.Combat;
			for (int i = 0; i < actorsInRange.Count; i++)
			{
				ActorModel actorModel = actorsInRange[i];
				if (actorModel.IsWalker || actorModel.IsEnvironmental || AffectSurvivors)
				{
					actorModel.DealExplosionDamage(DamagePercentageFromHealth * actorModel.Hitpoints / 100, this, instigator);
				}
			}
			if (combat.IsEndlessBattleMission && actorsInRange.Any((ActorModel x) => x.IsDead))
			{
				combat.EndlessModeCombatModel.HandleKillScoreIncrease();
				combat.NotifyChange("EndlessModeScoreChanged");
			}
			NotifyChange("Exploded", instigator);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
