using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivorCardTraitBuffsInfoList : MonoBehaviour
{
	public const int BuffMaxCount = 3;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private UIGridExtended grid;

	[SerializeField]
	private LeaderBuffInspireStateIndicator inspirationStateIndicator;

	[SerializeField]
	private LeaderBuffFightingFuryStateIndicator fightingFuryStateIndicator;

	[SerializeField]
	private LeaderBuffBetterTogetherStateIndicator betterTogetherStateIndicator;

	[SerializeField]
	private LeaderBuffProtectStateIndicator leaderBuffProtectStateIndicator;

	private ActorModel actor;

	private List<GameObject> Entries = new List<GameObject>();

	private List<TraitInfoEntryIcon> EntryComponents = new List<TraitInfoEntryIcon>();

	public void InitData(ActorModel at)
	{
		actor = at;
	}

	public void UpdateUI()
	{
		if (this == null || actor == null)
		{
			return;
		}
		Entries.ForEach(delegate(GameObject t)
		{
			Object.DestroyImmediate(t);
		});
		Entries.Clear();
		EntryComponents.Clear();
		List<string> effectShowBuffs = actor.GetEffectShowBuffs();
		List<string> list = new List<string>();
		int num = 0;
		if (effectShowBuffs != null && effectShowBuffs.Count > 0)
		{
			for (int num2 = effectShowBuffs.Count - 1; num2 >= 0; num2--)
			{
				GameObject gameObject = base.gameObject.AddChild(EntryPrefab);
				NGUITools.SetActive(gameObject, state: true);
				TraitInfoEntryIcon componentInChildren = gameObject.GetComponentInChildren<TraitInfoEntryIcon>();
				componentInChildren.SetContent(effectShowBuffs[num2], actor);
				EntryComponents.Add(componentInChildren);
				Entries.Add(gameObject);
				list.Add(effectShowBuffs[num2]);
				num++;
				if (num >= 3)
				{
					break;
				}
			}
		}
		Helpers.GameObjectSetActive(inspirationStateIndicator, value: false);
		Helpers.GameObjectSetActive(fightingFuryStateIndicator, value: false);
		Helpers.GameObjectSetActive(betterTogetherStateIndicator, value: false);
		Helpers.GameObjectSetActive(leaderBuffProtectStateIndicator, value: false);
		int num3 = 3 - num;
		if (num3 > 0)
		{
			inspirationStateIndicator?.SetActorModel(actor);
			if (inspirationStateIndicator.gameObject.activeSelf)
			{
				num3--;
			}
		}
		if (num3 > 0)
		{
			fightingFuryStateIndicator?.SetActorModel(actor);
			if (fightingFuryStateIndicator.gameObject.activeSelf)
			{
				num3--;
			}
		}
		if (num3 > 0)
		{
			betterTogetherStateIndicator?.SetActorModel(actor);
			if (betterTogetherStateIndicator.gameObject.activeSelf)
			{
				num3--;
			}
		}
		if (num3 > 0)
		{
			leaderBuffProtectStateIndicator?.SetActorModel(actor);
			if (leaderBuffProtectStateIndicator.gameObject.activeSelf)
			{
				num3--;
			}
		}
		grid.Reposition();
	}

	public void OnClickBuffBtn()
	{
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (combatHUD != null)
		{
			combatHUD.OpenTraitInfoContainer(actor);
		}
	}
}
