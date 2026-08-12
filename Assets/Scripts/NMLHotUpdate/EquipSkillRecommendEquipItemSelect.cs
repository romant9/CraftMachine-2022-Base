using UnityEngine;

public class EquipSkillRecommendEquipItemSelect : MonoBehaviour
{
	[SerializeField]
	private GameObject BgSelect;

	[SerializeField]
	private UILabel LabelTag;

	private new string tag;

	public void SetInfo(string name)
	{
		tag = name;
		LabelTag.text = LocalizationManager.GetText(EquipSkillRecommendEquipModel.GetTagText(name));
	}

	public void SetSelect(string name)
	{
		Helpers.GameObjectSetActive(BgSelect, tag == name);
	}
}
