using UnityEngine;

public class UIEvent : MonoBehaviour
{
	public delegate void UIEventDelegate(string type, object parameter);

	public static event UIEventDelegate OnUIEvent;

	public static void Send(string type, object parameter = null)
	{
		if (UIEvent.OnUIEvent != null)
		{
			UIEvent.OnUIEvent(type, parameter);
		}
	}
}
