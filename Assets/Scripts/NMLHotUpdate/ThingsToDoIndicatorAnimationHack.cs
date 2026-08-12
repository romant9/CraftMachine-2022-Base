using UnityEngine;

public class ThingsToDoIndicatorAnimationHack : MonoBehaviour
{
	private UISprite fillSprite;

	private void Start()
	{
		fillSprite = GetComponent<UISprite>();
		GetComponent<TweenProgressBar>().style = UITweener.Style.PingPong;
	}

	private void Update()
	{
		if (fillSprite != null && fillSprite.fillDirection == UIBasicSprite.FillDirection.Radial360)
		{
			if (fillSprite.fillAmount < 0.01f)
			{
				fillSprite.invert = true;
			}
			else if (fillSprite.fillAmount > 0.99f)
			{
				fillSprite.invert = false;
			}
		}
	}
}
