using System;
using Client.Constants;
using UnityEngine;

public class UvScroll : MonoBehaviour
{
	public bool UvScrollSpeedModfiedAtRuntime;

	public Vector2 uvScrollSpeed = new Vector2(1f, 0f);

	public Vector2 uvScrollSpeed2 = new Vector2(0f, 0f);

	private float startTime;

	private DateTime startDate;

	private bool initialized;

	private Renderer cachedRenderer;

	private MaterialPropertyBlock materialPropertyBlock;

	private Vector2 textureScaleA = Vector2.one;

	private Vector2 textureScaleB = Vector2.one;

	private void Start()
	{
		InitComponent();
	}

	private void InitComponent()
	{
		if (!initialized)
		{
			cachedRenderer = GetComponent<Renderer>();
			materialPropertyBlock = new MaterialPropertyBlock();
			textureScaleA = cachedRenderer.sharedMaterial.GetTextureScale(MaterialParameters.MainTex);
			if (cachedRenderer.sharedMaterial.HasProperty(MaterialParameters.SecondTex))
			{
				textureScaleB = cachedRenderer.sharedMaterial.GetTextureScale(MaterialParameters.SecondTex);
			}
			cachedRenderer.GetPropertyBlock(materialPropertyBlock);
			if (Application.isPlaying)
			{
				startTime = Time.time;
			}
			else
			{
				startDate = DateTime.Now;
			}
			initialized = true;
		}
	}

	private void Update()
	{
		InitComponent();
		float num;
		if (Application.isPlaying)
		{
			num = Time.time - startTime;
		}
		else
		{
			TimeSpan timeSpan = DateTime.Now - startDate;
			num = 60f * (float)timeSpan.Minutes + (float)timeSpan.Seconds + 0.001f * (float)timeSpan.Milliseconds;
		}
		if (UvScrollSpeedModfiedAtRuntime)
		{
			num = 1f;
		}
		materialPropertyBlock.Clear();
		materialPropertyBlock.SetVector(MaterialParameters.MainTexST, new Vector4(textureScaleA.x, textureScaleA.y, Mathf.Repeat(num * uvScrollSpeed.x, 1f), Mathf.Repeat(num * uvScrollSpeed.y, 1f)));
		materialPropertyBlock.SetVector(MaterialParameters.SecondTexST, new Vector4(textureScaleB.x, textureScaleB.y, Mathf.Repeat(num * uvScrollSpeed2.x, 1f), Mathf.Repeat(num * uvScrollSpeed2.y, 1f)));
		cachedRenderer.SetPropertyBlock(materialPropertyBlock);
	}
}
