using TWDModel;
using UnityEngine;

public class TraitInfoContainer : MonoBehaviour
{
	[SerializeField]
	private TraitInfoList traitInfoList;

	public void UpdateUI(ActorModel actor)
	{
		traitInfoList.InitData(actor);
	}
}
