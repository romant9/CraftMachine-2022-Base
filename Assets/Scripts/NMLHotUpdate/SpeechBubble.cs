public class SpeechBubble : HUDElementFollowTarget
{
	public UISprite BubbleIcon;

	public void SetActive(bool active)
	{
		BubbleIcon.gameObject.SetActive(active);
	}
}
