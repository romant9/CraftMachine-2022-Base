using UnityEngine;

public class CombatEndFlowStarWidget : CombatEndWidget
{
	[SerializeField]
	private CombatEndFlowStar[] StartArray;

	[ContextMenu("UpdateUI")]
	public override void UpdateUI()
	{
		if (StartArray == null)
		{
			return;
		}
		for (int i = 0; i < StartArray.Length; i++)
		{
			if (StartArray[i] != null)
			{
				StartArray[i].SetStar(i);
			}
		}
	}
}
