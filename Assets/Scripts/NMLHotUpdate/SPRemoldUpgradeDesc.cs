using TWDModel;
using UnityEngine;

public class SPRemoldUpgradeDesc : MonoBehaviour
{
	[SerializeField]
	private GameObject toollBtn;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public void SetToolBtnActive(bool active)
	{
		Helpers.GameObjectSetActive(toollBtn, value: false);
		if (Helpers.IsSystemOpenById("SystemBase.EquipRemold") && active)
		{
			Helpers.GameObjectSetActive(toollBtn, value: true);
		}
	}
}
