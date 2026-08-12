using BaseModel;
using TWDModel;
using UnityEngine;

public class ActivatedObjectView : ModelView<ActivatedObjectModel>
{
	private WalkerSpawnWarning walkerSpawnWarning;

	private GameObject soundWave { get; set; }

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
	}

	private void InitializeVisualization()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/WalkerSpawnWarning", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/WalkerSpawnWarning");
		}
		else
		{
			soundWave = Object.Instantiate(prefabResource.GetPrefab());
			soundWave.transform.parent = Object.FindObjectOfType<Scenario>().transform;
			soundWave.transform.position = base.transform.position;
		}
		walkerSpawnWarning = soundWave.GetComponent<WalkerSpawnWarning>();
	}

	private void DestroyVisualization()
	{
		if (soundWave != null)
		{
			Object.Destroy(soundWave);
			soundWave = null;
		}
	}

	private void OnDisable()
	{
		DestroyVisualization();
	}

	private void Update()
	{
		if (base.IsInitialized)
		{
			if (base.Model.Awoken && soundWave == null)
			{
				InitializeVisualization();
			}
			else if (!base.Model.Awoken && soundWave != null)
			{
				DestroyVisualization();
			}
			if (soundWave != null && walkerSpawnWarning != null)
			{
				walkerSpawnWarning.Duration = 2f - (float)base.Model.AwokenPercentage;
			}
		}
	}
}
