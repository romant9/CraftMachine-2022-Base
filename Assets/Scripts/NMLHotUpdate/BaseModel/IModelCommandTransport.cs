namespace BaseModel
{
	public interface IModelCommandTransport
	{
		bool Send(IModelCommand command);
	}
}
