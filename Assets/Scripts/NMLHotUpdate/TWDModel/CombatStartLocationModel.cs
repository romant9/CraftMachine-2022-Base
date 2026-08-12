namespace TWDModel
{
	public class CombatStartLocationModel : TWDModelObject
	{
		public GridCoordinate Location;

		public int ActorTagHash;

		public int Order;

		public CombatStartLocationModel()
		{
		}

		public CombatStartLocationModel(GridCoordinate inCoordinate, int inTagHash, int inOrder)
		{
			Location = inCoordinate;
			ActorTagHash = inTagHash;
			Order = inOrder;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
