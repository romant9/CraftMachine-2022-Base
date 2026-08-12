using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class SupportSelectionPanel : ScrollableListPanel<SupportModel>
{
	private TeamSelectionSurvivorsListPanel panel;

	private int currentIndex;

	private MapCategory currentMapCategory;

	private SupportEquipCallback callback;

	private bool equipOnSelect;

	private ICollection<string> equippedSupports;

	private TeamSelectionSurvivorsListPanel Panel
	{
		get
		{
			if ((bool)panel)
			{
				return panel;
			}
			return panel = GetComponent<TeamSelectionSurvivorsListPanel>();
		}
	}

	public void Show(int equipSlotIndex, ICollection<string> alreadyEquippedSupports, MapCategory mapCategory, SupportEquipCallback selectCallback)
	{
		currentMapCategory = mapCategory;
		Panel.OpenPanelWithoutCards();
		currentIndex = equipSlotIndex;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		callback = selectCallback;
		equippedSupports = alreadyEquippedSupports;
		SetCards(playerModel.SupportModels.Where((SupportModel model) => model.Unlocked));
	}

	protected override GameObject CreateCard(SupportModel item)
	{
		GameObject gameObject = base.CreateCard(item);
		RegularSupportCard card = gameObject.GetComponent<RegularSupportCard>();
		int indexInPanel = -1;
		for (int i = 0; i < equippedSupports.Count(); i++)
		{
			if (equippedSupports.ElementAt(i) == item.SupportId)
			{
				indexInPanel = i;
			}
		}
		card.Initialize(item, equippedSupports.Contains(item.SupportId), delegate
		{
			OnCardClick(item);
		}, delegate
		{
			((SupportDetailsPopup)HUDManager.TryOpenPopup(UIType.SupportDetailsPopup)).Show(item, canUpgrade: true, delegate
			{
				card.Refresh();
			}, currentMapCategory != MapCategory.Endless);
		}, indexInPanel, currentMapCategory);
		return gameObject;
	}

	private void OnCardClick(SupportModel supportModel)
	{
		Panel.ClosePanel();
		callback?.Invoke(supportModel, currentIndex);
	}

	public void HideImmediate()
	{
		base.gameObject.SetActive(value: false);
	}
}
