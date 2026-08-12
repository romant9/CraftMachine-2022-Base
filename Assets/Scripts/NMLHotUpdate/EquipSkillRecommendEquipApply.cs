using TWDModel;
using UnityEngine;

public class EquipSkillRecommendEquipApply : HUDElement
{
	[SerializeField]
	private GameObject ItemParent;

	[SerializeField]
	private EquipmentButton equipmentButtonHave;

	[SerializeField]
	private EquipmentButton equipmentButtonNo;

	[SerializeField]
	private UITable table;

	[SerializeField]
	private UIButton SpriteConfigm;

	[SerializeField]
	private UIButton SpriteGetNow;

	private EquipmentItemModel equipmentItemModel;

	private EquipSkillRecommendEquipModel equipmentSkillSuggestion;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void Awake()
	{
		SpriteConfigm.onClick.Add(new EventDelegate(OnClickConfigm));
		SpriteGetNow.onClick.Add(new EventDelegate(OnClickGetNow));
	}

	public void SetInfo(EquipmentItemModel model, EquipSkillRecommendEquipModel conf, GameObject go)
	{
		equipmentItemModel = model;
		equipmentSkillSuggestion = conf;
		Helpers.InstantiateToParent(go, ItemParent).TryGetComponent<EquipSkillRecommendEquipItem>(out var component);
		component.HideGrid();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(equipmentButtonHave.gameObject, value: false);
		Helpers.GameObjectSetActive(equipmentButtonNo.gameObject, value: false);
		if (equipmentItemModel.Owner != null)
		{
			Helpers.GameObjectSetActive(equipmentButtonHave.gameObject, value: true);
			equipmentButtonHave.Setup(equipmentItemModel, null, null, "", showOwnerAndUpgradeIndicator: false);
		}
		else
		{
			Helpers.GameObjectSetActive(equipmentButtonNo.gameObject, value: true);
			equipmentButtonNo.Setup(equipmentItemModel, null, null, "", showOwnerAndUpgradeIndicator: false);
		}
		int num = 0;
		ModSkillSlot[] resultModSkillSlots = GetResultModSkillSlots();
		for (int i = 0; i < resultModSkillSlots.Length; i++)
		{
			if (resultModSkillSlots[i].ModSkillMode.ModSkillState != ModSkillState.Count)
			{
				num++;
			}
		}
		Helpers.GameObjectSetActive(SpriteConfigm.gameObject, num > 0);
		Helpers.GameObjectSetActive(SpriteGetNow.gameObject, num != resultModSkillSlots.Length);
		table.Reposition();
	}

	private ModSkillSlot[] GetResultModSkillSlots()
	{
		ModSkillSlot[] resultModSkillSlots = equipmentSkillSuggestion.GetResultModSkillSlots(playerModel);
		for (int i = 0; i < resultModSkillSlots?.Length; i++)
		{
			if (resultModSkillSlots[i].ModSkillMode.EquipmentItemModel == null)
			{
				resultModSkillSlots[i].ModSkillMode.EquipmentItemModel = equipmentItemModel;
			}
		}
		return resultModSkillSlots;
	}

	private void OnClickConfigm()
	{
		ModSkillSlot[] resultModSkillSlots = GetResultModSkillSlots();
		for (int i = 0; i < resultModSkillSlots.Length; i++)
		{
			ModSkillSlot modSkillSlot = resultModSkillSlots[i];
			if (modSkillSlot.ModSkillMode.ModSkillState != ModSkillState.Count)
			{
				ExecuteConfigm(i, modSkillSlot.ModSkillMode.ModelId, equipmentItemModel.ModelId);
			}
		}
		Close();
		UIEvent.Send("SPRemoldEquipModSkill");
	}

	private void ExecuteConfigm(int slotIndex, int skillId, int itemId)
	{
		Helpers.ExecuteCommand(new EquipModSkillCommand(slotIndex, skillId, itemId));
	}

	private void OnClickGetNow()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		NewPhonePopup.OpenRadiophoneFeaturePopup();
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup).OnClickGoldRadio();
	}
}
