using System;
using BaseModel;

namespace TWDModel
{
	public class BurningOutAction : StatusEffectAction
	{
		public bool OnRedHealthBar { get; set; }

		public int BurnTurns { get; set; }

		public BurningOutAction(ActorModel actor, ActorModel target, SupportModel sourceSupport = null, Func<int> damage = null, int burnTurns = 0)
			: base(actor, target, sourceSupport, damage)
		{
			OnRedHealthBar = true;
			BurnTurns = burnTurns;
		}

		public BurningOutAction(ActorModel actor, ActorModel target, bool onRedHealthBar, SupportModel sourceSupport = null, Func<int> damage = null, int burnTurns = 0)
			: base(actor, target, sourceSupport, damage)
		{
			OnRedHealthBar = onRedHealthBar;
			BurnTurns = burnTurns;
		}

		public override bool Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			bool result = false;
			if (combatModel != null && base.TargetActor != null && base.TargetActor.IsValid())
			{
				ActorModel sourceActor = base.SourceActor;
				if (sourceActor == null || !sourceActor.HasAnyLevelTrait("DebuffMarkEnemy"))
				{
					if (base.Avoided)
					{
						return true;
					}
					if (CombatHelpers.CheckPreventIncendiary(base.TargetActor))
					{
						base.TargetActor.NotifyChange("AbilityVisited", new object[2] { "PreventIncendiary", false });
						return true;
					}
					bool isBurning = base.TargetActor.IsBurning;
					result = combatModel.BurningOutActor(base.TargetActor, OnRedHealthBar, BurnTurns);
					if (result)
					{
						CheckMaggieLeaderTrait(tWDModelManager, combatModel, isBurning);
						if (OnRedHealthBar)
						{
							base.TargetActor.StrugglesLeft--;
						}
						tWDModelManager.ExecuteAction(new PostStatusEffectAction(base.SourceActor, base.TargetActor, TimedEffectType.Burning, base.SourceSupport));
					}
					return result;
				}
			}
			return result;
		}

		private void CheckMaggieLeaderTrait(TWDModelManager twdModelManager, CombatModel combatModel, bool isBurning)
		{
			if ((base.SourceSupport == null || !base.SourceSupport.SupportId.Equals("Hwacha")) && isBurning)
			{
				FixedPoint value = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseScorchChance", ref value, base.SourceActor);
				FixedPoint value2 = 0.0;
				if (combatModel.AbilityManager.VisitParameter("HeirloomsMaggiePocketWatchScorchChance", ref value2, base.SourceActor))
				{
					value = value2;
				}
				FixedPoint value3 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseScorchTurns", ref value3, base.SourceActor);
				combatModel.AbilityManager.VisitParameter("HeirloomsMaggiePocketWatchScorchTurns", ref value3, base.SourceActor);
				if (base.TargetActor.AttributeModel?.GetAttributeModelValue("burn_be_ratio") != 0L)
				{
					FixedPoint value4 = value;
					FixedPoint value5 = 1L;
					FixedPoint? obj = base.TargetActor.AttributeModel?.GetAttributeModelValue("burn_be_ratio");
					FixedPoint? fixedPoint = value5 + obj;
					value = (value4 * fixedPoint).Value;
				}
				if (twdModelManager.Player.RollDice(RollDiceType.Scorch, value) != PlayerRandomChanceResult.Failed)
				{
					base.TargetActor.Scorch((int)value3, base.SourceActor);
				}
			}
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null");
		}
	}
}
