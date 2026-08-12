using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GuildModel : GroupModelBase, ICustomLoggerDebugInfo, ILeaderboardState
	{
		public const int MaxNumberRequestExtra = 5;

		public const int OnlineStatusThresholdMinutes = 5;

		public const string GuildCreated = "GuildCreated";

		public const string GuildModified = "GuildModified";

		public const string MemberAdded = "MemberAdded";

		public const string MemberRefused = "MemberRefused";

		public const string MemberAccepted = "MemberAccepted";

		public const string MemberChanged = "MemberChanged";

		public const string MemberRemoved = "MemberRemoved";

		public const string MessageAdded = "MessageAdded";

		public const string MessagesTruncated = "MessagesTruncated";

		public const string StarsAdded = "StarsAdded";

		public const string GiftSent = "GiftSent";

		public const string MemberActivityStatusChanged = "MemberActivityStatusChanged";

		public const string GuildNameChanged = "GuildNameChanged";

		public const string VictoryPointsChanged = "VictoryPointsChanged";

		public const string PinnedChatMessage = "PinnedChatMessaged";

		public static int TotalRotatingDaysForScore = 14;

		public const int MaxMembers = 20;

		public const int MaxChatMessages = 50;

		public const int DescriptionMinLength = 0;

		public const int DescriptionMaxLength = 200;

		public List<GuildMemberInfo> GuildMembers = new List<GuildMemberInfo>();

		public List<GuildMemberInfo> GuildMembersPending = new List<GuildMemberInfo>();

		public List<ChatMessage> ChatMessages = new List<ChatMessage>();

		public Dictionary<string, int> CurrentChallengeMemberInfos = new Dictionary<string, int>();

		public List<string> CurrentChallengeParticipants = new List<string>();

		public Dictionary<string, int> ChallengeStars = new Dictionary<string, int>();

		public Dictionary<string, int> TotalAllTimeGvGVpAccumulatedPerPlayer = new Dictionary<string, int>();

		[NonSerialized]
		[JsonIgnore]
		public static int GuildLeaderboardScoreBufferSize = 30;

		public override int Score
		{
			get
			{
				List<int> accumulatedStars = new List<int>();
				long num = 0L;
				if (LastNewDayScoreTimeStamp <= 0)
				{
					accumulatedStars.Add(CurrentChallengeStars);
				}
				else
				{
					num = (TimeStamp - LastNewDayScoreTimeStamp) / 86400000;
				}
				List<int> accumulatedMissionCount = null;
				if (RotationAccumulatedStars != null)
				{
					accumulatedStars.AddRange(RotationAccumulatedStars);
				}
				if (num > 0)
				{
					ResizeScoreListForElapsedDays(ref accumulatedStars, ref accumulatedMissionCount, num);
				}
				int num2 = 0;
				for (int i = Math.Max(accumulatedStars.Count - TotalRotatingDaysForScore, 0); i < accumulatedStars.Count; i++)
				{
					num2 += accumulatedStars[i];
				}
				return num2;
			}
		}

		public override float IndexScore
		{
			get
			{
				List<int> accumulatedMissionCount = new List<int>();
				long num = 0L;
				if (LastNewDayScoreTimeStamp <= 0)
				{
					accumulatedMissionCount.Add(1);
				}
				else
				{
					num = (TimeStamp - LastNewDayScoreTimeStamp) / 86400000;
				}
				List<int> accumulatedStars = null;
				if (RotationAccumulatedMissionCount != null)
				{
					accumulatedMissionCount.AddRange(RotationAccumulatedMissionCount);
				}
				if (num > 0)
				{
					ResizeScoreListForElapsedDays(ref accumulatedStars, ref accumulatedMissionCount, num);
				}
				int num2 = 0;
				for (int i = Math.Max(accumulatedMissionCount.Count - TotalRotatingDaysForScore, 0); i < accumulatedMissionCount.Count; i++)
				{
					num2 += accumulatedMissionCount[i];
				}
				if (num2 > 0)
				{
					int num3 = Score / num2;
					if (float.TryParse(Score + "." + num3, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
					{
						return result;
					}
				}
				return 0f;
			}
		}

		public int AdBucket { get; set; }

		public string AdIdentifierForAnalytics { get; set; }

		public long AdCreationTimeStampSeconds { get; set; }

		public long AdExpireTimeStampSeconds { get; set; }

		public long AdAvailableTimeSeconds => Math.Max(0L, AdExpireTimeStampSeconds - Math.Max(0L, TimeStamp) / 1000);

		public long FirstActiveDateTimeStamp { get; set; }

		public long NextChangeNameTimeStampSeconds { get; set; }

		[JsonIgnore]
		public long ChangeNameColdTimeSeconds => Math.Max(0L, NextChangeNameTimeStampSeconds - Math.Max(0L, TimeStamp) / 1000);

		public string AdCreatorId { get; set; }

		public float AverageMemberLevel
		{
			get
			{
				float num = 0f;
				if (GuildMembers.Count > 0)
				{
					for (int i = 0; i < GuildMembers.Count; i++)
					{
						num += (float)GuildMembers[i].PlayerLevel;
					}
					return num / (float)GuildMembers.Count;
				}
				return num;
			}
		}

		public int LowestMemberLevel
		{
			get
			{
				if (GuildMembers != null)
				{
					int num = int.MaxValue;
					for (int i = 0; i < GuildMembers.Count; i++)
					{
						num = Math.Min(GuildMembers[i].PlayerLevel, num);
					}
					return num;
				}
				return 0;
			}
		}

		public int HighestMemberLevel
		{
			get
			{
				int num = 0;
				if (GuildMembers != null)
				{
					for (int i = 0; i < GuildMembers.Count; i++)
					{
						num = Math.Max(GuildMembers[i].PlayerLevel, num);
					}
				}
				return num;
			}
		}

		public string Version { get; set; }

		public List<GuildGift> AvailableGifts { get; set; }

		public GuildJoinType JoinType { get; set; }

		public string Purpose { get; set; }

		public bool IsFull { get; set; }

		public string CurrentChallengeId { get; set; }

		public int CurrentChallengeStars { get; set; }

		public int TotalChallengeStars { get; set; }

		public int PreviousChallengeStars { get; set; }

		public int PreviousChallengeStarsPerMember { get; set; }

		public int HighestChallengeStarsCount { get; set; }

		public int NumberChallengeStarted { get; set; }

		public List<int> RotationAccumulatedStars { get; set; }

		public List<int> RotationAccumulatedMissionCount { get; set; }

		public long LastNewDayScoreTimeStamp { get; set; }

		public long LastPurposeEditTimeStamp { get; set; }

		public int TotalAllTimeAccumulatedVp { get; set; }

		public GvGSeasonModel GvGSeasonModel { get; set; }

		[JsonIgnore]
		public GuildWarModel GuildWarModel => GvGSeasonModel.GuildWarModel;

		public int MatchmakingVersion { get; set; }

		public GuildBattleMatchmakingInfo GuildBattleMatchmakingInfo { get; set; }

		public GuildRemotePushNotification GuildRemotePushNotification { get; set; }

		public int GuildInfoCurrentVP { get; set; }

		[JsonIgnore]
		public int GuildBattleTier => GvGSeasonModel?.CurrentTier ?? 0;

		[JsonIgnore]
		public int PreviousVictoryPoints => GvGSeasonModel?.PreviousVictoryPoints ?? 0;

		[JsonIgnore]
		public int CurrentVictoryPoints => GvGSeasonModel?.CurrentVictoryPoints ?? 0;

		[JsonIgnore]
		public int CurrentSeasonVictories => GvGSeasonModel?.CurrentSeasonVictories ?? 0;

		[JsonIgnore]
		public int CurrentSeasonDefeats => GvGSeasonModel?.CurrentSeasonDefeats ?? 0;

		public int NumberMembers => GuildMembers.Count;

		public int NumberPendingRequests => GuildMembersPending.Count;

		[JsonIgnore]
		public long TimeStamp
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return (long)(base.Created.ToUniversalTime() - dateTime).TotalSeconds * 1000 + base.LifeTime;
			}
		}

		[JsonIgnore]
		public bool CanReceiveRequest => GuildMembersPending.Count < 20 - GuildMembers.Count + 5;

		public bool LeaderboardUpdated { get; set; } = true;

		public long LastGvGLeaderboardUpdateTime { get; set; }

		public string LeaderboardName => Leaderboards.GvgGuildGlobalVpAllTimeTotal;

		public GuildBanModel GuildBanModel { get; set; } = new GuildBanModel();

		public bool IsPurposeEditable(int editIntervalSecs)
		{
			long num = 86400000L;
			if (editIntervalSecs > 0)
			{
				num = (long)editIntervalSecs * 1000L;
			}
			if (TimeStamp > LastPurposeEditTimeStamp + num)
			{
				return true;
			}
			return false;
		}

		public bool CanEdit(string memberId)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(memberId);
			if (memberInfo != null)
			{
				return memberInfo.Role > GuildMemberRole.Elder;
			}
			return false;
		}

		public bool CanAcceptRequests(string memberId)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(memberId);
			if (memberInfo != null)
			{
				return memberInfo.Role > GuildMemberRole.Normal;
			}
			return false;
		}

		public bool CanKickOut(string memberId)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(memberId);
			if (memberInfo != null)
			{
				return memberInfo.Role > GuildMemberRole.Elder;
			}
			return false;
		}

		public bool HasPendingRequest(string memberId)
		{
			return GetMemberPendingInfo(memberId) != null;
		}

		public GuildMemberRole? GetMemberRole(string actorId)
		{
			return GetMemberInfo(actorId)?.Role;
		}

		public GuildModel()
		{
			if (GuildRemotePushNotification == null)
			{
				GuildRemotePushNotification = new GuildRemotePushNotification();
			}
		}

		public GuildModel(string id)
			: base(id)
		{
			if (GuildRemotePushNotification == null)
			{
				GuildRemotePushNotification = new GuildRemotePushNotification();
			}
		}

		public void Start()
		{
			clearExpiredAndRegisteredGifts();
			clearExpiredAd();
			GuildInfoCurrentVP = CurrentVictoryPoints;
		}

		public void StartGroupChildren(PlayerModel player, GameEconomyData ged)
		{
			if (GvGSeasonModel == null)
			{
				GvGSeasonModel = new GvGSeasonModel();
			}
			GvGSeasonModel.SetPlayerOwnerAndGameEconomyData(ged, GvGSeasonModel, player);
			InitializeGuildBattleMatchmakingInfo();
			GuildBattleMatchmakingInfo?.Start();
		}

		public bool IsValidNameLength(string name)
		{
			if (name.Length >= 3)
			{
				return name.Length <= 15;
			}
			return false;
		}

		public bool IsValidDescriptionLength(string description)
		{
			if (description.Length >= 0)
			{
				return description.Length <= 200;
			}
			return false;
		}

		public bool IsValidCharacters(string name)
		{
			if (name.Length == 0)
			{
				return true;
			}
			return new Regex("^[\\w\\-]+( [\\w\\-]+)*$").IsMatch(name);
		}

		public bool IsValidPurpose(string purpose, List<string> purposeTypes)
		{
			if (purpose != null && purposeTypes != null)
			{
				return purposeTypes.Contains(purpose.ToLowerInvariant());
			}
			return false;
		}

		public static string GetDefaultPurpose(List<string> purposeTypes)
		{
			if (purposeTypes != null && purposeTypes.Count > 0)
			{
				return purposeTypes[0];
			}
			return null;
		}

		public void SetFullParameter()
		{
			IsFull = NumberMembers >= 20 || !CanReceiveRequest;
		}

		public void CreateGuild(string name, string description, GuildJoinType joinType, GuildMemberInfo leader, string leaderCountryCode, long timeStamp, string purpose, GameEconomyData ged)
		{
			if (IsValidCharacters(name) && IsValidNameLength(name) && IsValidDescriptionLength(description))
			{
				base.Name = name;
				base.Description = description;
				JoinType = joinType;
				Purpose = purpose;
				LastPurposeEditTimeStamp = timeStamp;
				base.CountryCode = leaderCountryCode.ToLowerInvariant();
				GuildMembers = new List<GuildMemberInfo>();
				GuildMembers.Add(leader);
				leader.GuildJoinedDate = timeStamp;
				StartGroupChildren(null, ged);
				GuildBattleMatchmakingInfo.GuildName = base.Name;
				NotifyChange("GuildCreated");
			}
		}

		public TWDModelResult AddMember(string playerId, string name, PlayerEmblem playerEmblem, int level, int totalVPPoints, long timestamp, GuildMemberRole role = GuildMemberRole.Normal, GuildMemberState state = GuildMemberState.PendingRequest)
		{
			if (GetMemberInfo(playerId) != null || GetMemberPendingInfo(playerId) != null)
			{
				return TWDModelResult.Error;
			}
			GuildMemberInfo guildMemberInfo = new GuildMemberInfo();
			guildMemberInfo.Name = name;
			guildMemberInfo.GuildId = base.Id;
			guildMemberInfo.MemberId = playerId;
			guildMemberInfo.PlayerLevel = level;
			guildMemberInfo.PlayerEmblem = playerEmblem;
			guildMemberInfo.TotalVP = totalVPPoints;
			guildMemberInfo.Role = role;
			guildMemberInfo.State = state;
			guildMemberInfo.LastActiveDate = TimeStamp;
			if (state == GuildMemberState.PendingRequest)
			{
				GuildMembersPending.Add(guildMemberInfo);
			}
			else
			{
				if (NumberMembers >= 20)
				{
					NotifyChange("MemberRemoved");
					return TWDModelResult.Error;
				}
				GuildMembers.Add(guildMemberInfo);
				guildMemberInfo.GuildJoinedDate = timestamp;
				SetFullParameter();
			}
			NotifyChange("MemberAdded");
			return TWDModelResult.OK;
		}

		public bool CanGiveGift()
		{
			return GuildMembers.Count > 1;
		}

		public TWDModelResult GiveGiftToMembers(string senderPlayerID, string senderName, DropType giftType, long expirationMs, string message, bool includeSelf = false)
		{
			GuildGift guildGift = new GuildGift();
			guildGift.Id = TimeStamp + "_" + senderPlayerID;
			guildGift.Type = giftType;
			guildGift.Creationtime = TimeStamp;
			guildGift.ExpireTime = TimeStamp + expirationMs;
			guildGift.GuildId = base.Id;
			guildGift.Claimed = false;
			guildGift.SenderId = senderPlayerID;
			guildGift.SenderName = senderName;
			guildGift.SenderMessage = message;
			guildGift.Recipients = new List<string>();
			foreach (GuildMemberInfo guildMember in GuildMembers)
			{
				if (guildMember.MemberId != senderPlayerID || includeSelf)
				{
					guildGift.Recipients.Add(guildMember.MemberId);
				}
			}
			if (AvailableGifts == null)
			{
				AvailableGifts = new List<GuildGift>();
			}
			AvailableGifts.Add(guildGift);
			NotifyChange("GiftSent");
			return TWDModelResult.OK;
		}

		public TWDModelResult CreateAd(string senderPlayerID, long expirationInSeconds, int bucket, string uniqueId)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(senderPlayerID);
			if (NumberMembers >= 20 || memberInfo == null || memberInfo.Role == GuildMemberRole.Normal)
			{
				return TWDModelResult.Error;
			}
			if (AdAvailableTimeSeconds <= 0)
			{
				if (JoinType == GuildJoinType.Closed)
				{
					JoinType = GuildJoinType.Invite;
				}
				AdCreatorId = senderPlayerID;
				AdCreationTimeStampSeconds = TimeStamp / 1000;
				AdExpireTimeStampSeconds = AdCreationTimeStampSeconds + expirationInSeconds;
				AdBucket = bucket;
				AdIdentifierForAnalytics = uniqueId;
				return TWDModelResult.OK;
			}
			return TWDModelResult.GuildAdStillRunning;
		}

		public TWDModelResult SetMemberLastActiveDate(string memberId, long memberUTCTimestamp)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(memberId);
			if (memberInfo != null)
			{
				if (FirstActiveDateTimeStamp == 0L)
				{
					FirstActiveDateTimeStamp = memberUTCTimestamp;
				}
				bool num = memberInfo.IsOnline(memberUTCTimestamp);
				long lastActiveDate = memberInfo.LastActiveDate;
				memberInfo.LastActiveDate = memberUTCTimestamp;
				if (num != memberInfo.IsOnline(memberUTCTimestamp) || lastActiveDate == 0L)
				{
					NotifyChange("MemberActivityStatusChanged", memberId);
				}
				return TWDModelResult.OK;
			}
			return TWDModelResult.Error;
		}

		public TWDModelResult DemoteLeader(float inactivityThreshold)
		{
			List<GuildMemberInfo> list = GuildMembers.FindAll((GuildMemberInfo m) => m.Role != GuildMemberRole.Leader && (float)(TimeStamp - m.LastActiveDate) < inactivityThreshold);
			if (list.Count == 0)
			{
				return TWDModelResult.Error;
			}
			list.StableSort(delegate(GuildMemberInfo a, GuildMemberInfo b)
			{
				int num = b.Role.CompareTo(a.Role);
				if (num == 0)
				{
					num = b.TotalChallengeStars.CompareTo(a.TotalChallengeStars);
				}
				if (num == 0)
				{
					num = b.LastActiveDate.CompareTo(a.LastActiveDate);
				}
				if (num == 0)
				{
					num = string.Compare(a.MemberId, b.MemberId, StringComparison.Ordinal);
				}
				return num;
			});
			GuildMemberInfo leaderMemberInfo = GetLeaderMemberInfo();
			list[0].Role = GuildMemberRole.Leader;
			leaderMemberInfo.Role = GuildMemberRole.Normal;
			AddNotificationMessage(ChatNotificationType.LeaderDemoted, string.Empty, string.Empty, leaderMemberInfo.MemberId, leaderMemberInfo.Name);
			AddNotificationMessage(ChatNotificationType.MemberPromotedToLeader, string.Empty, string.Empty, list[0].MemberId, list[0].Name);
			return TWDModelResult.OK;
		}

		public TWDModelResult SetUpNewLeader(float inactivityThreshold)
		{
			List<GuildMemberInfo> list = GuildMembers.FindAll((GuildMemberInfo m) => m.Role != GuildMemberRole.Leader && (float)(TimeStamp - m.LastActiveDate) < inactivityThreshold);
			if (list.Count == 0)
			{
				return TWDModelResult.Error;
			}
			list.StableSort(delegate(GuildMemberInfo a, GuildMemberInfo b)
			{
				int num = b.Role.CompareTo(a.Role);
				if (num == 0)
				{
					num = b.TotalChallengeStars.CompareTo(a.TotalChallengeStars);
				}
				if (num == 0)
				{
					num = b.LastActiveDate.CompareTo(a.LastActiveDate);
				}
				if (num == 0)
				{
					num = string.Compare(a.MemberId, b.MemberId, StringComparison.Ordinal);
				}
				return num;
			});
			list[0].Role = GuildMemberRole.Leader;
			AddNotificationMessage(ChatNotificationType.MemberPromotedToLeader, string.Empty, string.Empty, list[0].MemberId, list[0].Name);
			return TWDModelResult.OK;
		}

		public void ClearGuildAd()
		{
			AdCreationTimeStampSeconds = 0L;
			AdExpireTimeStampSeconds = 0L;
			AdCreatorId = "";
			AdIdentifierForAnalytics = "";
		}

		public GuildMemberInfo GetMemberInfo(string memberId)
		{
			for (int i = 0; i < GuildMembers.Count; i++)
			{
				if (GuildMembers[i].MemberId == memberId)
				{
					return GuildMembers[i];
				}
			}
			return null;
		}

		public GuildMemberInfo GetLeaderMemberInfo()
		{
			for (int i = 0; i < GuildMembers.Count; i++)
			{
				GuildMemberInfo guildMemberInfo = GuildMembers[i];
				if (guildMemberInfo.Role == GuildMemberRole.Leader)
				{
					return guildMemberInfo;
				}
			}
			return null;
		}

		public GuildMemberInfo GetMemberPendingInfo(string memberId)
		{
			for (int i = 0; i < GuildMembersPending.Count; i++)
			{
				if (GuildMembersPending[i].MemberId == memberId)
				{
					return GuildMembersPending[i];
				}
			}
			return null;
		}

		private void RemoveMember(string memberId)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(memberId);
			GuildMemberInfo guildMemberInfo = null;
			if (memberInfo == null)
			{
				guildMemberInfo = GetMemberPendingInfo(memberId);
			}
			if (memberInfo != null || guildMemberInfo != null)
			{
				if (memberInfo != null && GuildMembers.Contains(memberInfo))
				{
					GuildMembers.Remove(memberInfo);
				}
				if (guildMemberInfo != null && GuildMembersPending.Contains(guildMemberInfo))
				{
					GuildMembersPending.Remove(guildMemberInfo);
				}
				GuildBattleMatchmakingInfo.DeleteGuildBattlePlayerSnapshot(memberId);
				SetFullParameter();
			}
		}

		public TWDModelResult KickOutMember(string senderId, string targetId)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(targetId);
			if (memberInfo == null || memberInfo.Role == GuildMemberRole.Leader)
			{
				NotifyChange("MemberRemoved", targetId);
				return TWDModelResult.Error;
			}
			GuildMemberInfo memberInfo2 = GetMemberInfo(senderId);
			string senderName = ((memberInfo2 != null) ? memberInfo2.Name : "");
			AddNotificationMessage(ChatNotificationType.MemberKickedOut, senderId, senderName, targetId, memberInfo.Name);
			RemoveMember(targetId);
			SetFullParameter();
			NotifyChange("MemberRemoved", targetId);
			return TWDModelResult.OK;
		}

		public TWDModelResult SetMemberRole(string memberId, string senderId, GuildMemberRole newRole, ref bool isPromotion)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(memberId);
			GuildMemberInfo memberInfo2 = GetMemberInfo(senderId);
			bool flag = GetLeaderMemberInfo() == null;
			string senderName = ((memberInfo2 != null) ? memberInfo2.Name : "");
			if (memberInfo == null || memberInfo2 == null)
			{
				NotifyChange("MemberRemoved", memberId);
				return TWDModelResult.Error;
			}
			if (memberInfo2.Role != GuildMemberRole.Leader && memberInfo2.Role != GuildMemberRole.CoLeader && !flag)
			{
				NotifyChange("MemberRemoved", memberId);
				return TWDModelResult.Error;
			}
			if ((newRole == GuildMemberRole.Leader || newRole == GuildMemberRole.CoLeader) && memberInfo2.Role != GuildMemberRole.Leader && !flag)
			{
				return TWDModelResult.Error;
			}
			if (IsPromotion(memberInfo.Role, newRole))
			{
				isPromotion = true;
				AddNotificationMessage(ChatNotificationType.MemberPromoted, senderId, senderName, memberId, memberInfo.Name);
			}
			else if (IsDemotion(memberInfo.Role, newRole))
			{
				AddNotificationMessage(ChatNotificationType.MemberDemoted, senderId, senderName, memberId, memberInfo.Name);
			}
			memberInfo.Role = newRole;
			NotifyChange("MemberChanged", memberId);
			return TWDModelResult.OK;
		}

		public void AddPlayerNameChangedNotification(string playerHashedId, string newName, string oldName)
		{
			AddNotificationMessage(ChatNotificationType.MemberNameChanged, playerHashedId, oldName, playerHashedId, newName);
		}

		public void AddGuildNameChangedNotification(string playerHashedId, string name)
		{
			AddNotificationMessage(ChatNotificationType.ChangeGuildName, playerHashedId, name, playerHashedId, name);
		}

		public void AddPlayerRegisteredForBattleNotification(string playerHashedId, string name)
		{
			AddNotificationMessageGvg(ChatNotificationGvgType.MemberRegisteredForBattle, playerHashedId, name, playerHashedId, name);
		}

		public void AddPlayerResignedFromBattleNotification(string playerHashedId, string name)
		{
			AddNotificationMessageGvg(ChatNotificationGvgType.MemberResignedFromBattle, playerHashedId, name, playerHashedId, name);
		}

		public bool IsPromotion(GuildMemberRole oldRole, GuildMemberRole newRole)
		{
			return oldRole < newRole;
		}

		public bool IsDemotion(GuildMemberRole oldRole, GuildMemberRole newRole)
		{
			return oldRole > newRole;
		}

		public TWDModelResult AcceptMemberRequest(string senderId, string targetId, long timestamp)
		{
			GuildMemberInfo memberPendingInfo = GetMemberPendingInfo(targetId);
			GuildMemberInfo memberInfo = GetMemberInfo(senderId);
			string senderName = ((memberInfo != null) ? memberInfo.Name : "");
			if (memberPendingInfo == null || memberPendingInfo.State != GuildMemberState.PendingRequest)
			{
				NotifyChange("MemberRemoved", targetId);
				return TWDModelResult.Error;
			}
			if (NumberMembers >= 20)
			{
				RemoveMember(targetId);
				NotifyChange("MemberRefused", targetId);
				return TWDModelResult.Error;
			}
			memberPendingInfo.State = GuildMemberState.Normal;
			memberPendingInfo.GuildJoinedDate = timestamp;
			GuildMembers.Add(memberPendingInfo);
			GuildMembersPending.Remove(memberPendingInfo);
			SetFullParameter();
			SetMemberLastActiveDate(targetId, TimeStamp);
			NotifyChange("MemberAccepted", targetId);
			AddNotificationMessage(ChatNotificationType.MemberAccepted, senderId, senderName, targetId, memberPendingInfo.Name);
			return TWDModelResult.OK;
		}

		public TWDModelResult RefuseMemberRequest(string playerId)
		{
			GuildMemberInfo memberPendingInfo = GetMemberPendingInfo(playerId);
			if (memberPendingInfo == null || memberPendingInfo.State != GuildMemberState.PendingRequest)
			{
				NotifyChange("MemberRemoved", playerId);
				return TWDModelResult.Error;
			}
			RemoveMember(playerId);
			SetFullParameter();
			NotifyChange("MemberRefused", playerId);
			return TWDModelResult.OK;
		}

		public TWDModelResult LeaveMember(string senderId, string targetId)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(targetId);
			if (memberInfo != null)
			{
				GuildMemberInfo memberInfo2 = GetMemberInfo(senderId);
				string senderName = ((memberInfo2 != null) ? memberInfo2.Name : "");
				AddNotificationMessage(ChatNotificationType.MemberLeft, senderId, senderName, targetId, memberInfo.Name);
			}
			RemoveMember(targetId);
			SetFullParameter();
			NotifyChange("MemberRemoved", targetId);
			return TWDModelResult.OK;
		}

		private void clearExpiredAndRegisteredGifts()
		{
			if (AvailableGifts == null)
			{
				return;
			}
			List<GuildGift> list = new List<GuildGift>();
			for (int i = 0; i < AvailableGifts.Count; i++)
			{
				GuildGift guildGift = AvailableGifts[i];
				List<string> list2 = new List<string>();
				for (int j = 0; j < guildGift.Recipients.Count; j++)
				{
					string text = guildGift.Recipients[j];
					if (string.IsNullOrEmpty(text) || GetMemberInfo(text) == null)
					{
						list2.Add(text);
					}
				}
				for (int k = 0; k < list2.Count; k++)
				{
					guildGift.Recipients.Remove(list2[k]);
				}
				bool flag = guildGift.ExpireTime > -1 && guildGift.ExpireTime < TimeStamp;
				if (guildGift.Recipients.Count < 1 || flag)
				{
					list.Add(guildGift);
				}
			}
			for (int l = 0; l < list.Count; l++)
			{
				GuildGift item = list[l];
				AvailableGifts.Remove(item);
			}
		}

		private void clearExpiredAd()
		{
			if (AdExpireTimeStampSeconds > 0 && AdExpireTimeStampSeconds < TimeStamp / 1000)
			{
				ClearGuildAd();
			}
		}

		public void AddChatMessage(ChatMessage message)
		{
			ChatMessages.Add(message);
			NotifyChange("MessageAdded");
			if (ChatMessages.Count > 50)
			{
				ChatMessages.RemoveAt(ChatMessages[0].IsPinned ? 1 : 0);
				NotifyChange("MessagesTruncated");
			}
		}

		protected void AddNotificationMessage(ChatNotificationType type, string senderId, string senderName, string targetId, string targetName)
		{
			ChatMessage message = new ChatMessage
			{
				NotificationType = type,
				Name = targetName,
				GuildId = base.Id,
				Time = base.LifeTime,
				PlayerId = targetId,
				SenderId = senderId,
				SenderName = senderName
			};
			AddChatMessage(message);
		}

		protected void AddNotificationMessageGvg(ChatNotificationGvgType type, string senderId, string senderName, string targetId, string targetName)
		{
			ChatMessage message = new ChatMessage
			{
				NotificationGvGType = type,
				Name = targetName,
				GuildId = base.Id,
				Time = base.LifeTime,
				PlayerId = targetId,
				SenderId = senderId,
				SenderName = senderName
			};
			AddChatMessage(message);
		}

		public int GetActiveMembers()
		{
			return GuildMembers.Count;
		}

		public void StartChallenge(string newChallengeId)
		{
			if (!(CurrentChallengeId == newChallengeId) && !ChallengeStars.ContainsKey(newChallengeId))
			{
				NumberChallengeStarted++;
				CurrentChallengeId = newChallengeId;
				PreviousChallengeStars = CurrentChallengeStars;
				if (NumberMembers > 0)
				{
					PreviousChallengeStarsPerMember = PreviousChallengeStars / NumberMembers;
				}
				else
				{
					PreviousChallengeStarsPerMember = 0;
				}
				HighestChallengeStarsCount = Math.Max(HighestChallengeStarsCount, PreviousChallengeStars);
				CurrentChallengeStars = 0;
				if (CurrentChallengeMemberInfos == null)
				{
					CurrentChallengeMemberInfos = new Dictionary<string, int>();
				}
				CurrentChallengeMemberInfos.Clear();
				for (int i = 0; i < GuildMembers.Count; i++)
				{
					GuildMembers[i].PreviousChallengeStars = GuildMembers[i].CurrentChallengeStars;
					GuildMembers[i].HighestChallengeStars = Math.Max(GuildMembers[i].HighestChallengeStars, GuildMembers[i].CurrentChallengeStars);
					GuildMembers[i].CurrentChallengeStars = 0;
					GuildMembers[i].ExcludedFromChallenge = false;
					GuildMembers[i].TotalChallengeStarsAtChallengeStart = GuildMembers[i].TotalChallengeStars;
				}
				ChallengeStars.Add(CurrentChallengeId, 0);
			}
		}

		public TWDModelResult SetChallengeStars(string challengeId, string memberId, int currentChallengeStars, bool isChallengeEnded)
		{
			if (challengeId != CurrentChallengeId)
			{
				return TWDModelResult.ChallengeMismatch;
			}
			GuildMemberInfo memberInfo = GetMemberInfo(memberId);
			if (memberInfo == null)
			{
				return TWDModelResult.MemberNotFound;
			}
			if (memberInfo.TotalChallengeStarsAtChallengeStart == 0)
			{
				memberInfo.TotalChallengeStarsAtChallengeStart = memberInfo.TotalChallengeStars - memberInfo.CurrentChallengeStars;
			}
			int currentChallengeStars2 = memberInfo.CurrentChallengeStars;
			memberInfo.CurrentChallengeStars = currentChallengeStars;
			memberInfo.HighestChallengeStars = Math.Max(memberInfo.HighestChallengeStars, memberInfo.CurrentChallengeStars);
			memberInfo.TotalChallengeStars = memberInfo.TotalChallengeStarsAtChallengeStart + currentChallengeStars;
			int num = Math.Max(0, memberInfo.CurrentChallengeStars - currentChallengeStars2);
			if (CurrentChallengeMemberInfos != null && !IsParticipant(memberInfo) && CurrentChallengeMemberInfos.Count >= 20)
			{
				memberInfo.ExcludedFromChallenge = true;
				return TWDModelResult.OK;
			}
			CurrentChallengeMemberInfos[memberInfo.MemberId] = currentChallengeStars;
			if (!ChallengeStars.ContainsKey(CurrentChallengeId))
			{
				ChallengeStars.Add(CurrentChallengeId, 0);
			}
			int num2 = 0;
			foreach (KeyValuePair<string, int> currentChallengeMemberInfo in CurrentChallengeMemberInfos)
			{
				num2 += currentChallengeMemberInfo.Value;
			}
			ChallengeStars[CurrentChallengeId] = num2;
			if (RotationAccumulatedStars == null)
			{
				RotationAccumulatedStars = new List<int> { 0 };
			}
			if (RotationAccumulatedMissionCount == null)
			{
				RotationAccumulatedMissionCount = new List<int> { 0 };
			}
			if (LastNewDayScoreTimeStamp <= 0)
			{
				RotationAccumulatedStars[RotationAccumulatedStars.Count - 1] += CurrentChallengeStars;
				if (CurrentChallengeStars > 0)
				{
					RotationAccumulatedMissionCount[RotationAccumulatedMissionCount.Count - 1]++;
				}
				LastNewDayScoreTimeStamp = TimeStamp;
			}
			long num3 = (TimeStamp - LastNewDayScoreTimeStamp) / 86400000;
			if (isChallengeEnded)
			{
				num3 = 0L;
			}
			if (num3 > 0)
			{
				LastNewDayScoreTimeStamp = TimeStamp;
				int num4 = Math.Min((int)num3, GuildLeaderboardScoreBufferSize);
				for (int i = 0; i < num4; i++)
				{
					RotationAccumulatedStars.Add(0);
					RotationAccumulatedMissionCount.Add(0);
				}
				int num5 = Math.Max(0, RotationAccumulatedStars.Count - GuildLeaderboardScoreBufferSize);
				if (num5 > 0)
				{
					RotationAccumulatedStars.RemoveRange(0, num5);
					RotationAccumulatedMissionCount.RemoveRange(0, num5);
				}
			}
			if (RotationAccumulatedStars.Count > 0)
			{
				RotationAccumulatedStars[RotationAccumulatedStars.Count - 1] += num;
			}
			if (RotationAccumulatedMissionCount.Count > 0)
			{
				RotationAccumulatedMissionCount[RotationAccumulatedMissionCount.Count - 1]++;
			}
			if (RotationAccumulatedStars.Count != RotationAccumulatedMissionCount.Count)
			{
				if (RotationAccumulatedStars.Count > RotationAccumulatedMissionCount.Count)
				{
					RotationAccumulatedStars.RemoveRange(0, RotationAccumulatedStars.Count - RotationAccumulatedMissionCount.Count);
				}
				else
				{
					RotationAccumulatedMissionCount.RemoveRange(0, RotationAccumulatedMissionCount.Count - RotationAccumulatedStars.Count);
				}
			}
			CurrentChallengeStars = ChallengeStars[CurrentChallengeId];
			HighestChallengeStarsCount = Math.Max(HighestChallengeStarsCount, CurrentChallengeStars);
			int num6 = 0;
			foreach (KeyValuePair<string, int> challengeStar in ChallengeStars)
			{
				num6 += challengeStar.Value;
			}
			TotalChallengeStars = num6;
			NotifyChange("StarsAdded");
			return TWDModelResult.OK;
		}

		private bool IsParticipant(GuildMemberInfo info)
		{
			if (info != null)
			{
				return CurrentChallengeMemberInfos.ContainsKey(info.MemberId);
			}
			return false;
		}

		public TWDModelResult DEBUG_cheatExpireAd(long timeLeftSeconds)
		{
			timeLeftSeconds = Math.Max(0L, timeLeftSeconds);
			AdExpireTimeStampSeconds = TimeStamp / 1000 + timeLeftSeconds;
			return TWDModelResult.OK;
		}

		public bool DEBUG_cheatSimulateNewStarsInANewDay(string memberId, int elapsedDays, int stars, int totalPlayedMissions)
		{
			if (GetMemberInfo(memberId) == null)
			{
				return false;
			}
			if (RotationAccumulatedStars == null)
			{
				RotationAccumulatedStars = new List<int> { 0 };
			}
			if (RotationAccumulatedMissionCount == null)
			{
				RotationAccumulatedMissionCount = new List<int> { 0 };
			}
			LastNewDayScoreTimeStamp = TimeStamp;
			int num = Math.Min(elapsedDays, GuildLeaderboardScoreBufferSize);
			for (int i = 0; i < num; i++)
			{
				RotationAccumulatedStars.Add(0);
				RotationAccumulatedMissionCount.Add(0);
			}
			int num2 = Math.Max(0, RotationAccumulatedStars.Count - GuildLeaderboardScoreBufferSize);
			if (num2 > 0)
			{
				RotationAccumulatedStars.RemoveRange(0, num2);
				RotationAccumulatedMissionCount.RemoveRange(0, num2);
			}
			RotationAccumulatedStars[RotationAccumulatedStars.Count - 1] += stars;
			RotationAccumulatedMissionCount[RotationAccumulatedMissionCount.Count - 1] += totalPlayedMissions;
			NotifyChange("StarsAdded");
			return true;
		}

		private void ResizeScoreListForElapsedDays(ref List<int> accumulatedStars, ref List<int> accumulatedMissionCount, long elapsedDays)
		{
			if (elapsedDays <= 0 || (accumulatedStars == null && accumulatedMissionCount == null))
			{
				return;
			}
			int num = Math.Min((int)elapsedDays, GuildLeaderboardScoreBufferSize);
			for (int i = 0; i < num; i++)
			{
				if (accumulatedStars != null)
				{
					accumulatedStars.Add(0);
				}
				if (accumulatedMissionCount != null)
				{
					accumulatedMissionCount.Add(0);
				}
			}
			int num2 = accumulatedStars?.Count ?? accumulatedMissionCount.Count;
			int num3 = Math.Max(0, num2 - GuildLeaderboardScoreBufferSize);
			if (num3 > 0)
			{
				if (accumulatedStars != null)
				{
					accumulatedStars.RemoveRange(0, num3);
				}
				if (accumulatedMissionCount != null)
				{
					accumulatedMissionCount.RemoveRange(0, num3);
				}
			}
		}

		public int GetChallengeStars(string challengeId)
		{
			if (challengeId == CurrentChallengeId)
			{
				return CurrentChallengeStars;
			}
			if (challengeId != null && ChallengeStars.ContainsKey(challengeId))
			{
				return ChallengeStars[challengeId];
			}
			return 0;
		}

		public int GetUnreadChatAmount(string memberId, long lastReadTime)
		{
			int num = 0;
			if (GetMemberInfo(memberId) == null)
			{
				return 0;
			}
			for (int i = 0; i < ChatMessages.Count; i++)
			{
				if (ChatMessages[i].Time > lastReadTime && ChatMessages[i].PlayerId != memberId && ChatMessages[i].IsBothTypesNone)
				{
					num++;
				}
			}
			return num;
		}

		public long GetLastChatTime()
		{
			long num = 0L;
			for (int i = 0; i < ChatMessages.Count; i++)
			{
				if (ChatMessages[i].Time > num)
				{
					num = ChatMessages[i].Time;
				}
			}
			return num;
		}

		public static List<GuildMemberInfo> GetGuildMembersOrderedByScore(GuildModel guildModel)
		{
			List<GuildMemberInfo> list = new List<GuildMemberInfo>();
			for (int i = 0; i < guildModel.GuildMembers.Count; i++)
			{
				if (guildModel.GuildMembers[i] != null)
				{
					list.Add(guildModel.GuildMembers[i]);
				}
			}
			list.StableSort((GuildMemberInfo a, GuildMemberInfo b) => (a != null && b != null) ? Math.Sign((float)b.PreviousChallengeStars - (float)a.PreviousChallengeStars) : 0);
			return list;
		}

		public void EndCurrentGuildBattle()
		{
			GuildWarModel?.EndBattle();
			GvGSeasonModel.UpdateStatsFromLastBattle();
			GuildInfoCurrentVP += GvGSeasonModel.GuildWarModel.CurrentBattle.FinalVictoryPoints;
			GuildBattleMatchmakingInfo.UpdateInfoOnEndBattle(GuildBattleTier, CurrentVictoryPoints);
			GvGSeasonModel.AddToBattleLog(GuildWarModel.CurrentBattle);
			GuildWarModel.NotifyChange("GuildBattleEnded");
		}

		public void UpdateGuildBattleLeaderboards(TWDModelManager manager, string senderId, string guildId, string guildName, bool battleEnd = false, bool updateMembers = true, bool updateSectors = false)
		{
			if (GvGSeasonModel == null || GuildWarModel == null)
			{
				return;
			}
			GuildBattleModel currentBattle = GuildWarModel.CurrentBattle;
			if (currentBattle == null || string.IsNullOrEmpty(currentBattle.BattleId))
			{
				return;
			}
			IServerService serverService = manager.ServerService;
			if (HelpersModel.IsOfflineMode)
			{
				currentBattle.FetchBattleHighscores(TimeStamp, manager, forceBroadcast: true, forceUpdate: true, updateGuildBattleResults: true);
			}
			else
			{
				if (serverService == null || (!string.IsNullOrEmpty(senderId) && senderId != manager.GetPlayer().HashedId))
				{
					return;
				}
				bool flag = true;
				if (!battleEnd)
				{
					string guildBattleLiveScoreLeaderboardName = Leaderboards.GetGuildBattleLiveScoreLeaderboardName(currentBattle.BattleId, currentBattle.CurrentMapModel.RandomSeed);
					List<LeaderboardEntry> leaderboard = serverService.GetLeaderboard(guildBattleLiveScoreLeaderboardName, "2");
					if (leaderboard != null)
					{
						for (int i = 0; i < leaderboard.Count; i++)
						{
							LeaderboardEntry leaderboardEntry = leaderboard[i];
							if (leaderboardEntry.Tags != null && leaderboardEntry.Tags.Contains(Leaderboards.GvgEndBattleTag))
							{
								flag = false;
								break;
							}
						}
					}
				}
				if (!flag)
				{
					return;
				}
				UpdateGuildGvGLeaderboards(serverService, manager, battleEnd, updateOnlyFailedSaves: false);
				if (string.IsNullOrEmpty(senderId) || !updateMembers)
				{
					return;
				}
				if (updateSectors)
				{
					int num = currentBattle.VictoryPointsSectorRewardPerSector.Sum((KeyValuePair<int, int> keyValue) => keyValue.Value);
					if (num > 0)
					{
						LeaderboardEntry entry = Leaderboards.CreateGuildBattlePlayersScoreLeaderboardEntry(manager, currentBattle.BattleId, guildId, guildId, null, num);
						serverService.SaveLeaderboardEntry(currentBattle.BattleId, entry);
					}
				}
				PlayerModel player = manager.Player;
				if (currentBattle.IsPlayerRegistered(player.HashedId))
				{
					string playerEmblem = manager.GetMessageSerializer().Serialize(player.PlayerEmblem);
					int totalVictoryPointsForPlayer = currentBattle.GetTotalVictoryPointsForPlayer(player.HashedId);
					LeaderboardEntry entry2 = Leaderboards.CreateGuildBattlePlayersScoreLeaderboardEntry(manager, player.Name, senderId, guildId, playerEmblem, totalVictoryPointsForPlayer);
					serverService.SaveLeaderboardEntry(currentBattle.BattleId, entry2);
				}
				currentBattle.FetchBattleHighscores(TimeStamp, manager, forceBroadcast: true, forceUpdate: true, updateGuildBattleResults: true);
				NotifyOpponentGuildBattleHighscoresChanged(manager, currentBattle, guildId);
			}
		}

		private void NotifyOpponentGuildBattleHighscoresChanged(TWDModelManager manager, GuildBattleModel currentBattle, string sourceGuildId)
		{
			if (manager.ServerService != null && currentBattle != null && !currentBattle.IsFakeBattle && currentBattle.EnemyGuildData != null)
			{
				List<string> registeredPlayersList = currentBattle.EnemyGuildData.RegisteredPlayersList;
				if (registeredPlayersList != null && registeredPlayersList.Count != 0)
				{
					manager.ServerService.NotifyGuildBattleHighscoresChanged(new GuildBattleHighscoresChangedNotification
					{
						BattleId = currentBattle.BattleId,
						WarId = currentBattle.WarId,
						SourceGuildId = sourceGuildId,
						TargetGuildId = currentBattle.EnemyGuildData.GroupId,
						Timestamp = TimeStamp
					}, new List<string>(registeredPlayersList));
				}
			}
		}

		public void AccumulatePlayerTotalVp(TWDModelManager twdManager, string playerHashedId, int deltaVp)
		{
			if (GvGSeasonModel != null && GuildWarModel?.CurrentBattle != null)
			{
				Dictionary<string, int> target = TotalAllTimeGvGVpAccumulatedPerPlayer;
				AccumulateToTarget(ref target, playerHashedId, deltaVp);
				target = GvGSeasonModel.SeasonTotalVpAccumulatedPerPlayer;
				AccumulateToTarget(ref target, playerHashedId, deltaVp);
				target = GuildWarModel.WarTotalVpAccumulatedPerPlayer;
				AccumulateToTarget(ref target, playerHashedId, deltaVp);
				if (GuildWarModel.CurrentBattle.NotInPvP(playerHashedId))
				{
					twdManager.GvGLogError($"AccumulatePlayerTotalVp:{playerHashedId}-{deltaVp}");
					return;
				}
				target = GuildWarModel.CurrentBattle.VictoryPointsPerPlayer;
				AccumulateToTarget(ref target, playerHashedId, deltaVp);
			}
		}

		public int GetAllTimeVpTotalForPlayer(string playerHashedId)
		{
			if (TotalAllTimeGvGVpAccumulatedPerPlayer == null)
			{
				return 0;
			}
			TotalAllTimeGvGVpAccumulatedPerPlayer.TryGetValue(playerHashedId, out var value);
			return value;
		}

		private static void AccumulateToTarget(ref Dictionary<string, int> target, string key, int deltaAmount)
		{
			if (target == null)
			{
				target = new Dictionary<string, int>();
			}
			if (target.ContainsKey(key))
			{
				target[key] += deltaAmount;
			}
			else
			{
				target.Add(key, deltaAmount);
			}
		}

		private int CalculateCurrentOngoingBattle(int warId)
		{
			int num = 0;
			if (GuildWarModel.CurrentBattle.WarId == warId && GuildWarModel.CurrentBattle.CurrentState == GuildBattleModel.GuildBattleState.Started)
			{
				num += GuildWarModel.CurrentBattle.CalculateTotalVictoryPoints();
			}
			return num;
		}

		private void InitializeGuildBattleMatchmakingInfo()
		{
			if (GuildBattleMatchmakingInfo == null)
			{
				GuildBattleMatchmakingInfo = new GuildBattleMatchmakingInfo
				{
					GroupId = base.Id
				};
				GuildBattleMatchmakingInfo.UpdateInfoOnEndBattle(GuildBattleTier, CurrentVictoryPoints);
				GuildBattleMatchmakingInfo.GuildName = base.Name;
			}
		}

		public string GetDebugInfo()
		{
			return $"Guild : [SeasonID : {GvGSeasonModel.SeasonDefinitionId}, WarID : {GuildWarModel.WarDefinitionId}, BattleTimeSlot : {GuildWarModel.CurrentBattle.TimeSlot}, BattleId : {GuildWarModel.CurrentBattle.BattleId}]";
		}

		public TWDModelResult UpdateTotalVp(string playerId, int totalVpPoints)
		{
			GuildMemberInfo memberInfo = GetMemberInfo(playerId);
			if (memberInfo != null)
			{
				memberInfo.TotalVP = totalVpPoints;
				return TWDModelResult.OK;
			}
			return TWDModelResult.Skip;
		}

		public bool UpdateMatchmakingVersion(int newVersion, ref GuildBattleMatchmakingInfo guildBattleMatchmakingInfo)
		{
			if (newVersion <= MatchmakingVersion)
			{
				return false;
			}
			MatchmakingVersion = newVersion;
			return true;
		}

		public void UpdateGuildGvGLeaderboards(IServerService serverService, TWDModelManager manager, bool battleEnd, bool updateOnlyFailedSaves)
		{
			int warDefinitionId = GvGSeasonModel.GuildWarModel.WarDefinitionId;
			int num = CalculateCurrentOngoingBattle(warDefinitionId);
			bool flag = updateOnlyFailedSaves || !LeaderboardUpdated || !GvGSeasonModel.LeaderboardUpdated || !GuildWarModel.LeaderboardUpdated || !GuildWarModel.CurrentBattle.LeaderboardUpdated;
			if (!updateOnlyFailedSaves || !GuildWarModel.CurrentBattle.LeaderboardUpdated)
			{
				if (GuildWarModel.CurrentBattle.HasStarted())
				{
					LeaderboardEntry leaderboardEntry = Leaderboards.CreateGuildBattleLiveScoreLeaderboardEntry(manager, base.Id, base.Name, battleEnd);
					leaderboardEntry.Score = GuildWarModel.CurrentBattle.CalculateTotalVictoryPoints();
					flag |= !GuildWarModel.CurrentBattle.SaveLeaderboard(serverService, leaderboardEntry);
				}
				else
				{
					GuildWarModel.CurrentBattle.LeaderboardUpdated = true;
				}
			}
			if (manager.GameEconomyData.GetFeature("GvgGuildLeaderboardConstantUpdate").Enabled || battleEnd || updateOnlyFailedSaves)
			{
				if (!updateOnlyFailedSaves || !GuildWarModel.LeaderboardUpdated)
				{
					LeaderboardEntry leaderboardEntry = Leaderboards.CreateGuildBattleLiveScoreLeaderboardEntry(manager, base.Id, base.Name);
					leaderboardEntry.Score = GvGSeasonModel.CalculateBattleLogTotalScoreForWar(warDefinitionId) + num;
					flag |= !GuildWarModel.SaveLeaderboard(serverService, leaderboardEntry);
				}
				if (!updateOnlyFailedSaves || !GvGSeasonModel.LeaderboardUpdated)
				{
					LeaderboardEntry leaderboardEntry = Leaderboards.CreateGuildBattleLiveScoreLeaderboardEntry(manager, base.Id, base.Name);
					leaderboardEntry.Score = GvGSeasonModel.CalculateBattleLogTotalScoreForSeason() + num;
					flag |= !GvGSeasonModel.SaveLeaderboard(serverService, leaderboardEntry);
				}
				if (!updateOnlyFailedSaves || !LeaderboardUpdated)
				{
					LeaderboardEntry leaderboardEntry = Leaderboards.CreateGuildBattleLiveScoreLeaderboardEntry(manager, base.Id, base.Name);
					leaderboardEntry.Score = TotalAllTimeAccumulatedVp + num;
					flag |= !SaveLeaderboard(serverService, leaderboardEntry);
				}
			}
			if (flag)
			{
				UpdateLeaderboardSaveStateGroupCommand command = new UpdateLeaderboardSaveStateGroupCommand(LeaderboardUpdated, GvGSeasonModel.LeaderboardUpdated, GuildWarModel.LeaderboardUpdated, GuildWarModel.CurrentBattle.LeaderboardUpdated, TimeStamp);
				HelpersModel.ExecuteGroupCommand(manager, command);
			}
		}

		public bool SaveLeaderboard(IServerService serverService, LeaderboardEntry entry)
		{
			return LeaderboardUpdated = serverService.TrySaveLeaderboardEntry(LeaderboardName, entry);
		}

		public void BanPlayer(string playerHashedId, long until, long currentTime)
		{
			GuildBanModel.Ban(playerHashedId, until, currentTime);
		}

		public bool IsBanned(string playerHashedId, long currentTime)
		{
			if (JoinType == GuildJoinType.Open)
			{
				return GuildBanModel.IsBanned(playerHashedId, currentTime);
			}
			return false;
		}
	}
}
