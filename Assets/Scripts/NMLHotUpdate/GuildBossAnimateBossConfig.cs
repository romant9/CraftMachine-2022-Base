using System;
using UnityEngine;

[Serializable]
public class GuildBossAnimateBossConfig
{
	public string BossId;

	public string TextureName;

	public Texture BossTexture;

	public string LabelLocalizationKey;

	[Tooltip("NGUI tweenGroup on animationRoot children. Configure TweenPosition/TweenScale curves in the prefab.")]
	public int EnterTweenGroup;
}
