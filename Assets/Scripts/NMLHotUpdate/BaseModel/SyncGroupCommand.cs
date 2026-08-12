namespace BaseModel
{
	public class SyncGroupCommand : GroupCommandBase
	{
		public override GroupCommandBase Execute(ModelManager manager)
		{
			return this;
		}
	}
}
