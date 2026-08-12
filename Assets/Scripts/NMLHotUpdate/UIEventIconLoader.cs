using TWDModel;
using UnityEngine;

public class UIEventIconLoader : MonoBehaviourExtended
{
	[Tooltip("If targetParent is not set, this.gameObject will be used.")]
	[Header("Optional")]
	[SerializeField]
	private GameObject targetParent;

	private const string prefabNameDoubleXP = "Event_Icon_Xp";

	private UIEventIcon cachedItem;

	private void Awake()
	{
		DebugIdString = "UIEventIconLoader";
	}

	private void OnDisable()
	{
		Clear();
	}

	public virtual void Start()
	{
		if (!IsFeatureDisabled() && GameManager.Instance.playerModel.ActivityManager.IsActivityOpen(ActivityType.DoubleXPFromKills))
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
		if (FeatureUIHighlights.IsActive(FeatureUIHighlights.FeaturesIds.DoubleXpUIBadges))
		{
			if (!(cachedItem == null))
			{
				return;
			}
			GameObject gameObject = UnityUtils.LoadFromAssetBundle("Event_Icon_Xp", HUDElementConfig.BundleName) as GameObject;
			if (gameObject != null)
			{
				cachedItem = Helpers.InstantiateWithComponent<UIEventIcon>(gameObject, targetParent);
				if (cachedItem == null)
				{
					DebugLogError("Failed to Instantiate UIEventIcon Event_Icon_Xp from asset bundles");
				}
			}
			else
			{
				DebugLogError("Could not load prefab Event_Icon_Xp from asset bunfles ");
			}
		}
		else
		{
			Clear();
		}
	}

	private bool IsFeatureDisabled()
	{
		return !GameManager.Instance.gameEconomyData.GetFeature("UIEventIconLoader").Enabled;
	}
}
