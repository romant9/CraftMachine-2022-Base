using TWDModel;
using UnityEngine;

public class PortraitHealthBar : MonoBehaviour
{
	[SerializeField]
	private int survivorCardPosition;

	[SerializeField]
	private UISprite healthBar;

	[SerializeField]
	private UISprite healthBarRecovered;

	private HealthIndicator healthIndicatorRef;

	[SerializeField]
	private UILabel healthValue;

	private ActorModel actor;

	private HealthBarInjuryTypeColors healthBarInjuryTypeColorsConfigInternal;

	private HealthBarInjuryTypeColors healthBarInjuryTypeColorsConfig
	{
		get
		{
			if (healthBarInjuryTypeColorsConfigInternal == null)
			{
				healthBarInjuryTypeColorsConfigInternal = UnityUtils.LoadFromAssetBundle<HealthBarInjuryTypeColors>("HealthBarInjuryTypeColorsConfig", "scriptableobjects");
			}
			return healthBarInjuryTypeColorsConfigInternal;
		}
	}

	private void Awake()
	{
		SetHealthBarRecovered(null, 0f);
	}

	private void UpdateHealthBar()
	{
		healthBar.fillAmount = healthIndicatorRef.HealthBar.value;
		InjuryType injuryType = ((actor != null) ? ActorView.GetInjuryTypeFromRatio(GameManager.Instance.gameEconomyData, actor, healthBar.fillAmount) : InjuryType.None);
		HealthBarInjuryTypeColor healthBarInjuryTypeColorConfig = healthBarInjuryTypeColorsConfig.GetHealthBarInjuryTypeColorConfig(injuryType);
		healthBar.gradientTop = healthBarInjuryTypeColorConfig.ColorTop;
		healthBar.gradientBottom = healthBarInjuryTypeColorConfig.ColorBottom;
		if (actor != null)
		{
			healthValue.text = actor.Hitpoints + "/" + actor.MaxHitPoints;
		}
		else
		{
			healthValue.text = string.Empty;
		}
	}

	public void SetHealthBarRecovered(HealthIndicator healthIndicator, float value)
	{
		if (healthIndicator == healthIndicatorRef && healthIndicator != null)
		{
			healthBarRecovered.fillAmount = healthBar.fillAmount + value;
			InjuryType injuryTypeFromRatio = ActorView.GetInjuryTypeFromRatio(GameManager.Instance.gameEconomyData, actor, healthBarRecovered.fillAmount);
			healthBarRecovered.color = healthBarInjuryTypeColorsConfig.GetColorForInjuryType(injuryTypeFromRatio);
		}
		else
		{
			healthBarRecovered.fillAmount = 0f;
		}
	}

	public void SetActorModel(ActorModel actor)
	{
		this.actor = actor;
		if (actor != null)
		{
			ActorView actorView = GameManager.Instance.GetViewForModel(actor) as ActorView;
			if (actorView != null && actorView.HealthIndicator != null)
			{
				healthIndicatorRef = actorView.HealthIndicator;
				EventDelegate.Add(actorView.HealthIndicator.HealthBar.onChange, UpdateHealthBar);
				UpdateHealthBar();
			}
		}
	}
}
