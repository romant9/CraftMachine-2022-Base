using Client.Connectivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TwdCustomMod;
using UnityEngine;
using UnityEngine.PostProcessing;

public static class MyTools
{
	public static void OpenAlert(string text)
	{
		AlertPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
		if (confirmationPopup != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);

			confirmationPopup.SetContent("", text);
			confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			confirmationPopup.SetCallbacks(delegate
			{
				confirmationPopup.Close();
			});
			confirmationPopup.Open();
		}
	}

	public static void OpenInfo(string text, Callback callbackOk = null, Callback callCancel = null)
	{
		ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
		if (confirmationPopup != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);

			confirmationPopup.SetContent("", text);
			confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			confirmationPopup.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));

			callbackOk ??= delegate { confirmationPopup.Close(); };
			callCancel ??= delegate { confirmationPopup.Close(); };

			confirmationPopup.SetCallbacks(callbackOk, callCancel);
			confirmationPopup.Open();
		}
	}

	public static void UpdateLogPanel(string textEn, string textRu = "")
	{
		string message;
		if (string.IsNullOrEmpty(textRu)) message = textEn;
		else
		{
			message = DataManager.Instance.language == DataManager.Language.Ru ? textRu : textEn;
		}

		OfflineManager.Instance.LogPanelList?.Add(message + '\n');
    }

	public static void TweenLogPanel(bool isForward)
	{
		var logPanel = OfflineManager.Instance.LogPanelList;
		if (logPanel != null)
		{
			if (isForward)
				OfflineManager.Instance.LogPanelList.textLabel.GetComponent<TweenScale>().PlayForward();
			else
				OfflineManager.Instance.LogPanelList.textLabel.GetComponent<TweenScale>().PlayReverse();
		}
	}

	public static void CopyToClipboard(string text)
	{
		TextEditor te = new TextEditor();
		te.text = text.Trim();
		te.SelectAll();
		te.Copy();
	}

	public static void SaveToFile(string content, string path, bool append = false, bool writeList = false, List<string> contentList = null)
	{
		try
		{
			if (File.Exists(path) && append)
			{
				if (!writeList) File.AppendAllText(path, content); else File.AppendAllLines(path, contentList);
			}
			else
			{
				if (!writeList) File.WriteAllText(path, content); else File.WriteAllLines(path, contentList);
			}
		}
		catch
		{
			Debug.LogWarning("Can't save file: " + path);
		}
	}

	public static GameObject GetParent(GameObject refObject = null)
	{
		return OfflineManager.IsLoadDataManager ? refObject == null || refObject.layer == 5 ? HUDManager.Instance.UIContainer : HUDManager.Instance.UIContainerTopCameras : null;
	}

	public static TimeSpan TimeSpanFromLong(long x)
	{
		return TimeSpan.FromSeconds(x / 1000);
	}

	public static string ToReadableString(long x)
	{
		return ToReadableString(TimeSpanFromLong(x));
	}

	public static string LongToTime(long longTime)
	{
		return LongToDate(longTime).ToString(UserPrefsKeys.TimeFormat);
	}

	public static DateTime LongToDate(long longTime)
	{
		DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return baseTime.AddMilliseconds(longTime).ToLocalTime();
	}

	public static long TimeSpanToLong(TimeSpan period)
	{
		return (long)period.TotalSeconds * 1000;
	}

	public static string LongToTimeString(long longTime)
	{
		return LongToDate(longTime).ToString(UserPrefsKeys.TimeFormat);
	}

	public static string DateToLong(long longTime)
	{
		return LongToDate(longTime).ToString(UserPrefsKeys.TimeFormat);
	}

	public static string DateTimeToTimeString(DateTime date)
	{
		return date.ToLocalTime().ToString(UserPrefsKeys.TimeFormat);
	}

	public static string ReplaceNewlines(string text)
	{
		return Regex.Replace(text, "\\t|\\n|\\r", "");
	}

	public static string ToReadableString(TimeSpan span)
	{
		string formatted = string.Format("{0}{1}{2}{3}{4}",
			Mathf.Round(span.Days / 365) > 0 ? string.Format("{0:0} years, ", Mathf.Round(span.Days / 365)) : string.Empty,
			span.Days > 0 && Mathf.Round(span.Days / 365) > 0 ? string.Format("{0:0} days, ", span.Days % 365) :
			span.Days > 0 ? string.Format("{0:0} days, ", span.Days % 365) : string.Empty,
			span.Hours > 0 ? string.Format("{0:0} hours, ", span.Hours) : string.Empty,
			span.Minutes > 0 ? string.Format("{0:0} minutes, ", span.Minutes) : string.Empty,
			span.Seconds > 0 ? string.Format("{0:0} seconds, ", span.Seconds) : string.Empty);

		if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

		return formatted;
	}

	public static void ResetLogPanel()
	{
		TWDPlayerPrefs.Save();

		string text;

		DataManager.Instance.CheckPinID();
		bool pro_guild = TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_ProGuild);
		bool pro_Link = TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_ProLink);
		bool anonymous = TWDPlayerPrefs.GetBool(UserPrefsKeys.Player_Anonymous);

		if (DataManager.Instance.language == DataManager.Language.Ru)
		{
			text = "При наличии интернет соединения кликайте Коннект для загрузки данных игрока из игрового профиля Эпик.\n";

			text += "Нажмите «Начать загрузку», чтобы загрузить данные игрока локально из кэша.\n";

			text += "ModelRandom – это ваш рандомизатор для крафта значков, рерола значков, рерола навыков героев\n" +
				"ModelRandom обновляется после каждого перечисленного действия и влияет на дальнейшее.\n" +
				"ModelRandom не влияет на вызовы. (На них ничто не влияет)\n";
			text += "В этом моде рероллы НЕ ВЛИЯЮТ на крафт значков,\n- но крафт значков ВЛИЯЕТ на рероллы.\n" +
				"PlayerRandom - это ваш рандомизатор ремодела снаряжения, прорыва снаряжения, покупку наград за вызовы.\n" +
				"PlayerRandom обновляется после каждого перечисленного действия и влияет на дальнейшее.";

			text += "\n---------------------------------------\n";

			text += CheckSavedContent($"\nДоступен ранее сохраненный профиль : {UserPrefsKeys.Player_Name}\n\n");

			if (DataManager.IsPinId) text += "Домашний игровой профиль : " + UserPrefsKeys.Player_Pin_Name + '\n';

			text += "Вип доступ к данным гильдии : " + (pro_guild ? "ОТКРЫТ" : "ЗАКРЫТ") + "\n";
			text += "Вип доступ к поиску и загрузке гильдий : " + (pro_guild ? "ОТКРЫТ" : "ЗАКРЫТ") + "\n";
			text += "Вип доступ к базе аккаунтов : " + (pro_Link ? "ОТКРЫТ" : "ЗАКРЫТ") + "\n";
			text += "Анонимность личного профиля : " + (anonymous ? "ВКЛЮЧЕНА" : "ВЫКЛЮЧЕНА") + "\n";
		}
		else
		{
			text = "If you have an Internet connection, click Connect to download player data from the Epic game profile\n";

			text += "Click \"Start Loading\" to download player's content locally from cache\n";

			text += "ModelRandom is your randomizer for crafting badges, rerolling badges, rerolling hero traits.\n" +
				"ModelRandom is updated after each listed action and affects further.\n" +
				"ModelRandom does not affect calls. (Nothing affects them)\n";
			text += "In this mod, rerolls DO NOT AFFECT badge crafting,\n- but badge crafting DOES AFFECT rerolls.\n" +
				"PlayerRandom is your randomizer for equipment remodeling, equipment breakthrough, and buying challenge rewards.\n" +
				"PlayerRandom is updated after each listed action and affects the next one.";

			text += "\n---------------------------------------\n";
			text += CheckSavedContent($"\nLocal saved player's content is available: {UserPrefsKeys.Player_Name}\n\n");

			if (DataManager.IsPinId) text += "Base game profile : " + UserPrefsKeys.Player_Pin_Name + '\n';

			text += "VIP access to guild data is : " + (pro_guild ? "OPEN" : "CLOSED") + "\n";
			text += "VIP access to guild search and download is : " + (pro_guild ? "OPEN" : "CLOSED") + "\n";
			text += "VIP access to the account database is : " + (pro_Link ? "OPEN" : "CLOSED") + "\n";
			text += "Personal profile anonymity is : " + (anonymous ? "ENABLED" : "DISABLED") + "\n";
		}

		var logPanelList = OfflineManager.Instance.LogPanelList;
		if (logPanelList != null)
		{
			logPanelList.Clear();
			logPanelList.Add(text);
		}
	}

	public static string CheckSavedContent(string text)
	{
		string newText = string.Empty;
		string path = DataManager.GameDataFolder + "ContentCache/Player/";

		if (Directory.Exists(path))
		{
			DirectoryInfo dir = new (path);
			var file = dir.GetFiles().FirstOrDefault(x => x.Name != "index.txt");
			if (file != null)
			{
				newText += text;
			}
		}
		return newText;
	}

	public static float InterpolateRange(float min1, float max1, float min2, float max2, float value2)
	{
		//min2, max2, value2 - slider
		//min1, max1 - my range
		//float delta = (value2 - min2) / (max2 - min2);
		//float value1 = min1 + delta * (max1 - min1);
		//return value1;
		return Mathf.Lerp(min1, max1, (value2 - min2) / (max2 - min2));
	}

	public static void SaveModelMessages()
	{
		var messages = SignalRClient.Instance.Messages;
		if (messages.Count > 0)
		{
			SaveToFile('[' + string.Join(",", messages) + ']', CommandHelper.GlobalPath + "messages.json", false);
		}
	}

	public static Texture2D ToTexture2D(Texture texture)
	{
		return Texture2D.CreateExternalTexture(
			texture.width,
			texture.height,
			TextureFormat.RGB24,
			false, false,
			texture.GetNativeTexturePtr());
	}

	//For ActionCamera
	public static void ModifyPProfile(GameObject go)
	{
		if (!go.TryGetComponent<PostProcessingBehaviour>(out PostProcessingBehaviour ppBehav))
			ppBehav = go.AddComponent<PostProcessingBehaviour>();
		ppBehav.profile = OfflineManager.Instance.PostProfile;
	}

	//For AssetBundleController
	#region GetFileName2Md5Dict
	public static Dictionary<string, string> GetFileName2Md5Dict()
	{
		var localVersion = Resources.Load<TextAsset>("version");
		if (localVersion != null)
		{
			return FromString(localVersion.text);
		}
		DebugTWD.LogError("Failed GetFileName2Md5Dict");
		return null;
	}

	private static Dictionary<string, string> FromString(string csv)
	{
		if (string.IsNullOrEmpty(csv))
		{
			return null;
		}
		var versionFileInfos = new Dictionary<string, string>();
		string[] array = csv.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			var versionFileInfo = FromStringRow(array[i]);
			if (versionFileInfo != null)
			{
				versionFileInfos.Add(versionFileInfo[0], versionFileInfo[1]);
			}
		}
		return versionFileInfos;
	}

	public static string[] FromStringRow(string csv)
	{
		if (string.IsNullOrEmpty(csv))
		{
			return null;
		}
		string[] array = csv.Split('\t');
		if (array.Length < 3)
		{
			Debug.LogError((object)("Error VersionFileInfo Length Smaller than 3: " + csv));
			return null;
		}
		string text = array[0];
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError((object)("Error VersionFileInfo Empty or Null Name: " + csv));
			return null;
		}
		string text2 = array[1];
		if (string.IsNullOrEmpty(text2))
		{
			Debug.LogError((object)("Error VersionFileInfo Empty or Null MD5: " + csv));
			return null;
		}
		if (!int.TryParse(array[2], out _))
		{
			Debug.LogError((object)("Error VersionFileInfo Parse Size Failed: " + csv));
			return null;
		}
		return new string[2] { text, text2 };
	}
	#endregion

	#region ArrayTools
	public static T[] Add<T>(this T[] arr, T buttonToggle)
	{
		int newCount = arr.Length + 1;
		Array.Resize(ref arr, newCount);
		arr[arr.Length - 1] = buttonToggle;
		return arr;
	}

	public static void RemoveElementAt<T>(this T[] arr, int index)
	{
		if (index < 0 || index >= arr.Length)
		{
			return;
		}

		for (int i = index; i < arr.Length - 1; i++)
		{
			arr[i] = arr[i + 1];
		}
		Array.Resize(ref arr, arr.Length - 1);
	}

	public static void RemoveElement<T>(this T[] arr, T buttonToggle)
	{
		int index = arr.IndexOf(buttonToggle);
		arr.RemoveElementAt(index);
	}

	public static int IndexOf<T>(this T[] arr, T buttonToggle)
	{
		return Array.IndexOf(arr, buttonToggle);
	}
	#endregion
}
