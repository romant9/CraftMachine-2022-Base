using System.Collections.Generic;
using TWDModel;

public class BattleLogListPanel : ScrollableListPanel<OutpostVisitEntry>
{
	protected override bool LastEntryAtTop => true;

	private void OnEnable()
	{
		List<OutpostVisitEntry> list = new List<OutpostVisitEntry>();
		if (GameManager.Instance.playerModel.DefenseOutpostVisitLog != null && GameManager.Instance.playerModel.DefenseOutpostVisitLog.Count > 0)
		{
			list.AddRange(GameManager.Instance.playerModel.DefenseOutpostVisitLog);
		}
		if (GameManager.Instance.playerModel.AttackOutpostVisitLog != null && GameManager.Instance.playerModel.AttackOutpostVisitLog.Count > 0)
		{
			list.AddRange(GameManager.Instance.playerModel.AttackOutpostVisitLog);
		}
		list.StableSort((OutpostVisitEntry a, OutpostVisitEntry b) => a.UtcTime.CompareTo(b.UtcTime));
		SetCards(list);
	}
}
