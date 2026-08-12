using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class PersonalityManager : MonoBehaviour
{
	public float WalkerKillCooldown = 30f;

	public float WalkerKillChance = 50f;

	private static PersonalityManager instance;

	private Dictionary<ActorModel, float> ActorWalkerKillCooldowns = new Dictionary<ActorModel, float>();

	public static PersonalityManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<PersonalityManager>();
			}
			return instance;
		}
	}

	public void ReactToWalkerKill(ActorModel actor)
	{
		TraitDefinition traitWithTag = actor.GetTraitWithTag("Personality");
		if (traitWithTag == null)
		{
			return;
		}
		bool flag = false;
		if (ActorWalkerKillCooldowns.ContainsKey(actor))
		{
			if (Time.time - ActorWalkerKillCooldowns[actor] > WalkerKillCooldown)
			{
				ActorWalkerKillCooldowns.Remove(actor);
			}
			else
			{
				flag = true;
			}
		}
		if (!flag && Random.Range(0f, 100f) < WalkerKillChance && TryStartDialog(actor, traitWithTag.Identifier, "WalkerKill"))
		{
			ActorWalkerKillCooldowns.Add(actor, Time.time);
		}
	}

	private bool TryStartDialog(ActorModel actor, string trait, string action)
	{
		string text = "";
		List<string> keysThatContain = SingularityMonoBehaviour<LocalizationManager>.Instance.GetKeysThatContain("Personality." + trait + "." + action);
		if (keysThatContain != null && keysThatContain.Count > 0)
		{
			text = keysThatContain[Random.Range(0, keysThatContain.Count)];
		}
		if (text != null && text.Length > 0)
		{
			int num = -1;
			List<ActorModel> factionActors = GameManager.Instance.playerModel.Combat.GetFactionActors(Faction.Survivor);
			for (int i = 0; i < factionActors.Count; i++)
			{
				if (factionActors[i] == actor)
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				List<string> list = new List<string> { "Survivor_A", "Survivor_B", "Survivor_C" };
				List<string> list2 = new List<string>();
				string item = "Dialog," + list[num] + "," + text;
				list2.Add(item);
				DialogVisualizationTask task = new DialogVisualizationTask(list2);
				VisualizationQueue.Instance.Add(task);
				return true;
			}
		}
		return false;
	}
}
