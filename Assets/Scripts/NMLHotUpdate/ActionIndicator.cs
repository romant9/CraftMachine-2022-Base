using System;
using UnityEngine;

public class ActionIndicator : HUDElementFollowTarget
{
	public GameObject forceIcon;

	public UIButton forceButton;

	public GameObject turnCountRoot;

	public UILabel turnCountLabel;

	private Action forceButtonAction;

	private bool isInteractive = true;

	public void SetInteractive(bool interactive)
	{
		UIEventListener uIEventListener = UIEventListener.Get(forceButton.gameObject);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnForceClicked));
		if (interactive)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(forceButton.gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnForceClicked));
		}
		isInteractive = interactive;
	}

	public void SetTurnCount(int count)
	{
		turnCountLabel.text = count.ToString();
	}

	public void SetForceButtonAction(Action action)
	{
		forceButtonAction = action;
	}

	public void ShowIndicator()
	{
		base.gameObject.SetActive(value: true);
	}

	public void HideIndicator()
	{
		base.gameObject.SetActive(value: false);
	}

	protected void Start()
	{
		if (isInteractive)
		{
			UIEventListener uIEventListener = UIEventListener.Get(forceButton.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnForceClicked));
		}
	}

	protected void OnDestroy()
	{
		UIEventListener uIEventListener = UIEventListener.Get(forceButton.gameObject);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnForceClicked));
	}

	protected void OnForceClicked(GameObject button)
	{
		if (forceButtonAction != null)
		{
			forceButtonAction();
		}
	}
}
