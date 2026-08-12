using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class RemodelPopup : HUDElement
{
	public static string RemodelExchangePlayerPrefs = "RemodelExchangePlayerPrefs";

	[SerializeField]
	private UILabel traitLabel;

	[SerializeField]
	private UISprite traitSprite;

	[SerializeField]
	private UILabel[] traitSelectLabels;

	[SerializeField]
	private Animator[] effectAnimators;

	[SerializeField]
	private GameObject TraitContainer;

	[SerializeField]
	private UILabel GoldAmount;

	[SerializeField]
	private UILabel RemodelTokenAmount;

	[SerializeField]
	private GameObject CheckBtnGO;

	[SerializeField]
	private UIButton RemodelBtn;

	private EquipmentItemModel equipmentItemModel;

	private UpgradeTraitsData traitData;

	private int _needToken;

	private int _needTokenParam;

	private int _needGold;

	private float delayTime = 1f;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public override void Open()
	{
		base.Open();
		List<int> equipTraitsRemodelToken = GameManager.Instance.gameEconomyData.ConfigData.EquipTraitsRemodelToken;
		_needToken = equipTraitsRemodelToken[0];
		_needTokenParam = equipTraitsRemodelToken[1];
		_needGold = playerModel.ActivityManager.GetEquipTraitsRemodelGold(GameManager.Instance.gameEconomyData.ConfigData);
		UpdateUI();
	}

	public override void UpdateUI()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");

			TraitSelectorButon.gameObject.SetActive(IsBatchRemodelTree);
			if (TraitSelectorButon.gameObject.activeSelf)
			{
				var gameEconomyData = DataManager.Instance.GameData;
				int upgradeTraitsDataIndex = equipmentItemModel.GetUpgradeTraitsDataIndex(traitData.Identifier);

				int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(traitData.Identifier);
				List<string> expectTratIdList = GetExpectTratIdList(traitLevelIdentifier);
				List<EquipTraitsDefinition> equipTraitsDefinitions = gameEconomyData.getEquipTraitsDefinitions(equipmentItemModel.Definition.SurvivorClass, equipmentItemModel.Definition.Category, upgradeTraitsDataIndex, traitLevelIdentifier, expectTratIdList);

				var trait = equipTraitsDefinitions.First();

				SetInstansTraitData(HelpersGfx.GetEquipmentTraitIconNameUsingTraitDefinition(trait), trait.TraitsQualityLevel);
			}
		}
		base.UpdateUI();
		ShwoEffect(show: false);
		if (equipmentItemModel != null && traitData != null)
		{
			string level = "";
			traitLabel.text = HelpersLocalization.GetLastInstantiatedTraitDescription(traitData);
			traitSprite.spriteName = HelpersGfx.GetEquipmentTraitIconName(traitData);
			if (IsLoadDataManager)
			{
				level = traitSprite.spriteName.Split('_').Last();
			}
			Helpers.GameObjectSetActive(CheckBtnGO, IsExchange());
			int value = playerModel.GetCurrency(CurrencyType.Diamonds).Value;
			GoldAmount.text = value + " / " + _needGold;
			int value2 = playerModel.GetCurrency(CurrencyType.EquipTraitsRemodelToken).Value;
			RemodelTokenAmount.text = value2 + " / " + _needToken;
			if (traitData.RemodelIng)
			{
				UpgradeTraitsData upgradeTraitsData = new UpgradeTraitsData();
				upgradeTraitsData.RemodelIng = true;
				upgradeTraitsData.Identifier = traitData.ThisRemodeIds[0];
				upgradeTraitsData.ThisRemodeIds = traitData.ThisRemodeIds;
				upgradeTraitsData.ThisRemodeValues = traitData.ThisRemodeValues;
				upgradeTraitsData.ThisRemodeParamIndex = traitData.ThisRemodeParamIndex;
				traitSelectLabels[0].text = HelpersLocalization.GetInstantiatedTraitDescription(upgradeTraitsData);
				if (IsLoadDataManager)
				{
					var traitReSprite1Name = HelpersGfx.GetEquipmentTraitIconName(upgradeTraitsData);
					var level1 = traitReSprite1Name.Split('_').Last();
					traitReSprite1.spriteName = traitReSprite1Name.Replace(level1, level);
				}
				upgradeTraitsData.Identifier = traitData.ThisRemodeIds[1];
				traitSelectLabels[1].text = HelpersLocalization.GetInstantiatedTraitDescription(upgradeTraitsData);
				if (IsLoadDataManager)
				{
					var traitReSprite2Name = HelpersGfx.GetEquipmentTraitIconName(upgradeTraitsData);
					var level2 = traitReSprite2Name.Split('_').Last();

					if (level == "Highest" && traitReSprite2Name.Contains("Tactical"))
						traitReSprite2Name = traitReSprite2Name.Replace("Tactical", "ArmorTactical");
					traitReSprite2.spriteName = traitReSprite2Name.Replace(level2, level);
				}
				Helpers.GameObjectSetActive(TraitContainer, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(TraitContainer, value: false);
			}
		}
	}

	public override void Close()
	{
		if (!IsLoadDataManager)
		{
			if (traitData != null && traitData.RemodelIng)
			{
				EquipmentUpgradePopup equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
				if (equipmentUpgradePopup != null && equipmentUpgradePopup.IsOpen)
				{
					equipmentUpgradePopup.Close();
				}
			}
		}
		else
		{
			if (IsOpenRemodelTree)
			{
				var tree = DataManager.Instance.SurvivorManagementPopUp.remodelTraitsTree;
				if (tree && tree.gameObject.activeSelf)
				{
					if (tree.AllTreeItemsCount > 0)
						tree.DestroyAll();
					tree.SetRemodelPopup(this);
				}
			}
			else
			{
				if (traitData != null && traitData.RemodelIng)
				{
					string ru = "Пожалуйста сделайте выбор перед выходом из ремодела";
					string eng = "Please make a choice before exit remodel window";
					string text = DataManager.Instance.language == DataManager.Language.Ru ? ru : eng;

					AlertPopup.ShowPopup("", text, LocalizationManager.GetText("Button.Ok"));
					return;
				}
			}
		}

		base.Close();
	}

	public void InitData(EquipmentItemModel equipmentModel, UpgradeTraitsData upgradeTraitData)
	{
		equipmentItemModel = equipmentModel;
		traitData = upgradeTraitData;
	}

	public void ClickCheckBtn()
	{
		if (!IsLoadDataManager)
		{
			bool temp = IsExchange();
			if (temp)
			{
				Helpers.GameObjectSetActive(CheckBtnGO, !temp);
				SetExchange(!temp);
				return;
			}
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent("", LocalizationManager.GetText("Popup.EquipRemodel.Label2"));
			obj.SetCallbacks(delegate
			{
				Helpers.GameObjectSetActive(CheckBtnGO, !temp);
				SetExchange(!temp);
			});
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
		}
		else
		{
			bool flag = IsExchange();
			Helpers.GameObjectSetActive(CheckBtnGO, !flag);
			SetExchange(!flag);
		}
	}

	public void ClickRemodelBtn()
	{
		if (!IsLoadDataManager)
		{
			int equipmentRemodelRarity = GameManager.Instance.gameEconomyData.ConfigData.EquipmentRemodelRarity;
			if (equipmentItemModel.RarityLevel >= equipmentRemodelRarity && traitData.UnlockingLevel <= equipmentItemModel.Level && CheckCoin())
			{
				Helpers.ExecuteCommandDelayed(new EquipRemodelCommand(equipmentItemModel, traitData.Identifier, IsExchange()), OnNewEquipRemodePopup);
			}
		}
		else
		{
			if (IsOpenRemodelTree)
			{
				if (IsBatchRemodelTree && !CoroutineStarted)
				{
					StartCoroutine(StartRemodelBatch());
					return;
				}
				RemodelTraitsTree tree = DataManager.Instance.SurvivorManagementPopUp != null ? DataManager.Instance.SurvivorManagementPopUp.remodelTraitsTree : null;
				if (tree)
				{
					if (tree.AllTreeItemsCount > 0)
						tree.DestroyAll();
				}
				else
				{
					DebugTWD.LogError("RemodelTraitsTree не найден. Return", DebugType.Error);
					return;
				}

				tree.SetRemodelPopup(this);
				tree.gameObject.SetActive(true);
				tree.equipmentItemModel = equipmentItemModel;
				tree.upgradeTraitsData = traitData;
				tree.upgradeTraitsDataIdentifierOrigin = traitData.Identifier;
				tree.traitIndex = equipmentItemModel.GetUpgradeTraitsDataIndex(traitData.Identifier);

				StartCoroutine(tree.Main(true));
			}
			else
			{
				SurvivorModel survivor = DataManager.Instance.SurvivorManagementPopUp != null ? DataManager.Instance.SurvivorManagementPopUp.SurvivorModel : null;

				DebugTWD.Log("Need to Initiate Survivors - Turn Off", DebugType.System);
				DataManager.Instance.SurvivorManagementPopUp.BackupTraitsData(equipmentItemModel, survivor);
				int equipmentRemodelRarity = playerModel.gameEconomyData.ConfigData.EquipmentRemodelRarity;
				if (equipmentItemModel.RarityLevel >= equipmentRemodelRarity && traitData.UnlockingLevel <= equipmentItemModel.Level && CheckCoin())
				{
					if (IsLoadDataManager)
					{
						TWDModelResult tWDModelResult = equipmentItemModel.EquipmentRemodel(traitData.Identifier, IsExchange());
						if (tWDModelResult != TWDModelResult.OK)
						{
							DebugTWD.Log("Remodel result is " + tWDModelResult);
							return;
						}
					}
					else
					{
						if (Helpers.ExecuteCommand(new EquipRemodelCommand(equipmentItemModel, traitData.Identifier, IsExchange())) != TWDModelResult.OK) return;
					}
				}
				UpdateUI();
				ShwoEffect(show: true);
			}
		}
	}

	private void OnNewEquipRemodePopup(bool result)
	{
		if (result)
		{
			GameManager.Instance.CheckConnectionReachability(showPopup: true, "EquipRemodelCommand");
			UpdateUI();
			ShwoEffect(show: true);
			HUDNotification.Info(LocalizationManager.GetText("Popup.EquipRemodel.Label3"));
			Helpers.GameObjectSetActive(RemodelBtn, value: false);
			Invoke("FreshRemodelBtn", delayTime);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_workshop");
	}

	private void FreshRemodelBtn()
	{
		Helpers.GameObjectSetActive(RemodelBtn, value: true);
	}

	public void ClickSelectBtn1()
	{
		SelectBtn(0);
	}

	public void ClickSelectBtn2()
	{
		SelectBtn(1);
	}

	public void ClickSelectBtn3()
	{
		SelectBtn(2);
	}

	private bool IsExchange()
	{
		if (TWDPlayerPrefs.GetInt(RemodelExchangePlayerPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	private void SetExchange(bool on)
	{
		if (on)
		{
			TWDPlayerPrefs.SetInt(RemodelExchangePlayerPrefs, 1);
		}
		else
		{
			TWDPlayerPrefs.SetInt(RemodelExchangePlayerPrefs, 0);
		}
		TWDPlayerPrefs.Save();
	}

	public void SelectBtn(int i_selectIndex)
	{
		if (IsLoadDataManager && equipmentItemModel.IsValid())
		{
			TWDModelResult tWDModelResult = equipmentItemModel.SelectRemodeId(traitData.Identifier, i_selectIndex);
			if (tWDModelResult == TWDModelResult.OK)
			{
				UpdateUI();
				UIEvent.Send("EquipmentRemodelSelectioned");
			}
		}
		else
		{
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent("", LocalizationManager.GetText("Popup.EquipRemodelConfirmationPopup.Label1"));
			obj.SetCallbacks(delegate
			{
				if (Helpers.ExecuteCommand(new EquipmentRemodelSelectionCommand(equipmentItemModel, traitData.Identifier, i_selectIndex)) == TWDModelResult.OK)
				{
					UpdateUI();
					UIEvent.Send("EquipmentRemodelSelectioned");
				}
			});
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
		}
	}

	private void ShwoEffect(bool show)
	{
		if (IsLoadDataManager && IsBatchRemodelTree)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && IsBatchRemodelTree) return");
			return;
		}
		for (int i = 0; i < effectAnimators.Length; i++)
		{
			if (show)
			{
				effectAnimators[i].Play(Helpers.ShowNameHash);
			}
			else
			{
				effectAnimators[i].Play(Helpers.HideNameHash);
			}
		}
	}

	private bool CheckCoin()
	{
		int num = 0;
		int value = playerModel.GetCurrency(CurrencyType.EquipTraitsRemodelToken).Value;
		if (!IsExchange())
		{
			if (value < _needToken)
			{
				TooltipManager.OpenTextBoxWithText(RemodelBtn.gameObject, LocalizationManager.GetText("Popup.EquipRemodel.tips1"));
				return false;
			}
		}
		else if (value < _needToken)
		{
			num = _needToken - value;
		}
		int num2 = 0;
		if (IsExchange())
		{
			num2 += _needTokenParam * num;
		}
		num2 += _needGold;
		if (playerModel.GetCurrency(CurrencyType.Diamonds).Value < num2)
		{
			TooltipManager.OpenTextBoxWithText(RemodelBtn.gameObject, LocalizationManager.GetText("Popup.EquipRemodel.tips2"));
			return false;
		}
		return true;
	}

	public void OnClickInfoButton()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeaponRemodelInfoPopup).Open();
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	[SerializeField]
	private UISprite traitReSprite1;
	[SerializeField]
	private UISprite traitReSprite2;
	public UILabel TokenAmountLabel;
	private bool CoroutineStarted;
	public UISprite traitInstanceSprite;
	public UISprite traitInstanceBackground;
	public UIButtonExtended TraitSelectorButon;
	public int batchCount { get; set; }
	public bool IsFineConstructFinded { get; set; }

	public bool IsOpenRemodelTree { get; set; }
	public bool IsBatchRemodelTree { get; set; }
	public string TraintInstanceID { get; set; }
	#endregion

	#region mycode
	private void OnEnable()
	{
		if (!IsLoadDataManager) return;
		TraintInstanceID = string.Empty;
		UIEvent.OnUIEvent += OnUiEvent;
		PlayerRandomValues.Instance.On_Call_Reset += OnClickReset;
	}

	private void OnDisable()
	{
		if (!IsLoadDataManager) return;
		UIEvent.OnUIEvent -= OnUiEvent;
		PlayerRandomValues.Instance.On_Call_Reset -= OnClickReset;
	}

	private void OnClickReset(bool isZeroCounter)
	{
		if (isZeroCounter)
			IsFineConstructFinded = true;
	}

	public void SwitchRemodelTypeToggle(UIToggle tg)
	{
		IsOpenRemodelTree = tg.value;
	}

	public void SwitchRemodelAutoToggle(UIToggle tg)
	{
		IsBatchRemodelTree = tg.value;
		TraitSelectorButon.gameObject.SetActive(tg.value);

		if (TraitSelectorButon.gameObject.activeSelf)
		{
			TraitDefinition trait = null;
			var level = traitData.RarityLevel;
			SetInstansTraitData(HelpersGfx.GetEquipmentTraitIconNameUsingTraitDefinition(trait), level);
		}
	}
	public List<string> GetExpectTratIdList(int level)
	{
		var UpgradeTraits = equipmentItemModel.UpgradeTraits;
		List<string> list = new List<string>();
		for (int i = 0; i < UpgradeTraits.Count; i++)
		{
			int num = UpgradeTraits[i].Identifier.LastIndexOf('.');
			if (num != -1)
			{
				string item = UpgradeTraits[i].Identifier.Substring(0, num) + ".Level" + level;
				list.Add(item);
			}
			EquipTraitsMutualExclusion equipTraitsMutualExclusion = DataManager.Instance.GameData.getEquipTraitsMutualExclusion(UpgradeTraits[i].Identifier);
			if (equipTraitsMutualExclusion != null)
			{
				list.AddRange(equipTraitsMutualExclusion.MutualExclusionTraits);
			}
		}
		return list.Distinct().ToList();
	}

	public void SetCurrencyAmount()
	{
		int tokenAmount;
		try
		{
			tokenAmount = Convert.ToInt32(TokenAmountLabel.text.ToString());
		}
		catch
		{
			tokenAmount = 0;
		}
		playerModel.SetCurrency(CurrencyType.EquipTraitsRemodelToken, tokenAmount);
		int tokens = playerModel.GetCurrency(CurrencyType.EquipTraitsRemodelToken).Value;
		RemodelTokenAmount.text = tokens + " / " + _needToken;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "UpdateTraitInstance")
		{
			var item = parameter as TraitSelectorButton;

			SetInstansTraitData(item.Icon.spriteName, item.traitLevel);
		}
	}

	private void SetInstansTraitData(string spriteName, int level)
	{
		if (spriteName == "Ui_Icon_Trait_Unknown")
		{
			string text2 = "";
			switch (level)
			{
				case 0:
					text2 = "_Low";
					break;
				case 1:
					text2 = "_Mid";
					break;
				case 2:
				case 3:
				case 4:
				case 5:
					text2 = "_High";
					break;
				default:
					text2 = "";
					break;
			}
			spriteName += text2;
		}
		TraintInstanceID = spriteName;
		traitInstanceSprite.spriteName = spriteName;
		traitInstanceBackground.color = HelpersGfx.GetTraitRarityColor(level);
	}

	public void OnClickInstanceTrait(UIButtonExtended bt)
	{
		CraftSettings.Instance.ToolTipTraitLarge.SetActive(true);
		TooltipManager.OpenForTraitSlot(bt.gameObject, CraftSettings.Instance.ToolTipTraitLarge, traitData, equipmentItemModel, this.gameObject.layer);
	}

	public IEnumerator StartRemodelBatch()
	{
		RandomValuesPopup randomValuesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RandomValuesPopup) as RandomValuesPopup;
		if (randomValuesPopup != null)
		{
			if (!randomValuesPopup.IsOpen)
			{
				randomValuesPopup.Open();
			}

			IsFineConstructFinded = false;

			CoroutineStarted = true;
			batchCount = 0;

			while (true)
			{
				int count = batchCount;
				ClickRemodelBtn();
				yield return new WaitUntil(() => batchCount > count);
				if (IsFineConstructFinded || count > 1000 || !IsBatchRemodelTree)
				{
					CoroutineStarted = false;
					IsFineConstructFinded = false;

					DebugTWD.Log("FineConstruct Finded for " + (count + 1).ToString() + "repeates");
					yield break;
				}
				randomValuesPopup.Change_HubUp();
			}
		}
		else
		{
			CoroutineStarted = false;
			IsFineConstructFinded = false;

			yield break;
		}
	}
	#endregion
}
