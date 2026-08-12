using UnityEngine;

public class NUITouchManager : MonoBehaviour
{
	public static float PinchDelta;

	public static float PinchDeltaScale;

	private void Update()
	{
		if (Input.touchCount > 1)
		{
			Touch touch = Input.touches[0];
			Touch touch2 = Input.touches[1];
			Vector2 vector = touch.position - touch.deltaPosition;
			Vector2 vector2 = touch2.position - touch2.deltaPosition;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (touch.position - touch2.position).magnitude;
			PinchDelta = magnitude - magnitude2;
			PinchDeltaScale = magnitude / magnitude2 - 1f;
		}
		else
		{
			PinchDelta = 0f;
			PinchDeltaScale = 0f;
		}
	}
}
