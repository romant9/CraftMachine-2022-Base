using Newtonsoft.Json;

namespace TWDModel
{
	public class CampObjectModel : TWDModelObject
	{
		protected int HpBoost;

		public GridPosition GridPosition { get; set; }

		[JsonIgnore]
		public GridSize Size { get; protected set; }

		[JsonIgnore]
		public CampModel Camp { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			GridPosition = new GridPosition();
		}

		public override bool IsValid()
		{
			return Camp != null;
		}

		public void SetGridPosition(FixedVec2 gridPosition)
		{
			SetGridPosition(gridPosition.X, gridPosition.Y);
		}

		public void SetGridPosition(FixedPoint x, FixedPoint y)
		{
			GridPosition = new GridPosition(x, y);
			NotifyChange("position");
		}
	}
}
