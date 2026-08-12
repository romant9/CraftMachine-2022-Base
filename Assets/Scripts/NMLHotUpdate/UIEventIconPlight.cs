using TWDModel;
using UnityEngine;

public class UIEventIconPlight : MonoBehaviourExtended
{
	[Tooltip("If targetParent is not set, this.gameObject will be used.")]
	[Header("Optional")]
	[SerializeField]
	private GameObject targetParent;

	private const string prefabName = "Event_Icon_Plight";

	private UIEventIcon cachedItem;

	private void Awake()
	{
		DebugIdString = "UIEventIconPlight";
	}

	private void OnDisable()
	{
		Clear();
	}

	public virtual void Start()
	{
		if (!IsFeatureDisabled())
		{
			if (targetParent == null)
			{
				targetParent = base.gameObject;
			}
			UpdateCachedItem();
			if (targetParent.GetComponent<UIGrid>() != null)
			{
				targetParent.GetComponent<UIGrid>().repositionNow = true;
			}
		}
	}

	public override void Clear()
	{
		if (!IsFeatureDisabled())
		{
			base.Clear();
			if (cachedItem != null)
			{
				Object.Destroy(cachedItem);
				cachedItem = null;
			}
		}
	}

	private void UpdateCachedItem()
	{
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel == null)
		{
			return;
		}
		if (weeklyChallengeModel.IsDebufCycles())
		{
			if (!(cachedItem == null))
			{
				return;
			}
			GameObject gameObject = UnityUtils.LoadFromAssetBundle("Event_Icon_Plight", HUDElementConfig.BundleName) as GameObject;
			if (gameObject != null)
			{
				cachedItem = Helpers.InstantiateWithComponent<UIEventIcon>(gameObject, targetParent);
				if (cachedItem == null)
				{
					DebugLogError("Failed to Instantiate UIEventIcon Event_Icon_Plight from asset bundles");
				}
			}
			else
			{
				DebugLogError("Could not load prefab Event_Icon_Plight from asset bunfles ");
			}
		}
		else
		{
			Clear();
		}
	}

	private bool IsFeatureDisabled()
	{
		return !GameManager.Instance.gameEconomyData.GetFeature("UIEventIconPlight").Enabled;
	}
}
