using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Extention_Methods;

public class Extention_Methods
{
	public enum ItemReplaceType
	{
		Shader,
		Script,
		Image,
		Asset,
		Font,
		Prefab,
		Material
	}

	public enum CompareType
	{
		NotInPlace1,
		NotInPlace2,
		NotEqual
	}

	public enum PrefabReplaceType
	{
		mAtlas,
		m_Script,
		mColor,
		mTrueTypeFont,
		mTexture
	}

	public static void GetFiles(DirectoryInfo root, ref List<ResourceItem> files, List<ItemReplaceType> types, string[] mask = null)
	{
		for (int i = 0; i < types.Count; i++)
		{
			string ext = SetExtention(types[i]);
			//get the files in the current directory
			var shaders = root.GetFiles().Where(x => x.Extension == ext && (mask != null && !string.IsNullOrEmpty(mask[i]) ? x.Name.ToLower().Contains(mask[i]) : true));
			if (shaders != null && shaders.Count() > 0)
			{
				foreach (var s in shaders)
				{
					var name = Path.GetFileNameWithoutExtension(s.FullName);
					var pathGlobal = s.FullName.Replace("\\", "/");
					ResourceItem item = new ResourceItem(name, pathGlobal);
					string guid = "";
					if (File.Exists(s.FullName + ".meta"))
					{
						guid = File.ReadAllLines(s.FullName + ".meta")[1].Split(":")[1].TrimStart(' ');
					}
					item.FileId_Old = SetFileID(types[i]);
					item.FileGuid_Old = guid;
					item.typeOld = (Extention_Methods.ItemReplaceType)types[i];

					files.Add(item);
				}
			}

			// scan subdirectories
			DirectoryInfo[] dirs = root.GetDirectories();
			foreach (DirectoryInfo dir in dirs)
				GetFiles(dir, ref files, types);
		}

	}

	public static string SetExtention(ItemReplaceType type)
	{
		switch (type)
		{
			case ItemReplaceType.Shader: return ".shader";
			case ItemReplaceType.Script: return ".cs";
			case ItemReplaceType.Image: return ".png";
			case ItemReplaceType.Asset: return ".asset";
			case ItemReplaceType.Prefab: return ".prefab";
			case ItemReplaceType.Font: return ".ttf";

			default: return null;
		}
	}

	public static string SetFileID(ItemReplaceType type)
	{
		switch (type)
		{
			case ItemReplaceType.Shader: return "4800000";
			case ItemReplaceType.Material: return "2100000";
			case ItemReplaceType.Script: return "11500000";
			case ItemReplaceType.Image: return "2800000";
			case ItemReplaceType.Asset: return "11400000";
			case ItemReplaceType.Font: return "12800000";

			default: return null;
		}
	}

	public static long GetFileID(ItemReplaceType type)
	{
		switch (type)
		{
			case ItemReplaceType.Shader: return 4800000;
			case ItemReplaceType.Material: return 2100000;
			case ItemReplaceType.Script: return 11500000;
			case ItemReplaceType.Image: return 2800000;
			case ItemReplaceType.Asset: return 11400000;
			case ItemReplaceType.Font: return 12800000;

			default: return 11500000;
		}
	}
}

[Serializable]
public class ResourceItemNew
{
	public string Name { get; set; }
	public string Extention { get; set; }

	//from
	public long DllFileId { get; set; }
	public long ScriptFileId { get; set; }

	//to
	public string DllGuid { get; set; }
	public string ScriptGuid { get; set; }

	public string FilePath { get; set; }

	public ResourceItemNew()
	{
	}

	public ResourceItemNew(string dllGuid, string name, long dllFileId)
	{
		Name = name;
		Extention = ".cs";
		DllFileId = dllFileId;
		DllGuid = dllGuid;
	}

	public ResourceItemNew(string name, string ext, string guid, string path)
	{
		Name = name;
		Extention = ext;
		ScriptFileId = 11500000;
		ScriptGuid = guid;
		FilePath = path;
	}
}

public class ResourceItem
{
	public string Name_Old { get; set; }
	public string LocalPath_Old { get; set; }
	public string FileId_Old { get; set; }
	public string FileGuid_Old { get; set; }

	public ItemReplaceType typeOld { get; set; }

	public string Name_New { get; set; }
	public string LocalPath_New { get; set; }
	public string FileId_New { get; set; }
	public string FileGuid_New { get; set; }

	public ItemReplaceType typeNew { get; set; }

	public bool IsAbsentInNew { get; set; }

	public ResourceItem()
	{
	}

	public ResourceItem(string name, string pathOrigin)
	{
		Name_Old = name;
		LocalPath_Old = pathOrigin;
	}

	public ResourceItem(ResourceItem item)
	{
		Name_Old = item.Name_Old ?? null;
		LocalPath_Old = item.LocalPath_Old ?? null;
		FileId_Old = item.FileId_Old ?? null;
		FileGuid_Old = item.FileGuid_Old ?? null;
		typeOld = item.typeOld;
		typeNew = item.typeNew;
		Name_New = item.Name_New ?? null;
		LocalPath_New = item.LocalPath_New ?? null;
		FileId_New = item.FileId_New ?? null;
		FileGuid_New = item.FileGuid_New ?? null;
		IsAbsentInNew = item.IsAbsentInNew;
	}
}

public class ResourceCompareItem
{
	public string Name { get; set; }
	public string LocalPath1 { get; set; }
	public string LocalPath2 { get; set; }
	public CompareType compareType { get; set; }
	public ResourceCompareItem(string name, string path1, string path2, CompareType type)
	{
		Name = name;
		LocalPath1 = path1;
		LocalPath2 = path2;
		compareType = type;
	}
}
