using System.Collections.Generic;
using TWDModel;

public class LockedHeroSurvivorFilterList : ISurvivorFilterList
{
	public List<string> CurrentSurvivors { get; private set; }

	public LockedHeroSurvivorFilterList(List<string> list)
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
			string actorDefinitionID = currentModel.ActorDefinitionID;
			int num = CurrentSurvivors.IndexOf(actorDefinitionID);
			string text = null;
			text = ((num < 0 || num + 1 >= CurrentSurvivors.Count) ? CurrentSurvivors[0] : CurrentSurvivors[++num]);
			ActorDefinition actorDefinition = GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(text);
			if (actorDefinition != null)
			{
				result = HeroUnlockHelper.GetOrCreateMockSurvivorModel(actorDefinition);
			}
		}
		return result;
	}

	public SurvivorModel GetPreviousSurvivor(SurvivorModel currentModel)
	{
		SurvivorModel result = null;
		if (CurrentSurvivors != null && currentModel != null)
		{
			string actorDefinitionID = currentModel.ActorDefinitionID;
			int num = CurrentSurvivors.IndexOf(actorDefinitionID);
			string text = null;
			text = ((num <= 0) ? CurrentSurvivors[CurrentSurvivors.Count - 1] : CurrentSurvivors[num - 1]);
			ActorDefinition actorDefinition = GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(text);
			if (actorDefinition != null)
			{
				result = HeroUnlockHelper.GetOrCreateMockSurvivorModel(actorDefinition);
			}
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
		return new LockedHeroSurvivorFilterList(new List<string>(CurrentSurvivors.ToArray()));
	}
}
