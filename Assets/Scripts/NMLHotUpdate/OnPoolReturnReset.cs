using UnityEngine;

public class OnPoolReturnReset : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	[SerializeField]
	private bool resetPosition;

	[SerializeField]
	private Vector3 position = new Vector3(0f, 0f, 0f);

	[SerializeField]
	private bool resetScale;

	[SerializeField]
	private Vector3 scale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	private bool resetRotation;

	[SerializeField]
	private Vector3 euler = new Vector3(0f, 0f, 0f);

	public virtual void OnPoolReturn()
	{
		if (target != null)
		{
			if (resetPosition)
			{
				target.localPosition = position;
			}
			if (resetScale)
			{
				target.localScale = scale;
			}
			if (resetRotation)
			{
				target.localEulerAngles = euler;
			}
		}
	}
}
