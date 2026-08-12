using BaseModel;
using Newtonsoft.Json;
using System.Collections.Generic;

public class CallData
{
	public int CallNumber { get; set; }
	[JsonIgnore]
	public RadioCallButton ButtonBySlotNumber { get; set; }
	[JsonIgnore]
	public List<LootEntry> LootEntryList { get; set; }
	public int CallPrice => ButtonBySlotNumber != null ? ButtonBySlotNumber.GetCallPrice() : 0;
	public int SlotNumber => ButtonBySlotNumber != null ? ButtonBySlotNumber.SlotNumber : -1;
	[JsonIgnore]
	public Dictionary<string, ModelRandom> DedicatedRandoms { get; set; }
	public List<List<bool>> LootsRerollLockingList { get; set; }
	public List<List<string>> RewardAmountList { get; set; }

	public CallData()
	{
		LootsRerollLockingList = new List<List<bool>>();
		RewardAmountList = new List<List<string>>();
		LootEntryList = new List<LootEntry>();
	}
}