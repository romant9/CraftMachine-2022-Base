using BaseModel;
using TWDModel;
using UnityEngine;

public class EquipSkillRecommend : HUDElement
{
	[SerializeField]
	private EquipSkillRecommendEquip equipSkillRecommendEquip;

	private EquipmentItemModel equipmentItemModel;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
	}

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		equipmentItemModel = model as EquipmentItemModel;
		equipSkillRecommendEquip.OpenForModel(model);
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}

	public void OnClickEquipTips()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EquipSkillRecommendEquipHelp)?.Open();
	}
}
