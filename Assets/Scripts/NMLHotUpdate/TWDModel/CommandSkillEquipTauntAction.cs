using BaseModel;

namespace TWDModel
{
	public class CommandSkillEquipTauntAction : ModelAction
	{
		public ActorModel Target { get; set; }

		public int Turns { get; private set; }

		public int TauntLatticeNumber { get; private set; }

		public int Shield { get; private set; }

		public bool Avoided { get; set; }

		public CommandSkillEquipTauntAction(ActorModel target, int tauntLatticeNumber, int turns, int shield)
			: base(target)
		{
			Target = target;
			TauntLatticeNumber = tauntLatticeNumber;
			Turns = turns;
			Shield = shield;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager)?.CombatModel;
			if (combatModel != null && Target != null && Target.IsValid() && !Avoided && !Target.IsDead)
			{
				addTaunt(Target, combatModel);
				if (!Target.IsShieldBreaker())
				{
					Target.StartSkillEquipTauntShieldTaunt(Turns, Shield);
				}
				return true;
			}
			(manager as TWDModelManager)?.Debug.LogError("CommandSkillEquipTauntAction action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Target Actor: " + ((Target != null) ? "not null" : "NULL"));
			return false;
		}

		private void addTaunt(ActorModel actor, CombatModel combatModel)
		{
			actor.GridCoordinate.GetEnemiesByDistanceAndFaction(actor.GridCoordinate, combatModel, TauntLatticeNumber, actor.Faction).ForEach(delegate(ActorModel x)
			{
				if (x != null && !x.IsDead)
				{
					combatModel.manager.ExecuteAction(new TauntAction(actor, x, 1));
				}
			});
		}

		public override string ToString()
		{
			return "TargetActor = " + ((Target != null) ? Target.DebugInfo : "null");
		}
	}
}
