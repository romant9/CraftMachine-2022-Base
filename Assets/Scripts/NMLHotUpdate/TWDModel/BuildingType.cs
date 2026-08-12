using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BuildingType
	{
		public string Name;

		public int SizeX;

		public int SizeY;

		public string RequiredBuilding;

		public BuildingCategory Category;

		public bool CanMove;

		public bool DisableUpgrade;

		public CurrencyType ProductionType;

		[JsonIgnore]
		public GridSize Size => new GridSize(SizeX, SizeY);
	}
}
