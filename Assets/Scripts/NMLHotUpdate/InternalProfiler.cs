using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class InternalProfiler : MonoBehaviour
{
	private struct Profile
	{
		public string name;

		public long time;
	}

	private static List<Profile> profiles;

	private static Stopwatch internalTimer = new Stopwatch();

	public static void RecordTime(string inName)
	{
		Profile item = new Profile
		{
			name = inName,
			time = internalTimer.ElapsedMilliseconds
		};
		profiles.Add(item);
	}

	public static void dumpData()
	{
		for (int i = 0; i < profiles.Count; i++)
		{
			_ = profiles[i];
		}
	}

	public static void initProfiler()
	{
		profiles = new List<Profile>(128);
		internalTimer.Start();
	}
}
