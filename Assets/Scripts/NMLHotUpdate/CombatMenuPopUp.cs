using System;
using TWDModel;
using UnityEngine;

public class CombatMenuPopUp : HUDElement
{
	[Tooltip("Button to change to map scene.")]
	[SerializeField]
	private UIButton goToMapButton;

	[SerializeField]
	private UIButton closeMenuButton;

	private void OnEnable()
	{
		if (goToMapButton != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(goToMapButton.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnGoToMap));
		}
		if (closeMenuButton != null)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(closeMenuButton.gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnCloseButton));
		}
	}

	private void OnDisable()
	{
		if (goToMapButton != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(goToMapButton.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnGoToMap));
		}
		if (closeMenuButton != null)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(closeMenuButton.gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnCloseButton));
		}
	}

	private void OnGoToMap(GameObject button)
	{
		CombatView.Instance.RequestEndCombat(ECombatResult.Flee);
		Close();
	}

	private void OnCloseButton(GameObject button)
	{
		base.gameObject.SetActive(value: false);
	}
}
