using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ApocalypticShowEffectPopup : HUDElement
{
	[SerializeField]
	private ApocalypticListPanel apocalypticListPanel;

	[SerializeField]
	private UILabel pageLabel;

	private int _currentPage;

	private readonly int singlePageCount = 8;

	public override void Open()
	{
		base.Open();
		_currentPage = 1;
		UpdateUI();
	}

	private new void UpdateUI()
	{
		List<WeeklyChallengeApocalypseBuff> weeklyChallengeApocalypseBuffs = GameManager.Instance.playerModel.ApocalypseWeeklyChallenge.weeklyChallengeApocalypseBuffs;
		if (weeklyChallengeApocalypseBuffs == null || weeklyChallengeApocalypseBuffs.Count <= 0)
		{
			return;
		}
		int num = Mathf.CeilToInt((float)weeklyChallengeApocalypseBuffs.Count * 1f / (float)singlePageCount);
		if (_currentPage > 0 && _currentPage <= num)
		{
			pageLabel.text = $"{_currentPage}/{num}";
			List<WeeklyChallengeApocalypseBuff> page = GetPage(weeklyChallengeApocalypseBuffs, _currentPage);
			if (apocalypticListPanel != null && page != null)
			{
				apocalypticListPanel.Init(page);
			}
		}
	}

	public void OnClickNextButton()
	{
		_currentPage++;
		UpdateUI();
	}

	public void OnClickBackButton()
	{
		_currentPage--;
		UpdateUI();
	}

	private List<WeeklyChallengeApocalypseBuff> GetPage(List<WeeklyChallengeApocalypseBuff> pageList, int currentPage)
	{
		int num = (currentPage - 1) * singlePageCount;
		if (num >= pageList.Count)
		{
			return null;
		}
		int count = Mathf.Min(singlePageCount, pageList.Count - num);
		return pageList.GetRange(num, count);
	}
}
