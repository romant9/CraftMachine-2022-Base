using BaseModel;

namespace TWDModel
{
	public class SetOutpostSliceCommand : ModelCommand
	{
		public bool ClearPrevious { get; private set; }

		public string SliceViewId { get; private set; }

		public SlicePosition SlicePosition { get; private set; }

		public SetOutpostSliceCommand()
		{
		}

		public SetOutpostSliceCommand(SlicePosition slicePosition, string sliceViewId, bool clearPrevious)
		{
			ClearPrevious = clearPrevious;
			SliceViewId = sliceViewId;
			SlicePosition = slicePosition;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				OutpostLevelModel editLevelModel = tWDModelManager.Player.OutpostModel.EditLevelModel;
				if (editLevelModel != null)
				{
					if (ClearPrevious)
					{
						string chosenSliceViewId = editLevelModel.GetChosenSliceViewId(SlicePosition);
						if (chosenSliceViewId != null)
						{
							editLevelModel.ClearHotspotInfo(chosenSliceViewId);
						}
					}
					editLevelModel.SetSlice(SlicePosition, SliceViewId);
				}
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
