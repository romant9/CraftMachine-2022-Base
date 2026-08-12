using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorInfoTraitButton : AnimatedUIButtonExtended
{
	[SerializeField]
	private UISprite mainSprite;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private GameObject nextUpgradeHighlight;

	[SerializeField]
	private GameObject fullyUpgradedHighlight;

	[SerializeField]
	private GameObject canRerollTrait;

	//кнопка навыка на правой панели навыков
	public void UpdateWithTrait(UpgradeTraitsData upgradeTraitsData, int survivorRarity, UpgradeTraitsData nextUpgradeTraitsData, SurvivorModel survivorModel)
	{
		if (upgradeTraitsData != null)
		{
			int rarityLevel = upgradeTraitsData.RarityLevel;
			TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
			traitIdentifier = traitDefinition.Identifier;

			if (mainSprite != null)
			{
				mainSprite.spriteName = HelpersGfx.GetSurvivorTraitIconName(upgradeTraitsData);
			}
			if (nameLabel != null && traitDefinition != null)
			{
				nameLabel.text = HelpersLocalization.GetTraitName(traitDefinition);
			}
			if (levelLabel != null)
			{
				levelLabel.text = "0";
				if (!upgradeTraitsData.IsLocked && !upgradeTraitsData.IsTactical)
				{
					int num = upgradeTraitsData.RarityLevel + 1;
					levelLabel.text = num.ToString();
				}
			}
			SurvivorInfoStateBase.States state;
			if (OfflineManager.IsLoadDataManager)
			{
				survivorManagementPop = DataManager.Instance.SurvivorManagementPopUp;
				if (survivorManagementPop == null || survivorManagementPop.SurvivorInfoPopupCurrent == null)
				{
					state = (SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup).currentStateMachineState;
				}
				else
				{
					state = survivorManagementPop.SurvivorInfoPopupCurrent.currentStateMachineState;
				}
			}
			else
			{
				state = (SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup).currentStateMachineState;
			}
			if (state == SurvivorInfoStateBase.States.SurvivorOverview && survivorModel.CanRerollTrait && !traitDefinition.HasTag("FactionBuffTrait") && survivorModel.GetTraitRerollCashier(traitDefinition.Identifier).CanAfford())
			{
				Helpers.GameObjectSetActive(canRerollTrait, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(canRerollTrait, value: false);
			}
			Clear();
			SetClickCallback(delegate
			{
				if (!OfflineManager.IsLoadDataManager)
				{
					(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.TraitInfoPopup) as TraitInfoPopup).OpenForModel(survivorModel, traitDefinition, state);
				}
				else
				{
					var index = int.TryParse(this.gameObject.name.Substring(this.gameObject.name.Length - 1), out int result);
					survivorManagementPop.rerollTraitIndexCurrent = result;

					var tree = survivorManagementPop.survivorTraitTree;

					bool isAvailable = survivorModel.IsHero ? result != 1 : true;

					if (survivorManagementPop.IsOpenTraitsTree && isAvailable)
					{
						tree.gameObject.SetActive(true);
						tree.survivorModel = survivorModel;
						tree.DestroyAll();
						StartCoroutine(tree.Main(traitDefinition));
					}
					else
					{
						var traitInfoPopup = survivorManagementPop.traitInfoPopup;
						traitInfoPopup.gameObject.SetActive(true);
						traitInfoPopup.OpenForModel(survivorModel, traitDefinition, state);
					}
				}
			});
			bool flag = Helpers.GameObjectSetActive(fullyUpgradedHighlight, rarityLevel == survivorRarity);
			Helpers.GameObjectSetActive(nextUpgradeHighlight, !flag && nextUpgradeTraitsData != null && upgradeTraitsData == nextUpgradeTraitsData);
			Show();
		}
		else
		{
			Hide();
			Debug.LogError("SurvivorTraitButton: Could not update. Data was NULL!");
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}


	#region myparams
	public string traitIdentifier { get; private set; }
	public SurvivorManagementPopUp survivorManagementPop { get; private set; }
	#endregion
}
