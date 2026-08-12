using System;

namespace TWDModel
{
	[Serializable]
	public class MissionLogicCondition
	{
		public MissionConditionType ConditionType;

		public int ActorTagId;

		public Faction ActorFaction = Faction.Walker;

		public string VariableName;

		public MissionConditionOperator ConditionOperator;

		public int ComparisonValue;

		private int VariableHash;

		public int GetVariableHash()
		{
			if (VariableHash == 0)
			{
				VariableHash = VariableName.GetHashCode();
			}
			return VariableHash;
		}

		public bool CheckCondition(int currentValue)
		{
			return ConditionOperator switch
			{
				MissionConditionOperator.Equal => currentValue == ComparisonValue, 
				MissionConditionOperator.GreaterThan => currentValue > ComparisonValue, 
				MissionConditionOperator.GreaterThanOrEqual => currentValue >= ComparisonValue, 
				MissionConditionOperator.LessThan => currentValue < ComparisonValue, 
				MissionConditionOperator.LessThanOrEqual => currentValue <= ComparisonValue, 
				MissionConditionOperator.NotEqual => currentValue != ComparisonValue, 
				_ => false, 
			};
		}

		public override string ToString()
		{
			string text = "";
			text = "Condition type = " + ConditionType;
			if (ConditionType == MissionConditionType.AliveActorCount)
			{
				text = text + " { Faction = " + ActorFaction.ToString() + ", ActorTagId = " + ActorTagId + " }";
			}
			else if (ConditionType == MissionConditionType.VariableValue)
			{
				text = text + " { VariableName = " + VariableName + " }";
			}
			return text + ", Operator = " + ConditionOperator.ToString() + ", Comparison Value = " + ComparisonValue;
		}
	}
}
