public struct ActorEffectSprite
{
	public string Name;

	public INGUIAtlas Atlas;

	public bool IsValid => !string.IsNullOrEmpty(Name);

	public static ActorEffectSprite From(UISprite sprite)
	{
		if (sprite == null)
		{
			return default(ActorEffectSprite);
		}
		return new ActorEffectSprite
		{
			Name = sprite.spriteName,
			Atlas = sprite.atlas
		};
	}

	public void ApplyTo(UISprite target)
	{
		if (target == null)
		{
			return;
		}
		if (!IsValid || Atlas == null)
		{
			Helpers.GameObjectSetActive(target.gameObject, value: false);
			return;
		}
		if (target.atlas != Atlas)
		{
			target.atlas = Atlas;
		}
		HelpersUI.SetSprite(target, Name);
	}
}
