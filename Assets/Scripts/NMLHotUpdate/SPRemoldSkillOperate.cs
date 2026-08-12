using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillOperate : MonoBehaviour
{
	private enum OrderType
	{
		Star = 0,
		Level = 1
	}

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UILabel traitDesc;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private UIButton equipBtn;

	[SerializeField]
	private UIButton unEquipBtn;

	[SerializeField]
	private GameObject rightContainerContent;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject MoreEntryPrefab;

	[SerializeField]
	private UILabel OrderText;

	[SerializeField]
	private UILabel pageLabel;

	[SerializeField]
	private UIButton pagePrevButton;

	[SerializeField]
	private UIButton pageNextButton;

	[SerializeField]
	private UIScrollView scrollView;

	[SerializeField]
	private GameObject bgRecommend;

	private int slotIndex = -1;

	private OrderType currentOrderType;

	private EquipmentItemModel equipmentItemModel;

	private string currentModSkillId = "";

	private readonly List<GameObject> Entries = new List<GameObject>();

	private int currentPage = 1;

	private int pageSize = 10;

	private int pageOtherSize
	{
		get
		{
			if (!bgRecommend.activeSelf)
			{
				return 1;
			}
			return 2;
		}
	}

	private ModSkillMode modSkillMode => GameManager.Instance.playerModel.ModSkillManager.GetModSkillMode(currentModSkillId);

	private SPTraitsRemoldDefinitions definition => modSkillMode.GetSpTraitsDefaultTrait();

	private void OnEnable()
	{
		UpdateRightUI();
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SPRemoldEquipModSkill":
		case "SPRemoldUnEquipModSkill":
			FreshListData();
			UpdateRightUI();
			break;
		case "SPRemoldOperatePreviewItemClick":
			if (parameter != null && parameter is string)
			{
				string text = (string)parameter;
				currentModSkillId = text;
			}
			UpdateRightUI();
			break;
		}
	}

	public void Setup(int slotIndex, EquipmentItemModel equipmentItemModel)
	{
		this.slotIndex = slotIndex;
		this.equipmentItemModel = equipmentItemModel;
		currentModSkillId = "";
		if (slotIndex >= 0 && slotIndex < equipmentItemModel.ModSkillSlots.Length)
		{
			currentModSkillId = equipmentItemModel.ModSkillSlots[slotIndex].ModSkillMode?.ID ?? "";
		}
		UpdateRightUI();
		UpdateTabRecommend();
		FreshListData();
	}

	private void UpdateTabRecommend()
	{
		Helpers.GameObjectSetActive(bgRecommend, Helpers.IsSystemOpenById("SystemBase.SkillBag"));
	}

	public void UpdateRightUI()
	{
		Helpers.GameObjectSetActive(equipBtn, value: false);
		Helpers.GameObjectSetActive(unEquipBtn, value: false);
		Helpers.GameObjectSetActive(rightContainerContent, value: false);
		if (modSkillMode != null)
		{
			Helpers.GameObjectSetActive(rightContainerContent, value: true);
			if (modSkillMode.ModSkillState == ModSkillState.Equipped)
			{
				Helpers.GameObjectSetActive(unEquipBtn, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(equipBtn, value: true);
			}
			traitName.text = LocalizationManager.GetText(definition.SPTraitsName);
			UILabel uILabel = traitDesc;
			string sPTraitsDesc = definition.SPTraitsDesc;
			object[] arguments = definition.SPTraitsLcValue.ToArray();
			uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
			level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", definition.Level);
			HelpersUI.SetTraitsIconOnSprite(traitIcon, definition.SPTraitsIcon, definition.SPTraitsIconOnCloud);
			HelpersUI.SetSprite(classIcon, HelpersGfx.GetSurvivorClassSmallIconName(equipmentItemModel.Definition.SurvivorClass));
			starList.Setup(definition.Star);
			scrollView.ResetPosition();
		}
	}

	private List<ModSkillMode> GetListDatas()
	{
		if (equipmentItemModel == null)
		{
			return new List<ModSkillMode>();
		}
		List<ModSkillMode> acquiredModSkillsByClass = GameManager.Instance.playerModel.ModSkillManager.GetAcquiredModSkillsByClass(equipmentItemModel);
		List<ModSkillMode> list = new List<ModSkillMode>();
		List<ModSkillMode> source = acquiredModSkillsByClass.Where((ModSkillMode a) => a.ModSkillState == ModSkillState.Equipped).ToList();
		List<ModSkillMode> source2 = acquiredModSkillsByClass.Where((ModSkillMode a) => a.ModSkillState == ModSkillState.Unequipped).ToList();
		if (currentOrderType == OrderType.Star)
		{
			source = (from a in source
				orderby a.GetSpTraitsDefaultTrait().Star descending, a.GetSpTraitsDefaultTrait().Level descending
				select a).ToList();
			source2 = (from a in source2
				orderby a.GetSpTraitsDefaultTrait().Star descending, a.GetSpTraitsDefaultTrait().Level descending
				select a).ToList();
		}
		else
		{
			source = (from a in source
				orderby a.GetSpTraitsDefaultTrait().Level descending, a.GetSpTraitsDefaultTrait().Star descending
				select a).ToList();
			source2 = (from a in source2
				orderby a.GetSpTraitsDefaultTrait().Level descending, a.GetSpTraitsDefaultTrait().Star descending
				select a).ToList();
		}
		list.AddRange(source);
		list.AddRange(source2);
		return list;
	}

	private static int GetTotalPages(int totalCount, int sizePerPage)
	{
		if (totalCount <= 0)
		{
			return 1;
		}
		return Mathf.CeilToInt((float)totalCount / (float)sizePerPage);
	}

	private void UpdatePageNav(int totalPages)
	{
		if (pagePrevButton != null)
		{
			pagePrevButton.isEnabled = totalPages > 1;
		}
		if (pageNextButton != null)
		{
			pageNextButton.isEnabled = totalPages > 1;
		}
	}

	private void FreshListData()
	{
		if (currentOrderType == OrderType.Star)
		{
			OrderText.text = LocalizationManager.GetText("System.EquipInfo.OrderStar");
		}
		else
		{
			OrderText.text = LocalizationManager.GetText("System.EquipInfo.OrderLevel");
		}
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		List<ModSkillMode> listDatas = GetListDatas();
		int num = pageSize - pageOtherSize;
		int totalPages = GetTotalPages(listDatas.Count, num);
		if (currentPage > totalPages)
		{
			currentPage = totalPages;
		}
		if (currentPage < 1)
		{
			currentPage = 1;
		}
		UpdatePageNav(totalPages);
		int num2 = (currentPage - 1) * num;
		int num3 = Mathf.Min(num, Mathf.Max(0, listDatas.Count - num2));
		for (int i = 0; i < num3; i++)
		{
			ModSkillMode modSkillMode = listDatas[num2 + i];
			if (modSkillMode != null)
			{
				GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
				if (gameObject.TryGetComponent<SPRemoldSkillOperateItem>(out var component2))
				{
					component2.Setup(modSkillMode, equipmentItemModel);
					Entries.Add(gameObject);
				}
			}
		}
		GameObject item = EntryContainer.AddChild(MoreEntryPrefab);
		Entries.Add(item);
		component.Reposition();
		SyncListSelectionWithCurrentPreview();
	}

	private void SyncListSelectionWithCurrentPreview()
	{
		if (!string.IsNullOrEmpty(currentModSkillId))
		{
			UIEvent.Send("SPRemoldOperatePreviewItemClick", currentModSkillId);
		}
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	public void OnclickBag()
	{
		if (modSkillMode != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			ConsumablesPopup consumablesPopup = HUDManager.TryOpenPopup(UIType.ConsumablesCampPopup) as ConsumablesPopup;
			if (consumablesPopup != null)
			{
				consumablesPopup.OnClickToolBagSkillTab();
			}
		}
	}

	public void OnclickEquip()
	{
		if (modSkillMode != null && Helpers.ExecuteCommand(new EquipModSkillCommand(slotIndex, modSkillMode.ModelId, equipmentItemModel.ModelId)) == TWDModelResult.OK)
		{
			UIEvent.Send("SPRemoldEquipModSkill");
		}
	}

	public void OnclickUnEquip()
	{
		if (modSkillMode != null && Helpers.ExecuteCommand(new UnEquipmentModSkillCommand(modSkillMode.ModelId)) == TWDModelResult.OK)
		{
			UIEvent.Send("SPRemoldUnEquipModSkill");
		}
	}

	public void OnclickOrder()
	{
		currentOrderType = ((currentOrderType == OrderType.Star) ? OrderType.Level : OrderType.Star);
		currentPage = 1;
		FreshListData();
	}

	public void OnclickOperateClose()
	{
		UIEvent.Send("SPRemoldOperateCloseClick");
	}

	public void OnRadioClick()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		NewPhonePopup.OpenRadiophoneFeaturePopup();
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup).OnClickGoldRadio();
	}

	public void OnclickLeft()
	{
		List<ModSkillMode> listDatas = GetListDatas();
		int num = GetTotalPages(sizePerPage: pageSize - pageOtherSize, totalCount: listDatas.Count);
		if (num > 1)
		{
			currentPage = ((currentPage <= 1) ? num : (currentPage - 1));
			FreshListData();
		}
	}

	public void OnclickRight()
	{
		List<ModSkillMode> listDatas = GetListDatas();
		int num = GetTotalPages(sizePerPage: pageSize - pageOtherSize, totalCount: listDatas.Count);
		if (num > 1)
		{
			currentPage = ((currentPage >= num) ? 1 : (currentPage + 1));
			FreshListData();
		}
	}
}
