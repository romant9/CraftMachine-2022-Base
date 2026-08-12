using System;

namespace TWDModel
{
	[Serializable]
	public enum AbilityEffectType
	{
		ChangeHealth = 0,
		ChangeState = 1,
		InstantiateObject = 2,
		MoveSourceActorToTargetPosition = 3
	}
}
