using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TimelinePlayableListener : MonoBehaviour
{
	[SerializeField]
	private PlayableDirector director;

	[SerializeField]
	private Playable playableTrack;

	public UnityEvent PlayableStoppedNotify;

	private void OnEnable()
	{
		if ((bool)director)
		{
			director.stopped += OnDirectorStopped;
		}
	}

	private void OnDisable()
	{
		if ((bool)director)
		{
			director.stopped -= OnDirectorStopped;
		}
	}

	private void OnDirectorStopped(PlayableDirector director)
	{
		PlayableStoppedNotify?.Invoke();
	}
}
