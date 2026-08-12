using System;
using UnityEngine;

public abstract class UIButtonToggleSetBase : MonoBehaviour
{
	public Action<bool[]> OnStateUpdate;

	public abstract void ResetToDefault();

	public abstract bool DefaultIsSelected();

	public abstract bool[] GetState();
}
