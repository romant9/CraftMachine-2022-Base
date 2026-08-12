using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class LevelLoopingSoundView : ModelView<LevelLoopingSoundModel>
{
	[SerializeField]
	private LevelLoopingSoundType soundType;

	private string soundEvent;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		base.Model.Changed += OnModelChanged;
		Dictionary<LevelLoopingSoundType, string> dictionary = new Dictionary<LevelLoopingSoundType, string>();
		dictionary[LevelLoopingSoundType.NoiseAmp] = "combat_level/noise_amp";
		dictionary[LevelLoopingSoundType.CarStereo] = "combat_level/car_stereo";
		dictionary[LevelLoopingSoundType.TankEngine] = "combat_level/tank_engine";
		dictionary[LevelLoopingSoundType.PhoneRing] = "combat_level/phone_ring";
		dictionary[LevelLoopingSoundType.River] = "combat_level/TWD-river-sfx-mono";
		soundEvent = dictionary[soundType];
		if (base.Model.LoopingSoundPlayState == LoopingSoundPlayState.Started)
		{
			StartSound();
		}
	}

	public void StartSound()
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null && !string.IsNullOrEmpty(soundEvent))
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(soundEvent, base.gameObject);
			}));
		}
	}

	public void StopSound()
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null && !string.IsNullOrEmpty(soundEvent))
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				SingularityMonoBehaviour<AudioManager>.Instance.StopEvent(soundEvent, base.gameObject);
			}));
		}
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "StateChanged" && this != null)
		{
			if (base.Model.LoopingSoundPlayState == LoopingSoundPlayState.Started)
			{
				StartSound();
			}
			else
			{
				StopSound();
			}
		}
	}
}
