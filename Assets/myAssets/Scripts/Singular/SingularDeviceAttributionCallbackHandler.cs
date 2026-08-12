using System.Collections.Generic;

namespace Singular
{
	public interface SingularDeviceAttributionCallbackHandler
	{
		void OnSingularDeviceAttributionCallback(Dictionary<string, object> attributionInfo);
	}
}