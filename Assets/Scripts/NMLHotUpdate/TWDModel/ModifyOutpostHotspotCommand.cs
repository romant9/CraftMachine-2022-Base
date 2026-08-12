using BaseModel;

namespace TWDModel
{
	public class ModifyOutpostHotspotCommand : ModelCommand
	{
		public string SliceViewId { get; set; }

		public string HotspotViewId { get; set; }

		public HotspotState State { get; set; }

		public WalkerType WalkerType { get; set; }

		public int Count { get; set; }

		public AIMode DefensiveMode { get; set; }

		public ModifyOutpostHotspotCommand()
		{
		}

		public ModifyOutpostHotspotCommand(string sliceViewId, string hotspotViewId, HotspotState state, WalkerType walkerType = WalkerType.WalkerNormal, int count = 0, AIMode defensiveMode = AIMode.Aggressive)
		{
			SliceViewId = sliceViewId;
			HotspotViewId = hotspotViewId;
			State = state;
			WalkerType = walkerType;
			Count = count;
			DefensiveMode = defensiveMode;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				OutpostLevelModel editLevelModel = tWDModelManager.Player.OutpostModel.EditLevelModel;
				HotspotState state = State;
				if (editLevelModel != null)
				{
					if (State != HotspotState.None)
					{
						int num = editLevelModel.CanAffordHotspotModifiaction(SliceViewId, HotspotViewId, state, WalkerType, Count);
						if (num > 0 || (editLevelModel.HasExceededTotalType(tWDModelManager.Player.OutpostModel, WalkerType) && State != HotspotState.DefenderSpawn_0 && State != HotspotState.DefenderSpawn_1 && State != HotspotState.DefenderSpawn_2))
						{
							tWDModelManager.Debug.LogError("Player can afford to deploy hotspot. Missing deployment points: " + num);
							return new NGModelCommandRespond(this, TWDModelResult.AlreadyMaxAmount);
						}
					}
					bool num2 = (state != HotspotState.Flag || !editLevelModel.HasFlag) && (state != HotspotState.ResourceContainer || !editLevelModel.HasResourceContainer);
					bool num3 = state == HotspotState.DefenderSpawn_0 || state == HotspotState.DefenderSpawn_1 || state == HotspotState.DefenderSpawn_2;
					HotspotInfo hotspotInfo = editLevelModel.FindHotspotInfo(HotspotViewId);
					if (num3 && editLevelModel.HasDefender(state))
					{
						HotspotInfo hotspotInfoForDefender = editLevelModel.GetHotspotInfoForDefender(state);
						if (hotspotInfo != null)
						{
							HotspotState hotspotState = HotspotState.None;
							hotspotState = ((!hotspotInfo.IsDefenderSpawn) ? editLevelModel.GetFirstFreeDefenderState() : hotspotInfo.State);
							hotspotInfoForDefender.State = hotspotState;
						}
					}
					if (num2)
					{
						editLevelModel.SetHotspotInfo(SliceViewId, HotspotViewId, state, WalkerType, Count, DefensiveMode);
					}
				}
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
