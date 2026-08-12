namespace TWDModel
{
	public interface IRunLocationErrorContext
	{
		void ReportError(string message);

		void ReportFatalError(string message);
	}
}
