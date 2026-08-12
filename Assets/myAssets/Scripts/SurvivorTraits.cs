using System.Collections.Generic;
using BaseModel;
using TWDModel;


public class SurvivorTraits
{
	public SurvivorModel Survivor { get; set; }
	public bool[] TraitRerolledList  = new bool[5];

	public List<UpgradeTraitsData> UpgradeTraits { get; set; }

    public string equipmentItemModel { get; set; }

    public List<string> RandomTraitsFromReroll { get; set; }

    public List<string> PreviousRandomRolledTraits { get; set; }

	public ModelRandom random { get; set; }
    public ModelRandom equiRandom { get; set; }

    public TraitDefinition traitDefinitionCurrent { get; set; }

	public string TraitToBeRerolledCandidate { get; set; }
}

