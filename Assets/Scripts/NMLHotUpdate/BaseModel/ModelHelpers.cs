using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace BaseModel
{
	public static class ModelHelpers
	{
		public static long SecondsToMilliSeconds(int seconds)
		{
			return (long)seconds * 1000L;
		}

		public static int TicksToSeconds(long tickCount)
		{
			return (int)(tickCount / 1000);
		}

		public static string MD5Sum(string str)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			bytes = mD5CryptoServiceProvider.ComputeHash(bytes);
			int num = 0;
			char[] array = new char[bytes.Length * 2];
			for (int i = 0; i < array.Length; i += 2)
			{
				byte b = bytes[num++];
				array[i] = GetHexValue(b / 16);
				array[i + 1] = GetHexValue(b % 16);
			}
			return new string(array);
		}

		public static string MD5Sum(byte[] bytes)
		{
			bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
			int num = 0;
			char[] array = new char[bytes.Length * 2];
			for (int i = 0; i < array.Length; i += 2)
			{
				byte b = bytes[num++];
				array[i] = GetHexValue(b / 16);
				array[i + 1] = GetHexValue(b % 16);
			}
			return new string(array);
		}

		private static char GetHexValue(int index)
		{
			if (index < 10)
			{
				return (char)(index + 48);
			}
			return (char)(index - 10 + 97);
		}

		public static long MD5SumLong(string str)
		{
			long num = 0L;
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			bytes = mD5CryptoServiceProvider.ComputeHash(bytes);
			for (int i = 0; i < bytes.Length && i < 8; i++)
			{
				num = (num << 8) + bytes[i];
			}
			return num;
		}

		public static string GetRarityNameForAnalytics(int rarityLevel)
		{
			string result = string.Empty;
			if (rarityLevel < 5)
			{
				switch (rarityLevel)
				{
				case 0:
					result = "Common";
					break;
				case 1:
					result = "Uncommon";
					break;
				case 2:
					result = "Rare";
					break;
				case 3:
					result = "Epic";
					break;
				case 4:
					result = "Legendary";
					break;
				}
			}
			else
			{
				result = "Legendary" + (rarityLevel - 4);
			}
			return result;
		}
	}
}
