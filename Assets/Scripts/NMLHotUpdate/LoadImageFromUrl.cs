using System.Collections.Generic;
using UnityEngine;

public class LoadImageFromUrl : MonoBehaviour
{
	public static Dictionary<string, Texture2D> downloadedImages = new Dictionary<string, Texture2D>();

	public void LoadImage(string url, UITexture texture, int textureMaxHeight)
	{
		if (downloadedImages.ContainsKey(url))
		{
			Texture2D texture2D = downloadedImages[url];
			if (texture2D != null && texture != null)
			{
				texture.mainTexture = texture2D;
				float num = (float)textureMaxHeight / (float)Mathf.Max(texture2D.height, textureMaxHeight);
				int width = Mathf.RoundToInt((float)texture2D.width * num);
				int height = Mathf.RoundToInt((float)texture2D.height * num);
				texture.width = width;
				texture.height = height;
			}
			return;
		}
		ContentManager.Instance.GetCDNContent(url, "CDNImage", null, delegate(byte[] cdnContent)
		{
			if (cdnContent != null)
			{
				OnImage(cdnContent, url, texture, textureMaxHeight);
			}
		});
	}

	protected void OnImage(byte[] imageBytes, string url, UITexture texture, int textureMaxHeight)
	{
		Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGB24, mipChain: false);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.LoadImage(imageBytes);
		if (texture2D != null && texture != null)
		{
			texture.mainTexture = texture2D;
			float num = (float)textureMaxHeight / (float)Mathf.Max(texture2D.height, textureMaxHeight);
			int width = Mathf.RoundToInt((float)texture2D.width * num);
			int height = Mathf.RoundToInt((float)texture2D.height * num);
			texture.width = width;
			texture.height = height;
		}
		if (!downloadedImages.ContainsKey(url) && texture2D != null)
		{
			downloadedImages.Add(url, texture2D);
			UIEvent.Send("OnLoadImageComplete", url);
		}
	}
}
