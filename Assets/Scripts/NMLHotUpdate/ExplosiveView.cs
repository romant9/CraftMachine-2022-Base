using BaseModel;
using TWDModel;
using UnityEngine;

public class ExplosiveView : ModelView<ExplosiveModel>
{
	public GameObject ExplosionPrefab;

	public GameObject PreExplosionMesh;

	public GameObject PostExplosionMesh;

	private GameObject ActiveExplosion;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
		if (PreExplosionMesh != null)
		{
			PreExplosionMesh.SetActive(!base.Model.HasExploded);
		}
		if (PostExplosionMesh != null)
		{
			PostExplosionMesh.SetActive(base.Model.HasExploded);
		}
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		if (this != null)
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, DelayedNotification));
		}
	}

	public void DelayedNotification()
	{
		if (PreExplosionMesh != null)
		{
			PreExplosionMesh.SetActive(value: false);
		}
		if (PostExplosionMesh != null)
		{
			PostExplosionMesh.SetActive(value: true);
		}
		if (ExplosionPrefab != null)
		{
			ActiveExplosion = Object.Instantiate(ExplosionPrefab);
			ActiveExplosion.transform.position = base.transform.position;
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_level/barrel_explosion_1");
	}

	public void Update()
	{
		if (ActiveExplosion != null)
		{
			ParticleSystem component = ActiveExplosion.GetComponent<ParticleSystem>();
			if (component != null && !component.IsAlive())
			{
				Object.Destroy(ActiveExplosion);
				ActiveExplosion = null;
			}
		}
	}
}
