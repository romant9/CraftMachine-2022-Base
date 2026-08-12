using TWDModel;

public interface ISurvivorFilterList
{
	bool CanSwitchSurvivor();

	SurvivorModel GetNextSurvivor(SurvivorModel model);

	SurvivorModel GetPreviousSurvivor(SurvivorModel model);

	void Clear();

	ISurvivorFilterList Copy();
}
