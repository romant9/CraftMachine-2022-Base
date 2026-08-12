using System;
using TWDModel;
using UnityEngine;

public class CombatAreaActorEffectView : MonoBehaviour
{
	[SerializeField]
	private GameObject allyEffectObject;

	[SerializeField]
	private GameObject opponentEffectObject;

	public void Show(Faction faction)
	{
		bool flag = faction == Faction.Survivor;
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		Helpers.GameObjectSetActive(allyEffectObject, flag);
		Helpers.GameObjectSetActive(opponentEffectObject, !flag);
	}

	public virtual void StartKill(Action killEndAction)
	{
		killEndAction?.Invoke();
	}
}
