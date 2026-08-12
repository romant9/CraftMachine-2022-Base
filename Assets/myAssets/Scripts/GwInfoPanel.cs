using System;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class GwInfoPanel : MonoBehaviour
{
    private GuildModel guildModel => GWTeamUtils.Instance.IsOpponentGuild ? GWTeamUtils.Instance.OpponentGuilModel : GWTeamUtils.Instance.GuildModel;
    private string allText = string.Empty;

    public UITextList InfoList;
    public bool isDetailedView;

    private string TeamsForMapsSer;
    private List<GuildBattleTeamInfo> TeamsForMaps;

    public UIToggle opponentGuildToggle;


    void Start()
    {       
    }

    private void OnEnable()
    {
        SavePlayerCloseMaps();
        GWTeamUtils.Instance.ActionGuildChange += SwitchGuild;
    }

    private void OnDisable()
    {
        GWTeamUtils.Instance.ActionGuildChange -= SwitchGuild;
    }

    public void SwitchDetailedView(UIToggle tg)
    {
        if (isDetailedView != tg.value)
        {
            isDetailedView = tg.value;

            SwitchGuild();
        }
    }

    public void SwitchGuild()
    {
        string enemyGuildId = GWTeamsManager.Instance.CurrentGuildModel.GuildWarModel.CurrentBattle.EnemyGuildData.GroupId;
        TeamsForMapsSer = GWTeamsManager.Instance.GWProtList.TeamsForMapsOpponenets != null && GWTeamsManager.Instance.GWProtList.TeamsForMapsOpponenets.ContainsKey(enemyGuildId) ?
            GWTeamsManager.Instance.GWProtList.TeamsForMapsOpponenets[enemyGuildId] : null;

        if (string.IsNullOrEmpty(TeamsForMapsSer))
        {
            if (isDetailedView)
                OpenAlert();
            return;
        }
        TeamsForMaps = OfflineManager.JsonSerializer.Deserialize<List<GuildBattleTeamInfo>>(TeamsForMapsSer);
        SavePlayerCloseMaps();
    }

    private void OpenAlert()
    {
        AlertPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
        if (confirmationPopup != null)
        {
            confirmationPopup.SetContent("", "Сперва рассчитайте защиту врагов на соседней вкладке!");
            confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
            confirmationPopup.SetCallbacks(delegate
            {
                confirmationPopup.Close();
            });
            confirmationPopup.Open();
        }
    }

    public void SavePlayerCloseMaps()
    {
        DebugTWD.Log("SavePlayerCloseMaps");
        InfoList.Clear();
        allText = string.Empty;
        //сколько ОП у игроков
        var totalVP = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.CalculateTotalVictoryPoints();
        var sectors_closed = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.CompletedSectors;

        var vp_all = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.VictoryPointsPerPlayer;
        //var bonusSectorVP = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.VictoryPointsSectorRewardPerSector;
        var missions_closed = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.CompletedMissionsPerPlayer;
        var attacks = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.NumberOfAttacksPerPlayer;
        //var sectors = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.CurrentMapModel.Sectors;
        var currentBattle = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle;

        string guildName = GWTeamsManager.Instance.CurrentGuildModel.GuildWarModel.CurrentBattle.EnemyGuildName;
        string result = guildName + '\n' + DateTime.Now.ToLocalTime().ToString(UserPrefsKeys.TimeFormat) + '\n';

        allText = result + "Всего ОП : " + totalVP + '\n';

        string file = Resources.Load<TextAsset>("Config/PvpTeamsIndexPerMission").text;
        Dictionary<string, string> PvpTeamsIndexPerMission = OfflineManager.JsonSerializer.Deserialize<Dictionary<string, string>>(file);
        Dictionary<string, string> PvpTeamsIndexPerMissionNew = new Dictionary<string, string>();
        foreach (var d in PvpTeamsIndexPerMission)
        {
            var key = d.Key.Split('_')[1];
            if (!PvpTeamsIndexPerMissionNew.ContainsKey(key))
                PvpTeamsIndexPerMissionNew.Add(key, d.Value.Substring(0, d.Value.Length - 1));
        }
        //
        foreach (var sector in currentBattle.CurrentMapModel.Sectors.ToList())
        {
            for (int i = 0; i < sector.RandomizedMissions.Count; i++)
            {
                GuildBattleMapMissionModel guildBattleMapMissionModel = sector.RandomizedMissions[i];
                if (guildBattleMapMissionModel.PvpParticipants.Count != 0 && guildBattleMapMissionModel.Type == GuildBattleMapMissionModel.MissionType.PVP)
                {
                    DebugTWD.Log("_1 : " + guildBattleMapMissionModel.PvpParticipants.First()
                        + "__" + guildBattleMapMissionModel.PvpPlayerHashedId);
                    DebugTWD.Log("1 : " + (guildModel.GetMemberInfo(guildBattleMapMissionModel.PvpParticipants.First())?.Name ?? "null")
                            //+ "__" + guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.EnemyGuildData.GetParticipantInfo(guildBattleMapMissionModel.PvpPlayerHashedId));
                            + "__" + (guildModel.GetMemberInfo(guildBattleMapMissionModel.PvpPlayerHashedId)?.Name ?? "null"));
                    DebugTWD.Log("__1 : " + guildBattleMapMissionModel.Id + " " + guildBattleMapMissionModel.IsPvpComplete() + " " + guildBattleMapMissionModel.CompletionAmount);
                }

                if (guildBattleMapMissionModel.SavedData.Count != 0 && guildBattleMapMissionModel.SavedData.Count < 3 && guildBattleMapMissionModel.Type == GuildBattleMapMissionModel.MissionType.PVP)
                {
                    DebugTWD.Log("_2 : " + guildBattleMapMissionModel.Id + " " + guildBattleMapMissionModel.IsPvpComplete() + " " + guildBattleMapMissionModel.CompletionAmount);
                    //DebugTWD.Log("2 : " + guildModel.GuildBattleMatchmakingInfo.GetParticipantInfo(guildBattleMapMissionModel.PvpParticipants.First()).Name
                    //        + "__" + guildModel.GuildBattleMatchmakingInfo.GetParticipantInfo(guildBattleMapMissionModel.PvpPlayerHashedId));
                }
            }
        }
        //

        string closedSectors = "Закрытые сектора : ";
        foreach (var s in sectors_closed)
        {
            PvpTeamsIndexPerMissionNew.TryGetValue(s.ToString(), out string sectorName);
            closedSectors += sectorName + (sectors_closed.IndexOf(s) < sectors_closed.Count - 1 ? ", " : "\n");
        }

        var gwPlayers = guildModel.GvGSeasonModel.GuildWarModel.CurrentBattle.RegisteredPlayers;
        if (gwPlayers.Count == 0) return;
        var bbData = guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot;
        var gwPlayersData = new List<GwPlayer>();

        foreach (var player in gwPlayers)
        {
            var gwPlayer = new GwPlayer();
            gwPlayer.id = player;
            if (bbData.TryGetValue(player, out GuildBattleParticipantInfo info))
                gwPlayer.Name = info.Name;
            else gwPlayer.Name = "UNKNOWN";
            int allVictoryPoints = 0;
            List<string> mapsClosed = new List<string>();
            List<string> mapPvpDetail = new List<string>();

            var mapClosed = missions_closed.ContainsKey(player) ? missions_closed[player] : null;
            if (mapClosed != null)
            {
                for (int i = 0; i < mapClosed.Count; i++)
                {
                    var map = mapClosed[i].Split('_');
                    var sectorIndex = map[1];
                    var mapIndex = int.Parse(map[2]);
                    var sectorContainMap = PvpTeamsIndexPerMission.FirstOrDefault(x => x.Key.Split('_')[1] == sectorIndex);
                    var sectorContainMapValue = sectorContainMap.Value;
                    var sectorContainMapName = sectorContainMapValue.Substring(0, sectorContainMapValue.Length - 1);
                    int maxIndex = int.Parse(sectorContainMap.Key.Split('_')[2]);

                    var mapClosedName = sectorContainMapName + MapName(mapIndex, sectorContainMapName.ToLower().Contains("c"));

                    GuildBattleMapMissionModel missionModel = currentBattle.CurrentMapModel.GetMissionModel(mapClosed[i]);
                    if (missionModel != null)
                    {
                        bool isPvp = currentBattle.CurrentMapModel.PvpTeamsIndexPerMission.ContainsKey(mapClosed[i]);
                        //int victoryPoints = currentBattle.GetGuildBattleMissionVictoryPoints(missionModel.SectorModelOwner.SectorId, isPvp, missionModel.AreaIndex);
                        int victoryPoints = GetGuildBattleMissionVictoryPoints(int.Parse(sectorIndex), isPvp, missionModel.AreaIndex);
                        allVictoryPoints += victoryPoints;

                        if (isPvp)
                        {
                            //List<GuildBattleTeamInfo> TeamsForMaps = GWTeamsManager.instance.GWProtList.TeamsForMaps;
                            if (TeamsForMaps != null)
                            {
                                string pvpMapId = mapClosedName.Replace(".pvp", "");
                                var pvpTeam = TeamsForMaps.FirstOrDefault(x => x.SectorNames.Contains(pvpMapId));
                                if (pvpTeam != null)
                                {
                                    string pvpPlayerName = pvpTeam.Name;
                                    string pvpTeamIndex = pvpTeam.TeamIndex;
                                    int pvpTeamLevel = GWTeamUtils.GetAverageAdjustedLevel(pvpTeam.Team);

                                    string pvpData = $"{pvpMapId} : {pvpPlayerName}, Team.{pvpTeamIndex}, Lv.{pvpTeamLevel}";

                                    mapPvpDetail.Add(pvpData);
                                }  
                            }
                        }
                    }

                    mapsClosed.Add(mapClosedName);
                }
            }

            //mapsClosed?.StableSort((string a, string b) => a.CompareTo(b));
            gwPlayer.MapsDetailList = mapPvpDetail;
            gwPlayer.MapsClosed = mapsClosed;
            gwPlayer.MapsCount = mapsClosed != null ? mapsClosed.Count : 0;
            gwPlayer.VpBase = allVictoryPoints;
            gwPlayer.VpCount = vp_all.ContainsKey(player) ? vp_all[player] : 0;
            gwPlayer.AttacksLeft = attacks.ContainsKey(player) ? attacks[player] : 18;
            gwPlayersData.Add(gwPlayer);
        }

        string playersCaption = "\nСтатистика по игрокам:\n";
        string playersData = "";

        foreach (var gwPlayerData in gwPlayersData)
        {
            var mapsClosedSort = gwPlayerData.MapsClosed;
            mapsClosedSort.StableSort((string a, string b) => a.CompareTo(b));

            string detailViewData = "";
            if (isDetailedView && gwPlayerData.MapsDetailList.Count > 0)
            {
                detailViewData = "\nPVP враги : " + gwPlayerData.MapsDetailList.Count + "\n" + string.Join("\n", gwPlayerData.MapsDetailList);
            }

            playersData += gwPlayerData.Name + '\n' +
                "ОП : " + gwPlayerData.VpCount + " / " + gwPlayerData.VpBase + '\n' +
                "Закрытые локации : " + (gwPlayerData.MapsClosed != null ? string.Join(", ", mapsClosedSort) : "") + '\n' +
                "Закрыто : " + gwPlayerData.MapsCount.ToString() + " локаций\n" +
                "Осталось атак : " + gwPlayerData.AttacksLeft.ToString() + detailViewData + "\n-----------------\n";
        }
        allText += closedSectors + playersCaption + playersData;
        InfoList.Add(allText);
    }

    public void CopyToClipboardGWData()
    {
        MyTools.CopyToClipboard(allText);
    }

    public int GetGuildBattleMissionVictoryPoints(int sectorId, bool isPvP, int column)
    {
        int num = -1;
        GuildBattleSectorDefinition guildBattleSectorDefinition = DataManager.Instance.GameData.FindMissionSectorDefinition(sectorId);
        if (guildBattleSectorDefinition != null)
        {
            int difficultyOffset = (isPvP ? guildBattleSectorDefinition.PVPModifierPerArea[column] : guildBattleSectorDefinition.ColumnsDifficulty[column]);
            num = DataManager.Instance.GameData.GetGuildBattleMissionRewardVP(difficultyOffset, isPvP);
        }
        if (num == -1 || num == 0)
        {
            DebugTWD.LogWarning("No reward for Mission Completions, fallback to default value 2");
            return 2;
        }
        return num;
    }

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

    public class GwPlayer
    {
        public string id {  get; set; }
        public string Name { get; set; }
        public int VpCount { get; set; }
        public int VpBase {  get; set; }
        public List<string> MapsClosed { get; set; }
        public List<string> MapsDetailList { get; set; }

        public int MapsCount { get; set; }
        public int AttacksLeft { get; set; }

        public GwPlayer() { }
    }
}
