using System.Collections.Generic;
using TWDModel;

public class OutpostTierListPanel : ScrollableListPanel<OutpostTier>
{
	private void OnEnable()
	{
		List<OutpostTier> list = new List<OutpostTier>();
		OutpostSeason outpostSeasonById = GameManager.Instance.gameEconomyData.GetOutpostSeasonById(GameManager.Instance.playerModel.CurrentOutpostSeasonId);
		if (outpostSeasonById != null)
		{
			list = GameManager.Instance.gameEconomyData.GetOutpostTiers(outpostSeasonById.TierSetId);
			list.StableSort(delegate(OutpostTier a, OutpostTier b)
			{
				int num = ((a.Rank < 0) ? (-1) : (1000 - a.Rank)) * 100000 + a.MinInfluence;
				int value = ((b.Rank < 0) ? (-1) : (1000 - b.Rank)) * 100000 + b.MinInfluence;
				return num.CompareTo(value);
			});
		}
		SetCards(list);
	}

	private void OnDisable()
	{
	}

	public void Update()
	{
	}
}
