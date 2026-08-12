using BaseModel;

namespace TWDModel
{
	public class DelayedEventData
	{
		public ModelObject model;

		public string changed;

		public object args;

		public DelayedEventData()
		{
		}

		public DelayedEventData(ModelObject model, string changed, object args)
		{
			this.model = model;
			this.changed = changed;
			this.args = args;
		}
	}
}
