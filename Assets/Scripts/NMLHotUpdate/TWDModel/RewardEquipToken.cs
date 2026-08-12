namespace TWDModel
{
	public class RewardEquipToken : IReward
	{
		public string EquipTokenId { get; set; }

		public int RewardAmount { get; set; }

		public EquipTokenItemModel GivenEquipmentToken { get; private set; }

		public RewardType Type => RewardType.EquipToken;

		public object Give(TWDModelManager manager, object[] param = null)
		{
			return GivenEquipmentToken = manager.Player.EquipTokenContainer.AddEquipToken(EquipTokenId, RewardAmount);
		}

		public EquipTokenItemModel FakeRewardEquipTokenItemModel(TWDModelManager manager)
		{
			EquipTokenItemModel equipTokenItemModel = new EquipTokenItemModel(EquipTokenId, RewardAmount);
			equipTokenItemModel.SetManager(manager);
			equipTokenItemModel.Initialize();
			return equipTokenItemModel;
		}
	}
}
