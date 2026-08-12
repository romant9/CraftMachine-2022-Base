using System;
using System.Threading;
using BaseModel;

public class DeserializationWorker<T>
{
	public bool ready;

	private IMessageSerializer serializer;

	private string json;

	private Thread worker;

	public Exception Error { get; private set; }

	public T Result { get; private set; }

	public bool Ready
	{
		get
		{
			if (Error != null)
			{
				throw Error;
			}
			return ready;
		}
	}

	public DeserializationWorker(IMessageSerializer serializer, string json)
	{
		this.serializer = serializer;
		this.json = json;
	}

	public void Start()
	{
		worker = new Thread(Execute);
		worker.Start();
	}

	public void Join()
	{
		if (worker != null)
		{
			worker.Join();
		}
		if (Error != null)
		{
			throw Error;
		}
	}

	private void Execute()
	{
		try
		{
			Result = serializer.DeserializeObject<T>(json);
		}
		catch (Exception error)
		{
			Error = error;
		}
		finally
		{
			ready = true;
		}
	}
}
