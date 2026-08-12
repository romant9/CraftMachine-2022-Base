using System.Collections;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorTraitsList : MonoBehaviourExtended
{
	[SerializeField]
	private SurvivorInfoTraitButton[] traitButtonsArray;

	[SerializeField]
	private UISprite talkingDeadIcon;

	private void Awake()
	{
		DebugIdString = "SurvivorTraitsList";
	}

	//обновление листа навыков справа
	public void UpdateWith(SurvivorModel survivorModel, bool skipFirstTrait = true)
	{
		SurvivorManagementPopUp survivorManagementPopUp = null;
		if (OfflineManager.IsLoadDataManager)
		{
			survivorManagementPopUp = DataManager.Instance.SurvivorManagementPopUp;
			if (survivorManagementPopUp != null && survivorManagementPopUp.gameObject.activeSelf && survivorManagementPopUp.EquipmentButtonClicked) return;
		}

		if (IsNotNull(survivorModel) && IsNotNull(traitButtonsArray))
		{
			List<UpgradeTraitsData> upgradeTraits = survivorModel.UpgradeTraits;
			UpgradeTraitsData nextUpgradeTraitsData = null;
			if (survivorModel.CanUpgradeTraitRarity())
			{
				nextUpgradeTraitsData = survivorModel.GetLowestLevelUpgradeTrait();
			}
			if (IsNotNull(upgradeTraits))
			{
				int num = 0;
				for (int i = 0; i < traitButtonsArray.Length; i++)
				{
					num = (skipFirstTrait ? (i + 1) : i);
					if (IsNotNull(traitButtonsArray[i]))
					{
						if (upgradeTraits.Count > num && IsNotNull(upgradeTraits[num]))
						{
							traitButtonsArray[i].UpdateWithTrait(upgradeTraits[num], survivorModel.SurvivorRarityLevel, nextUpgradeTraitsData, survivorModel);

							if (OfflineManager.IsLoadDataManager && survivorManagementPopUp.gameObject.activeSelf)
							{
								Dictionary<string, SurvivorTraits> traitList = survivorManagementPopUp.survivorTraitsList;

								string name = survivorModel.IsHero ? survivorModel.FullName : survivorModel.SurvivorName;
								var changedTrait = traitButtonsArray[i].transform.GetChild(1).gameObject;
								bool changedTraitState = changedTrait.activeSelf;
								changedTrait.SetActive(traitList.ContainsKey(name) ? traitList[name].TraitRerolledList[i] : false);
								if (changedTraitState != changedTrait.activeSelf)
								{
									//survivorManagementPopUp.SurvivorCardCurrent.UpdateUI();
									survivorManagementPopUp.survivorCardSelected.GetComponent<SurvivorCard>().UpdateUI();
									survivorManagementPopUp.SurvivorCardCurrent.ChangeTrait(i, upgradeTraits[num]);
								}
							}
						}
						else
						{
							traitButtonsArray[i].Hide();
						}
					}
				}
			}
		}
		if (survivorModel != null && survivorModel.ActorDefinitionID != null)
		{
			Helpers.GameObjectSetActive(talkingDeadIcon, survivorModel.ActorDefinitionID.ToLower().Contains("talkingdead"));
		}
	}

	#region mycode
	public IEnumerator UpdateTraits()
	{
		yield return new WaitForEndOfFrame();
		DataManager.Instance.SurvivorManagementPopUp.UpdateUI();
		if (DataManager.Instance.SurvivorManagementPopUp.survivorCardSelected != null)
			DataManager.Instance.SurvivorManagementPopUp.survivorCardSelected.GetComponent<SurvivorCard>().OnCardClicked();
	}
	#endregion
}
