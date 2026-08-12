namespace BaseModel
{
	public sealed class LoginRequest
	{
		public string Identification;

		public string ModelChecksum;

		public string EconomyChecksum;

		public string ClientVersion;

		public string ClientModelVersion;

		public string HotFixVersion;

		public string InstallationId;

		public string BuildId;

		public long CurrentDateStamp;

		public long InstallDateStamp;

		public long InstallLaunchCount;

		public long LastSessionDateStamp;

		public int LicenseValidationStatus;

		public DeviceInfo Device;

		public TDPresetProperties TDPresetProperties;

		public PcPlatform PcPlatform;
	}
}
