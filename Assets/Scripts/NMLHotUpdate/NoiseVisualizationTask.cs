using Client.Utils;
using TWDModel;
using UnityEngine;

public class NoiseVisualizationTask : ActorVisualizationTask
{
	private EffectThreatWave threatWaveEffect;

	private bool effectStarted;

	private int threatValue;

	public NoiseVisualizationTask(NoiseAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		GridModel grid = GameManager.Instance.playerModel.Grid;
		threatValue = action.ThreatValue;
		AddDependency(base.Actor, reserve: false);
		AddFactionDependency(base.Actor.Faction);
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/SoundWave", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/SoundWave");
		}
		else if (!CombatHUD.IsSpeedUpEnabled)
		{
			FixedVec3 position = GridView.Instance.GetPosition(action.Source);
			GameObject gameObject = Object.Instantiate(prefabResource.GetPrefab());
			threatWaveEffect = gameObject.GetComponentInChildren<EffectThreatWave>();
			if (threatWaveEffect != null)
			{
				float num = (float)grid.CellSize.X * 2f;
				threatWaveEffect.EndScale = (float)action.NoiseRange * num;
				threatWaveEffect.transform.position = position.ToVector3();
			}
		}
	}

	public override bool Update(float deltaTime)
	{
		ReleaseAllDependencies();
		if (!effectStarted)
		{
			effectStarted = true;
			GameManager.Instance.playerModel.Combat.NotifyChange("threatMeterValueChanged", threatValue);
			if (threatWaveEffect != null)
			{
				threatWaveEffect.Begin();
			}
		}
		else
		{
			if (threatWaveEffect != null && (threatWaveEffect.CurrentScale >= threatWaveEffect.EndScale || threatWaveEffect.Age >= 1f))
			{
				Object.Destroy(threatWaveEffect.transform.parent.gameObject);
				return false;
			}
			if (threatWaveEffect == null)
			{
				return false;
			}
		}
		return true;
	}
}
