using System.Text;
using UnityEngine;

public class MonoBehaviourExtended : MonoBehaviour
{
	protected string DebugIdString = "MonoBehaviourExtended";

	private const string ErrorMessageHeader = "UI Null Pointer Error!";

	private const string ErrorMessageContent = "Could not update Data or Reference was NULL!";

	private StringBuilder stringBuilder = new StringBuilder();

	public virtual void Clear()
	{
	}

	public bool IsNotNull(object obj, string originInfo = "")
	{
		if (obj != null)
		{
			return true;
		}
		NUllPointerMessage(originInfo);
		return false;
	}

	protected void NUllPointerMessage(string originInfo = "")
	{
		stringBuilder = new StringBuilder();
		stringBuilder.Append(DebugIdString);
		if (originInfo != "")
		{
			stringBuilder.Append(".");
			stringBuilder.Append(originInfo);
		}
		stringBuilder.Append(" : ");
		stringBuilder.Append("Could not update Data or Reference was NULL!");
		DebugLogError(stringBuilder.ToString(), this);
	}

	protected void DebugLog(string message, Object origin = null)
	{
		Debug.Log(DebugIdString + ": " + message, origin);
	}

	protected void DebugLogWarning(string message, Object origin = null)
	{
		Debug.LogWarning(DebugIdString + ": " + message, origin);
	}

	protected void DebugLogError(string message, Object origin = null)
	{
		Debug.LogError(DebugIdString + ": " + message, origin);
	}
}
