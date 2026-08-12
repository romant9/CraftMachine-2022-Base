using System.Collections.Generic;

namespace TWDModel
{
	public interface IModifierCollection
	{
		bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor = null);

		void VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions);

		void RegisterModifier(ModelModifier modifier);

		void RemoveModifier(ModelModifier modifier);

		bool HasModifier(ModelModifier modifier);

		int GetCount();

		ModelModifier GetModifier(int index);
	}
}
