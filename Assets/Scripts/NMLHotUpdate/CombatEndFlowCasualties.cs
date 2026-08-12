using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CombatEndFlowCasualties : CombatEndFlowStep
{
	[Tooltip("Survivor card prefab")]
	[SerializeField]
	private GameObject survivorCardPrefab;

	[Tooltip("Grid to place the casualties cards")]
	[SerializeField]
	private GameObject casualtiesContainerGrid;

	[Tooltip("Casualties title label.")]
	[SerializeField]
	private UILabel casualtiesTitle;

	[Tooltip("Distance between each survivor card")]
	[SerializeField]
	private float survivorCardContainerOffset;

	private List<GameObject> survivorCards = new List<GameObject>();

	private List<SurvivorModel> survivorModels;

	private int numberSurvivors;

	private int numberAnimationOver;

	public bool EndWithSurvivorBoxCentered { get; set; }

	public bool AnimationOver => numberAnimationOver >= numberSurvivors;

	public CombatEndFlowCasualties()
	{
		DestroyAfterCompletion = false;
		EndWithSurvivorBoxCentered = false;
		base.ReturnToCampAllowed = false;
	}

	private MissionSpawnPointGroup SolveMapForMissionId(string missionId)
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		for (int i = 0; i < gameEconomyData.MissionDefinitions.Length; i++)
		{
			MissionSpawnPoint missionSpawnPoint = gameEconomyData.MissionDefinitions[i];
			if (missionSpawnPoint != null && missionSpawnPoint.MissionId == missionId)
			{
				return missionSpawnPoint.OwningGroup;
			}
		}
		return null;
	}

	private bool WasLastMissionSurvival()
	{
		if (GameManager.Instance.playerModel.Combat != null)
		{
			return GameManager.Instance.playerModel.Combat.IsSurvivalMission;
		}
		MissionData missionData = GameManager.Instance.gameEconomyData.GetMissionData(GameManager.Instance.playerModel.LastCompletedMissionId);
		if (missionData == null)
		{
			Debug.LogWarning("Failed to solve mission data for last selected mission id.");
			return false;
		}
		MissionSpawnPointGroup missionSpawnPointGroup = SolveMapForMissionId(missionData.Id);
		if (missionSpawnPointGroup == null)
		{
			Debug.LogWarning("Failed to solve map id for selected mission id.");
			return false;
		}
		return missionSpawnPointGroup.Category == MapCategory.Survival;
	}

	public override void StartFlow()
	{
		base.StartFlow();
		numberAnimationOver = 0;
		numberSurvivors = 0;
		bool isSurvivalEndScreen = WasLastMissionSurvival();
		foreach (SurvivorModel survivorModel in survivorModels)
		{
			if (GameManager.Instance.playerModel.SurvivorContainer.ContainsSurvivor(survivorModel) || survivorModel.IsDead)
			{
				numberSurvivors++;
				GameObject gameObject = Object.Instantiate(survivorCardPrefab);
				gameObject.transform.parent = base.gameObject.transform;
				survivorCards.Add(gameObject);
				SurvivorCard component = gameObject.GetComponent<SurvivorCard>();
				component.EnableCardFlipping(enable: false);
				component.Item = survivorModel;
				component.UpdateUIForEndScreenStatus(survivorAnimOver, isSpecialCharacter: false, isSurvivalEndScreen);
			}
		}
		UnityUtils.AlignItemsInsideContainerLine(survivorCards, casualtiesContainerGrid, survivorCardContainerOffset, addToContainer: true, 1f);
	}

	private void survivorAnimOver()
	{
		numberAnimationOver++;
		if (AnimationOver)
		{
			AnimationEnded();
		}
	}

	public void SetupCasualties(List<SurvivorModel> deployedTeam)
	{
		survivorModels = deployedTeam;
		foreach (GameObject survivorCard in survivorCards)
		{
			Object.Destroy(survivorCard);
		}
		survivorCards.Clear();
	}

	public override void ForceFlowEnd()
	{
		base.ForceFlowEnd();
		casualtiesTitle.gameObject.SetActive(EndWithSurvivorBoxCentered);
	}

	public override void Update()
	{
		base.Update();
		if (Input.GetMouseButtonUp(0))
		{
			if (AnimationOver)
			{
				ForceFlowEnd();
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}
}
