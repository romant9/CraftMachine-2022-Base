using System.Collections.Generic;
using System.Text;

namespace TWDModel
{
	public class QuestDefinitionOperator
	{
		public enum Op
		{
			Invalid = 0,
			ContextField = 1,
			Value = 2,
			Neg = 3,
			Group = 4,
			Bracket = 5,
			FunctionCall = 6,
			IN = 7,
			GreaterEqual = 8,
			LessEqual = 9,
			Greater = 10,
			Less = 11,
			Equal = 12,
			And = 13,
			Or = 14
		}

		public QuestDefinitionOperator Left;

		public QuestDefinitionOperator Right;

		public string RuleId;

		public Op Operation;

		public long Value;

		public List<long> GroupValues;

		public List<QuestDefinitionOperator> Arguments;

		public int Precedence => (int)Operation;

		public QuestDefinitionOperator(string ruleId)
		{
			RuleId = ruleId;
		}

		private static bool MapIndexToContextValue(QuestCompleteContext context, long fieldIndex, out long valueOut)
		{
			valueOut = 0L;
			if ((ulong)fieldIndex <= 14uL)
			{
				switch ((int)fieldIndex)
				{
				case 0:
					valueOut = context.MapStringValueToInteger(context.Variables.Operation);
					return true;
				case 1:
					valueOut = context.MapStringValueToInteger(context.Variables.MissionKind);
					return true;
				case 2:
					valueOut = context.MapStringValueToInteger(context.Variables.AbilityType);
					return true;
				case 3:
					valueOut = context.MapStringValueToInteger(context.Variables.AbilityId);
					return true;
				case 4:
					valueOut = context.MapStringValueToInteger(context.Variables.TargetType);
					return true;
				case 5:
					valueOut = ((context.Variables.SurvivorClass.Count > 0) ? context.MapStringValueToInteger(context.Variables.SurvivorClass[0]) : 0);
					return true;
				case 6:
					valueOut = ((context.Variables.Hero.Count > 0) ? context.MapStringValueToInteger(context.Variables.Hero[0]) : 0);
					return true;
				case 7:
					valueOut = context.Variables.Count;
					return true;
				case 8:
					valueOut = context.MapStringValueToInteger(context.Variables.GameMode);
					return true;
				case 9:
					valueOut = context.MapStringValueToInteger(context.Variables.ShopType);
					return true;
				case 10:
					valueOut = context.Variables.CurrentTime;
					return true;
				case 11:
					valueOut = context.Variables.CouncilLevel;
					return true;
				case 12:
					valueOut = context.Variables.EquipmentCount;
					return true;
				case 13:
					valueOut = context.MapStringValueToInteger(context.Variables.TargetSpecificType);
					return true;
				case 14:
					valueOut = context.Variables.ChallengeRoundsComplete;
					return true;
				}
			}
			return false;
		}

		private static bool IsValueInContextField(QuestCompleteContext context, long fieldIndex, long value, out bool isInField)
		{
			isInField = false;
			if ((ulong)fieldIndex <= 14uL)
			{
				switch ((int)fieldIndex)
				{
				case 0:
					isInField = value == context.MapStringValueToInteger(context.Variables.Operation);
					return true;
				case 1:
					isInField = value == context.MapStringValueToInteger(context.Variables.MissionKind);
					return true;
				case 2:
					isInField = value == context.MapStringValueToInteger(context.Variables.AbilityType);
					return true;
				case 3:
					isInField = value == context.MapStringValueToInteger(context.Variables.AbilityId);
					return true;
				case 4:
					isInField = value == context.MapStringValueToInteger(context.Variables.TargetType);
					return true;
				case 5:
				{
					if (value >= context.StringValues.Count)
					{
						return false;
					}
					string item2 = context.StringValues[(int)value];
					isInField = context.Variables.SurvivorClass.IndexOf(item2) >= 0;
					return true;
				}
				case 6:
				{
					if (value >= context.StringValues.Count)
					{
						return false;
					}
					string item = context.StringValues[(int)value];
					isInField = context.Variables.Hero.IndexOf(item) >= 0;
					return true;
				}
				case 7:
					isInField = value == context.Variables.Count;
					return true;
				case 8:
					isInField = value == context.MapStringValueToInteger(context.Variables.GameMode);
					return true;
				case 9:
					isInField = value == context.MapStringValueToInteger(context.Variables.ShopType);
					return true;
				case 10:
					isInField = value == context.Variables.CurrentTime;
					return true;
				case 11:
					isInField = value == context.Variables.CouncilLevel;
					return true;
				case 12:
					isInField = value == context.Variables.EquipmentCount;
					return true;
				case 13:
					isInField = value == context.MapStringValueToInteger(context.Variables.TargetSpecificType);
					return true;
				case 14:
					isInField = value == context.Variables.ChallengeRoundsComplete;
					return true;
				}
			}
			return false;
		}

		public static long MapIdentifierToContextField(string identifier)
		{
			return identifier switch
			{
				"Operation" => 0L,
				"MissionKind" => 1L,
				"AbilityType" => 2L,
				"AbilityId" => 3L,
				"TargetType" => 4L,
				"SurvivorClass" => 5L,
				"Hero" => 6L,
				"Count" => 7L,
				"GameMode" => 8L,
				"ShopType" => 9L,
				"CurrentTime" => 10L,
				"CouncilLevel" => 11L,
				"EquipmentCount" => 12L,
				"TargetSpecificType" => 13L,
				"ChallengeRoundsComplete" => 14L,
				_ => -1L,
			};
		}

		public long Evaluate(QuestCompleteContext context)
		{
			bool flag = false;
			if (Operation == Op.ContextField)
			{
				if (!MapIndexToContextValue(context, Value, out var valueOut))
				{
					context.ModelManager.Debug.LogError($"Could not match value {Value} to context value. Quest rule: {RuleId}");
				}
				return valueOut;
			}
			if (Operation == Op.Value)
			{
				return Value;
			}
			if (Operation == Op.Neg)
			{
				if (Right == null)
				{
					context.ModelManager.Debug.LogError($"Negating, but no right hand side available. Quest rule: {RuleId}");
					return 0L;
				}
				return (Right.Evaluate(context) == 0L) ? 1 : 0;
			}
			if (Operation == Op.FunctionCall)
			{
				if (Value >= context.StringValues.Count)
				{
					context.ModelManager.Debug.LogError($"FunctionCall operator value does not match a string value. Quest rule: {RuleId}");
					return 0L;
				}
				string text = context.StringValues[(int)Value];
				if (context.Functions.ContainsKey(text))
				{
					return context.Functions[text](context, Arguments);
				}
				context.ModelManager.Debug.LogError($"FunctionCall function {text} not found in function table. Quest rule: {RuleId}");
			}
			else
			{
				if (Operation == Op.IN)
				{
					long valueOut2 = 0L;
					if (Left.Operation == Op.ContextField)
					{
						if (!MapIndexToContextValue(context, Left.Value, out valueOut2))
						{
							context.ModelManager.Debug.LogError($"Could not match left operator value {Left.Value} to context value. Quest rule: {RuleId}");
						}
					}
					else
					{
						valueOut2 = Left.Value;
					}
					if (Right.Operation == Op.Group)
					{
						if (Right.GroupValues != null)
						{
							for (int i = 0; i < Right.GroupValues.Count; i++)
							{
								if (valueOut2 == Right.GroupValues[i])
								{
									return 1L;
								}
							}
						}
						return 0L;
					}
					if (Right.Operation == Op.ContextField)
					{
						if (!IsValueInContextField(context, Right.Value, valueOut2, out var isInField))
						{
							context.ModelManager.Debug.LogError($"Could not match right operator value {Right.Value} to context value. Quest rule: {RuleId}");
						}
						return isInField ? 1 : 0;
					}
					return 0L;
				}
				if (Left == null)
				{
					context.ModelManager.Debug.LogError($"Left operator missing when applying IN operator. Quest rule: {RuleId}");
					return 0L;
				}
				if (Right == null)
				{
					context.ModelManager.Debug.LogError($"Right operator missing when applying IN operator. Quest rule: {RuleId}");
					return 0L;
				}
				long num = Left.Evaluate(context);
				long num2 = Right.Evaluate(context);
				switch (Operation)
				{
				case Op.And:
					flag = num != 0L && num2 != 0;
					break;
				case Op.Equal:
					flag = num == num2;
					break;
				case Op.LessEqual:
					flag = num <= num2;
					break;
				case Op.GreaterEqual:
					flag = num >= num2;
					break;
				case Op.Less:
					flag = num < num2;
					break;
				case Op.Greater:
					flag = num > num2;
					break;
				default:
					return 0L;
				}
			}
			return flag ? 1 : 0;
		}

		public void PrintEquation(QuestCompleteContext context, StringBuilder sb)
		{
			if (Operation == Op.Group)
			{
				sb.Append("{");
				for (int i = 0; i < GroupValues.Count; i++)
				{
					sb.Append(GroupValues[i]);
					if (i + 1 < GroupValues.Count)
					{
						sb.Append(", ");
					}
				}
				sb.Append("}");
			}
			else if (Operation == Op.Bracket)
			{
				sb.Append("(");
				if (Right != null)
				{
					Right.PrintEquation(context, sb);
				}
				sb.Append(")");
			}
			else if (Operation == Op.FunctionCall)
			{
				sb.Append(Value);
				sb.Append("(");
				for (int j = 0; j < Arguments.Count; j++)
				{
					Arguments[j].PrintEquation(context, sb);
					if (j + 1 < Arguments.Count)
					{
						sb.Append(", ");
					}
				}
				sb.Append(")");
			}
			else if (Operation == Op.Value)
			{
				sb.Append("C(");
				sb.Append(Value);
				sb.Append(")");
			}
			else if (Operation == Op.ContextField)
			{
				sb.Append("F(");
				sb.Append(Value);
				sb.Append(")");
			}
			else
			{
				if (Left != null)
				{
					Left.PrintEquation(context, sb);
				}
				sb.Append(" <");
				sb.Append(Operation.ToString());
				sb.Append("> ");
				if (Right != null)
				{
					Right.PrintEquation(context, sb);
				}
			}
		}
	}
}
