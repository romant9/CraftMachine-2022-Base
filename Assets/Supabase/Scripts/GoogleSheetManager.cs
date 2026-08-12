using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Supabase.TWD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static Supabase.TWD.SupabaseManager;
using ValueRange = Google.Apis.Sheets.v4.Data.ValueRange;

public class GoogleSheetManager: MonoBehaviour
{
    [SerializeField]
    private GoogleSheetSettings Settings;
    // ID таблицы можно взять из её URL-адреса в браузере
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
#if UNITY_EDITOR
	//private static readonly string ApplicationName = "BadgeCraft Users";
	//private static readonly string SpreadsheetId = "1rz_YMUaov96sCZ8yfq2jXSszz3U9q5-_TdMWO5O_ulA";
	//"2PACX-1vTI5kXJHuIKF9DTOChSaqaQktsdSTqPhE7u7aRNO_GCekcuzYfPmGrZa3_vs7yg8TFbf_uOd2SQ-My8";
	//"1rz_YMUaov96sCZ8yfq2jXSszz3U9q5-_TdMWO5O_ulA";
	//private static readonly string WebAppUrl = "https://script.google.com/macros/s/AKfycbz2R1wrUbHAeQs3Nym7-SLQCBcVDfKJLm10R_XhwfIznHjGiAqfiCRtnPfCjeABu9jB/exec";
	//Идентификатор развертывания - AKfycbz2R1wrUbHAeQs3Nym7-SLQCBcVDfKJLm10R_XhwfIznHjGiAqfiCRtnPfCjeABu9jB
#endif

	[Serializable]
    private class RequestPayload
    {
        public string fileUrl;
		public string action;
    }

    // Классы для десериализации ответа от Google
    [Serializable]
    private class ResponseData
    {
        public bool success;
        public string data;
        public string error;
        public string datatype;
    }

    [ContextMenu("Execute Read Sheet")]
    public void Execute()
    {
        // Авторизация с использованием JSON-ключа сервисного аккаунта
        var credentials = Resources.Load<TextAsset>("craftmachine-credentials").text;
        // 1. Загружаем конкретный тип учетных данных через фабрику
        var specificCredential = CredentialFactory.FromJson<ServiceAccountCredential>(credentials);

        // 2. Преобразуем его в универсальный GoogleCredential и задаем Scope
        var credential = specificCredential.ToGoogleCredential().CreateScoped(Scopes);

        // 3. Создаем сервис
        SheetsService service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = Settings.ApplicationName,
        });
        
        // 1. ЗАПИСЬ ДАННЫХ в диапазон "Лист1!A1:B2"
        var rangeToWrite = "GED!A1:B2";
        var valueRange = new ValueRange
        {
            Values = new List<IList<object>>
            {
                new List<object> { "Имя", "Возраст" },
                new List<object> { "Алексей", 28 }
            }
        };

        var updateRequest = service.Spreadsheets.Values.Update(valueRange, Settings.SpreadsheetId, rangeToWrite);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        updateRequest.Execute();
        DebugTWD.Log("Данные успешно записаны!");

        // 2. ЧТЕНИЕ ДАННЫХ из диапазона "Лист1!A1:B2"
        var rangeToRead = "GED!A1:B2";
        var getRequest = service.Spreadsheets.Values.Get(Settings.SpreadsheetId, rangeToRead);
        var response = getRequest.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0)
        {
            DebugTWD.Log("\nСчитанные данные:");
            foreach (var row in values)
            {
                DebugTWD.Log($"{row[0]} | {row[1]}");
            }
        }
    }

	public async void GetImageFromGoogle(string targetJsonUrl, Action<byte[]> contentCallback)
	{
		byte[] fileBytes = await GetBytesFromGoogle(targetJsonUrl);
		contentCallback(fileBytes);
	}

	public async Task<byte[]> GetBytesFromGoogle(string targetJsonUrl)
	{
		byte[] fileBytes = null;
		try
		{
			var contentString = await CallGoogleScriptAsync(targetJsonUrl);
			if (!string.IsNullOrEmpty(contentString))
			{
				fileBytes = Convert.FromBase64String(contentString);
				DebugTWD.Log($"Image {targetJsonUrl} loaded from Apps Script: {fileBytes.Length}");
			}
			else
			{
				DebugTWD.Log($"Ответ от Apps Script: Image is NULL");
			}
		}
		catch (Exception ex)
		{
			DebugTWD.Log($"Ошибка: {ex.Message}");
		}
		return fileBytes;
	}

	[ContextMenu("Get GED")]
    public async void GetGedFromGoogle(string targetJsonUrl, Action<string> contentCallback)
    {
        //string targetJsonUrl = "https://d2mzp1o3c365dy.cloudfront.net:443/ged-prd/07f6751e55dcfcd72bba1f76079b71ed.gzip";

        string content = await GetJsonFromGoogle(targetJsonUrl);        
		contentCallback(content);
	}

    public async Task<string> GetJsonFromGoogle(string targetJsonUrl)
    {
		string content = null;
		try
		{
			content = await CallGoogleScriptAsync(targetJsonUrl);
			if (!string.IsNullOrEmpty(content))
			{
				DebugTWD.Log($"GED loaded from Apps Script: {content.Length}");
			}
			else
			{
				DebugTWD.Log($"Ответ от Apps Script: GED is NULL");
			}
		}
		catch (Exception ex)
		{
			DebugTWD.Log($"Ошибка: {ex.Message}");
		}
        return content;
	} 

	public async Task<string> CallGoogleScriptAsync(string fileUrl)
    {
		// 1. Формируем данные запроса
		RequestPayload payload = new RequestPayload { fileUrl = fileUrl, action = "fetch_file" };
        string jsonPayload = JsonUtility.ToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        // 2. Настраиваем UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(Settings.WebAppUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Отправка асинхронного запроса в Google Apps Script...");

            // 3. Асинхронно ожидаем завершения запроса
            var operation = request.SendWebRequest();

            // Расширение для поддержки await (работает из коробки в современных версиях Unity)
            while (!operation.isDone)
            {
                await Task.Yield(); // Пропускаем кадр, чтобы не вешать главный поток
            }

            // 4. Проверяем сетевые ошибки
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Сетевая ошибка Unity: {request.error}");
                return null;
            }

            // 5. Обрабатываем успешный ответ
            string jsonResponse = request.downloadHandler.text;
            //Debug.Log($"Сырой ответ от сервера: {jsonResponse}");

            try
            {
                ResponseData response = JsonUtility.FromJson<ResponseData>(jsonResponse);

                if (response.success)
                {
                    return response.data;
                }

                Debug.LogError($"Ошибка внутри Apps Script: {response.error}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка парсинга JSON: {ex.Message}");
                return null;
            }
        }
    }



	// Классы-контейнеры для сериализации в JSON средствами Unity
	[Serializable]
	public class TWDAccountData
	{
		public string player_name;
		public int player_level;
		public string google_id;
		public DateTime last_used;
		public string guild_id;
		public string guild_name;
    }

	[Serializable]
	public class SupabaseRequest
	{
		public string action;
		public string tableName;
		public string rowId; // Передаем сюда наш hashID
		public TWDAccountData updateData;
	}

	[Serializable]
	public class AppsScriptResponse
	{
		public bool success;
		public string data; // Сюда придет либо текст ошибки, либо json-ответ от Supabase
	}

	// Пример вызова метода (можно повесить на кнопку или вызвать при сохранении игры)
	public async void TestUpdate(TWDAccount account)
	{
		Debug.Log("Отправка запроса на обновление...");
		var result = await UpdatePlayerAccount(account);
		Debug.Log($"{result.Status}: {result.Message}");
	}

	public async Task<TaskResult> UpdatePlayerAccount(TWDAccount account)
	{
		// 1. Формируем структуру данных
		var requestPayload = new SupabaseRequest
		{
			action = "update_supabase",
			tableName = account.TableName,
			rowId = account.HashID,
			updateData = new TWDAccountData
			{
				player_name = account.PlayerName,
				player_level = account.PlayerLevel,
				last_used = account.LastUsed,
				google_id = account.GoogleID
			}
		};
		if (!string.IsNullOrEmpty(account.GoogleID)) requestPayload.updateData.google_id = account.GoogleID;
        if (!string.IsNullOrEmpty(account.GuildName)) requestPayload.updateData.guild_name = account.GuildName;
        if (!string.IsNullOrEmpty(account.GuildID)) requestPayload.updateData.guild_id = account.GuildID;

        // 2. Сериализуем объект в JSON строку
        string jsonBody = JsonUtility.ToJson(requestPayload);
		byte[] rawBody = Encoding.UTF8.GetBytes(jsonBody);
		string logMessage = "";
		// 3. Настраиваем UnityWebRequest
		using (UnityWebRequest request = new UnityWebRequest(Settings.WebAppUrl, "POST"))
		{
			request.uploadHandler = new UploadHandlerRaw(rawBody);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			// 4. Отправляем запрос асинхронно
			var operation = request.SendWebRequest();
			
			while (!operation.isDone)
			{
				await Task.Yield(); // Ожидаем завершения без блокировки потока Unity
			}

			// 5. Обрабатываем результат сетевого запроса
			if (request.result != UnityWebRequest.Result.Success)
			{
				logMessage = $"Сетевая ошибка Unity: {request.error}";
				Debug.LogError(logMessage);
				return new TaskResult(TaskResult.TaskStatus.Offline, logMessage);
			}

			// 6. Парсим ответ от вашего Apps Script
			string responseText = request.downloadHandler.text;
			try
			{
				AppsScriptResponse response = JsonUtility.FromJson<AppsScriptResponse>(responseText);

				if (response.success)
				{
					logMessage = "Данные аккаунта успешно обновлены в Supabase!";
					Debug.Log(logMessage);
					return new TaskResult(TaskResult.TaskStatus.Success, logMessage);
				}
				else
				{
					logMessage = $"Apps Script вернул ошибку: {response.data}";
					Debug.LogError(logMessage);
					return new TaskResult(TaskResult.TaskStatus.Error, logMessage);
				}
			}
			catch (Exception ex)
			{
				logMessage = $"Ошибка парсинга ответа: {ex.Message}. Ответ сервера: {responseText}";
				Debug.LogError(logMessage);
				return new TaskResult(TaskResult.TaskStatus.Exception, logMessage);
			}
		}
	}
}