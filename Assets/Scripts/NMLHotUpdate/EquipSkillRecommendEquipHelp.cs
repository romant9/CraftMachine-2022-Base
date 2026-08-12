using UnityEngine;

public class EquipSkillRecommendEquipHelp : HUDElement
{
	[SerializeField]
	private UIButton SpriteCheck;

	private void Awake()
	{
		SpriteCheck.onClick.Add(new EventDelegate(OnClickDetailInfo));
	}

	public void OnClickDetailInfo()
	{
		Application.OpenURL(GameManager.Instance.gameEconomyData.ConfigData.Hyperlink_Discord_EquipSkillSuggestion);
	}
}
