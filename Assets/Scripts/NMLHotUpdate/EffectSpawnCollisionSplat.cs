using UnityEngine;

public class EffectSpawnCollisionSplat : MonoBehaviour
{
	public GameObject Splat;

	private bool done;

	private int id;

	private void Start()
	{
		id = GetInstanceID();
	}

	private void OnCollisionEnter(Collision coll)
	{
		Random.InitState(id + Mathf.FloorToInt(Time.time * 18f));
		int mask = LayerMask.GetMask("LVL", "Static");
		if (((1 << coll.gameObject.layer) & mask) == 0)
		{
			return;
		}
		float num = 1f + 1f * Random.value;
		if (!done)
		{
			GameObject gameObject = Object.Instantiate(Splat);
			if (coll.contacts != null && coll.contacts.Length != 0)
			{
				gameObject.transform.localPosition = new Vector3(coll.contacts[0].point.x, 0.012f, coll.contacts[0].point.z);
				gameObject.transform.localEulerAngles = new Vector3(90f, 360f * Random.value, 0f);
				gameObject.transform.localScale = 0.7f * new Vector3(num, num * 0.9f, num);
				gameObject.SetActive(value: true);
			}
			done = true;
		}
	}
}
