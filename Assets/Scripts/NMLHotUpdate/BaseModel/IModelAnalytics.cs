using System.Collections.Generic;

namespace BaseModel
{
	public interface IModelAnalytics
	{
		void CreateEvent<T>(string type, Dictionary<string, T> properties);
	}
}
