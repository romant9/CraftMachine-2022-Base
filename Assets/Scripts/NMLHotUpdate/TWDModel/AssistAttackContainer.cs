using System.Collections.Generic;

namespace TWDModel
{
	public sealed class AssistAttackContainer
	{
		private Dictionary<ActorModel, List<int>> AssistAttackRecords;

		public AssistAttackContainer()
		{
			AssistAttackRecords = new Dictionary<ActorModel, List<int>>();
		}

		public void AddRecord(ActorModel source, int turn)
		{
			if (!AssistAttackRecords.ContainsKey(source))
			{
				AssistAttackRecords[source] = new List<int>();
			}
			AssistAttackRecords[source].Add(turn);
		}

		public bool CanAssistAttackTargetThisTurn(ActorModel source, int turn)
		{
			if (!AssistAttackRecords.ContainsKey(source))
			{
				return true;
			}
			if (AssistAttackRecords[source].Count == 0)
			{
				return true;
			}
			if (AssistAttackRecords[source].Contains(turn))
			{
				return false;
			}
			return true;
		}
	}
}
