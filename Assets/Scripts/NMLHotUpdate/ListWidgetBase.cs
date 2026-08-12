public class ListWidgetBase : UIListCard<string>
{
	protected string DebugClassString = "ListWidgetBase";

	public virtual void Awake()
	{
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
	}

	public virtual void Activate()
	{
	}

	public virtual void Deactivate()
	{
	}

	public virtual void AddedToList()
	{
		UpdateUI();
	}

	protected void DebugLog(string message)
	{
	}

	protected void DebugLogWarning(string message)
	{
		Debug.LogWarning(DebugClassString + ": " + message);
	}

	protected void DebugLogError(string message)
	{
		Debug.LogError(DebugClassString + ": " + message);
	}
}
