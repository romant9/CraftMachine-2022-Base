using System.Collections.Generic;
using UnityEngine;

public class WalkerTurnNotification : HUDElement
{
	public Color TurnCounterDefaultColor;

	public Color WaveIncomingWarningColor;

	[Tooltip("Background sprite for the turn counter.")]
	public UISprite TurnCountBgSprite;

	[Tooltip("Label for the walker turn heading text.")]
	public UILabel WalkerTurnHeading;

	[Tooltip("Label for the turn counter.")]
	public UILabel TurnCounter;

	[Tooltip("Label for the turn counter body text.")]
	public UILabel TurnCounterBody;

	[Tooltip("Label wave incoming warning text.")]
	public UILabel WaveIncomingLabel;

	[Tooltip("Container for turn counter text.")]
	public GameObject TurnCounterContainer;

	[Tooltip("Container for active wave warning text.")]
	public GameObject WaveActiveContainer;

	[Tooltip("Container for wave content indicators.")]
	public GameObject WaveSpawnIndicatorContainer;

	[Tooltip("Indicator icon for normal walker.")]
	public GameObject WaveSpawnIndicatorNormal;

	[Tooltip("Padding between walker icons.")]
	public float IconPadding = 25f;

	private List<GameObject> waveSizeIndicators = new List<GameObject>();

	public event AnimationCompletionCallback AnimationCompleted;

	public void SetMessage(string heading, string turns, string body, int spawnCount)
	{
		base.gameObject.SetActive(value: true);
		SetTurnCountEnabled(enabled: true);
		SetWaveIncomingEnabled(enabled: false);
		SetWaveActiveEnabled(enabled: false);
		WalkerTurnHeading.text = heading;
		TurnCounter.text = turns;
		TurnCounterBody.text = body;
		TurnCountBgSprite.color = TurnCounterDefaultColor;
		if (spawnCount != waveSizeIndicators.Count)
		{
			ClearWaveSpawnIndicators();
			for (int i = 0; i < spawnCount; i++)
			{
				GameObject gameObject = Object.Instantiate(WaveSpawnIndicatorNormal);
				gameObject.transform.parent = WaveSpawnIndicatorContainer.transform;
				gameObject.transform.localPosition = new Vector3(IconPadding, 0f, 0f) * i;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.gameObject.SetActive(value: true);
				waveSizeIndicators.Add(gameObject);
			}
		}
		TweenManager.PlayTweenGroup(base.gameObject, 0, forward: true, OnAnimationCompleted);
	}

	public void SetIncomingMessage(string heading, string body, int spawnCount)
	{
		base.gameObject.SetActive(value: true);
		SetTurnCountEnabled(enabled: false);
		SetWaveIncomingEnabled(enabled: true);
		SetWaveActiveEnabled(enabled: false);
		WalkerTurnHeading.text = heading;
		WaveIncomingLabel.text = body;
		TurnCountBgSprite.color = WaveIncomingWarningColor;
		if (spawnCount != waveSizeIndicators.Count)
		{
			ClearWaveSpawnIndicators();
			for (int i = 0; i < spawnCount; i++)
			{
				GameObject gameObject = Object.Instantiate(WaveSpawnIndicatorNormal);
				gameObject.transform.parent = WaveSpawnIndicatorContainer.transform;
				gameObject.transform.localPosition = new Vector3(25f, 0f, 0f) * i;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.gameObject.SetActive(value: true);
				waveSizeIndicators.Add(gameObject);
			}
		}
		TweenManager.PlayTweenGroup(base.gameObject, 0, forward: true, OnAnimationCompleted);
	}

	public void SetWaveActiveMessage(string heading, string body)
	{
		base.gameObject.SetActive(value: true);
		SetTurnCountEnabled(enabled: false);
		SetWaveIncomingEnabled(enabled: false);
		SetWaveActiveEnabled(enabled: true);
		WalkerTurnHeading.text = heading;
		WaveActiveContainer.GetComponentInChildren<UILabel>().text = body;
		TweenManager.PlayTweenGroup(base.gameObject, 0, forward: true, OnAnimationCompleted);
	}

	private void SetTurnCountEnabled(bool enabled)
	{
		TurnCounterContainer.SetActive(value: true);
		TurnCounter.gameObject.SetActive(enabled);
		TurnCounterBody.gameObject.SetActive(enabled);
	}

	private void SetWaveIncomingEnabled(bool enabled)
	{
		TurnCounterContainer.SetActive(value: true);
		WaveIncomingLabel.gameObject.SetActive(enabled);
	}

	private void SetWaveActiveEnabled(bool enabled)
	{
		TurnCounterContainer.SetActive(!enabled);
		WaveActiveContainer.SetActive(enabled);
	}

	private void ClearWaveSpawnIndicators()
	{
		foreach (GameObject waveSizeIndicator in waveSizeIndicators)
		{
			Object.Destroy(waveSizeIndicator);
		}
		waveSizeIndicators.Clear();
	}

	private void OnAnimationCompleted()
	{
		NotifyAnimationCompletion();
	}

	private void NotifyAnimationCompletion()
	{
		this.AnimationCompleted?.Invoke();
	}
}
