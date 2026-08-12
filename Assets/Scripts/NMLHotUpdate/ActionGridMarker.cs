using System.Collections.Generic;
using UnityEngine;

public class ActionGridMarker : MonoBehaviour
{
	public List<MoveActionEntry> Actions = new List<MoveActionEntry>();

	public void SetActionMarker(MoveActionType actionType)
	{
		foreach (MoveActionEntry action in Actions)
		{
			if (action.MoveActionType == actionType)
			{
				action.Marker.SetActive(value: true);
			}
			else
			{
				action.Marker.SetActive(value: false);
			}
		}
	}
}
