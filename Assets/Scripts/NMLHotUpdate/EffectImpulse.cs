using UnityEngine;

public class EffectImpulse : MonoBehaviour
{
	public Vector3 Impulse = new Vector3(0f, 0f, 0f);

	public Vector3 Torque = new Vector3(0f, 0f, 0f);

	public bool Rand;

	private void Start()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			if (Rand)
			{
				Impulse = new Vector3(Impulse.x * 2f * (Random.value - 0.5f), Impulse.y * 2f * (Random.value - 0.5f), Impulse.z * 2f * (Random.value - 0.5f));
				Torque = new Vector3(Torque.x * Random.value, Torque.y * Random.value, Torque.z * Random.value);
			}
			component.AddForce(Impulse, ForceMode.Impulse);
			component.AddTorque(Torque, ForceMode.Impulse);
		}
	}

	private void Update()
	{
	}
}
