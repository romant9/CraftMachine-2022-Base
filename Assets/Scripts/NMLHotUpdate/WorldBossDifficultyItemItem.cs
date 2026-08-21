using System;
using TWDModel;
using UnityEngine;

public class WorldBossDifficultyItemItem : MonoBehaviour
{
	[SerializeField]
	private UILabel desLabel;

	[SerializeField]
	private UISprite bgSprite;

	[SerializeField]
	private GameObject selectedHighlight;

	[SerializeField]
	private GameObject lockContainer;

	[SerializeField]
	private UILabel levelTxt;

	[SerializeField]
	private UILabel resourceTxt;

	[SerializeField]
	private UILabel guranteeTxt;

	[SerializeField]
	private GameObject enemyContainer;

	[SerializeField]
	private UILabel enemyTitleLabel;

	[SerializeField]
	private GameObject recourceContainer;

	[SerializeField]
	private UILabel recourceTitleLabel;

	[SerializeField]
	private GameObject guranteeContainer;

	[SerializeField]
	private UILabel guranteeTitleLabel;

	private WorldBossDifficultySubItem data;

	private Action<WorldBossDifficultySubItem, WorldBossDifficultyItemItem> clickCallback;

	private static readonly Color Class1DesColor = Helpers.HexToColor("#86e93b");

	private static readonly Color Class1BgColor = Helpers.HexToColor("#7e8b65");

	private static readonly Color Class2DesColor = Helpers.HexToColor("#e6c218");

	private static readonly Color Class2BgColor = Helpers.HexToColor("#8e7552");

	private static readonly Color Class3DesColor = Helpers.HexToColor("#e66539");

	private static readonly Color Class3BgColor = Helpers.HexToColor("#9d4637");

	private static readonly Color Class4DesColor = Helpers.HexToColor("#d21010");

	private static readonly Color Class4BgColor = Helpers.HexToColor("#8b2929");

	public WorldBossDifficultySubItem GetData()
	{
		return data;
	}

	public void SetData(WorldBossDifficultySubItem itemData)
	{
		data = itemData;
		EnsureReferences();
		if (itemData == null)
		{
			HelpersUI.SetContentToLabel(desLabel, string.Empty);
			return;
		}
		HelpersUI.SetContentToLabel(desLabel, itemData.des);
		Helpers.GameObjectSetActive(lockContainer, itemData.maxDifficulty < itemData.difficultyDefinition.Difficulty);
		WorldBossBattlegroundDefinition[] array = GameManager.Instance.gameEconomyData.FindWorldBossBattlegroundDefinitionsByDifficulty(itemData.difficultyDefinition.Difficulty);
		if (array != null && array.Length != 0)
		{
			WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = array[0];
			HelpersUI.SetContentToLabel(levelTxt, "lv" + worldBossBattlegroundDefinition.EnemyLevel);
		}
		HelpersUI.SetContentToLabel(resourceTxt, itemData.difficultyDefinition.VSReward.ToString());
		HelpersUI.SetContentToLabel(guranteeTxt, itemData.difficultyDefinition.Guarantee.ToString());
		ApplyDifficultyClassVisuals(itemData.difficultyDefinition.DifficultyClass);
		enemyContainer.SetActive(value: false);
		recourceContainer.SetActive(value: false);
		guranteeContainer.SetActive(value: false);
	}

	private void ApplyDifficultyClassVisuals(int difficultyClass)
	{
		Color color;
		Color color2;
		switch (difficultyClass)
		{
		default:
			return;
		case 1:
			color = Class1DesColor;
			color2 = Class1BgColor;
			break;
		case 2:
			color = Class2DesColor;
			color2 = Class2BgColor;
			break;
		case 3:
			color = Class3DesColor;
			color2 = Class3BgColor;
			break;
		case 4:
			color = Class4DesColor;
			color2 = Class4BgColor;
			break;
		}
		if (desLabel != null)
		{
			desLabel.color = color;
		}
		if (bgSprite != null)
		{
			bgSprite.color = color2;
		}
	}

	public void SetClickCallback(Action<WorldBossDifficultySubItem, WorldBossDifficultyItemItem> onClick)
	{
		clickCallback = onClick;
	}

	public void SetSelected(bool selected)
	{
		EnsureReferences();
		Helpers.GameObjectSetActive(selectedHighlight, selected);
	}

	private void EnsureReferences()
	{
		if (selectedHighlight == null)
		{
			Transform transform = base.transform.Find("Selected");
			if (transform != null)
			{
				selectedHighlight = transform.gameObject;
			}
		}
		if (bgSprite == null)
		{
			Transform transform2 = base.transform.Find("Bg") ?? base.transform.Find("BG");
			if (transform2 != null)
			{
				bgSprite = transform2.GetComponent<UISprite>();
			}
		}
	}

	public void ClickEnemy()
	{
		if (!lockContainer.activeSelf)
		{
			enemyContainer.SetActive(value: true);
			enemyTitleLabel.text = LocalizationManager.GetText("World.Boss.Enemy.Tips");
		}
	}

	public void ClickRecource()
	{
		if (!lockContainer.activeSelf)
		{
			recourceContainer.SetActive(value: true);
			recourceTitleLabel.text = LocalizationManager.GetText("World.Boss.Rescource.Tips");
		}
	}

	public void ShowTime(string str)
	{
		enemyContainer.SetActive(value: true);
		HelpersUI.SetContentToLabel(enemyTitleLabel, str);
	}

	public void ClickGurantee()
	{
		if (!lockContainer.activeSelf)
		{
			guranteeContainer.SetActive(value: true);
			guranteeTitleLabel.text = LocalizationManager.GetText("World.Boss.Guarantee.Tips");
		}
	}

	private void OnClick()
	{
		if (!(lockContainer != null) || !lockContainer.activeSelf)
		{
			clickCallback?.Invoke(data, this);
		}
	}

	public void ClickCloseTip()
	{
		enemyContainer.SetActive(value: false);
		recourceContainer.SetActive(value: false);
		guranteeContainer.SetActive(value: false);
	}

	private void OnEnable()
	{
		UIEventListener.Get(base.gameObject).onClick = delegate
		{
			OnClick();
		};
	}
}
