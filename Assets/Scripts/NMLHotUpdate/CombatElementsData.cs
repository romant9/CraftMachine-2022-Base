using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CombatElementsData
{
	private static CombatElementsData instance;

	public Dictionary<Faction, List<ActorVisualRepresentation>> GenericActorVisualRepresentations { get; private set; }

	public Dictionary<Faction, List<string>> GenericActorNames { get; private set; }

	public static CombatElementsData Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CombatElementsData();
			}
			return instance;
		}
	}

	private CombatElementsData()
	{
		ActorVisualRepresentation item = new ActorVisualRepresentation
		{
			Icon = "twdproto_character_icon1",
			Prefab = "Characters/ActorSurvivor"
		};
		ActorVisualRepresentation item2 = new ActorVisualRepresentation
		{
			Icon = "twdproto_character_icon1",
			Prefab = "Characters/ActorWalker"
		};
		ActorVisualRepresentation item3 = new ActorVisualRepresentation
		{
			Icon = "twdproto_character_icon1",
			Prefab = "Characters/ActorWalker"
		};
		GenericActorVisualRepresentations = new Dictionary<Faction, List<ActorVisualRepresentation>>();
		GenericActorVisualRepresentations[Faction.Survivor] = new List<ActorVisualRepresentation>();
		GenericActorVisualRepresentations[Faction.Survivor].Add(item);
		GenericActorVisualRepresentations[Faction.Walker] = new List<ActorVisualRepresentation>();
		GenericActorVisualRepresentations[Faction.Walker].Add(item2);
		GenericActorVisualRepresentations[Faction.Dormant] = new List<ActorVisualRepresentation>();
		GenericActorVisualRepresentations[Faction.Dormant].Add(item3);
		GenericActorNames = new Dictionary<Faction, List<string>>();
		GenericActorNames[Faction.Survivor] = new List<string>();
		GenericActorNames[Faction.Survivor].Add("Generic Survivor");
		GenericActorNames[Faction.Walker] = new List<string>();
		GenericActorNames[Faction.Walker].Add("Generic Walker");
		GenericActorNames[Faction.Dormant] = new List<string>();
		GenericActorNames[Faction.Dormant].Add("Generic Dormant");
	}

	public ActorDefinition GenerateRandomActorDefinitionForFaction(Faction actorFaction)
	{
		ActorDefinition actorDefinition = new ActorDefinition();
		List<string> list = GenericActorNames[actorFaction];
		actorDefinition.Name = list[Random.Range(0, list.Count)];
		return actorDefinition;
	}
}
