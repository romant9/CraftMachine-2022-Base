using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ScavengePopup : HUDElement
{
	[SerializeField]
	private UILabel titleSmall;

	[SerializeField]
	private UILabel titleBig;

	[SerializeField]
	private ScavengeListBase contentParent;

	[SerializeField]
	private UIButtonToggleSet toggleSet;

	[SerializeField]
	private GameObject hardModeOverlay;

	[Header("Tweens Configuration")]
	[SerializeField]
	private int normalTweenGroup = 10;

	[SerializeField]
	private int hardTweenGroup = 20;

	public static string DifficultyPlayerPrefs = "DifficultyPlayerPrefs";

	private Dictionary<UIButtonToggle, GrindButtonDefinition.Difficulty> toggleToEnumDict;

	private GrindButtonDefinition.Difficulty currentDifficultyMode;

	private GrindButtonDefinition.Difficulty[] tabsToDifficultyMapping = new GrindButtonDefinition.Difficulty[2]
	{
		GrindButtonDefinition.Difficulty.Normal,
		GrindButtonDefinition.Difficulty.Hard
	};

	private bool firstUpdate = true;

	public override void Open()
	{
		base.Open();
		UITypeOpenOnClose = UIType.MissionHubPopup;
		currentDifficultyMode = GetSavedDifficultyState();
		if (toggleSet != null && toggleSet.GetUIButtonToggleList != null)
		{
			toggleSet.SetChangeCallback(OnToggleChange);
			if (toggleSet != null && tabsToDifficultyMapping != null)
			{
				int num = Array.IndexOf(tabsToDifficultyMapping, currentDifficultyMode);
				if (num > -1)
				{
					toggleSet.SetInitialToggle(num);
				}
			}
		}
		InitAllButtonPrefabs();
		CampView.Instance.Hud.UpdateGenericElementsAfterChange();
	}

	public override void OnClickClose()
	{
		base.OnClickClose();
	}

	public GrindButtonDefinition.Difficulty GetSavedDifficultyState()
	{
		if (TWDPlayerPrefs.HasKey(DifficultyPlayerPrefs))
		{
			try
			{
				return (GrindButtonDefinition.Difficulty)Enum.Parse(typeof(GrindButtonDefinition.Difficulty), TWDPlayerPrefs.GetString(DifficultyPlayerPrefs));
			}
			catch (Exception)
			{
			}
		}
		else if (GameManager.Instance.playerModel.MapContainerModel != null && GameManager.Instance.playerModel.MapContainerModel.CurrentGrindMissionModel != null)
		{
			GrindButtonDefinition grindButtonDefinition = GameManager.Instance.playerModel.gameEconomyData.GetGrindButtonDefinition(GameManager.Instance.playerModel.MapContainerModel.CurrentGrindMissionModel.GrindButtonDefinitionId);
			if (grindButtonDefinition != null)
			{
				return grindButtonDefinition.GrindDifficulty;
			}
		}
		return GrindButtonDefinition.Difficulty.Normal;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (hardModeOverlay != null)
		{
			int num = -1;
			if (currentDifficultyMode == GrindButtonDefinition.Difficulty.Normal)
			{
				num = normalTweenGroup;
			}
			else if (currentDifficultyMode == GrindButtonDefinition.Difficulty.Hard)
			{
				num = hardTweenGroup;
			}
			if (firstUpdate)
			{
				firstUpdate = false;
				TweenManager.PlayTweenGroup(hardModeOverlay, num, forward: true, null, resetToEnd: true);
			}
			else
			{
				TweenManager.PlayTweenGroup(hardModeOverlay, num);
			}
		}
	}

	public virtual void Clear()
	{
		if (contentParent != null)
		{
			contentParent.Clear();
		}
		if (toggleSet != null)
		{
			toggleSet.Clear();
		}
		if (toggleToEnumDict != null)
		{
			toggleToEnumDict = new Dictionary<UIButtonToggle, GrindButtonDefinition.Difficulty>();
		}
	}

	private void InitAllButtonPrefabs()
	{
		string text = "";
		GrindButtonDefinition[] grindButtonDefinitions = GameManager.Instance.gameEconomyData.GrindButtonDefinitions;
		if (grindButtonDefinitions != null)
		{
			Vector3 vector = Vector3.zero;
			ScavengeMissionButton scavengeMissionButton = null;
			if (!(contentParent != null))
			{
				return;
			}
			contentParent.Clear();
			for (int i = 0; i < grindButtonDefinitions.Length; i++)
			{
				if (grindButtonDefinitions[i] == null || grindButtonDefinitions[i].GrindDifficulty != GrindButtonDefinition.Difficulty.Normal || string.IsNullOrEmpty(grindButtonDefinitions[i].PrefabName))
				{
					continue;
				}
				text = grindButtonDefinitions[i].PrefabName;
				GameObject gameObject = UnityUtils.LoadFromAssetBundle<GameObject>(text, HUDElementConfig.BundleName);
				if (gameObject != null)
				{
					scavengeMissionButton = contentParent.InstantiateButton(gameObject, this);
					if (scavengeMissionButton != null)
					{
						vector = scavengeMissionButton.GetLocalSize();
					}
				}
				else
				{
					DebugLogError("Could not load prefab at assetPath: " + text);
				}
			}
			contentParent.SetGridSize(vector.x, vector.y);
			contentParent.RepositionNow();
			Helpers.GameObjectSetActive(contentParent, value: true);
		}
		else
		{
			DebugLogError("Can not InitAllButtons, GrindButtonDefinitions was NULL!");
		}
	}

	private void UpdateButtons()
	{
		Helpers.GameObjectSetActive(contentParent, value: false);
		GrindButtonDefinition[] grindButtonDefinitions = GameManager.Instance.gameEconomyData.GrindButtonDefinitions;
		List<GrindButtonDefinition> list = new List<GrindButtonDefinition>();
		if (grindButtonDefinitions != null && contentParent.GetButtonList() != null)
		{
			for (int i = 0; i < grindButtonDefinitions.Length; i++)
			{
				if (grindButtonDefinitions[i] != null && grindButtonDefinitions[i].GrindDifficulty == currentDifficultyMode)
				{
					list.Add(grindButtonDefinitions[i]);
				}
			}
			for (int j = 0; j < contentParent.GetButtonList().Count; j++)
			{
				if (contentParent.GetButtonList()[j] != null)
				{
					if (list.Count > j && list[j] != null)
					{
						Helpers.GameObjectSetActive(contentParent.GetButtonList()[j], value: true);
						contentParent.GetButtonList()[j].Init(list[j], this, OnClickCallback);
					}
					else
					{
						Helpers.GameObjectSetActive(contentParent.GetButtonList()[j], value: false);
					}
				}
			}
			contentParent.RepositionNow();
			contentParent.UpdateUI();
		}
		Helpers.GameObjectSetActive(contentParent, value: true);
	}

	private void OnToggleChange(UIButtonExtended toggleButton)
	{
		GrindButtonDefinition.Difficulty difficulty = GrindButtonDefinition.Difficulty.None;
		if (toggleButton != null && tabsToDifficultyMapping != null && !string.IsNullOrEmpty(toggleButton.id))
		{
			int result = 0;
			if (int.TryParse(toggleButton.id, out result) && result >= 0 && result < tabsToDifficultyMapping.Length && Enum.IsDefined(typeof(GrindButtonDefinition.Difficulty), tabsToDifficultyMapping[result]) && tabsToDifficultyMapping[result] != GrindButtonDefinition.Difficulty.None && tabsToDifficultyMapping[result] != GrindButtonDefinition.Difficulty.Count)
			{
				difficulty = tabsToDifficultyMapping[result];
			}
		}
		if ((difficulty != GrindButtonDefinition.Difficulty.None && firstUpdate) || currentDifficultyMode != difficulty)
		{
			currentDifficultyMode = difficulty;
			TWDPlayerPrefs.SetString(DifficultyPlayerPrefs, currentDifficultyMode.ToString());
			UpdateUI();
			UpdateButtons();
		}
	}

	private void OnClickCallback(UIButtonExtended button)
	{
		if (button != null && button.GetComponent<ScavengeMissionButton>() != null)
		{
			PlaySelectedGrind(button.GetComponent<ScavengeMissionButton>().GetDefinition());
		}
	}

	private void PlaySelectedGrind(GrindButtonDefinition definition)
	{
		if (definition != null)
		{
			Helpers.ExecuteCommand(new SpawnGrindMissionCommand
			{
				GrindButtonDefinitionId = definition.Id
			});
			TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
			obj.SurvivorType = SurvivorContainerModel.SurvivorType.Combat;
			obj.SetUITypeOpenOnClose(UIType.ScavengePopup);
			obj.OpenForModel(GameManager.Instance.playerModel.MapContainerModel.CurrentGrindMissionModel);
			EventManager.NotifyClick("SelectTeam");
		}
	}
}
