using System;
using BaseModel;

namespace TWDModel
{
	public class BananaPopupCommand : ModelCommand
	{
		public ShopRoleType Platform { get; private set; }

		public BananaPopupCommand()
		{
		}

		public BananaPopupCommand(ShopRoleType platform)
		{
			Platform = platform;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			DateTime utcTime = tWDModelManager.Player.UtcTime;
			bool flag = false;
			if (Platform == ShopRoleType.IOS && tWDModelManager.GameEconomyData?.ConfigData?.IsInCountryControlIOS(tWDModelManager.Player.Country) != true)
			{
				flag = true;
			}
			if (flag)
			{
				if (tWDModelManager.Player.CouncilLevel >= tWDModelManager.GameEconomyData.ConfigData.BananaPopupLimitCouncilLevelIOS && tWDModelManager.Player.TotalUSDSpent >= tWDModelManager.GameEconomyData.ConfigData.RechargeLimitWBIOS && tWDModelManager.GameEconomyData.ConfigData.InBananaTimeIOS(utcTime))
				{
					long bananaStartTimeIOS = tWDModelManager.GameEconomyData.ConfigData.BananaStartTimeIOS;
					long bananaPopupFreLongIOS = tWDModelManager.GameEconomyData.ConfigData.BananaPopupFreLongIOS;
					long num = (utcTime.TotalMilliseconds() - bananaStartTimeIOS) / bananaPopupFreLongIOS * bananaPopupFreLongIOS + bananaStartTimeIOS;
					if (tWDModelManager.Player.WebShopPopupLastFreshTime != num)
					{
						tWDModelManager.Player.WebShopPopupLastFreshTime = num;
						tWDModelManager.Player.WebShopPopupTimes = 1;
						tWDModelManager.Player.WebShopPopupLastTime = utcTime.TotalMilliseconds();
						return new NGModelCommandRespond(this, TWDModelResult.OK);
					}
					if (tWDModelManager.Player.WebShopPopupLastTime + tWDModelManager.GameEconomyData.ConfigData.BananaPopupFreShortIOS <= utcTime.TotalMilliseconds() && tWDModelManager.Player.WebShopPopupTimes < tWDModelManager.GameEconomyData.ConfigData.BananaPopupFreTimesIOS)
					{
						tWDModelManager.Player.WebShopPopupLastTime = utcTime.TotalMilliseconds();
						tWDModelManager.Player.WebShopPopupTimes++;
						return new NGModelCommandRespond(this, TWDModelResult.OK);
					}
				}
				return new NGModelCommandRespond(this, TWDModelResult.Skip);
			}
			if (tWDModelManager.Player.CouncilLevel >= tWDModelManager.GameEconomyData.ConfigData.BananaPopupLimitCouncilLevel && tWDModelManager.Player.TotalUSDSpent >= tWDModelManager.GameEconomyData.ConfigData.RechargeLimitWB && tWDModelManager.GameEconomyData.ConfigData.InBananaTime(utcTime))
			{
				long bananaStartTime = tWDModelManager.GameEconomyData.ConfigData.BananaStartTime;
				long bananaPopupFreLong = tWDModelManager.GameEconomyData.ConfigData.BananaPopupFreLong;
				long num2 = (utcTime.TotalMilliseconds() - bananaStartTime) / bananaPopupFreLong * bananaPopupFreLong + bananaStartTime;
				if (tWDModelManager.Player.WebShopPopupLastFreshTime != num2)
				{
					tWDModelManager.Player.WebShopPopupLastFreshTime = num2;
					tWDModelManager.Player.WebShopPopupTimes = 1;
					tWDModelManager.Player.WebShopPopupLastTime = utcTime.TotalMilliseconds();
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
				if (tWDModelManager.Player.WebShopPopupLastTime + tWDModelManager.GameEconomyData.ConfigData.BananaPopupFreShort <= utcTime.TotalMilliseconds() && tWDModelManager.Player.WebShopPopupTimes < tWDModelManager.GameEconomyData.ConfigData.BananaPopupFreTimes)
				{
					tWDModelManager.Player.WebShopPopupLastTime = utcTime.TotalMilliseconds();
					tWDModelManager.Player.WebShopPopupTimes++;
					return new NGModelCommandRespond(this, TWDModelResult.OK);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.Skip);
		}
	}
}
