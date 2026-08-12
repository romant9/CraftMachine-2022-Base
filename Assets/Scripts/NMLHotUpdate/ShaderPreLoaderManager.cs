using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPreLoaderManager
{
	private Queue<Material> materialsToRender = new Queue<Material>();

	private static ShaderPreLoaderManager sm_Instance;

	public static ShaderPreLoaderManager Instance
	{
		get
		{
			if (sm_Instance == null)
			{
				sm_Instance = new ShaderPreLoaderManager();
			}
			return sm_Instance;
		}
	}

	public void addShaderToPreload(Material inMat)
	{
		materialsToRender.Enqueue(inMat);
	}

	public IEnumerator flushShaderQueue()
	{
		Camera localCamera = Camera.allCameras[0];
		int height = 32;
		RenderTexture targetTexture = new RenderTexture(32, height, 16, RenderTextureFormat.ARGB32);
		RenderTexture previousRenderTexture = localCamera.targetTexture;
		localCamera.targetTexture = targetTexture;
		GameObject localObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		localObject.transform.position = new Vector3(0f, 100f, -100f);
		foreach (Material item in materialsToRender)
		{
			localObject.GetComponent<Renderer>().material = item;
			localCamera.Render();
			yield return null;
		}
		localCamera.targetTexture = previousRenderTexture;
	}
}
