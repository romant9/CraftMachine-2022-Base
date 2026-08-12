using UnityEngine;

public class Swing : MonoBehaviour
{
	public float amplitude;

	public float speed;

	protected float time;

	private void Start()
	{
		time = 0f;
	}

	private void Update()
	{
		float z = Mathf.Sin(time) * amplitude;
		time += speed * Time.deltaTime;
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.z = z;
		base.transform.eulerAngles = eulerAngles;
	}
}
