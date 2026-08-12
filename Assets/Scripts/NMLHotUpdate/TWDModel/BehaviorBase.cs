using System.Collections.Generic;

namespace TWDModel
{
	public class BehaviorBase : IComparer<BehaviorBase>
	{
		protected AIController Controller { get; private set; }

		protected ActorModel Actor
		{
			get
			{
				if (Controller == null)
				{
					return null;
				}
				return Controller.Actor;
			}
		}

		protected CombatModel CombatModel
		{
			get
			{
				if (Controller == null)
				{
					return null;
				}
				return Controller.CombatModel;
			}
		}

		protected AIDataModel AIDataModel
		{
			get
			{
				if (Controller == null)
				{
					return null;
				}
				return Controller.AIDataModel;
			}
		}

		public BehaviorBase(AIController controller)
		{
			Controller = controller;
		}

		public virtual int GetPriority()
		{
			return 0;
		}

		public virtual void ExecuteAction()
		{
		}

		public int Compare(BehaviorBase x, BehaviorBase y)
		{
			int num = x?.GetPriority() ?? 0;
			return ((x != null) ? y.GetPriority() : 0) - num;
		}
	}
}
