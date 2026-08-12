using System.Collections.Generic;

namespace TWDModel
{
	public class ErrorCollector : IRunLocationErrorContext
	{
		public List<string> FatalErrors = new List<string>();

		public List<string> Errors = new List<string>();

		public void ReportError(string message)
		{
			Errors.Add(message);
		}

		public void ReportFatalError(string message)
		{
			FatalErrors.Add(message);
		}
	}
}
