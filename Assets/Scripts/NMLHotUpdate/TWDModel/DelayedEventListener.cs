using BaseModel;

namespace TWDModel
{
	public class DelayedEventListener
	{
		public int listenerCount;

		public event ModelChangeEventHandler listeners;

		public DelayedEventListener()
		{
			listenerCount = 0;
		}

		public void Dispatch(DelayedEventData ded)
		{
			this.listeners?.Invoke(ded.model, ded.changed, ded.args);
		}
	}
}
