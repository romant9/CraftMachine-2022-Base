using Client.Connectivity;
using Supabase.TWD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TWDModel;
using UnityEngine;

namespace TwdCustomMod
{
	public class GWTeamUtils : MonoBehaviour
	{
		public static GWTeamUtils Instance { get; private set; }

		public const string playerFolder = @"Bloodymary\new\";
		private GameEconomyData GameData => DataManager.Instance.GameData;
		private MessageSerializer jsonSerializer => OfflineManager.JsonSerializer;

		public GuildModel CurrentGuildModel => IsOpponentGuild && OpponentGuilModel != null ? OpponentGuilModel : GuildModel;
		public GuildModel GuildModel { get; set; }

		public GuildModel OpponentGuilModel { get; set; }

		public bool IsSaveTeams;
		public bool IsSaveGuild;
		public bool IsStartProcess;

		public bool IsGuildLoaded;
		public bool IsGuildStarted;
		public bool IsFirstTimeGuildLoad;

		public string GuildID;
		public string OpponentGuildID;

		public bool IsCustomGuild;
		public string CustomGuildID;

		public bool IsOpponentGuild; //переключатель своя / чужая ги, полностью переключается на чужую
		public bool IsOpponentMaps; //кнопка расчета защитников на своей / чужой территории. true - TeamsForEnemyMaps

		public bool IsViewEmblems;

		//UI
		public UILabel GuildName;
		public UIToggle OpponentsToggle;
		public UIToggle SwitchCustomGuild;

		//рассчитать оптимальные команды из 4х шаблонных команд
		public bool IsGenerateCustomTeams = false;

		public int count_Top_base = 20;
		public int count_preTop_base = 0;
		public bool middleIsPretop = false;
		public int lowest_location_level = 0;

		public GWTeamsManager gwManager;
		public List<GuildBattleTeamInfo> TeamsForEnemyMaps { get; set; } //наши у них на карте
		public List<GuildBattleTeamInfo> TeamsForOurMaps { get; set; } //они на нашей карте

		public List<UILabel> UiParticipantsDateLabels;
		public UILabel ParticipantLoadStatus;

		public bool IsBatchLoadGuilds;

		public Action ActionGuildChange;

		public void Reset()
		{
			IsGuildLoaded = false;
			IsGuildStarted = false;
			GuildModel = null;
			OpponentGuilModel = null;
			GuildID = "";
			OpponentGuildID = "";
			IsOpponentGuild = false;
		}

		private void Awake()
		{
			if (Instance != null)
			{
				DebugTWD.LogError("Multiple GWTeamUtils!");
				return;
			}
			Instance = this;
		}

		private void Update()
		{
			if (IsStartProcess)
			{
				IsStartProcess = false;
			}
		}

		public void SetTeamsForOurMaps()
		{
			DebugTWD.Log("SetTeamsForOurMaps");

			TeamsForOurMaps = gwManager.TeamsForOurMaps(GuildModel);
			if (!IsSaveTeams) return;

			string data = jsonSerializer.Serialize(TeamsForOurMaps);
			var EnemyGuild = GuildModel.GuildWarModel.CurrentBattle.EnemyGuildData;
			string guildABR = EnemyGuild.GuildName.Substring(0, 3) + '_' + EnemyGuild.GuildName.Last();
			var path = CommandHelper.GlobalPath + playerFolder + "TeamsForOurMaps_" + guildABR + ".txt";
			MyTools.SaveToFile(data, path, append: false);
			DebugTWD.Log("Guild current sectors saved");
		}

		public void SetTeamsForEnemyMaps()
		{
			DebugTWD.Log("SetTeamsForEnemyMaps");

			TeamsForOurMaps = gwManager.TeamsForOurMaps(OpponentGuilModel);

			if (!IsSaveTeams) return;

			string guildABR = GuildModel.Name.Substring(0, 3) + '_' + GuildModel.Name.Last();
			string data = jsonSerializer.Serialize(TeamsForEnemyMaps);
			var path = CommandHelper.GlobalPath + playerFolder + "TeamsForEnemyMaps_" + guildABR + ".txt";
			MyTools.SaveToFile(data, path, append: false);
			DebugTWD.Log("Guild opposite sectors saved");
		}

		public void SaveCustomTeamsOld()
		{
			IsGenerateCustomTeams = true;
			gwManager.InitData(false);

			var currentGuild = IsOpponentGuild ? OpponentGuilModel : GuildModel;
			var enemyData = currentGuild.GuildWarModel.CurrentBattle.EnemyGuildData;

			Dictionary<string, GuildBattleParticipantInfo> enemySnapshot = enemyData.PlayerInfoSnapshot;
			var snapshotSer = jsonSerializer.Serialize(enemySnapshot);
			enemyData.SetSnapshot(gwManager.GeneratedCustomTeams(enemySnapshot));

			gwManager.GWProtList.StartStopBattle();
			List<GuildBattleTeamInfo> CustomTeams = jsonSerializer.Deserialize<List<GuildBattleTeamInfo>>(gwManager.GWProtList.TeamsForMapsOpponenets[gwManager.CurrentGuildModel.Id]);
			IsGenerateCustomTeams = false;

			gwManager.SetDataToSectorNameLabels(CustomTeams);

			Dictionary<string, GuildBattleParticipantInfo> snapshotDeser;
			if (!IsSaveTeams)
			{
				snapshotDeser = jsonSerializer.Deserialize<Dictionary<string, GuildBattleParticipantInfo>>(snapshotSer);
				currentGuild.GuildWarModel.CurrentBattle.EnemyGuildData.SetSnapshot(snapshotDeser);
				gwManager.InitData(false);
				return;
			}

			string guildABR = GuildModel.Name.Substring(0, 3) + '_' + GuildModel.Name.Last();
			string data = jsonSerializer.Serialize(CustomTeams);

			snapshotDeser = jsonSerializer.Deserialize<Dictionary<string, GuildBattleParticipantInfo>>(snapshotSer);
			currentGuild.GuildWarModel.CurrentBattle.EnemyGuildData.SetSnapshot(snapshotDeser);
			gwManager.InitData(false);

			if (IsSaveTeams)
			{
				var path = CommandHelper.GlobalPath + playerFolder + "GW_CustomProtectors_of_" + guildABR + ".txt";
				MyTools.SaveToFile(data, path, append: false);
				DebugTWD.Log("Guild opposite sectors saved");
			}
		}

		public void SaveCustomTeams()
		{
			IsGenerateCustomTeams = true;
			GWTeamsManager.Instance.InitData(false);
			StartStopBattle();
			IsGenerateCustomTeams = false;
		}

		public void StartStopBattle()
		{
			var guildModel = GWTeamsManager.Instance.CurrentGuildModel;

			GuildBattleModel currentBattle = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle;
			string currentBattleSer = jsonSerializer.Serialize(currentBattle);

			Dictionary<string, GuildBattleParticipantInfo> enemySnapshot = GWTeamsManager.Instance.GwMatchMakingInfo.PlayerInfoSnapshot;
			string snapshotSer = jsonSerializer.Serialize(enemySnapshot);
			GWTeamsManager.Instance.GwMatchMakingInfo.SetSnapshot(GWTeamsManager.Instance.GeneratedCustomTeams(enemySnapshot));

			StartGWBattle.Instance.StartBattle(guildModel);

			var TeamsForMaps = GWTeamsManager.Instance.TeamsForOurMaps(guildModel);
			GWTeamsManager.Instance.SetDataToSectorNameLabels(TeamsForMaps);

			guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle = jsonSerializer.Deserialize<GuildBattleModel>(currentBattleSer);

			var snapshotDeser = jsonSerializer.Deserialize<Dictionary<string, GuildBattleParticipantInfo>>(snapshotSer);
			GWTeamsManager.Instance.GwMatchMakingInfo.SetSnapshot(snapshotDeser);
		}

		public void LoadGuildData(bool isOpponent)
		{
			IsGuildLoaded = false;
			IsGuildStarted = true;
			if (GameManager.Instance.IsConnectedToServer)
			{
				LoadDataOnline(id : isOpponent ? OpponentGuildID : GuildID); //354c053cba634e7ba9f7c30d2218cf7b
            }
			else
			{
				StartCoroutine(LoadDataOffline(id : isOpponent ? OpponentGuildID : GuildID));
			}
		}

		public void SaveCurrentGuild(GuildModel guild)
		{
			string data = jsonSerializer.Serialize(guild);
			string guildABR = guild.Name.Substring(0, 3) + '_' + guild.Name.Last();
			var path = CommandHelper.GlobalPath + playerFolder + "GuildModel_" + guildABR + ".txt";

			MyTools.SaveToFile(data, path, append: false);
			DebugTWD.Log($"Guild data {guild.Name} saved");
		}

		public IEnumerator LoadDataOffline(string id)
		{
			IsGuildLoaded = false;
			IsGuildStarted = true;

			if (string.IsNullOrEmpty(id) || id == CustomGuildID)
			{
				id = CustomGuildID;
				DebugTWD.Log("try load custom guild id " + id);
			}

			string cashGuildJson = ContentManager.Instance.GetCache("Guild").GetContentById<string>(id);
			if (cashGuildJson != null)
			{
				var partId = id + "_0";
				var isPartSaved = ContentManager.Instance.GetCache("GuildWarParticipant").HasContentById(partId);

				if (GuildModel == null)
				{
					GuildModel = jsonSerializer.DeserializeObject<GuildModel>(cashGuildJson);
					GuildModel.StartGroupChildren(DataManager.Instance.Player, DataManager.Instance.GameData);
					GuildID = GuildModel.Id;

                    if (!isPartSaved)
					{
						SaveGuildParticipants(GuildModel, 0);
					}
				}
				else
				{
					OpponentGuilModel = jsonSerializer.DeserializeObject<GuildModel>(cashGuildJson);
					OpponentGuilModel.StartGroupChildren(null, DataManager.Instance.GameData);

					if (!isPartSaved)
					{
						SaveGuildParticipants(OpponentGuilModel, 0);
					}
				}

				GuildLoadedCallback(false, id);

				if (IsBatchLoadGuilds && OpponentGuilModel == null)
				{
					OpponentGuildID = GuildModel.GuildWarModel.CurrentBattle.EnemyGuildData.GroupId;
					StartCoroutine(LoadDataOffline(OpponentGuildID));

					yield break;
				}

				IsGuildLoaded = true;
				IsGuildStarted = false;

				AwakeGuildWarManager();
			}
			else
			{
				CraftSettings.Instance.CheckInternetStatus();
				if (!OfflineManager.IsInternetOn)
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
					IsGuildLoaded = true;
					IsGuildStarted = false;
					yield break;
				}
				else
				{
					GetPlayerData.Instance.OnClickGetGuild();
					yield return new WaitUntil(() => GetPlayerData.Instance.waitingLogin);
					LoadDataOnline(id);
				}
			}
		}

		private void GuildLoadedCallback(bool online, string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				SetErrorGuildUI(online);
			}
			else
			{
				SetFindGuildUI(online, id); //cf91a18093114a83a7adb60034b9bfca
			}
		}

		public void LoadDataOnline(string id) //354c053cba634e7ba9f7c30d2218cf7b оппоненты
        {
			IsGuildLoaded = false;
			IsGuildStarted = true;

			if (string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(CustomGuildID))
			{
				id = CustomGuildID;
				DebugTWD.Log("try load custom guild id " + id);

				if (string.IsNullOrEmpty(id))
				{
					IsGuildLoaded = true;
					DataManager.Instance.guildPopup.gameObject.SetActive(true);
					DataManager.Instance.guildPopup.OpenForTab(3);
					return;
				}
			}

            //SignalRClient.Instance.OnSocialMessage -= OnSocialMessage;
            //SignalRClient.Instance.OnSocialMessage += OnSocialMessage;

            //var arg = "[\"" + id + "\"]"; //354c053cba634e7ba9f7c30d2218cf7b
            //SignalRClient.Instance.RequestCommand("LoadGroups", arg, null, waitForResponse: true);
			//var guild = await GetGuild(id);

            SignalRClient.Instance.RequestCommand("TryGetGroupInfo", id, OnGuildReceived, waitForResponse: true); //GetGroupInfo
        }


        public static async Task<GuildModel> GetGuild(string guildId)
        {
            TaskCompletionSource<GuildModel> completion = new TaskCompletionSource<GuildModel>();
            SignalRClient.Instance.RequestCommand("GetGroupInfo", guildId, delegate (string message)
            {
                if (string.IsNullOrEmpty(message) || message == "null")
                {
                    completion.SetResult(null);
                }
                else
                {
                    completion.SetResult(GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<GuildModel>(message));
                }
            }, waitForResponse: true);
            return await completion.Task;
        }

        private void SetErrorGuildUI(bool online)
		{
			var guildName = GuildModel == null ? "" : OpponentGuilModel == null ? GuildModel?.GuildWarModel.CurrentBattle.EnemyGuildData.GuildName : "";

			string log;
			if (DataManager.Instance.language != DataManager.Language.Ru)
			{
				log = "Guild Data " + guildName + " is null from " + (online ? "WEB" : "Cache");
			}
			else
			{
				log = "Данные гильдии " + guildName + " не найдены в " + (online ? "Сети" : "Кэше");
			}
			MyTools.UpdateLogPanel(log);
			DebugTWD.LogWarning(log, DebugType.Load);
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);

			AlertPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
			if (confirmationPopup != null)
			{
				confirmationPopup.SetContent("", log);
				confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
				confirmationPopup.SetCallbacks(delegate
				{
					confirmationPopup.Close();
				});
				confirmationPopup.Open();
			}
		}

		private void SetFindGuildUI(bool online, string id)
		{
			string log;
			var guildModel = !string.IsNullOrEmpty(id) && id == GuildModel.Id ? GuildModel : OpponentGuilModel != null ? OpponentGuilModel : GuildModel;
			string GuildTime = MyTools.LongToTime(guildModel.LastNewDayScoreTimeStamp);

			if (DataManager.Instance.language != DataManager.Language.Ru)
			{
				log = $"Guild Data {guildModel.Name} was loaded from " + (online ? "WEB" : "Cache") + "\nActual date is : " + GuildTime;
			}
			else
			{
				log = $"Данные гильдии {guildModel.Name} были загружены из " + (online ? "Сети" : "Кэша") + "\nАктуальная дата профиля : " + GuildTime;
			}
			MyTools.UpdateLogPanel(log);
			DebugTWD.Log(log);
		}

        public void OnSocialMessage(string message, string type)
		{
            if (type == "SocialGroupLoaded")
            {
                OnGuildReceived(message);
            }
        }

        private void OnGuildReceived(string message)
		{
			if (string.IsNullOrEmpty(message) || message == "null")
			{
				IsGuildLoaded = true;
				IsGuildStarted = false;
				SignalRClient.Instance.ClearError();
				GuildLoadedCallback(true, null);
				return;
			}

			string id;
			if (GuildModel == null)
			{
				GuildModel = jsonSerializer.DeserializeObject<GuildModel>(message);
				GuildModel.StartGroupChildren(DataManager.Instance.Player, DataManager.Instance.GameData);
				GuildID = GuildModel.Id;
				id = GuildID;

				ContentManager.Instance.GetCache("Guild").SetContent(GuildID, null, null, message);
				SaveGuildParticipants(GuildModel, 0);

				//DataManager.Instance.SaveGuildNameToSheet(id, GuildModel.Name);
				if (!IsFirstTimeGuildLoad)
				{
					IsFirstTimeGuildLoad = true;
					UserPrefsKeys.Player_GuildID = GuildID;
					UserPrefsKeys.Player_GuildName = GuildModel.Name;

					if (SupabaseManager.IsOnline)
					{
						DataManager.Instance.DatabaseManager.UpdateTWDAccountGuilds();
					}
				}
			}
			else
			{
				OpponentGuilModel = jsonSerializer.DeserializeObject<GuildModel>(message);
				OpponentGuilModel.StartGroupChildren(null, DataManager.Instance.GameData);

				ContentManager.Instance.GetCache("Guild").SetContent(OpponentGuilModel.Id, null, null, message);
				SaveGuildParticipants(OpponentGuilModel, 0);
				id = OpponentGuilModel.Id;
			}

			GuildLoadedCallback(true, id);

			if (IsBatchLoadGuilds && OpponentGuilModel == null)
			{
				OpponentGuildID = GuildModel.GuildWarModel.CurrentBattle.EnemyGuildData.GroupId;
				SignalRClient.Instance.RequestCommand("GetGroupInfo", OpponentGuildID, OnGuildReceived, waitForResponse: true);
				return;
			}

			IsGuildLoaded = true;
			IsGuildStarted = false;

			AwakeGuildWarManager();

			//DataManager.Instance.GuildManager = new GuildManager(DataManager.Instance.ModelManager);
		}
		//------------------

		public bool SetHomeGuild(string json)
		{
			if (string.IsNullOrEmpty(json) || GuildModel != null) return false;

            GuildModel = jsonSerializer.DeserializeObject<GuildModel>(json);
            GuildModel.StartGroupChildren(DataManager.Instance.Player, DataManager.Instance.GameData);
            GuildID = GuildModel.Id;
            ContentManager.Instance.GetCache("Guild").SetContent(GuildID, null, null, json);
            SaveGuildParticipants(GuildModel, 0);
            //DataManager.Instance.SaveGuildNameToSheet(GuildID, GuildModel.Name);
            GuildLoadedCallback(true, GuildID);

			if (!IsFirstTimeGuildLoad)
			{
				IsFirstTimeGuildLoad = true;
				UserPrefsKeys.Player_GuildID = GuildID;
				UserPrefsKeys.Player_GuildName = GuildModel.Name;
				if (SupabaseManager.IsOnline)
				{
					DataManager.Instance.DatabaseManager.UpdateTWDAccountGuilds();
				}
			}

			IsGuildLoaded = true;
            IsGuildStarted = false;

            AwakeGuildWarManager();
			return true;
        }

        private void AwakeGuildWarManager()
		{
			if (DataManager.Instance.Player.IsGuildMember)
			{
				DataManager.Instance.NotifyLoadCompleted();
				if (SingularityMonoBehaviour<GuildWarManager>.Instance != null)
				{
					SingularityMonoBehaviour<GuildWarManager>.Instance.OnLoadCompleted();
				}
			}
		}

		public void SaveGuildParticipants(GuildModel guild, int saveNumber)
		{
			if (guild == null) return;

			var participantsList = new List<GuildBattleMatchmakingInfoData>();

			var participantsListItem1 = new GuildBattleMatchmakingInfoData(guild.GuildBattleMatchmakingInfo);
			var participantsListItem2 = new GuildBattleMatchmakingInfoData(guild.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData);

			var timeStamp = DateTime.Now.ToUniversalTime().TotalMilliseconds();
			participantsListItem1.CurrentBattleStartTime = timeStamp;
			participantsListItem2.CurrentBattleStartTime = timeStamp;

			participantsList.Add(participantsListItem1);
			participantsList.Add(participantsListItem2);

			string participantsListData = jsonSerializer.Serialize(participantsList);
			if (!string.IsNullOrEmpty(participantsListData))
			{
				participantsList.Clear();
				var id = guild.Id + "_" + saveNumber;
				DebugTWD.Log("Save part " + id + " " + guild.Name);
				var cash = ContentManager.Instance.GetCache("GuildWarParticipant");
				if (cash.HasContentById(id))
				{
					cash.DeleteContentWithId(id);
				}
				cash.SetContent(guild.Id + "_" + saveNumber, null, null, participantsListData, timeStamp);
				SetUIParticipantsDates(saveNumber);
				DebugTWD.Log("сохранили защитников в слот " + saveNumber);
				StartCoroutine(SetParticipantsStatusUI("Сохранили"));
			}
			else StartCoroutine(SetParticipantsStatusUI("Ошибка сохранения"));
		}

		private void SetUIParticipantsDates(int saveNumber = -1)
		{
			for (int i = 0; i < UiParticipantsDateLabels.Count; i++)
			{
				var toolTip = UiParticipantsDateLabels[i].GetComponent<ShowTooltip>();
				if (ContentManager.Instance.GetCache("GuildWarParticipant").HasContentByIndex(i, out long timeStamp, out string id))
				{
					UiParticipantsDateLabels[i].color = saveNumber == i ? Color.green : Color.white;
					UiParticipantsDateLabels[i].text = MyTools.LongToTime(timeStamp);
					if (toolTip != null)
					{
						GuildModel guild = null;
						if (GuildModel.Id == id)
						{
							guild = GuildModel;
						}
						else if (OpponentGuilModel != null && OpponentGuilModel.Id == id)
						{
							guild = OpponentGuilModel;
						}

						if (guild != null) { toolTip.enabled = true; toolTip.EnCustomText = toolTip.RuCustomText = guild.Name; }
						else toolTip.enabled = false;
					}
				}
				else
				{
					UiParticipantsDateLabels[i].color = Color.white;
					UiParticipantsDateLabels[i].text = "";
				}
			}
		}
		public void LoadGuildParticipants(ref GuildModel guild, int saveNumber)
		{
			//0 - for autosave
			//1 - save1
			//2 - save2
			var id = guild.Id + "_" + saveNumber;
			DebugTWD.Log("Try load part " + guild.Id + "_" + saveNumber + " " + guild.Name);

			var cash = ContentManager.Instance.GetCache("GuildWarParticipant");
			if (!cash.HasContentById(id))
			{
				StartCoroutine(SetParticipantsStatusUI("Нет сохранения"));
				return;
			}
			string cashGuildParticipantsJson = ContentManager.Instance.GetCache("GuildWarParticipant").GetContentById<string>(id);
			var listParticopantsData = jsonSerializer.Deserialize<List<GuildBattleMatchmakingInfoData>>(cashGuildParticipantsJson);

			guild.GuildBattleMatchmakingInfo = listParticopantsData[0].guildBattleMatchmakingInfo;
			guild.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData = listParticopantsData[1].guildBattleMatchmakingInfo;

			SetUIParticipantsDates(saveNumber);

			DebugTWD.Log("загрузили защитников в слот" + saveNumber);

			gwManager.GWProtList.IsIndexChanged = true;
			StartCoroutine(SetParticipantsStatusUI("Загрузили"));
		}
		public void SaveParticipants(UIButtonToggle tg)
		{
			int number = int.Parse(tg.name.Split('.')[1]);

			var guildModel = IsOpponentGuild ? OpponentGuilModel : GuildModel;
			if (guildModel == null) return;
			SaveGuildParticipants(guildModel, number);
		}

		public void LoadParticipants(UIButtonToggle tg)
		{
			int number = int.Parse(tg.name.Split('.')[1]);

			var guildModel = IsOpponentGuild ? OpponentGuilModel : GuildModel;
			if (guildModel == null)
			{
				StartCoroutine(SetParticipantsStatusUI("Гильдия не загружена"));
				return;
			}
			LoadGuildParticipants(ref guildModel, number);
		}

		private IEnumerator SetParticipantsStatusUI(string status)
		{
			ParticipantLoadStatus.gameObject.SetActive(true);
			ParticipantLoadStatus.text = status;
			yield return new WaitForSeconds(2);
			ParticipantLoadStatus.gameObject.SetActive(false);
		}

		#region temp
		//-------------
		//распределение врагов на нашей карте
		public void SaveTeamsCurrentMap(GuildModel guildModel)
		{
			GuildBattleModel battleModel = guildModel.GuildWarModel.CurrentBattle;

			string file = Resources.Load<TextAsset>("Config/PvpTeamsIndexPerMission").text;
			Dictionary<string, string> PvpTeamsIndexPerMission = jsonSerializer.Deserialize<Dictionary<string, string>>(file);

			Dictionary<string, string> dic = battleModel.CurrentMapModel.PvpTeamsIndexPerMission;
			List<GuildBattleTeamInfo> guildBattlePlayerInfoList = new List<GuildBattleTeamInfo>();

			var enemyData = battleModel.EnemyGuildData;
			var enemyInfo = enemyData.PlayerInfoSnapshot;

			if (IsSaveTeams)
			{
				string data2 = jsonSerializer.Serialize(enemyInfo);
				var path2 = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "1.PlayerInfoSnapshot" + ".json";
				MyTools.SaveToFile(data2, path2, append: false);
				DebugTWD.Log("1.PlayerInfoSnapshot saved");
			}

			var origin = guildModel.GuildWarModel.NextBattlesOpponentMatchmakingInfo;
			if (origin != null && origin.Count > 0)
			{
				var OpponentMatchmakingInfoData = origin[0].OpponentMatchmakingInfo;
				if (!string.IsNullOrEmpty(OpponentMatchmakingInfoData))
				{
					GuildBattleMatchmakingInfo matchmakingInfoDeserialized = jsonSerializer.DeserializeObject<GuildBattleMatchmakingInfo>(OpponentMatchmakingInfoData);
					enemyData = matchmakingInfoDeserialized;
					enemyInfo = matchmakingInfoDeserialized.PlayerInfoSnapshot;

					if (IsSaveTeams)
					{
						string data2 = jsonSerializer.Serialize(enemyInfo);
						var path2 = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "1.PlayerInfoSnapshot_origin" + ".json";
						MyTools.SaveToFile(data2, path2, append: false);
						DebugTWD.Log("1.PlayerInfoSnapshot_origin saved");
					}
				}
			}

			List<GuildBattlePvpTeam> allTeamsEnemySorted = GetAllTeamsSorted(enemyInfo);
			Dictionary<int, List<GuildBattlePvpTeam>> _PVPTeamsListPerSector = PVPTeamsListPerSector(guildModel, allTeamsEnemySorted);
			DebugTWD.Log("SetupMissionAndPvpPlacement");
			SetupMissionAndPvpPlacement(guildModel);

			foreach (var pair in dic)
			{
				GuildBattleMapModel.ParsePvpTeamIndexId(pair.Value, out int sectorId, out int index);
				GuildBattlePvpTeam pvpTeamForMission = _PVPTeamsListPerSector[sectorId][index];
				var survivors = enemyInfo[pvpTeamForMission.OwnerHashedPlayerId].SelectedSurvivors;

				SurvivorMockData survivor = null;
				foreach (var s in survivors)
				{
					foreach (var s2 in pvpTeamForMission.Survivors)
					{
						if (s.Name == s2.Name)
						{
							survivor = s;
							break;
						}
					}
				}
				if (survivor == null) continue;

				int teamIndex = survivors.IndexOf(survivor);
				GuildBattleParticipantInfo currentGuildBattlePlayerInfo = battleModel.GetCurrentGuildBattlePlayerInfo(pvpTeamForMission);

				string name = currentGuildBattlePlayerInfo != null ? currentGuildBattlePlayerInfo.Name : "";

				GuildBattleTeamInfo item = guildBattlePlayerInfoList.FirstOrDefault(x => x.Team == pvpTeamForMission.Survivors);
				PvpTeamsIndexPerMission.TryGetValue(pair.Key, out string mapID);
				if (item == null)
				{
					item = new GuildBattleTeamInfo();
					item.Name = name;
					item.HashID = pvpTeamForMission.OwnerHashedPlayerId;
					item.AdjustedLevel = pvpTeamForMission.AverageAdjustedLevel;
					item.Team = pvpTeamForMission.Survivors;
					item.TeamIndex = TeamIndex(teamIndex);
					item.SectorNames = new List<string> { mapID };
					guildBattlePlayerInfoList.Add(item);
				}
				else
				{
					item.SectorNames.Add(mapID);
				}
			}

			foreach (var team in guildBattlePlayerInfoList)
			{
				List<List<string>> list = guildBattlePlayerInfoList.Where(x => x.HashID == team.HashID).Select(x => x.SectorNames).ToList();
				var sectorsAll = new List<string>();
				int SectorLevel = 0;
				foreach (var l in list)
				{
					foreach (var s in l)
					{
						SectorLevel += TeamLevel(s);
					}
					sectorsAll.AddRange(l);
				}
				team.SectorNamesAll = sectorsAll;
				team.SectorLevel = SectorLevel;
			}

			guildBattlePlayerInfoList.StableSort((GuildBattleTeamInfo a, GuildBattleTeamInfo b) => a.SectorLevel.CompareTo(b.SectorLevel));

			string data = jsonSerializer.Serialize(guildBattlePlayerInfoList);
			string guildABR = enemyData.GuildName.Substring(0, 3) + '_' + enemyData.GuildName.Last();
			var path = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "GW_Sectors_for_" + guildABR + ".txt";
			MyTools.SaveToFile(data, path, append: false);
			DebugTWD.Log("Guild current sectors saved");
		}

		//распределение союзников на вражеской карте
		public void SaveTeamsOppositeMap(GuildModel guildModel)
		{
			string guildABR = guildModel.Name.Substring(0, 3) + '_' + guildModel.Name.Last();

			var file = Resources.Load<TextAsset>("Config/PvpTeamsIndexPerMission").text;
			Dictionary<string, string> PvpTeamsIndexPerMission = jsonSerializer.Deserialize<Dictionary<string, string>>(file);
			Dictionary<string, GuildBattleParticipantInfo> bbInfo = guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot;

			var dicBbInfoIndex = new Dictionary<string, int>();
			int BbIndex = 0;
			foreach (var b in bbInfo.Keys)
			{
				dicBbInfoIndex.Add(b, BbIndex);
				BbIndex++;
			}
			List<GuildBattlePvpTeam> allTeamsSorted = GetAllTeamsSorted(bbInfo);

			if (IsSaveTeams)
			{
				string data2 = jsonSerializer.Serialize(bbInfo);
				var path2 = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "1.PlayerInfoSnapshot" + ".json";
				MyTools.SaveToFile(data2, path2, append: false);
				DebugTWD.Log("1.PlayerInfoSnapshot saved");
			}

			//
			if (IsGenerateCustomTeams)
			{
				int count = allTeamsSorted.Count;

				var guildBattlePvpTeam_Top = allTeamsSorted[count - 1]; //самые крутые
				var guildBattlePvpTeam_preTop = allTeamsSorted[count - 10]; //менее крутые
				var guildBattlePvpTeam_Middle = middleIsPretop ? allTeamsSorted[count - 10] : allTeamsSorted[10]; //чуть лучше бомжей
				var guildBattlePvpTeam_Low = allTeamsSorted[0]; //бомжи

				int count_Top_current = 0;
				int count_preTop_current = 0;

				foreach (var survivor in bbInfo)
				{
					List<SurvivorMockData> selectedSurvivors = survivor.Value.SelectedSurvivors;
					if (selectedSurvivors.Count >= 9)
					{
						List<SurvivorMockData> team_A;
						List<SurvivorMockData> team_B;
						List<SurvivorMockData> team_C;

						if (count_Top_current < count_Top_base)
						{
							count_Top_current++;
							team_A = guildBattlePvpTeam_Top.Survivors;

							if (count_preTop_current < count_preTop_base)
							{
								count_preTop_current++;
								team_B = guildBattlePvpTeam_preTop.Survivors;
							}
							else
							{
								team_B = guildBattlePvpTeam_Middle.Survivors;
							}
						}
						else
						{
							if (count_preTop_current < count_preTop_base)
							{
								count_preTop_current++;
								team_A = guildBattlePvpTeam_preTop.Survivors;
							}
							else
							{
								team_A = guildBattlePvpTeam_Middle.Survivors;
							}
							team_B = guildBattlePvpTeam_Middle.Survivors;
						}
						team_C = guildBattlePvpTeam_Low.Survivors;

						var selectedSurvivorsCustom = new List<SurvivorMockData>
						{
							team_A[0], team_A[1], team_A[2],
							team_B[0], team_B[1], team_B[2],
							team_C[0], team_C[1], team_C[2]
						};
						survivor.Value.SelectedSurvivors = selectedSurvivorsCustom;
					}
				}
			}
			//
			Dictionary<int, List<GuildBattlePvpTeam>> _PVPTeamsListPerSector = PVPTeamsListPerSector(guildModel, allTeamsSorted);
			SetupMissionAndPvpPlacement(guildModel);

			if (IsSaveTeams)
			{
				string data2 = jsonSerializer.Serialize(_PVPTeamsListPerSector);
				var path2 = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "4.PVPTeamsListPerSector_after" + ".json";
				MyTools.SaveToFile(data2, path2, append: false);
				DebugTWD.Log("4.PVPTeamsListPerSector_after saved");
			}

			List<GuildBattleTeamInfo> GuildBattlePlayerInfoList = new List<GuildBattleTeamInfo>();
			Dictionary<string, string> dic = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel.PvpTeamsIndexPerMission;

			foreach (var pair in dic)
			{
				GuildBattleMapModel.ParsePvpTeamIndexId(pair.Value, out int sectorId, out int index);
				GuildBattlePvpTeam pvpTeamForMission = _PVPTeamsListPerSector[sectorId][index];
				var survivors = bbInfo[pvpTeamForMission.OwnerHashedPlayerId].SelectedSurvivors;
				SurvivorMockData survivor = null;
				foreach (var s in survivors)
				{
					foreach (var s2 in pvpTeamForMission.Survivors)
					{
						if (s.Name == s2.Name)
						{
							survivor = s;
							break;
						}
					}
				}

				GuildBattleTeamInfo item = GuildBattlePlayerInfoList.FirstOrDefault(x => x.Team == pvpTeamForMission.Survivors);
				PvpTeamsIndexPerMission.TryGetValue(pair.Key, out string mapID);
				var id = pvpTeamForMission.OwnerHashedPlayerId;

				if (item == null)
				{
					item = new GuildBattleTeamInfo();
					item.Name = bbInfo[id].Name;
					item.HashID = id;
					item.AdjustedLevel = pvpTeamForMission.AverageAdjustedLevel;
					item.Team = pvpTeamForMission.Survivors;
					item.TeamIndex = TeamIndex(survivors.IndexOf(survivor));
					item.SectorNames = new List<string> { mapID };
					item.BasePlayerIndex = dicBbInfoIndex[id];
					item.TeamSortedPlayerIndex = allTeamsSorted.IndexOf(allTeamsSorted.FirstOrDefault(x => x.OwnerHashedPlayerId ==
						id && x.Survivors == pvpTeamForMission.Survivors));

					item.GuildJoinedDate = MyTools.LongToTime(guildModel.GetMemberInfo(id).GuildJoinedDate);

					GuildBattlePlayerInfoList.Add(item);
				}
				else
				{
					item.SectorNames.Add(mapID);
				}
			}

			if (GuildBattlePlayerInfoList.Count < allTeamsSorted.Count)
			{
				List<GuildBattleTeamInfo> GuildBattlePlayerInfoListEmpty = new List<GuildBattleTeamInfo>();
				foreach (var team in allTeamsSorted)
				{
					var id = team.OwnerHashedPlayerId;

					if (GuildBattlePlayerInfoList.FirstOrDefault(x => x.Team == team.Survivors) == null)
					{
						var itemEmpty = new GuildBattleTeamInfo();
						itemEmpty.Name = bbInfo[id].Name;
						itemEmpty.HashID = id;
						itemEmpty.AdjustedLevel = team.AverageAdjustedLevel;
						itemEmpty.Team = team.Survivors;
						var survivors = bbInfo[id].SelectedSurvivors;

						itemEmpty.TeamIndex = TeamIndex(survivors.IndexOf(team.Survivors.First()));
						itemEmpty.SectorNames = new List<string> { };
						itemEmpty.BasePlayerIndex = dicBbInfoIndex[id];
						itemEmpty.TeamSortedPlayerIndex = allTeamsSorted.IndexOf(allTeamsSorted.FirstOrDefault(x => x.OwnerHashedPlayerId ==
								id && x.Survivors == team.Survivors));

						itemEmpty.GuildJoinedDate = MyTools.LongToTime(guildModel.GetMemberInfo(id).GuildJoinedDate);

						GuildBattlePlayerInfoListEmpty.Add(itemEmpty);
					}
				}
				GuildBattlePlayerInfoList.AddRange(GuildBattlePlayerInfoListEmpty);
			}

			if (IsSaveTeams)
			{
				string data = jsonSerializer.Serialize(GuildBattlePlayerInfoList);
				var path = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "5.GW_Sectors_for_" + guildABR + "before.json";
				MyTools.SaveToFile(data, path, append: false);
				DebugTWD.Log("5.GuildBattlePlayerInfoList_before saved");
			}

			foreach (var team in GuildBattlePlayerInfoList)
			{
				List<List<string>> list;
				if (IsGenerateCustomTeams)
				{
					list = GuildBattlePlayerInfoList.Where(x => x.AdjustedLevel == team.AdjustedLevel).Select(x => x.SectorNames).ToList();
				}
				else
				{
					list = GuildBattlePlayerInfoList.Where(x => x.HashID == team.HashID).Select(x => x.SectorNames).ToList();
				}

				var sectorsAll = new List<string>();
				var sectorLevels = new List<int>();

				int SectorLevel = 0;
				foreach (var l in list)
				{
					foreach (var s in l)
					{
						var teamLevel = TeamLevel(s);
						SectorLevel += teamLevel;
						sectorLevels.Add(teamLevel);
					}
					sectorsAll.AddRange(l);
				}

				var temp = new List<string>();

				foreach (var s in sectorsAll)
				{
					temp.Add(s);
				}
				sectorsAll.StableSort((string a, string b) => sectorLevels[temp.IndexOf(a)].CompareTo(sectorLevels[temp.IndexOf(b)]));
				team.SectorNamesAll = sectorsAll;
				team.SectorLevel = SectorLevel;
			}

			List<GuildBattleTeamInfo> GuildBattlePlayerInfoClean = null;
			if (IsGenerateCustomTeams)
			{
				GuildBattlePlayerInfoClean = new List<GuildBattleTeamInfo>();

				foreach (var team in GuildBattlePlayerInfoList)
				{
					if (team.AdjustedLevel == 0) continue;
					var item = GuildBattlePlayerInfoClean.FirstOrDefault(x => x.AdjustedLevel == team.AdjustedLevel);
					if (item == null) GuildBattlePlayerInfoClean.Add(team);
				}

				for (int i = 0; i < GuildBattlePlayerInfoClean.Count; i++)
				{
					var locations = GuildBattlePlayerInfoClean[i].SectorNamesAll.Where(x => TeamLevel(x) >= lowest_location_level).ToList();
					GuildBattlePlayerInfoClean[i].SectorNamesAll = locations;
					DebugTWD.Log("team.SectorNames " + locations.Count + " for " + GuildBattlePlayerInfoClean[i].AdjustedLevel);
				}

				GuildBattlePlayerInfoClean.StableSort((GuildBattleTeamInfo a, GuildBattleTeamInfo b) => a.TeamSortedPlayerIndex.CompareTo(b.TeamSortedPlayerIndex));
			}
			else
			{
				GuildBattlePlayerInfoList.StableSort((GuildBattleTeamInfo a, GuildBattleTeamInfo b) => a.TeamSortedPlayerIndex.CompareTo(b.TeamSortedPlayerIndex));
				GuildBattlePlayerInfoClean = GuildBattlePlayerInfoList;
			}

			if (IsSaveTeams)
			{
				string data = jsonSerializer.Serialize(GuildBattlePlayerInfoClean);
				var path = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "6.GW_Sectors_for_" + guildABR + "after.json";
				MyTools.SaveToFile(data, path, append: false);
				DebugTWD.Log("6.Guild opposite sectors saved");
			}
		}

		public void SetupMissionAndPvpPlacement(GuildModel guildModel)
		{
			var mapModel = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel;

			for (int i = 0; i < mapModel.Sectors.Count; i++)
			{
				SetupArea(mapModel.Sectors[i]);
			}
		}
		public void SetupArea(GuildBattleMapSectorModel sector)
		{
			var guildModel = IsOpponentGuild ? OpponentGuilModel : GuildModel;
			var CurrentMapModel = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel;
			if (sector.StartIndexPerArea == null)
			{
				return;
			}
			if (sector.MissionSectorDefinition == null)
			{
				return;
			}
			sector.StartIndexPerArea.Clear();
			if (sector.StartIndexPerArea.Count == 0)
			{
				int num = 0;
				for (int i = 0; i < 4; i++)
				{
					num = ((i != 0) ? (i * sector.MissionSectorDefinition.MissionAmountPerPVPEnemy) : 0);
					sector.StartIndexPerArea.Add(new List<int>());
					for (int j = num; j < num + sector.MissionSectorDefinition.MissionAmountPerPVPEnemy; j++)
					{
						GuildBattleMapMissionModel missionModel = GetMissionModel(sector, GuildBattleMapMissionModel.GenerateId(sector.MissionPoolName, sector.SectorId, j));
						missionModel.AreaIndex = i;
						CurrentMapModel.TryAssignPvpTeamForMission(missionModel);
					}
					sector.StartIndexPerArea[i].Add(num);
				}
				sector.CurrentBatchIndex = num;
			}
			sector.UpdateAreaMissionsLists();
		}

		public GuildBattleMapMissionModel GetMissionModel(GuildBattleMapSectorModel sector, string uniqueMissionId)
		{
			for (int i = 0; i < sector.RandomizedMissions.Count; i++)
			{
				GuildBattleMapMissionModel guildBattleMapMissionModel = sector.RandomizedMissions[i];
				if (guildBattleMapMissionModel.Id == uniqueMissionId)
				{
					return guildBattleMapMissionModel;
				}
			}
			return null;
		}

		public List<GuildBattlePvpTeam> GetAllTeamsSorted(Dictionary<string, GuildBattleParticipantInfo> players)
		{
			DebugTWD.Log("GetAllTeamsSorted");
			List<GuildBattlePvpTeam> list = new List<GuildBattlePvpTeam>();
			foreach (var key in players.Keys)
			{
				var value = players[key];
				List<SurvivorMockData> selectedSurvivors = value.SelectedSurvivors;
				if (selectedSurvivors.Count >= 9)
				{
					foreach (var surv in selectedSurvivors)
					{
						surv.OwnerHashedPlayerId = key;
					}

					GuildBattlePvpTeam item = new GuildBattlePvpTeam(selectedSurvivors.GetRange(0, 3));
					GuildBattlePvpTeam item2 = new GuildBattlePvpTeam(selectedSurvivors.GetRange(3, 3));
					GuildBattlePvpTeam item3 = new GuildBattlePvpTeam(selectedSurvivors.GetRange(6, 3));
					list.Add(item);
					list.Add(item2);
					list.Add(item3);
				}
			}

			list.StableSort((GuildBattlePvpTeam a, GuildBattlePvpTeam b) => a.AverageAdjustedLevel.CompareTo(b.AverageAdjustedLevel));
			return list;
		}

		public Dictionary<int, List<GuildBattlePvpTeam>> PVPTeamsListPerSector(GuildModel guildModel, List<GuildBattlePvpTeam> allTeamsSorted)
		{
			var CurrentMapModel = guildModel.GuildWarModel.CurrentBattle.CurrentMapModel;
			var _PVPTeamsListPerSector = new Dictionary<int, List<GuildBattlePvpTeam>>();
			GuildWarDefinition warDefinition = GameData.FindGuildWarWithId(CurrentMapModel.WarDefinitionId);
			if (warDefinition == null) return null;

			if (IsSaveTeams)
			{
				string data = jsonSerializer.Serialize(allTeamsSorted);
				var path = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "2.AllTeamsSorted" + ".json";
				MyTools.SaveToFile(data, path, append: false);
				DebugTWD.Log("2.AllTeamsSorted saved");
			}

			List<GuildBattleSectorDefinition> list = GameData.GuildBattleSectorDefinitions.Where((GuildBattleSectorDefinition x) => warDefinition.SectorsIds.Contains(x.Id)).ToList();
			int minVp = list.Min((GuildBattleSectorDefinition x) => x.SectorVP);
			int maxVp = list.Max((GuildBattleSectorDefinition x) => x.SectorVP);
			foreach (GuildBattleSectorDefinition item in list)
			{
				if (!_PVPTeamsListPerSector.TryGetValue(item.Id, out var value))
				{
					value = new List<GuildBattlePvpTeam>();
					GuildBattleMapSectorModel sectorModel = CurrentMapModel.GetSectorModel(item.Id);
					if (sectorModel == null) continue;

					int amountOfTeamsNeeded = sectorModel.RandomizedMissions.Count((GuildBattleMapMissionModel x) => x.Type == GuildBattleMapMissionModel.MissionType.PVP);
					List<GuildBattlePvpTeam> pvpTeamsForSector = GetPvpTeamsForSector(item.SectorVP, minVp, maxVp, allTeamsSorted, amountOfTeamsNeeded);
					value.AddRange(pvpTeamsForSector);
					value?.StableSort((GuildBattlePvpTeam a, GuildBattlePvpTeam b) => a.AverageAdjustedLevel.CompareTo(b.AverageAdjustedLevel));

					_PVPTeamsListPerSector.Add(item.Id, value);
				}
			}

			if (IsSaveTeams)
			{
				string data = jsonSerializer.Serialize(_PVPTeamsListPerSector);
				var path = CommandHelper.GlobalPath + CommandHelper.PlayerFolder + "3.PVPTeamsListPerSector_before" + ".json";
				MyTools.SaveToFile(data, path, append: false);
				DebugTWD.Log("3.PVPTeamsListPerSector_before saved");
			}

			foreach (KeyValuePair<string, string> item in CurrentMapModel.PvpTeamsIndexPerMission)
			{
				GuildBattleMapModel.ParsePvpTeamIndexId(item.Value, out var sectorId, out var index);
				if (_PVPTeamsListPerSector.TryGetValue(sectorId, out var value) && value.Count > index)
				{
					value[index].MissionId = item.Key;
				}
			}

			return _PVPTeamsListPerSector;
		}

		private List<GuildBattlePvpTeam> GetPvpTeamsForSector(int sectorVp, int minVp, int maxVp, List<GuildBattlePvpTeam> allTeams, int amountOfTeamsNeeded)
		{
			List<GuildBattlePvpTeam> list = new List<GuildBattlePvpTeam>();

			if (amountOfTeamsNeeded == 0 || allTeams.Count == 0)
			{
				return list;
			}

			if (amountOfTeamsNeeded >= allTeams.Count)
			{
				int num = 0;
				while (list.Count != amountOfTeamsNeeded)
				{
					var team = new GuildBattlePvpTeam(allTeams[num % allTeams.Count].Survivors);
					//team.numIndex = num % allTeams.Count;
					list.Add(team);
					num++;
				}
				return list;
			}

			FixedPoint fixedPoint = sectorVp - minVp;
			FixedPoint fixedPoint2 = maxVp - minVp;
			FixedPoint fixedPoint3 = fixedPoint / fixedPoint2;
			int num2 = (int)Math.Floor((double)(allTeams.Count - 1) * (double)fixedPoint3);
			int num3 = num2 + amountOfTeamsNeeded;
			int num4 = num3 - allTeams.Count;
			if (num4 > 0)
			{
				num2 -= num4;
				num3 -= num4;
			}

			if (num2 < 0)
			{
				num2 = 0;
				num3 = allTeams.Count;
			}

			if (num3 > allTeams.Count)
			{
				num3 = allTeams.Count;
			}

			for (int i = num2; i < num3; i++)
			{
				var team = new GuildBattlePvpTeam(allTeams[i].Survivors);
				//team.numIndex = i;
				list.Add(team);
			}
			return list;
		}
		#endregion
		//------------------

		public static string MapName(int index, bool isC)
		{
			if (!isC)
			{
				switch (index)
				{
					case 0: return "1.1";
					case 1: return "1.2";
					case 2: return "1.pvp";
					case 3: return "2.1";
					case 4: return "2.2";
					case 5: return "2.pvp";
					case 6: return "3.1";
					case 7: return "3.2";
					case 8: return "3.pvp";
					case 9: return "4.1";
					case 10: return "4.2";
					case 11: return "4.pvp";
					default: return null;
				}
			}
			else
			{
				switch (index)
				{
					case 0: return "1.1";
					case 1: return "1.pvp";
					case 2: return "2.1";
					case 3: return "2.pvp";
					case 4: return "3.1";
					case 5: return "3.pvp";
					case 6: return "4.1";
					case 7: return "4.pvp";
					default: return null;
				}
			}
		}

		public static string TeamIndex(int index)
		{
			switch (index)
			{
				case 0: return "A";
				case 1: return "A";
				case 2: return "A";
				case 3: return "B";
				case 4: return "B";
				case 5: return "B";
				case 6: return "C";
				case 7: return "C";
				case 8: return "C";
				default: return null;
			}
		}
		public static string TeamIndexFromTg(int index)
		{
			switch (index)
			{
				case 0: return "A";
				case 1: return "B";
				case 2: return "C";
				default: return "A";
			}
		}
		public static int GetTeamIndex(string indexSign)
		{
			switch (indexSign)
			{
				case "A": return 0;
				case "B": return 1;
				case "C": return 2;
				default: return 0;
			}
		}

		public static List<SurvivorMockData> GetPvpTeam(List<SurvivorMockData> GetPvpTeam, int index)
		{
			switch (index)
			{
				case 0: return GetPvpTeam.GetRange(0, 3);
				case 1: return GetPvpTeam.GetRange(3, 3);
				case 2: return GetPvpTeam.GetRange(6, 3);
				default: return null;
			}
		}
		public static int TeamLevel(string sectorID)
		{
			string sector = sectorID.Substring(0, sectorID.Length - 2);

			switch (sector)
			{
				case "bonus": return 0;
				case "1": return 1;
				case "6": return 2;
				case "13": return 3;
				case "2": return 4;
				case "7": return 5;
				case "14": return 6;
				case "3": return 7;
				case "8": return 8;
				case "15": return 9;
				case "4": return 10;
				case "9": return 11;
				case "16": return 12;
				case "5": return 13;
				case "10": return 14;
				case "17": return 15;
				case "11": return 16;
				case "18": return 17;
				case "12": return 18;
				case "19": return 19;
				case "20": return 20;
				default: return 0;
			}
		}

		public void ChangeGuildModelBase(bool isOpponent)
		{
			OpponentsToggle.value = isOpponent;
			if (!isOpponent)
			{
				var tween = OpponentsToggle.activeSprite.GetComponent<TweenAlpha>();
				tween.to = 0;
				tween.PlayForward();
			}
			ChangeGuildModel(OpponentsToggle);
		}

		[ContextMenu("SetOpponentsToggleValue")]
		public void SetOpponentsToggleValue()
		{
			OpponentsToggle.value = false;
			var tween = OpponentsToggle.activeSprite.GetComponent<TweenAlpha>();
			tween.to = 0;
			tween.PlayForward();
		}

		public void ChangeGuildModel(UIToggle tg)
		{
			bool isOpponent = tg.value;
			int guildTabIndex = 6;
			ResidencePopup.Instance.ActivateTab(guildTabIndex, false);
			GWTeamsManager.Instance.GWProtList.TeamsForMaps = null;

			if (isOpponent)
			{
				if (GuildModel == null) return;

				IsOpponentGuild = true;
				OpponentGuildID = GuildModel.GuildWarModel.CurrentBattle.EnemyGuildData.GroupId;
			}
			else
			{
				IsOpponentGuild = false;
			}
			StartCoroutine(ResidencePopup.Instance.OpenGuildTab(guildTabIndex, guildTabIndex, false));
			ActionGuildChange?.Invoke();
		}

		public void SwitchToCustomGuild(UIToggle tg)
		{
			if (IsGuildStarted) return;

			if (tg.value == IsCustomGuild && IsCustomGuild == false) return;

			IsCustomGuild = tg.value;

			StopAllCoroutines();
			DebugTWD.Log(tg.value ? "Загружаем чужую гильдию" : "Загружаем свою гильдию");

			bool isCustomAvailable = PlayerPrefs.HasKey("CustomGuild");

			if (!isCustomAvailable)
			{
				tg.value = false;
				return;
			}

			if (tg.value)
			{
				var custom = PlayerPrefs.GetString("CustomGuild");

				if (GuildID != custom && OpponentGuildID != custom && !string.IsNullOrEmpty(custom)) //"cf91a18093114a83a7adb60034b9bfca" //"7a4af41da3f64496ac7f8534670af0bd"
                {
					CustomGuildID = custom; //"8a1635b748c34b6ea79d9a9abc003571"
                }
				else
				{
					CustomGuildID = "";
					tg.value = false;
					return;
				}
			}
			else
			{
				CustomGuildID = "";
			}

			string originId = DataManager.Instance.Player.GuildId;
            string id = string.IsNullOrEmpty(CustomGuildID) && !string.IsNullOrEmpty(originId) ? originId : CustomGuildID;

			if (!string.IsNullOrEmpty(id))
			{
				Reset();
				GuildID = id;
				LoadGuildData(false);
				StartCoroutine(LoadGuildFromToggle());
			}
		}

		private IEnumerator LoadGuildFromToggle()
		{
			yield return new WaitUntil(() => IsGuildLoaded);

			if (DataManager.Instance.guildPopup.SelectedTab != 0)
				DataManager.Instance.guildPopup.OpenForTab(0);
			else
			{
				DataManager.Instance.guildPopup.OpenForTab(1);
				DataManager.Instance.guildPopup.OpenForTab(0);
			}
			GuildName.text = CurrentGuildModel.Name;
		}

		public static int GetAdjustedLevel(SurvivorModel survivor)
		{
			int num = (survivor.IsHero ? 1 : 0);
			return (int)(survivor.Level + num * DataManager.Instance.GameData.GuildWarConfig.HeroLevelEq +
				UtilsMath.Max(0, survivor.SurvivorRarityLevel - 4) * DataManager.Instance.GameData.GuildWarConfig.PinkLevelEq);
		}

		public static int GetAdjustedLevel(SurvivorMockData survivor)
		{
			int num = (survivor.IsHero ? 1 : 0);
			return (int)(survivor.Level + num * DataManager.Instance.GameData.GuildWarConfig.HeroLevelEq +
				UtilsMath.Max(0, survivor.RarityLevel - 4) * DataManager.Instance.GameData.GuildWarConfig.PinkLevelEq);
		}

		public static int GetAverageAdjustedLevel(List<SurvivorMockData> survivors)
		{
			if (survivors == null) return -1;
			return survivors.Sum((SurvivorMockData x) => x.AdjustedLevel) / 3;
		}
	}

	public class GuildBattleTeamInfo
	{
		public string Name { get; set; }
		public string HashID { get; set; }
		public int AdjustedLevel { get; set; }
		public List<SurvivorMockData> Team { get; set; }
		//A, B, C
		public string TeamIndex { get; set; }
		public List<string> SectorNames { get; set; }
		public List<string> SectorNamesAll { get; set; }
		public int SectorLevel { get; set; }
		//индекс в изначальном PlayerInfoSnapshot
		public int BasePlayerIndex { get; set; }
		//индекс в AllTeamsSorted
		public int TeamSortedPlayerIndex { get; set; }
		//low sector vp
		//public List<int> num2 { get; set; }
		//high sector vp
		//public List<int> num3 { get; set; }
		//public List<int> numIndex { get; set; }
		//public List<bool> amountOfTeamsNeededGreater { get; set; }
		public string GuildJoinedDate { get; set; }
	}

	public class GuildBattlePvpTeamHashed
	{
		public string HashID { get; set; }
		public GuildBattlePvpTeam Team { get; set; }
	}

	public class GuildBattleMatchmakingInfoData
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public DateTime TimeStamp { get; set; }
		public long CurrentBattleStartTime { get; set; }
		public GuildBattleMatchmakingInfo guildBattleMatchmakingInfo { get; set; }

		public GuildBattleMatchmakingInfoData(GuildBattleMatchmakingInfo info)
		{
			if (info == null) return;
			Id = info.GroupId;
			Name = info.GuildName;
			TimeStamp = DateTime.Now.ToLocalTime();
			guildBattleMatchmakingInfo = info;
		}
	}
}
