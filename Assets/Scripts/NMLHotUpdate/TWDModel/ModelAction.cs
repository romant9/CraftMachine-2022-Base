using BaseModel;

namespace TWDModel
{
	public class ModelAction
	{
		protected bool hasOrderWhenGrouped;

		protected int sortOrder;

		public bool Visited { get; set; }

		public int ModelId { get; private set; }

		public ModelAction(ModelObject model)
		{
			ModelId = model?.ModelId ?? (-1);
			Visited = false;
		}

		public bool HasOrderWhenGrouped()
		{
			return hasOrderWhenGrouped;
		}

		public virtual bool Execute(ModelManager manager)
		{
			return false;
		}

		public virtual void PostAbilityExecute(ModelManager manager)
		{
		}

		public virtual bool CanExecute()
		{
			return true;
		}

		public virtual int SortOrder()
		{
			return sortOrder;
		}

		public virtual void SetSortOrder(int order)
		{
			sortOrder = order;
		}
	}
}
