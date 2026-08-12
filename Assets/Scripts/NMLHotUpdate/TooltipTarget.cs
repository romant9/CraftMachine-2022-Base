using UnityEngine;

public class TooltipTarget : MonoBehaviour
{
	public enum Orientation
	{
		AUTO = 0,
		UP = 1,
		DOWN = 2,
		LEFT = 3,
		RIGHT = 4,
		AUTOVERTICAL = 5,
		CENTER = 6
	}

	[Header("Oriantation Override. Default is AUTO")]
	public Orientation OrientationOverride;

	[Header("Position offset")]
	public float OffsetFromTarget;
}
