using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class BuildingResource : ScriptableObject
{
	public string PrefabName;

	private GameObject prefab;

	public string ConstructionScaffoldingPrefabName;

	private GameObject constructionScaffoldingPrefab;

	public string SelectBuildingAudioEvent;

	public static string BundleName => "buildingsprefabs";

	public static BuildingResource GetBuildingResource(BuildingModel model)
	{
		BuildingUpgradeLevel currentUpgradeLevel = model.GetCurrentUpgradeLevel();
		if (currentUpgradeLevel == null)
		{
			Debug.LogError($"A building with the type name \"{model.TypeName}\" does not have a definition of an upgrade of level {model.Level}!");
			return null;
		}
		int level = ((currentUpgradeLevel.Level > 4) ? 4 : currentUpgradeLevel.Level);
		return GetBuildingResourceFromStats(model.TypeName, level);
	}

	public static BuildingResource GetBuildingResourceFromStats(string buildingType, int level)
	{
		BuildingResource buildingResource = LoadBuildingAsset(buildingType, level);
		if (buildingResource != null)
		{
			return buildingResource;
		}
		buildingResource = GetHigherExistingBuildingResource(buildingType, level);
		if (buildingResource != null)
		{
			return buildingResource;
		}
		buildingResource = GetHighestExistingBuildingResource(buildingType);
		if (buildingResource != null)
		{
			return buildingResource;
		}
		Debug.LogError(string.Format("Could not find any prefab for building type \"{0}\" requested for \"{1}\"!", buildingType, "Buildings/" + buildingType + "_level" + level));
		return null;
	}

	public static BuildingResource LoadBuildingAsset(string buildingType, int level)
	{
		string text = "";
		text = ((!SingularityMonoBehaviour<AssetBundleController>.Instance.IsAssetExistedInAssetBundle("scriptableobjects", buildingType + "_level" + level)) ? ("Buildings/" + buildingType + "_Level" + level) : ("Buildings/" + buildingType + "_level" + level));
		return UnityUtils.LoadFromAssetBundle<BuildingResource>(text, "scriptableobjects");
	}

	public static BuildingResource GetHighestExistingBuildingResource(string buildingType)
	{
		bool flag = true;
		int num = 0;
		BuildingResource result = null;
		while (flag)
		{
			BuildingResource buildingResource = LoadBuildingAsset(buildingType, num++);
			if (buildingResource == null)
			{
				flag = false;
			}
			else
			{
				result = buildingResource;
			}
		}
		return result;
	}

	public static BuildingResource GetHigherExistingBuildingResource(string buildingType, int level)
	{
		bool flag = false;
		BuildingResource result = null;
		while (!flag)
		{
			BuildingResource buildingResource = LoadBuildingAsset(buildingType, level++);
			if (buildingResource != null)
			{
				flag = true;
				result = buildingResource;
			}
			if (level > 999)
			{
				break;
			}
		}
		return result;
	}

	public GameObject GetPrefab()
	{
		if (prefab == null)
		{
			prefab = AssetBundleManager.Instance.LoadAsset<GameObject>(PrefabName, BundleName);
		}
		return prefab;
	}

	public GameObject GetScaffoldingPrefab()
	{
		if (constructionScaffoldingPrefab == null)
		{
			constructionScaffoldingPrefab = AssetBundleManager.Instance.LoadAsset<GameObject>(ConstructionScaffoldingPrefabName, BundleName);
		}
		return constructionScaffoldingPrefab;
	}
}
