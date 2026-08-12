using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GIB.Integrations;
using OdinSerializer;

public class PlaytestManager : MonoBehaviour
{
	[Tooltip("Enables or disables playtest data collection mode.")]
	public bool PlaytestMode;
	
	[Tooltip("Version of the game being playtested.")]
	public string PlaytestVersion;
	
	[Tooltip("Unique identifier for the player.")]
	public string PlayerID;
	
	[Tooltip("Serialized JSON string containing player data.")]
	public string PlayerData;

	[OdinSerialize, Tooltip("Dictionary containing variable names and their values for submission.")]
	protected Dictionary<string, string> variableDict = new Dictionary<string, string>();

	private void Start()
	{
		var playerData = new PlayerData();
		PlayerData = playerData.GetJson();

		string playerGuid = PlayerPrefs.GetString("PlaytesterId", "na");
		PlayerID = "Roman";// playerGuid.Substring(playerGuid.Length - 12);

		SubmitPlaytest();

    }
    //{"PlayerEmail":"Player email Missing","PlayerId":"Error generating guid","Persona":"","SteamId":""}

    /// <summary>
    /// Updates or adds a variable to the playtest submission dictionary.
    /// </summary>
    /// <param name="key">The name of the variable.</param>
    /// <param name="value">The value of the variable.</param>
    /// <param name="noDeclareOk">Allows adding new keys without warning if set to true.</param>
    public void UpdateVar(string key, string value, bool noDeclareOk = false)
	{
		if (!variableDict.ContainsKey(key) && !noDeclareOk)
		{
			DebugTWD.LogWarning($"Key {key} is not in Playtest variable list. Add this key to the variable list first!");
			return;
		}

		variableDict.Remove(key);
		variableDict.Add(key, value);
	}

	/// <summary>
	/// Retrieves the value of a specified variable from the playtest submission dictionary.
	/// </summary>
	/// <param name="key">The name of the variable to retrieve.</param>
	/// <returns>The value of the variable, or an empty string if not found.</returns>
	public string GetVarValue(string key)
	{
		if (variableDict.TryGetValue(key,out string value))
		{
			return value;
		}
		return "";
	}

	/// <summary>
	/// Submits the collected playtest data and technical information to Google Sheets.
	/// </summary>
	public void SubmitPlaytest()
	{
		if (!PlaytestMode) return;

		string feedbackString = "";
		//feedbackString += FeedbackBox.text;
		UpdateVar("_Feedback", feedbackString, true);

		// log playtime
		UpdateVar("_PlayTime", Time.realtimeSinceStartup.ToString(), true);
		
		StartCoroutine(StaggerAndSubmitPlaytest());
	}

	public IEnumerator StaggerAndSubmitPlaytest()
	{
		// get timestamp and submit
		string time = System.DateTime.Now.ToString("F");
		GoogleSheets.Instance.SubmitPlaytest(time, PlaytestVersion + "P", PlayerID, variableDict);
		
		yield return new WaitForSecondsRealtime(1f);

		var techDict = new Dictionary<string, string>();

		// log environment
		var userEnvironment = new PlayerEnvironment();
		string environmentString = userEnvironment.GetJson();
		techDict.Add("Environment", environmentString);

		// PlayerData 

		techDict.Add("PlayerData", PlayerData);

		GoogleSheets.Instance.SubmitPlaytest(time, PlaytestVersion+"T", PlayerID, techDict);
	}
	
	
}