using Supabase.TWD;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TwdCustomMod;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityAuth;
using UnityEngine;
using Unity.Services.CloudSave;
using System.Threading.Tasks;

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
	[SerializeField] private UILabel LinkGoogle_Status_Label;
	public UIButton SignOut_Bt;

	[Header("Поля ResetPassword")]
	public UIButton SendCode_Btn;
	public UIButton SubmitReset_Bt;
	public UIButton Reset_GeneratePass_Bt;
	public UIInput Reset_EmailInput;
	public UIInput Reset_CodeInput;
	public UIInput Reset_PassInput;

	[Header("Поля Status")]
    public UIButton SetLocalSession_Bt;
    public UIButton CopyData_Bt;
	public UILabel StatusMessage_Label;
	private string statusMessageEn;
    private string statusMessageRu;

	private UnityAuthManager authManager;
	private int lastSelectedTabIndex;


	public enum ToggleState
	{
		SignUp = 0,
		SignIn = 1,
		Actions = 2,
		Restore = 3,
		Status = 4
	}

	void Start()
	{
		authManager = UnityAuthManager.Instance;

        SignUp_Bt.onClick.Add(new EventDelegate(OnSignUp_Submit));
		SignUp_Google_Bt.onClick.Add(new EventDelegate(SignInWithGoogle));
        GeneratePass_Bt.onClick.Add(new EventDelegate(() => PasswordGenerator(ToggleState.SignUp)));
        GoTo_SignIn_Bt.onClick.Add(new EventDelegate(() => ForceToggleTabs(ToggleState.SignIn)));

		Fill_Bt.onClick.Add(new EventDelegate(() => GetSavedRegData(ToggleState.SignIn)));
		ForgotPass_Bt.onClick.Add(new EventDelegate(GoToPasswordReset));
        SignIn_Bt.onClick.Add(new EventDelegate(OnSignIn_Submit));
        SignIn_Google_Bt.onClick.Add(new EventDelegate(() => OnGoogleLoginClick(ToggleState.SignIn)));
        GoTo_SignUp_Bt.onClick.Add(new EventDelegate(() => ForceToggleTabs(ToggleState.SignUp)));

		SendCode_Btn.onClick.Add(new EventDelegate(OnSendCodeClicked));
		SubmitReset_Bt.onClick.Add(new EventDelegate(OnSubmitResetClicked));
		Reset_GeneratePass_Bt.onClick.Add(new EventDelegate(() => PasswordGenerator(ToggleState.Restore)));

		Actions_GeneratePass_Bt.onClick.Add(new EventDelegate(() => PasswordGenerator(ToggleState.Actions)));
		ChangePass_Bt.onClick.Add(new EventDelegate(OnChangePasswordClicked));
        LinkGoogle_Bt.onClick.Add(new EventDelegate(OnLinkClicked));
		UnlinkGoogle_Bt.onClick.Add(new EventDelegate(OnUnlinkGoogleClicked));
		SignOut_Bt.onClick.Add(new EventDelegate(SignOut));

		ForceToggleTabs(ToggleState.SignIn);
	}

	private void OnEnable()
	{
		AuthenticationService.Instance.SignedIn += SignedIn_Handle;
		toggleSet.OnChangeDelegate += OnTabChanged;
		SignUp_HidePass_BtTg.OnClickToggleEvent += SetPasswordHide;
		SignIn_HidePass_BtTg.OnClickToggleEvent += SetPasswordHide;
		Actions_HidePass_BtTg.OnClickToggleEvent += SetPasswordHide;
	}

	private void OnDisable()
	{
		AuthenticationService.Instance.SignedIn -= SignedIn_Handle;
		toggleSet.OnChangeDelegate -= OnTabChanged;
		SignUp_HidePass_BtTg.OnClickToggleEvent -= SetPasswordHide;
		SignIn_HidePass_BtTg.OnClickToggleEvent -= SetPasswordHide;
		Actions_HidePass_BtTg.OnClickToggleEvent -= SetPasswordHide;
	}

	private void ForceToggleTabs(ToggleState state)
	{
		var index = (int)state;
		toggleSet.SetSelectedIndex(index);
	}

	private void OnTabChanged(UIButtonExtended toggle)
	{
		var index = toggleSet.GetSelectedIndex();
		PlayUITabAnimations(index);
		if ((ToggleState)index == ToggleState.Actions)
		{
			OnLinkCheck();
		}
		lastSelectedTabIndex = index;
	}

	private void SignedIn_Handle()
	{
		bool isLocalPlayer = AuthenticationService.Instance.IsSignedIn;
		bool isChanged = isLocalPlayer != DataManager.Instance.IsLocalPlayer;
		
		DataManager.Instance.SetLocalPlayer(!isLocalPlayer);
		if (isChanged)
		{
			DataManager.Instance.SetGameStatus();
		}
		DebugTWD.Log("SetLocalPlayer to " + !isLocalPlayer);
	}

	/// <summary>
	/// Логика кнопки "Привязать Google"
	/// </summary>
	private async void OnLinkCheck()
	{
		string message;
		if (!AuthenticationService.Instance.IsSignedIn)
		{
			message = "Пользователь не вошел по логину/паролю";
			LinkGoogle_Bt.isEnabled = false;     
		}
		else if (authManager.CheckIfGoogleIsLinked())
		{
			message = "Вы можете привязать вход почта/пароль для текущего аккаунта Google";
			LinkGoogle_Bt.isEnabled = true;
		}
		else
		{
			message = "Вы можете привязать вход через Google для текущего аккаунта почта/пароль";
			LinkGoogle_Bt.isEnabled = true;
		}
		LinkGoogle_Status_Label.text = message;
	}

	/// <summary>
	/// Логика кнопки "Привязать Google или почта/пароль"
	/// </summary>
	private async void OnLinkClicked()
	{
		if (!AuthenticationService.Instance.IsSignedIn) 
		{
			ShowError("Пользователь не вошел по логину/паролю");
			return;
		}

		LinkGoogle_Bt.isEnabled = false; // Блокируем кнопку на время запроса

		if (authManager.CheckIfGoogleIsLinked())
		{
			string pass = string.IsNullOrEmpty(Actions_PasswordInput.value) ? UserPrefsKeys.User_Pass : Actions_PasswordInput.value;

			if (string.IsNullOrEmpty(pass))
			{
				ShowError("Пароль не задан!");
				return;
			}

			try
			{
				await authManager.LinkUsernamePasswordAsync(pass);

				// Если метод выполнился без исключений — привязка прошла успешно!
				ShowSuccess("[Link] Пароль успешно добавлен к вашей учетной записи Google");
				UpdateButtonsState(isLinked: true);
			}
			catch (System.Exception)
			{
				// Обработка ошибок происходит внутри LinkGoogleAccountAsync, 
				// которая вызовет OnLinkFailed, но на всякий случай разблокируем кнопку.
				LinkGoogle_Bt.isEnabled = true;
			}
		}
		else
		{
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

		string email = SignUp_EmailInput.value.Trim();
		string password = SignUp_PasswordInput.value;

		if (!ValidateInputs(email, password)) return;

		SaveToPlayerPrefs(ToggleState.SignUp);

		SetButtonsInteractable(ToggleState.SignUp, false);

		try
		{
			// Генерируем валидный Username (16 символов) из длинной почты
			string shortUsername = GenerateUsernameFromEmail(email);

			// Регистрируем в Unity под коротким уникальным именем
			await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(shortUsername, password);

			// 3. Сохраняем РЕАЛЬНЫЙ Email в Cloud Save игрока, чтобы бэкенд мог его прочитать
			var emailData = new Dictionary<string, object> { { "USER_EMAIL", email } };
			await CloudSaveService.Instance.Data.Player.SaveAsync(emailData);

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

	public void SignInWithGoogle()
	{
		authManager.SignInWithGoogle();
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
		else if (state == ToggleState.Restore)
		{
			Reset_PassInput.Set(result.ToString());
		}
	}

	//On Click - Замена пароля
	public async void OnChangePasswordClicked()
	{
		// требования: от 8 до 30 символов, минимум одна строчная, одна заглавная буквы, цифра и спецсимвол)
		string errorText = "";
		var passOld = UserPrefsKeys.User_Pass;
		var passNew = Actions_PasswordInput.value;
		if (string.IsNullOrEmpty(passNew))
		{
			errorText = "Старый пароль не сохранен. Выполните вход под старым паролем";
		}
		else if (string.IsNullOrEmpty(passNew))
		{
			errorText = "Пароль не введен";
		}
		else if (passNew == passOld)
		{
			errorText = "Новый пароль совпадает с текущим. Поменяйте пароль";
		}
		else if (passNew.Length < 8)
		{
			errorText = "Число знаков должно быть не меньше 8";
		}
		else
		{
			ChangePass_Bt.isEnabled = false;

			try
			{
				await authManager.ChangePasswordAsync(passOld, passNew);
			}
			catch (Exception ex) 
			{
				errorText = ex.Message;
			}

			Actions_PasswordInput.Set("");
			ChangePass_Bt.isEnabled = false;
		}
		if (!string.IsNullOrEmpty(errorText))
		{
			MyTools.OpenAlert(errorText);
		}
	}

	#region Restore Password
	private async void OnSendCodeClicked()
	{
		if (string.IsNullOrEmpty(Reset_EmailInput.value))
		{
			Reset_EmailInput.Set(UserPrefsKeys.User_Mail);
		}
		string email = Reset_EmailInput.value.Trim();

		// Простая проверка на клиенте, чтобы не дергать сеть зря
		if (string.IsNullOrEmpty(email) || !email.Contains("@"))
		{
			HandleError("Пожалуйста, введите корректный формат Email.");
			return;
		}

		SendCode_Btn.isEnabled = false;
		var result = await authManager.RequestPasswordResetAsync(email);

		if (result != null && result.success)
		{
			// Если вы тестируете в Unity Editor, то можете подсмотреть сгенерированный PIN 
			// прямо на экране, не заходя в логи Dashboard!
			if (!string.IsNullOrEmpty(result.debugPin))
			{
				Debug.Log($"[DEBUG] Тестовый PIN-код из облака: {result.debugPin}");
			}
		}
		else
		{
			ShowError("Не удалось отправить запрос. Проверьте сеть или корректность Email.");
		}
		SendCode_Btn.isEnabled = true;
	}

	private async void OnSubmitResetClicked()
	{
		string email = Reset_EmailInput.value.Trim();
		string pin = Reset_CodeInput.value.Trim();
		string newPassword = Reset_PassInput.value;

		if (string.IsNullOrEmpty(pin) || pin.Length < 4)
		{
			HandleError("Код восстановления должен содержать не менее 4 символов.");
			return;
		}

		if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
		{
			HandleError("Новый пароль должен содержать не менее 8 символов.");
			return;
		}

		SubmitReset_Bt.isEnabled = false;

		SaveToPlayerPrefs(ToggleState.Restore);

		bool isResetSuccessful = await authManager.ConfirmPasswordResetAsync(email, pin, newPassword);

		if (isResetSuccessful)
		{
			Debug.Log("[UI] Процесс сброса завершен. Окно закрыто.");
		}
		else
		{
			ShowError("Неверный код восстановления, либо срок его действия (15 мин) истек.");
		}

		SubmitReset_Bt.isEnabled = true;
	}

	private void HandleError(string message)
	{
		MyTools.OpenAlert(message);

		// Возвращаем кнопкам кликабельность для повторной попытки
		SendCode_Btn.isEnabled = true;
		SubmitReset_Bt.isEnabled = true;
	}
	#endregion

	// OnClick
	public void GoToPasswordReset()
	{
		ForceToggleTabs(ToggleState.Restore);
	}

	// OnClick
	public void SetPasswordHide(bool isToggled)
	{
		IsPassHide = isToggled;

		var inputType = IsPassHide ? UIInput.InputType.Password : UIInput.InputType.Standard;
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
		else if (state == ToggleState.Restore)
		{
			if (!string.IsNullOrEmpty(Reset_EmailInput.value)) UserPrefsKeys.User_Mail = Reset_EmailInput.value;
			if (!string.IsNullOrEmpty(Reset_PassInput.value)) UserPrefsKeys.User_Pass = Reset_PassInput.value;
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

	private void PlayUITabAnimations(int index)
	{
		var toggles = toggleSet.GetUIButtonToggleList;
		for (int i = 0; i < toggles.Length; i++)
		{
			TweenerPlayer component = toggles[i].GetComponent<TweenerPlayer>();
			var label = component.transform.GetChild(0).GetComponent<UILabel>();
			if (i == index)
			{
				//раскрыть
				Helpers.GameObjectSetActive(tabContainers[i], true);
				label.overflowMethod = UILabel.Overflow.ShrinkContent;
				component.PlayGroup(11, false);
			}
			else if (i == lastSelectedTabIndex)
			{
				Helpers.GameObjectSetActive(tabContainers[i], false);
				label.overflowMethod = UILabel.Overflow.ClampContent;
				component.PlayGroup(10, false);
			}
		}
	}

	private string GenerateUsernameFromEmail(string email)
	{
		using (MD5 md5 = MD5.Create())
		{
			byte[] inputBytes = Encoding.UTF8.GetBytes(email.ToLower().Trim());
			byte[] hashBytes = md5.ComputeHash(inputBytes);

			StringBuilder sb = new StringBuilder();
			// Берем только первые 8 байт (16 символов в HEX), чтобы гарантированно уложиться в лимит 3-20 символов
			for (int i = 0; i < 8; i++)
			{
				sb.Append(hashBytes[i].ToString("x2"));
			}
			return sb.ToString(); // Возвращает уникальную строку, например "a1b2c3d4e5f6g7h8"
		}
	}
}
