public class UIStateObjectBase : IUIStateObject, IUIState
{
	private UIStateMachine stateMachineInternal;

	public virtual int Id { get; set; }

	public virtual UIStateMachine stateMachine => stateMachineInternal;

	public virtual void Init()
	{
	}

	public virtual void Added(UIStateMachine addedToStateMachine)
	{
		stateMachineInternal = addedToStateMachine;
	}

	public virtual void Enter()
	{
		UpdateUI();
	}

	public virtual void Update()
	{
	}

	public virtual void UpdateUI()
	{
	}

	public virtual void Exit()
	{
	}

	public virtual bool AllowExit()
	{
		return true;
	}

	public virtual bool AllowAddToHistory()
	{
		return false;
	}
}
