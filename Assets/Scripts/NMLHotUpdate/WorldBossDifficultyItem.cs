using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldBossDifficultyItem : NUIListItem<WorldBossDifficultyData>
{
	private const float DefaultSubItemWidth = 200f;

	private const float DefaultItemHeight = 360f;

	private const float DefaultMinItemWidth = 360f;

	private const float DefaultBgWidthPadding = 42f;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private WorldBossDifficultyItemItem subItemTemplate;

	[SerializeField]
	private UISprite bgSprite;

	[SerializeField]
	private float subItemSpacing;

	[SerializeField]
	private GameObject difficultyIcon;

	[SerializeField]
	private GameObject lockContainer;

	[SerializeField]
	private float horizontalPadding = 30f;

	[SerializeField]
	private float bgWidthPadding = 42f;

	[SerializeField]
	private float listItemSpacing = 24f;

	private readonly List<WorldBossDifficultyItemItem> activeSubItems = new List<WorldBossDifficultyItemItem>();

	private Action<WorldBossDifficultyData, WorldBossDifficultySubItem, WorldBossDifficultyItemItem> subItemClickCallback;

	private float subItemLayoutY;

	private static readonly Color Class1TitleColor = Helpers.HexToColor("#86e93b");

	private static readonly Color Class1BgColor = Helpers.HexToColor("#2e4b26");

	private static readonly Color Class2TitleColor = Helpers.HexToColor("#e6c218");

	private static readonly Color Class2BgColor = Helpers.HexToColor("#6c512b");

	private static readonly Color Class3TitleColor = Helpers.HexToColor("#e66539");

	private static readonly Color Class3BgColor = Helpers.HexToColor("#772416");

	private static readonly Color Class4TitleColor = Helpers.HexToColor("#d21010");

	private static readonly Color Class4BgColor = Helpers.HexToColor("#4b0c0c");

	public override void SetData(WorldBossDifficultyData data)
	{
		base.SetData(data);
		UpdateUI();
	}

	public void SetSubItemClickCallback(Action<WorldBossDifficultyData, WorldBossDifficultySubItem, WorldBossDifficultyItemItem> onClick)
	{
		subItemClickCallback = onClick;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		WorldBossDifficultyData data = GetData();
		if (data != null)
		{
			HelpersUI.SetContentToLabel(titleLabel, data.difficultyName);
			ApplyDifficultyClassVisuals(GetDifficultyClass(data));
			RebuildSubItems(data);
		}
	}

	private static int GetDifficultyClass(WorldBossDifficultyData data)
	{
		if (data?.items == null || data.items.Length == 0 || data.items[0]?.difficultyDefinition == null)
		{
			return 0;
		}
		return data.items[0].difficultyDefinition.DifficultyClass;
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
			color = Class1TitleColor;
			color2 = Class1BgColor;
			break;
		case 2:
			color = Class2TitleColor;
			color2 = Class2BgColor;
			break;
		case 3:
			color = Class3TitleColor;
			color2 = Class3BgColor;
			break;
		case 4:
			color = Class4TitleColor;
			color2 = Class4BgColor;
			break;
		}
		if (titleLabel != null)
		{
			titleLabel.color = color;
		}
		if (bgSprite != null)
		{
			bgSprite.color = color2;
		}
		UISprite uISprite = ((difficultyIcon != null) ? difficultyIcon.GetComponent<UISprite>() : null);
		if (uISprite != null)
		{
			uISprite.spriteName = "UI_WB_Diffic_" + difficultyClass;
		}
	}

	public void UpdateSubItemSelection(WorldBossDifficultyData selectedDifficulty, WorldBossDifficultySubItem selectedSubItem)
	{
		WorldBossDifficultyData data = GetData();
		if (data?.items != null)
		{
			for (int i = 0; i < activeSubItems.Count; i++)
			{
				WorldBossDifficultySubItem worldBossDifficultySubItem = ((i < data.items.Length) ? data.items[i] : null);
				bool selected = data == selectedDifficulty && worldBossDifficultySubItem == selectedSubItem;
				activeSubItems[i].SetSelected(selected);
			}
		}
	}

	private void Awake()
	{
		EnsureInitialized();
	}

	private void EnsureInitialized()
	{
		EnsureReferences();
		if (subItemTemplate != null)
		{
			if (subItemLayoutY == 0f)
			{
				subItemLayoutY = subItemTemplate.transform.localPosition.y;
			}
			subItemTemplate.gameObject.SetActive(value: false);
		}
	}

	private void EnsureReferences()
	{
		if (subItemTemplate == null)
		{
			subItemTemplate = GetComponentInChildren<WorldBossDifficultyItemItem>(includeInactive: true);
		}
		if (bgSprite == null)
		{
			Transform transform = base.transform.Find("Bg");
			if (transform != null)
			{
				bgSprite = transform.GetComponent<UISprite>();
			}
		}
	}

	private void RebuildSubItems(WorldBossDifficultyData data)
	{
		EnsureInitialized();
		ClearSubItems();
		if (subItemTemplate == null || data.items == null || data.items.Length == 0)
		{
			UpdateItemSize(0);
			return;
		}
		for (int i = 0; i < data.items.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(subItemTemplate.gameObject, base.transform);
			gameObject.name = subItemTemplate.gameObject.name + "_" + i;
			gameObject.SetActive(value: true);
			WorldBossDifficultyItemItem component = gameObject.GetComponent<WorldBossDifficultyItemItem>();
			if (component == null)
			{
				UnityEngine.Object.Destroy(gameObject);
				continue;
			}
			WorldBossDifficultySubItem data2 = data.items[i];
			component.SetData(data2);
			component.SetClickCallback(delegate(WorldBossDifficultySubItem subItemDataClicked, WorldBossDifficultyItemItem subItemView)
			{
				subItemClickCallback?.Invoke(GetData(), subItemDataClicked, subItemView);
			});
			activeSubItems.Add(component);
		}
		LayoutSubItems();
		UpdateItemSize(activeSubItems.Count);
	}

	private void ClearSubItems()
	{
		for (int num = activeSubItems.Count - 1; num >= 0; num--)
		{
			if (activeSubItems[num] != null)
			{
				UnityEngine.Object.Destroy(activeSubItems[num].gameObject);
			}
		}
		activeSubItems.Clear();
	}

	private void LayoutSubItems()
	{
		if (activeSubItems.Count != 0)
		{
			float subItemWidth = GetSubItemWidth();
			float num = (float)activeSubItems.Count * subItemWidth + (float)(activeSubItems.Count - 1) * subItemSpacing;
			float num2 = GetSubItemGroupCenterX() - num * 0.5f + subItemWidth * 0.5f;
			for (int i = 0; i < activeSubItems.Count; i++)
			{
				activeSubItems[i].transform.localPosition = new Vector3(num2 + (float)i * (subItemWidth + subItemSpacing), subItemLayoutY, 0f);
			}
		}
	}

	private float GetSubItemGroupCenterX()
	{
		float num = ((bgSprite != null) ? bgSprite.transform.localPosition.x : 0f);
		float num2 = 0f;
		if (subItemTemplate != null)
		{
			Transform transform = subItemTemplate.transform.Find("Bg");
			if (transform != null)
			{
				num2 = transform.localPosition.x;
			}
		}
		return num - num2;
	}

	private float GetSubItemWidth()
	{
		if (subItemTemplate != null)
		{
			BoxCollider component = subItemTemplate.GetComponent<BoxCollider>();
			if (component != null && component.size.x > 0f)
			{
				return component.size.x;
			}
		}
		return 200f;
	}

	private void UpdateItemSize(int subItemCount)
	{
		float subItemWidth = GetSubItemWidth();
		float y = ((boxCollider != null && boxCollider.size.y > 0f) ? boxCollider.size.y : 360f);
		float num = ((subItemCount > 0) ? ((float)subItemCount * subItemWidth + (float)(subItemCount - 1) * subItemSpacing) : subItemWidth);
		float num2 = ((subItemCount > 0) ? (num + horizontalPadding * 2f) : 360f) + bgWidthPadding;
		float x = num2 + listItemSpacing;
		if (boxCollider != null)
		{
			boxCollider.size = new Vector3(x, y, boxCollider.size.z);
		}
		if (bgSprite != null)
		{
			bgSprite.width = Mathf.RoundToInt(num2);
		}
	}

	private void OnDestroy()
	{
		ClearSubItems();
	}
}
