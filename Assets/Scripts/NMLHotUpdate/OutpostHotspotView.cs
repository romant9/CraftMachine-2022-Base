using TWDModel;
using UnityEngine;

public class OutpostHotspotView : ModelView<RegionModel>
{
	public HotspotType HotspotType;

	public GameObject Flag;

	public GameObject ResourceContainer;

	public override bool AutoGenerateViewID => true;
}
