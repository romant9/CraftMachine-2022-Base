using System;

namespace TWDModel
{
	public class ModelModifier : TWDModelObject
	{
		public IModifierCollection OwningCollection { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public virtual void Disable()
		{
			OwningCollection.RemoveModifier(this);
		}

		public virtual void Enable(IModifiableModel modifiable)
		{
			OwningCollection.RegisterModifier(this);
		}

		public void OnAdded(IModifierCollection collection)
		{
			OwningCollection = collection;
		}

		public void OnRemoved(IModifierCollection collection)
		{
			if (collection != OwningCollection)
			{
				throw new Exception("Trying to remove modifier from collection that it does not belong to!");
			}
			OwningCollection = null;
		}
	}
}
