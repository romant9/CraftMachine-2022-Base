using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildBossAnimatePopup : HUDElement
{
	private const float AutoDismissSeconds = 3f;

	[SerializeField]
	private GameObject animationRoot;

	[SerializeField]
	private UITexture bossTexture;

	[SerializeField]
	private UILabel centerLabel;

	[SerializeField]
	private GuildBossAnimateBossConfig[] bossConfigs;

	[SerializeField]
	private string defaultLabelLocalizationKey = "Battle_GuildBoss_BossEntry";

	[SerializeField]
	private string bossTextureResourcePathPrefix = "UI/Textures/";

	[SerializeField]
	[Tooltip("Default NGUI tweenGroup played on open/close when no boss-specific group is set.")]
	private int defaultEnterTweenGroup;

	private bool canDismissByClick;

	private bool isPlayingExitAnimation;

	private float dismissTimer;

	private int activeEnterTweenGroup;

	private void Awake()
	{
		DebugClassString = "GuildBossAnimatePopup";
		if (animationRoot == null)
		{
			animationRoot = base.gameObject;
		}
	}

	public override void Open()
	{
		OpenWithBoss(null);
	}

	public override void OpenWithStateData(object data)
	{
		OpenWithBoss(data as GuildBossAnimatePopupData);
	}

	public override void OpenForModel(ModelObject model)
	{
		if (!(model is ActorModel actorModel))
		{
			OpenWithBoss(null);
			return;
		}
		OpenWithBoss(new GuildBossAnimatePopupData
		{
			BossId = actorModel.ActorDefinitionID,
			TextureName = actorModel.ActorDefinitionID,
			LabelText = actorModel.Name
		});
	}

	public void OpenWithBoss(GuildBossAnimatePopupData data)
	{
		if (!base.gameObject.activeSelf || isPlayingExitAnimation)
		{
			isPlayingExitAnimation = false;
			canDismissByClick = false;
			dismissTimer = 0f;
			ApplyBossVisual(data);
			base.gameObject.SetActive(value: true);
			activeEnterTweenGroup = ResolveEnterTweenGroup(data);
			PlayEnterTweens(activeEnterTweenGroup);
		}
	}

	public override void Close()
	{
		Dismiss();
	}

	public override void Update()
	{
		base.Update();
		if (base.gameObject.activeSelf && !isPlayingExitAnimation && canDismissByClick)
		{
			dismissTimer -= Time.unscaledDeltaTime;
			if (dismissTimer <= 0f)
			{
				Dismiss();
			}
			else if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
			{
				Dismiss();
			}
		}
	}

	private void PlayEnterTweens(int tweenGroup)
	{
		TweenManager.ResetToBeginningTweenGroup(animationRoot, tweenGroup);
		TweenManager.PlayTweenGroup(animationRoot, tweenGroup, forward: true, OnEnterAnimationComplete);
	}

	private void OnEnterAnimationComplete()
	{
		canDismissByClick = true;
		dismissTimer = 3f;
	}

	private void Dismiss()
	{
		if (!isPlayingExitAnimation && base.gameObject.activeSelf)
		{
			isPlayingExitAnimation = true;
			canDismissByClick = false;
			dismissTimer = 0f;
			TweenManager.PlayTweenGroup(animationRoot, activeEnterTweenGroup, forward: false, OnCloseAnimOver);
		}
	}

	protected override void OnCloseAnimOver()
	{
		isPlayingExitAnimation = false;
		base.gameObject.SetActive(value: false);
	}

	private void ApplyBossVisual(GuildBossAnimatePopupData data)
	{
		GuildBossAnimateBossConfig guildBossAnimateBossConfig = ResolveBossConfig(data);
		Texture texture = null;
		if (data != null && data.BossTexture != null)
		{
			texture = data.BossTexture;
		}
		else if (guildBossAnimateBossConfig != null && guildBossAnimateBossConfig.BossTexture != null)
		{
			texture = guildBossAnimateBossConfig.BossTexture;
		}
		else
		{
			string text = data?.TextureName;
			if (string.IsNullOrEmpty(text) && guildBossAnimateBossConfig != null)
			{
				text = guildBossAnimateBossConfig.TextureName;
			}
			texture = LoadBossTexture(text);
		}
		if (bossTexture != null && texture != null)
		{
			bossTexture.mainTexture = texture;
		}
		if (centerLabel != null)
		{
			string text2 = ResolveLabelText(data, guildBossAnimateBossConfig);
			if (!string.IsNullOrEmpty(text2))
			{
				centerLabel.text = text2;
			}
		}
	}

	private string ResolveLabelText(GuildBossAnimatePopupData data, GuildBossAnimateBossConfig config)
	{
		if (data != null && !string.IsNullOrEmpty(data.LabelText))
		{
			return data.LabelText;
		}
		if (config != null && !string.IsNullOrEmpty(config.LabelLocalizationKey))
		{
			return LocalizationManager.GetText(config.LabelLocalizationKey);
		}
		if (!string.IsNullOrEmpty(defaultLabelLocalizationKey))
		{
			return LocalizationManager.GetText(defaultLabelLocalizationKey);
		}
		return string.Empty;
	}

	private GuildBossAnimateBossConfig ResolveBossConfig(GuildBossAnimatePopupData data)
	{
		if (bossConfigs == null || data == null)
		{
			return null;
		}
		string text = ((!string.IsNullOrEmpty(data.TextureName)) ? data.TextureName : data.BossId);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		for (int i = 0; i < bossConfigs.Length; i++)
		{
			GuildBossAnimateBossConfig guildBossAnimateBossConfig = bossConfigs[i];
			if (guildBossAnimateBossConfig != null && (guildBossAnimateBossConfig.TextureName == text || guildBossAnimateBossConfig.BossId == text))
			{
				return guildBossAnimateBossConfig;
			}
		}
		return null;
	}

	private int ResolveEnterTweenGroup(GuildBossAnimatePopupData data)
	{
		if (data != null && data.EnterTweenGroup >= 0)
		{
			return data.EnterTweenGroup;
		}
		return ResolveBossConfig(data)?.EnterTweenGroup ?? defaultEnterTweenGroup;
	}

	private Texture LoadBossTexture(string textureName)
	{
		if (string.IsNullOrEmpty(textureName))
		{
			return null;
		}
		string text = bossTextureResourcePathPrefix + textureName;
		Texture texture = UnityUtils.LoadAsset<Texture>(text);
		if (texture == null && text != textureName)
		{
			texture = UnityUtils.LoadAsset<Texture>(textureName);
		}
		return texture;
	}
}
