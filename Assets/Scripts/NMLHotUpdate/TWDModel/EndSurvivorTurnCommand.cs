using System.Collections;
using BaseModel;

namespace TWDModel
{
	public class EndSurvivorTurnCommand : ModelCommand
	{
		public override ModelRespondCode respondCode { get; protected set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			bool flag = false;
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				if (combatModel.TurnManager.ActiveFaction == Faction.Survivor)
				{
					if (!combatModel.MissionCompleted)
					{
						combatModel.TurnManager.ExecuteNPCTurn();
					}
				}
				else
				{
					flag = false;
				}
				flag = true;
			}
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}

		public override IEnumerator ExecuteForClient(ModelManager manager)
		{
			respondCode = ModelRespondCode.Error;
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel == null)
			{
				yield break;
			}
			if (combatModel.TurnManager.ActiveFaction == Faction.Survivor)
			{
				if (!combatModel.MissionCompleted)
				{
					yield return combatModel.TurnManager.ExecuteNPCTurnForClient();
				}
			}
			else
			{
				respondCode = ModelRespondCode.Error;
			}
			respondCode = ModelRespondCode.OK;
		}
	}
}
