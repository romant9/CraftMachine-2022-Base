public class NUIListItem<T> : NUIListItemBase where T : class
{
	private T dataInternal;

	public virtual void SetData(T data)
	{
		dataInternal = data;
	}

	public virtual T GetData()
	{
		return dataInternal;
	}

	public override void Clear()
	{
		base.Clear();
		dataInternal = null;
	}
}
