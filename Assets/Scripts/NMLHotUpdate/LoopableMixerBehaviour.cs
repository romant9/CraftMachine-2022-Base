using System;
using System.Collections.Generic;
using UnityEngine.Playables;

[Serializable]
public class LoopableMixerBehaviour : PlayableBehaviour
{
	public Dictionary<string, double> MarkerClips;

	private PlayableDirector director;

	public override void OnPlayableCreate(Playable playable)
	{
		director = playable.GetGraph().GetResolver() as PlayableDirector;
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (director == null)
		{
			return;
		}
		int inputCount = playable.GetInputCount();
		for (int i = 0; i < inputCount; i++)
		{
			float inputWeight = playable.GetInputWeight(i);
			double time = director.time;
			LoopableBehaviour behaviour = ((ScriptPlayable<LoopableBehaviour>)playable.GetInput(i)).GetBehaviour();
			if (!(inputWeight > 0f) || behaviour.ClipExecuted)
			{
				continue;
			}
			switch (behaviour.Action)
			{
			case TimelineAction.Pause:
				if (!behaviour.ConditionMet())
				{
					director.time = time;
					behaviour.ClipExecuted = false;
				}
				else
				{
					director.Play();
					behaviour.ClipExecuted = true;
				}
				break;
			case TimelineAction.JumpToTime:
			case TimelineAction.JumpToMarker:
				if (behaviour.ConditionMet())
				{
					if (behaviour.Action == TimelineAction.JumpToTime)
					{
						director.time = behaviour.TimeToJumpTo;
					}
					else
					{
						double time2 = MarkerClips[behaviour.MarkerToJumpTo];
						director.time = time2;
					}
					behaviour.ClipExecuted = false;
				}
				break;
			}
		}
	}
}
