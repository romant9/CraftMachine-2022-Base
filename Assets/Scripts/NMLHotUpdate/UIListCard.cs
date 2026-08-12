using UnityEngine;

public class UIListCard<T> : MonoBehaviour where T : class
{
	public enum SurvivorSortOrder
	{
		SurvivorMoreSlotsCard = 0,
		SurvivorClassInfoCard = 1,
		FirstSurvivorCard = 2,
		SurvivorHeroUnlock = 3,
		SurvivorHeroTutorial = 4,
		SurvivorBruiserInPhoneTutorial = 5,
		SurvivorUnavailable = 6,
		FeaturedStarHero = 7,
		SurvivorCombatTeam = 8,
		SurvivorUpgrading = 9,
		SurvivorFavourite = 10,
		SurvivorGeneric = 11,
		SurvivorOutpost = 12,
		SurvivorSurvivalOutOfAction = 13,
		SurvivorInCurrentTeam = 14,
		Count = 15
	}

	[Tooltip("Force this collider for the space between cards in the list.")]
	public BoxCollider ColliderForBuildingTheList;

	public T Item { get; set; }

	public static int GetSortIntFor(SurvivorSortOrder order, int distanceInt = 1000)
	{
		return (int)(15 - order) * distanceInt;
	}

	public virtual void UpdateUI()
	{
	}

	public void EnableToggle()
	{
		UIToggle component = base.gameObject.GetComponent<UIToggle>();
		if (component != null)
		{
			component.enabled = true;
			component.activeSprite.gameObject.SetActive(value: true);
		}
	}

	public virtual int GetSortValue()
	{
		return -1;
	}

	public virtual long GetSortLongValue()
	{
		return -1L;
	}
}
