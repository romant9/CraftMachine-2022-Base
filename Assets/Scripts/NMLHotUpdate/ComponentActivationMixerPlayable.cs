using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ComponentActivationMixerPlayable : PlayableBehaviour
{
	public ActivationTrack.PostPlaybackState PostPlaybackState;

	private bool boundComponentInitialStateIsActive;

	private MonoBehaviour boundComponent;

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (boundComponent == null)
		{
			boundComponent = playerData as MonoBehaviour;
			boundComponentInitialStateIsActive = boundComponent != null && boundComponent.enabled;
		}
		if (boundComponent == null)
		{
			return;
		}
		int inputCount = playable.GetInputCount();
		if (inputCount == 0)
		{
			return;
		}
		bool enabled = false;
		for (int i = 0; i < inputCount; i++)
		{
			if (playable.GetInputWeight(i) > 0f)
			{
				enabled = true;
				break;
			}
		}
		boundComponent.enabled = enabled;
	}

	public override void OnPlayableDestroy(Playable playable)
	{
		if (!(boundComponent == null))
		{
			switch (PostPlaybackState)
			{
			case ActivationTrack.PostPlaybackState.Active:
				boundComponent.enabled = true;
				break;
			case ActivationTrack.PostPlaybackState.Inactive:
				boundComponent.enabled = false;
				break;
			case ActivationTrack.PostPlaybackState.Revert:
				boundComponent.enabled = boundComponentInitialStateIsActive;
				break;
			case ActivationTrack.PostPlaybackState.LeaveAsIs:
				break;
			}
		}
	}
}
