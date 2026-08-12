using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ActorSayNode : ClientNodeBase
{
	[Tooltip("Who should say the line.")]
	public DialogSource DialogSource;

	[Tooltip("Optional actor tag.")]
	public int ActorTagHash;

	[Tooltip("Voice-over to be played if any")]
	public int VoiceOverIndex;

	[GraphItVariable("Localization key")]
	public string LocalizationKey;

	[GraphItImportData("Instigator", "")]
	public ActorModel Instigator => Import("Instigator") as ActorModel;

	public override void OnNodeBind()
	{
	}

	[GraphItInput("Activate", "")]
	public void Activate()
	{
		List<string> list = new List<string>();
		List<ActorModel> factionActors = GameManager.Instance.playerModel.Combat.GetFactionActors(Faction.Survivor);
		List<string> list2 = new List<string> { "Survivor_A", "Survivor_B", "Survivor_C" };
		string text = list2[0];
		int num = 0;
		switch (DialogSource)
		{
		case DialogSource.Any:
			num = Random.Range(0, list2.Count);
			text = list2[num];
			break;
		case DialogSource.Instigator:
		{
			for (int j = 0; j < factionActors.Count; j++)
			{
				if (factionActors[j] == Instigator && j < list2.Count)
				{
					text = list2[j];
					break;
				}
			}
			break;
		}
		case DialogSource.Non_Instigator:
		{
			for (int i = 0; i < factionActors.Count; i++)
			{
				if (factionActors[i] == Instigator && i < list2.Count)
				{
					list2.RemoveAt(i);
					break;
				}
			}
			num = Random.Range(0, list2.Count);
			text = list2[num];
			break;
		}
		case DialogSource.Survivor_A:
			text = list2[0];
			break;
		case DialogSource.Survivor_B:
		{
			int index2 = ((factionActors.Count > 1) ? 1 : 0);
			text = list2[index2];
			break;
		}
		case DialogSource.Survivor_C:
		{
			int index = ((factionActors.Count > 2) ? 2 : 0);
			text = list2[index];
			break;
		}
		case DialogSource.ActorWithTag:
			text = "Tag_" + ActorTagHash;
			break;
		}
		if (VoiceOverIndex != 0)
		{
			string item = "VoiceOver," + VoiceOverIndex;
			list.Add(item);
		}
		if (text != null)
		{
			string item2 = "Dialog," + text + "," + LocalizationKey;
			list.Add(item2);
		}
		if (list != null && list.Count > 0)
		{
			DialogVisualizationTask task = new DialogVisualizationTask(list);
			VisualizationQueue.Instance.Add(task);
		}
		Started();
	}

	[GraphItOutput("Started", "")]
	public void Started()
	{
		Fire("Started");
	}
}
