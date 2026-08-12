using System.Collections.Generic;
using UnityEngine;

public class UIStateMachine : MonoBehaviourExtended
{
	public List<IUIStateObject> StatesList = new List<IUIStateObject>();

	public bool LockCurrentState;

	private int defaultState = -1;

	private Stack<int> statesHistory = new Stack<int>();

	private IUIStateObject currentStateObject;

	public int currentStateId
	{
		get
		{
			if (currentStateObject != null)
			{
				return currentStateObject.Id;
			}
			DebugLogError("Could not get the current state. State is NULL");
			return -1;
		}
	}

	public IUIStateObject currentState
	{
		get
		{
			if (currentStateObject == null)
			{
				DebugLogError("Could not get the current state object. Object is NULL");
			}
			return currentStateObject;
		}
	}

	public virtual void Awake()
	{
		DebugIdString = "UIStateMachine";
	}

	public override void Clear()
	{
		base.Clear();
		StatesList = new List<IUIStateObject>();
		currentStateObject = null;
		LockCurrentState = false;
		ClearHistory();
	}

	public virtual void Update()
	{
		if (currentStateObject != null)
		{
			currentStateObject.Update();
		}
	}

	public virtual void UpdateUI()
	{
		if (currentStateObject != null)
		{
			currentStateObject.UpdateUI();
		}
	}

	public static UIStateMachine AddTo(GameObject obj)
	{
		if (obj != null)
		{
			UIStateMachine component = obj.GetComponent<UIStateMachine>();
			if (!(component != null))
			{
				return obj.AddComponent<UIStateMachine>();
			}
			return component;
		}
		return null;
	}

	public void AddState(IUIStateObject state)
	{
		if (IsNotNull(state))
		{
			state.Init();
			if (GetState(state.Id) == null)
			{
				StatesList.Add(state);
				state.Added(this);
			}
			else
			{
				DebugLogWarning("Cant add state with id: " + state.Id + ". Already contains state!");
			}
		}
	}

	public bool TrySwitchToState(int stateId, bool forceUpdate = false)
	{
		if (IsLockActive())
		{
			return false;
		}
		if (!IsExitAllowed(currentStateObject))
		{
			return false;
		}
		IUIStateObject state = GetState(stateId);
		if (state == null && defaultState != -1)
		{
			DebugLog("TrySwitchToState(), State: " + stateId + " not found. Falling back on default State: " + defaultState);
			state = GetState(defaultState);
		}
		if (IsNotNull(state, "TrySwitchToState(), Could not find state with id: " + stateId))
		{
			if (forceUpdate || currentStateObject != state)
			{
				if (currentStateObject != null)
				{
					if (!currentStateObject.AllowExit())
					{
						DebugLog("Current state:" + currentStateObject.Id + " did not allow exit");
						return false;
					}
					currentStateObject.Exit();
				}
				currentStateObject = state;
				currentStateObject.Enter();
				if (currentStateObject.AllowAddToHistory() && (statesHistory.Count == 0 || statesHistory.Peek() != stateId))
				{
					statesHistory.Push(stateId);
				}
			}
			return true;
		}
		return false;
	}

	public bool SwitchToPreviousState(bool allowDefaultState = true)
	{
		if (IsLockActive())
		{
			return false;
		}
		if (statesHistory.Count > 0 && statesHistory.Peek() == currentStateId)
		{
			statesHistory.Pop();
		}
		bool flag = false;
		if (statesHistory.Count > 0)
		{
			if (!IsExitAllowed(GetState(statesHistory.Peek())))
			{
				return false;
			}
			flag = TrySwitchToState(statesHistory.Pop());
		}
		else
		{
			DebugLog("No Previous Items in State Queque");
		}
		if (!flag && allowDefaultState)
		{
			DebugLog("Could not SwitchToPreviousState. Trying to use default state: " + defaultState);
			flag = TrySwitchToState(defaultState);
		}
		return flag;
	}

	public void ClearHistory()
	{
		statesHistory = new Stack<int>();
	}

	public Stack<int> GetHistory()
	{
		return statesHistory;
	}

	public void SetHistory(Stack<int> history)
	{
		statesHistory = history;
	}

	public IUIStateObject GetState(int stateId)
	{
		for (int i = 0; StatesList.Count > i; i++)
		{
			if (StatesList[i] != null && StatesList[i].Id == stateId)
			{
				return StatesList[i];
			}
		}
		return null;
	}

	public void SetDefaultState(int stateId)
	{
		if (GetState(stateId) != null)
		{
			defaultState = stateId;
		}
		else
		{
			DebugLogError("Can't set state: " + stateId + ", as default. State does not exist! Please use UIStateMachine.AddState(). To add the state first.");
		}
	}

	private bool IsLockActive()
	{
		if (LockCurrentState)
		{
			DebugLogWarning("Cannot switch state. LOCK ACTIVE!");
			return LockCurrentState;
		}
		return LockCurrentState;
	}

	private bool IsExitAllowed(IUIStateObject stateObj)
	{
		if (stateObj != null && !stateObj.AllowExit())
		{
			DebugLog("Exit blocked by state: " + stateObj.Id);
			return false;
		}
		return true;
	}
}
