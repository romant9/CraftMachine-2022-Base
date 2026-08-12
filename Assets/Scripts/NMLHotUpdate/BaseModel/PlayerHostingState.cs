using System;

namespace BaseModel
{
	public sealed class PlayerHostingState
	{
		public bool Exists { get; set; }

		public bool IsOnline { get; set; }

		public DateTime LastLoadTime { get; set; }
	}
}
