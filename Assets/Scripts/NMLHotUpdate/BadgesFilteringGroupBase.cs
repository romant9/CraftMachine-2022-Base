using System;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public abstract class BadgesFilteringGroupBase : MonoBehaviour
{
	private List<BadgeInfo> badgesFiltered = new List<BadgeInfo>();

	public UILabel[] LabelsGroup;

	public Action OnBadgesUpdated;

	private List<BadgeInfo> playerBadges;

	public UIButtonToggleSetBase ToggleGroup;

	protected abstract List<BadgeInfo> UpdateFilter(List<BadgeInfo> allBadges, bool[] states = null);

	public List<BadgeInfo> GetBadgesFiltered()
	{
		return badgesFiltered;
	}

	public void ForceUpdate()
	{
		playerBadges = GetPlayerBadges();
		SetBadgesFiltered(UpdateFilter(playerBadges, ToggleGroup.GetState()));
	}

	private void OnEnable()
	{
		playerBadges = GetPlayerBadges();
		UIButtonToggleSetBase toggleGroup = ToggleGroup;
		toggleGroup.OnStateUpdate = (Action<bool[]>)Delegate.Combine(toggleGroup.OnStateUpdate, new Action<bool[]>(OnToggleGroupUpdate));

		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
			SetBadgesFiltered(UpdateFilter(playerBadges, ToggleGroup.GetState()));
			//StartCoroutine(WaitAction());
		}
	}

	private void OnDisable()
	{
		UIButtonToggleSetBase toggleGroup = ToggleGroup;
		toggleGroup.OnStateUpdate = (Action<bool[]>)Delegate.Remove(toggleGroup.OnStateUpdate, new Action<bool[]>(OnToggleGroupUpdate));
	}

	private void OnToggleGroupUpdate(bool[] obj)
	{
		SetBadgesFiltered(UpdateFilter(playerBadges, obj));
	}

	private void SetBadgesFiltered(List<BadgeInfo> badges)
	{
		badgesFiltered = badges;
		if (OnBadgesUpdated != null)
		{
			OnBadgesUpdated();
		}
	}

	private List<BadgeInfo> GetPlayerBadges()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		List<BadgeInfo> list = new List<BadgeInfo>();
		foreach (BadgeModel model in playerModel.Equipment.Badges.Models)
		{
			list.Add(new BadgeInfo(model));
		}
		for (int i = 0; i < playerModel.SurvivorContainer.Survivors.Count; i++)
		{
			SurvivorModel survivorModel = playerModel.SurvivorContainer.Survivors[i];
			foreach (BadgeModel model2 in survivorModel.BadgeContainer.Badges.Models)
			{
				BadgeInfo item = new BadgeInfo(model2)
				{
					OwnerName = survivorModel.Name
				};
				list.Add(item);
			}
		}
		return list;
	}



	#region mycode
	private IEnumerator WaitAction()
	{
		yield return new WaitUntil(() => OnBadgesUpdated != null);
		OnBadgesUpdated();
	}
	#endregion
}
