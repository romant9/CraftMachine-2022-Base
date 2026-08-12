#if UNITY_POSTRES
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using TwdCustomMod;
using UnityEngine;

public class PostGreManager : MonoBehaviour
{
    public static PostGreManager Instance;

    public static bool IsPostgreActivated
    {
        get
        {
            if (!OfflineManager.IsInternetOn)
            {
                isPostgreActivated = false;
            }
            else
            {
                if (!isPostgreActivated)
                {
                    isPostgreActivated = PingBDResult();
                }
            }          
            return isPostgreActivated;
        }
        set { isPostgreActivated = value; }
    }
    private static bool isPostgreActivated;

    public bool IsError { get; protected set; }
    public SqlCommand SqlCommandType;
    public User CurrentUser { get; protected set; }

    public float responseTimeout = 5;

    public bool IsDebug;
    public string HashId_debug = "73429574a2f3497886e10ccc7753510a";
    public string PlayerName_debug = "Sowa";

    public static string BdServer = "5.188.30.49";
    public static string DbConnectionString => DbBuilder.ToString();
    public static NpgsqlConnectionStringBuilder DbBuilder
    {
        get
        {
            return new NpgsqlConnectionStringBuilder()
            {
                Host = BdServer,
                Port = 5432,
                Database = "twd",
                Username = "twd_admin",
                Password = "kvhK2ntRwMZqKvDY"
            };
        }
    }

    public enum SqlCommand
    {
        ReadOne,
        ReadAll,
        AddNew,
        Replace,
        Remove
    }

    public void Awake()
    {
        Instance = this;
        InitModAccount();
    }

    void Start()
    {
    }

    public void InitModAccount()
    {
        CurrentUser = UserPrefsKeys.GetNullUser();
    }

    [ContextMenu("AddNewPlayer")]
    public void AddNewPlayer()
    {
        using (var conn = new NpgsqlConnection(DbConnectionString))
        {
            try
            {
                conn.Open();

                string sql = $"INSERT INTO CraftMachineUsers (hashid, playername) VALUES ('{HashId_debug}', '{PlayerName_debug}') " +
                    $"ON CONFLICT (hashid) DO NOTHING;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    //cmd.Parameters.AddWithValue("hashid", hashid);
                    //cmd.Parameters.AddWithValue("playername", playername);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Debug.Log($"Success: {rowsAffected} row(s) added.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Что-то пошло не так " + '\n' + ex.Message + '\n' + ex.StackTrace);
                IsError = true;
            }
        }
    }

    [ContextMenu("AddNewPlayerAllData")]
    public void AddNewPlayerAllData()
    {
        using (var conn = new NpgsqlConnection(DbConnectionString))
        {
            try
            {
                conn.Open();

                var hashId = HashId_debug;
                var playerName = PlayerName_debug;
                var firstRun = MyTools.DateTimeToTimeString(DateTime.Now.ToLocalTime());
                var lastRun = MyTools.DateTimeToTimeString(DateTime.Now.ToLocalTime());
                var timesRun = 1;
                var timesConnect = 0;
                var regged = false;
                var blocked = false;
                var proGuild = false;
                var proLink = false;
                var googleId = "G02-D05-1540289e-84cd-43f0-8528-fead1abe8874";
                var epicId = "aa9054a80c264f529bf996af464cf95a";
                var deviceInfo = "StellateSapphire\nSniper";
                var guilName = "BREAKINGϟBAD";
                var guildId = "e3b6832df41941ee8f8a54d818c669ea";
                var country = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var wishes = "";
                var feedback = "";
                var clientVersion = OfflineManager.ClientVersion;
                var modVersion = Application.version;
                var regCode = UserPrefsKeys.GeneratedCode(HashId_debug);

                string sql = $"INSERT INTO CraftMachineUsers (hashid, playername, firstrun, lastrun, timesrun, timesconnect," +
                    $"regged, blocked, proguild, prolink, googleid, epicid, deviceinfo, guildname, guildid, country," +
                    $"wishes, feedback, clientversion, modversion, regcode) " +
                    $"VALUES ('{hashId}', '{playerName}', '{firstRun}', '{lastRun}', {timesRun}, {timesConnect}," +
                    $"{regged}, {blocked}, {proGuild}, {proLink}, '{googleId}', '{epicId}', '{deviceInfo}', '{guilName}', '{guildId}', '{country}'," +
                    $"'{wishes}', '{feedback}', '{clientVersion}', '{modVersion}', {regCode}) " +
                    $"ON CONFLICT (hashid) DO NOTHING;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    //cmd.Parameters.AddWithValue("hashid", hashid);
                    //cmd.Parameters.AddWithValue("playername", playername);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Debug.Log($"Success: {rowsAffected} row(s) added.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Что-то пошло не так " + '\n' + ex.Message + '\n' + ex.StackTrace);
                IsError = true;
            }
        }
    }


    [ContextMenu("ChangePlayerData")]
    public void ChangePlayerData()
    {
        using (var conn = new NpgsqlConnection(DbConnectionString))
        {
            try
            {
                conn.Open();
                string sql = $"UPDATE CraftMachineUsers SET playername = @{PlayerName_debug} WHERE hashid = @{HashId_debug}";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("playername", PlayerName_debug);
                    cmd.Parameters.AddWithValue("hashid", HashId_debug);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Что-то пошло не так " + '\n' + ex.Message + '\n' + ex.StackTrace);
                IsError = true;
            }
        }
    }

    [ContextMenu("GetPlayersAll")]
    public void GetPlayersAll()
    {
        using (var conn = new NpgsqlConnection(DbConnectionString))
        {
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM CraftMachineUsers";
                    int userCount = 0;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                //string hashId = "";
                                var values = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    values.Add(reader[i].ToString());
                                    //hashId += reader.GetValue(i);
                                }
                                userCount++;
                                Debug.Log(string.Join('|', values));
                            }
                            Debug.Log("UsersCount: " + userCount);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Что-то пошло не так " + '\n' + ex.Message + '\n' + ex.StackTrace);
                IsError = true;
            }
        }
    }

    [ContextMenu("GetPlayersOne")]
    public User GetPlayersOne(string hashId)
    {
        if (IsDebug) hashId = HashId_debug;
        IsError = false;
        using var conn = new NpgsqlConnection(DbConnectionString);
        try
        {
            conn.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM CraftMachineUsers Where hashid=@{hashId}";
            using var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                List<object> results = new();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        results.Add(reader.GetValue(i));
                    }
                }
                if (results.Count > 0)
                {
                    return UserPrefsKeys.GetPostgreUser(results);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Что-то пошло не так " + '\n' + ex.Message + '\n' + ex.StackTrace);
            IsError = true;
        }
        return null;
    }

    public static bool PingBDResult()
    {
        Ping ping = new Ping(BdServer);
        float timeout = 5;
        float startTime = Time.time;

        while (!ping.isDone)
        {
            if (Time.time > startTime + timeout)
            {
                Debug.Log("Ping timed out.");
                return false;
            }
        }

        Debug.Log($"Ping to {BdServer}: {ping.time}ms");
        return true;
    }

    public void SetPlayer(User player)
    {
        CurrentUser = player;
    }
}
#endif
