using System;

namespace UnityAuth
{
    public enum TaskStatus
    {
        Success,
        Fail,
        NeedAuth,
        Error,
        Offline,
        Exception
    }

    public class TaskResult
	{
		public TaskStatus Status { get; set; }
		public string Message { get; set; }
		public Exception Exception { get; set; }

		public TaskResult(TaskStatus status, string message, Exception ex = null) 
		{ 
			Status = status; Message = message; Exception = ex;
			if (ex == null)
			{
				DebugTWD.Log($"[{status}]{message}");
			}
			else
			{
				DebugTWD.LogError($"[{status}]{message}\n{ex.Message}");
			}
		}
	}

}
