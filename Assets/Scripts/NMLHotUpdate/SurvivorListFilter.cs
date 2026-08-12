using TWDModel;

public class SurvivorListFilter
{
	public enum FilterType
	{
		SurvivorClass = 0,
		Hero = 1,
		All = 2
	}

	public SurvivorClass ClassFilter = SurvivorClass.None;

	public FilterType TypeFilter;
}
