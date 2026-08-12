using TWDModel;

public class SoundNode : ClientNodeBase
{
	[GraphItVariable("Sound type")]
	public SoundType levelSound;

	[Tooltip("Voice-over to be played if any")]
	public int VoiceOverIndex;

	public override void OnNodeBind()
	{
	}

	[GraphItInput("Play Sound", "")]
	public void PlaySound()
	{
		VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayLevelEvent(levelSound);
		}));
	}

	[GraphItInput("Play Voice-over", "")]
	public void PlayVoiceOver()
	{
		VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayVoiceOver(VoiceOverIndex);
		}));
	}
}
