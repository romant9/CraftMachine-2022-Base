using System.Collections.Generic;

namespace TWDModel
{
	public class EffectIndexPriorityItem
	{
		public int Priority { get; private set; }

		public List<string> NegativeEffects { get; private set; }

		public EffectIndexPriorityItem(int priority, List<string> negativeEffects)
		{
			Priority = priority;
			NegativeEffects = negativeEffects;
		}
	}
}
