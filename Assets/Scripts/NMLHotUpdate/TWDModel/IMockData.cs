namespace TWDModel
{
	public interface IMockData<T> where T : class
	{
		T CreateMockData();
	}
}
