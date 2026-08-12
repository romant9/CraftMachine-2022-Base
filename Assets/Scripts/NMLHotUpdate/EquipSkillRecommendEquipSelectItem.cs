using System;
using UnityEngine;

public class EquipSkillRecommendEquipSelectItem : MonoBehaviour
{
	[SerializeField]
	private UIButton BtnThis;

	[SerializeField]
	private GameObject Drop_Bg;

	[SerializeField]
	private GameObject Triangle;

	[SerializeField]
	private UILabel LabelTag;

	private int index;

	public Action<int> ActCall;

	public void Initialize()
	{
		BtnThis.onClick.Add(new EventDelegate(OnClickSet));
	}

	public void SetInfo(int i, string name)
	{
		index = i;
		LabelTag.text = LocalizationManager.GetText(EquipSkillRecommendEquipModel.GetTagText(name));
	}

	public void SetSelect(int i)
	{
		Helpers.GameObjectSetActive(Triangle, index == i);
	}

	private void OnClickSet()
	{
		ActCall?.Invoke(index);
	}
}
