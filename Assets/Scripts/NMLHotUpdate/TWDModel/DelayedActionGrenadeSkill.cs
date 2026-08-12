using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public sealed class DelayedActionGrenadeSkill : BaseCommandSkill
	{
		private const int NeverExpireTurn = int.MaxValue;

		public int DelayTurns { get; private set; }

		public int ExplosionRadius { get; private set; }

		public FixedPoint PanelDamagePercent { get; private set; }

		public FixedPoint MaxHpDamagePercent { get; private set; }

		public FixedPoint OnFlameTrapExtraPercent { get; private set; }

		public FixedPoint FlameTrapChancePercent { get; private set; }

		public int FlameTrapTurns { get; private set; }

		public FixedPoint FlameTrapInjuryPercent { get; private set; }

		public List<string> SelfTraitsApply { get; private set; }

		public List<string> TargetTraitsApply { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillDelayedActionGrenade;

		public DelayedActionGrenadeSkill()
		{
			SelfTraitsApply = new List<string>();
			TargetTraitsApply = new List<string>();
		}

		public DelayedActionGrenadeSkill(DelayedActionGrenadeSkill skill)
			: base(skill)
		{
			DelayTurns = skill.DelayTurns;
			ExplosionRadius = skill.ExplosionRadius;
			PanelDamagePercent = skill.PanelDamagePercent;
			MaxHpDamagePercent = skill.MaxHpDamagePercent;
			OnFlameTrapExtraPercent = skill.OnFlameTrapExtraPercent;
			FlameTrapChancePercent = skill.FlameTrapChancePercent;
			FlameTrapTurns = skill.FlameTrapTurns;
			FlameTrapInjuryPercent = skill.FlameTrapInjuryPercent;
			SelfTraitsApply = ((skill.SelfTraitsApply != null) ? new List<string>(skill.SelfTraitsApply) : new List<string>());
			TargetTraitsApply = ((skill.TargetTraitsApply != null) ? new List<string>(skill.TargetTraitsApply) : new List<string>());
		}

		public DelayedActionGrenadeSkill(int delayTurns, int explosionRadius, FixedPoint panelDamagePercent, FixedPoint maxHpDamagePercent, FixedPoint onFlameTrapExtraPercent, FixedPoint flameTrapChancePercent, int flameTrapTurns, FixedPoint flameTrapInjuryPercent, List<string> selfTraitsApply, List<string> targetTraitsApply)
		{
			DelayTurns = delayTurns;
			ExplosionRadius = explosionRadius;
			PanelDamagePercent = panelDamagePercent;
			MaxHpDamagePercent = maxHpDamagePercent;
			OnFlameTrapExtraPercent = onFlameTrapExtraPercent;
			FlameTrapChancePercent = flameTrapChancePercent;
			FlameTrapTurns = flameTrapTurns;
			FlameTrapInjuryPercent = flameTrapInjuryPercent;
			SelfTraitsApply = ((selfTraitsApply != null) ? new List<string>(selfTraitsApply) : new List<string>());
			TargetTraitsApply = ((targetTraitsApply != null) ? new List<string>(targetTraitsApply) : new List<string>());
		}

		public override bool CanExecute(GridCoordinate targetCell)
		{
			if (base.LeftCooldownTurns > 0)
			{
				return false;
			}
			if (!CanExecuteWhereAPEnough())
			{
				return false;
			}
			if (base.OwnActorModel == null)
			{
				return false;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return false;
			}
			if (!combatModel.Grid.IsCoordinateValid(targetCell) || combatModel.IsBlocked(targetCell))
			{
				return false;
			}
			CommandSkillDefinition definition = base.Definition;
			if (definition == null)
			{
				return false;
			}
			if (definition.Range >= 0 && base.OwnActorModel.GridCoordinate.ChebyshevDistance(targetCell) > definition.Range)
			{
				return false;
			}
			return true;
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			TWDModelManager tWDModelManager = base.manager;
			CombatModel combatModel = tWDModelManager?.CombatModel;
			if (combatModel == null || base.OwnActorModel == null)
			{
				return;
			}
			DelayedActionGrenadeAreaManager delayedActionGrenadeAreaManager = combatModel.GetModel<DelayedActionGrenadeAreaManager>();
			if (delayedActionGrenadeAreaManager == null)
			{
				delayedActionGrenadeAreaManager = new DelayedActionGrenadeAreaManager();
				delayedActionGrenadeAreaManager.SetManager(tWDModelManager);
				combatModel.AddModel(delayedActionGrenadeAreaManager);
			}
			List<DelayedActionGrenadeArea> list = (from b in combatModel.Models.OfType<DelayedActionGrenadeArea>()
				where b.EffectiveAreaGridCoordinate == targetCell
				select b).ToList();
			int detonateTurn = combatModel.TurnManager.TurnCount + DelayTurns;
			DelayedActionGrenadeArea delayedActionGrenadeArea = new DelayedActionGrenadeArea(base.OwnActorModel, targetCell, base.OwnActorModel.Faction, int.MaxValue, detonateTurn, ExplosionRadius, PanelDamagePercent, MaxHpDamagePercent, OnFlameTrapExtraPercent, FlameTrapChancePercent, FlameTrapTurns, FlameTrapInjuryPercent, SelfTraitsApply, TargetTraitsApply);
			delayedActionGrenadeArea.SetManager(tWDModelManager);
			delayedActionGrenadeAreaManager.AddArea(delayedActionGrenadeArea);
			base.OwnActorModel.NotifyChange("DelayedActionGrenadeThrow", targetCell);
			bool flag = false;
			if (list.Count > 0)
			{
				foreach (DelayedActionGrenadeArea item in list)
				{
					tWDModelManager.ExecuteAction(new DetonateGrenadeAction(item));
				}
				flag = true;
			}
			else
			{
				ActorModel occupier = combatModel.GetOccupier(targetCell);
				bool num = occupier != null && base.OwnActorModel.IsEnemy(occupier);
				bool flag2 = combatModel.Models.OfType<TrapFlameArea>().Any((TrapFlameArea x) => x.EffectiveAreaGridCoordinate == targetCell);
				if (num || flag2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				tWDModelManager.ExecuteAction(new DetonateGrenadeAction(delayedActionGrenadeArea));
			}
		}
	}
}
