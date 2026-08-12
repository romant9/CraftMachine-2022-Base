using System;

namespace TWDModel
{
	[Serializable]
	public class SPTraitSlot
	{
		public string ID { get; set; }

		public SPTraitsLockState LockState { get; set; }

		public int Level { get; set; }

		public int MaxLevel { get; set; }

		public bool CanUpgrade { get; set; }

		public SPTraitSlot()
		{
			LockState = SPTraitsLockState.Unlocked;
			Level = 1;
			MaxLevel = 1;
			CanUpgrade = false;
		}

		public SPTraitSlot(string id)
			: this()
		{
			ID = id;
		}

		public SPTraitSlot(string id, SPTraitsLockState lockState)
			: this(id)
		{
			LockState = lockState;
		}

		public bool IsMaxLevel()
		{
			return Level >= MaxLevel;
		}

		public string GetLockIcon()
		{
			string text = "";
			return LockState switch
			{
				SPTraitsLockState.Unlocked => "UI_Icon_SPTraitsRemold_Unlock", 
				SPTraitsLockState.ForceLocked => "UI_Icon_SPTraitsRemold_LockedYellow", 
				SPTraitsLockState.Locked => "UI_Icon_SPTraitsRemold_Locked", 
				_ => "icon_lock", 
			};
		}
	}
}
