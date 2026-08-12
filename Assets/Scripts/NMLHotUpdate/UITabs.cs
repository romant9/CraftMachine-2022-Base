using UnityEngine;

public class UITabs : MonoBehaviour
{
	public delegate void NewTabSelectedDelegate(int tabIndex);

	private static int lastTabIndex = -1;

	[SerializeField]
	[Tooltip("Save the tab for when we reopen the tabs.")]
	private bool saveLastTabIndex;

	[SerializeField]
	[Tooltip("The button of the tabs.")]
	protected UIToggle[] buttons;

	[SerializeField]
	[Tooltip("The content of the tabs.")]
	private GameObject[] contentPanels;

	private const string RedDotNodeName = "Red";

	private GameObject[] redDots;

	private bool hasBeenInitialised;

	public int CurrentTabIndex { get; private set; }

	public int TabNumber => buttons.Length;

	public event NewTabSelectedDelegate OnNewTabSelectedEvent;

	private void Start()
	{
		CurrentTabIndex = -1;
		bool flag = false;
		for (int i = 0; i < buttons.Length; i++)
		{
			UIToggle obj = buttons[i];
			EventDelegate.Add(obj.onChange, OnButtonStateChanged);
			if (obj.value)
			{
				flag = true;
			}
			else if (contentPanels != null && i < contentPanels.Length)
			{
				contentPanels[i].SetActive(value: false);
			}
		}
		if (!flag)
		{
			SelectTab(0);
		}
		hasBeenInitialised = true;
		RestoreLastTabIndex();
	}

	private void OnEnable()
	{
		if (hasBeenInitialised && this.OnNewTabSelectedEvent != null)
		{
			this.OnNewTabSelectedEvent(CurrentTabIndex);
		}
	}

	private void OnDisable()
	{
		StoreLastTabIndex();
	}

	private void RestoreLastTabIndex()
	{
		UITabs componentInChildren = GetComponentInChildren<UITabs>();
		if (componentInChildren != null && lastTabIndex >= 0)
		{
			componentInChildren.SetSelectedTab(lastTabIndex);
		}
	}

	private void StoreLastTabIndex()
	{
		UITabs componentInChildren = GetComponentInChildren<UITabs>();
		if (componentInChildren != null)
		{
			lastTabIndex = componentInChildren.CurrentTabIndex;
		}
	}

	public void SelectTab(int index)
	{
		if (buttons != null)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].Set(state: false, notify: false);
			}
			if (index < buttons.Length && buttons[index] != null)
			{
				buttons[index].value = true;
			}
			else
			{
				Debug.LogWarning("Could not find button with index: " + index);
			}
		}
	}

	private void OnButtonStateChanged()
	{
		UIToggle current = UIToggle.current;
		if (current.value)
		{
			int tabIndex = GetTabIndex(current);
			SetSelectedTab(tabIndex);
		}
	}

	public virtual void SetSelectedTab(int tabIndex)
	{
		if (tabIndex >= 0 && tabIndex < contentPanels.Length)
		{
			int currentTabIndex = CurrentTabIndex;
			CurrentTabIndex = tabIndex;
			SwitchContent(currentTabIndex, tabIndex);
		}
	}

	private void SwitchContent(int oldTabIndex, int newTabIndex)
	{
		if (oldTabIndex == newTabIndex)
		{
			return;
		}
		if (contentPanels != null && contentPanels.Length != 0)
		{
			if (oldTabIndex == -1)
			{
				contentPanels[newTabIndex].SetActive(value: true);
			}
			else if (contentPanels[oldTabIndex] != contentPanels[newTabIndex])
			{
				contentPanels[oldTabIndex].SetActive(value: false);
				contentPanels[newTabIndex].SetActive(value: true);
			}
		}
		if (this.OnNewTabSelectedEvent != null)
		{
			this.OnNewTabSelectedEvent(newTabIndex);
		}
	}

	private int GetTabIndex(UIToggle clickedGameObject)
	{
		int num = 0;
		UIToggle[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == clickedGameObject)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public UIToggle GetButton(int index)
	{
		if (buttons != null && index >= 0 && index < buttons.Length)
		{
			return buttons[index];
		}
		return null;
	}

	public void SetRedDot(int index, bool show)
	{
		EnsureRedDots();
		if (redDots != null && index >= 0 && index < redDots.Length)
		{
			Helpers.GameObjectSetActive(redDots[index], show);
		}
	}

	private void EnsureRedDots()
	{
		if (redDots == null && buttons != null)
		{
			redDots = new GameObject[buttons.Length];
			for (int i = 0; i < buttons.Length; i++)
			{
				redDots[i] = ((buttons[i] != null) ? buttons[i].gameObject.FindInChildren("Red") : null);
				Helpers.GameObjectSetActive(redDots[i], value: false);
			}
		}
	}

	public GameObject GetContent(int index)
	{
		if (contentPanels != null && index < contentPanels.Length)
		{
			return contentPanels[index];
		}
		return null;
	}
}
