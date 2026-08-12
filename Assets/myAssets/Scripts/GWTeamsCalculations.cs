using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class GWTeamsCalculations : MonoBehaviour
{
    private MessageSerializer jsonSerializer { get; set; }

    GuildBattlePvpTeam guildBattlePvpTeam_A; //top, last item
    GuildBattlePvpTeam guildBattlePvpTeam_B; //low, first item
    GuildBattlePvpTeam guildBattlePvpTeam_C; //low, second item

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private List<GuildBattlePvpTeam> GetAllTeamsSorted(Dictionary<string, GuildBattleParticipantInfo> players)
    {
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

                list.Add(guildBattlePvpTeam_A);
                list.Add(guildBattlePvpTeam_B);
                list.Add(guildBattlePvpTeam_C);
            }
        }

        list.StableSort((GuildBattlePvpTeam a, GuildBattlePvpTeam b) => a.AverageAdjustedLevel.CompareTo(b.AverageAdjustedLevel));
        return list;
    }

    public void SaveTeamsOppositeMap(GuildModel guildModel)
    {
        jsonSerializer = new MessageSerializer();
        var file = Resources.Load<TextAsset>("Config/PvpTeamsIndexPerMission").text;
        Dictionary<string, string> PvpTeamsIndexPerMission = jsonSerializer.Deserialize<Dictionary<string, string>>(file);

        Dictionary<string, GuildBattleParticipantInfo> bbInfo = guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot;

        //
        List<GuildBattlePvpTeam> allTeamsSorted = GetAllTeamsSorted(bbInfo);
        guildBattlePvpTeam_A = allTeamsSorted.Last();
        guildBattlePvpTeam_B = allTeamsSorted[1];
        guildBattlePvpTeam_C = allTeamsSorted[0];

        foreach (var survivor in bbInfo)
        {
            List<SurvivorMockData> selectedSurvivors = survivor.Value.SelectedSurvivors;
            if (selectedSurvivors.Count >= 9)
            {
                var selectedSurvivorsCustom = new List<SurvivorMockData>
                {
                    guildBattlePvpTeam_A.Survivors[0],
                    guildBattlePvpTeam_A.Survivors[1],
                    guildBattlePvpTeam_A.Survivors[2],
                    guildBattlePvpTeam_B.Survivors[0],
                    guildBattlePvpTeam_B.Survivors[1],
                    guildBattlePvpTeam_B.Survivors[2],
                    guildBattlePvpTeam_C.Survivors[0],
                    guildBattlePvpTeam_C.Survivors[1],
                    guildBattlePvpTeam_C.Survivors[2]
                };

                survivor.Value.SelectedSurvivors = selectedSurvivorsCustom;              
            }
        }
        //

        
    }
}
