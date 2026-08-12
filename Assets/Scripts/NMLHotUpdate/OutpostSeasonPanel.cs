using TWDModel;
using UnityEngine;

public class OutpostSeasonPanel : MonoBehaviour
{
	[SerializeField]
	private UILabel SeasonNameLabel;

	[SerializeField]
	private UILabel SeasonTimerLabel;

	[SerializeField]
	private UILabel CurrentInfluenceLabel;

	[SerializeField]
	private float UpdateInterval = 1f;

	private float timer;

	private OutpostSeason season;

	private bool hasCurrent;

	private long timestamp;

	private void UpdateUI()
	{
		if (SeasonNameLabel != null)
		{
			SeasonNameLabel.text = ((season != null) ? LocalizationManager.GetText(season.LocalizationKey, season.Id) : LocalizationManager.GetText("OutpostSeason.NoneFound"));
		}
		if (CurrentInfluenceLabel != null)
		{
			CurrentInfluenceLabel.text = GameManager.Instance.playerModel.RankingScore.ToString();
		}
		if (SeasonTimerLabel != null)
		{
			long num = timestamp - GameManager.Instance.playerModel.UtcTimeStamp;
			if (num < 0)
			{
				num = 0L;
			}
			string text = (hasCurrent ? LocalizationManager.GetText("OutpostSeason.EndsIn{Time}", Helpers.FormatTimeNoZero(num)) : LocalizationManager.GetText("OutpostSeason.StartsIn{Time}", Helpers.FormatTimeNoZero(num)));
			SeasonTimerLabel.text = ((season != null) ? text : "");
		}
	}

	private void CacheSeason()
	{
		if (!GameManager.Instance.playerModel.HasValidOutpost)
		{
			hasCurrent = false;
			return;
		}
		season = GameManager.Instance.gameEconomyData.GetOutpostSeasonById(GameManager.Instance.playerModel.CurrentOutpostSeasonId);
		hasCurrent = season != null;
		if (season == null)
		{
			season = GameManager.Instance.gameEconomyData.GetNextOutpostSeason(GameManager.Instance.playerModel.UtcTimeStamp);
			if (season != null)
			{
				timestamp = season.StartTimeMilliseconds;
			}
		}
		else
		{
			timestamp = season.EndTimeMilliseconds;
		}
	}

	private void OnEnable()
	{
		CacheSeason();
		UpdateUI();
	}

	public void Update()
	{
		timer += Time.deltaTime;
		if (timer > UpdateInterval)
		{
			timer = 0f;
			UpdateUI();
		}
	}
}
