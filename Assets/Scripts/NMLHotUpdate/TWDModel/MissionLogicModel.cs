using System.Collections.Generic;

namespace TWDModel
{
	public class MissionLogicModel : TWDModelObject
	{
		public List<MissionLogicCondition> MissionLogicConditions { get; set; }

		public List<TriggerReceiver> Receivers { get; set; }

		public bool HasFired { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			HasFired = false;
		}

		private void NotifyReceivers()
		{
			if (!HasFired)
			{
				for (int i = 0; i < Receivers.Count; i++)
				{
					Receivers[i].OnTriggered(null);
				}
				HasFired = true;
			}
		}

		private int GetValue(MissionLogicCondition condition)
		{
			switch (condition.ConditionType)
			{
			case MissionConditionType.AliveActorCount:
			{
				List<ActorModel> factionActors = base.manager.Player.Combat.GetFactionActors(condition.ActorFaction);
				int num = 0;
				for (int i = 0; i < factionActors.Count; i++)
				{
					if (factionActors[i].ActorTag == condition.ActorTagId || condition.ActorTagId == 0)
					{
						num++;
					}
				}
				return num;
			}
			case MissionConditionType.KilledWalkerCount:
				return base.manager.Player.Combat.MissionStatistics.WalkersKilled;
			case MissionConditionType.VariableValue:
				return base.manager.Player.Combat.GetOrCreateVariable(condition.GetVariableHash());
			case MissionConditionType.Turn:
				return base.manager.Player.Combat.TurnManager.TurnCount;
			case MissionConditionType.Keys:
				return base.manager.Player.Combat.MissionStatistics.CollectedLoot;
			default:
				return 0;
			}
		}

		public void CheckConditions()
		{
			if (!HasFired)
			{
				bool flag = true;
				for (int i = 0; i < MissionLogicConditions.Count; i++)
				{
					MissionLogicCondition missionLogicCondition = MissionLogicConditions[i];
					int value = GetValue(missionLogicCondition);
					flag &= missionLogicCondition.CheckCondition(value);
				}
				if (flag)
				{
					NotifyReceivers();
				}
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
