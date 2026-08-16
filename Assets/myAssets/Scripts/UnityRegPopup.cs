using Postgrest;
using Supabase.TWD;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TwdCustomMod;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityAuth;
using UnityEngine;
using UnityEngine.UI;

public class UnityRegPopup : MonoBehaviour
{
    public UIButtonToggleSet toggleSet;

    public GameObject[] tabContainers;

	private bool IsPassHide;

    [Header("Поля SignUp")]
    public UIButton SignUp_Google_Bt;
    public UIInput SignUp_EmailInput;
	public UIInput SignUp_PasswordInput;
	public UIButton GeneratePass_Bt;
    public UIButton SignUp_Bt;
    public UIButtonToggle SignUp_HidePass_BtTg;
    public UIButton GoTo_SignIn_Bt;

    [Header("Поля SignIn")]
    public UIButton SignIn_Google_Bt;
    public UIInput SignIn_EmailInput;
    public UIInput SignIn_PasswordInput;
    public UIButtonToggle SignIn_HidePass_BtTg;
    public UIButton Fill_Bt;
    public UIButton ForgotPass_Bt;
    public UIButton SignIn_Bt;
    public UIButton GoTo_SignUp_Bt;

    [Header("Поля Actions")]
    public UIInput Actions_PasswordInput;
    public UIButtonToggle Actions_HidePass_BtTg;
    public UIButton Actions_GeneratePass_Bt;
    public UIButton ChangePass_Bt;
    [SerializeField] private UIButton LinkGoogle_Bt;
    [SerializeField] private UIButton UnlinkGoogle_Bt;
	public UIButton SignOut_Bt;

    [Header("Поля Status")]
    public UIButton SetLocalSession_Bt;
    public UIButton CopyData_Bt;
	public UILabel StatusMessage_Label;
	private string statusMessageEn;
    private string statusMessageRu;

    private UnityAuthManager authManager;


	public enum ToggleState
	{
		SignUp = 0,
		SignIn = 1,
		Actions = 2,
		Status = 3
	}

	public ToggleState CurrentState = ToggleState.SignUp;

	void Start()
	{
		authManager = UnityAuthManager.Instance;

        SignUp_Bt.onClick.Add(new EventDelegate(OnSignUp_Submit));
        SignUp_Google_Bt.onClick.Add(new EventDelegate(() => OnGoogleLoginClick(ToggleState.SignUp)));
        GeneratePass_Bt.onClick.Add(new EventDelegate(() => PasswordGenerator(ToggleState.SignUp)));
        GoTo_SignIn_Bt.onClick.Add(new EventDelegate(() => ToggleTabs(ToggleState.SignIn)));

		Fill_Bt.onClick.Add(new EventDelegate(() => GetSavedRegData(ToggleState.SignIn)));
		ForgotPass_Bt.onClick.Add(new EventDelegate(RequestPasswordReset));
        SignIn_Bt.onClick.Add(new EventDelegate(OnSignIn_Submit));
        SignIn_Google_Bt.onClick.Add(new EventDelegate(() => OnGoogleLoginClick(ToggleState.SignIn)));
        GoTo_SignUp_Bt.onClick.Add(new EventDelegate(() => ToggleTabs(ToggleState.SignUp)));

        Actions_GeneratePass_Bt.onClick.Add(new EventDelegate(() => PasswordGenerator(ToggleState.Actions)));
		ChangePass_Bt.onClick.Add(new EventDelegate(AddPasswordToGoogleAccount));
        LinkGoogle_Bt.onClick.Add(new EventDelegate(OnLinkGoogleClicked));
		UnlinkGoogle_Bt.onClick.Add(new EventDelegate(OnUnlinkGoogleClicked));
		SignOut_Bt.onClick.Add(new EventDelegate(SignOut));

        ToggleTabs(ToggleState.SignIn);
	}

	/// <summary>
	/// Логика кнопки "Привязать Google"
	/// </summary>
	private async void OnLinkGoogleClicked()
	{
		if (!AuthenticationService.Instance.IsSignedIn) 
		{
			ShowError("Пользователь не вошел по логину/паролю");
			return;
		}

		if (authManager.CheckIfGoogleIsLinked())
		{
			ShowError("Google уже подключен!");
			return;
		}

		LinkGoogle_Bt.isEnabled = false; // Блокируем кнопку на время запроса

		string googleIdToken = await authManager.GetGoogleTokenSilentAsync();

		if (string.IsNullOrEmpty(googleIdToken))
		{
			ShowError("Не удалось получить данные от Google. Попробуйте еще раз.");
			LinkGoogle_Bt.isEnabled = true;
			return;
		}

		try
		{
			await authManager.LinkGoogleAccountAsync(googleIdToken);

			// Если метод выполнился без исключений — привязка прошла успешно!
			ShowSuccess("Google-аккаунт успешно привязан! Теперь вы можете использовать его для входа.");
			UpdateButtonsState(isLinked: true);
		}
		catch (System.Exception)
		{
			// Обработка ошибок происходит внутри LinkGoogleAccountAsync, 
			// которая вызовет OnLinkFailed, но на всякий случай разблокируем кнопку.
			LinkGoogle_Bt.isEnabled = true;
		}
	}

	/// <summary>
	/// Логика кнопки "Отвязать Google"
	/// </summary>
	private async void OnUnlinkGoogleClicked()
	{
		UnlinkGoogle_Bt.isEnabled = false;

		if (!authManager.CheckIfGoogleIsLinked())
		{
			ShowError("Этот аккаунт не связан с Google!");
			return;
		}

		try
		{
			await authManager.UnlinkProviderAsync("google");

			// Если метод выполнился без исключений — привязка прошла успешно!
			ShowSuccess("Google-аккаунт отключен от вашего игрового профиля.");
			UpdateButtonsState(isLinked: false);
		}
		catch (System.Exception)
		{
			// Обработка ошибок происходит внутри LinkGoogleAccountAsync, 
			// которая вызовет OnLinkFailed, но на всякий случай разблокируем кнопку.
			UnlinkGoogle_Bt.isEnabled = true;
		}
	}

	private void UpdateButtonsState(bool isLinked)
	{
		// Если привязан: кнопку "Привязать" выключаем, кнопку "Отвязать" включаем
		LinkGoogle_Bt.isEnabled = !isLinked;
		UnlinkGoogle_Bt.isEnabled = isLinked;
	}

	public void ShowError(string message)
	{
		LinkGoogle_Bt.isEnabled = true; // Разблокируем кнопку, чтобы игрок мог попробовать снова

		MyTools.OpenAlert(message);
        statusMessageEn = message;
	}

	public void ShowSuccess(string message)
	{
		statusMessageEn = message;
	}
	//

	private void ToggleTabs(ToggleState state)
	{
        toggleSet.SetSelectedIndex((int)state);
    }

    // ==========================================
    // 1. СЦЕНАРИЙ: ВХОД (SIGN IN)
    // ==========================================
    private async void OnSignIn_Submit()
	{
		GetSavedRegData(ToggleState.SignIn);

        string email = SignIn_EmailInput.value.Trim();
        string password = SignIn_PasswordInput.value;

		if (!ValidateInputs(email, password)) return;

		SaveToPlayerPrefs(ToggleState.SignIn);

        SetButtonsInteractable(ToggleState.SignIn, false);

		try
		{
			// Передаем строку email в качестве параметра username
			await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(email, password);
			Debug.Log($"[UI] Вход по почте успешен! ID: {AuthenticationService.Instance.PlayerId}");
			OnAuthProcessComplete();
		}
		catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.InvalidParameters)
		{
			ShowError("Пароль или Email не соответствуют правилам безопасности Unity.");
		}
		// 2. Затем ловим родительский класс (ошибки сервера, неверный пароль, нет сети)
		catch (RequestFailedException ex)
		{
			// Сюда же попадут и другие AuthenticationException, если они не подошли под условие выше
			Debug.LogWarning($"[UI] Ошибка сервера при входе: {ex.Message} (Код: {ex.ErrorCode})");
			ShowError("Неверный Email или пароль. Пожалуйста, проверьте данные.");
		}
		catch (Exception ex)
		{
			ShowError($"Непредвиденная ошибка: {ex.Message}");
		}
	}

	// ==========================================
	// 2. СЦЕНАРИЙ: РЕГИСТРАЦИЯ (SIGN UP)
	// ==========================================
	private async void OnSignUp_Submit()
	{
		GetSavedRegData(ToggleState.SignUp);

		string email = SignIn_EmailInput.value.Trim();
		string password = SignIn_PasswordInput.value;

		if (!ValidateInputs(email, password)) return;

		SaveToPlayerPrefs(ToggleState.SignUp);

		SetButtonsInteractable(ToggleState.SignUp, false);

		try
		{
			// Регистрируем, используя почту как уникальное имя пользователя
			await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(email, password);
			Debug.Log($"[UI] Регистрация по почте успешна! ID: {AuthenticationService.Instance.PlayerId}");
			OnAuthProcessComplete();
		}
		catch (AuthenticationException ex)
		{
			// Перехватываем локальные ошибки самого SDK (например, неверный формат)
			Debug.LogError($"[UI] Ошибка SDK при регистрации: {ex.Message} (Код: {ex.ErrorCode})");
			ShowError("Ошибка параметров регистрации. Проверьте длину пароля.");
		}
		catch (RequestFailedException ex)
		{
			// Перехватываем ошибку от сервера. Если имя занято или сервер отклонил запрос:
			Debug.LogWarning($"[UI] Сервер отклонил регистрацию: {ex.Message} (Код: {ex.ErrorCode})");

			// Универсальное и безопасное сообщение для игрока
			ShowError("Не удалось зарегистрироваться. Возможно, этот Email уже занят или введены некорректные данные.");
		}	
		catch (Exception ex)
		{
			// Общие системные исключения
			ShowError($"Непредвиденная ошибка: {ex.Message}");
		}
		finally
		{
			SetButtonsInteractable(ToggleState.SignUp, true);
		}
	}

	// ==========================================
	// 3. СЦЕНАРИЙ: ВХОД ЧЕРЕЗ GOOGLE
	// ==========================================
	private async void OnGoogleLoginClick(ToggleState state)
	{
		SetButtonsInteractable(state, false);
		string googleIdToken = await authManager.GetGoogleTokenAsync();

		if (string.IsNullOrEmpty(googleIdToken))
		{
			ShowError("Не удалось авторизоваться в Google.");
			SetButtonsInteractable(state, true);
			return;
		}

		try
		{
			await AuthenticationService.Instance.SignInWithGoogleAsync(googleIdToken);
			OnAuthProcessComplete();
		}
		catch (AuthenticationException ex)
		{
			ShowError($"Ошибка Google авторизации: {ex.Message}");
		}
		finally
		{
			SetButtonsInteractable(state, true);
		}
	}

	// ==========================================
	// ВАЛИДАЦИЯ И ПРОВЕРКА ПОЧТЫ (EMAIL)
	// ==========================================

	private bool ValidateInputs(string email, string password)
	{
		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			ShowError("Поля Email и пароля не должны быть пустыми.");
			return false;
		}

		// Вызов метода валидации почты через регулярное выражение
		if (!IsValidEmail(email))
		{
			ShowError("Введен некорректный формат Email (пример: name@example.com).");
			return false;
		}

		if (password.Length < 8)
		{
			ShowError("Пароль должен быть не менее 8 символов.");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Проверка строки на соответствие стандарту электронной почты
	/// </summary>
	private bool IsValidEmail(string email)
	{
		// Стандартное регулярное выражение для проверки структуры email
		string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

		return Regex.IsMatch(email, emailPattern);
	}

	private void SetButtonsInteractable(ToggleState tgState, bool state)
	{
		if (tgState == ToggleState.SignUp)
		{
            SignUp_Bt.isEnabled = state;
            SignUp_Google_Bt.isEnabled = state;
        }
		else if (tgState == ToggleState.SignIn)
        {
            SignIn_Bt.isEnabled = state;
            SignIn_Google_Bt.isEnabled = state;
        }
    }

	private void OnAuthProcessComplete()
	{
		gameObject.SetActive(false);
		Debug.Log("[UI] Авторизация завершена. Окно закрыто.");
	}
	//

	// OnClick
	public void PasswordGenerator(ToggleState state)
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

		if (state == ToggleState.SignUp)
		{
			SignUp_PasswordInput.Set(result.ToString());
        }
		else if (state == ToggleState.Actions)
		{
			Actions_PasswordInput.Set(result.ToString());
		}
	}

	//On Click
	public async void AddPasswordToGoogleAccount()
	{
		string text;
		var pass = Actions_PasswordInput.value;

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
	}

	// OnClick
	public async void RequestPasswordReset()
	{
		string text;
		string email = string.IsNullOrEmpty(SignIn_EmailInput.value) ? UserPrefsKeys.User_Mail : SignIn_EmailInput.value;
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
	}

	// OnClick
	public void SetPasswordHide(UIButtonToggle btg)
	{
		IsPassHide = btg.IsToggled;

		var inputType = IsPassHide ? UIInput.InputType.Password : UIInput.InputType.Standard; ;
		SignUp_PasswordInput.inputType = inputType;
        SignUp_PasswordInput.UpdateLabel();

        SignIn_PasswordInput.inputType = inputType;
        SignIn_PasswordInput.UpdateLabel();

        Actions_PasswordInput.inputType = inputType;
        Actions_PasswordInput.UpdateLabel();
	}

	// OnClick
	public async void SignUp()
	{
		GetSavedRegData(ToggleState.SignUp);

		var email = SignUp_EmailInput.value;
		var pass = SignUp_PasswordInput.value;

		await SupabaseManager.Instance.SignUpTask(email, pass);

		SaveToPlayerPrefs(ToggleState.SignUp);
	}

	// OnClick
	public async void SignIn()
	{
        GetSavedRegData(ToggleState.SignIn);

        var email = SignUp_EmailInput.value;
        var pass = SignUp_PasswordInput.value;

        var result = await authManager.SignInWithPasswordAsync(email, pass);

        SaveToPlayerPrefs(ToggleState.SignUp);
    }

	// OnClick
	public async void SignOut()
	{
		await SupabaseManager.Instance.SignOutTask();
	}

	private void SaveToPlayerPrefs(ToggleState state)
	{
		if (state == ToggleState.SignUp)
		{
            if (!string.IsNullOrEmpty(SignUp_EmailInput.value)) UserPrefsKeys.User_Mail = SignUp_EmailInput.value;
            if (!string.IsNullOrEmpty(SignUp_PasswordInput.value)) UserPrefsKeys.User_Pass = SignUp_PasswordInput.value;
        }
        else if (state == ToggleState.SignIn) 
		{
            if (!string.IsNullOrEmpty(SignIn_EmailInput.value)) UserPrefsKeys.User_Mail = SignIn_EmailInput.value;
            if (!string.IsNullOrEmpty(SignIn_PasswordInput.value)) UserPrefsKeys.User_Pass = SignIn_PasswordInput.value;
        }
		else if (state == ToggleState.Actions)
		{
            if (!string.IsNullOrEmpty(Actions_PasswordInput.value)) UserPrefsKeys.User_Pass = Actions_PasswordInput.value;
        }
	}

	public void GetSavedRegData(ToggleState state)
	{
        if (state == ToggleState.SignUp)
        {
			if (string.IsNullOrEmpty(SignUp_EmailInput.value)) SignUp_EmailInput.Set(UserPrefsKeys.User_Mail);
            if (string.IsNullOrEmpty(SignUp_PasswordInput.value)) SignUp_PasswordInput.Set(UserPrefsKeys.User_Pass);
        }
        else if (state == ToggleState.SignIn)
        {
            if (string.IsNullOrEmpty(SignIn_EmailInput.value)) SignIn_EmailInput.Set(UserPrefsKeys.User_Mail);
            if (string.IsNullOrEmpty(SignIn_PasswordInput.value)) SignIn_PasswordInput.Set(UserPrefsKeys.User_Pass);
        }
        else if (state == ToggleState.Actions)
        {
            if (string.IsNullOrEmpty(Actions_PasswordInput.value)) Actions_PasswordInput.Set(UserPrefsKeys.User_Pass);
        }
	}

	private void OpenAlert()
	{

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
