using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

[ExecuteInEditMode]
public class CombatDialogPlayerView : ModelView<CombatDialogPlayerModel>
{
	public List<DialogLine> DialogLines;

	public string guid;

	public string DialogId;

	public List<string> Actions;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
	}

	public void OnDestroy()
	{
		if (base.Model != null)
		{
			base.Model.Changed -= OnModelChanged;
		}
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		if (!(changed == "DialogPlayerTriggered"))
		{
			return;
		}
		ActorModel actorModel = args as ActorModel;
		List<string> list = new List<string>();
		for (int i = 0; i < DialogLines.Count; i++)
		{
			DialogLine dialogLine = DialogLines[i];
			List<SurvivorModel> missionRoster = base.Model.manager.Player.Combat.MissionRoster;
			List<string> list2 = new List<string> { "Survivor_A", "Survivor_B", "Survivor_C" };
			int index = 0;
			string text = list2[index];
			switch (dialogLine.DialogSource)
			{
			case DialogSource.Any:
			{
				List<int> list3 = new List<int>();
				for (int l = 0; l < missionRoster.Count; l++)
				{
					if (!missionRoster[l].IsDead)
					{
						list3.Add(l);
					}
				}
				index = Random.Range(0, list3.Count);
				text = list2[list3[index]];
				_ = missionRoster[list3[index]].Name;
				break;
			}
			case DialogSource.Instigator:
			{
				for (int k = 0; k < missionRoster.Count; k++)
				{
					if (missionRoster[k] == actorModel && k < list2.Count)
					{
						_ = missionRoster[k].Name;
						text = list2[k];
						break;
					}
				}
				break;
			}
			case DialogSource.Non_Instigator:
			{
				if (list2.Count > missionRoster.Count)
				{
					list2.RemoveRange(missionRoster.Count, list2.Count - missionRoster.Count);
				}
				for (int j = 0; j < missionRoster.Count; j++)
				{
					if (missionRoster[j] == actorModel && j < list2.Count)
					{
						list2.RemoveAt(j);
						break;
					}
				}
				index = Random.Range(0, list2.Count);
				text = list2[index];
				_ = missionRoster[index].Name;
				break;
			}
			case DialogSource.Survivor_A:
				text = list2[0];
				_ = missionRoster[0].Name;
				break;
			case DialogSource.Survivor_B:
			{
				int index3 = ((missionRoster.Count > 1) ? 1 : 0);
				text = list2[index3];
				_ = missionRoster[index3].Name;
				break;
			}
			case DialogSource.Survivor_C:
			{
				int index2 = ((missionRoster.Count > 2) ? 2 : 0);
				text = list2[index2];
				_ = missionRoster[index2].Name;
				break;
			}
			case DialogSource.ActorWithTag:
				text = "Tag_" + dialogLine.SourceActorTag;
				break;
			}
			if (dialogLine.VoiceOverIndex != 0)
			{
				string item = "VoiceOver," + dialogLine.VoiceOverIndex;
				list.Add(item);
			}
			if (text != null)
			{
				string item2 = "Dialog," + text + "," + dialogLine.LocalizationKey;
				list.Add(item2);
			}
		}
		if (list != null && list.Count > 0)
		{
			DialogVisualizationTask task = new DialogVisualizationTask(list);
			VisualizationQueue.Instance.Add(task);
		}
	}
}
