using System.Collections.Generic;
using Newtonsoft.Json.Utilities;
using UnityEngine;

public class AotTypeEnforcer : MonoBehaviour
{
	public void Awake()
	{
		AotHelper.EnsureList<string>();
		AotHelper.EnsureList<int>();
		AotHelper.EnsureDictionary<string, HashSet<string>>();
	}
}
