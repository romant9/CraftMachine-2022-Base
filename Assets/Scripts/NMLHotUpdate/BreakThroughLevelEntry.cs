using TWDModel;
using UnityEngine;

public class BreakThroughLevelEntry : MonoBehaviour
{
	[SerializeField]
	private UILabel Label;

	[SerializeField]
	private GameObject lockGO;

	[SerializeField]
	private Animator effectAnimator;

	private EquipmentItemModel equipmentItemModel;

	private int _level;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "BreakThroughed" && _level == equipmentItemModel.BreakthroughLevel)
		{
			ShwoEffect(show: true);
		}
	}

	public void SetContent(int level, EquipmentItemModel itemModel)
	{
		equipmentItemModel = itemModel;
		EquipBreakthroughDefinition equipBreakthroughDefinitionByRarityAndLevel = GameManager.Instance.gameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(equipmentItemModel.RarityLevel, level);
		HelpersUI.SetContentToLabel(Label, LocalizationManager.GetText(equipBreakthroughDefinitionByRarityAndLevel.Describe));
		Helpers.GameObjectSetActive(lockGO, !itemModel.IsLevelBreakThrough(level));
		_level = level;
	}

	private void ShwoEffect(bool show)
	{
		if (show)
		{
			effectAnimator.Play(Helpers.ShowNameHash);
		}
		else
		{
			effectAnimator.Play(Helpers.HideNameHash);
		}
	}
}
