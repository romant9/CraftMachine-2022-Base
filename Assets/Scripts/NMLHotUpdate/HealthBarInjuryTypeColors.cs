using TWDModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Healthbar Injury Type Colors Configuration", fileName = "HealthBarInjuryTypeColorsConfig")]
public class HealthBarInjuryTypeColors : ScriptableObject
{
	public HealthBarInjuryTypeColor[] ColorPerInjuryType;

	public Color GetColorForInjuryType(InjuryType injuryType)
	{
		HealthBarInjuryTypeColor[] colorPerInjuryType = ColorPerInjuryType;
		foreach (HealthBarInjuryTypeColor healthBarInjuryTypeColor in colorPerInjuryType)
		{
			if (healthBarInjuryTypeColor.InjuryType == injuryType)
			{
				return healthBarInjuryTypeColor.ColorTop;
			}
		}
		return Color.white;
	}

	public HealthBarInjuryTypeColor GetHealthBarInjuryTypeColorConfig(InjuryType injuryType)
	{
		HealthBarInjuryTypeColor[] colorPerInjuryType = ColorPerInjuryType;
		foreach (HealthBarInjuryTypeColor healthBarInjuryTypeColor in colorPerInjuryType)
		{
			if (healthBarInjuryTypeColor.InjuryType == injuryType)
			{
				return healthBarInjuryTypeColor;
			}
		}
		return null;
	}
}
