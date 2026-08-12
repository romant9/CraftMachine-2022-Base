using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class HealActorStatusCommand : ModelCommand
	{
		public HealActorStatusCommand()
		{
		}

		public HealActorStatusCommand(ActorModel actor)
			: base(actor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			combatModel.HealActorStatus(model);
			if (model.SelectedEquipment.IsConsumable)
			{
				model.EquipWeaponEquipment();
				model.UnequipConsumableEquipment();
			}
			if (combatModel.TurnManager.CanSwitchActiveActor)
			{
				List<ActorModel> factionActors = combatModel.GetFactionActors(model.Faction);
				for (int i = 0; i < factionActors.Count; i++)
				{
					ActorModel actorModel = factionActors[i];
					if (model != actorModel && !actorModel.TurnComplete)
					{
						combatModel.TurnManager.ActiveActor = actorModel;
						break;
					}
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
