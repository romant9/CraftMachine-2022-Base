using Client.Constants;
using UnityEngine;

public class UvAnimateTileBatching : MonoBehaviour
{
	public int uvAnimationTileX = 10;

	public int uvAnimationTileY = 1;

	public float framesPerSecond = 1f;

	public float debug = 1f;

	public bool onePerRow;

	private int id;

	private int row;

	private Vector2 offset;

	private Vector2 offset_next;

	private ParticleSystem PS;

	private float startTime;

	private float age;

	private void Start()
	{
		id = GetInstanceID();
		startTime = Time.time;
	}

	private void Update()
	{
		age = Time.time - startTime;
		float num = age * framesPerSecond;
		int num2 = (int)num;
		float num3 = num - (float)num2;
		Random.InitState(id);
		row = Mathf.FloorToInt(Random.value * (float)uvAnimationTileY);
		num2 %= uvAnimationTileX * uvAnimationTileY;
		int num4 = (num2 + 1) % (uvAnimationTileX * uvAnimationTileY);
		Vector2 value = new Vector2(1f / (float)uvAnimationTileX, 1f / (float)uvAnimationTileY);
		int num5 = num2 % uvAnimationTileX;
		int num6 = num2 / uvAnimationTileX;
		int num7 = num4 % uvAnimationTileX;
		int num8 = num4 / uvAnimationTileX;
		if (!onePerRow)
		{
			offset = new Vector2((float)num5 * value.x, 1f - value.y - (float)num6 * value.y);
			offset_next = new Vector2((float)num7 * value.x, 1f - value.y - (float)num8 * value.y);
		}
		else
		{
			offset = new Vector2((float)num5 * value.x, 1f - value.y - (float)row * value.y);
			offset_next = new Vector2((float)num7 * value.x, 1f - value.y - (float)row * value.y);
		}
		Material material = GetComponent<Renderer>().material;
		material.SetTextureOffset("_MainTex", offset);
		material.SetTextureScale("_MainTex", value);
		material.SetTextureOffset("_NextTex", offset_next);
		material.SetTextureScale("_NextTex", value);
		material.SetFloat(MaterialParameters.FractionalFrame, num3 * debug);
	}
}
