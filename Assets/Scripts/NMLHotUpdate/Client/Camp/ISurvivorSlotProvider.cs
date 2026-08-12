using UnityEngine;

namespace Client.Camp
{
	public interface ISurvivorSlotProvider
	{
		Transform SelectedSlotPosition { get; }

		Transform FirstSlotPosition { get; }
	}
}
