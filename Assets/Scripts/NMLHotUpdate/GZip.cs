using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;

public class GZip
{
	public static string DeflateBase64String(string base64String)
	{
		string result = base64String;
		try
		{
			using MemoryStream baseInputStream = new MemoryStream(Convert.FromBase64String(base64String));
			using GZipInputStream stream = new GZipInputStream(baseInputStream);
			using StreamReader streamReader = new StreamReader(stream);
			result = streamReader.ReadToEnd();
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static string Deflate(byte[] bytes)
	{
		try
		{
			using MemoryStream baseInputStream = new MemoryStream(bytes);
			using GZipInputStream stream = new GZipInputStream(baseInputStream);
			using StreamReader streamReader = new StreamReader(stream);
			return streamReader.ReadToEnd();
		}
		catch (Exception)
		{
		}
		return "";
	}
}
