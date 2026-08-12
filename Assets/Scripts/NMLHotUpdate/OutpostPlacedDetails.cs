using TWDModel;
using UnityEngine;

public class OutpostPlacedDetails : MonoBehaviour
{
	[SerializeField]
	private UISprite HasDefenderA;

	[SerializeField]
	private UISprite HasDefenderB;

	[SerializeField]
	private UISprite HasDefenderC;

	[SerializeField]
	private UISprite HasContainer;

	[SerializeField]
	private UISprite HasFlag;

	public void UpdateUI()
	{
		if (GameManager.Instance.playerModel.OutpostModel.EditLevelModel != null && GameManager.Instance.playerModel.SurvivorContainer != null)
		{
			OutpostLevelModel editLevelModel = GameManager.Instance.playerModel.OutpostModel.EditLevelModel;
			editLevelModel.GetFirstFreeDefenderState();
			if (HasDefenderA != null)
			{
				HasDefenderA.gameObject.SetActive(!editLevelModel.HasDefender(HotspotState.DefenderSpawn_0));
			}
			if (HasDefenderB != null)
			{
				HasDefenderB.gameObject.SetActive(!editLevelModel.HasDefender(HotspotState.DefenderSpawn_1));
			}
			if (HasDefenderC != null)
			{
				HasDefenderC.gameObject.SetActive(!editLevelModel.HasDefender(HotspotState.DefenderSpawn_2));
			}
			if (HasContainer != null)
			{
				HasContainer.gameObject.SetActive(value: false);
			}
			if (HasFlag != null)
			{
				HasFlag.gameObject.SetActive(value: false);
			}
		}
	}
}
