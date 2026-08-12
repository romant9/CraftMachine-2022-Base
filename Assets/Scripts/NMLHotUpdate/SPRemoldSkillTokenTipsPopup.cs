using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillTokenTipsPopup : HUDElement
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel descLabel;

	[SerializeField]
	private UISprite skillTokenIcon;

	[SerializeField]
	private UISprite skillTokenIconBg;

	[SerializeField]
	private UILabel numLabel;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject leftContainer;

	[SerializeField]
	private SPRemoldSkillTokenTipsPopupLeft left;

	private CurrencyType currencyType;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
		StartCoroutine(FreshListDataCoroutine());
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		StopAllCoroutines();
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SPRemoldSkillTokenTipsPopupItemClick" && parameter != null && parameter is string)
		{
			string definitionId = parameter as string;
			UpdateSkillcontent(definitionId);
		}
	}

	public void Setup(CurrencyType currencyType)
	{
		this.currencyType = currencyType;
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		SPTraitsSkillKitTokenSet skillKitTokenSetDefinition = HelpersGfx.GetSkillKitTokenSetDefinition(currencyType);
		nameLabel.text = LocalizationManager.GetText(skillKitTokenSetDefinition.Name);
		descLabel.text = LocalizationManager.GetText(skillKitTokenSetDefinition.Desc);
		numLabel.text = LocalizationManager.GetText("System.EquipRemold.ItemTips.Fucn1") + GameManager.Instance.playerModel.GetCurrencyAmount(currencyType);
		Helpers.GameObjectSetActive(leftContainer, value: false);
		HelpersUI.SetTraitsIconOnSprite(skillTokenIcon, skillKitTokenSetDefinition.TopIcon, skillKitTokenSetDefinition.TopIconOnCloud);
		skillTokenIconBg.spriteName = skillKitTokenSetDefinition.BGIcon;
		FreshListData();
	}

	private void FreshListData()
	{
		ClearEntries();
		List<SPTraitsRemoldDefinitions> sPTraitsRemoldSkillList = Helpers.GetSPTraitsRemoldSkillList(currencyType);
		sPTraitsRemoldSkillList.RemoveAll((SPTraitsRemoldDefinitions x) => x.Level != 1);
		int count = sPTraitsRemoldSkillList.Count;
		for (int num = 0; num < count; num++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<SPRemoldSkillTokenTipsPopupItem>(out var component))
			{
				component.Setup(sPTraitsRemoldSkillList[num]);
			}
			Entries.Add(gameObject);
		}
		EntryContainer.GetComponent<UITable>().Reposition();
	}

	private IEnumerator FreshListDataCoroutine()
	{
		yield return new WaitForEndOfFrame();
		EntryContainer.GetComponent<UITable>().Reposition();
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	public void OnclickCloseSkillContent()
	{
		Helpers.GameObjectSetActive(leftContainer, value: false);
	}

	public void UpdateSkillcontent(string definitionId)
	{
		SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(definitionId);
		if (sPTraitsRemodeDefinition != null)
		{
			Helpers.GameObjectSetActive(leftContainer, value: true);
			left.Setup(sPTraitsRemodeDefinition);
		}
	}
}
