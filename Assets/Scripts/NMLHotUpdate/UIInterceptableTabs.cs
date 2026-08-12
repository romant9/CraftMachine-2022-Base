using System.Linq;

public class UIInterceptableTabs : UITabs
{
	private UIButtonToggle[] buttonToggles;

	private IInterceptor interceptor;

	private void Awake()
	{
		buttonToggles = buttons.Select((UIToggle x) => x.GetComponent<UIButtonToggle>()).ToArray();
	}

	public override async void SetSelectedTab(int tabIndex)
	{
		if (tabIndex != base.CurrentTabIndex)
		{
			bool flag = interceptor == null;
			if (!flag)
			{
				flag = await interceptor.Intercept();
			}
			if (flag)
			{
				base.SetSelectedTab(tabIndex);
				return;
			}
			buttons[base.CurrentTabIndex].Set(state: true);
			buttonToggles[base.CurrentTabIndex]?.ForceClick();
		}
	}

	public void SetInterceptor(IInterceptor i)
	{
		interceptor = i;
	}
}
