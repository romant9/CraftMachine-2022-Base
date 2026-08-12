using TWDModel;
using UnityEngine;

public class RifleClapClickHandler : MonoBehaviour
{
	private string tooltipText;

	private Faction faction;

	public void Initialize(string text, Faction targetFaction)
	{
		tooltipText = text;
		faction = targetFaction;
	}

	public bool HandleClick()
	{
		if (faction != Faction.Survivor)
		{
			return false;
		}
		string text = (string.IsNullOrEmpty(tooltipText) ? base.gameObject.name : tooltipText);
		TooltipManager.OpenTextBoxWithText(base.gameObject, text, TooltipManager.Prefabs.TooltipCombatTextbox);
		return true;
	}
}
