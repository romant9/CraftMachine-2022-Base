using Epic.OnlineServices;
using Epic.OnlineServices.Achievements;
using PlayEveryWare.EpicOnlineServices;
using TWDModel;
using UnityEngine;

public class EOSAchievement
{
	private static AchievementsInterface eosAchievementInterface;

	public static void RegisterAchievementsChangedListener()
	{
		GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged -= OnAchievementsChanged;
		GameManager.Instance.playerModel.AchievementManager.OnAchievementsChanged += OnAchievementsChanged;
	}

	private static void OnAchievementsChanged()
	{
		AchievementManager achievementManager = GameManager.Instance.playerModel.AchievementManager;
		for (int i = 0; i < achievementManager.GED.AchievementDefinitions.Length; i++)
		{
			AchievementDefinition achievementDefinition = achievementManager.GED.AchievementDefinitions[i];
			if (achievementDefinition != null && !string.IsNullOrEmpty(achievementDefinition.EpicID) && achievementManager.IsAchievementCompleted(achievementDefinition))
			{
				UnlockAchievementManually(EOSLogin.GetProductUserId(), achievementDefinition.EpicID);
			}
		}
	}

	private static AchievementsInterface GetEOSAchievementInterface()
	{
		if (eosAchievementInterface == null)
		{
			eosAchievementInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAchievementsInterface();
		}
		return eosAchievementInterface;
	}

	public static void QueryAchievementDefinitions(ProductUserId productUserId)
	{
		QueryDefinitionsOptions options = new QueryDefinitionsOptions
		{
			LocalUserId = productUserId
		};
		GetEOSAchievementInterface().QueryDefinitions(ref options, null, delegate(ref OnQueryDefinitionsCompleteCallbackInfo data)
		{
			if (data.ResultCode != Result.Success)
			{
				Debug.LogError("unable to query achievement definitions: " + data.ResultCode);
			}
			else
			{
				CacheAllAchievementDefinitions();
			}
		});
	}

	private static void CacheAllAchievementDefinitions()
	{
		uint achievementDefinitionCount = GetAchievementDefinitionCount();
		CopyAchievementDefinitionV2ByIndexOptions options = new CopyAchievementDefinitionV2ByIndexOptions
		{
			AchievementIndex = 0u
		};
		for (uint num = 0u; num < achievementDefinitionCount; num++)
		{
			options.AchievementIndex = num;
			GetEOSAchievementInterface().CopyAchievementDefinitionV2ByIndex(ref options, out var outDefinition);
			UnityEngine.Debug.LogFormat("Achievements (CacheAllAchievementDefinitions): Id={0}, LockedDisplayName={1}", outDefinition?.AchievementId, outDefinition?.LockedDisplayName);
		}
	}

	private static uint GetAchievementDefinitionCount()
	{
		GetAchievementDefinitionCountOptions options = default(GetAchievementDefinitionCountOptions);
		return GetEOSAchievementInterface().GetAchievementDefinitionCount(ref options);
	}

	public static void QueryPlayerAchievements(ProductUserId productUserId)
	{
		QueryPlayerAchievementsOptions options = MakeQueryPlayerAchievementsOptions(productUserId);
		GetEOSAchievementInterface().QueryPlayerAchievements(ref options, null, delegate(ref OnQueryPlayerAchievementsCompleteCallbackInfo data)
		{
			if (data.ResultCode != Result.Success)
			{
				Debug.LogError("Error after query player achievements: " + data.ResultCode);
			}
			else
			{
				CacheAllPlayerAchievements(productUserId);
			}
		});
	}

	private static QueryPlayerAchievementsOptions MakeQueryPlayerAchievementsOptions(ProductUserId productUserId)
	{
		return new QueryPlayerAchievementsOptions
		{
			LocalUserId = productUserId,
			TargetUserId = productUserId
		};
	}

	private static void CacheAllPlayerAchievements(ProductUserId productUserId)
	{
		AchievementsInterface eOSAchievementInterface = GetEOSAchievementInterface();
		GetPlayerAchievementCountOptions options = new GetPlayerAchievementCountOptions
		{
			UserId = productUserId
		};
		uint playerAchievementCount = eOSAchievementInterface.GetPlayerAchievementCount(ref options);
		CopyPlayerAchievementByIndexOptions options2 = MakeCopyPlayerAchievementByIndexOptions(productUserId);
		for (uint num = 0u; num < playerAchievementCount; num++)
		{
			options2.AchievementIndex = num;
			PlayerAchievement? outAchievement;
			Result result = eOSAchievementInterface.CopyPlayerAchievementByIndex(ref options2, out outAchievement);
			if (result != Result.Success)
			{
				Debug.LogError("Failed to copy player achievement : " + result);
			}
			else
			{
				_ = outAchievement.HasValue;
			}
		}
	}

	private static CopyPlayerAchievementByIndexOptions MakeCopyPlayerAchievementByIndexOptions(ProductUserId productUserId)
	{
		return new CopyPlayerAchievementByIndexOptions
		{
			AchievementIndex = 0u,
			LocalUserId = productUserId,
			TargetUserId = productUserId
		};
	}

	public static void UnlockAchievementManually(ProductUserId productUserId, string achievementId)
	{
		AchievementsInterface eOSAchievementInterface = GetEOSAchievementInterface();
		UnlockAchievementsOptions options = new UnlockAchievementsOptions
		{
			UserId = productUserId,
			AchievementIds = new Utf8String[1] { achievementId }
		};
		eOSAchievementInterface.UnlockAchievements(ref options, null, delegate(ref OnUnlockAchievementsCompleteCallbackInfo info)
		{
			if (info.ResultCode != Result.Success)
			{
				Debug.LogError("UnlockAchievement Failed : " + achievementId);
			}
		});
	}
}
