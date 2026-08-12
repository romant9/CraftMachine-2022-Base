using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using UnityEngine.Networking;

namespace Supabase.TWD
{
	public class DatabaseManager
	{
		public Client SupaClient { get; private set; }

		public CMUser CurrentCMUser { get; set; }
		public TWDAccount CurrentTWDAccount { get; set; }

		private DateTime lastRunStart;

		private PlayerModel playerModel => GameManager.Instance?.playerModel ?? null;

		public bool IsInited { get; set; }

		private Postgrest.QueryOptions queryOptions = new Postgrest.QueryOptions()
		{
			DuplicateResolution = Postgrest.QueryOptions.DuplicateResolutionType.MergeDuplicates
		};

		public DatabaseManager()
		{
			SupaClient = SupabaseManager.Instance.GetClient();
			PlayersIDDataList = new();
		}

		public List<PlayersIDData> PlayersIDDataList { get; set; }

		private string GetSavedAccountName(string mail)
		{
			var savedAccountName = UserPrefsKeys.UserAccountName;
			return !string.IsNullOrEmpty(savedAccountName) ? savedAccountName : (!string.IsNullOrEmpty(mail) ? mail.Split('@')[0] : "NoName");
		}

		public async Task<bool> UpdateReged(bool regged)
		{
			CurrentCMUser.Regged = regged;

			try
			{
				var result = await CurrentCMUser.Update<CMUser>();
				if (result.ResponseMessage.IsSuccessStatusCode)
				{
					DebugTWD.Log($"Статус \"Regged\" успешно изменен на {regged}", DebugType.Supabase);
				}
				else
				{
					DebugTWD.Log($"Ошибка изменения статуса \"Regged\" на {regged}", DebugType.Supabase);
				}
				return result.ResponseMessage.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
				return false;
			}
		}

		public async Task<bool> UpdateLastRun()
		{
			bool isAvailable = await GetCMUser();

			var uid = SupaClient.Auth.CurrentUser.Id;
			var mail = SupaClient.Auth.CurrentUser.Email;
			var userName = SupaClient.Auth.CurrentUser.UserMetadata.TryGetValue("full_name", out object name) ? name.ToString() : GetSavedAccountName(mail);

			lastRunStart = DateTime.Now;

			Postgrest.Responses.ModeledResponse<CMUser> cmUserResult;
			if (!isAvailable)
			{
				var cmUser = new CMUser
				{
					UID = uid,
					Email = mail,
					UserName = userName,
					LastRun = DateTime.UtcNow,
					DeviceInfo = UserPrefsKeys.UserDeviceName,
					ClientVersion = OfflineManager.ClientVersion,
					ModVersion = Application.version,
					TimesRun = 1,
					TrialCount = DataManager.Instance.TrialModeDays,
					RegCode = UserPrefsKeys.GeneratedCode(uid)
				};
				try
				{
					cmUserResult = await SupaClient.From<CMUser>().Insert(cmUser, queryOptions);
				}
				catch(Exception ex)
				{
					DebugTWD.LogError(ex, DebugType.Supabase);
					return false;
				}
			}
			else
			{
				CurrentCMUser.Email = mail;
				CurrentCMUser.UserName = userName;
				CurrentCMUser.LastRun = DateTime.Now;
				CurrentCMUser.DeviceInfo = UserPrefsKeys.UserDeviceName;
				CurrentCMUser.ClientVersion = OfflineManager.ClientVersion;
				CurrentCMUser.ModVersion = Application.version;

				CurrentCMUser.TimesRun++;
				CurrentCMUser.SessionDuration = 0;

				try
				{
					cmUserResult = await CurrentCMUser.Update<CMUser>();
				}
				catch (Exception ex)
				{
					DebugTWD.LogError(ex, DebugType.Supabase);
					return await UpdateCMUserFix(CurrentCMUser);
				}
			}

			if (cmUserResult.ResponseMessage.IsSuccessStatusCode)
			{
				CurrentCMUser = cmUserResult.Model;
				DebugTWD.Log($"Данные {userName} успешно обновлены в cm_users", DebugType.Supabase);
			}
			else
			{
				DebugTWD.LogError($"Ошибка записи данных {userName} в cm_users, код: {cmUserResult.ResponseMessage.StatusCode}", DebugType.Supabase);
			}

			return cmUserResult.ResponseMessage.IsSuccessStatusCode;
		}

		public async Task<bool> UpdateCMUserFix(CMUser userToUpdate)
		{
			var settings = SupabaseManager.Instance.SupabaseSettings;

			// Формируем URL с фильтрацией по UID
			string url = $"{settings.SupabaseURL}/rest/v1/cm_users?uid=eq.{SupaClient.Auth.CurrentUser.Id}";

			// Сериализуем модель в JSON string
			var cmUserSer = new CMUserSerialized(userToUpdate);
			var settingsJson = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
			string jsonBody = JsonConvert.SerializeObject(cmUserSer, settingsJson);
			byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

			// Создаем пустой запрос и вручную настраиваем PATCH метод
			using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
			{
				request.uploadHandler = new UploadHandlerRaw(bodyRaw);
				request.downloadHandler = new DownloadHandlerBuffer();

				// Устанавливаем заголовки Supabase
				request.SetRequestHeader("Content-Type", "application/json");
				request.SetRequestHeader("apikey", settings.SupabaseAnonKey);
				request.SetRequestHeader("Authorization", $"Bearer {SupaClient.Auth.CurrentSession.AccessToken}");
				// Способ принудительно вернуть ОДИН объект вместо массива через Accept
				request.SetRequestHeader("Accept", "application/vnd.pgrst.object+json");
				// Просим сервер вернуть представление объекта
				request.SetRequestHeader("Prefer", "return=representation");
				try
				{
					// Асинхронно ожидаем завершения сетевого запроса
					var operation = request.SendWebRequest();
					while (!operation.isDone)
					{
						await Task.Yield(); // Пропускаем кадр, чтобы не вешать основной поток Unity
					}

					if (request.result == UnityWebRequest.Result.Success)
					{
						// Десериализуем ответ сервера, чтобы обновить локальные данные актуальным состоянием
						string jsonResult = request.downloadHandler.text;
						if (!string.IsNullOrEmpty(jsonResult))
						{
							cmUserSer = JsonConvert.DeserializeObject<CMUserSerialized>(jsonResult);
							CurrentCMUser = new CMUser(cmUserSer);
						}

						DebugTWD.Log($"Данные {userToUpdate.UserName} успешно обновлены в cm_users", DebugType.Supabase);
						return true;
					}
					else
					{
						Debug.LogError($"Ошибка обновления {userToUpdate.UserName} в cm_users: {request.error} | {request.downloadHandler.text}");
						return false;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError($"Исключение при отправке PATCH запроса в cm_users: {ex.Message}");
					return false;
				}
			}
		}

		public async Task<bool> GetCMUser()
		{
			try
			{
				CurrentCMUser = await SupaClient.From<CMUser>().Where(x => x.UID == SupaClient.Auth.CurrentUser.Id).Single();
			}
			catch (Exception ex)
			{
				DebugTWD.LogError(ex);
				CurrentCMUser = await GetCMUserFix();
			}
			return CurrentCMUser != null;
		}

		public async Task<CMUser> GetCMUserFix()
		{
			CMUser cmUser = null;
			var settings = SupabaseManager.Instance.SupabaseSettings;
			string url = $"{settings.SupabaseURL}/rest/v1/cm_users?uid=eq.{SupaClient.Auth.CurrentUser.Id}&select=*";

			using (UnityWebRequest request = UnityWebRequest.Get(url))
			{
				// Добавляем обязательные заголовки Supabase
				request.SetRequestHeader("apikey", settings.SupabaseAnonKey);
				request.SetRequestHeader("Authorization", $"Bearer {SupaClient.Auth.CurrentSession.AccessToken}");
				request.SetRequestHeader("Accept", "application/vnd.pgrst.object+json"); // Возвращает один объект вместо массива

				var operation = request.SendWebRequest();
				while (!operation.isDone)
				{
					await Task.Yield();
				}

				if (request.result == UnityWebRequest.Result.Success)
				{
					string jsonResult = request.downloadHandler.text;
					var cmUserSer = JsonConvert.DeserializeObject<CMUserSerialized>(jsonResult);
					cmUser = new CMUser(cmUserSer);
					Debug.Log("Данные успешно загружены!");
				}
				else
				{
					Debug.LogError($"Ошибка запроса: {request.error} | {request.downloadHandler.text}");
				}
			}
			return cmUser;
		}

		public async Task<bool> GetTWDAccount()
		{
			if (string.IsNullOrEmpty(CurrentCMUser.HashID))
			{
				DebugTWD.Log("hashID is null. Данные аккаунта невозможно идентифицировать", DebugType.Supabase);
				return false;
			}
			try
			{
				CurrentTWDAccount = await SupaClient.From<TWDAccount>().Where(x => x.HashID == CurrentCMUser.HashID).Single();
			}
			catch (Exception ex)
			{
				DebugTWD.LogError(ex);
			}
			return CurrentTWDAccount != null;
		}

		public async void UpdateCMUser()
		{
			if (SupaClient == null)
			{
				//await GetCMUser();
				//if (CurrentCMUser == null) return;
				DebugTWD.Log("SupaClient is null. Данные аккаунта невозможно идентифицировать", DebugType.Supabase);
				return;
			}

			if (CurrentCMUser == null)
			{
				DebugTWD.Log("CurrentCMUser is null. Данные аккаунта невозможно идентифицировать", DebugType.Supabase);
				return;
			}

			//CurrentCMUser.UserName = SupaClient.Auth.CurrentUser.UserMetadata.TryGetValue("full_name", out object name) ? name.ToString() : GetSavedAccountName(CurrentCMUser.Email);
			//CurrentCMUser.LastRun = DateTime.UtcNow.ToLocalTime();
			CurrentCMUser.HashID = playerModel != null ? playerModel.HashedId : UserPrefsKeys.Player_HashID;
			CurrentCMUser.PinHashID = UserPrefsKeys.Player_Pin_HashID;
			CurrentCMUser.EpicID = UserPrefsKeys.Player_EpicAccountID;
			if (DataManager.Instance.contentSource != ContentSource.Local) CurrentCMUser.TimesConnect += 1;
			//CurrentCMUser.DeviceInfo = UserPrefsKeys.UserDeviceName;
			CurrentCMUser.Country = playerModel != null ? playerModel.Country : CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
			//CurrentCMUser.Wishes = DataManager.Instance.UserWishes;
			//CurrentCMUser.ClientVersion = OfflineManager.ClientVersion;
			//CurrentCMUser.ModVersion = Application.version;
			//CurrentCMUser.RegCode = UserPrefsKeys.GeneratedCode(CurrentCMUser.UID);

			try
			{
				var result = await CurrentCMUser.Update<CMUser>();
				if (result.ResponseMessage.IsSuccessStatusCode)
				{
					DebugTWD.Log($"Данные {CurrentCMUser.UserName} успешно обновлены в cm_users");
					UpdateTWDAccount();
				}
				else
				{
					DebugTWD.LogError($"Ошибка записи данных {CurrentCMUser.UserName} в cm_users, код: {result.ResponseMessage.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				DebugTWD.LogError($"Ошибка записи данных {CurrentCMUser.UserName} в cm_users\n{ex.Message}");
			}
		}

		public async void UpdateTWDAccount()
		{
			if (SupaClient == null)
			{
				DebugTWD.Log("SupaClient is null. Данные аккаунта невозможно идентифицировать", DebugType.Supabase);
				return;
			}

			if (CurrentCMUser == null)
			{
				DebugTWD.Log("CurrentCMUser is null. Данные аккаунта невозможно идентифицировать", DebugType.Supabase);
				return;
			}

			var hashID = playerModel != null ? playerModel.HashedId : UserPrefsKeys.Player_HashID;
			var playerName = playerModel != null ? playerModel.Name : UserPrefsKeys.Player_Name;
			var playerLevel = playerModel != null ? playerModel.Level : UserPrefsKeys.Player_Level;
			var googleID = UserPrefsKeys.Player_GoogleID;
			var uid_Linked = SupaClient.Auth.CurrentUser.Id; //c6bf1a39-1ca6-4904-8a25-4cc3163234e3

			if (string.IsNullOrEmpty(hashID) || string.IsNullOrEmpty(playerName))
			{
				DebugTWD.Log("hashID is null. Данные аккаунта невозможно идентифицировать", DebugType.Supabase);
				return;
			}

			bool isAccAvailable = await GetTWDAccount();

			Postgrest.Responses.ModeledResponse<TWDAccount> result = null;
			if (!isAccAvailable)
			{
				try
				{
					var twdAccount = new TWDAccount
					{
						//HashID = hashID, // Заглушка, сервер сотрет её и запишет хэш из CM_Users //083d2c1c9981467a9276429558161d24
						PlayerName = playerName, //арт
						PlayerLevel = playerLevel, //103
						LastUsed = DateTime.UtcNow // Изменение этого поля перехватит триггер и увеличит times_used
						//UID_Linked = uid_Linked, // Изменение этого поля перехватит триггер
					};
					if (!string.IsNullOrEmpty(googleID)) twdAccount.GoogleID = googleID; //G02-D05-6f0bb18b-afee-4fcd-a6c5-f019f4b9bda8
					result = await SupaClient.From<TWDAccount>().Upsert(twdAccount, queryOptions);
				}
				catch (Exception ex)
				{
					DebugTWD.LogError(ex, DebugType.Supabase);
					return;
				}
			}
			else
			{
				if (DataManager.Instance.contentSource == ContentSource.Local)
				{
					return;
				}

				//var accountToUpdate = new TWDAccount
				//{
				//	HashID = CurrentTWDAccount.HashID,
				//	PlayerName = playerName,
				//	PlayerLevel = playerLevel,
				//	LastUsed = DateTime.UtcNow
				//};
				//if (!string.IsNullOrEmpty(googleID)) accountToUpdate.GoogleID = googleID;

				//CurrentTWDAccount.HashID = hashID;
				CurrentTWDAccount.PlayerName = playerName;
				CurrentTWDAccount.PlayerLevel = playerLevel;
				if (!string.IsNullOrEmpty(googleID)) CurrentTWDAccount.GoogleID = googleID;
				CurrentTWDAccount.LastUsed = DateTime.UtcNow;
				//CurrentTWDAccount.UID_Linked = uid_Linked;

				try
				{
					//result = await SupaClient.From<TWDAccount>().Where(x => x.HashID == accountToUpdate.HashID).Update(accountToUpdate);
					//result = await SupaClient.From<TWDAccount>().Upsert(CurrentTWDAccount);
					if (SupabaseManager.Instance.IsGoogleBotRequest)
					{
						var result_Google = await DataManager.Instance.GoogleSheetManager.UpdatePlayerAccount(CurrentTWDAccount);
						return;
					}
					else
					{
						result = await CurrentTWDAccount.Update<TWDAccount>();
					}
				}
				catch (Exception ex)
				{
					DebugTWD.LogError(ex, DebugType.Supabase);
				}
			}

			if (result != null && result.ResponseMessage.IsSuccessStatusCode)
			{
				DebugTWD.Log($"Данные аккаунта {playerName} успешно обновлены в twd_accounts", DebugType.Supabase);
				CurrentTWDAccount = result.Model;
			}
			else
			{
				DebugTWD.Log($"Ошибка записи данных {playerName} в twd_accounts", DebugType.Supabase);
			}
		}

		public async void UpdateTWDAccountGuilds()
		{
			if (CurrentTWDAccount == null) return;

			try
			{
				CurrentTWDAccount.GuildID = playerModel != null ? playerModel.GuildId : UserPrefsKeys.Player_GuildID;
				CurrentTWDAccount.GuildName = playerModel != null ? playerModel.GuildName : UserPrefsKeys.Player_GuildName;

                if (SupabaseManager.Instance.IsGoogleBotRequest)
                {
                    var result_Google = await DataManager.Instance.GoogleSheetManager.UpdatePlayerAccount(CurrentTWDAccount);
                    return;
                }
                else
                {
                    var result = await CurrentTWDAccount.Update<TWDAccount>();

                    if (result.ResponseMessage.IsSuccessStatusCode)
                    {
                        Debug.Log($"Данные гильдий {CurrentTWDAccount.PlayerName} успешно обнавлены в twd_accounts");
                    }
                    else
                    {
                        Debug.Log($"Ошибка записи данных {CurrentTWDAccount.PlayerName} в twd_accounts, код: {result.ResponseMessage.StatusCode}");
                    }
                } 
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
		}

		public async Task<List<PlayersIDData>> GetIDListAsync()
		{
			if (PlayersIDDataList.Count > 0) return PlayersIDDataList;

			PlayersIDDataList = new ();

			string[] columns = new string[4] { "player_level", "player_name", "hash_id", "google_id" };

			var accountResponse = await SupaClient.From<TWDAccount>().Select(x => new object[] { x.PlayerName, x.PlayerLevel, x.HashID, x.GoogleID }).Get().ConfigureAwait(false);
			//Select("player_name, hash_id, google_id")

			if (accountResponse == null || !accountResponse.ResponseMessage.IsSuccessStatusCode)
			{
				Debug.LogError("error");
				return PlayersIDDataList;
			}
			var accounts = accountResponse.Models;
			if (accounts.Count > 0)
			{
				foreach (var item in accounts)
				{
					PlayersIDDataList.Add(new PlayersIDData(item.PlayerName, item.HashID, item.GoogleID, item.PlayerLevel));
				}
				Debug.Log($"Успешно создан лист игроков: {PlayersIDDataList.Count}");
			}
			return PlayersIDDataList;
		}

		public async Task OnApplicationQuit()
		{
			if (CurrentCMUser != null)
			{
				CurrentCMUser.SessionDuration = (long)(DateTime.Now - lastRunStart).TotalSeconds;
				CurrentCMUser.LastRun = DateTime.Now;
				CurrentCMUser.PinHashID = UserPrefsKeys.Player_Pin_HashID;
				await CurrentCMUser.Update<CMUser>().ConfigureAwait(false);
			}
		}
	}
}
