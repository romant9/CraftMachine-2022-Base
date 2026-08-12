using System;

namespace BaseModel
{
	public class LockRespond
	{
		public enum LockStatus : byte
		{
			InternalError = 0,
			Locked = 1,
			AlreadyLocked = 2,
			UnkonwnPlayer = 3,
			LoadLocked = 4,
			Banned = 5,
			PlayerDisabled = 6
		}

		public LockStatus Status { get; set; }

		public bool IsLocked
		{
			get
			{
				if (Status != LockStatus.Locked && Status != LockStatus.AlreadyLocked && Status != LockStatus.LoadLocked)
				{
					return Status == LockStatus.Banned;
				}
				return true;
			}
		}

		public DateTime LockedUntil { get; set; }

		public string Reason { get; set; }
	}
}
