using System;

namespace TWDModel
{
	public class BehaviorLogEntry
	{
		public int Id;

		public int TurnId;

		public AIBehaviorEnum Behavior;

		public GridCoordinate BeginCoordinate = GridCoordinate.Invalid;

		public GridCoordinate EndCoordinate = GridCoordinate.Invalid;

		public GridCoordinate MoveTargetCoordinate = GridCoordinate.Invalid;

		public ActorModel PreExecuteCurrentTarget;

		public ActorModel AfterExecuteCurrentTarget;

		public AIAlertness AlertnessState;

		public AIMode AIMode;

		public GridField<FixedPoint> MovementField;

		public GridField<FixedPoint> AttackField;

		public GridField<FixedPoint> DefenceField;

		public GridField<FixedPoint> ExploreField;

		public GridField<FixedPoint> DistanceField;

		public GridField<FixedPoint> DistanceToTarget;

		public GridField<FixedPoint> CoverField;

		public GridField<bool> CoverLocations;

		public FixedPoint AttackMultiplier;

		public FixedPoint CurrentTargetMultiplier;

		public FixedPoint DefenceMultiplier;

		public FixedPoint ExploreMultiplier;

		public FixedPoint DistanceMultiplier;

		public FixedPoint DistanceToTargetMultiplier;

		public FixedPoint CoverBaseValue;

		public FixedPoint CoverEnemyMultiplier;

		public string GetMovementFieldCalculation(int index)
		{
			if (MovementField != null && AttackField != null && DefenceField != null && ExploreField != null && DistanceField != null && DistanceToTarget != null && CoverField != null)
			{
				string result = "N/A";
				if (MovementField[index] > FixedPoint.MinValue)
				{
					float num = (float)AttackField[index];
					float num2 = (float)DefenceField[index];
					float num3 = (float)ExploreField[index];
					float num4 = (float)DistanceField[index];
					float num5 = (float)DistanceToTarget[index];
					float num6 = (float)CoverField[index];
					float num7 = (float)(num * AttackMultiplier * CurrentTargetMultiplier);
					float num8 = (float)(num2 * DefenceMultiplier);
					float num9 = (float)(num3 * ExploreMultiplier);
					float num10 = (float)(num4 * DistanceMultiplier);
					float num11 = (float)(num5 * DistanceToTargetMultiplier);
					float num12 = (CoverLocations[index] ? ((float)(CoverBaseValue * num6)) : 0f);
					result = "AttackValue: " + num.ToString("0.##") + " * " + ((float)AttackMultiplier).ToString("0.##") + " * " + ((float)CurrentTargetMultiplier).ToString("0.##") + " = " + num7.ToString("0.##");
					result = result + "\nDefenceValue: " + num2.ToString("0.##") + " * " + ((float)DefenceMultiplier).ToString("0.##") + " = " + num8.ToString("0.##");
					result = result + "\nExploreField: " + num3.ToString("0.##") + " * " + ((float)ExploreMultiplier).ToString("0.##") + " = " + num9.ToString("0.##");
					result = result + "\nDistanceValue: " + num4.ToString("0.##") + " * " + ((float)DistanceMultiplier).ToString("0.##") + " = " + num10.ToString("0.##");
					result = result + "\nDistanceToTarget: " + num5.ToString("0.##") + " * " + ((float)DistanceToTargetMultiplier).ToString("0.##") + " = " + num11.ToString("0.##");
					result = result + "\nCoverValue: " + (CoverLocations[index] ? (((float)CoverBaseValue).ToString("0.##") + " * " + num6.ToString("0.##") + " = " + num12.ToString("0.##")) : "0");
					result = result + "\n\n" + num7.ToString("0.##") + " + " + num8.ToString("0.##") + " + " + num9.ToString("0.##") + " + " + num10.ToString("0.##") + " + " + num11.ToString("0.##") + " + " + num12.ToString("0.##") + " = " + ((float)MovementField[index]).ToString("0.##");
				}
				return result;
			}
			return null;
		}

		public override string ToString()
		{
			GridCoordinate beginCoordinate = BeginCoordinate;
			string text = "Begin Coordinate: " + beginCoordinate.ToString();
			beginCoordinate = EndCoordinate;
			string text2 = text + "\nEnd Coordinate: " + beginCoordinate.ToString();
			beginCoordinate = MoveTargetCoordinate;
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(text2 + "\nMove Target Coordinate: " + beginCoordinate.ToString(), "\nPreExecute Current Target: '", (PreExecuteCurrentTarget != null) ? (PreExecuteCurrentTarget.Name + "@" + PreExecuteCurrentTarget.GridCoordinate.ToString()) : "NULL", "'"), "\nAfterExecute Current Target: '", (AfterExecuteCurrentTarget != null) ? (AfterExecuteCurrentTarget.Name + "@" + AfterExecuteCurrentTarget.GridCoordinate.ToString()) : "NULL", "'"), "\nAlertness State: '", Enum.GetName(typeof(AIAlertness), AlertnessState), "'"), "\nAI Mode: '", Enum.GetName(typeof(AIMode), AIMode), "'"), "\nExecuted '", Enum.GetName(typeof(AIBehaviorEnum), Behavior), "' behavior");
		}
	}
}
