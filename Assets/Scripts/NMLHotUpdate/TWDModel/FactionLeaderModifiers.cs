using System.Collections.Generic;

namespace TWDModel
{
	public class FactionLeaderModifiers : TWDModelObject, IModifierCollection
	{
		public ActorModel Leader { get; set; }

		public IModifierCollection ModifierCollection { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			ModifierCollection = new ModifierCollection();
			TWDModelObject obj = ModifierCollection as TWDModelObject;
			obj.SetManager(base.manager);
			obj.Initialize();
		}

		public override bool IsValid()
		{
			return true;
		}

		public bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor = null)
		{
			return ModifierCollection.VisitParameter(paramName, ref value, actor);
		}

		public void VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			ModifierCollection.VisitActions(action, actor, addedActions);
		}

		public void RegisterModifier(ModelModifier modifier)
		{
			ModifierCollection.RegisterModifier(modifier);
		}

		public void RemoveModifier(ModelModifier modifier)
		{
			ModifierCollection.RemoveModifier(modifier);
		}

		public bool HasModifier(ModelModifier modifier)
		{
			return ModifierCollection.HasModifier(modifier);
		}

		public int GetCount()
		{
			return ModifierCollection.GetCount();
		}

		public ModelModifier GetModifier(int index)
		{
			return ModifierCollection.GetModifier(index);
		}
	}
}
