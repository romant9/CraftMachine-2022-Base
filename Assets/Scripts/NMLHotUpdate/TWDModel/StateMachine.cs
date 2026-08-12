using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class StateMachine<T>
	{
		public delegate void StateChangeHandler(T otherState);

		public delegate void StateUpdateHandler(long deltaTime);

		protected class StateDefinition
		{
			public T State;

			public StateChangeHandler EnterHandler;

			public StateUpdateHandler UpdateHandler;

			public StateChangeHandler LeaveHandler;
		}

		[JsonIgnore]
		protected Dictionary<T, StateDefinition> states = new Dictionary<T, StateDefinition>();

		[JsonIgnore]
		protected StateDefinition currentState;

		public T State
		{
			get
			{
				return currentState.State;
			}
			set
			{
				SetState(value);
			}
		}

		public long TimeInState { get; protected set; }

		public event StateChangeHandler StateChanged;

		public void AddState(T state, StateChangeHandler enterHandler, StateUpdateHandler updateHandler, StateChangeHandler leaveHandler)
		{
			StateDefinition stateDefinition = new StateDefinition();
			stateDefinition.State = state;
			stateDefinition.EnterHandler = enterHandler;
			stateDefinition.UpdateHandler = updateHandler;
			stateDefinition.LeaveHandler = leaveHandler;
			states.Add(state, stateDefinition);
		}

		public void Update(long deltaTime)
		{
			TimeInState += deltaTime;
			if (currentState.UpdateHandler != null)
			{
				currentState.UpdateHandler(deltaTime);
			}
		}

		protected void SetState(T newState)
		{
			T otherState = default(T);
			if (currentState != null)
			{
				if (State.Equals(newState))
				{
					return;
				}
				otherState = State;
				if (currentState.LeaveHandler != null)
				{
					currentState.LeaveHandler(newState);
				}
				TimeInState = 0L;
			}
			currentState = states[newState];
			if (currentState.EnterHandler != null)
			{
				currentState.EnterHandler(otherState);
			}
			if (State.Equals(newState) && this.StateChanged != null)
			{
				this.StateChanged(newState);
			}
		}
	}
}
