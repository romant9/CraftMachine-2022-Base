using System;
using UnityEngine;

public abstract class TimerComponent : MonoBehaviour
{
	public abstract void Set(TimeSpan timeSpan);
}
