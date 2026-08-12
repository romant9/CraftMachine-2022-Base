using System.Threading.Tasks;

namespace BaseModel
{
	public interface IServerServiceV1
	{
		void Save(SaveType saveType, bool skipClearVisited = false);

		bool ValidateReceipt(string transactionId);

		bool ValidateReceiptV2(string purchaseInfoRequest);

		void SaveReplay(string guid, string replayData);

		void SendGroupCommand(string groupId, JsonCommand jsonCommand);

		void SaveGroupModel(string groupId);

		void JoinGroup(string groupId, string memberHashedId, JsonCommand jsonCommand);

		void AddGroupMember(string groupId, string memberHashedId, JsonCommand jsonCommand);

		void RemoveGroupMember(string groupId, string memberHashedId, JsonCommand jsonCommand);

		void DisbandGroup(string groupId, JsonCommand jsonCommand);

		void CreateSteamOrder(ulong orderId, string steamId, uint itemCount, string gLanguage, string gCurrency, uint gItemId, int gQuy, int gAmount, string gDescription);

		Task<string> GetSteamUserInfo(string steamId);

		bool ValidateGooglePlaySubscriptionV2();
	}
}
