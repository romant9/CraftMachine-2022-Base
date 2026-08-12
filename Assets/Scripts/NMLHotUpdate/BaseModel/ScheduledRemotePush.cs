using System.Runtime.Serialization;

namespace BaseModel
{
	public sealed class ScheduledRemotePush
	{
		[IgnoreDataMember]
		public string PlayerId;

		public string HashedId;

		public string AppleNotificationId;

		public string GoogleNotificationId;
	}
}
