using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SupportTalentPopup : HUDElement
{
	[SerializeField]
	private SupportTalentTreeCardList treeCardList;

	[SerializeField]
	private UILabel talentNameLabel;

	[SerializeField]
	private UILabel talentCurLevelLabel;

	[SerializeField]
	private UILabel talentCurDesLabel;

	[SerializeField]
	private UILabel talentNextLevelLabel;

	[SerializeField]
	private UILabel talentNextDesLabel;

	[SerializeField]
	private UITexture talentTexture;

	[SerializeField]
	private UISprite talentBg;

	[SerializeField]
	private UIButton levelUpButton;

	[SerializeField]
	private UILabel levelUpLabel;

	[SerializeField]
	private GameObject talentPointPrefab;

	[SerializeField]
	private GameObject talentTraitPointPrefab;

	[SerializeField]
	private GameObject talentDetailContent;

	[SerializeField]
	private GameObject talentCurContent;

	[SerializeField]
	private GameObject talentRightArrow;

	[SerializeField]
	private GameObject talentMaxLevelContent;

	[SerializeField]
	private GameObject talentTreePrefab;

	[SerializeField]
	private GameObject consumeContent;

	[SerializeField]
	private GameObject consumeContainer;

	[SerializeField]
	private GameObject consumePrefab;

	[SerializeField]
	private UILabel talentLevelUpLabel;

	[SerializeField]
	private UIScrollView curScrollView;

	[SerializeField]
	private UIScrollView nextScrollView;

	[SerializeField]
	private Color[] rarityColors;

	[SerializeField]
	private GameObject supportEffectGo;

	private SupportModel supportModel;

	private int selectTreeID;

	private int selectNodeID;

	public SupportTalentPointCard selectedPointCard;

	private readonly List<GameObject> _consumeEntries = new List<GameObject>();

	private readonly List<GameObject> _talentPointEntries = new List<GameObject>();

	private float pointCreatePositionX = 20f;

	private float pointCreatePositionY = 374f;

	private float pointCreateOffset = 107f;

	private float traitPointCreateOffset = 160f;

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SupportTalentSelectedEvent" && parameter is SupportTalentTreeMainDefinition supportTalentTreeMainDefinition && supportTalentTreeMainDefinition.Id != selectTreeID)
		{
			if (supportModel.Level >= supportTalentTreeMainDefinition.UnlockLevel)
			{
				RefreshTalentTreePointContent(supportTalentTreeMainDefinition.Id);
				selectTreeID = supportTalentTreeMainDefinition.Id;
			}
			else
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HUDNotification).Open();
				HUDNotification.Info(LocalizationManager.GetText("SupportTalentTreeUI_Tips", supportTalentTreeMainDefinition.UnlockLevel));
			}
		}
	}

	public void Show(SupportModel supportModel)
	{
		this.supportModel = supportModel;
		talentBg.color = GetRarityColor();
		talentTexture.mainTexture = HelpersGfx.LoadSupportIcon(supportModel.SupportId);
		Helpers.GameObjectSetActive(supportEffectGo, supportModel.Level > 5);
		List<SupportTalentTreeMainDefinition> list = new List<SupportTalentTreeMainDefinition>();
		if (supportModel.SupportTalentTreeModels != null)
		{
			foreach (SupportTalentTreeModel supportTalentTreeModel in supportModel.SupportTalentTreeModels)
			{
				list.Add(supportTalentTreeModel.Definition);
			}
		}
		treeCardList.InitContentList(supportModel, list);
	}

	public void RefreshTalentTreePointContent(int id)
	{
		SupportTalentTreeModel supportTalentTreeModelByID = supportModel.GetSupportTalentTreeModelByID(id);
		if (supportTalentTreeModelByID != null)
		{
			pointCreatePositionY = 374f;
			InitTalentTree(supportTalentTreeModelByID);
		}
	}

	public void RefreshRightPanel()
	{
		ClearConsumeEntries();
		SupportTalentDefinition currentTalentNodeDefinition = selectedPointCard.talentModel.GetCurrentTalentNodeDefinition();
		if (currentTalentNodeDefinition.Level == 0)
		{
			SupportTalentDefinition nextLevelTalentNodeDefinition = selectedPointCard.talentModel.GetNextLevelTalentNodeDefinition();
			RefreshBasicInfo(nextLevelTalentNodeDefinition);
			RefreshLevelZeroState(currentTalentNodeDefinition);
		}
		else if (currentTalentNodeDefinition.Level < selectedPointCard.talentModel.GetMaxLevel())
		{
			RefreshBasicInfo(currentTalentNodeDefinition);
			RefreshUpgradableState(currentTalentNodeDefinition);
		}
		else
		{
			RefreshBasicInfo(currentTalentNodeDefinition);
			RefreshMaxLevelState();
		}
		consumeContainer.GetComponent<UITable>().Reposition();
		curScrollView.ResetPosition();
		nextScrollView.ResetPosition();
	}

	private void RefreshBasicInfo(SupportTalentDefinition definition)
	{
		HelpersUI.SetContentToLabel(talentNameLabel, LocalizationManager.GetText(selectedPointCard.talentModel.GetTalentName()));
		HelpersUI.SetContentToLabel(talentCurLevelLabel, definition.Level + "/" + selectedPointCard.talentModel.GetMaxLevel());
		RefreshTalentDescription(definition);
	}

	private void RefreshTalentDescription(SupportTalentDefinition definition)
	{
		if (definition.Type == SupportTalentType.Attribute)
		{
			HelpersUI.SetContentToLabel(talentCurDesLabel, LocalizationManager.GetText(definition.TalentTraitDesc, definition.TalentAttributeValue));
		}
		else if (definition.Type == SupportTalentType.Trait)
		{
			TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(definition.TalentTrait);
			if (traitDefinition != null)
			{
				UILabel label = talentCurDesLabel;
				string talentTraitDesc = definition.TalentTraitDesc;
				object[] arguments = traitDefinition.ConstructionParameters.ToArray();
				HelpersUI.SetContentToLabel(label, LocalizationManager.GetText(talentTraitDesc, arguments));
			}
		}
	}

	private void RefreshLevelZeroState(SupportTalentDefinition definition)
	{
		SetContentVisibility(curContent: true, rightArrow: false, maxLevelContent: false, content: true);
		HelpersUI.SetContentToLabel(talentLevelUpLabel, LocalizationManager.GetText("SupportTalentTreeUI_UnlockButton"));
		RefreshConsumeEntries(definition);
		RefreshButtonState(definition);
	}

	private void RefreshUpgradableState(SupportTalentDefinition definition)
	{
		SupportTalentDefinition nextLevelTalentNodeDefinition = selectedPointCard.talentModel.GetNextLevelTalentNodeDefinition();
		HelpersUI.SetContentToLabel(talentLevelUpLabel, LocalizationManager.GetText("SupportTalentTreeUI_Upgrade"));
		SetContentVisibility(curContent: true, rightArrow: true, maxLevelContent: true, content: true);
		RefreshNextLevelInfo(nextLevelTalentNodeDefinition, definition.Level);
		RefreshConsumeEntries(definition);
		RefreshButtonState(definition);
	}

	private void RefreshMaxLevelState()
	{
		SetContentVisibility(curContent: true, rightArrow: false, maxLevelContent: false, content: false);
		Helpers.GameObjectSetActive(levelUpLabel.gameObject, value: true);
		HelpersUI.SetContentToLabel(levelUpLabel, LocalizationManager.GetText("SupportTalentTreeUI_MaxLevel"));
	}

	private void SetContentVisibility(bool curContent, bool rightArrow, bool maxLevelContent, bool content)
	{
		Helpers.GameObjectSetActive(talentCurContent, curContent);
		Helpers.GameObjectSetActive(talentRightArrow, rightArrow);
		Helpers.GameObjectSetActive(talentMaxLevelContent, maxLevelContent);
		Helpers.GameObjectSetActive(consumeContent, content);
	}

	private void RefreshNextLevelInfo(SupportTalentDefinition nextLevelDefinition, int currentLevel)
	{
		HelpersUI.SetContentToLabel(talentNextLevelLabel, currentLevel + 1 + "/" + selectedPointCard.talentModel.GetMaxLevel());
		if (nextLevelDefinition.Type == SupportTalentType.Attribute)
		{
			HelpersUI.SetContentToLabel(talentNextDesLabel, LocalizationManager.GetText(nextLevelDefinition.TalentTraitDesc, nextLevelDefinition.TalentAttributeValue));
		}
		else if (nextLevelDefinition.Type == SupportTalentType.Trait)
		{
			TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(nextLevelDefinition.TalentTrait);
			if (traitDefinition != null)
			{
				UILabel label = talentNextDesLabel;
				string talentTraitDesc = nextLevelDefinition.TalentTraitDesc;
				object[] arguments = traitDefinition.ConstructionParameters.ToArray();
				HelpersUI.SetContentToLabel(label, LocalizationManager.GetText(talentTraitDesc, arguments));
			}
		}
	}

	private void RefreshConsumeEntries(SupportTalentDefinition definition)
	{
		if (definition.PrimarySupportTalentTokenAmount > 0)
		{
			AddConsumeEntries(HelpersGfx.GetCurrencyIconName(CurrencyType.PrimarySupportTalentToken), GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.PrimarySupportTalentToken), definition.PrimarySupportTalentTokenAmount);
		}
		if (definition.AdvancedSupportTalentTokenAmount > 0)
		{
			AddConsumeEntries(HelpersGfx.GetCurrencyIconName(CurrencyType.AdvancedSupportTalentToken), GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.AdvancedSupportTalentToken), definition.AdvancedSupportTalentTokenAmount);
		}
		AddConsumeEntries(HelpersGfx.GetCurrencyIconName(supportModel.Currency), GameManager.Instance.playerModel.GetCurrencyAmount(supportModel.Currency), definition.SupportTokenAmount);
	}

	private void RefreshButtonState(SupportTalentDefinition definition)
	{
		SupportTalentTreeModel supportTalentTreeModelByID = supportModel.GetSupportTalentTreeModelByID(selectTreeID);
		Cashier upgradeNodeCashierByNodeId = supportTalentTreeModelByID.GetUpgradeNodeCashierByNodeId(definition.SupportTalentId);
		if (!supportTalentTreeModelByID.CanUpgradeNodeByNodeId(selectedPointCard.talentModel.GetCurrentTalentNodeId()))
		{
			SetButtonState(showButton: false, showLabel: true);
			int requireTrunkMinLevel = supportTalentTreeModelByID.GetNodeModelByNodeId(selectedPointCard.talentModel.GetCurrentTalentNodeId()).GetRequireTrunkMinLevel();
			HelpersUI.SetContentToLabel(levelUpLabel, LocalizationManager.GetText("SupportTalentTreeUI_Unlock", requireTrunkMinLevel));
		}
		else if (upgradeNodeCashierByNodeId.CanAfford())
		{
			SetButtonState(showButton: true, showLabel: false);
		}
		else
		{
			SetButtonState(showButton: false, showLabel: true);
			HelpersUI.SetContentToLabel(levelUpLabel, LocalizationManager.GetText("SupportTalentTreeUI_InsufficientMaterials"));
		}
	}

	private void SetButtonState(bool showButton, bool showLabel)
	{
		Helpers.GameObjectSetActive(levelUpButton.gameObject, showButton);
		Helpers.GameObjectSetActive(levelUpLabel.gameObject, showLabel);
	}

	public void OnLevelUpClicked()
	{
		SupportTalentTreeModel supportTalentTreeModelByID = supportModel.GetSupportTalentTreeModelByID(selectTreeID);
		int currentTalentNodeId = selectedPointCard.talentModel.GetCurrentTalentNodeId();
		if (supportTalentTreeModelByID.GetUpgradeNodeCashierByNodeId(currentTalentNodeId).CanAfford() && Helpers.ExecuteCommand(new UpgradeSupportTalentNodeCommand(supportModel.ModelId, supportTalentTreeModelByID.ModelId, currentTalentNodeId)) == TWDModelResult.OK)
		{
			SupportTalentUpdatePopup supportTalentUpdatePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SupportTalentUpdatePopup) as SupportTalentUpdatePopup;
			if ((bool)supportTalentUpdatePopup)
			{
				supportTalentUpdatePopup.Open();
				supportTalentUpdatePopup.SetContent(selectedPointCard.talentModel);
			}
			SupportDetailsPopup supportDetailsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SupportDetailsPopup) as SupportDetailsPopup;
			if ((bool)supportDetailsPopup)
			{
				supportDetailsPopup.RefreshTalent();
			}
			RefreshTalentTreePointContent(selectTreeID);
			RefreshRightPanel();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
		}
	}

	private void AddConsumeEntries(string spriteName, int haveNum, int useNum)
	{
		GameObject gameObject = consumeContainer.AddChild(consumePrefab);
		gameObject.TryGetComponent<SupportConsumeCard>(out var component);
		_consumeEntries.Add(gameObject);
		component.SetContent(spriteName, haveNum, useNum);
	}

	private void ClearConsumeEntries()
	{
		for (int i = 0; i < _consumeEntries.Count; i++)
		{
			NGUITools.Destroy(_consumeEntries[i]);
		}
		_consumeEntries.Clear();
	}

	private void InitTalentTree(SupportTalentTreeModel treeModel)
	{
		ClearTalentPointEntries();
		SupportTalentNodeTrunkModel firstTalentNodeByTrunkNodes = SupportHelpers.GetFirstTalentNodeByTrunkNodes(treeModel.TrunkNodes);
		SupportTalentPointCard card = CreateTrunkNodeCard(treeModel, firstTalentNodeByTrunkNodes);
		if (selectTreeID != treeModel.TreeId || selectNodeID == firstTalentNodeByTrunkNodes.TrunkId)
		{
			SetSelectedCard(card, firstTalentNodeByTrunkNodes.TrunkId, treeModel.TreeId);
		}
		GenerateSubsequentNodes(treeModel, firstTalentNodeByTrunkNodes.TrunkId);
		RefreshRightPanel();
	}

	private void GenerateSubsequentNodes(SupportTalentTreeModel treeModel, int startTrunkId)
	{
		int requireTrunkId = startTrunkId;
		SupportTalentTreeTrunkDefinition supportTalentTreeTrunkDefinitionByRequireTrunkId = GameManager.Instance.gameEconomyData.GetSupportTalentTreeTrunkDefinitionByRequireTrunkId(requireTrunkId);
		while (supportTalentTreeTrunkDefinitionByRequireTrunkId != null && treeModel.GetNodeModelByNodeId(supportTalentTreeTrunkDefinitionByRequireTrunkId.TrunkId) is SupportTalentNodeTrunkModel supportTalentNodeTrunkModel)
		{
			SupportTalentPointCard card = CreateTrunkNodeCard(treeModel, supportTalentNodeTrunkModel);
			TrySelectCard(card, supportTalentNodeTrunkModel.TrunkId);
			GenerateBranchNode(treeModel, supportTalentNodeTrunkModel);
			requireTrunkId = supportTalentNodeTrunkModel.TrunkId;
			supportTalentTreeTrunkDefinitionByRequireTrunkId = GameManager.Instance.gameEconomyData.GetSupportTalentTreeTrunkDefinitionByRequireTrunkId(requireTrunkId);
		}
	}

	private SupportTalentPointCard CreateTrunkNodeCard(SupportTalentTreeModel treeModel, SupportTalentNodeTrunkModel trunkModel)
	{
		var (flag, canAfford) = GetNodeUpgradeStatus(treeModel, trunkModel.TrunkId);
		return AddTalentPointEntry(trunkModel, canAfford, !flag);
	}

	private void GenerateBranchNode(SupportTalentTreeModel treeModel, SupportTalentNodeTrunkModel trunkModel)
	{
		if (trunkModel.SupportTalentNodeBranchModel != null)
		{
			SupportTalentNodeBranchModel supportTalentNodeBranchModel = trunkModel.SupportTalentNodeBranchModel;
			(bool canUpdate, bool canAfford) nodeUpgradeStatus = GetNodeUpgradeStatus(treeModel, supportTalentNodeBranchModel.BranchId);
			bool item = nodeUpgradeStatus.canUpdate;
			bool item2 = nodeUpgradeStatus.canAfford;
			SupportTalentPointCard card = AddTalentTraitPointEntry(supportTalentNodeBranchModel, item2, !item, supportTalentNodeBranchModel.GetDirection());
			TrySelectCard(card, supportTalentNodeBranchModel.BranchId);
		}
	}

	private (bool canUpdate, bool canAfford) GetNodeUpgradeStatus(SupportTalentTreeModel treeModel, int nodeId)
	{
		bool num = treeModel.CanUpgradeNodeByNodeId(nodeId);
		bool item = false;
		if (num)
		{
			item = treeModel.GetUpgradeNodeCashierByNodeId(nodeId).CanAfford();
		}
		return (canUpdate: num, canAfford: item);
	}

	private void TrySelectCard(SupportTalentPointCard card, int nodeId)
	{
		if (selectNodeID == nodeId)
		{
			SetSelectedCard(card, nodeId, selectTreeID);
		}
	}

	private void SetSelectedCard(SupportTalentPointCard card, int nodeId, int treeId)
	{
		selectedPointCard = card;
		selectedPointCard.SetSelect(isSelect: true);
		selectNodeID = nodeId;
		selectTreeID = treeId;
	}

	private SupportTalentPointCard AddTalentPointEntry(SupportTalentNodeTrunkModel trunkModel, bool canAfford, bool isLock)
	{
		GameObject gameObject = talentDetailContent.AddChild(talentPointPrefab);
		pointCreatePositionY -= pointCreateOffset;
		gameObject.transform.localPosition = new Vector3(pointCreatePositionX, pointCreatePositionY, 0f);
		SupportTalentPointCard component = gameObject.GetComponent<SupportTalentPointCard>();
		component.SetContent(trunkModel, canAfford, isLock);
		bool trunkTalentLine = trunkModel.GetRequireTrunkId() != 0;
		component.SetTrunkTalentLine(trunkTalentLine);
		if (trunkModel.SupportTalentNodeBranchModel != null)
		{
			component.SetBranchTalentLine(trunkModel.SupportTalentNodeBranchModel.GetDirection());
		}
		SetupCardClickEvent(component);
		_talentPointEntries.Add(gameObject);
		return component;
	}

	private SupportTalentPointCard AddTalentTraitPointEntry(SupportTalentNodeBranchModel branchModel, bool canAfford, bool isLock, SupportTalentTreeBranchDirection direction)
	{
		GameObject gameObject = talentDetailContent.AddChild(talentTraitPointPrefab);
		float x = direction switch
		{
			SupportTalentTreeBranchDirection.Left => pointCreatePositionX - traitPointCreateOffset,
			SupportTalentTreeBranchDirection.Right => pointCreatePositionX + traitPointCreateOffset,
			_ => pointCreatePositionX,
		};
		gameObject.transform.localPosition = new Vector3(x, pointCreatePositionY, 0f);
		SupportTalentPointCard component = gameObject.GetComponent<SupportTalentPointCard>();
		component.SetContent(branchModel, canAfford, isLock);
		SetupCardClickEvent(component);
		_talentPointEntries.Add(gameObject);
		return component;
	}

	private void SetupCardClickEvent(SupportTalentPointCard card)
	{
		EventDelegate.Set(card.talentButton.onClick, delegate
		{
			OnTalentCardClicked(card);
		});
	}

	private void OnTalentCardClicked(SupportTalentPointCard clickedCard)
	{
		if (!(selectedPointCard == clickedCard))
		{
			clickedCard.SetSelect(isSelect: true);
			selectedPointCard?.SetSelect(isSelect: false);
			selectedPointCard = clickedCard;
			selectNodeID = clickedCard.talentModel.GetCurrentTalentNodeId();
			RefreshRightPanel();
		}
	}

	private void ClearTalentPointEntries()
	{
		for (int i = 0; i < _talentPointEntries.Count; i++)
		{
			NGUITools.Destroy(_talentPointEntries[i]);
		}
		_talentPointEntries.Clear();
	}

	private Color GetRarityColor()
	{
		return rarityColors[Mathf.Clamp(supportModel.Level - 1, 0, rarityColors.Length - 1)];
	}
}
