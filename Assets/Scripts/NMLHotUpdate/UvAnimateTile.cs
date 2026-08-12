using Client.Constants;
using UnityEngine;

public class UvAnimateTile : MonoBehaviour
{
	public int uvAnimationTileX = 10;

	public int uvAnimationTileY = 1;

	public float framesPerSecond = 1f;

	public float debug = 1f;

	public bool Loop = true;

	public bool onePerRow;

	private int id;

	private int row;

	private Vector2 offset;

	private Vector2 offset_next;

	private float startTime;

	private float age;

	private Renderer cachedRenderer;

	private MaterialPropertyBlock materialPropertyBlock;

	private void Start()
	{
		cachedRenderer = GetComponent<Renderer>();
		materialPropertyBlock = new MaterialPropertyBlock();
		id = GetInstanceID();
		startTime = Time.time;
		Update();
	}

	private void Update()
	{
		age = Time.time - startTime;
		float num = age * framesPerSecond;
		int num2 = (int)num;
		float num3 = num - (float)num2;
		Random.InitState(id);
		row = Mathf.FloorToInt(Random.value * (float)uvAnimationTileY);
		int num4 = 0;
		if (Loop)
		{
			num2 %= uvAnimationTileX * uvAnimationTileY;
			num4 = (num2 + 1) % (uvAnimationTileX * uvAnimationTileY);
		}
		else
		{
			num4 = Mathf.Min(num2 + 1, uvAnimationTileX * uvAnimationTileY - 1);
			num2 = Mathf.Min(num2, uvAnimationTileX * uvAnimationTileY - 1);
		}
		Vector2 vector = new Vector2(1f / (float)uvAnimationTileX, 1f / (float)uvAnimationTileY);
		int num5 = num2 % uvAnimationTileX;
		int num6 = num2 / uvAnimationTileX;
		int num7 = num4 % uvAnimationTileX;
		int num8 = num4 / uvAnimationTileX;
		if (!onePerRow)
		{
			offset = new Vector2((float)num5 * vector.x, 1f - vector.y - (float)num6 * vector.y);
			offset_next = new Vector2((float)num7 * vector.x, 1f - vector.y - (float)num8 * vector.y);
		}
		else
		{
			offset = new Vector2((float)num5 * vector.x, 1f - vector.y - (float)row * vector.y);
			offset_next = new Vector2((float)num7 * vector.x, 1f - vector.y - (float)row * vector.y);
		}
		materialPropertyBlock.Clear();
		materialPropertyBlock.SetVector(MaterialParameters.MainTexST, new Vector4(vector.x, vector.y, offset.x, offset.y));
		materialPropertyBlock.SetVector(MaterialParameters.NextTexST, new Vector4(vector.x, vector.y, offset_next.x, offset_next.y));
		materialPropertyBlock.SetFloat(MaterialParameters.FractionalFrame, num3 * debug);
		cachedRenderer.SetPropertyBlock(materialPropertyBlock);
	}
}
