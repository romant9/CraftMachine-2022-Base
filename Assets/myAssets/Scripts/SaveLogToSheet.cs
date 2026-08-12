#if GOOGLE_SHEET
using Google;
using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TwdCustomMod;
using UnityEngine;
using UnityEngine.Events;

public partial class SaveLogToSheet : MonoBehaviour
{
	public static SaveLogToSheet Instance { get; private set; }

	public List<BadgeLog> BadgeLogList;

	[SerializeField]
	private string SheetRegID;
	private string wsRegName = "Users";
	private string currentHashID;

	public string SheetID { get; set; }
	public const string WsNeedUpdateName = "BadgeCraft";
	public bool IsBuizy { get; set; }

	public bool IsBlocked {get; set;}

	private List<List<string>> tempList = new List<List<string>>();

	public bool IsSetRegCodes;
    private User currentUser => PostGreManager.Instance.CurrentUser;

	[ContextMenu("GetAcountInfo")]
	void GetAcountInfo()
	{
		Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
		if (signIn.Result != null)
		{
			Debug.Log("Mail is " + signIn.Result.Email);
		}
	}

	private void Awake()
	{
		Instance = this;
		IsBlocked = false;
		IsBuizy = false;
		//userIndex = -1;
    }

    void Start()
	{
		//Calculate RegCode for all players
		if (IsSetRegCodes) GetUserData("", OnGetPlayersList);
	}

	public void SaveLog()
	{
		DataTable DtCampaignDefinitions = ToDataTableList(BadgeLogList, out List<List<string>> list);
		tempList = list;
		DebugTWD.Log(SheetID + " : " + WsNeedUpdateName);
		SpreadsheetManager.Write(new GSTU_Search(SheetID, WsNeedUpdateName), new ValueRange(tempList[0]), OnWriteFinish);
	}

	public void OnWriteFinish()
	{
		SpreadsheetManager.Append(new GSTU_Search(SheetID, WsNeedUpdateName), new ValueRange(tempList.GetRange(1, tempList.Count - 1)), OnAppendFinish);
	}

	public void OnAppendFinish()
	{
		tempList.Clear();
		var log = "Badge Log BadgesCraft was saved to Google Spreadsheet";
		MyTools.UpdateLogPanel(log);
		DebugTWD.Log(log);
	}

	public static DataTable ToDataTableList<T>(List<T> ability, out List<List<string>> list)
	{
		list = new List<List<string>>();
		try
		{
			Type elementType = typeof(T);
			DataTable dt = new DataTable();
			var listCol = new List<string>();
			try
			{
				//add a column to table for each public property on T
				foreach (var propInfo in elementType.GetProperties())
				{
					try
					{
						Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

						dt.Columns.Add(propInfo.Name, ColType);
						listCol.Add(propInfo.Name);

					}
					catch (Exception ex)
					{
						DebugTWD.LogException(ex);
					}
				}
			}
			catch (Exception ex)
			{
				DebugTWD.LogException(ex);
			}

			list.Add(listCol);

			try
			{
				//go through each property on T and add each value to the table
				for (int i = 0; i < ability.Count; i++)
				{
					var item = ability[i];
					DataRow row = dt.NewRow();
					var listRow = new List<string>();

					foreach (var propInfo in elementType.GetProperties())
					{
						row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
						listRow.Add(row[propInfo.Name].ToString());
					}

					dt.Rows.Add(row);
					list.Add(listRow);
				}
			}
			catch (Exception ex)
			{
				DebugTWD.LogException(ex);
			}

			return dt;
		}
		catch (Exception ex)
		{
			DebugTWD.LogException(ex);

			return null;
		}
	}

	public void TestReadSheet(UnityAction<GstuSpreadSheet> callback)
	{
		SpreadsheetManager.Read(new GSTU_Search(SheetRegID, wsRegName, "A1"), callback);
	}

	public void GetUserData(string hashID, UnityAction<GstuSpreadSheet> callback)
	{
		IsBuizy = true;
		currentHashID = hashID;
		SpreadsheetManager.Read(new GSTU_Search(SheetRegID, wsRegName, "A1"), callback);
	}

	public void OnGetPlayersList(GstuSpreadSheet sheet)
	{
		if (!IsSetRegCodes) return;

		DebugTWD.Log("Save Reg Codes");
		List<string> list = RowUser(sheet);
		BatchRequestBody​ updateRequest = new BatchRequestBody();

		foreach(var item in list)
		{
			updateRequest.Add(sheet[item, "Code"].AddCellToBatchUpdate(SheetRegID, wsRegName, UserPrefsKeys.GeneratedCode(item).ToString()));
		}
		updateRequest.Send(SheetRegID, wsRegName, OnUpdatePlayer);
	}

    public void OnSendMessage(GstuSpreadSheet​ ss)
	{
		if (ss == null || currentUser == null || string.IsNullOrEmpty(currentUser.HashID))
		{
			DebugTWD.Log("Can't save settings online");
			IsBuizy = false;
			return;
		}
		BatchRequestBody​ updateRequest = new BatchRequestBody();
		var wishes = DataManager.Instance.UserWishes;
		if (!string.IsNullOrEmpty(wishes) && wishes != "null") updateRequest.Add(ss[currentUser.HashID, "Wishes"].AddCellToBatchUpdate(SheetRegID, wsRegName, wishes));
		updateRequest.Send(SheetRegID, wsRegName, OnUpdatePlayer);
	}

	public void OnSendGuildName(GstuSpreadSheet​ ss)
	{
		if (ss == null || currentUser == null || string.IsNullOrEmpty(currentUser.HashID))
		{
			DebugTWD.Log("Can't save settings online");
			IsBuizy = false;
			return;
		}
		BatchRequestBody​ updateRequest = new BatchRequestBody();
		updateRequest.Add(ss[currentUser.HashID, "Guild"].AddCellToBatchUpdate(SheetRegID, wsRegName, currentUser.GuilName));
		updateRequest.Send(SheetRegID, wsRegName, OnUpdatePlayer);
	}

	public void OnSetUserData(GstuSpreadSheet​ ss)
	{
		if (ss == null || currentUser == null || string.IsNullOrEmpty(currentUser.HashID) || string.IsNullOrEmpty(DataManager.UserInfo))
		{
			DebugTWD.Log("Can't save settings online");
			IsBuizy = false;
			return;
		}

		BatchRequestBody​ updateRequest = new BatchRequestBody();

		var userHash = currentUser.HashID;

		DebugTWD.Log("GS User HashID : " + userHash);
		DebugTWD.Log("GS User LastRun : " + DateTime.Now.ToString(UserPrefsKeys.TimeFormat));
		DebugTWD.Log("GS User TimesRun : " + currentUser.TimesRun);
		DebugTWD.Log("GS User TimesGetContent : " + currentUser.TimesConnect);
		DebugTWD.Log("GS User ClientVersion : " + OfflineManager.ClientVersion);

		updateRequest.Add(ss[userHash, "UserName"].AddCellToBatchUpdate(SheetRegID, wsRegName, DataManager.UserInfo));
		updateRequest.Add(ss[userHash, "LastRun"].AddCellToBatchUpdate(SheetRegID, wsRegName, DateTime.Now.ToString(UserPrefsKeys.TimeFormat)));
		updateRequest.Add(ss[userHash, "TimesRun"].AddCellToBatchUpdate(SheetRegID, wsRegName, currentUser.TimesRun.ToString()));
		updateRequest.Add(ss[userHash, "TimesGetContent"].AddCellToBatchUpdate(SheetRegID, wsRegName, currentUser.TimesConnect.ToString()));
		updateRequest.Add(ss[userHash, "Regged"].AddCellToBatchUpdate(SheetRegID, wsRegName, currentUser.Regged.ToString()));
		if (currentUser.Whishes != "null" && !string.IsNullOrEmpty(currentUser.Whishes)) updateRequest.Add(ss[userHash, "Wishes"].AddCellToBatchUpdate(SheetRegID, wsRegName, currentUser.Whishes));
		if (currentUser.Country != "null" && !string.IsNullOrEmpty(currentUser.Country)) updateRequest.Add(ss[userHash, "Country"].AddCellToBatchUpdate(SheetRegID, wsRegName, currentUser.Country));
		if (currentUser.GuilName != "null" && !string.IsNullOrEmpty(currentUser.GuilName)) updateRequest.Add(ss[userHash, "Guild"].AddCellToBatchUpdate(SheetRegID, wsRegName, currentUser.GuilName));
		updateRequest.Add(ss[userHash, "ClientVersion"].AddCellToBatchUpdate(SheetRegID, wsRegName, OfflineManager.ClientVersion));
		updateRequest.Add(ss[userHash, "Code"].AddCellToBatchUpdate(SheetRegID, wsRegName, UserPrefsKeys.GeneratedCode(userHash).ToString()));
		updateRequest.Add(ss[userHash, "ModVersion"].AddCellToBatchUpdate(SheetRegID, wsRegName, Application.version));

		string EpicId = currentUser.EpicId;
		if (DataManager.Instance.Anonymous && !EpicId.Contains("hide"))
		{
			if (DataManager.IsPinId && userHash == DataManager.Instance.Pin_HashID)
				EpicId = "hide-" + currentUser.EpicId;
		}
		updateRequest.Add(ss[userHash, "EosUserId"].AddCellToBatchUpdate(SheetRegID, wsRegName, EpicId));

		string EosAccountID = currentUser.GoogleId;
		if (!string.IsNullOrEmpty(EosAccountID))
			updateRequest.Add(ss[userHash, "EosAccountID"].AddCellToBatchUpdate(SheetRegID, wsRegName, EosAccountID));

		updateRequest.Send(SheetRegID, wsRegName, OnUpdatePlayer);

		//SpreadsheetManager.Write(new GSTU_Search(SheetRegID, wsRegName), callback);
	}

	public void OnUpdatePlayer()
	{
		IsBuizy = false;
        DebugTWD.Log("update successfully");
    }

    public void OnGetBlockedData(GstuSpreadSheet​ ss)
	{
		IsBlocked = GetBlockedStatus(ss);
		IsBuizy = false;
	}

	public bool GetBlockedStatus(GstuSpreadSheet​ ss)
	{
		var blockUser = CellUser(ss);
		if (blockUser != null) { return bool.Parse(ss[blockUser.value, "Blocked"].value); }
		else { return false; }
	}

	public List<string> RowUser(GstuSpreadSheet​ ss)
	{
		List<string> result = new List<string>();
		foreach (var cell in ss.columns["A"])
		{
			if (cell.value != "HashID")
			{
				result.Add(cell.value);
			}
		}
		return result;
	}

	public GSTU_Cell CellUser(GstuSpreadSheet​ ss)
	{
		if (ss == null || ss.columns["A"] == null) return null;

		foreach (var cell in ss.columns["A"])
		{
			if (cell.value == currentHashID)
			{
				//userIndex = ss.columns["A"].IndexOf(cell);
				return cell;
			}
		}
		return null;
	}

	public void OnGetUserData(GstuSpreadSheet​ ss)
	{
		if (ss == null || ss.columns["A"] == null)
		{
			if (DataManager.Instance.language == DataManager.Language.Ru)
			{
				MyTools.UpdateLogPanel("Не могу прочитать рег данные пользователя. Попробуйте переподключиться...");
			}
			else
			{
				MyTools.UpdateLogPanel("Can't recall user reg data from dev server. Please reconnect!");
			}
		}

		GSTU_Cell cellUser = CellUser(ss);

		currentUser.HashID = PlayerPrefs.GetString(UserPrefsKeys.Player_HashID);
		currentUser.PlayerName = PlayerPrefs.GetString(UserPrefsKeys.Player_Name);
		currentUser.EpicId = PlayerPrefs.GetString(UserPrefsKeys.Player_GoogleID);
		currentUser.GoogleId = PlayerPrefs.GetString(UserPrefsKeys.Player_EpicAccountID);

		if (cellUser != null)
		{
			//HashID = cellUser.value,
			//Name = ss[cellUser.value,"Name"].value,
			currentUser.FirstRun = ss[cellUser.value, "FirstRun"].value;
			currentUser.TimesRun = int.Parse(ss[cellUser.value, "TimesRun"].value);
			currentUser.TimesConnect = int.Parse(ss[cellUser.value, "TimesGetContent"].value);
			currentUser.Regged = bool.Parse(ss[cellUser.value, "Regged"].value);
			currentUser.Blocked = bool.Parse(ss[cellUser.value, "Blocked"].value);
			//Description = ss[cellUser.value, "Description"] != null ? ss[cellUser.value, "Description"].value : "",
			//Whishes = ss[cellUser.value, "Whishes"] != null ? ss[cellUser.value, "Whishes"].value : "",
			//SheetID = ss[cellUser.value, "Log Sheet"] != null ? ss[cellUser.value, "Log Sheet"].value : ""
			currentUser.Feedback = ss[cellUser.value, "Feedback"].value;
			currentUser.ProGuild = bool.Parse(ss[cellUser.value, "Pro"].value);
			currentUser.ProLink = bool.Parse(ss[cellUser.value, "ProEos"].value);
			currentUser.ClientVersion = ss[cellUser.value, "ClientVersion"].value;
			currentUser.RegCode = long.Parse(ss[cellUser.value, "Code"].value);
			currentUser.ModVersion = ss[cellUser.value, "ModVersion"].value;

			IsBuizy = false;
			DebugTWD.Log("read saved user, feedback : " + currentUser.Feedback);
		}
		else
		{
			currentUser.FirstRun = DataManager.Instance.FirstRun;
			currentUser.LastRun = DateTime.Now.ToString(UserPrefsKeys.TimeFormat);
			currentUser.TimesRun = DataManager.Instance.TimesRun;
			currentUser.TimesConnect = DataManager.Instance.TimesConnect;
			currentUser.Regged = false;
			currentUser.Blocked = false;
			currentUser.Whishes = DataManager.Instance.UserWishes ?? "null";
			currentUser.Feedback = "null";
			currentUser.Country = DataManager.CountryCode ?? "null";
			currentUser.ProGuild = false;
			currentUser.GuilName = DataManager.Instance.Player.GuildName ?? "null";
			currentUser.DeviceInfo = DataManager.UserInfo;
			currentUser.ProLink = false;
			currentUser.ClientVersion = OfflineManager.ClientVersion;
			currentUser.RegCode = UserPrefsKeys.GeneratedCode(currentUser.HashID);
			currentUser.ModVersion = Application.version;

			DebugTWD.Log("TimesRun: " + currentUser.TimesRun, DebugType.System);
			DebugTWD.Log("Create user " + currentUser.PlayerName);

			var userList = new List<User>() { currentUser };
			DataTable DtCampaignDefinitions = ToDataTableList(userList, out List<List<string>> list);
			tempList = list;

			SpreadsheetManager.Append(new GSTU_Search(SheetRegID, wsRegName), new ValueRange(list.GetRange(1, tempList.Count - 1)), OnAddUserFinish);
		}
	}

	public void OnAddUserFinish()
	{
		IsBuizy = false;
		tempList.Clear();
		DebugTWD.Log("User add to User SpreadSheet");
	}
}
#endif
