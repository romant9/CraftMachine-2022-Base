using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using System;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Supabase.TWD
{
    public class SignInWithEmailPassword : MonoBehaviour
	{
		// Public Unity References
		public TMP_InputField EmailInput = null;
		public TMP_InputField PasswordInput = null;
		public TMP_Text ErrorText = null!;
		public TMP_Text PlayerName = null!;
		public SupabaseManager SupabaseManager = null;
		public SimpleGoogleSignIn SimpleGoogleSignIn = null;

		[SerializeField]
		private string _display_name = "Bloodymary";

        [SerializeField]
        private string _uid = "6c4e449e-f43d-48d6-86f4-0540aec2581c";
        [SerializeField]
		private string _player_name = "Bloodymary";
        [SerializeField]
        private string _mail = "amelchenkorv@gmail.com";
        [SerializeField]
        private string _hash_id = "997273bf510241e0938af9350b9f0760";
        [SerializeField]
		private string _epic_id = "b796d93e807144d6a6da1e392f245f0d";
        [SerializeField]
        private string _guild_id = "cf91a18093114a83a7adb60034b9bfca";
        [SerializeField]
        private string _guild_name = "BREAKINGϟBAD 3";

        private void SaveToPlayerPrefs()
		{
			if (!string.IsNullOrEmpty(EmailInput.text)) PlayerPrefs.SetString("Player_Mail", EmailInput.text);
			else EmailInput.text = PlayerPrefs.GetString("Player_Mail");
			if (!string.IsNullOrEmpty(PasswordInput.text)) PlayerPrefs.SetString("Player_Pass", PasswordInput.text);
			else PasswordInput.text = PlayerPrefs.GetString("Player_Pass");
		}

		// OnClick SignUp
		public async void SignUp()
		{
			if (SupabaseManager.IsSignedIn)
			{
				ErrorText.text = $"User {SupabaseManager.GetUser().Email} is signedIn yet";
				return;
			}
			if (!SupabaseManager.GetClient().Auth.Online)
			{
				ErrorText.text = $"Ошибка подключения к базе пользователей. Проверьте интернет";
				return;
			}
			try
			{
				SaveToPlayerPrefs();
				Session session = await SupabaseManager.GetClient().Auth.SignUp(EmailInput.text, PasswordInput.text);
				if (session.User != null)
				{
					SupabaseManager.SetUser(session.User);
					if (session.User.UserMetadata.TryGetValue("display_name", out object value))
					{
						PlayerName.text = value.ToString();
						PlayerPrefs.SetString("Player_Name", PlayerName.text);
					}

					ErrorText.text = $"Success! Signed Up as {session.User.Email}";
					SupabaseManager.IsSignedIn = true;
				}
				else
				{
					ErrorText.text = $"Failed! Signed Up. User with {EmailInput.text} does not exist";
				}
			}
			catch (GotrueException goTrueException)
			{
				ErrorText.text = $"{goTrueException.Reason} {goTrueException.Message}";
				Debug.Log(goTrueException.Message, gameObject);
				Debug.LogException(goTrueException, gameObject);
			}
			catch (Exception e)
			{
				Debug.Log(e.Message, gameObject);
				Debug.Log(e, gameObject);
			}
		}

		// OnClick SignIn
		public async void SignIn()
		{
			bool result = await SignInTask();
			if (result)
			{
				Debug.Log("Успешно подключились");
			}
		}

		private async Task<bool> SignInTask()
		{
			if (SupabaseManager.IsSignedIn)
			{
				ErrorText.text = $"User {SupabaseManager.GetUser().Email} is signedIn yet";
				return true;
			}
			if (!SupabaseManager.GetClient().Auth.Online)
			{
				ErrorText.text = $"Ошибка подключения к базе пользователей. Проверьте интернет";
				return false;
			}
			try
			{
				SaveToPlayerPrefs();
				Session session = await SupabaseManager.GetClient().Auth.SignIn(EmailInput.text, PasswordInput.text);
				if (session.User != null)
				{
					if (session.User.UserMetadata.TryGetValue("display_name", out object value))
					{
						PlayerName.text = value.ToString();
						PlayerPrefs.SetString("Player_Name", PlayerName.text);
					}
					SupabaseManager.SetUser(session.User);
					ErrorText.text = $"Success! Signed In as {session.User.Email}";
					SupabaseManager.IsSignedIn = true;
					return true;
				}
				else
				{
					ErrorText.text = $"Failed! Signed In. User with {EmailInput.text} does not exist";
				}
			}
			catch (GotrueException goTrueException)
			{
				ErrorText.text = $"{goTrueException.Reason} {goTrueException.Message}";
				Debug.Log(goTrueException.Message, gameObject);
				Debug.LogException(goTrueException, gameObject);
			}
			catch (Exception e)
			{
				Debug.Log(e.Message, gameObject);
				Debug.Log(e, gameObject);
			}
			return false;
		}

		private CMUser GetDefaultUser()
		{
			return new CMUser
			{
				UID = _uid,
				Email = EmailInput.text,
				UserName = "",
                HashID = _hash_id,
				PinHashID = _hash_id,
				EpicID = _epic_id,
                FirstRun = DateTime.Now.ToLocalTime(),
				LastRun = DateTime.Now.ToLocalTime(),
				TimesRun = 1,
				TimesConnect = 0,
				Regged = false,
				Blocked = false,
				ProGuild = false,
				ProLink = false,
				DeviceInfo = "StellateSapphire|Sniper",
				Country = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
                Wishes = "",
                Feedback = "",
                ClientVersion = "7.15.0.100",
				ModVersion = Application.version,
				RegCode = 0,
                Description = "",
                Content = ""			
			};
		}

		private TWDAccount GetDefaultAccount(CMUser user)
		{
			return new TWDAccount()
			{
				HashID = user.HashID,
				PlayerName = _player_name,
				GuildID = _guild_id,
				GuildName = _guild_name,
				LastUsed = DateTime.Now.ToLocalTime(),
                TimesUsed = 0,
                UID_Linked = user.UID
            };
        }

        private async Task<Postgrest.Responses.ModeledResponse<CMUser>> AddRowUserTask(Supabase.Client client)
		{
			var newPlayer = GetDefaultUser();
			return await client.From<CMUser>().Insert(newPlayer);
		}

        private async Task<Postgrest.Responses.ModeledResponse<TWDAccount>> AddRowAccountTask()
        {
			var cmUser = SupabaseManager.GetCMUser();
            var account = GetDefaultAccount(cmUser);
            return await SupabaseManager.GetClient().From<TWDAccount>().Insert(account);
        }

        private async Task<CMUser> GetPlayerData(string uid)
		{
            var response = await SupabaseManager.GetClient().From<CMUser>().Where(s => s.UID == uid).Get();
			return response.Model;
		}

        private async Task<TWDAccount> GetAccountData(string hashID)
        {
            var response = await SupabaseManager.GetClient().From<TWDAccount>().Where(s => s.HashID == hashID).Get();
            return response.Model;
        }

        // OnClick AddNewPlayer
        public async void AddNewUser()
		{
			bool isSigned = await SignInTask();
			if (!isSigned) return;
			var client = SupabaseManager.GetClient();
			if (client == null) return;
			var result = await AddRowUserTask(client);

            string message;
			if (result.ResponseMessage.IsSuccessStatusCode)
			{
                SupabaseManager.SetCMUser(result.Model);

                message = $"Добавили игрока {_display_name}";
				ErrorText.text = message;
				Debug.Log(message);

				UserAttributes userAttributes = new()
				{
					Data = new System.Collections.Generic.Dictionary<string, object> { { "display_name", _display_name }}
				};

				var response = await client.Auth.Update(userAttributes);
				if (response != null)
				{
					SupabaseManager.SetUser(response);
					message = $"\nSuccess! Update of {response.Email}";
					ErrorText.text += message;
					Debug.Log(message);
				}
			}
			else
			{
				message = $"Ошибка добавления игрока {_display_name}";
				ErrorText.text = message;
				Debug.Log(message);
			}
		}

		// OnClick EditPlayer
		public async void ChangePlayerData()
		{
			if (!SupabaseManager.IsSignedIn) return;
			var client = SupabaseManager.GetClient();
			if (client == null) return;

			var uid = client.Auth.CurrentUser.Id;

            var cmuser = await GetPlayerData(uid);
			if (cmuser == null)
			{
				Debug.LogError("error get user data");
				return;
			}
            SupabaseManager.SetCMUser(cmuser);

			var hashID = cmuser.HashID;
            var account = await GetAccountData(hashID);

            if (account == null)
            {
                Debug.LogError("error get account data");
                return;
            }

            var playerName = account.PlayerName;

			account.UID_Linked = cmuser.UID;
            cmuser.Description = "Changed_" + DateTime.Now.ToLocalTime().ToString("g");
			var updateResultCMUser = await UpdateCMUserData(cmuser);
			var updateResultAccount = await UpdateAccountData(account);
            string message;
			if (updateResultCMUser.ResponseMessage.IsSuccessStatusCode)
			{
				message = $"Изменили данные пользователя {client.Auth.CurrentUser.Email}";
				ErrorText.text = message;
				Debug.Log(message);

				UserAttributes userAttributes = new()
				{
					Data = new System.Collections.Generic.Dictionary<string, object> { { "display_name", playerName } }
				};

				var response = await client.Auth.Update(userAttributes);
				if (response != null)
				{
					message = $"\nSuccess! Update of {client.Auth.CurrentUser.Email}";
					ErrorText.text += message;
					Debug.Log(message);
				}
			}
			else
			{
				message = $"Ошибка добавления игрока {playerName}";
				ErrorText.text = message;
				Debug.Log(message);
			}

            if (updateResultAccount.ResponseMessage.IsSuccessStatusCode)
            {
                message = $"Изменили данные аккаунта {playerName}";
                ErrorText.text = message;
                Debug.Log(message);

                UserAttributes userAttributes = new()
                {
                    Data = new System.Collections.Generic.Dictionary<string, object> { { "display_name", playerName } }
                };

                var response = await client.Auth.Update(userAttributes);
                if (response != null)
                {
                    message = $"\nSuccess! Update of {client.Auth.CurrentUser.Email}";
                    ErrorText.text += message;
                    Debug.Log(message);
                }
            }
            else
            {
                message = $"Ошибка добавления игрока {playerName}";
                ErrorText.text = message;
                Debug.Log(message);
            }
        }

		private async Task<Postgrest.Responses.ModeledResponse<CMUser>> UpdateCMUserData(CMUser cmuser)
		{
			var result = await SupabaseManager.GetClient().From<CMUser>().Where(s => s.UID == cmuser.UID).Update(cmuser);
			return result;
		}

        private async Task<Postgrest.Responses.ModeledResponse<TWDAccount>> UpdateAccountData(TWDAccount account)
        {
			var user = SupabaseManager.GetCMUser();
            var result = await SupabaseManager.GetClient().From<TWDAccount>().Where(s => s.HashID == user.HashID).Update(account);
            return result;
        }

        // OnClick GetPlayer By Hash
        public async void GetPlayerByHash()
		{
			bool isSigned = await SignInTask();
			if (!isSigned) return;
			var client = SupabaseManager.GetClient();
			if (client == null) return;
			var player = await GetPlayerData(_uid);
			string message;
			if (player != null)
			{
				message = $"Получили данные игрока {player.HashID}";
				ErrorText.text = message;
				Debug.Log(message);
			}
			else
			{
				message = $"Ошибка получения данных игрока для {client.Auth.CurrentUser.Email}";
				ErrorText.text = message;
				Debug.Log(message);
			}
		}

		// OnClick GetPlayer By Mail
		public async void GetPlayerByMail()
		{
			bool isSigned = await SignInTask();
			if (!isSigned) return;
			var client = SupabaseManager.GetClient();
			if (client == null) return;
			string message;

			var player = await GetPlayerData(client.Auth.CurrentUser.Id);
			if (player != null)
			{
				var hashID = player.HashID;
				var account = await GetAccountData(hashID);
				message = $"Получили данные игрока {account.PlayerName}";
				ErrorText.text = message;
				Debug.Log(message);
			}
			else
			{
				message = $"Ошибка получения данных игрока для {client.Auth.CurrentUser.Email}";
				ErrorText.text = message;
				Debug.Log(message);
			}
		}

		// OnClick SignOut
		public async void SignOut()
		{
			await SignOutTask();
		}

		private async Task SignOutTask()
		{
			if (SupabaseManager.IsSignedIn)
			{
				var signOutResult = SupabaseManager.GetClient().Auth.SignOut();
				await signOutResult;
			}
			SupabaseManager.IsSignedIn = false;
			SupabaseManager.SetUser(null);
		}

		// OnClick SignIn With Google
		public async void SignUpGoogle()
		{
			if (SupabaseManager.IsSignedIn)
			{
				ErrorText.text = $"User {SupabaseManager.GetUser().Email} is signedIn yet";
				return;
			}
			if (!SupabaseManager.GetClient().Auth.Online)
			{
				ErrorText.text = $"Ошибка подключения к базе пользователей. Проверьте интернет";
				return;
			}
			try
			{
                Session session = await SimpleGoogleSignIn.SignInWithGoogleTask();
				if (session != null)
				{
					if (session.User.UserMetadata.TryGetValue("display_name", out object value))
					{
						PlayerName.text = value.ToString();
						PlayerPrefs.SetString("Player_Name", PlayerName.text);
					}
					SupabaseManager.SetUser(session.User);
					ErrorText.text = $"Success! Signed In Google as {session.User.Email}";
					SupabaseManager.IsSignedIn = true;
				}
				else
				{
					ErrorText.text = $"Failed! Signed In. User with {EmailInput.text} does not exist";
				}
			}
			catch (GotrueException goTrueException)
			{
				ErrorText.text = $"{goTrueException.Reason} {goTrueException.Message}";
				Debug.Log(goTrueException.Message, gameObject);
				Debug.LogException(goTrueException, gameObject);
			}
			catch (Exception e)
			{
				Debug.Log(e.Message, gameObject);
				Debug.Log(e, gameObject);
			}
		}

		// OnClick SetPlayerMetadata
		public async void SetPlayerMetadata()
		{
			bool isSigned = await SignInTask();
			if (!isSigned) return;
			try
			{
				UserAttributes userAttributes = new()
				{
					Data = new System.Collections.Generic.Dictionary<string, object> { { "display_name", _player_name }}
				};

				var response = await SupabaseManager.GetClient().Auth.Update(userAttributes);
				if (response != null)
				{
					ErrorText.text = $"Success! Update of {SupabaseManager.GetClient().Auth.CurrentUser}";
				}
			}
			catch (GotrueException goTrueException)
			{
				ErrorText.text = $"{goTrueException.Reason} {goTrueException.Message}";
				Debug.Log(goTrueException.Message, gameObject);
				Debug.LogException(goTrueException, gameObject);
			}
			catch (Exception e)
			{
				Debug.Log(e.Message, gameObject);
				Debug.Log(e, gameObject);
			}
		}

		public void GetSavedRegData()
		{
			if (PlayerPrefs.HasKey("Player_Mail")) EmailInput.text = PlayerPrefs.GetString("Player_Mail");
			if (PlayerPrefs.HasKey("Player_Pass")) PasswordInput.text = PlayerPrefs.GetString("Player_Pass");
			if (PlayerPrefs.HasKey("Player_Name")) PlayerName.text = PlayerPrefs.GetString("Player_Name");
		}
	}
}
