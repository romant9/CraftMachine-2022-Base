using System.Collections.Generic;
using System.IO;
using System.Linq;
using TWDModel;
using UnityEngine;

public class PortraitCache
{
	private string cacheDirectory;

	public PortraitCache()
	{
		if (IsLoadFromResources)
		{
			cacheDirectory = "Portraits";
			return;
		}
		cacheDirectory = Application.persistentDataPath + "/Portraits";
		if (File.Exists(cacheDirectory + "/Version"))
		{
			GameVersion gameVersion = new GameVersion(File.ReadAllText(cacheDirectory + "/Version"));
			GameVersion other = new GameVersion(OfflineManager.ShortVersion);
			if (gameVersion.CompareTo(other) != 0)
			{
				Directory.Delete(cacheDirectory, recursive: true);
			}
		}
		if (!Directory.Exists(cacheDirectory))
		{
			Directory.CreateDirectory(cacheDirectory);
			File.WriteAllText(cacheDirectory + "/Version", OfflineManager.ShortVersion);
		}
	}

	private string GetFilePath(IPortraitRenderSource source)
	{
		string text = string.IsNullOrEmpty(source.Prefab) ? source.ActorDefinitionId : source.Prefab;
		return cacheDirectory + "/" + text + (!IsLoadFromResources ? "_" + source.OutfitDefinitionId + ".jpg" : "");
	}

	public bool Contains(IPortraitRenderSource source)
	{
		if (IsLoadFromResources) return Resources.Load(GetFilePath(source)) != null;

		return File.Exists(GetFilePath(source));
	}

	public RenderTexture Load(IPortraitRenderSource source, int width, int height, RenderTexture renderTexture = null)
	{
		string filePath = GetFilePath(source);
		try
		{
			Texture2D texture2D;
			if (IsLoadFromResources)
			{
				texture2D = Resources.Load(filePath) as Texture2D;
			}
			else
			{
				byte[] data = File.ReadAllBytes(filePath);
				texture2D = new Texture2D(8, 8, TextureFormat.ARGB32, mipChain: false, linear: false);
				texture2D.LoadImage(data, markNonReadable: true);
			}

			if (texture2D == null) return null;

			texture2D.filterMode = FilterMode.Bilinear;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			width = Mathf.Min(width, texture2D.width);
			height = Mathf.Min(height, texture2D.height);
			if (renderTexture == null)
			{
				renderTexture = new RenderTexture(width, height, 0, PortraitManager.Instance.Format);
			}
			else if (!renderTexture.IsCreated())
			{
				renderTexture.width = width;
				renderTexture.height = height;
			}
			renderTexture.hideFlags = HideFlags.HideAndDontSave;
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.autoGenerateMips = false;
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			renderTexture.name = "PortraitFromCache";
			renderTexture.Create();
			renderTexture.DiscardContents();
			Graphics.Blit(texture2D, renderTexture);
			return renderTexture;
		}
		catch (IOException ex)
		{
			DebugTWD.LogWarning("Failed to load portrait from: " + filePath + ". Message: " + ex.Message);
			if (!OfflineManager.IsLoadDataManager) File.Delete(filePath);
		}
		return null;
	}

	public void Store(IPortraitRenderSource source, Texture2D texture)
	{
		if (Contains(source) && !source.IsRebuild)
		{
			return;
		}
		string filePath = GetFilePath(source);
		if (source.IsRebuild) Remove(source);
		try
		{
			byte[] bytes = texture.EncodeToJPG();
			File.WriteAllBytes(filePath, bytes);
		}
		catch (IOException ex)
		{
			Debug.LogWarning("Failed to store portrait " + ex.Message);
		}
	}

	public void Remove(IPortraitRenderSource source)
	{
		string filePath = GetFilePath(source);
		try
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
		catch (IOException ex)
		{
			Debug.LogWarning("Failed to remove portrait " + ex.Message);
		}
	}

	public void RemoveAll()
	{
		cacheDirectory = Application.persistentDataPath + "/Portraits";
		if (!Directory.Exists(cacheDirectory))
		{
			return;
		}
		var hashSet = new HashSet<string> { ".png", ".jpg" };
		var files = new DirectoryInfo(cacheDirectory).EnumerateFiles("*.*", SearchOption.TopDirectoryOnly).Where(f => hashSet.Contains(f.Extension));
		foreach (FileInfo item in files)
		{
			if (item.Exists) item.Delete();
		}
	}



	#region myparams
	private bool IsLoadFromResources => OfflineManager.ConfigBuildType == OfflineManager.ConfigDataType.Light || OfflineManager.IsLoadFromResources;
	#endregion
}
