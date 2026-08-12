using BaseModel;

public abstract class ModelView<T> : ModelViewBase where T : ModelObject
{
	protected bool RegisterViewToModel = true;

	public T Model { get; set; }

	public bool IsInitialized => Model != null;

	public void setRegisterViewToModel(bool value)
	{
		RegisterViewToModel = value;
	}

	public virtual void Initialize(ModelObject model)
	{
		Model = model as T;
		if (RegisterViewToModel)
		{
			GameManager.Instance.RegisterViewWithModel(Model, this);
		}
	}
}
