using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ExecuteCombatSupportAction : ModelActorAction
	{
		public readonly GridCoordinate Target;

		public readonly int EquipIndex;

		private ICollection<ActorModel> affectedTargets;

		public IEnumerable<ActorModel> Targets
		{
			get
			{
				if (affectedTargets != null)
				{
					return affectedTargets;
				}
				if (base.Actor.manager.CombatModel.SupportManager.TryGetSupport(EquipIndex, out var combatSupportModel))
				{
					return combatSupportModel.GetTargets(Target);
				}
				return null;
			}
		}

		public ExecuteCombatSupportAction(ActorModel actorModel, int supportEquipIndex, GridCoordinate targetCoord)
			: base(actorModel)
		{
			Target = targetCoord;
			EquipIndex = supportEquipIndex;
		}

		public override bool Execute(ModelManager manager)
		{
			if (base.Actor.manager.CombatModel.SupportManager.Execute(EquipIndex, Target, out affectedTargets))
			{
				return true;
			}
			return false;
		}
	}
}
