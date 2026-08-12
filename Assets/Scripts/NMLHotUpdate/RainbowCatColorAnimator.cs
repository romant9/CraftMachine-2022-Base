using UnityEngine;

public class RainbowCatColorAnimator : MonoBehaviour
{
	private static readonly int TintTextureId = Shader.PropertyToID("_TintTexture");

	[SerializeField]
	private float scrollSpeed;

	[SerializeField]
	private Material referenceMaterial;

	[SerializeField]
	private Renderer[] renderers;

	private Material materialInstance;

	private Vector2 currentOffset;

	private int tintTextureId;

	private void Start()
	{
		materialInstance = Object.Instantiate(referenceMaterial);
		Renderer[] array = renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial = materialInstance;
		}
		currentOffset = materialInstance.GetTextureOffset(TintTextureId);
	}

	private void Update()
	{
		currentOffset += Vector2.right * Time.deltaTime * scrollSpeed;
		materialInstance.SetTextureOffset(TintTextureId, currentOffset);
	}
}
