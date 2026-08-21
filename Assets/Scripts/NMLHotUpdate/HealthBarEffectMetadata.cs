using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public static class HealthBarEffectMetadata
{
	private const string LocKeyPrefix = "CombatToolitp.";

	public static bool TryToInfoData(ActorEffectInfoSnapshot snapshot, out ActorEffectInfoData data)
	{
		data = default(ActorEffectInfoData);
		if (string.IsNullOrEmpty(snapshot.Id) || !snapshot.Icon.IsValid)
		{
			return false;
		}
		string nameLocKey = "CombatToolitp." + snapshot.Id;
		data = CreateInfoData(snapshot.Icon, snapshot.Bg, nameLocKey, ResolveDescLocKey(nameLocKey), snapshot.TurnCount);
		return true;
	}

	public static bool TryFromTimedEffect(ActorStatusInfoHealthBar statusInfo, List<TimedEffectEntry> timedEffectIndicators, UISprite timedEffectIconSprite, GameObject timedEffectSearchRoot, UISprite secondaryTimedEffectIconSprite, GameObject secondaryTimedEffectSearchRoot, INGUIAtlas timedEffectAtlas, INGUIAtlas secondaryTimedEffectAtlas, out ActorEffectInfoData data)
	{
		data = default(ActorEffectInfoData);
		if (statusInfo == null)
		{
			return false;
		}
		int num = timedEffectIndicators.FindIndex((TimedEffectEntry x) => x.TimedEffectType == statusInfo.StatusType);
		if (num < 0)
		{
			return false;
		}
		TimedEffectEntry timedEffectEntry = timedEffectIndicators[num];
		string nameLocKey = "CombatToolitp." + statusInfo.StatusType;
		INGUIAtlas atlas = ((statusInfo.StatusType == TimedEffectType.Marked && secondaryTimedEffectAtlas != null) ? secondaryTimedEffectAtlas : timedEffectAtlas);
		int num2;
		UISprite uISprite;
		if (statusInfo.StatusType == TimedEffectType.Marked)
		{
			num2 = ((secondaryTimedEffectIconSprite != null) ? 1 : 0);
			if (num2 != 0)
			{
				uISprite = secondaryTimedEffectIconSprite;
				goto IL_009e;
			}
		}
		else
		{
			num2 = 0;
		}
		uISprite = timedEffectIconSprite;
		goto IL_009e;
		IL_009e:
		UISprite iconSprite = uISprite;
		GameObject searchRoot = ((num2 != 0) ? secondaryTimedEffectSearchRoot : timedEffectSearchRoot);
		ActorEffectCapture actorEffectCapture = HealthBarEffectIconCapture.CaptureFromSprite(iconSprite, searchRoot);
		data = CreateInfoData(new ActorEffectSprite
		{
			Name = timedEffectEntry.Sprite,
			Atlas = atlas
		}, actorEffectCapture.Bg, nameLocKey, ResolveDescLocKey(nameLocKey), statusInfo.TurnCount);
		return true;
	}

	private static string ResolveDescLocKey(string nameLocKey)
	{
		string text = nameLocKey + ".Desc";
		if (!SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text))
		{
			return nameLocKey;
		}
		return text;
	}

	private static ActorEffectInfoData CreateInfoData(ActorEffectSprite icon, ActorEffectSprite bg, string nameLocKey, string descLocKey, int turnCount)
	{
		return new ActorEffectInfoData
		{
			Icon = icon,
			Bg = bg,
			NameLocKey = nameLocKey,
			DescLocKey = descLocKey,
			TurnCount = turnCount
		};
	}
}
