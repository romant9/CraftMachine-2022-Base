using System;

namespace TWDModel
{
	[Serializable]
	public class NodeConnection
	{
		public int TargetGuidHash;

		public string InputPinId;

		public string OutputPinId;
	}
}
