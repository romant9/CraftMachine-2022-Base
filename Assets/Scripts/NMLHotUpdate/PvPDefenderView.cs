using TWDModel;

public class PvPDefenderView : ModelView<PvPDefenderModel>
{
	public SurvivorClass SurvivorClass = SurvivorClass.Scout;

	public int BaseLevel = 1;

	public int UpgradeSteps;

	public int RarityLevel;

	public ActorGender Gender;

	public int EquipmentBaseLevel;

	public int EquipmentUpgradeSteps;

	public int EquipmentRarityLevel;

	public AIMode DefensiveMode = AIMode.Stationary;
}
