using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
	[SerializeField]
	private float speed;

	[SerializeField]
	private float arcHeight;

	[SerializeField]
	private bool rotateWeapon;

	[SerializeField]
	private float rotationSpeed;

	[SerializeField]
	private UnityEvent OnThrown;

	[SerializeField]
	private UnityEvent OnReset;

	private Vector3 startPosition;

	private Vector3 target;

	private float stepScale;

	private float currentProgress;

	private Vector3 initialPosition;

	private Quaternion initialRotation;

	[SerializeField]
	private Transform originalParent;

	private void Awake()
	{
		initialPosition = base.transform.localPosition;
		initialRotation = base.transform.localRotation;
	}

	private void Update()
	{
		currentProgress = Mathf.Min(currentProgress + Time.deltaTime * stepScale, 1f);
		float num = 1f - 4f * (currentProgress - 0.5f) * (currentProgress - 0.5f);
		Vector3 vector = Vector3.Lerp(startPosition, target, currentProgress);
		vector.y += num * arcHeight;
		if (rotateWeapon)
		{
			base.transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f, Space.Self);
		}
		else
		{
			base.transform.LookAt(vector, base.transform.forward);
		}
		base.transform.position = vector;
		if (Mathf.Approximately(currentProgress, 1f))
		{
			Reset();
		}
	}

	private void Reset()
	{
		OnReset?.Invoke();
		currentProgress = 0f;
		base.transform.SetParent(originalParent);
		base.transform.localPosition = initialPosition;
		base.transform.localRotation = initialRotation;
		base.enabled = false;
	}

	public void Throw(Vector3 target)
	{
		base.transform.parent = null;
		startPosition = base.transform.position;
		float num = Vector3.Distance(startPosition, target);
		stepScale = speed / num;
		SetTarget(target);
		base.enabled = true;
		OnThrown?.Invoke();
		Vector3 normalized = (target - base.transform.position).normalized;
		base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, Mathf.Atan2(normalized.x, normalized.z) * 57.29578f, base.transform.eulerAngles.z);
	}

	private void SetTarget(Vector3 target)
	{
		this.target = target;
	}
}
