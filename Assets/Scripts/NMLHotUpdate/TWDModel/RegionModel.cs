using System.Collections.Generic;

namespace TWDModel
{
	public class RegionModel : TWDSpatialModelObject
	{
		public override void Initialize()
		{
		}

		public RegionModel()
		{
		}

		public RegionModel(string viewId, List<GridCoordinate> gridCoordinates)
			: this()
		{
			base.ViewId = viewId;
			base.Location = new TWDObjectLocation(gridCoordinates, null);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
