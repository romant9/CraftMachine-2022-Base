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
			switch (ObjectiveType)
			{
			case CheckedObjectiveType.Unspecified:
				flag = !base.manager.Player.Combat.IsUsingSurvivalMissionConfig;
				break;
			case CheckedObjectiveType.SurvGoToExit:
				if (base.manager.Player.Combat.IsUsingSurvivalMissionConfig)
				{
					flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(base.manager.Player.Combat) == SurvivalMissionConfig.SurvivalObjectiveType.GoToExit;
				}
				break;
			case CheckedObjectiveType.SurvKillAllWalkers:
				if (base.manager.Player.Combat.IsUsingSurvivalMissionConfig)
				{
					flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(base.manager.Player.Combat) == SurvivalMissionConfig.SurvivalObjectiveType.KillAllWalkers;
				}
				break;
			case CheckedObjectiveType.SurvKillAllRaiders:
				if (base.manager.Player.Combat.IsUsingSurvivalMissionConfig)
				{
					flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(base.manager.Player.Combat) == SurvivalMissionConfig.SurvivalObjectiveType.KillAllRaiders;
				}
				break;
			case CheckedObjectiveType.SurvKillAmountAndExit:
				if (base.manager.Player.Combat.IsUsingSurvivalMissionConfig)
				{
					flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(base.manager.Player.Combat) == SurvivalMissionConfig.SurvivalObjectiveType.KillAmountAndExit;
				}
				break;
			case CheckedObjectiveType.SurvKillBossAndExit:
				if (base.manager.Player.Combat.IsUsingSurvivalMissionConfig)
				{
					flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(base.manager.Player.Combat) == SurvivalMissionConfig.SurvivalObjectiveType.KillBossAndExit;
				}
				break;
			case CheckedObjectiveType.SurvFindLoot:
				if (base.manager.Player.Combat.IsUsingSurvivalMissionConfig)
				{
					flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(base.manager.Player.Combat) == SurvivalMissionConfig.SurvivalObjectiveType.FindLoot;
				}
				break;
			case CheckedObjectiveType.SurvSurviveTurnAmountAndExit:
				if (base.manager.Player.Combat.IsUsingSurvivalMissionConfig)
				{
					flag = SurvivalCombatHelper.GetSurvivalMissionObjectiveType(base.manager.Player.Combat) == SurvivalMissionConfig.SurvivalObjectiveType.SurviveTurnAmountAndExit;
				}
				break;
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
