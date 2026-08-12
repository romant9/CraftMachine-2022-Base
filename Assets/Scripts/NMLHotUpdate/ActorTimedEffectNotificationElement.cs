using System.Collections.Generic;
using UnityEngine;

public class ActorTimedEffectNotificationElement : ActorNotificationElement
{
	[SerializeField]
	private List<TimedEffectEntry> TimedEffectEntries = new List<TimedEffectEntry>();

	public void Init()
	{
		TimedEffectEntry timedEffectEntry = TimedEffectEntries.Find((TimedEffectEntry x) => x.Sprite == NotificationIcon.spriteName);
		NotificationIcon.gradientTop = timedEffectEntry.GradientTop;
		NotificationIcon.gradientBottom = timedEffectEntry.GradientBottom;
	}
}
