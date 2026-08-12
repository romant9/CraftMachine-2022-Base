using UnityEngine;

public class EffectPhysicsAutoDestroy : MonoBehaviour
{
	public float LifeTime = 5f;

	public float FadeOutTime = 1f;

	private float startTime;

	private float age;

	private int stage;

	private Rigidbody RB;

	private SquashStretch SS;

	private void Start()
	{
		startTime = Time.time;
		stage = 0;
		RB = GetComponent<Rigidbody>();
		SS = GetComponent<SquashStretch>();
	}

	private void Update()
	{
		age = Time.time - startTime;
		if (age > LifeTime && stage == 0)
		{
			if (RB != null)
			{
				RB.isKinematic = true;
				RB.Sleep();
			}
			if (SS != null)
			{
				SS.enabled = false;
			}
			stage = 1;
		}
		if (age > LifeTime && stage == 1 && age < LifeTime + FadeOutTime)
		{
			float num = 0.02f * (age - LifeTime) / FadeOutTime;
			Vector3 vector = base.transform.InverseTransformDirection(Vector3.down);
			base.transform.Translate(num * vector);
		}
		if (stage == 1 && age > LifeTime + FadeOutTime)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
