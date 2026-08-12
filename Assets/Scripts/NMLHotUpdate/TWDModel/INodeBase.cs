using System.Collections.Generic;

namespace TWDModel
{
	public interface INodeBase
	{
		bool IsClientOnly { get; }

		List<NodeConnection> NodeConnections { get; }

		List<NodeConnection> NodeDataConnections { get; }

		Dictionary<string, List<NodeDelegate_Void>> RuntimeDelegateMap { get; }

		Dictionary<string, List<RuntimeExportPinInfo>> RuntimePropertyInfoMap { get; }

		int GuidHash { get; set; }
	}
}
