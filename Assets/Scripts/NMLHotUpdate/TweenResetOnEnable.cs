using System.Collections.Generic;
using UnityEngine;

public class TweenResetOnEnable : MonoBehaviour
{
	[SerializeField]
	private List<UITweener> tweeners;

	public void OnEnable()
	{
		tweeners.ForEach(delegate(UITweener x)
		{
			x.ResetToBeginning();
			x.PlayForward();
		});
	}
}
