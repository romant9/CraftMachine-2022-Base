using System.Collections.Generic;

namespace TWDModel
{
	public class ActionLog
	{
		public ModelAction Action;

		public List<ActionModifierLog> Modifiers;

		public List<ActionLog> NestedActions;

		public List<AbilityLog> AbilityLogs;

		public AbilityLog CurrentAbilityLog;

		public bool Success;

		public void StartAbilityLog(ActorModel actor, AbilityModel ability)
		{
			CurrentAbilityLog = new AbilityLog(actor, ability);
		}

		public void EndAbilityLog(AbilityResult result)
		{
			if (CurrentAbilityLog != null)
			{
				CurrentAbilityLog.Result = result;
				if (AbilityLogs == null)
				{
					AbilityLogs = new List<AbilityLog>();
				}
				AbilityLogs.Add(CurrentAbilityLog);
			}
			CurrentAbilityLog = null;
		}

		public void Modifier(ActorModel actor, ActionModifier modifier, ActionListClearFlag clearFlag, List<ModelAction> visitAddedActions)
		{
			if (Modifiers == null)
			{
				Modifiers = new List<ActionModifierLog>();
			}
			Modifiers.Add(new ActionModifierLog
			{
				Actor = actor,
				ActionModifier = modifier,
				ClearFlag = clearFlag,
				AddedActions = visitAddedActions
			});
		}

		public string GetDetails(int depth, bool recurse)
		{
			string text = "";
			for (int i = 0; i < depth; i++)
			{
				text += "\t";
			}
			string text2 = text;
			text2 = text2 + Action.GetType().Name + "(" + Action.ToString() + "), succeeded = " + Success;
			if (Modifiers != null && Modifiers.Count > 0)
			{
				for (int j = 0; j < Modifiers.Count; j++)
				{
					text2 = text2 + "\n" + text + Modifiers[j].ToString();
				}
				text2 += "\n";
			}
			if (recurse && NestedActions != null)
			{
				for (int k = 0; k < NestedActions.Count; k++)
				{
					text2 = text2 + "\n" + NestedActions[k].GetDetails(depth + 1, recurse: true);
				}
			}
			return text2;
		}

		public override string ToString()
		{
			string text = "\n" + GetDetails(0, recurse: true);
			if (AbilityLogs != null)
			{
				for (int i = 0; i < AbilityLogs.Count; i++)
				{
					text += AbilityLogs[i].ToString();
				}
			}
			return text;
		}
	}
}
