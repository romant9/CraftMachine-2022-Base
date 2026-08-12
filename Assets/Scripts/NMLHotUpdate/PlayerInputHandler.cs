using System;
using TWDModel;
using UnityEngine;

public abstract class PlayerInputHandler : IComparable<PlayerInputHandler>
{
	public virtual bool RequiresPlayerInputEnabled => true;

	public virtual bool TapOnly => false;

	public int ProcessedTapIndex { get; set; }

	public bool ClickThrough { get; protected set; }

	public virtual bool ResetOtherHandlers => true;

	protected PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;

	protected GridView GridView => GridView.Instance;

	protected GridModel Grid => GridView.Grid;

	protected CombatModel Combat => GameManager.Instance.playerModel.Combat;

	protected TurnManager TurnManager => GameManager.Instance.playerModel.Combat.TurnManager;

	protected BoxCollider GridCollider => GridView.GridCollider;

	public virtual int Priority => 0;

	public virtual void OnControlledActorChanged(ActorModel newControlledActor)
	{
	}

	public virtual void OnControlledActorPropertiesChanged(string changed, object args)
	{
	}

	public int CompareTo(PlayerInputHandler compareHandler)
	{
		if (compareHandler == null)
		{
			return 1;
		}
		return compareHandler.Priority - Priority;
	}

	public virtual void Initialize()
	{
	}

	public abstract bool CanHandleInteraction();

	public virtual void InteractionStarted()
	{
	}

	public virtual void InteractionStopped()
	{
	}

	public virtual void Reset()
	{
	}

	public virtual bool UpdateInteraction(float deltaTime)
	{
		return true;
	}

	public virtual void Update(float deltaTime)
	{
	}
}
