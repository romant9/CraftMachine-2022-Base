using System.Collections.Generic;
using UnityEngine;

public class TweenerPlayer : MonoBehaviour
{
	private Dictionary<int, List<UITweener>> animationsIndexedByGroupId;

	private bool IsInitialized => animationsIndexedByGroupId != null;

	public Dictionary<int, List<UITweener>> AnimationsIndexedByGroupId()
	{
		if (animationsIndexedByGroupId == null)
		{
			Init();
		}
		return animationsIndexedByGroupId;
	}

	private void Awake()
	{
		if (!IsInitialized)
		{
			Init();
		}
	}

	private void Init()
	{
		animationsIndexedByGroupId = new Dictionary<int, List<UITweener>>();
		UITweener[] componentsInChildren = GetComponentsInChildren<UITweener>();
		foreach (UITweener uITweener in componentsInChildren)
		{
			if (animationsIndexedByGroupId.ContainsKey(uITweener.tweenGroup))
			{
				animationsIndexedByGroupId[uITweener.tweenGroup].Add(uITweener);
				continue;
			}
			animationsIndexedByGroupId[uITweener.tweenGroup] = new List<UITweener> { uITweener };
		}
	}

	public void PlayGroup(int groupId, bool instant)
	{
		if (!IsInitialized)
		{
			Init();
		}
		if (!animationsIndexedByGroupId.ContainsKey(groupId))
		{
			return;
		}
		animationsIndexedByGroupId[groupId].ForEach(delegate(UITweener x)
		{
			if (instant)
			{
				x.ResetToEnd();
			}
			else
			{
				x.ResetToBeginning();
			}
			x.PlayForward();
		});
	}
}
