using System.Collections.Generic;

namespace TWDModel
{
	public class ActionModifierLog
	{
		public ActionModifier ActionModifier;

		public ActionListClearFlag ClearFlag;

		public List<ModelAction> AddedActions;

		public ActorModel Actor;

		public override string ToString()
		{
			string text = ActionModifier.GetType().Name + "(" + ((Actor != null) ? Actor.Name : "") + ", " + ClearFlag.ToString() + ")";
			if (AddedActions != null && AddedActions.Count > 0)
			{
				text += " => ";
				for (int i = 0; i < AddedActions.Count; i++)
				{
					if (i > 0)
					{
						text += ", ";
					}
					text += AddedActions[i].GetType().Name;
				}
			}
			return text;
		}
	}
}
