public class CombatEndWidgetList : ScrollableWidgetListPanel<ListWidgetBase>
{
	public UIScrollView ScrollView => scrollView;

	public void SetDragAmount(float x, float y, bool updateScrollbars)
	{
		if (scrollView != null)
		{
			scrollView.SetDragAmount(x, y, updateScrollbars);
		}
	}
}
