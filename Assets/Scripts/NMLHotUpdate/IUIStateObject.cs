public interface IUIStateObject : IUIState
{
	UIStateMachine stateMachine { get; }

	void Init();

	void Added(UIStateMachine addedToStateMachine);

	void Enter();

	void Update();

	void UpdateUI();

	void Exit();

	bool AllowExit();

	bool AllowAddToHistory();
}
