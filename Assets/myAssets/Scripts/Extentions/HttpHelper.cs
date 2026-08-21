using Newtonsoft.Json.Linq;
using Supabase.TWD;
using System;
using System.Text;

public static class HttpHelper
{
	private static JObject[] DecodeIdToken(string idToken)
	{
		var jObjects = new JObject[3];
		if (string.IsNullOrEmpty(idToken))
			throw new ArgumentException("id_token не может быть пустым");

		string[] parts = idToken.Split('.');
		if (parts.Length != 3)
			throw new FormatException("Некорректный JWT: ожидается 3 части (header.payload.signature)");

		int index = 0;
		foreach (var part in parts)
		{
			// Base64Url → обычный Base64
			string payloadBase64 = part.Replace('-', '+').Replace('_', '/');

			// Добавляем padding '=' если нужно
			int padding = payloadBase64.Length % 4;
			if (padding != 0)
				payloadBase64 += new string('=', 4 - padding);

			byte[] payloadBytes = Convert.FromBase64String(payloadBase64);
			string payloadJson = Encoding.UTF8.GetString(payloadBytes);
			try
			{
				jObjects[index] = JObject.Parse(payloadJson);
			}
			catch
			{
				jObjects[index] = null;
			}
			index++;
		}
		return jObjects;
	}

	/// <summary>
	/// Парсинг полей idToken
	/// </summary>
	/// <param name="idToken"></param>
	public static void ParsePayload(string idToken)
	{
		try
		{
			var jObjects = DecodeIdToken(idToken);
			if (jObjects == null || jObjects.Length != 3) return;
			JObject payload = jObjects[1];

			// Примеры получения полей
			string sub = payload.Value<string>("sub");           // уникальный ID пользователя
			string email = payload.Value<string>("email");       // email
			string name = payload.Value<string>("name");         // имя
			string givenName = payload.Value<string>("given_name");
			string familyName = payload.Value<string>("family_name");

			Debug.Log($"User: {name} ({email}), sub={sub}");
		}
		catch (Exception e)
		{
			Debug.LogError("Ошибка декодирования JWT: " + e.Message);
		}
	}

	/// <summary>
	/// вариант парсинга redirect url, чтобы взять code или id_token
	/// </summary>
	/// <param name="url"></param>
	/// <returns></returns>
	public static string ExtractCodeFromUrl(string url)
	{
		var uri = new System.Uri(url);
		var queryDict = HttpUtility.ParseQueryString(uri.Query); // или свой простой парсер
		return queryDict["code"];
	}
}
