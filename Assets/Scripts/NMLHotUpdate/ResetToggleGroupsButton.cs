using System;
using System.Linq;
using UnityEngine;

public class ResetToggleGroupsButton : MonoBehaviour
{
	public UIButtonExtended ResetButton;

	public UIButtonToggleSetBase[] Sets;

	private void OnEnable()
	{
		ResetButton.gameObject.SetActive(value: false);
		ResetButton.SetClickCallback(OnResetButtonClick);
		UIButtonToggleSetBase[] sets = Sets;
		foreach (UIButtonToggleSetBase obj in sets)
		{
			obj.OnStateUpdate = (Action<bool[]>)Delegate.Combine(obj.OnStateUpdate, new Action<bool[]>(OnStateUpdate));
		}
	}

	private void OnDisable()
	{
		ResetButton.RemoveClickCallback(OnResetButtonClick);
		UIButtonToggleSetBase[] sets = Sets;
		foreach (UIButtonToggleSetBase obj in sets)
		{
			obj.OnStateUpdate = (Action<bool[]>)Delegate.Remove(obj.OnStateUpdate, new Action<bool[]>(OnStateUpdate));
		}
	}

	private void OnResetButtonClick(UIButtonExtended button)
	{
		UIButtonToggleSetBase[] sets = Sets;
		for (int i = 0; i < sets.Length; i++)
		{
			sets[i].ResetToDefault();
		}
	}

	private void OnStateUpdate(bool[] obj)
	{
		ResetButton.gameObject.SetActive(Sets.Any((UIButtonToggleSetBase x) => !x.DefaultIsSelected()));
	}
}
