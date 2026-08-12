using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EndlessModeNormalRewardList : MonoBehaviour
{
	[SerializeField]
	private GameObject rewardEntryPrefab;

	[SerializeField]
	private GameObject rewardEntryContainer;

	private readonly List<GameObject> rewardEntries = new List<GameObject>();

	private void OnEnable()
	{
		UpdateUI();
	}

	private void ShowRewardsByConfig()
	{
		if (this == null)
		{
			return;
		}
		List<EndlessModeNormalRewardDefiniton> getOrderedEndlessModeNormalRewardsDefinitions = EndlessModeHelpers.EndlessManagerModel().GetOrderedEndlessModeNormalRewardsDefinitions;
		if (getOrderedEndlessModeNormalRewardsDefinitions.Count == 0)
		{
			return;
		}
		ClearRewardEntries();
		UITable component = rewardEntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = rewardEntryContainer.GetComponentInParent<UIScrollView>();
		for (int i = 0; i < getOrderedEndlessModeNormalRewardsDefinitions.Count; i++)
		{
			EndlessModeNormalRewardDefiniton endlessModeNormalRewardDefiniton = getOrderedEndlessModeNormalRewardsDefinitions[i];
			if (endlessModeNormalRewardDefiniton != null)
			{
				GameObject gameObject = rewardEntryContainer.AddChild(rewardEntryPrefab);
				NGUITools.SetActive(gameObject, state: true);
				if (gameObject.TryGetComponent<EndlessModeNormalRewardEntry>(out var component2))
				{
					component2.SetContent(endlessModeNormalRewardDefiniton);
				}
				rewardEntries.Add(gameObject);
			}
		}
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void ClearRewardEntries()
	{
		for (int i = 0; i < rewardEntries.Count; i++)
		{
			NGUITools.Destroy(rewardEntries[i]);
		}
		rewardEntries.Clear();
	}

	public void UpdateUI()
	{
		ShowRewardsByConfig();
	}
}
