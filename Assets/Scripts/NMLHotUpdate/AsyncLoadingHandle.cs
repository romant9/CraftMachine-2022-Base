public class AsyncLoadingHandle
{
	public bool IsFinished { get; private set; }

	public string ErrorMessage { get; private set; }

	public float Progress { get; private set; }

	public void SignalFinished(string errorMessage)
	{
		IsFinished = true;
		ErrorMessage = errorMessage;
		Progress = 1f;
	}

	public void ReportProgress(float progress)
	{
		Progress = progress;
	}
}
