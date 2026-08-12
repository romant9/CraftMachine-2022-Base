using UnityEngine;

public class RenderQueue : MonoBehaviour
{
	[Header("Background is 1000")]
	[Header("Geometry is 2000")]
	[Header("AlphaTest is 2450")]
	[Header("Transparent is 3000")]
	[Header("Overlay is 4000")]
	public int renderQueue = 2100;

	private void Start()
	{
		for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
		{
			GetComponent<Renderer>().materials[i].renderQueue = renderQueue;
		}
	}
}
