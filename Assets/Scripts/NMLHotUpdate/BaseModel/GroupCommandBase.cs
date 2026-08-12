namespace BaseModel
{
	public class GroupCommandBase
	{
		public string GroupId;

		public string SenderId;

		public long Time;

		public long SequenceId;

		public virtual GroupCommandBase Execute(ModelManager manager)
		{
			return this;
		}
	}
}
