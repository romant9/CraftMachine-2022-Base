using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public abstract class DebuffParameterBase
	{
		public string ParameterKey { get; set; }

		public int ExpiryTurn { get; set; }

		[IgnoreModelProperty]
		public ActorModel Source { get; private set; }

		[JsonIgnore]
		public Faction AppliedFaction => Source?.Faction ?? Faction.Survivor;

		public abstract bool IgnoreExpiryTurn { get; }

		public abstract Faction ExpirationCheckFactionTurn { get; }

		public DebuffParameterBase(DebuffParameterBase debuffDebuffParameterBase)
		{
			ParameterKey = debuffDebuffParameterBase.ParameterKey;
			ExpiryTurn = debuffDebuffParameterBase.ExpiryTurn;
		}

		public DebuffParameterBase()
		{
		}

		public DebuffParameterBase(string parameterKey)
		{
			ParameterKey = parameterKey;
		}

		public DebuffParameterBase(string parameterKey, int expiryTurn)
		{
			ParameterKey = parameterKey;
			ExpiryTurn = expiryTurn;
		}

		public void SetSource(ActorModel source)
		{
			Source = source;
		}
	}
}
