using System.Collections.Generic;
using UnityEngine;

public class PrizeCounter: MonoBehaviour
{
	public UILabel CounterLabel;
	public int CounterValue { get; set; }
	public int CounterNearestValue { get; set; }
	public List<int> CounterNearestList { get; set; }
}
