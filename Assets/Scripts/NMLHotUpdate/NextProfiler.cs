using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class NextProfiler
{
	private struct AssetInfo
	{
		public string Name;

		public long Size;

		public AssetInfo(string name, long size)
		{
			Name = name;
			Size = size;
		}
	}

	public struct MemoryUsage
	{
		public long UITextureUsage;

		public long OtherTextureUsage;

		public long MeshUsage;
	}

	private static NextProfiler instance;

	private List<AssetInfo> assetInfos = new List<AssetInfo>();

	public static NextProfiler Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new NextProfiler();
			}
			return instance;
		}
	}

	public int GetResidentMemoryUsage()
	{
		return 0;
	}

	public void LogResourceUsage(string tag)
	{
		StringBuilder stringBuilder = new StringBuilder(2048);
		stringBuilder.AppendFormat("Resource Usage at {0}\n", tag);
		DumpAssets<Texture>(stringBuilder, 100, doBinning: false);
		DumpAssets<Mesh>(stringBuilder, 10, doBinning: false);
		DumpAssets<AudioClip>(stringBuilder, 10, doBinning: false);
		DumpAssets<Material>(stringBuilder, 5, doBinning: false);
		DumpAssets<GameObject>(stringBuilder, 5, doBinning: false);
		DumpAssets<Component>(stringBuilder, 5, doBinning: false);
		DumpAssets<Shader>(stringBuilder, 100000, doBinning: false);
		stringBuilder.AppendFormat("Mono: heap {0} used {1}\n", Profiler.GetMonoHeapSizeLong(), Profiler.GetMonoUsedSizeLong());
		int num = 1048576;
		stringBuilder.AppendFormat("Total allocated {0},  MB\nTotal reserved {1} MB\n", Profiler.GetTotalAllocatedMemoryLong() / num, Profiler.GetTotalReservedMemoryLong() / num);
		if (PlatformInfo.CurrentPlatform == TargetPlatform.Android)
		{
			File.WriteAllText(Application.persistentDataPath + "/" + DateTime.Now.ToShortTimeString().Replace(":", "-") + "_asset_dump.txt", stringBuilder.ToString());
		}
	}

	public void Mark(string tag)
	{
	}

	public void LogShaderDependencies()
	{
	}

	public void LogInvalidAABBs()
	{
	}

	private bool IsVectorFiniteNumber(Vector3 v)
	{
		if (!float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) && !float.IsInfinity(v.x) && !float.IsInfinity(v.y))
		{
			return !float.IsInfinity(v.z);
		}
		return false;
	}

	protected void DumpAssets<T>(StringBuilder text, int dumpTopCount, bool doBinning) where T : UnityEngine.Object
	{
		T[] array = Resources.FindObjectsOfTypeAll<T>();
		string arg = typeof(T).Name.ToUpper();
		if (array.Length == 0)
		{
			text.AppendFormat("NO ASSETS OF TYPE {0}\n", arg);
			return;
		}
		assetInfos.Clear();
		long num = 0L;
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i].name))
			{
				AssetInfo item = new AssetInfo(array[i].name, Profiler.GetRuntimeMemorySizeLong(array[i]));
				assetInfos.Add(item);
				num += item.Size;
			}
		}
		assetInfos.Sort(delegate(AssetInfo x, AssetInfo y)
		{
			if (y.Size > x.Size)
			{
				return 1;
			}
			return (y.Size < x.Size) ? (-1) : 0;
		});
		int num2 = 0;
		text.AppendFormat("{0} ({1})\n", arg, assetInfos.Count);
		if (doBinning)
		{
			text.Append("Bins:\n");
			long size = assetInfos[0].Size;
			for (int num3 = 0; num3 < assetInfos.Count; num3++)
			{
				if (assetInfos[num3].Size != size)
				{
					text.AppendFormat("Bin {0} bytes: {1}\n", size, num2);
					num2 = 0;
					size = assetInfos[num3].Size;
				}
				num2++;
			}
			text.AppendFormat("Bin {0} bytes: {1}\n", size, num2);
		}
		if (dumpTopCount > 0)
		{
			text.AppendFormat("Top {0}:\n", dumpTopCount);
			for (int num4 = 0; num4 < dumpTopCount && num4 < assetInfos.Count; num4++)
			{
				text.AppendFormat("{0}: {1}\n", assetInfos[num4].Name, assetInfos[num4].Size);
			}
		}
		text.AppendFormat("{0} TOTAL: {1} bytes\n", arg, num);
	}

	public MemoryUsage DumpAssetBudgetting(int uiTextureMaxSize, int otherTextureMaxSize, List<string> oversizedAssetNames)
	{
		MemoryUsage result = default(MemoryUsage);
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(Texture));
		UnityEngine.Object[] array2 = Resources.FindObjectsOfTypeAll(typeof(Mesh));
		UnityEngine.Object[] array3 = array;
		foreach (UnityEngine.Object obj in array3)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			TextureFormat format;
			if (obj is Texture2D && !obj.name.Contains("Gizmo"))
			{
				Texture2D obj2 = (Texture2D)obj;
				num = obj2.width;
				num2 = obj2.height;
				format = obj2.format;
				num3 = obj2.mipmapCount;
			}
			else
			{
				if (!(obj is Texture3D))
				{
					continue;
				}
				Texture3D obj3 = (Texture3D)obj;
				num = obj3.width;
				num2 = obj3.height;
				format = obj3.format;
			}
			long num4 = 0L;
			bool flag = true;
			switch (format)
			{
			case TextureFormat.PVRTC_RGB2:
				num4 = (num * num2 * 2 + 7) / 8;
				break;
			case TextureFormat.PVRTC_RGB4:
				num4 = (num * num2 * 4 + 7) / 8;
				break;
			case TextureFormat.PVRTC_RGBA2:
				num4 = (num * num2 * 2 + 7) / 8;
				break;
			case TextureFormat.PVRTC_RGBA4:
				num4 = (num * num2 * 4 + 7) / 8;
				break;
			default:
				num4 = Profiler.GetRuntimeMemorySizeLong(obj);
				flag = false;
				break;
			}
			if (flag && num3 > 0)
			{
				num4 = (int)((float)num4 * 1.33f);
			}
			string text = obj.name.ToUpper();
			if (text.StartsWith("UI_") || text.StartsWith("ATLAS_UI_") || text.Contains("HUD_"))
			{
				if (oversizedAssetNames != null && obj is Texture && num4 > uiTextureMaxSize)
				{
					oversizedAssetNames.Add("UI  " + obj.name + ", " + num4);
				}
				result.UITextureUsage += num4;
				continue;
			}
			if (oversizedAssetNames != null && obj is Texture)
			{
				string text2 = obj.name;
				if (obj is RenderTexture)
				{
					text2 += "RT";
				}
				if (text2.Length == 0)
				{
					text2 = "Unnamed " + format;
				}
				if (num4 > otherTextureMaxSize)
				{
					oversizedAssetNames.Add("TEX " + text2 + ", " + num4);
				}
			}
			result.OtherTextureUsage += num4;
		}
		array3 = array2;
		foreach (UnityEngine.Object o in array3)
		{
			result.MeshUsage += Profiler.GetRuntimeMemorySizeLong(o);
		}
		return result;
	}

	public static long GetAssetsSize<T>() where T : UnityEngine.Object
	{
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll<T>();
		UnityEngine.Object[] array2 = array;
		long num = 0L;
		array = array2;
		foreach (UnityEngine.Object o in array)
		{
			num += Profiler.GetRuntimeMemorySizeLong(o);
		}
		return num;
	}

	public string DumpTextures()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Texture information\n");
		DumpAssets<Texture>(stringBuilder, 10, doBinning: false);
		return stringBuilder.ToString();
	}

	public static void FindAllReferencesOfType(Type typeToFind, string assetNamePrefix)
	{
	}

	public static int FindObjectsReferencing<T>(T objectToFind) where T : UnityEngine.Object
	{
		return 0;
	}

	public static bool FieldReferencesComponent<T>(Component obj, FieldInfo fieldInfo, T objectToFind) where T : UnityEngine.Object
	{
		if (fieldInfo.FieldType.IsArray)
		{
			if (!(fieldInfo.GetValue(obj) is Array array))
			{
				return false;
			}
			foreach (object item in array)
			{
				if (item != null && item.GetType() == objectToFind.GetType() && item as T == objectToFind)
				{
					return true;
				}
			}
		}
		else if (fieldInfo.FieldType == objectToFind.GetType() && fieldInfo.GetValue(obj) as T == objectToFind)
		{
			return true;
		}
		return false;
	}
}
