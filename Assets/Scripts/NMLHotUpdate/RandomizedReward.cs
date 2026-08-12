using System;
using BaseModel;

public abstract class RandomizedReward
{
	public virtual ModelRandom GetRandom(object[] param)
	{
		ModelRandom modelRandom = null;
		if (param != null && param.Length != 0)
		{
			modelRandom = (ModelRandom)param[0];
		}
		if (modelRandom == null)
		{
			throw new Exception("Cannot randomize reward with null random parameter. Ensure that RandomizedRewards get ModelRandom.");
		}
		return modelRandom;
	}
}
