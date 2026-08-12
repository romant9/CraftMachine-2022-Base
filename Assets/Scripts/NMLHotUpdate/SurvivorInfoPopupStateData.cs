using System.Collections.Generic;
using TWDModel;

public class SurvivorInfoPopupStateData
{
	public SurvivorModel model;

	public ISurvivorFilterList currentFilter;

	public SurvivorInfoStateBase.States state;

	public Stack<int> stateMachineHistory = new Stack<int>();
}
