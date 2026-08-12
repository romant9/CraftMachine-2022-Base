using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public sealed class CommandSkillRemoveNegativeEffectAction : ModelActorAction
	{
		public ActorModel SourceActor { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public CommandSkillDefinition CommandSkillDefinition { get; private set; }

		public int RemoveCount { get; private set; }

		public CommandSkillRemoveNegativeEffectAction(ActorModel sourceActor, ActorModel targetActor, CommandSkillDefinition commandSkillDefinition, int removeCount)
			: base(sourceActor)
		{
			SourceActor = sourceActor;
			TargetActor = targetActor;
			CommandSkillDefinition = commandSkillDefinition;
			RemoveCount = removeCount;
		}

		public override bool Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { CombatModel: var combatModel }))
			{
				return false;
			}
			if (combatModel == null)
			{
				return false;
			}
			if (CommandSkillDefinition == null)
			{
				return false;
			}
			List<EffectIndexPriorityItem> effectIndexPriorityItems = CommandSkillDefinition.GetEffectIndexPriorityItems();
			if (effectIndexPriorityItems.Count == 0)
			{
				return false;
			}
			return RemoveActorNegativeEffects(combatModel, effectIndexPriorityItems);
		}

		private bool RemoveActorNegativeEffects(CombatModel combat, List<EffectIndexPriorityItem> effectIndexPriorityItems)
		{
			int num = 0;
			foreach (EffectIndexPriorityItem effectIndexPriorityItem in effectIndexPriorityItems)
			{
				foreach (string negativeEffect in effectIndexPriorityItem.NegativeEffects)
				{
					if (num >= RemoveCount)
					{
						return true;
					}
					if (TargetActor.TryRemoveNegativeEffectByName(combat, negativeEffect))
					{
						num++;
					}
				}
			}
			return true;
		}

		public override bool CanExecute()
		{
			return TargetActor != null;
		}
	}
}
