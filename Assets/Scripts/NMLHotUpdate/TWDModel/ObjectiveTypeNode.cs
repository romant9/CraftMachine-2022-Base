using System;

namespace TWDModel
{
	[Serializable]
	public class ObjectiveTypeNode : NodeBase
	{
		[GraphItVariable("")]
		public CheckedObjectiveType ObjectiveType;

		public ObjectiveTypeNode()
		{
		}

		public ObjectiveTypeNode(ObjectiveTypeNode node)
			: base(node)
		{
			ObjectiveType = node.ObjectiveType;
		}

		public override NodeBase RecordValue()
		{
			return new ObjectiveTypeNode(this);
		}

		[GraphItInput("Check", "")]
		public void Check()
		{
			bool flag = false;
			CombatModel combat = base.manager.Player.Combat;
			if (combat.IsWorldBossMission && !combat.IsUsingSurvivalMissionConfig)
			{
				flag = base.manager.Player.GetAttackTargetMissionModel() is WorldBossMissionModel worldBossMissionModel && WorldBossCombatHelper.IsObjectiveType(worldBossMissionModel.WorldBossMissionType, ObjectiveType);
			}
			else
			{
				switch (ObjectiveType)
				{
				case CheckedObjectiveType.Unspecified:
					flag = !combat.IsUsingSurvivalMissionConfig;
					break;
				case CheckedObjectiveType.SurvGoToExit:
					if (combat.IsUsingSurvivalMissionConfig)
					{
						flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat) == SurvivalMissionConfig.SurvivalObjectiveType.GoToExit;
					}
					break;
				case CheckedObjectiveType.SurvKillAllWalkers:
					if (combat.IsUsingSurvivalMissionConfig)
					{
						flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat) == SurvivalMissionConfig.SurvivalObjectiveType.KillAllWalkers;
					}
					break;
				case CheckedObjectiveType.SurvKillAllRaiders:
					if (combat.IsUsingSurvivalMissionConfig)
					{
						flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat) == SurvivalMissionConfig.SurvivalObjectiveType.KillAllRaiders;
					}
					break;
				case CheckedObjectiveType.SurvKillAmountAndExit:
					if (combat.IsUsingSurvivalMissionConfig)
					{
						flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat) == SurvivalMissionConfig.SurvivalObjectiveType.KillAmountAndExit;
					}
					break;
				case CheckedObjectiveType.SurvKillBossAndExit:
					if (combat.IsUsingSurvivalMissionConfig)
					{
						flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat) == SurvivalMissionConfig.SurvivalObjectiveType.KillBossAndExit;
					}
					break;
				case CheckedObjectiveType.SurvFindLoot:
					if (combat.IsUsingSurvivalMissionConfig)
					{
						flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat) == SurvivalMissionConfig.SurvivalObjectiveType.FindLoot;
					}
					break;
				case CheckedObjectiveType.SurvSurviveTurnAmountAndExit:
					if (combat.IsUsingSurvivalMissionConfig)
					{
						flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(combat) == SurvivalMissionConfig.SurvivalObjectiveType.SurviveTurnAmountAndExit;
					}
					break;
				}
			}
			if (flag)
			{
				FireTrue();
			}
			else
			{
				FireFalse();
			}
		}

		[GraphItOutput("True", "")]
		public void FireTrue()
		{
			Fire("True");
		}

		[GraphItOutput("False", "")]
		public void FireFalse()
		{
			Fire("False");
		}
	}
}
