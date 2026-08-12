using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class RemotePushData
	{
		public long ScheduledTimeEpochSeconds;

		public long ExtraPushInfo;

		public List<string> NotificationIds;
	}
}
