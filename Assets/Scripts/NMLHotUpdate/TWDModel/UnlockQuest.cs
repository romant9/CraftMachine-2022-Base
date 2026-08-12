using System.Collections.Generic;

namespace TWDModel
{
	public class UnlockQuest : QuestModel
	{
		public List<UnlockEntry> UnlockEntries { get; private set; }

		public override int Steps => UnlockEntries.Count;

		public override int CompletedSteps
		{
			get
			{
				int num = 0;
				for (int i = 0; i < UnlockEntries.Count; i++)
				{
					UnlockEntry unlockEntry = UnlockEntries[i];
					switch (unlockEntry.EntryType)
					{
					case BlackboardEntryType.Unlock:
						num += (base.manager.Player.Blackboard.IsUnlockedAny(unlockEntry.EntryKey) ? 1 : 0);
						break;
					case BlackboardEntryType.Toggle:
						num += (base.manager.Player.Blackboard.IsToggleAny(unlockEntry.EntryKey) ? 1 : 0);
						break;
					case BlackboardEntryType.Counter:
						num += ((base.manager.Player.Blackboard.GetMaxCounterValue(unlockEntry.EntryKey) >= unlockEntry.Target) ? 1 : 0);
						break;
					}
				}
				return num;
			}
		}

		public UnlockQuest()
		{
		}

		public UnlockQuest(string unlockKeyString)
		{
			UnlockEntries = new List<UnlockEntry>();
			string[] array = unlockKeyString.Split(';');
			if (array == null || array.Length == 0)
			{
				return;
			}
			foreach (string text in array)
			{
				string[] array2 = text.Split('.');
				if (array2 == null || array2.Length <= 1)
				{
					continue;
				}
				string text2 = array2[0];
				if (text2 == BlackboardEntryType.Unlock.ToString())
				{
					UnlockEntries.Add(new UnlockEntry(BlackboardEntryType.Unlock, text));
				}
				else if (text2 == BlackboardEntryType.Counter.ToString())
				{
					string[] array3 = text.Split('=');
					int target = 1;
					if (array3.Length == 2)
					{
						target = int.Parse(array3[1]);
					}
					UnlockEntries.Add(new UnlockEntry(BlackboardEntryType.Counter, array3[0], target));
				}
				else if (text2 == BlackboardEntryType.Toggle.ToString())
				{
					UnlockEntries.Add(new UnlockEntry(BlackboardEntryType.Toggle, text));
				}
			}
		}

		public int GetStepCurrentValue(UnlockEntry unlockEntry)
		{
			switch (unlockEntry.EntryType)
			{
			case BlackboardEntryType.Unlock:
				if (!base.manager.Player.Blackboard.IsUnlockedAny(unlockEntry.EntryKey))
				{
					return 0;
				}
				return 1;
			case BlackboardEntryType.Toggle:
				if (!base.manager.Player.Blackboard.IsToggleAny(unlockEntry.EntryKey))
				{
					return 0;
				}
				return 1;
			case BlackboardEntryType.Counter:
				return base.manager.Player.Blackboard.GetMaxCounterValue(unlockEntry.EntryKey);
			default:
				return 0;
			}
		}
	}
}
