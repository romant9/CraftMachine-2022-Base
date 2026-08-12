using System;

namespace TWDModel
{
	[Serializable]
	public class BuildingsAmountsDefinition
	{
		public int CouncilLevel;

		public int WorkshopAmount;

		public int BuildingProduceSuppliesAmount;

		public int BuildingProduceGasAmount;

		public int BuildingStorageSuppliesAmount;

		public int BuildingStorageGasAmount;

		public int MissionCarAmount;

		public int TentsAmount;

		public int RadioTentAmount;

		public int GraveyardAmount;

		public int TrainingGroundAmount;

		public int MedicTentAmount;

		public int CageAmount;

		public int OutpostAmount;

		public int BuffBuildingCriticalChanceAmount;

		public int BuffBuildingDamageAmount;

		public int BuffBuildingHealthAmount;

		public int ScavengerAmount;

		public int ResidenceAmount;

		public int ArmoryAmount;

		public int GetAmountsForBuilding(string buildingTypeName)
		{
			return buildingTypeName switch
			{
				"Workshop" => WorkshopAmount,
				"BuildingProduceSupplies" => BuildingProduceSuppliesAmount,
				"BuildingProduceGas" => BuildingProduceGasAmount,
				"BuildingStorageSupplies" => BuildingStorageSuppliesAmount,
				"BuildingStorageGas" => BuildingStorageGasAmount,
				"MissionCar" => MissionCarAmount,
				"Tents" => TentsAmount,
				"RadioTent" => RadioTentAmount,
				"Graveyard" => GraveyardAmount,
				"TrainingGround" => TrainingGroundAmount,
				"MedicTent" => MedicTentAmount,
				"BuffBuildingCriticalChance" => BuffBuildingCriticalChanceAmount,
				"BuffBuildingDamage" => BuffBuildingDamageAmount,
				"BuffBuildingHealth" => BuffBuildingHealthAmount,
				"Cage" => CageAmount,
				"Outpost" => OutpostAmount,
				"Scavenger" => ScavengerAmount,
				"Residence" => ResidenceAmount,
				"Armory" => ArmoryAmount,
				_ => 0,
			};
		}
	}
}
