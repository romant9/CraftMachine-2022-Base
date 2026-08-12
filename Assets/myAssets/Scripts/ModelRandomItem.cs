using BaseModel;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ModelRandomItem
{  
    public int randomIndex { get; set; }

    public string Description { get; set; }

    public ModelRandom modelRandom { get; set; }

    public RandomChangeType randomChangeType { get; set; }

    public RandomSource randomSource { get; set; }

    public ModelRandomItem(int randomIndex, ModelRandom modelRandom, RandomChangeType randomType = RandomChangeType.Reload_Game, RandomSource randomSource = RandomSource.Player_Random, string description = "")
    {
        this.randomIndex = randomIndex;
        Description = description;
        this.modelRandom = modelRandom;
        this.randomChangeType = randomType;
        this.randomSource = randomSource;
    }
}
