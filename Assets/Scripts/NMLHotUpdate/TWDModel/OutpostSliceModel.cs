using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class OutpostSliceModel : TWDModelObjectWithViewId, IRunLocationItemContainer
	{
		public SlicePosition SlicePosition { get; set; }

		public List<TWDModelObject> Models { get; set; }

		public string ExportedVisibility { get; set; }

		public string ExportedMovement { get; set; }

		public OutpostSliceModel()
		{
			Models = new List<TWDModelObject>();
		}

		public OutpostSliceModel(string viewId)
			: this()
		{
			base.ViewId = viewId;
		}

		public void AddModelObject(TWDModelObject obj)
		{
			Models.Add(obj);
		}

		public void AddMission(MissionModel model)
		{
			throw new NotImplementedException();
		}

		public void AddSlice(OutpostSliceModel slice)
		{
		}

		public List<OutpostHotspotModel> GetHotspotModels()
		{
			List<OutpostHotspotModel> list = new List<OutpostHotspotModel>();
			for (int i = 0; i < Models.Count; i++)
			{
				if (Models[i] is OutpostHotspotModel item)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public OutpostHotspotModel GetHotspotModel(string viewId)
		{
			for (int i = 0; i < Models.Count; i++)
			{
				if (Models[i] is OutpostHotspotModel outpostHotspotModel && outpostHotspotModel.ViewId == viewId)
				{
					return outpostHotspotModel;
				}
			}
			return null;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
