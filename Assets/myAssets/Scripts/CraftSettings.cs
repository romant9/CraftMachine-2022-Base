using Client.Connectivity;
using NUnit.Framework;
using Supabase.TWD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TWDModel;
using UnityEngine;

namespace TwdCustomMod
{
	public class CraftSettings : MonoBehaviour
	{
		public static CraftSettings Instance;

		public ResidenceCraftBadgeTab badgeTab;

		public UIScrollBar scaleSlider;
		public UILabel scaleLabel;

		public UILabel scaleLabelUI;

		private float scaleValue;

		public PopupListData recipeData;
		public PopupListData rarityData;

		private List<CurrencyType> components;

		public float Scale { get { return scaleValue; } }

		public bool IsRealPlayerData { get; set; }

		//число компонентов каждого класса
		public int CurrencyCountMax { get; set; }
		private int CurrencyCountMaxPrev { get; set; }

		public List<CurrencyModel> Currency { get; set; }

		public int ResidenceLevel { get; set; }

		public int MaxBadgeCounts { get; set; }

		//для варианта без загрузки данных игрока или ручного переопределения State
		public int CustomInitialState { get; set; } //custom random для значков

		private List<GameObject> languages;
		[SerializeField]
		private UIScrollView languageScrollView;
		[SerializeField]
		private GameObject languagePrefab;
		[SerializeField]
		private UITable languageTable;
		[SerializeField]
		private UILabel currentLanguage;
		[SerializeField]
		private GameObject languageScrollContainer;

		[SerializeField]
		private UILabel EpicText;
		[SerializeField]
		private UILabel LinkDeviceText;
		[SerializeField]
		private GameObject EpicNotConnectedObject;
		//имя профиля
		public UILabel CraftsManLabel;
		//сохраненное имя
		public UILabel Pin_CraftsManLabel;
		private bool IsPlayerNameLableHide;
		//trial / regged / blocked
		public UILabel GameStatus;

		[SerializeField]
		private UIToggle IsRealPlayerToggle;
		public UIInput CurrencyCountMaxInput;

		public UIButton EpicButton;
		public UILabel EpicBtText;

		public GameObject ContentUrlObject;
		public GameObject ContentSheetObject;
		public RegPopup RegPopup;
        public UnityRegPopup UnityRegPopup;

        public GameObject GameVersionObject;
		public GameObject EpicObject;

		public GameObject WishObject;
		public GameObject FeedbackObject;

		public UIInput InitialStateInput;
		public UIInput ClientVersionInput;
		public UIInput ClientDataUrlInput;
		public UIInput ContentFileIDInput;
		public UIInput ContentSheetIDInput;

		public UIInput WishInput;
		public UIInput FeedbackInput;
		public GameObject FeedbackIndicator;
		public TweenColor FeedbackGlow;

		public UITable SettingLeftPanel;

		public TweenScale EpicSwitchTween;
		public TweenScale EpicConnectTween;
		public TweenScale InitStateTween;
		public TweenScale LogSheetTween;
		public TweenScale GameVersionTween;
		public TweenScale DataUrlTween;

		public TweenAlpha craftingPanelTween;

		public UILabel versionLabel;

		public UIRoot root;
		public UIRoot topCamera;

		private string prevLangKey { get; set; }
		public int currentHeight { get; private set; }
		//public bool IsGoogleSheetConnected { get; set; }

		public Texture2D donateTex;

		public UIPopupList serverDataPopupList;

		public GameObject tooltipPrefab;

		public UIButton LinkButton;

		public UIButton MissionHubButton;

		public UIButtonToggle pinToggle;

		public GameObject UIRootTransform => root.gameObject;
		public GameObject UITopCameraTransform => topCamera.gameObject;

		public EquipmentButton equipmentCardPrefab;

		public GameObject ToolTipTraitLarge;
		public GameObject ToolTipTokenLarge;

		public TweenPosition TeamSelectionTween { get; set; }
		public Vector3 TeamSelectionTweenFromPos { get; set; }

		public UITable CurrencyMeterContainer;
		public HUDMeter GoldMeter;
		public HUDMeter RadioMeter;
		public HUDMeter TraitRerollMeter;
		public HUDMeter GoldRadioMeter;
		public HUDMeter CustomCurrencyMeter;
		private List<HUDMeter> currencyMeters;

		// DebugUI значок сверху в контейнере CurrencyMeterContainer
		public ShowDebugPopup showDebugPopup;

		public UIToggle ResetEpicLoginToggle;

		private bool isInternetOn;
		private bool isSupaOnline;
		private bool isLocal;

		public void Awake()
		{
			if (Instance != null)
			{
				DebugTWD.LogError("Multiple CraftSettings!");
				return;
			}
			Instance = this;
			languages = new List<GameObject>();
		}

		private void Start()
		{
			currencyMeters = new List<HUDMeter>() { GoldMeter, RadioMeter, TraitRerollMeter, GoldRadioMeter, CustomCurrencyMeter };

			ShowMeters(null);
		}

		public void SetMetersCurrency()
		{
			if (GameManager.Instance.playerModel == null) return;

			GoldMeter.SetCurrencyType(CurrencyType.Diamonds);
			RadioMeter.SetCurrencyType(CurrencyType.Phone);
			TraitRerollMeter.SetCurrencyType(CurrencyType.TraitRerollToken);
			GoldRadioMeter.SetCurrencyType(CurrencyType.GoldRadio);
		}

		public void ShowMeters(int[] meterIndexes)
		{
			var player = GameManager.Instance.playerModel;

			foreach (var meter in currencyMeters)
			{
				if (!meter) return;

				bool isShow = meterIndexes != null && meterIndexes.Contains(currencyMeters.IndexOf(meter));
				meter.gameObject.SetActive(isShow);

				if (isShow)
				{
					if (meter == GoldMeter)
					{
						GoldMeter.SetValue(player.GetCurrencyAmount(CurrencyType.Diamonds));
					}
					else if (meter == RadioMeter)
					{
						RadioMeter.SetValue(player.GetCurrencyAmount(CurrencyType.Phone));
					}
					else if (meter == TraitRerollMeter)
					{
						TraitRerollMeter.SetValue(player.GetCurrencyAmount(CurrencyType.TraitRerollToken));
					}
					else if (meter == GoldRadioMeter)
					{
						GoldRadioMeter.SetValue(player.GetCurrencyAmount(CurrencyType.GoldRadio));
					}
					else if (meter == CustomCurrencyMeter)
					{
						CustomCurrencyMeter.SetValue(player.GetCurrencyAmount(CustomCurrencyMeter.CurrencyType));
					}
				}
			}
			CurrencyMeterContainer.Reposition();
		}

		public void StartSettings()
		{
			RepositionSettingsTab();

			currentHeight = root.manualHeight;
			IsPlayerNameLableHide = false;

			IsRealPlayerToggle.Set(true);

			IsRealPlayerData = true;

			CurrencyCountMax = 200;
			CurrencyCountMaxPrev = CurrencyCountMax;
			ResidenceLevel = 2;
			Currency = new List<CurrencyModel>();

			MaxBadgeCounts = 100;
			EpicButton.isEnabled = DataManager.Instance.IsUseEOSLogin;

			MyTools.ResetLogPanel();

			string text = "CraftMachine v" + Application.version + " ©  by Bloodymary";
			versionLabel.text = text;

			DebugTWD.Log("DataURL: " + DataManager.DataURL);

			//bool isGeneral = DataManager.DataURL.Contains("backup");
			//var localizeListData = serverDataPopupList.GetComponent<PopupListLocalization>();
			//int index = isGeneral ? 0 : 1;
			//string data = DataManager.Instance.language == DataManager.Language.Ru ? localizeListData.RuCustomText[index] : localizeListData.EnCustomText[index];
			//var label = serverDataPopupList.transform.GetChild(0).GetComponent<UILabel>();
			//label.text = data;

			var indexUrl = UserPrefsKeys.Data_Url_Index;
			serverDataPopupList.value = serverDataPopupList.items[indexUrl];
			var localizeListData = serverDataPopupList.GetComponent<PopupListLocalization>();
			string data = DataManager.Instance.language == DataManager.Language.Ru ? localizeListData.RuCustomText[indexUrl] : localizeListData.EnCustomText[indexUrl];
			var label = serverDataPopupList.transform.GetChild(0).GetComponent<UILabel>();
			label.text = data;

			MissionHubButton.isEnabled = false;
		}

		public void MissionHubButtonEnable(bool isEnable)
		{
			MissionHubButton.isEnabled = isEnable;
		}

		public void RepositionSettingsTab(bool IsFit = false)
		{
			if (IsFit)
			{
				currentHeight = root.manualHeight;
				var items = SettingLeftPanel.GetChildList();

				float tableItemsHeight = 0;
				foreach (var item in items)
				{
					var b = NGUIMath.CalculateRelativeWidgetBounds(item, false).size.y;
					tableItemsHeight += b;
				}

				float tableHeight = SettingLeftPanel.GetComponent<UIWidget>().height;

				SettingLeftPanel.padding.y = .7f * (tableHeight - tableItemsHeight) / SettingLeftPanel.transform.childCount;
			}

			SettingLeftPanel.Reposition();
		}

		public void CheckInternetStatus()
		{
			string lastString = OfflineManager.Instance.LogPanelList.GetLastString();
			string message = "";
			var langEng = DataManager.Instance.language != DataManager.Language.Ru;

			isInternetOn = OfflineManager.IsInternetOn;

			if (isInternetOn)
			{
				message = (langEng ? "Internet : ON" : "Интернет : ВКЛЮЧЕН") + '\n';
			}
			else
			{
				message += (langEng ? "Internet : OFF" : "Интернет : ВЫКЛЮЧЕН") + '\n';
			}
			if (isInternetOn)
			{
				isLocal = DataManager.Instance.IsLocalPlayer;
				isSupaOnline = SupabaseManager.IsOnline;
				if (isSupaOnline)
				{
					EpicButton.isEnabled = true;
					LinkButton.isEnabled = true;
					message += (langEng ? "CraftMachine.Database status: ONLINE" : "Статус CraftMachine.Database : ДОСТУПЕН") + '\n';
				}
				else
				{
					if (!DataManager.Instance.IsReged && (DataManager.Instance.TrialModeOver || isLocal))
					{
						EpicButton.isEnabled = false;
						LinkButton.isEnabled = false;
					}
					else
					{
						EpicButton.isEnabled = true;
						LinkButton.isEnabled = true;
					}
					message += (langEng ? "CraftMachine.Database status: OFFLINE" : "Статус CraftMachine.Database : НЕ ОТВЕЧАЕТ") + '\n';
				}
			}
			else
			{
				isLocal = true;
				if (!Application.isEditor)
				{
					EpicButton.isEnabled = false;
					LinkButton.isEnabled = false;
				}
				DebugTWD.Log("Switch Epic Button to FALSE");
			}

			if (isLocal)
			{
				// message += (langEng ? "Local Session: ON" : "Локальная сессия: ВКЛЮЧЕНА") + '\n';
				DataManager.Instance.SetLocalPlayerResult(isLocal);
			}

			message = message.TrimEnd('\n');
			if (string.IsNullOrEmpty(lastString) || !lastString.Contains(message))
				MyTools.UpdateLogPanel(message);
		}

		public void SetScale(float scale)
		{
			scaleValue = scale;
		}

		public void ScaleSetUI(UIScrollBar scrollBar)
		{
			float scale = RemapValue(scrollBar.value, 0, 1, 1600, 2060);

			root.manualWidth = (int)Math.Round(scale);
			root.manualHeight = (int)((root.manualWidth * 9)/16);

			scaleLabelUI.text = root.manualWidth.ToString() + "/" + root.manualHeight.ToString();
		}

		public float RemapValue(float x, float in_min, float in_max, float out_min, float out_max, bool clamp = false)
		{
			if (clamp) x = Math.Max(in_min, Math.Min(x, in_max));
			return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
		}

		public void SetComponents()
		{
			components = new List<CurrencyType>();
			int rarity = rarityData.currentIndex;

			string recipe = recipeData.recipeLabel.text;
			components.Add(ComponentHelper.GetCurrencyFromBaseAndRarity(CurrencyType.Badge0, rarity));

			foreach (char sign in recipe)
			{
				components.Add(ComponentHelper.GetCurrencyFromBaseAndRarity(GetCurrencyByChar(sign), rarity));
			}

			if (components != null && components.Count == 5)
			{
				badgeTab.SetCraftComponents(components);
			}
		}

		public CurrencyType GetCurrencyByChar(char sign)
		{
			switch (sign)
			{
				case 'A': return CurrencyType.Metal0;
				case 'B': return CurrencyType.Food0;
				case 'C': return CurrencyType.Chemicals0;
				case 'D': return CurrencyType.Cloth0;
				default: return CurrencyType.None;
			}
		}

		public void UpdateUI()
		{
			if (languageScrollView != null)
			{
				for (int i = 0; i < languages.Count; i++)
				{
					UnityEngine.Object.Destroy(languages[i]);
				}
				languages.Clear();
				//List<string> uISupportedLanguages = OfflineManager.IsLoadDataManager ? CustomLocalization.AvailableLanguages : LocalizationManager.Instance.SupportedLanguages; //DataManager.Instance.GameData.ConfigData.UISupportedLanguages;
				List<string> uISupportedLanguages = LocalizationManager.Instance.SupportedLanguages;

				for (int j = 0; j < uISupportedLanguages.Count; j++)
				{
					GameObject gameObject = Helpers.InstantiateToParent(languagePrefab, languageScrollView.gameObject);
					gameObject.GetComponent<LanguageItem>().SetKey(null, uISupportedLanguages[j]);
					languages.Add(gameObject);
				}
				languageScrollView.ResetPosition();
				languageTable.repositionNow = true;
			}
			currentLanguage.text = LocalizationManager.GetText("LanguageName." + SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage.ToLower());
		}

		//OnOpenDropDown languages
		public void ToggleLanguageScroll()
		{
			UpdateUI();

			languageScrollContainer.SetActive(!languageScrollContainer.activeSelf);
			languageScrollView.ResetPosition();
		}

		public void SetLanguage(string key)
		{
			if (SingularityMonoBehaviour<LocalizationManager>.Instance != null && SingularityMonoBehaviour<LocalizationManager>.Instance.SupportedLanguages.Contains(key))
			{
				DataManager.Instance.languageKey = key;
				if (key == "ru") DataManager.Instance.language = DataManager.Language.Ru;
				else if (key == "es") DataManager.Instance.language = DataManager.Language.Es;
				else DataManager.Instance.language = DataManager.Language.En;

				SingularityMonoBehaviour<LocalizationManager>.Instance.Load(key);
				DebugTWD.Log($"load {key.ToUpper()} language");
			}
			languageScrollContainer.SetActive(value: false);
			UpdateUI();
		}

		public void OnLoadPlayer()
		{
			if (DataManager.Instance.contentSource != ContentSource.Local)
			{
				if (EpicText.TryGetComponent<LocalizationUIUpdater>(out var EpicTextLocalize))
				{
					//EpicTextLocalize.UpdateCustomContent("EpicTextConnect");
					EpicTextLocalize.RuCustomText = "Соединено";
					EpicTextLocalize.EnCustomText = "Connected";
					EpicTextLocalize.UpdateContent();
					EpicBtText.text = "Reconnect";
				}
				EpicNotConnectedObject.SetActive(false);

				DataManager.Instance.IsReconnectPlayerState = true;

				if (DataManager.Instance.IsReconnectByCode)
				{
					//string message = LocalizationManager.GetCustomText("EpicTextReconnect");
					string message = "";
					if (DataManager.Instance.language == DataManager.Language.Ru)
						message = "Рекоммендуется перезапустить мод после подключения другого профиля";
					else
						message = "It is recommended to restart the mod after connecting another profile";
					MyTools.OpenAlert(message);
					DataManager.Instance.IsReconnectByCode = false;
				}
			}
			else
			{
				if (LinkDeviceText.TryGetComponent<LocalizationUIUpdater>(out var LinkDeviceTextLocalize))
				{
					//LinkDeviceTextLocalize.UpdateCustomContent("LinkDeviceLoaded");
					LinkDeviceTextLocalize.RuCustomText = "Загружено";
					LinkDeviceTextLocalize.EnCustomText = "Loaded";
					LinkDeviceTextLocalize.UpdateContent();
					EpicBtText.text = "Connect";
				}
			}

			if (CraftsManLabel.TryGetComponent<LocalizationUIUpdater>(out var CraftsManLabelLocalize))
			{
				//CraftsManLabelLocalize.UpdateCustomContent("", DataManager.Instance.Player.Name);
				CraftsManLabelLocalize.IsCustomTranslate = true;
				CraftsManLabelLocalize.EnCustomText = DataManager.Instance.Player.Name;
				CraftsManLabelLocalize.UpdateContent(new string[1] { DataManager.Instance.Player.Name });
			}
		}

		public void OnInitialStateChange(UIInput input)
		{
			CustomInitialState = int.Parse(input.value);
			BadgeCraft.Instance.SetNewRandom(CustomInitialState);
			MyTools.UpdateLogPanel("CustomInitialState Badge: " + CustomInitialState);
		}

		public void OnSetUrl(UIInput input)
		{
			//string url = input.value;
			//DebugTWD.Log("Content File ID : " + url);
			//MyTools.UpdateLogPanel("Content File ID : " + url);
			//PlayerPrefs.SetString(UserPrefsKeys.Key_ContentFileID, url);
			//PlayerPrefs.Save();
			//DataManager.Instance.ContentFileID = url;
		}

		public void OnSetSheetUrl(UIInput input)
		{
			//string sheet = input.value;
			//DebugTWD.Log("Badge Log Sheet ID : " + sheet);
			//MyTools.UpdateLogPanel("Badge Log Sheet ID : " + sheet);
			//PlayerPrefs.SetString(UserPrefsKeys.Key_ContentSheetID, sheet);
			//PlayerPrefs.Save();
			//DataManager.Instance.ContentSheetID = sheet;
		}

		public void SetSettingsView()
		{
			InitialStateInput.value = DataManager.Instance.InitState.ToString();
			ClientVersionInput.value = OfflineManager.ClientVersion;
			//ContentFileIDInput.value = DataManager.Instance.ContentFileID;
			//ContentSheetIDInput.value = DataManager.Instance.ContentSheetID;
		}

		public void IsRealPlayerDataChange(UIToggle toggle)
		{
			IsRealPlayerData = toggle.value;
			CurrencyCountMaxInput.transform.parent.gameObject.SetActive(!IsRealPlayerData);
			if (!IsRealPlayerData)
			{
				CurrencyCountMax = int.Parse(CurrencyCountMaxInput.value);
			}
			ResidencePopup.Instance.UpdateComponentInventory();
		}

		public void CurrencyCountMaxChange(UIInput input)
		{
			CurrencyCountMax = int.Parse(input.value);
			CurrencyCountMaxChangeInternal();
		}

		public void CurrencyCountMaxChangeBt()
		{
			CurrencyCountMax = int.Parse(CurrencyCountMaxInput.value);
			CurrencyCountMaxChangeInternal();
		}

		private void CurrencyCountMaxChangeInternal()
		{
			if (CurrencyCountMaxPrev != CurrencyCountMax)
			{
				CurrencyCountMaxPrev = CurrencyCountMax;
				Currency = new List<CurrencyModel>();
				ResidencePopup.Instance.UpdateComponentInventory();
			}
		}

		public void ClearLog()
		{
			MyTools.ResetLogPanel();
		}

		public void HideShowPlayerName()
		{
			IsPlayerNameLableHide = !IsPlayerNameLableHide;

			StartCoroutine(TakeScreenShotAdvanced());
		}

		public void OnClickDonate()
		{
			InfoCustomPopup donatePopup = HUDManager.Instance.Get(UIType.InfoCustomPopup) as InfoCustomPopup;

			if (donatePopup != null)
			{
				//string header = LocalizationManager.GetCustomText("DanateAuthorHeader");
				//string message = LocalizationManager.GetCustomText("DanateAuthorText");

				//donatePopup.SetContentWithTex(header, message, donateTex);
				if (DataManager.Instance.language == DataManager.Language.Ru)
				{
					donatePopup.SetContent("Поблагодари автора, если хочешь!", @"Донат через Юмани: https://yoomoney.ru/to/4100118657808052/500" + '\n' +
					@"Контакт в Telegram: https://t.me/BloodyModding" + '\n' + "Спасибо заранее!", donateTex);
				}
				else
				{
					donatePopup.SetContent("Donate if you want", @"Donate link: https://yoomoney.ru/to/4100118657808052/500" + '\n' +
					@"Contacts in Telegram: https://t.me/BloodyModding" +'\n' + "Thanks in advance!", donateTex);
				}
				donatePopup.Open();
			}
		}

		private IEnumerator TakeScreenShotAdvanced()
		{
			if (IsPlayerNameLableHide)
			{
				prevLangKey = SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage;
				SetLanguage("en");
				CraftsManLabel.GetComponent<TweenScale>().PlayForward();

				EpicConnectTween.PlayForward();
				EpicSwitchTween.PlayForward();
				InitStateTween.PlayForward();
				GameVersionTween.PlayForward();
				LogSheetTween.PlayForward();
				DataUrlTween.PlayForward();

				MyTools.TweenLogPanel(true);
				GameStatus.GetComponent<TweenScale>().PlayForward();
				yield return new WaitForSeconds(.6f);
				SettingLeftPanel.Reposition();
				//StartCoroutine(TakeSSAndShare());
			}
			else
			{
				SetLanguage(prevLangKey);
				CraftsManLabel.GetComponent<TweenScale>().PlayReverse();

				EpicConnectTween.PlayReverse();
				EpicSwitchTween.PlayReverse();
				InitStateTween.PlayReverse();
				GameVersionTween.PlayReverse();
				LogSheetTween.PlayReverse();
				DataUrlTween.PlayReverse();

				MyTools.TweenLogPanel(false);
				GameStatus.GetComponent<TweenScale>().PlayReverse();
				yield return new WaitForSeconds(.6f);
				SettingLeftPanel.Reposition();
			}

			if (ResidencePopup.Instance.GetSelectedIndex() == 1)
			{
				badgeTab.UpdateUI();
			}
			yield return null;
		}

		public void OnClickOpen(GameObject go)
		{
			go.SetActive(true);
		}

		public void OnClickClose(GameObject go)
		{
			DebugTWD.Log("close");
			go.SetActive(false);
		}

		public void OnClickFeedback()
		{
			FeedbackObject.transform.parent.gameObject.SetActive(true);

			bool IsFeedbackReaded = DataManager.Instance.IsFeedbackReaded;
			if (!IsFeedbackReaded)
			{
				WishObject.SetActive(false);
				FeedbackObject.SetActive(true);

				SetupGlowFeedback(isOn: false);

				DataManager.Instance.IsFeedbackReaded = true;
			}
			else
			{
				WishObject.SetActive(true);
				FeedbackObject.SetActive(false);
			}
		}

		public void SetupGlowFeedback(bool isOn)
		{
			FeedbackInput.value = DataManager.Instance.Feedback;
			FeedbackIndicator.SetActive(isOn);
			FeedbackGlow.gameObject.SetActive(isOn);
			FeedbackGlow.enabled = isOn;
		}

		public void OnSwitchFeedBack()
		{
			WishObject.SetActive(!WishObject.activeSelf);
			FeedbackObject.SetActive(!FeedbackObject.activeSelf);
		}

		private string GetAndroidExternalStoragePath()
		{
			if (Application.platform != RuntimePlatform.Android)
				return DataManager.PlayerSSFolder;

			var jc = new AndroidJavaClass("android.os.Environment");
			var path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory",
				jc.GetStatic<string>("DIRECTORY_DCIM"))
				.Call<string>("getAbsolutePath");
			return path + "/Screenshots";
		}

		public void ShowStatPanel(UIToggle tg)
		{
			OfflineManager.Instance.LogPanelList?.gameObject.SetActive(!tg.value);
			OfflineManager.Instance.StatPanelList?.gameObject.SetActive(tg.value);
			PlayerModel Player = OfflineManager.Instance.Player;

			if (tg.value && Player != null)
			{
				string text = "";

				TimeSpan tp = MyTools.TimeSpanFromLong(Player.LifeTime);
				string lifeTime = "Play Time : " + MyTools.ToReadableString(tp) + '\n';

				string lastSessionRun = "Last Time Epic Used : " + Player.UtcTime.ToLocalTime().ToString(UserPrefsKeys.TimeFormat) + '\n';

				TimeSpan tp2 = MyTools.TimeSpanFromLong(Player.SessionHistory.Last(x => x.Length > 1000).Length);
				string lastSessionLength = "Last Game Session Length : " + MyTools.ToReadableString(tp2) + '\n';
				string firstRun = "Play Start Time : " + Player.Created.ToLocalTime().ToString(UserPrefsKeys.TimeFormat) + '\n';

				string spentMoney = "Total USD Spent : " + Math.Round(Player.TotalUSDSpent, 2).ToString().Replace(',', '.') + " $" + '\n';
				string spentCount = "Total Purchases : " + Player.GetTotalPurchases().ToString() + '\n';

				text += lifeTime + lastSessionRun + lastSessionLength + firstRun + spentMoney + spentCount;

				OfflineManager.Instance.StatPanelList.Clear();
				OfflineManager.Instance.StatPanelList.Add(text);
			}
		}

		public void OnclickBag()
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			ConsumablesPopup consumablesPopup = HUDManager.TryOpenPopup(UIType.ConsumablesCampPopup) as ConsumablesPopup;
			if (consumablesPopup != null)
			{
				consumablesPopup.Open();
				consumablesPopup.OnClickToolBagSkillTab();
				consumablesPopup.OnToolBagTabSwitched(2);
			}
		}

		public void TryReconnectSignalR()
		{
			if (!SignalRClient.Instance)
			{
				return;
			}
			if (OfflineManager.IsInternetOn && SignalRClient.Instance.state == SignalRClientState.Disconnected)
			{
				SignalRClient.Instance.TryReconnect();
			}
		}

		private IEnumerator TakeSSAndShare()
		{
			string timeStamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
			yield return new WaitForEndOfFrame();
			Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			ss.Apply();

			string filePath = Path.Combine(GetAndroidExternalStoragePath(), "TWD_Screenshot_" + timeStamp + ".png");
			try
			{
				File.WriteAllBytes(filePath, ss.EncodeToPNG());
			}
			catch
			{
				yield break;
			}

			Destroy(ss);
		}
	}
}
