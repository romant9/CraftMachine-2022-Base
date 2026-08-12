using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class TraitInfoList : MonoBehaviour
{
	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject EntryContainer;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private ActorModel actorModel;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void UpdateList()
	{
		if (this == null || actorModel == null)
		{
			return;
		}
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = EntryContainer.GetComponentInParent<UIScrollView>();
		List<string> effectShowBuffs = actorModel.GetEffectShowBuffs();
		for (int i = 0; i < effectShowBuffs.Count; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<TraitInfoEntry>(out var component2))
			{
				component2.SetContent(effectShowBuffs[i], actorModel);
			}
			Entries.Add(gameObject);
		}
		component.Reposition();
		componentInParent.ResetPosition();
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	public void UpdateUI()
	{
		UpdateList();
	}

	public void InitData(ActorModel actor)
	{
		actorModel = actor;
		UpdateUI();
	}
}
