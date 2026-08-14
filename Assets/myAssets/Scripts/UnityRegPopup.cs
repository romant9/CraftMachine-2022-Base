using BestHTTP.Authentication;
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
	private GameObject activeWindow;

	public GameObject RegWindow;
	public GameObject SignInWindow;
	public GameObject ConfirmWindow;

	public LocalizationUIUpdater regcodeUpdater;
	public LocalizationUIUpdater headerUpdater;
	public LocalizationUIUpdater nextWindowUpdater;
	public LocalizationUIUpdater confirmationUpdater;

	private bool IsPassHide;

	public UIInput RegcodeInput;
	public UIInput EmailInput;
	public UIInput PasswordInput;

	public UIButton SetLocalButton;
	public UIButton ReloadSupaClientButton;
	public UITable ExtraButtonContainer;


	[SerializeField] private UnityAuthManager authManager;

	[SerializeField] private UIButton linkGoogleButton;
	[SerializeField] private UIButton unlinkGoogleButton;


	public enum RegState
	{
		Sign,
		Reg,
		Confirm
	}

	public RegState CurrentState = RegState.Sign;

	void Start()
	{
		authManager = UnityAuthManager.Instance;
		linkGoogleButton.onClick.Add(new EventDelegate(OnLinkGoogleClicked));
		unlinkGoogleButton.onClick.Add(new EventDelegate(OnUnlinkGoogleClicked));

		// новое
		loginButton.onClick.AddListener(OnLoginSubmit);
		switchToRegisterButton.onClick.AddListener(() => ToggleTabs(showRegister: true));

		registerButton.onClick.AddListener(OnRegisterSubmit);
		switchToLoginButton.onClick.AddListener(() => ToggleTabs(showRegister: false));

		googleLoginButton.onClick.AddListener(OnGoogleLoginClick);
		closeErrorButton.onClick.AddListener(() => errorWindow.SetActive(false));

		ToggleTabs(showRegister: false);
		errorWindow.SetActive(false);
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

		linkGoogleButton.isEnabled = false; // Блокируем кнопку на время запроса

		string googleIdToken = await authManager.GetGoogleTokenSilentAsync();

		if (string.IsNullOrEmpty(googleIdToken))
		{
			ShowError("Не удалось получить данные от Google. Попробуйте еще раз.");
			linkGoogleButton.isEnabled = true;
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
			linkGoogleButton.isEnabled = true;
		}
	}

	/// <summary>
	/// Логика кнопки "Отвязать Google"
	/// </summary>
	private async void OnUnlinkGoogleClicked()
	{
		unlinkGoogleButton.isEnabled = false;

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
			unlinkGoogleButton.isEnabled = true;
		}
	}

	private void UpdateButtonsState(bool isLinked)
	{
		// Если привязан: кнопку "Привязать" выключаем, кнопку "Отвязать" включаем
		linkGoogleButton.isEnabled = !isLinked;
		unlinkGoogleButton.isEnabled = isLinked;
	}

	public void ShowError(string message)
	{
		SetState(RegState.Confirm, new string[1] { message });
		linkGoogleButton.isEnabled = true; // Разблокируем кнопку, чтобы игрок мог попробовать снова

		errorText.text = message;
		errorWindow.SetActive(true);
	}

	public void ShowSuccess(string message)
	{
		SetState(RegState.Confirm, new string[1] { message });
	}
	//

	// новое
	[Header("Поля ввода (Вход)")]
	[SerializeField] private InputField loginEmailInput; // Изменено на Email
	[SerializeField] private InputField loginPasswordInput;
	[SerializeField] private Button loginButton;
	[SerializeField] private Button switchToRegisterButton;

	[Header("Поля ввода (Регистрация)")]
	[SerializeField] private InputField regEmailInput; // Изменено на Email
	[SerializeField] private InputField regPasswordInput;
	[SerializeField] private Button registerButton;
	[SerializeField] private Button switchToLoginButton;

	[Header("Кнопки социальных сетей")]
	[SerializeField] private Button googleLoginButton;

	[Header("Окна Уведомлений")]
	[SerializeField] private GameObject errorWindow;
	[SerializeField] private Text errorText;
	[SerializeField] private Button closeErrorButton;

	[Header("Вкладки UI (для переключения)")]
	[SerializeField] private GameObject loginTabGameObject;
	[SerializeField] private GameObject registerTabGameObject;

	private void ToggleTabs(bool showRegister)
	{
		loginTabGameObject.SetActive(!showRegister);
		registerTabGameObject.SetActive(showRegister);
	}

	// ==========================================
	// 1. СЦЕНАРИЙ: ВХОД (SIGN IN)
	// ==========================================
	private async void OnLoginSubmit()
	{
		string email = loginEmailInput.text.Trim();
		string password = loginPasswordInput.text;

		if (!ValidateInputs(email, password)) return;

		SetButtonsInteractable(false);

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
	private async void OnRegisterSubmit()
	{
		string email = regEmailInput.text.Trim();
		string password = regPasswordInput.text;

		if (!ValidateInputs(email, password)) return;

		SetButtonsInteractable(false);

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
			SetButtonsInteractable(true);
		}
	}

	// ==========================================
	// 3. СЦЕНАРИЙ: ВХОД ЧЕРЕЗ GOOGLE
	// ==========================================
	private async void OnGoogleLoginClick()
	{
		SetButtonsInteractable(false);
		string googleIdToken = await authManager.GetGoogleTokenAsync();

		if (string.IsNullOrEmpty(googleIdToken))
		{
			ShowError("Не удалось авторизоваться в Google.");
			SetButtonsInteractable(true);
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
			SetButtonsInteractable(true);
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

	private void SetButtonsInteractable(bool state)
	{
		loginButton.interactable = state;
		registerButton.interactable = state;
		googleLoginButton.interactable = state;
	}

	private void OnAuthProcessComplete()
	{
		gameObject.SetActive(false);
		Debug.Log("[UI] Авторизация завершена. Окно закрыто.");
	}
	//




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

		var result = await authManager.SignInWithPasswordAsync(EmailInput.value, PasswordInput.value);

		SetState(RegState.Confirm, new string[1] { result.Message});
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
