using System;
using System.IO;
using UnityEngine;

public class HiResScreenShots : MonoBehaviour
{
	public int resWidth = 2550;

	public int resHeight = 3300;

	public bool UseAlpha;

	public bool CaptureSequence;

	public int CaptureFPS = 25;

	private bool takeHiResShot;

	private bool capturing;

	public static string ScreenShotName(int width, int height, bool sequence = false)
	{
		if (sequence)
		{
			return $"{Application.dataPath}/screenshots/seq_{Time.frameCount:D04}.png";
		}
		return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", Application.dataPath, width, height, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public void TakeHiResShot()
	{
		takeHiResShot = true;
	}

	public void Start()
	{
		if (CaptureSequence)
		{
			Time.captureFramerate = CaptureFPS;
		}
	}

	private void LateUpdate()
	{
		takeHiResShot |= Input.GetKeyDown("k");
		if (takeHiResShot && CaptureSequence)
		{
			capturing = !capturing;
		}
		if (takeHiResShot || capturing)
		{
			RenderTexture renderTexture = ((!UseAlpha) ? new RenderTexture(resWidth, resHeight, 24) : new RenderTexture(resWidth, resHeight, 32, RenderTextureFormat.ARGB32));
			GetComponent<Camera>().targetTexture = renderTexture;
			Texture2D texture2D = ((!UseAlpha) ? new Texture2D(resWidth, resHeight, TextureFormat.RGB24, mipChain: false) : new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, mipChain: false));
			GetComponent<Camera>().Render();
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, resWidth, resHeight), 0, 0);
			GetComponent<Camera>().targetTexture = null;
			RenderTexture.active = null;
			UnityEngine.Object.Destroy(renderTexture);
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(ScreenShotName(resWidth, resHeight, CaptureSequence), bytes);
			takeHiResShot = false;
		}
	}
}
