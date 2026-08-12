using TWDModel;
using UnityEngine;

public class SurvivorButtonFilter : MonoBehaviour
{
	public SurvivorClass SurvivorClass = SurvivorClass.None;

	public SurvivorListFilter.FilterType FilterType;

	private SurvivorListFilter settingInternal;

	public SurvivorListFilter FilterSettings
	{
		get
		{
			if (settingInternal == null)
			{
				settingInternal = new SurvivorListFilter();
			}
			settingInternal.ClassFilter = SurvivorClass;
			settingInternal.TypeFilter = FilterType;
			return settingInternal;
		}
	}
}
