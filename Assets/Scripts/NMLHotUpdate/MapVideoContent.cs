using System;
using System.Collections.Generic;
using TWDModel;
using TWDModel.ContentTypes;
using UnityEngine;

public class MapVideoContent : MonoBehaviour
{
	private string videoURL;

	private string episodeId;

	[SerializeField]
	private UILabel videoDescription;

	[SerializeField]
	private GameObject videoButtonGlow;

	[SerializeField]
	private GameObject newIndicator;

	[SerializeField]
	private GameObject episodeVideoButton;

	[Header("Season promo video")]
	[SerializeField]
	private GameObject seasonVideoButton;

	[SerializeField]
	private GameObject newSeasonVideoIndicator;

	[SerializeField]
	private GameObject seasonVideoButtonGlow;

	private string seasonVideoURL;

	private string seasonVideoID;

	private bool destroyed;

	private bool videoExistsForSeason = true;

	public void Init(MissionSpawnPointGroup currentGroup)
	{
		destroyed = false;
		episodeId = currentGroup.MapId;
		Helpers.GameObjectSetActive(newIndicator, !GameManager.Instance.playerModel.Blackboard.IsToggleOn(BlackboardModel.GetEpisodeVideoWatchedKey(episodeId)));
		if (currentGroup.Category == MapCategory.Season && videoExistsForSeason)
		{
			GetVideoUrlForEpisode(episodeId);
		}
		UpdateActiveVideoButton();
	}

	public void SetSeasonVideo(string videoURL)
	{
		seasonVideoURL = videoURL;
		if (!string.IsNullOrEmpty(seasonVideoURL))
		{
			seasonVideoID = GetVideoIdFromYoutubeURL(seasonVideoURL);
		}
	}

	public void OnDestroy()
	{
		destroyed = true;
	}

	private string GetVideoIdFromYoutubeURL(string videoURL)
	{
		if (videoURL.Contains("v="))
		{
			string[] array = seasonVideoURL.Split(new string[1] { "v=" }, StringSplitOptions.None);
			if (array.Length >= 2)
			{
				array = array[1].Split('&');
				return array[0];
			}
		}
		else if (videoURL.Contains("youtu.be/"))
		{
			string[] array = seasonVideoURL.Split(new string[1] { "youtu.be/" }, StringSplitOptions.None);
			if (array.Length > 1)
			{
				array = array[1].Split('&');
				return array[0];
			}
		}
		return null;
	}

	public void GetVideoUrlForEpisode(string episodeId)
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			ContentManager.Instance.LoadContent(typeof(EpisodeVideo).Name + "/" + episodeId, OnEpisodeVideoContent);
		}
	}

	protected void OnEpisodeVideoContent(string transactionId, bool loaded)
	{
		if (!loaded)
		{
			return;
		}
		string content = ContentManager.Instance.GetContent(transactionId);
		if (string.IsNullOrEmpty(content))
		{
			videoExistsForSeason = false;
		}
		else
		{
			if (destroyed)
			{
				return;
			}
			List<EpisodeVideo> list = GameManager.Instance.jsonSerializer.Deserialize<List<EpisodeVideo>>(content);
			if (list == null || list.Count == 0)
			{
				videoExistsForSeason = false;
				return;
			}
			EpisodeVideo episodeVideo = list[0];
			videoURL = episodeVideo.ManifestUri;
			base.gameObject.SetActive(value: true);
			bool active = !GameManager.Instance.Blackboard.IsToggleOn(BlackboardModel.GetEpisodeVideoWatchedKey(episodeId));
			if (videoButtonGlow != null)
			{
				videoButtonGlow.SetActive(active);
			}
			videoDescription.text = HelpersLocalization.GetEpisodeVideoDescription(episodeId);
			UpdateActiveVideoButton();
		}
	}

	public void OnClickPlayVideo()
	{
		if (!string.IsNullOrEmpty(seasonVideoURL))
		{
			string text = GameManager.Instance.gameEconomyData.ConfigData.CurrentCampaign.ToString();
			GameManager.Instance.modelManager.Metrics.AddStart().AddSeasonVideo(text, "season_mission_selection").Send();
			Application.OpenURL(seasonVideoURL);
			Helpers.ExecuteCommand(new SetBlackboardToggleCommand(BlackboardModel.GetEpisodeVideoWatchedKey(seasonVideoID)));
			UpdateActiveVideoButton();
		}
	}

	public void UpdateActiveVideoButton()
	{
		Helpers.GameObjectSetActive(seasonVideoButton, value: false);
		Helpers.GameObjectSetActive(episodeVideoButton, value: false);
		if (!string.IsNullOrEmpty(seasonVideoID) && GameManager.Instance.gameEconomyData.ConfigData.CurrentCampaign != PromoCampaignType.None)
		{
			Helpers.GameObjectSetActive(seasonVideoButton, value: true);
			bool active = !GameManager.Instance.Blackboard.IsToggleOn(BlackboardModel.GetEpisodeVideoWatchedKey(seasonVideoID));
			if (newSeasonVideoIndicator != null)
			{
				newSeasonVideoIndicator.SetActive(active);
			}
			if (seasonVideoButtonGlow != null)
			{
				seasonVideoButtonGlow.SetActive(active);
			}
		}
		else if (!string.IsNullOrEmpty(videoURL) && !GameConfiguration.Instance.Config.LowViolence)
		{
			Helpers.GameObjectSetActive(episodeVideoButton, value: true);
		}
	}
}
