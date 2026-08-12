using UnityEngine;

public class ReturnSevenDayLoginReward : MonoBehaviour
{
	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UISprite border;

	[SerializeField]
	private UILabel dayLabel;

	[SerializeField]
	private UILabel rewardName;

	[SerializeField]
	private GameObject claimedGameobject;

	[SerializeField]
	private UITexture armorTexture;

	[SerializeField]
	private UITexture weaponTexture;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private UITexture heroTexture;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UISprite currencySprite;

	[SerializeField]
	private UIButtonExtended button;

	[SerializeField]
	private GameObject selectedGameObject;

	[SerializeField]
	private UIAtlas shopAtlas;

	private LoginRewardsVisualConfig loginRewardsVisualConfig;

	private bool hasPlayedClaimedTween;

	private bool hasPlayedActiveDayTween;

	private void OnEnable()
	{
		if (loginRewardsVisualConfig == null)
		{
			loginRewardsVisualConfig = UnityUtils.LoadAsset("LoginRewardsVisualConfig") as LoginRewardsVisualConfig;
		}
	}
}
