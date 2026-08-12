using System;
using System.Collections.Generic;

namespace Singular
{
	public class SingularAdData : Dictionary<string, object>
	{
		public static class AdPlatforms
		{
			public static readonly string MOPUB = "mopub";
		}

		private const string ADMON_IS_ADMON_REVENUE = "is_admon_revenue";

		private const string ADMON_AD_PLATFORM = "ad_platform";

		private const string ADMON_CURRENCY = "ad_currency";

		private const string ADMON_REVENUE = "ad_revenue";

		private const string ADMON_NETWORK_NAME = "ad_mediation_platform";

		private const string ADMON_AD_TYPE = "ad_type";

		private const string ADMON_AD_GROUP_TYPE = "ad_group_type";

		private const string ADMON_IMPRESSION_ID = "ad_impression_id";

		private const string ADMON_AD_PLACEMENT_NAME = "ad_placement_name";

		private const string ADMON_AD_UNIT_ID = "ad_unit_id";

		private const string ADMON_AD_UNIT_NAME = "ad_unit_name";

		private const string ADMON_AD_GROUP_ID = "ad_group_id";

		private const string ADMON_AD_GROUP_NAME = "ad_group_name";

		private const string ADMON_AD_GROUP_PRIORITY = "ad_group_priority";

		private const string ADMON_PRECISION = "ad_precision";

		private const string ADMON_PLACEMENT_ID = "ad_placement_id";

		private const string IS_REVENUE_EVENT_KEY = "is_revenue_event";

		private const string REVENUE_AMOUNT_KEY = "r";

		private const string REVENUE_CURRENCY_KEY = "pcc";

		private readonly string[] RequiredParams = new string[3] { "ad_platform", "ad_currency", "ad_revenue" };

		public SingularAdData(string adPlatform, string currency, double revenue)
		{
			SetValue("ad_platform", adPlatform);
			SetValue("ad_currency", currency);
			SetValue("pcc", currency);
			SetValue("ad_revenue", revenue);
			SetValue("r", revenue);
			SetValue("is_admon_revenue", true);
			SetValue("is_revenue_event", true);
			SetValue("ad_mediation_platform", adPlatform);
		}

		public SingularAdData WithNetworkName(string networkName)
		{
			SetValue("ad_mediation_platform", networkName);
			return this;
		}

		public SingularAdData WithAdType(string adType)
		{
			SetValue("ad_type", adType);
			return this;
		}

		public SingularAdData WithAdGroupType(string adGroupType)
		{
			SetValue("ad_group_type", adGroupType);
			return this;
		}

		public SingularAdData WithImpressionId(string impressionId)
		{
			SetValue("ad_impression_id", impressionId);
			return this;
		}

		public SingularAdData WithAdPlacmentName(string adPlacementName)
		{
			SetValue("ad_placement_name", adPlacementName);
			return this;
		}

		public SingularAdData WithAdUnitId(string adUnitId)
		{
			SetValue("ad_unit_id", adUnitId);
			return this;
		}

		public SingularAdData WithAdUnitName(string adUnitName)
		{
			SetValue("ad_unit_name", adUnitName);
			return this;
		}

		public SingularAdData WithAdGroupId(string adGroupId)
		{
			SetValue("ad_group_id", adGroupId);
			return this;
		}

		public SingularAdData WithAdGroupName(string adGroupName)
		{
			SetValue("ad_group_name", adGroupName);
			return this;
		}

		public SingularAdData WithAdGroupPriority(string adGroupPriority)
		{
			SetValue("ad_group_priority", adGroupPriority);
			return this;
		}

		public SingularAdData WithPrecision(string precision)
		{
			SetValue("ad_precision", precision);
			return this;
		}

		public SingularAdData WithPlacementId(string placementId)
		{
			SetValue("ad_placement_id", placementId);
			return this;
		}

		private void SetValue(string key, object value)
		{
			try
			{
				if (value != null && !(value.ToString().Trim() == string.Empty))
				{
					base[key] = value;
				}
			}
			catch (Exception)
			{
			}
		}

		public bool HasRequiredParams()
		{
			string[] requiredParams = RequiredParams;
			foreach (string key in requiredParams)
			{
				if (!ContainsKey(key) || base[key] == null || base[key].ToString().Trim() == string.Empty)
				{
					return false;
				}
			}
			return true;
		}
	}
}
