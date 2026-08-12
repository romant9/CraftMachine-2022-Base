using TWDModel;
using UnityEngine;

public class GoldRadioCallDetailPopupItemItemItem : MonoBehaviour
{
	[SerializeField]
	private UIButton btnThis;

	[SerializeField]
	private GameObject normalContent;

	[SerializeField]
	private UISprite skillBg;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private GameObject selectGo;

	[SerializeField]
	private GameObject up;

	private ModSkillMode modSkillDefault;

	private int index;

	private int highlightOperateSlotIndex = -1;

	private bool showUp;

	private ModSkillMode modSkillSlotShow => modSkillDefault;

	private SPTraitsRemoldDefinitions slotDefinition
	{
		get
		{
			if (modSkillSlotShow == null)
			{
				return null;
			}
			return modSkillSlotShow.GetSpTraitsDefaultTrait();
		}
	}

	public void Initialize()
	{
		btnThis.onClick.Add(new EventDelegate(OnclickOperate));
	}

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
		if (type == "SPRemoldOperateItemClick" && parameter is int num)
		{
			highlightOperateSlotIndex = num;
			ApplyOperateSlotSelectionHighlight();
		}
	}

	public void Setup(int i, ModSkillMode modSkillSlot, int highlightOperateSlotIndex = -1, bool showUp = false)
	{
		index = i;
		modSkillDefault = modSkillSlot;
		this.highlightOperateSlotIndex = highlightOperateSlotIndex;
		this.showUp = showUp;
		UpdateUI();
	}

	public void UpdateUI()
	{
		Helpers.GameObjectSetActive(selectGo, value: false);
		Helpers.GameObjectSetActive(normalContent, value: false);
		Helpers.GameObjectSetActive(up, value: false);
		if (modSkillSlotShow == null || slotDefinition == null)
		{
			ApplyOperateSlotSelectionHighlight();
			return;
		}
		Helpers.GameObjectSetActive(normalContent, value: true);
		Helpers.GameObjectSetActive(up, showUp);
		skillBg.color = Helpers.HexToColor(slotDefinition.Color);
		HelpersUI.SetTraitsIconOnSprite(traitIcon, slotDefinition.SPTraitsIcon, slotDefinition.SPTraitsIconOnCloud);
		starList.Setup(slotDefinition.Star);
		level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", slotDefinition.Level);
		ApplyOperateSlotSelectionHighlight();

		if (NewPhonePopup.Instance.FavoriteModSkillList.Contains(modSkillSlotShow.Type)) SetFavorite(true); //hunter_3035
	}

	private void ApplyOperateSlotSelectionHighlight()
	{
		bool value = highlightOperateSlotIndex >= 0 && modSkillSlotShow != null && highlightOperateSlotIndex == index;
		Helpers.GameObjectSetActive(selectGo, value);
	}

	public void OnclickOperate()
	{
		SPRemoldTraitsSkillMergedPopup sPRemoldTraitsSkillMergedPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillMergedPopup) as SPRemoldTraitsSkillMergedPopup;
		if (sPRemoldTraitsSkillMergedPopup != null)
		{
			sPRemoldTraitsSkillMergedPopup.Setup(modSkillSlotShow.ID);
			if (OfflineManager.IsLoadDataManager) sPRemoldTraitsSkillMergedPopup.SetItemData(this);
			sPRemoldTraitsSkillMergedPopup.Open();
		}
	}


	#region myparams
	[SerializeField]
	private GameObject favoriteObject;
	#endregion

	#region mycode
	public void SetFavorite(bool isFavorite)
	{
		Helpers.GameObjectSetActive(favoriteObject, isFavorite);
	}
	#endregion
}
