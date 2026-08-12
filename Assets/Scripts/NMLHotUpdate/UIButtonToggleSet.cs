using System;
using UnityEngine;

public class UIButtonToggleSet : MonoBehaviour
{
	public delegate void OnTabsChangeDelegate(UIButtonExtended toggle);

	private const string LogString = "UIButtonToggleSet: ";

	[NonSerialized]
	[HideInInspector]
	public bool CallbackOnlyFromClicks;

	protected UIButtonExtended CurrentToggle;

	private bool init;

	private int InitialToggleIndex = -1;

	public OnTabsChangeDelegate OnChangeDelegate;

	[SerializeField]
	private UIButtonToggle[] ToggleButtons;

	public UIButtonToggle[] GetUIButtonToggleList => ToggleButtons;

	public void Init()
	{
		if (init)
		{
			return;
		}
		for (int i = 0; i < ToggleButtons.Length; i++)
		{
			if (ToggleButtons[i] != null)
			{
				if (CurrentToggle == null || i == InitialToggleIndex)
				{
					CurrentToggle = ToggleButtons[i];
				}
				ToggleButtons[i].SetToggled(toggled: false);
				ToggleButtons[i].id = i.ToString();
			}
		}
		init = true;
	}

	public virtual void Start()
	{
		if (ToggleButtons.Length != 0)
		{
			Init();
			UpdateStates();
		}
	}

	public virtual void OnEnable()
	{
		for (int i = 0; i < ToggleButtons.Length; i++)
		{
			if (ToggleButtons[i] != null)
			{
				ToggleButtons[i].SetClickCallback(OnToggleClick);
			}
		}
		if (CurrentToggle != null)
		{
			CurrentToggle.UpdateColor(instant: true);
		}
	}

	public void OnDisable()
	{
		for (int i = 0; i < ToggleButtons.Length; i++)
		{
			if (ToggleButtons[i] != null)
			{
				ToggleButtons[i].RemoveClickCallback(OnToggleClick);
			}
		}
	}

	public virtual void OnToggleClick(UIButtonExtended toggleButton)
	{
		bool tabChanged = CurrentToggle != toggleButton;
		CurrentToggle = toggleButton;
		UpdateStates("", originOnClick: true, tabChanged);

		if (CustomCallbackFromClick) OnToggleClickCallback();
	}

	public void SetInitialToggle(int index)
	{
		InitialToggleIndex = index;
	}

	public int GetSelectedIndex()
	{
		UIButtonExtended[] toggleButtons = ToggleButtons;
		return Array.IndexOf(toggleButtons, CurrentToggle);
	}

	public void SetSelectedIndex(int index)
	{
		Init();
		if (index >= 0 && index < ToggleButtons.Length)
		{
			CurrentToggle = ToggleButtons[index];
			UpdateStates();
		}
	}

	public virtual void Clear()
	{
		for (int i = 0; i < ToggleButtons.Length; i++)
		{
			if (ToggleButtons[i] != null)
			{
				ToggleButtons[i].Clear();
			}
		}
		OnChangeDelegate = null;
		init = false;
	}

	public void SetButtonIdToIndex(string id, int index)
	{
		if (ToggleButtons == null || ToggleButtons.Length <= index)
		{
			return;
		}
		for (int i = 0; i < ToggleButtons.Length && i <= index; i++)
		{
			UIButtonToggle uIButtonToggle = ToggleButtons[i];
			if (!(uIButtonToggle == null) && i == index)
			{
				uIButtonToggle.id = id;
				break;
			}
		}
	}

	public virtual void SetActiveButtons(bool value)
	{
		for (int i = 0; i < ToggleButtons.Length; i++)
		{
			if (ToggleButtons[i] != null)
			{
				Helpers.GameObjectSetActive(ToggleButtons[i], value);
			}
		}
	}

	public void SetChangeCallback(OnTabsChangeDelegate callback, bool onlyFromClicks = false)
	{
		OnChangeDelegate = (OnTabsChangeDelegate)Delegate.Remove(OnChangeDelegate, callback);
		OnChangeDelegate = (OnTabsChangeDelegate)Delegate.Combine(OnChangeDelegate, callback);
		CallbackOnlyFromClicks = onlyFromClicks;
	}

	protected virtual void UpdateStates(string overrideId = "", bool originOnClick = false, bool tabChanged = true)
	{
		string text = "";
		if (overrideId != "")
		{
			text = overrideId;
		}
		else if (CurrentToggle != null)
		{
			text = CurrentToggle.id;
		}
		for (int i = 0; i < ToggleButtons.Length; i++)
		{
			if (ToggleButtons[i] != null)
			{
				if (text == ToggleButtons[i].id)
				{
					ToggleButtons[i].SetToggled(toggled: true);
					CurrentToggle = ToggleButtons[i];
					if (IsSwitchCustomContent)
					{
						CustomContent[i].SetActive(true);
					}
				}
				else
				{
					ToggleButtons[i].SetToggled(toggled: false);
					if (IsSwitchCustomContent)
					{
						CustomContent[i].SetActive(false);
					}
				}
			}
		}
		if (tabChanged && (!CallbackOnlyFromClicks || (CallbackOnlyFromClicks && originOnClick)))
		{
			InvokeChangeCallback(CurrentToggle);
		}
	}

	protected void InvokeChangeCallback(UIButtonExtended toggleButton)
	{
		if (OnChangeDelegate != null)
		{
			OnChangeDelegate(toggleButton);
		}
	}



	#region myparams
	public bool CustomCallbackFromClick;
	public bool IsSwitchCustomContent;
	public GameObject[] CustomContent;
	#endregion

	#region mycode
	private void OnToggleClickCallback()
	{
		transform.parent.gameObject.SetActive(false);
	}

	public int GetSelectedIndexByObject(UIButton button)
	{
		UIButtonExtended[] toggleButtons = ToggleButtons;
		return Array.IndexOf(toggleButtons, button);
	}

	public UIButtonToggle GetButton(int index)
	{
		return ToggleButtons[index];
	}
	#endregion
}
