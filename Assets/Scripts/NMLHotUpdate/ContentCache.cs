using BaseModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class ContentCache
{
	public class CacheFile
	{
		public string Id;

		public string Url;

		public string Checksum;

		#region myparams
		public long TimeStamp;
		#endregion
	}

	private static readonly string _rootPath = Application.persistentDataPath + "/ContentCache";

	private static readonly string _indexFile = "index";

	private List<CacheFile> _files;

	private string _cachePath;

	private string _contentType;

	private long _maxFiles;

	private IMessageSerializer _serializer;

	public static void DeleteAll()
	{
		try
		{
			string hosts = new DirectoryInfo(_rootPath).Parent.FullName + "/Hosts";
			if (File.Exists(hosts))
			{
				File.Delete(hosts);
			}
			if (Directory.Exists(_rootPath))
			{
				Directory.Delete(_rootPath, recursive: true);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("ContentCache.DeleteAll " + ex.Message.Split('"')[0]);
		}
	}

	public static void CheckVersion(string gameVersion)
	{
		try
		{
			string text = "";
			string path = _rootPath + "/version.txt";
			if (!Directory.Exists(_rootPath)) return;
			if (File.Exists(path))
			{
				text = File.ReadAllText(path).Trim();
			}
			if (string.IsNullOrEmpty(text) || gameVersion != text)
			{
				File.WriteAllText(path, gameVersion);
				DebugTWD.Log("Текущая версия: " + gameVersion + " отлична от сохраненной в кэше: " + text);
				return;
				//OfflineManager.ShortVersion = text;
				//MyTools.OpenInfo("Текущая версия: " + gameVersion + " отлична от сохраненной в кэше: " + text + "\nудалить весь кэш?", OnCheckVersionYesCallback, OnCheckVersionCancelCallback);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("ContentCache.CheckVersion " + ex.Message.Split('"')[0]);
		}
	}

	public static void OnCheckVersionYesCallback()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.ConfirmationPopup);

		DeleteAll();
		Directory.CreateDirectory(_rootPath);
		string path = _rootPath + "/version.txt";
		File.WriteAllText(path, OfflineManager.ShortVersion);
	}

	public static void OnCheckVersionCancelCallback()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.ConfirmationPopup);

		string path = _rootPath + "/version.txt";
		File.WriteAllText(path, OfflineManager.ShortVersion);
	}

	public ContentCache(string contentType, long maxFiles, IMessageSerializer serializer)
	{
		if (string.IsNullOrEmpty(contentType))
		{
			throw new Exception("contentType not defined");
		}
		if (maxFiles <= 0)
		{
			throw new Exception("maxFiles must be greater than 0");
		}
		_files = new List<CacheFile>();
		_cachePath = _rootPath + "/" + contentType;
		_maxFiles = maxFiles;
		_serializer = serializer;
		_contentType = contentType;
		try
		{
			if (!Directory.Exists(_cachePath))
			{
				Directory.CreateDirectory(_cachePath);
			}
			string filePath = GetFilePath(_indexFile);
			if (File.Exists(filePath))
			{
				string value = File.ReadAllText(filePath);
				if (!string.IsNullOrEmpty(value))
				{
					_files = _serializer.DeserializeObject<List<CacheFile>>(value);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("ContentCache " + ex.Message.Split('"')[0]);
		}
	}

	public string GetChecksum(string id)
	{
		for (int i = 0; i < _files.Count; i++)
		{
			if (_files[i].Id == id)
			{
				return _files[i].Checksum;
			}
		}
		return null;
	}

	public T GetContent<T>(string checksum) where T : class
	{
		try
		{
			byte[] array = File.ReadAllBytes(GetFilePath(checksum));
			if (CalculateChecksum(array) != checksum)
			{
				throw new Exception("Checksum mismatch");
			}
			if (typeof(T) == typeof(string))
			{
				return Encoding.UTF8.GetString(array) as T;
			}
			if (typeof(T) == typeof(byte[]))
			{
				return array as T;
			}
			throw new Exception("Invalid content type");
		}
		catch (Exception ex)
		{
			Debug.LogError("ContentCache.GetContent " + ex.Message.Split('"')[0] + ":" + (string.IsNullOrEmpty(checksum) ? checksum : ""));
		}
		if (!OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (!OfflineManager.IsLoadDataManager) DeleteContent(checksum)");
			DeleteContent(checksum);
		}
		return null;
	}

	public T GetContentByUrl<T>(string url) where T : class
	{
		for (int i = 0; i < _files.Count; i++)
		{
			if (_files[i].Url == url)
			{
				return GetContent<T>(_files[i].Checksum);
			}
		}
		return null;
	}

	public T GetContentById<T>(string id) where T : class
	{
		for (int i = 0; i < _files.Count; i++)
		{
			if (_files[i].Id == id)
			{
				return GetContent<T>(_files[i].Checksum);
			}
		}
		return null;
	}

	public string GetUrlById(string id)
	{
		for (int i = 0; i < _files.Count; i++)
		{
			if (_files[i].Id == id)
			{
				return _files[i].Url;
			}
		}
		return null;
	}

	public void SetContent<T>(string id, string url, string checksum, T content, long timeStamp = 0) where T : class
	{
		bool flag = false;
		if (string.IsNullOrEmpty(checksum))
		{
			checksum = CalculateChecksum(content);
		}
		for (int i = 0; i < _files.Count; i++)
		{
			if (_files[i].Checksum == checksum)
			{
				flag = true;
				if (_files[i].Id == id)
				{
					_files.RemoveAt(i);
					break;
				}
			}
		}
		if (OfflineManager.IsLoadDataManager && _contentType == "Image")
		{
			var deltaOver = _files.Count -_maxFiles;
			if (deltaOver > 0)
			{
				for (int j = 0; j < deltaOver; j++)
				{
					if (_files[j].Checksum != checksum)
					{
						DeleteFile(_files[j].Checksum);
					}
					_files.RemoveAt(j);
				}
			}
		}
		else
		{
			for (int num = _files.Count - 1; num >= _maxFiles - 1; num--)
			{
				if (_files[num].Checksum != checksum)
				{
					DeleteFile(_files[num].Checksum);
				}
				_files.RemoveAt(num);
			}
		}
				
		if (!flag)
		{
			WriteFile(checksum, content);
		}
		_files.Insert(0, new CacheFile
		{
			Id = id,
			Url = url,
			Checksum = checksum,
			TimeStamp = timeStamp
		});
		WriteFile(_indexFile, _serializer.SerializeObject(_files));
	}

	private void WriteFile<T>(string checksum, T content) where T : class
	{
		try
		{
			string filePath = GetFilePath(checksum);
			if (typeof(T) == typeof(string))
			{
				File.WriteAllText(filePath, content as string);
				return;
			}
			if (typeof(T) == typeof(byte[]))
			{
				File.WriteAllBytes(filePath, content as byte[]);
				return;
			}
			throw new Exception("Invalid content type");
		}
		catch (Exception ex)
		{
			Debug.LogError("ContentCache.WriteFile " + ex.Message.Split('"')[0]);
		}
	}

	private void DeleteFile(string checksum)
	{
		try
		{
			File.Delete(GetFilePath(checksum));
		}
		catch (Exception ex)
		{
			Debug.LogError("ContentCache.DeleteFile " + ex.Message.Split('"')[0]);
		}
	}

	public void DeleteContent(string checksum)
	{
		for (int num = _files.Count - 1; num >= 0; num--)
		{
			if (_files[num].Checksum == checksum)
			{
				DeleteFile(_files[num].Checksum);
				_files.RemoveAt(num);
			}
		}
		WriteFile(_indexFile, _serializer.SerializeObject(_files));
	}

	public void DeleteContentWithId(string fileId)
	{
		for (int num = _files.Count - 1; num >= 0; num--)
		{
			if (_files[num].Id == fileId)
			{
				DeleteFile(_files[num].Checksum);
				_files.RemoveAt(num);
			}
		}
		WriteFile(_indexFile, _serializer.SerializeObject(_files));
	}

	private string GetFilePath(string checksum)
	{
		return $"{_cachePath}/{checksum}.txt";
	}

	public static string CalculateChecksum<T>(T content)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = null;
		if (typeof(T) == typeof(string))
		{
			array = Encoding.UTF8.GetBytes(content as string);
		}
		else
		{
			if (!(typeof(T) == typeof(byte[])))
			{
				throw new Exception("Invalid content type");
			}
			array = content as byte[];
		}
		array = mD5CryptoServiceProvider.ComputeHash(array);
		int num = 0;
		char[] array2 = new char[array.Length * 2];
		for (int i = 0; i < array2.Length; i += 2)
		{
			byte b = array[num++];
			array2[i] = GetHexValue(b / 16);
			array2[i + 1] = GetHexValue(b % 16);
		}
		return new string(array2);
	}

	public static bool CheckIsValidChecksum(string checksum)
	{
		if (!OfflineManager.IsUseChecksum)
		{
			DebugTWD.LogMycode("if (!OfflineManager.IsUseChecksum) return true");
			return true;
		}

		bool flag = checksum.Length == 32;
		if (flag)
		{
			foreach (char c in checksum)
			{
				if ((c < '0' || c > '9') && (c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	private static char GetHexValue(int index)
	{
		if (index < 10)
		{
			return (char)(index + 48);
		}
		return (char)(index - 10 + 97);
	}



	#region mycode
	public bool HasContentById(string id)
	{
		for (int i = 0; i < _files.Count; i++)
		{
			if (_files[i].Id == id)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasContentByIndex(int index, out long date, out string id)
	{
		date = 0;
		id = null;
		for (int i = 0; i < _files.Count; i++)
		{
			var items = _files[i].Id.Split('_');
			if (items.Length > 1)
			{
				if (int.TryParse(items[1], out int result))
				{
					if (result == index)
					{
						date = _files[i].TimeStamp;
						id = items[0];

						return true;
					}
				}
			}
		}
		return false;
	}
	#endregion
}
