using BaseModel;

namespace TWDModel
{
	public class PreChangeTurnAction : ModelAction
	{
		public Faction CurrentActiveFaction { get; private set; }

		public Faction NextActiveFaction { get; private set; }

		public PreChangeTurnAction(Faction currentActiveFaction, Faction nextActiveFaction)
			: base(null)
		{
			CurrentActiveFaction = currentActiveFaction;
			NextActiveFaction = nextActiveFaction;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
