using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

[ExecuteInEditMode]
public class ExtractAtlas : MonoBehaviour
{
	public NGUIAtlas Atlas;
	public List<NGUIAtlas> Atlases;

	public bool DoExtract;

	public int pass = 0;

	public class SpriteEntry : UISpriteData
	{
		// Sprite texture -- original texture or a temporary texture
		public Texture2D tex;

		// Temporary game object -- used to prevent Unity from unloading the texture
		public GameObject tempGO;

		// Temporary material -- same usage as the temporary game object
		public Material tempMat;

		// Whether the texture is temporary and should be deleted
		public bool temporaryTexture = false;

		/// <summary>
		/// HACK: Prevent Unity from unloading temporary textures.
		/// Discovered by "alexkring": http://www.tasharen.com/forum/index.php?topic=3079.45
		/// </summary>

		public void SetTexture(Color32[] newPixels, int newWidth, int newHeight)
		{
			Release();

			temporaryTexture = true;

			tex = new Texture2D(newWidth, newHeight);
			tex.name = name;
			tex.SetPixels32(newPixels);
			tex.Apply();

			if (tempMat == null) return;

			tempMat = new Material(tempMat);
			tempMat.hideFlags = HideFlags.HideAndDontSave;
			tempMat.SetTexture("_MainTex", tex);

#if UNITY_EDITOR
			tempGO = EditorUtility.CreateGameObjectWithHideFlags(name, HideFlags.HideAndDontSave, typeof(MeshRenderer));
			tempGO.GetComponent<MeshRenderer>().sharedMaterial = tempMat;
#endif
		}

		/// <summary>
		/// Release temporary resources.
		/// </summary>

		public void Release()
		{
			if (temporaryTexture)
			{
				UnityEngine.Object.DestroyImmediate(tempGO);
				UnityEngine.Object.DestroyImmediate(tempMat);
				UnityEngine.Object.DestroyImmediate(tex);

				tempGO = null;
				tempMat = null;
				tex = null;
				temporaryTexture = false;
			}
		}
	}

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (DoExtract)
		{
			DoExtract = false;
			ExtractSingle();
		}
	}

	[ContextMenu("Extract Single")]
	public void ExtractSingle()
	{
		var sprites = new List<SpriteEntry>();

		ExtractSprites(Atlas, sprites);

		DebugTWD.Log("Sprites extracted from " + Atlas.name);

		var path = Application.dataPath + "/../Textures/" + Atlas.name + "/";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		foreach (var sprite in sprites)
		{
			SaveToPng(sprite.tex, sprite.name, path);
		}

		DebugTWD.Log("Sprites saved to \n" + path);
	}

	[ContextMenu("Extract Multi")]
	public void ExtractMulti()
	{
		//var tasks = new List<Task>();
		foreach (var atlas in Atlases)
		{
			List<UISpriteData> spritesAll = atlas.spriteList;
			int countAll = spritesAll.Count;
			int chunkSize = 50;
			//var chunkCount = (Math.Ceiling((double)countAll / 50) - 1);
			List<List<UISpriteData>> spritesAllList = new();

			var mat = atlas.spriteMaterial;
			var tex = ToTexture2D(mat.mainTexture);
			if (tex == null) continue;

			for (int i = 0; i < countAll; i += chunkSize)
			{
				int count = Math.Min(chunkSize, countAll - i);
				var chunk = spritesAll.GetRange(i, count);
				spritesAllList.Add(chunk);
			}

			var path = Application.dataPath + "/../Textures/" + atlas.name + "/";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			for (int j = 0; j < spritesAllList.Count; j ++)
			{
				var chunk = spritesAllList[j];
				string message = $"Process for {atlas.name}. Count is {j + 1}/{countAll}";
				//var task = Task.Run(() => ExtractSpritesRange(message, tex, mat, chunk));
				ExtractSpritesRange(path, message, tex, mat, chunk);
				//tasks.Add(task);
			}
		}
		DebugTWD.Log("Succesfull extract all atlases");
		//RunAllTasks(tasks);
	}

	public void RunAllTasks(List<Task> tasks)
	{
		Task.WhenAll(tasks);
		DebugTWD.Log("Succesfull extract all atlases");
	}

	private void ExtractSpritesRange(string path, string message, Texture2D tex, Material mat, List<UISpriteData> spritesIn)
	{
		DebugTWD.Log(message);

		List<SpriteEntry> spritesOut = new();
		Color32[] pixels = null;
		var width = tex.width;
		var height = tex.height;
		for (int i = 0; i < spritesIn.Count; i++)
		{
			UISpriteData es = spritesIn[i];

			Texture2D myTexture2D = Texture2DReadable(tex);

			pixels ??= myTexture2D.GetPixels32();
			var sprite = ExtractSprite(es, pixels, width, height, mat);
			if (sprite != null)
			{
				sprite.CopyBorderFrom(es);
				spritesOut.Add(sprite);
			}
		}

		if (spritesOut.Count > 0)
		{
			foreach (var sprite in spritesOut)
			{
				SaveToPng(sprite.tex, sprite.name, path);
			}
			DebugTWD.Log("Sprites saved to \n" + path);
		}
	}


	private int StartIndex(int count)
	{
		switch (pass)
		{
			case 0: return 0;
			case 1: return count > 50 ? 50 : count;
			case 2: return count > 100 ? 100 : count;
			case 3: return count > 150 ? 150 : count;
			case 4: return count > 200 ? 200 : count;
			case 5: return count > 250 ? 250 : count;
			case 6: return count > 300 ? 300 : count;
			case 7: return count > 350 ? 350 : count;
			case 8: return count > 400 ? 400 : count;

			default: return 0;
		}
	}
	private int EndIndex(int count)
	{
		switch (pass)
		{
			case 0: return count > 50 ? 50 : count;
			case 1: return count > 100 ? 100 : count;
			case 2: return count > 150 ? 150 : count;
			case 3: return count > 200 ? 200 : count;
			case 4: return count > 250 ? 250 : count;
			case 5: return count > 300 ? 300 : count;
			case 6: return count > 350 ? 350 : count;
			case 7: return count > 400 ? 400 : count;
			case 8: return count > 450 ? 450 : count;
			default: return count;
		}
	}

	public void ExtractSprites(INGUIAtlas atlas, List<SpriteEntry> finalSprites)
	{
		//Make the atlas texture readable
		//var tex = ImportTexture(atlas.texture, true, true, false);
		var tex = ToTexture2D(atlas.spriteMaterial.mainTexture);

		if (tex != null)
		{
			var sprites = atlas.spriteList;
			int count = sprites.Count;
			DebugTWD.Log("Count is " + count + "\nMax pass is " + (Math.Ceiling((double)count/50) - 1));

			if (sprites != null)
			{
				Color32[] pixels = null;
				var width = tex.width;
				var height = tex.height;
				for (int i = StartIndex(sprites.Count); i < EndIndex(sprites.Count); i++)
				{
					UISpriteData es = sprites[i];
					bool found = false;

					foreach (SpriteEntry fs in finalSprites)
					{
						if (es.name == fs.name)
						{
							fs.CopyBorderFrom(es);
							found = true;
							break;
						}
					}

					if (!found)
					{
						Texture2D myTexture2D = Texture2DReadable(tex);

						pixels ??= myTexture2D.GetPixels32();
						var sprite = ExtractSprite(es, pixels, width, height, atlas.spriteMaterial);
						if (sprite != null) finalSprites.Add(sprite);
					}
				}
			}
		}
	}

#if UNITY_EDITOR
	static public Texture2D ImportTexture(Texture tex, bool forInput, bool force, bool alphaTransparency)
	{
		if (tex != null)
		{
			var path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
			return ImportTexture(path, forInput, force, alphaTransparency);
		}
		return null;
	}

	static public Texture2D ImportTexture(string path, bool forInput, bool force, bool alphaTransparency)
	{
		if (!string.IsNullOrEmpty(path))
		{
			var tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			return tex;
		}
		return null;
	}
#endif

	public static Texture2D Texture2DReadable(Texture2D tex)
	{
		RenderTexture tmp = RenderTexture.GetTemporary(
						tex.width,
						tex.height,
						0,
						RenderTextureFormat.Default,
						RenderTextureReadWrite.Default);

		// Blit the pixels on texture to the RenderTexture
		Graphics.Blit(tex, tmp);

		// Backup the currently set RenderTexture
		RenderTexture previous = RenderTexture.active;

		// Set the current RenderTexture to the temporary one we created
		RenderTexture.active = tmp;

		// Create a new readable Texture2D to copy the pixels to it
		Texture2D myTexture2D = new Texture2D(tex.width, tex.height);

		// Copy the pixels from the RenderTexture to the new Texture
		myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
		myTexture2D.Apply();

		// Reset the active RenderTexture
		RenderTexture.active = previous;

		// Release the temporary RenderTexture
		RenderTexture.ReleaseTemporary(tmp);

		return myTexture2D;
	}

	static SpriteEntry ExtractSprite(UISpriteData es, Color32[] oldPixels, int oldWidth, int oldHeight, Material mat)
	{
		int xmin = Mathf.Clamp(es.x, 0, oldWidth);
		int ymin = Mathf.Clamp(es.y, 0, oldHeight);
		int xmax = Mathf.Min(xmin + es.width, oldWidth - 1);
		int ymax = Mathf.Min(ymin + es.height, oldHeight - 1);
		int newWidth = Mathf.Clamp(es.width, 0, oldWidth);
		int newHeight = Mathf.Clamp(es.height, 0, oldHeight);

		if (newWidth == 0 || newHeight == 0) return null;

		var newPixels = new Color32[newWidth * newHeight];

		for (int y = 0; y < newHeight; ++y)
		{
			int cy = ymin + y;
			if (cy > ymax) cy = ymax;

			for (int x = 0; x < newWidth; ++x)
			{
				int cx = xmin + x;
				if (cx > xmax) cx = xmax;

				int newIndex = (newHeight - 1 - y) * newWidth + x;
				int oldIndex = (oldHeight - 1 - cy) * oldWidth + cx;

				newPixels[newIndex] = oldPixels[oldIndex];
			}
		}

		// Create a new sprite
		var sprite = new SpriteEntry();
		sprite.CopyFrom(es);
		sprite.SetRect(0, 0, newWidth, newHeight);
		sprite.tempMat = mat;
		sprite.SetTexture(newPixels, newWidth, newHeight);
		return sprite;
	}

	public static Texture2D ToTexture2D(Texture texture)
	{
		return Texture2D.CreateExternalTexture(
			texture.width,
			texture.height,
			TextureFormat.RGB24,
			false, false,
			texture.GetNativeTexturePtr());
	}

	public static void SaveToPng(Texture2D texture, string name, string folder)
	{
		byte[] bytes = texture.EncodeToPNG();
		string path = folder + "\\" + name + ".png";
		File.WriteAllBytes(path, bytes);
	}
}
