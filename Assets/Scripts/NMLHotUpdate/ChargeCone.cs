using UnityEngine;

public class ChargeCone : MonoBehaviour
{
	public float rotSpeed = 100f;

	public float alphaSpeed = 2f;

	private float alpha;

	private void Start()
	{
		alpha = 1f;
		GetComponent<Renderer>().material.color = new Color(alpha, alpha, alpha, alpha);
	}

	private void Update()
	{
		base.transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f);
		int instanceID = base.gameObject.GetInstanceID();
		alpha = 0.3f + 0.7f * Mathf.PerlinNoise(11.77f + (float)instanceID, alphaSpeed * Time.time);
		GetComponent<Renderer>().material.color = new Color(alpha, alpha, alpha, alpha);
	}
}
