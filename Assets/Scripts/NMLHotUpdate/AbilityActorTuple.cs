using TWDModel;

public class AbilityActorTuple
{
	private AbilityModel ability;

	private ActorModel actor;

	public AbilityModel Ability
	{
		get
		{
			return ability;
		}
		private set
		{
			ability = value;
		}
	}

	public ActorModel Actor
	{
		get
		{
			return actor;
		}
		private set
		{
			actor = value;
		}
	}

	public AbilityActorTuple(AbilityModel ability, ActorModel actor)
	{
		Ability = ability;
		Actor = actor;
	}
}
