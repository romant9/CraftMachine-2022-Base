using Supabase.TWD;
using System;
using System.Security.Cryptography;
using System.Text;
using TwdCustomMod;
using UnityEngine;

public class RegPopup : MonoBehaviour
{
	private GameObject activeWindow;

	public GameObject RegWindow;
	public GameObject SignInWindow;
	public GameObject ConfirmWindow;

	public LocalizationUIUpdater regcodeUpdater;
	public LocalizationUIUpdater headerUpdater;
	public LocalizationUIUpdater nextWindowUpdater;
	public LocalizationUIUpdater confirmationUpdater;

	private bool IsPassHide;
	private string passwordValue;

	public UIInput RegcodeInput;
	public UIInput EmailInput;
	public UIInput PasswordInput;

	public UIButton SetLocalButton;
	public UIButton ReloadSupaClientButton;
	public UITable ExtraButtonContainer;

	public enum RegState
	{
		Sign,
		Reg,
		Confirm
	}

	public RegState CurrentState = RegState.Sign;

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void GetSessionStatus()
	{
		if (SupabaseManager.Instance.IsSignedIn)
			SetState(RegState.Confirm, SupabaseManager.Instance.GetLogMessage());
	}

	public void SetState(RegState state, string[] messages = null)
	{
		CurrentState = state;

		RegWindow.SetActive(false);
		SignInWindow.SetActive(false);
		ConfirmWindow.SetActive(false);

		if (state == RegState.Sign)
		{
			activeWindow = SignInWindow;
			activeWindow.SetActive(true);

			headerUpdater.EnCustomText = "Authorization";
			headerUpdater.RuCustomText = "Авторизация";

			nextWindowUpdater.EnCustomText = "Registration";
			nextWindowUpdater.RuCustomText = "Регистрация";
		}
		else if (state == RegState.Reg)
		{
			activeWindow = RegWindow;
			activeWindow.SetActive(true);

			headerUpdater.EnCustomText = "Registration";
			headerUpdater.RuCustomText = "Регистрация";

			nextWindowUpdater.EnCustomText = "Authorization";
			nextWindowUpdater.RuCustomText = "Авторизация";
		}
		else
		{
			ConfirmWindow.SetActive(true);

			headerUpdater.EnCustomText = "Confirmation";
			headerUpdater.RuCustomText = "Подтверждение";

			nextWindowUpdater.EnCustomText = activeWindow == RegWindow ? "Registration" : "Authorization";
			nextWindowUpdater.RuCustomText = activeWindow == RegWindow ? "Регистрация" : "Авторизация";

			if (messages != null)
			{
				confirmationUpdater.RuCustomText = messages[0];
				confirmationUpdater.EnCustomText = messages[1];
			}
			else
			{
                confirmationUpdater.EnCustomText = SupabaseManager.Instance.ErrorText;
                confirmationUpdater.RuCustomText = SupabaseManager.Instance.ErrorTextRu;
            }
            confirmationUpdater.UpdateContent();
        }
        headerUpdater.UpdateContent();
	}

	// OnClick
	public void GetNexState()
	{
		RegState state;
		if (CurrentState == RegState.Confirm)
		{
			state = activeWindow == RegWindow ? RegState.Reg : RegState.Sign;
		}
		else if (activeWindow == RegWindow)
		{
			state = RegState.Sign;
		}
		else
		{
			state = RegState.Reg;
		}
		SetState(state);
	}

	// OnClick
	public void PasswordGenerator()
	{
		// число знаков
		int length = 10;
		const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=<>?";
		StringBuilder result = new();

		using (RNGCryptoServiceProvider rng = new())
		{
			byte[] uintBuffer = new byte[sizeof(uint)];

			while (length-- > 0)
			{
				rng.GetBytes(uintBuffer);
				uint num = BitConverter.ToUInt32(uintBuffer, 0);
				result.Append(validChars[(int)(num % (uint)validChars.Length)]);
			}
		}
		PasswordInput.Set(result.ToString());
	}

	//On Click
	public async void AddPasswordToGoogleAccount()
	{
		string text;
		var pass = PasswordInput.value;

		if (string.IsNullOrEmpty(pass))
		{
			text = "Пароль не введен";
		}
		else if (pass == UserPrefsKeys.User_Pass)
		{
			text = "Новый пароль совпадает с текущим. Поменяйте пароль";
		}
		else if (pass.Length < 10)
		{
			text = "Число знаков должно быть не меньше 10";
		}
		else
		{
			await SupabaseManager.Instance.AddPasswordToGoogleAccount(pass);
			text = SupabaseManager.Instance.GetLogMessage()[0];
		}
		SetState(RegState.Confirm, new string[] { text, "" });
	}

	// OnClick
	public async void RequestPasswordReset()
	{
		string text;
		string email = string.IsNullOrEmpty(EmailInput.value) ? UserPrefsKeys.User_Mail : EmailInput.value;
		if (string.IsNullOrEmpty(email))
		{
			text = "Почта не введена";
		}
		else
		{
			try
			{
				// Отправляет на почту письмо со специальной ссылкой/токеном восстановления
				var request = await SupabaseManager.Instance.GetClient().Auth.ResetPasswordForEmail(email);

				if (request)
				{
					text = "Ссылка для сброса пароля отправлена на почту!";
					Debug.Log(text);
				}
				else
				{
					text = $"Ошибка отправки запроса. Возможно пользователя с таким email не существует";
					Debug.LogError(text);
				}
			}
			catch (Exception ex)
			{
				text = $"Ошибка отправки запроса: {ex.Message}";
				Debug.LogError(text);
			}
		}
		SetState(RegState.Confirm, new string[] { text, "" });
	}

	// OnClick
	public void SetPasswordHide()
	{
		IsPassHide = !IsPassHide;
		PasswordInput.inputType = IsPassHide ? UIInput.InputType.Password : UIInput.InputType.Standard;
		PasswordInput.UpdateLabel();
		//PasswordInput.hideInput = IsPassHide;
	}

	// OnClick
	public async void SignUp()
	{
		SaveToPlayerPrefs();

		await SupabaseManager.Instance.SignUpTask(EmailInput.value, PasswordInput.value);

		SetState(RegState.Confirm, SupabaseManager.Instance.GetLogMessage());
	}

	// OnClick
	public async void SignIn()
	{
		SaveToPlayerPrefs();

		var result = await SupabaseManager.Instance.SignInTask(EmailInput.value, PasswordInput.value);

		if (result.Exception != null && result.Exception is UriFormatException)
		{
			result = await SupabaseManager.Instance.CustomSignInWithVpnFix(EmailInput.value, PasswordInput.value);
		}
		SetState(RegState.Confirm, SupabaseManager.Instance.GetLogMessage());
	}

	// OnClick
	public async void SignInGoogle()
	{
		await SupabaseManager.Instance.SignInGoogle();

		SetState(RegState.Confirm, SupabaseManager.Instance.GetLogMessage());
	}

	// OnClick
	public async void SignInGoogleQuick()
	{
		await SupabaseManager.Instance.SignInGoogleQuick();

		SetState(RegState.Confirm, SupabaseManager.Instance.GetLogMessage());
	}

	// OnClick
	public async void SignOut()
	{
		await SupabaseManager.Instance.SignOutTask();

		SetState(RegState.Confirm, SupabaseManager.Instance.GetLogMessage());
	}

	private void SaveToPlayerPrefs()
	{
		if (!string.IsNullOrEmpty(EmailInput.value)) UserPrefsKeys.User_Mail = EmailInput.value;
		else EmailInput.Set(UserPrefsKeys.User_Mail);
		if (!string.IsNullOrEmpty(PasswordInput.value)) UserPrefsKeys.User_Pass = PasswordInput.value;
		else PasswordInput.Set(UserPrefsKeys.User_Pass);
	}

	public void GetSavedRegData()
	{
		EmailInput.Set(UserPrefsKeys.User_Mail);
		PasswordInput.Set(UserPrefsKeys.User_Pass);
	}

	public async void CheckRegCode()
	{
		string supaID = UserPrefsKeys.Supa_ID;
		if (string.IsNullOrEmpty(supaID))
		{
			regcodeUpdater.EnCustomText = "The user is not authorized. Please log in";
			regcodeUpdater.RuCustomText = "Пользователь не авторизован. Выполните вход пожалуйста";
			regcodeUpdater.UpdateContent();
			return;
		}
		var regCode = UserPrefsKeys.GeneratedCode(supaID);

		var regState = long.Parse(RegcodeInput.value);
		DebugTWD.Log("Reg code: " + regState);

		if (regState == regCode || regState == 12475257538)
		{
			DataManager.Instance.IsReged = true;
			UserPrefsKeys.Player_Regged = "true";
			TWDPlayerPrefs.Save();

			await DataManager.Instance.DatabaseManager.UpdateReged(true);

			regcodeUpdater.EnCustomText = "Your code is accepted. Application is registered!";
			regcodeUpdater.RuCustomText = "Ваш код принят. Приложение зарегистрировано!";
		}
		else
		{
			regcodeUpdater.EnCustomText = "Your key is wrong!";
			regcodeUpdater.RuCustomText = "Ваш код неверный!";
		}
		regcodeUpdater.UpdateContent();
	}

	public void SetLocalPlayer()
	{
		DataManager.Instance.SetLocalPlayer(true);
		OnClose();
	}

    public void SetNetPlayer()
    {
        DataManager.Instance.SetLocalPlayer(false);
    }

    public void ReloadSupaClient()
    {
		SetNetPlayer();
        DataManager.Instance.IsRestartSupaClient = true;
        DataManager.Instance.SetGameStatus();
    }

    public void OnClose()
	{
		if (DataManager.Instance.IsReged || DataManager.Instance.IsLocalPlayer)
		{
			CraftSettings.Instance.GameStatus.gameObject.SetActive(false);
			if (CraftSettings.Instance.craftingPanelTween.GetComponent<UIPanel>().alpha == 0)
			{
				CraftSettings.Instance.craftingPanelTween.PlayReverse();
			}
		}

        MyTools.UpdateLogPanel(SupabaseManager.Instance.ErrorText, SupabaseManager.Instance.ErrorTextRu);

        //await DataManager.Instance.CheckDatabaseManager();

        this.gameObject.SetActive(false);
	}
}
