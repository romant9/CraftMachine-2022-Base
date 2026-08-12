using System;

namespace BaseModel
{
	[Obsolete("Unused after 1.60 release")]
	public class InitResponse
	{
		public string ServerAddress { get; set; }

		public GameHostState State { get; set; }

		public string Version { get; set; }

		public DateTime? MaintenanceStarting { get; set; }

		public DateTime? MaintenanceEnding { get; set; }

		public bool? VersionSupported { get; set; }

		public DateTime? VersionValidUntil { get; set; }
	}
}
