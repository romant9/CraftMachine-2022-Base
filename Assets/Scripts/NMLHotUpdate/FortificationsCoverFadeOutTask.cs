using UnityEngine;

public class FortificationsCoverFadeOutTask : VisualizationTask
{
	private readonly FortificationsCoverView coverView;

	private readonly float duration;

	private Vector3 startScale;

	private float elapsed;

	private bool started;

	public FortificationsCoverFadeOutTask(FortificationsCoverView coverView, float duration)
		: base(null)
	{
		this.coverView = coverView;
		this.duration = Mathf.Max(0.01f, duration);
	}

	public override void Start()
	{
		base.Start();
		if (coverView == null)
		{
			started = true;
			elapsed = duration;
		}
		else
		{
			startScale = coverView.transform.localScale;
			started = true;
		}
	}

	public override bool Update(float deltaTime)
	{
		if (!started)
		{
			return true;
		}
		if (coverView == null)
		{
			return false;
		}
		elapsed += deltaTime;
		float num = Mathf.Clamp01(elapsed / duration);
		float t = num * num * (3f - 2f * num);
		coverView.transform.localScale = Vector3.LerpUnclamped(startScale, Vector3.zero, t);
		if (num < 1f)
		{
			return true;
		}
		coverView.DestroyAfterFade();
		return false;
	}
}
