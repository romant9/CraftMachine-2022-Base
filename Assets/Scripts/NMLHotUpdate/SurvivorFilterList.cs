using System.Collections.Generic;
using TWDModel;

public class SurvivorFilterList : ISurvivorFilterList
{
	public List<SurvivorModel> CurrentSurvivors { get; private set; }

	public SurvivorFilterList(List<SurvivorModel> list)
	{
		CurrentSurvivors = list;
	}

	public bool CanSwitchSurvivor()
	{
		if (CurrentSurvivors != null)
		{
			return CurrentSurvivors.Count > 1;
		}
		return false;
	}

	public SurvivorModel GetNextSurvivor(SurvivorModel currentModel)
	{
		SurvivorModel result = null;
		if (CurrentSurvivors != null && currentModel != null)
		{
			int num = CurrentSurvivors.IndexOf(currentModel);
			result = ((num < 0 || num + 1 >= CurrentSurvivors.Count) ? CurrentSurvivors[0] : CurrentSurvivors[++num]);
		}
		return result;
	}

	public SurvivorModel GetPreviousSurvivor(SurvivorModel currentModel)
	{
		SurvivorModel result = null;
		if (CurrentSurvivors != null && currentModel != null)
		{
			int num = CurrentSurvivors.IndexOf(currentModel);
			result = ((num <= 0) ? CurrentSurvivors[CurrentSurvivors.Count - 1] : CurrentSurvivors[num - 1]);
		}
		return result;
	}

	public void Clear()
	{
		if (CurrentSurvivors != null)
		{
			CurrentSurvivors.Clear();
		}
	}

	public ISurvivorFilterList Copy()
	{
		if (CurrentSurvivors != null)
		{
			return new SurvivorFilterList(new List<SurvivorModel>(CurrentSurvivors.ToArray()));
		}
		return null;
	}
}
