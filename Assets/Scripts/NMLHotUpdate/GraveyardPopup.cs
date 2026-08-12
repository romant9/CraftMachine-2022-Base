public class GraveyardPopup : HUDElement
{
	public override void Open()
	{
		base.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_graveyard");
	}
}
