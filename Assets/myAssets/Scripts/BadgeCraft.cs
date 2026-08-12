using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWDModel;
using BaseModel;
using System;
using System.Linq;
using Math = System.Math;
using Client.Tweener;
using Client.Utils;

namespace TwdCustomMod
{
	public class BadgeCraft : MonoBehaviour
	{
		public static BadgeCraft Instance;

		public enum CraftMethod
		{
			Change,
			Delete,
			AddNew,
		}

		public CraftMethod craftMethod = CraftMethod.AddNew;

		private DataManager dataManager => DataManager.Instance;

		public int CurrentState = 0;
		public int CurrentCallCount;
		public int CurrentInitialSeed;

		private int CurrentBadgeAnalyticsId;

		public string PlayerHashID;

		public CraftTools craftTools { get; set; }

		public List<CurrencyType> components { get; set; }
		public List<CurrencyType> newComponents { get; set; }
		private bool IsComponentsCalculated;

		public string timeStampStatic { get; set; }
		private string timeStampStaticNet { get; set; }

		public bool IsSaveLog;

		public SurvivorBadgesIcon badgePrefab; //префаб значка

		public GameEconomyData gameEconomyData { get; private set; }

		public TWDModelManager ModelManager { get; private set; }

		//public Dictionary<string, ModelRandom> DedicatedRandoms { get; set; }
		public ModelRandom DedicatedRandoms { get; set; } //Players random (из content)
		public ModelRandom modelRandomInit { get; set; } //базовая, после инициации
		public ModelRandom modelRandomLast { get; set; } //последняя, после крафта
		private ModelRandom modelRandomCurrent { get; set; } //текущаяя - перед крафтом
		public ModelRandom modelRandomReroll { get; set; } //сохраняемая modelRandomLast - для реролла

		private int MaxBadgeCounts = 50;
		public int CraftRangeCount = 1;
		public int CurrentIndex { get; set; }

		private List<string> LogList = new List<string>();
		private List<BadgeGridItem> BadgeModels  = new List<BadgeGridItem>();

		public BadgeGridItem SelectedBadge { get; set; }
		private UITable table;// { get; private set; }
		private UIButtonSingleToggleSet toggleSet;
		public BadgeModel LastCraftedBadge { get; set; }

		public List<SurvivorBadgesIcon> RerollBadgeIcons { get; set; }

		public BadgeModel rerolledBadgeOrigin {  get; set; }
		public List<int> originHistorySlots;
		public List<BadgeType> originHistorySet;
		public List<(string BonusId, List<string> BonusParameters)> originHistoryBonus;

		public string bonusOrigin;

		public SurvivorBadgesIcon rerolledBadgeOriginIcon;
		public GameObject rerolledBadgeOriginBlank;

		public BadgeReroll rerollLast {  get; set; }
		public List<SurvivorBadgesIcon> RerollMultiBadgeIcons { get; set; }
		public BadgeRerollPopupCustom RerollMultiBadgePopup;
		private bool IsShow;

		//public GameObject BadgeRerollPopup;
		//public GameObject CraftingBadgeViewScroll;
		public GameObject RerollTarget;
		public GameObject BadgePanel;

		public bool IsAutoComponents;

		public bool IsStopCycle;
		public float cycleTime;
		public int startState = 1000000;
		public int endState = 2000000;

		//public int currentState;
		//public int currentRandom;
		//public float currentPercent;

		public UISlider slider;
		public BadgeModel testBadge1;
		public BadgeModel testBadge2;
		public BadgeModel testBadge3;

		List<CurrencyType> testComponents;

		public List<int> stateCalculateList1;
		public List<int> stateCalculateList2;
		public List<int> stateCalculateList3;

		public int comparesResult1;
		public int comparesResult2;
		public int comparesResult3;

		private bool IsCalculateMode;
		public int residenceLevelCustom;

		public int cyclePartAmount = 100;

		public bool IsUseRerollPool;

		public delegate void OnFafourite(int modelId);
		public event OnFafourite On_Fafourite;


		public void InvokeOnFavorite(int modelId)
		{
			On_Fafourite?.Invoke(modelId);
		}

		void Awake()
		{
			Instance = this;
		}

		public List<CurrencyType> DefaultComponents()
		{
			return new List<CurrencyType>()
			{
				CurrencyType.Badge4,
				CurrencyType.Metal4,
				CurrencyType.Food4,
				CurrencyType.Chemicals4,
				CurrencyType.Cloth4
			};
		}

		void Start()
		{
			//CurrentState = MyState;

			//components = new List<CurrencyType>()
			//{
			//    CurrencyType.Badge4,
			//    CurrencyType.Metal4,
			//    CurrencyType.Food4,
			//    CurrencyType.Chemicals4,
			//    CurrencyType.Cloth4
			//};

			//Button.Edit - изменить
			//Popup.SurvivorLevelUp.Button.Demote - удалить
			//Popup.Residence.Crafting.RecipesButton - рецепты - убрать последний символ
			//Popup.Residence.Crafting.CraftButton - создать | craft
			//Error.Error - ошибка
			//Droprate.Column.Amount - количество
			//Popup.MissionBriefing.TotalScore - всего
			//BattlePass.Progress.Max - Макс | Max

			//DedicatedRandoms = new Dictionary<string, ModelRandom>();
			table = badgePrefab.transform.parent.GetComponent<UITable>();
			toggleSet = badgePrefab.transform.parent.GetComponent<UIButtonSingleToggleSet>();

			//StartCoroutine(InitData());

			CurrentIndex = 0;
			RerollBadgeIcons = new List<SurvivorBadgesIcon>();
			RerollMultiBadgeIcons = new List<SurvivorBadgesIcon>();
			rerollLast = BadgeReroll.Bonus;
		}

		public void SetNewRandom(int state)
		{
			ModelRandom playerDedicatedRandoms = ModelManager?.Player?.LootManager?.GetDedicatedRandom("BadgeRandom");

			modelRandomInit = new ModelRandom()
			{
				State = state,
				InitialSeed = playerDedicatedRandoms != null ? playerDedicatedRandoms.InitialSeed : 0,
				CallCount = playerDedicatedRandoms != null ? playerDedicatedRandoms.CallCount : 0,
			};

			modelRandomCurrent = new ModelRandom(modelRandomInit);
			modelRandomLast = modelRandomCurrent;

			CurrentState = modelRandomCurrent.State;
			CurrentInitialSeed = modelRandomCurrent.InitialSeed;
			CurrentCallCount = modelRandomCurrent.CallCount;
		}

		public IEnumerator InitData()
		{
			yield return new WaitUntil(() => OfflineManager.Instance.IsPlayerLoaded);

			gameEconomyData = dataManager.GameData;
			ModelManager = dataManager.ModelManager;

			//Debug.Log("BadgeRandom " + DataManager.Instance.Player.LootManager.GetDedicatedRandom("BadgeRandom").State);// DedicatedRandoms["BadgeRandom"].State);

			DedicatedRandoms = ModelManager.Player.LootManager.GetDedicatedRandom("BadgeRandom");

			modelRandomInit = new ModelRandom(DedicatedRandoms);
			modelRandomCurrent = new ModelRandom(DedicatedRandoms);
			modelRandomLast = modelRandomCurrent;

			CurrentState = modelRandomCurrent.State;
			CurrentInitialSeed = modelRandomCurrent.InitialSeed;
			CurrentCallCount = modelRandomCurrent.CallCount;

			PlayerHashID = ModelManager.Player.HashedId;
			CurrentBadgeAnalyticsId = ModelManager.Player.LootManager.CurrentBadgeAnalyticsId;

			craftTools = new CraftTools()
			{
				GameData = gameEconomyData,
			};

			MaxBadgeCounts = CraftSettings.Instance.MaxBadgeCounts;

			CraftSettings.Instance.InitialStateInput.value = modelRandomInit.State.ToString();
			//1485921072
			//919380905
			//CraftSettings.Instance.InitialStateInput.value = DataManager.Instance.Player.PlayerRandom.State.ToString();
			LastCraftedBadge = DataManager.Instance.Player.LastCraftedBadge;
		}

		public void Reset()
		{
			SelectedBadge = null;
			if (BadgeModels.Count > 0)
				OnClickDelete();
		}

		public void AutoDownGrade(UIToggle tg)
		{
			IsAutoComponents = tg.value;
		}

		public void OnClickDelete()
		{
			bool IsDeleteAll = SelectedBadge == null;
			int index = 0;

			modelRandomCurrent = new ModelRandom(modelRandomInit);
			modelRandomLast = modelRandomCurrent;

			if (!IsDeleteAll)
			{
				index = BadgeModels.IndexOf(SelectedBadge);
				CurrentIndex = index;
				if (index > 0)
				{
					modelRandomCurrent = BadgeModels[index - 1].modelRandom;
					modelRandomLast = BadgeModels[index].modelRandom;
				}
				string text = "Residence.CraftButton.Text";
				dataManager.craftTab.SetContentToCraftButton(LocalizationManager.GetText(text));
			}
			else CurrentIndex = 1;

			CurrentState = modelRandomCurrent.State;
			CurrentInitialSeed = modelRandomCurrent.InitialSeed;
			CurrentCallCount = modelRandomCurrent.CallCount;

			var BadgesForDelete = BadgeModels.GetRange(index, BadgeModels.Count - index);

			for (int i = 0; i < BadgesForDelete.Count; i++)
			{
				var item = BadgesForDelete[i];
				var buttonToggle = item.GetComponent<UIButtonToggle>();
				toggleSet.Buttons.Remove(buttonToggle);
				BadgeModels.Remove(item);
				Destroy(item.gameObject);
			}

			table.Reposition();
			table.transform.parent.GetComponent<UIScrollView>().ResetPosition();

			if (BadgeModels.Count > 0)
			{
				LastCraftedBadge = BadgeModels.Last().badgeModel.GetData().Model;
			}
			else
			{
				LastCraftedBadge = DataManager.Instance.Player.LastCraftedBadge;
			}

			if (RerollMultiBadgePopup.Content.activeSelf)
			{
				RerollMultiCommand();
			}
		}

		public void OnClickCraft()
		{
			DebugTWD.Log("start crafting");

			bool IsCraftNew = SelectedBadge == null;

			if (!IsCraftNew)
			{
				modelRandomCurrent = SelectedBadge.modelRandom;
			}
			else
			{
				modelRandomCurrent = modelRandomLast;
			}

			CurrentState = modelRandomCurrent.State;
			CurrentInitialSeed = modelRandomCurrent.InitialSeed;
			CurrentCallCount = modelRandomCurrent.CallCount;

			StartCoroutine(CraftBadge(IsCraftNew));
		}

		public IEnumerator CraftBadge(bool IsCraftNew)
		{
			int count;
			if (!IsCraftNew)
			{
				CurrentIndex = SelectedBadge.badgeIndex;
				count = BadgeModels.Count - CurrentIndex;
			}
			else
			{
				count = (CurrentIndex + CraftRangeCount > MaxBadgeCounts) ? MaxBadgeCounts - CurrentIndex : CraftRangeCount;
			}

			for (int i = 0; i < count; i++)
			{
				ModelRandom currentRandom = new ModelRandom(modelRandomCurrent);

				bool IsRecalculate = !IsCraftNew && i > 0;
				bool IsAutoWithSelected = !IsCraftNew ? i == 0 : true;
				components = IsRecalculate ? BadgeModels[CurrentIndex].Currencies : dataManager.craftTab.currentSelection.ToList();

				string typeIndex = craftTools.CreateBonusTypeIndex(components.GetRange(1, 4));
				DebugTWD.Log("typeIndex " + typeIndex);

				if (IsAutoComponents && IsAutoWithSelected)
				{
					DebugTWD.Log("IsAutoComponents for " + (i+1).ToString());
					IsComponentsCalculated = false;

					StartCoroutine(CalculateRarity());

					yield return new WaitUntil(() => IsComponentsCalculated);
				}

				BadgeModel badgeModel = GenerateBadge(components, typeIndex, out List<string> logList, out BadgeLog badgeLog);

				if (badgeModel != null)
				{
					modelRandomLast = modelRandomCurrent;

					BadgeInfo badgeInfo = new BadgeInfo(badgeModel);
					var newIcon = IsCraftNew ? Instantiate(badgePrefab, badgePrefab.transform.parent) : BadgeModels[CurrentIndex].badgeModel;

					newIcon.SetData(badgeInfo);
					//int currentSlotIndex = badgeInfo.Model.SlotIndex;
					newIcon.UpdateUI();
					int effectValueCorrect = newIcon.EffectValue;
					newIcon.gameObject.SetActive(true);

					TweenManager.PlayTweenGroup(newIcon.gameObject, 2);

					var badgeItem = newIcon.GetComponent<BadgeGridItem>();
					badgeItem.SetBadgeComponents(components);
					badgeItem.badgeIndex = CurrentIndex;
					badgeItem.randomRateLable.text = badgeLog.Random.ToString();
					badgeItem.badgeModel = newIcon;

					logList[8] = "strengthForRarityDisplay : " + effectValueCorrect;
					badgeLog.Strength = effectValueCorrect;

					badgeItem.LogList = logList;
					badgeItem.badgeLog = badgeLog;

					var toolTip = badgeItem.toolTip;
					if (toolTip != null)
					{
						string toolTipeText = string.Empty;
						for (int j = 0; j < logList.Count - 2; j++)
						{
							if (j == logList.Count - 3) toolTipeText += logList[j].ToString();
							else toolTipeText += logList[j].ToString() + '\n';
						}
						toolTip.EnCustomText = toolTipeText;
					}

					badgeItem.modelRandom = currentRandom;
					badgeItem.ShowRandom();
					badgeItem.IndexLable.text = (CurrentIndex + 1).ToString();

					if (IsCraftNew || (!IsCraftNew && i == 0))
					{
						var label = badgeItem.RecipeLable;
						UpdateRecipeLabel(label, badgeModel.EffectId, typeIndex);
					}

					if (IsCraftNew)
					{
						BadgeModels.Add(badgeItem);

						table.Reposition();
						table.transform.parent.GetComponent<UIScrollView>().ResetPositionInverted();

						var buttonToggle = badgeItem.GetComponent<UIButtonToggle>();
						toggleSet.Buttons.Add(buttonToggle);
						buttonToggle.SetClickCallback(toggleSet.OnClick);
					}
					else
					{
						BadgeModels[CurrentIndex] = badgeItem;
					}

					LastCraftedBadge = badgeModel;

					CurrentIndex++;
				}
				yield return null;
			}
			DebugTWD.Log("craft successed ");

			if (RerollMultiBadgePopup.Content.activeSelf)
			{
				RerollMultiCommand();
			}
		}

		public static void UpdateRecipeLabel(UILabel label, string EffectID, string typeIndex)
		{
			DataManager dataManager = DataManager.Instance;

			if (typeIndex == "12" || typeIndex == "13" || typeIndex == "23" || typeIndex == "34")
			{
				HelpersUI.SetContentToLabel(label, dataManager.language == DataManager.Language.Ru ? "Прыжок" : "Jump");
			}
			else if (dataManager.craftTab.GetPlannedRecipeResult() == string.Empty)
			{
				HelpersUI.SetContentToLabel(label, dataManager.language == DataManager.Language.Ru ? "Шаг" : "Step");
			}
			else if (EffectID != dataManager.craftTab.GetPlannedRecipeResult())
			{
				HelpersUI.SetContentToLabel(label, LocalizationManager.Instance.GetLocalizedText("Error.Error"));
				//badgeItem.RecipeLable.text = "Error";
			}
			else
			{
				string recipe = LocalizationManager.Instance.GetLocalizedText("Popup.Residence.Crafting.RecipesButton");
				HelpersUI.SetContentToLabel(label, recipe.TrimEnd(recipe.Last()));
				//badgeItem.RecipeLable.text = "Recipe";
			}
		}

		public int ResidenceLevel()
		{
			if (CraftSettings.Instance.IsRealPlayerData)
			{
				string TypeName = "Residence";
				int TypeIndex = dataManager.ModelManager.CampModel.GetBuildingCount(TypeName);
				DebugTWD.Log(TypeIndex);
				string counter = "Counter." + TypeName + "." + TypeIndex + ".Level";
				return dataManager.ModelManager.Blackboard.GetCounter(counter);
			}
			else
			{
				return CraftSettings.Instance.ResidenceLevel;
			}

		}

		public BadgeModel GenerateBadge(List<CurrencyType> components, string typeIndex, out List<string> logList, out BadgeLog badgeLog, ModelRandom random = null)
		{
			logList = null;
			badgeLog = null;

			ModelRandom randomCurrent = random != null ? random : modelRandomCurrent;

			int InitState = randomCurrent.State;

			BadgeRarityResult badgeRarityResult = gameEconomyData.CalculateBadgeRarityResult(components);

			if (badgeRarityResult == null) return null;

			int analyticsId = CurrentBadgeAnalyticsId + 1;
			//int analyticsIdPrev = CurrentBadgeAnalyticsId;
			CurrentBadgeAnalyticsId = analyticsId;

			//string badgeRandomIdentifier = "BadgeRandom";

			//string HashID = PlayerHashID;
			//int seed = (int)ModelHelpers.MD5SumLong(HashID + badgeRandomIdentifier);

			//уровень сборщика (2)
			int level = IsCalculateMode ? residenceLevelCustom : dataManager.ResidenceLevel;
			//int level = dataManager.Player.Camp.GetBuilding("Residence")?.Level ?? 1;
			//int level = dataManager.settings.IsRealPlayerData ?
			//    (dataManager.Player.Camp.GetBuilding("Residence")?.Level ?? 1) : dataManager.settings.ResidenceLevel;
			//int level = ResidenceLevel();
			//int level = 2;

			//parametersView[0].text = "0 :" + modelRandomCurrent.State.ToString();

			//State change 1
			int badgeRarity = craftTools.GetBadgeRarity(badgeRarityResult, randomCurrent.GetRandomInRange(1, 100), out int maxRarity);
			//parametersView[1].text = "1 :" + modelRandomCurrent.State.ToString();

			//State change 2
			string effect = craftTools.GetEffect(components, randomCurrent, out int state);
			//parametersView[2].text = "2 :" + state.ToString();

			//parametersView[3].text = "3 :" + modelRandomCurrent.State.ToString();

			if (string.IsNullOrEmpty(effect)) return null;

			//State change 3
			int originRandomInRange = randomCurrent.GetRandomInRange(1, 100);
			//parametersView[4].text = "4 :" + modelRandomCurrent.State.ToString();

			int randomInRange = Math.Min(originRandomInRange + (maxRarity - badgeRarity) * 10, 100);

			//State change 4
			int randomInRange2 = randomCurrent.GetRandomInRange(0, 5);
			//parametersView[5].text = "5 :" + modelRandomCurrent.State.ToString();

			//State change 5
			BadgeType randomInRange3 = (BadgeType)randomCurrent.GetRandomInRange(0, 4);
			//parametersView[6].text = "6 :" + modelRandomCurrent.State.ToString();

			BadgeModel badgeModel = new BadgeModel(analyticsId, randomInRange2, badgeRarity, randomInRange3, effect, randomInRange, level);
			//string typeIndex = craftUtils.CreateBonusTypeIndex(components.GetRange(1, 4));

			//State change 6
			badgeModel.BonusId = randomCurrent.GetRandomElement(craftTools.CreateBadgeGatchaDeckOfIds(typeIndex, gameEconomyData.BadgeBonusDefinitions), false);
			BadgeBonusDefinition badgeBonusDefinition = gameEconomyData.GetBadgeBonusDefinition(badgeModel.BonusId);
			//parametersView[7].text = "7 :" + modelRandomCurrent.State.ToString();

			if (badgeBonusDefinition != null)
			{
				craftTools.CreateBonusCondition(badgeBonusDefinition, randomCurrent, ref badgeModel);
			}

			string badgeName = badgeModel.GenerateName();

			List<int> strengthForRarity = gameEconomyData.GetBadgeEffectDefinition(effect, level).GetStrengthForRarity(badgeRarity);

			DebugTWD.Log("strengthForRarity array: " + strengthForRarity[0] + ',' + strengthForRarity[1]);

			double strengthForRarityDisplay = Math.Round((float)strengthForRarity[0] + (float)(strengthForRarity[1] - strengthForRarity[0]) * ((float)randomInRange / 100f));
			DebugTWD.Log($"strengthForRarityDisplay formula: Math.Round((float){strengthForRarity[0]} + (float)({strengthForRarity[1]} - {strengthForRarity[0]}) * ((float){randomInRange} / 100f) ");
			DebugTWD.Log("strengthForRarityDisplay: " + strengthForRarityDisplay);
			DebugTWD.Log("badgeRarity: " + badgeRarity);
			DebugTWD.Log("randomInRange: " + randomInRange);

			//badgeModel.Strength = Mathf.RoundToInt((float)strengthForRarityDisplay);

			if (!IsCalculateMode)
			{
				logList = new List<string>()
				{
				"State : " + InitState.ToString(),
				"effect : " + effect, "badgeRarity : " + badgeRarity,
				"randomInRange : " + randomInRange,
				"Slot : " + randomInRange2, "Form : " + randomInRange3, "RecipeID : " + typeIndex,
				"Bonus : " + badgeModel?.BonusId ?? "null",  "strengthForRarityDisplay : " + strengthForRarityDisplay,
				"StateLast : " + randomCurrent.State, "ShortName : " + badgeName, string.Empty
				};

				badgeLog = new BadgeLog()
				{
					State = InitState,
					Effect = effect,
					Rarity = badgeRarity,
					Random = randomInRange,
					Slot = randomInRange2,
					Form = randomInRange3.ToString(),
					Recipe = typeIndex,
					Bonus = badgeModel?.BonusId ?? "null",
					Strength = Mathf.RoundToInt((float)strengthForRarityDisplay),
					Components = string.Join(" , ", components.ToArray()),
					ShortName = badgeName,
					StateLast = randomCurrent.State
				};
			}

			//
			//BadgeModel badgeModel = lootManagerModel.GenerateBadge(components);
			//badgeModel.Initialize();
			//badgeModel.SetManager(lootManagerModel.manager);
			//badgeModel.Start();
			//lootManagerModel.manager.Player.Equipment.AddBadge(badgeModel);
			//lootManagerModel.manager.Player.LastCraftedBadge = badgeModel;
			//lootManagerModel.manager.Player.NotifyChange("BadgeCreated");

			//lootManagerModel.manager.Metrics.ResetTdEvent();
			//lootManagerModel.manager.Metrics.AddFind().AddBadge(badgeModel).AddCrafting(CraftingType.Badge, textGuid).Send();
			//lootManagerModel.manager.Metrics.TdEventType = "Find_Badge_Crafting";
			//lootManagerModel.manager.Metrics.TdEventPropertyTypes = new List<string> { "Badge", "Crafting" };
			//lootManagerModel.manager.Metrics.SendTdEvent();
			//

			return badgeModel;
		}

		public void SaveLog()
		{
			if (BadgeModels.Count > 0)
			{
				bool IsJSON = true;
				string path;
				if (IsJSON)
				{
					List<BadgeLog> badgeLogList = new List<BadgeLog>();

					foreach (var badge in BadgeModels)
					{
						badgeLogList.Add(badge.badgeLog);
					}

					timeStampStatic = "_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm");
					timeStampStaticNet = DateTime.Now.ToString("yyyy.MM.dd HH:mm");

					if (OfflineManager.IsInternetOn)
					{
						//badgeLogList[0].Time = timeStampStaticNet;
						//SaveLogToSheet.Instance.BadgeLogList = badgeLogList;

						//var sheetID = DataManager.Instance.ContentSheetID;
						//string url = DataManager.SetContentUrl(sheetID, 50, addPrefix: false);

						//if (string.IsNullOrEmpty(url))
						//{
						//	string log = "SpreadSheet ID Field is empty";
						//	MyTools.UpdateLogPanel(log);
						//	DebugTWD.Log(log);
						//}
						//else
						//{
						//	SaveLogToSheet.Instance.SheetID = url;
						//	SaveLogToSheet.Instance.SaveLog();
						//}
					}
					else
					{
						path = DataManager.PlayerBadgesFolder + "BadgesCraft" + timeStampStatic + ".json";
						string json = OfflineManager.JsonSerializer.Serialize(badgeLogList);
						MyTools.SaveToFile(json, path, append: false);

						string log = "Badge Log BadgesCraft" + timeStampStatic + ".json saved to \n" + path;
						MyTools.UpdateLogPanel(log);
						DebugTWD.Log(log);
					}
				}
				else
				{
					foreach (var badge in BadgeModels)
					{
						LogList.AddRange(badge.LogList);
					}
					path = DataManager.PlayerBadgesFolder + "BadgesCraft" + timeStampStatic + ".txt";
					MyTools.SaveToFile(null, path, append: false, writeList: true, LogList);

					string log = "Badge Log BadgesCraft was saved locally to:\n" + path;
					MyTools.UpdateLogPanel(log);
					DebugTWD.Log(log);
				}
			}
		}

		//масштаб таблицы значков
		public void ScaleSetValue(UIScrollBar scrollBar)
		{
			float scale = scrollBar.value /2 + .5f;
			CraftSettings.Instance.SetScale(scale);
			CraftSettings.Instance.scaleLabel.text = Math.Round(scale * 100).ToString();

			ScaleGrid();
		}

		public void ScaleGrid()
		{
			if (table)
			{
				var tweenScale = table.gameObject.GetComponent<TweenScale>();
				tweenScale.to = Vector3.one * CraftSettings.Instance.Scale;
				tweenScale.PlayForward();
			}
		}

		public void RerollMulti()
		{
			IsShow = RerollMultiBadgePopup.Content.activeSelf;
			if (!IsShow)
			{
				RerollMultiBadgePopup.Content.SetActive(true);

				//RerollMultiBadgePopup.GetRerollType();

				if (rerolledBadgeOrigin == null)
				{
					rerolledBadgeOriginIcon.gameObject.SetActive(false);
					rerolledBadgeOriginBlank.SetActive(true);

					foreach (Transform trans in RerollMultiBadgePopup.BadgeTable.transform)
					{
						trans.gameObject.SetActive(false);
					}
				}
				else
				{
					rerolledBadgeOriginIcon.gameObject.SetActive(true);
					rerolledBadgeOriginBlank.SetActive(false);

					foreach (Transform trans in RerollMultiBadgePopup.BadgeTable.transform)
					{
						trans.gameObject.SetActive(true);
					}

					RerollMultiCommand();
				}

				RerollTarget.GetComponent<TweenAnchors>().PlayForward();
				BadgePanel.GetComponent<TweenAnchors>().PlayForward();

				RerollMultiBadgePopup.Content.GetComponent<TweenAlpha>().PlayForward();

				//TweenManager.PlayTweenGroup(RerollMultiBadgePopup.gameObject, 1, true, Show);
				//CraftingBadgeViewScroll.GetComponent<TweenPosition>().PlayForward();
			}
			else
			{
				RerollTarget.GetComponent<TweenAnchors>().PlayBackwards();
				BadgePanel.GetComponent<TweenAnchors>().PlayBackwards();

				RerollMultiBadgePopup.Content.GetComponent<TweenAlpha>().PlayReverse();

				//CraftingBadgeViewScroll.GetComponent<TweenPosition>().PlayReverse();
				//TweenManager.PlayTweenGroup(RerollMultiBadgePopup.gameObject, 1, false, Hide);
			}
		}

		public void RerollMultiCommand()
		{
			if (rerolledBadgeOrigin == null) return;

			DebugTWD.Log("reroll multi");

			BadgeInfo badgeInfo = new BadgeInfo(rerolledBadgeOrigin);
			rerolledBadgeOriginIcon.SetData(badgeInfo);
			rerolledBadgeOriginIcon.UpdateUI();

			ModelRandom dedicatedRandom = new ModelRandom(BadgeCraft.Instance.modelRandomLast);

			//RerollMultiBadgePopup.SetRerollType(rerollLast);
			GetOriginBadgeData();

			var badges = new List<BadgeModel>
			{
				RerollBadge(rerolledBadgeOrigin, rerollLast, dedicatedRandom)
			};

			for (int i = 1; i < 5; i++)
			{
				BadgeModel badge = RerollBadge(badges[i - 1], rerollLast, dedicatedRandom);
				badges.Add(badge);
			}
			RerollMultiBadgePopup.OpenMulti(badges);

			GetOriginBadgeData();
			//SetOriginBadgeData(rerolledBadgeOrigin);
		}

		public void Hide()
		{
			DebugTWD.Log("Hide quick reroll");
			//CraftingBadgeView.GetComponent<UIWidget>().UpdateAnchors();

			if (IsShow)
			{
				IsShow = false;
				RerollMultiBadgePopup.Content.SetActive(false);
			}
		}

		void Show()
		{
			DebugTWD.Log("Show quick reroll");
			IsShow = true;
			//CraftingBadgeView.GetComponent<UIWidget>().UpdateAnchors();
		}

		public BadgeModel RerollBadge(BadgeModel badgeToReroll, BadgeReroll reroll, ModelRandom random)
		{
			GameEconomyData data = DataManager.Instance.GameData;

			int analyticsId = ++DataManager.Instance.Player.LootManager.CurrentBadgeAnalyticsId;

			//UnityEngine.Debug.Log("random " + random.State);

			int num = badgeToReroll.SlotIndex;
			BadgeType badgeType = badgeToReroll.Type;
			int num2 = badgeToReroll.RerollsSlot;
			int num3 = badgeToReroll.RerollsSet;
			int num4 = badgeToReroll.RerollsBonus;
			switch (reroll)
			{
				case BadgeReroll.Slot:
					num2++;

					if (IsUseRerollPool)
					{
						badgeToReroll.AddSlotToHistory(num);
						while (badgeToReroll.HistorySlots.Contains(num))
						{
							num = random.GetRandomInRange(0, 5);
						}
					}
					else
					{
						num = random.GetRandomInRange(0, 5);
					}

					break;
				case BadgeReroll.Set:
					num3++;

					if (IsUseRerollPool)
					{
						badgeToReroll.AddSetToHistory(badgeType);
						while (badgeToReroll.HistorySet.Contains(badgeType))
						{
							badgeType = (BadgeType)random.GetRandomInRange(0, 4);
						}
					}
					else
					{
						badgeType = (BadgeType)random.GetRandomInRange(0, 4);
					}

					break;
			}
			BadgeModel badgeModel = new BadgeModel(analyticsId, num, badgeToReroll.Rarity, badgeType, badgeToReroll.EffectId, badgeToReroll.EffectRoll, badgeToReroll.Level);
			if (reroll == BadgeReroll.Bonus)
			{
				if (badgeToReroll.BonusId == "Constant")
				{
					return null;
				}
				num4++;
				List<string> list = (from x in data.BadgeBonusDefinitions where x.ID != "Constant" select x.ID).ToList();

				if (IsUseRerollPool)
				{
					badgeToReroll.AddBonusToHistory();
				}
				string id = badgeModel.BonusId = random.GetRandomElement(list, remove: false);

				BadgeBonusDefinition badgeBonusDefinition = data.GetBadgeBonusDefinition(id);
				CreateBonusCondition(badgeBonusDefinition, random, ref badgeModel);
				if (IsUseRerollPool)
				{
					while (badgeToReroll.BonusHistoryContain(badgeModel))
					{
						id = badgeModel.BonusId = random.GetRandomElement(list, remove: false);
						badgeBonusDefinition = data.GetBadgeBonusDefinition(id);
						CreateBonusCondition(badgeBonusDefinition, random, ref badgeModel);
					}
				}
			}
			else
			{
				BadgeBonusDefinition badgeBonusDefinition2 = data.GetBadgeBonusDefinition(badgeToReroll.BonusId);
				badgeModel.BonusId = badgeToReroll.BonusId;
				if (badgeBonusDefinition2 != null)
				{
					CreateCopyOfBonusCondition(badgeBonusDefinition2, ref badgeModel, badgeToReroll);
				}
			}
			badgeModel.RerollsSlot = num2;
			badgeModel.RerollsSet = num3;
			badgeModel.RerollsBonus = num4;

			badgeModel.HistorySlots = badgeToReroll.HistorySlots;
			badgeModel.HistorySet = badgeToReroll.HistorySet;
			badgeModel.HistoryBonus = badgeToReroll.HistoryBonus;
			return badgeModel;
		}

		private void CreateBonusCondition(BadgeBonusDefinition bonusDef, ModelRandom random, ref BadgeModel badgeModel)
		{
			Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), bonusDef.ConditionClassName);
			if (!string.IsNullOrEmpty(bonusDef.ConditionClassName) && type == null)
			{
				DebugTWD.LogError("Failed to instantiate condition class " + bonusDef.ConditionClassName);
			}
			List<string> list = new List<string> { bonusDef.ConstructionParameters[0] };
			if (bonusDef.ConstructionParameters.Count > 1)
			{
				list.Add(random.GetRandomElement(bonusDef.ConstructionParameters.GetRange(1, bonusDef.ConstructionParameters.Count - 1), remove: false));
			}
			badgeModel.BonusCondition = (type != null) ? (ReflectionUtils.Instantiate(type, list) as BaseBonusCondition) : null;
			badgeModel.BonusParameters = list;
		}

		private void CreateCopyOfBonusCondition(BadgeBonusDefinition bonusDef, ref BadgeModel badgeModel, BadgeModel oldBadgeModel)
		{
			Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), bonusDef.ConditionClassName);
			if (!string.IsNullOrEmpty(bonusDef.ConditionClassName) && type == null)
			{
				DebugTWD.LogError("Failed to instantiate condition class " + bonusDef.ConditionClassName);
			}
			badgeModel.BonusCondition = (type != null) ? (ReflectionUtils.Instantiate(type, oldBadgeModel.BonusParameters) as BaseBonusCondition) : null;
			badgeModel.BonusParameters = oldBadgeModel.BonusParameters;
		}

		//сохранить исходные параметры значка
		public void SetOriginBadgeData(BadgeModel model)
		{
			rerolledBadgeOrigin = model;

			originHistoryBonus = null;
			originHistorySet = null;
			originHistorySlots = null;

			if (model.HistoryBonus != null && model.HistoryBonus.Count > 0)
			{
				originHistoryBonus = new List<(string BonusId, List<string> BonusParameters)>();

				foreach (var bonus in model.HistoryBonus)
				{
					string bonusID = bonus.BonusId;
					List<string> bonusParams = new List<string>();
					if (bonus.BonusParameters != null)
					{
						foreach (var bonusParam in bonus.BonusParameters)
						{
							bonusParams.Add(bonusParam);
						}
					}
					originHistoryBonus.Add((bonusID, bonusParams));
				}
				DebugTWD.Log("save history bonus. originHistoryBonus.count : " + originHistoryBonus.Count);
			}
			if (model.HistorySet != null && model.HistorySet.Count > 0)
			{
				originHistorySet = new List<BadgeType>();

				originHistorySet.AddRange(model.HistorySet);
			}
			if (model.HistorySlots != null && model.HistorySlots.Count > 0)
			{
				originHistorySlots = new List<int>();

				originHistorySlots.AddRange(model.HistorySlots);
			}
		}

		//вернуть сохраненные параметры для значка, который в рероле
		public void GetOriginBadgeData()
		{
			if (originHistoryBonus != null && originHistoryBonus.Count > 0)
			{
				if (rerolledBadgeOrigin.HistoryBonus != null)
				{
					rerolledBadgeOrigin.HistoryBonus.Clear();
					foreach (var bonus in originHistoryBonus)
					{
						string bonusID = bonus.BonusId;
						List<string> bonusParams = new List<string>();
						if (bonus.BonusParameters != null)
						{
							foreach (var bonusParam in bonus.BonusParameters)
							{
								bonusParams.Add(bonusParam);
							}
						}
						rerolledBadgeOrigin.HistoryBonus.Add((bonusID, bonusParams));
					}
					DebugTWD.Log("Return history bonus. originHistoryBonus.count : " + originHistoryBonus.Count);
				}
			}
			else rerolledBadgeOrigin.HistoryBonus = null;

			if (originHistorySet != null && originHistorySet.Count > 0)
			{
				if (rerolledBadgeOrigin.HistorySet != null)
				{
					rerolledBadgeOrigin.HistorySet.Clear();
					rerolledBadgeOrigin.HistorySet.AddRange(originHistorySet);
				}
			}
			else rerolledBadgeOrigin.HistorySet = null;
			if (originHistorySlots != null && originHistorySlots.Count > 0)
			{
				if (rerolledBadgeOrigin.HistorySlots != null)
				{
					rerolledBadgeOrigin.HistorySlots.Clear();
					rerolledBadgeOrigin.HistorySlots.AddRange(originHistorySlots);
				}
			}
			else rerolledBadgeOrigin.HistorySlots = null;

			rerolledBadgeOriginIcon.UpdateUI();
		}

		public IEnumerator CalculateRarity()
		{
			newComponents = new List<CurrencyType>();
			newComponents.AddRange(components);

			BadgeRarityResult badgeRarityResult = gameEconomyData.CalculateBadgeRarityResult(components);

			int testRandomValue = GetRandomInRange(1, 100, modelRandomCurrent.State);

			int badgeRarity = craftTools.GetBadgeRarity(badgeRarityResult, testRandomValue, out int maxRarity);

			if (badgeRarity < maxRarity)
			{
				IsComponentsCalculated = true;
				yield break;
			}

			int rarity = maxRarity;
			for (int i = 0; i < 4; i++)
			{
				rarity--;
				for (int j = 4; j >= 0; j--)
				{
					newComponents[j] = ComponentHelper.GetCurrencyFromBaseAndRarity(components[j], rarity);

					badgeRarityResult = gameEconomyData.CalculateBadgeRarityResult(newComponents);
					testRandomValue = GetRandomInRange(1, 100, modelRandomCurrent.State);
					badgeRarity = craftTools.GetBadgeRarity(badgeRarityResult, testRandomValue, out int testRarity);

					if (badgeRarity < testRarity)
					{
						IsComponentsCalculated = true;

						yield break;
					}
					components[j] = newComponents[j];
				}
				yield return null;
			}

			IsComponentsCalculated = true;
		}

		public int GetRandomInRange(int min, int max, int State)
		{
			return Next(max - min + 1, State) + min;
		}

		public int Next(int n, int State)
		{
			State = (State * 1103515245 + 12345) & 0x7FFFFFFF;
			return (State >> 4) % n;
		}

		public void CalculateState()
		{
			residenceLevelCustom = 2;
			testComponents = DefaultComponents();

			IsStopCycle = false;

			testBadge1 = new BadgeModel(0, slotIndex : 1, rarity : 4, type : BadgeType.Rugged, effectId : "Damage", 0, 0);
			testBadge1.BonusId = "Constant";
			testBadge1.Strength = 20;

			testBadge2 = new BadgeModel(0, slotIndex: 4, rarity: 4, type: BadgeType.Bold, effectId: "Damage", 0, 0);
			testBadge2.BonusId = "Constant";
			testBadge2.Strength = 19;

			testBadge3 = new BadgeModel(0, slotIndex: 2, rarity: 4, type: BadgeType.Jagged, effectId: "Damage", 0, 0);
			testBadge3.BonusId = "Constant";
			testBadge3.Strength = 19;

			stateCalculateList1 = new List<int>();
			stateCalculateList2 = new List<int>();
			stateCalculateList3 = new List<int>();

			comparesResult1 = 0;
			comparesResult2 = 0;
			comparesResult3 = 0;

			IsCalculateMode = true;

			StartCoroutine(CalculateStateC());
		}

		public void StopCalculate()
		{
			IsStopCycle = true;
		}

		private bool CompareBadges(BadgeModel testBadge, BadgeModel currentBadge)
		{
			if (testBadge.Strength == currentBadge.Strength &&
				testBadge.SlotIndex == currentBadge.SlotIndex &&
				testBadge.BonusId == currentBadge.BonusId &&
				testBadge.EffectId == currentBadge.EffectId &&
				testBadge.Type == currentBadge.Type &&
				testBadge.Rarity == currentBadge.Rarity
				) return true;
			else return false;
		}

		private IEnumerator CalculateStateC()
		{
			if (DataManager.Instance.GameData == null)
			{
				DataManager.Instance.LoadGameEconomyData();
				yield return new WaitUntil(() => OfflineManager.Instance.IsGedLoaded);

				gameEconomyData = DataManager.Instance.GameData;
				craftTools = new CraftTools()
				{
					GameData = gameEconomyData
				};
			}

			string typeIndex = craftTools.CreateBonusTypeIndex(testComponents.GetRange(1, 4));

			float startTime = Time.realtimeSinceStartup;
			ModelRandom random = new ModelRandom(startState);

			int incremental = 0;

			for (int i = startState; i < endState; i++)
			{
				if (IsStopCycle)
				{
					IsStopCycle = false;
					ShowTime(startTime);
					//StartCoroutine(CalculateStateCIteration1());

					yield break;

				}

				random.State = i;

				BadgeModel currentBadge = GenerateBadge(testComponents, typeIndex, out List<string> logList, out BadgeLog badgeLog, random);

				incremental++;

				if (currentBadge == null) continue;

				FixedPoint increment = currentBadge.Increment;

				if (currentBadge.BonusCondition is ConstantBonusCondition constantBonusCondition)
				{
					increment += FixedPoint.Max(1L, FixedPoint.Round(increment * (constantBonusCondition.BonusValue / 100.0)));
				}
				currentBadge.Strength = (int)increment.UIRounding();

				if (CompareBadges(testBadge1, currentBadge) == true)
				{
					comparesResult1++;

					BadgeModel currentBadge2 = GenerateBadge(testComponents, typeIndex, out List<string> logList2, out BadgeLog badgeLog2, random);

					increment = currentBadge2.Increment;

					if (currentBadge2.BonusCondition is ConstantBonusCondition constantBonusCondition2)
					{
						increment += FixedPoint.Max(1L, FixedPoint.Round(increment * (constantBonusCondition2.BonusValue / 100.0)));
					}
					currentBadge2.Strength = (int)increment.UIRounding();

					if (CompareBadges(testBadge2, currentBadge2) == true)
					{
						comparesResult2++;

						BadgeModel currentBadge3 = GenerateBadge(testComponents, typeIndex, out List<string> logList3, out BadgeLog badgeLog3, random);

						increment = currentBadge3.Increment;

						if (currentBadge3.BonusCondition is ConstantBonusCondition constantBonusCondition3)
						{
							increment += FixedPoint.Max(1L, FixedPoint.Round(increment * (constantBonusCondition3.BonusValue / 100.0)));
						}
						currentBadge3.Strength = (int)increment.UIRounding();

						if (CompareBadges(testBadge3, currentBadge3) == true)
						{
							stateCalculateList3.Add(i);
							comparesResult3++;
						}
					}
				}

				if (incremental > cyclePartAmount)
				{
					incremental = 0;
					float pers = (i - startState) / (float)(endState - startState);
					slider.value = pers;
					yield return null;
				}
			}

			yield return null;

			ShowTime(startTime);
			//StartCoroutine(CalculateStateCIteration1());
		}

		private void ShowTime(float startTime)
		{
			cycleTime = Time.realtimeSinceStartup - startTime;
			TimeSpan timespan = TimeSpan.FromSeconds(cycleTime);
			DebugTWD.Log("timespan : " + timespan.ToString("hh':'mm':'ss"));
			IsCalculateMode = false;
		}
	}

	public class BadgeLog
	{
		public int State { get; set; }
		public string Effect { get; set; }
		public int Rarity { get; set; }
		public int Random { get; set; }
		public int Slot { get; set; }
		public string Form { get; set; }
		public string Recipe { get; set; }
		public string Bonus {  get; set; }
		public int Strength { get; set; }
		public string ShortName { get; set; }
		public string Components { get; set; }
		public int StateLast { get; set; }
		public string Time { get; set; }

	}
}

