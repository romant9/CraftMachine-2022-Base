using Client.Constants;
using UnityEngine;

public class UvAnimateTileNGUI : MonoBehaviour
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

	private UITexture texture;

	private Material mat;

	private void Start()
	{
		id = GetInstanceID();
		startTime = Time.time;
		texture = base.gameObject.GetComponent<UITexture>();
		texture.onRender = OnRender;
	}

	private void OnRender(Material mat)
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
		if (mat != null)
		{
			mat.SetTextureOffset("_MainTex", offset);
			mat.SetTextureScale("_MainTex", value);
			mat.SetTextureOffset("_NextTex", offset_next);
			mat.SetTextureScale("_NextTex", value);
			mat.SetFloat(MaterialParameters.FractionalFrame, num3 * debug);
		}
	}
}
