using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class SetActiveActorCommand : ModelCommand
	{
		public SetActiveActorCommand()
		{
		}

		public SetActiveActorCommand(ActorModel actor)
			: base(actor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			if (combatModel.TurnManager.CanSwitchActiveActor)
			{
				if (model.Faction == Faction.Survivor)
				{
					List<ActorModel> factionActors = combatModel.GetFactionActors(Faction.Survivor);
					for (int i = 0; i < (factionActors?.Count ?? 0); i++)
					{
						factionActors[i].EquipWeaponEquipment();
						factionActors[i].UnequipConsumableEquipment();
					}
				}
				combatModel.TurnManager.ActiveActor = model;
				model?.UnityOutputCurrentTraits("Set active actor Completed");
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
