using System;
using System.Linq;
using TWDModel;
using UnityEngine;

public class TeamSelectSupportsView : MonoBehaviour
{
	[SerializeField]
	private SmallSupportCard[] supportCards;

	[SerializeField]
	private GameObject[] emptySupportCards;

	private SupportSelectionPanel supportSelectionPanel;

	private PlayerModel playerModel;

	private int lastTeamSize;

	private bool supportsAreFixed;

	private MapMissionModel currentMapMissionModel;

	private void Initialize()
	{
		playerModel = GameManager.Instance.playerModel;
		supportSelectionPanel = GetComponentInParent<TeamSelectionPopup>().SupportSelectionPanel;
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "ReloadSurvivorList")
		{
			Refresh(lastTeamSize);
		}
	}

	public void OnPopupActive(int teamSize, bool isDefenderSelection, MapMissionModel mapMissionModel)
	{
		if (playerModel == null)
		{
			Initialize();
		}
		currentMapMissionModel = mapMissionModel;
		supportsAreFixed = SupportHelpers.AreSupportsFixed(mapMissionModel);
		base.gameObject.SetActive(!isDefenderSelection && (supportsAreFixed || playerModel.SupportModels.Any((SupportModel model) => model.Unlocked)));
		Refresh(teamSize);
		supportSelectionPanel.HideImmediate();
	}

	private void Refresh(int teamSize)
	{
		lastTeamSize = teamSize;
		MapCategory mapCategory = (currentMapMissionModel?.MissionSpawnPointGroup?.Category).GetValueOrDefault();
		for (int i = 0; i < supportCards.Length; i++)
		{
			int index = i;
			SupportModel supportModel = SupportHelpers.GetMissionSupport(currentMapMissionModel, playerModel, i);
			bool flag = i < teamSize || supportsAreFixed;
			SmallSupportCard card = supportCards[i];
			Action action = delegate
			{
				OnRemoveClick(card, index);
			};
			card.Initialize(flag ? supportModel : null, delegate
			{
				OnSupportClick(index);
			}, delegate
			{
				((SupportDetailsPopup)HUDManager.TryOpenPopup(UIType.SupportDetailsPopup)).Show(supportModel, !supportsAreFixed, delegate
				{
					card.Refresh();
				}, mapCategory != MapCategory.Endless);
			}, supportsAreFixed ? null : action, mapCategory);
			emptySupportCards[i].SetActive(flag && !supportsAreFixed);
		}
	}

	private void OnSupportClick(int index)
	{
		if (!supportsAreFixed)
		{
			supportSelectionPanel.Show(index, playerModel.EquippedSupportIds, (currentMapMissionModel?.MissionSpawnPointGroup?.Category).GetValueOrDefault(), delegate(SupportModel support, int i)
			{
				Helpers.ExecuteCommand(new EquipSupportCommand(i, support.SupportId));
				Refresh(lastTeamSize);
				UIEvent.Send("NewSupportEquipped");
			});
		}
	}

	private void OnRemoveClick(SupportCard card, int index)
	{
		if (Helpers.ExecuteCommand(new EquipSupportCommand(index, string.Empty)) == TWDModelResult.OK)
		{
			card.SetItem(null);
			UIEvent.Send("NewSupportEquipped");
		}
	}
}
