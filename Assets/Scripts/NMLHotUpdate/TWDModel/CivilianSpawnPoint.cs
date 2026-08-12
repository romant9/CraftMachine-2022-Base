namespace TWDModel
{
	public class CivilianSpawnPoint : TWDModelObject
	{
		public GridCoordinate Coordinate;

		public override void Start()
		{
			base.Start();
			base.Debug.LogError("Legacy Civilian spawn point used. Remove it from level");
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
