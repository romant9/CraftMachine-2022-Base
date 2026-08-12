namespace TWDModel
{
	public interface IRunLocationItemContainer
	{
		void AddModelObject(TWDModelObject obj);

		void AddMission(MissionModel model);

		void AddSlice(OutpostSliceModel slice);
	}
}
