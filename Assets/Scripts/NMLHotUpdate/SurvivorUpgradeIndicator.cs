using TWDModel;
using UnityEngine;

public class SurvivorUpgradeIndicator : BuildingUpgradeIndicator
{
	[SerializeField]
	private UITexture portrait;

	private SurvivorModel survivor;

	protected override void Start()
	{
		base.Start();
		TrainingGroundBuildingModel trainingGroundBuildingModel = (TrainingGroundBuildingModel)base.Building.Model;
		survivor = trainingGroundBuildingModel.UpgradingSurvivor;
		if (survivor.manager == null)
		{
			Helpers.ExecuteCommand(new CancelUpgradeCommand(trainingGroundBuildingModel));
		}
		else if (portrait != null && PortraitManager.Instance != null && survivor != null)
		{
			portrait.gameObject.SetActive(value: false);
			PortraitRenderSource info = PortraitRenderSource.fromActorModel(survivor);
			if (PortraitManager.Instance.GetPortrait(info) == null)
			{
				ModularCharacter prefabForActor = ActorView.GetPrefabForActor(survivor);
				PortraitManager.Instance.CreatePortrait(info, prefabForActor, OnMissingPortraitRendered);
			}
			else
			{
				portrait.mainTexture = PortraitManager.Instance.GetPortrait(info);
				portrait.gameObject.SetActive(value: true);
			}
		}
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource info)
	{
		if (portrait != null && survivor != null && info != null && info.UniqueId != null && survivor.ModelId.ToString().ToLowerInvariant() == info.UniqueId.ToLowerInvariant())
		{
			portrait.mainTexture = PortraitManager.Instance.GetPortrait(info);
			portrait.gameObject.SetActive(value: true);
		}
	}
}
