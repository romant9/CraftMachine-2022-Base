public class ComponentInventoryButton : RecipeComponentView
{
	public void SetClickCallback(UIButtonExtended.OnClickCallback callback)
	{
		if (button != null)
		{
			button.SetClickCallback(callback);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GetData() != null)
		{
			Initialize(GetData().Type, GetData().Value);
		}
	}
}
