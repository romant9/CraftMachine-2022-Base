using System;
using System.Collections.Generic;

public static class CdnUrlHelper
{
	private const string RuCdnBaseUrl = "https://nml-client-update.drillerservices.com";

	private static readonly HashSet<string> RuCdnCountryCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RU" };

	public static bool IsRussianPlayer()
	{
		string countryCode = GameManager.GetCountryCode();
		if (!string.IsNullOrEmpty(countryCode))
		{
			return RuCdnCountryCodes.Contains(countryCode);
		}
		return false;
	}

	public static string GetDefaultCdnBaseUrl()
	{
		if (IsRussianPlayer())
		{
			return "https://nml-client-update.drillerservices.com";
		}
		return GameConfiguration.Instance.Config.ContentBaseUrl;
	}

	public static string RewriteCdnUrl(string url)
	{
		if (string.IsNullOrEmpty(url) || !IsRussianPlayer())
		{
			return url;
		}
		if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			return url;
		}
		if (!Uri.TryCreate("https://nml-client-update.drillerservices.com", UriKind.Absolute, out var result2))
		{
			Debug.LogWarning("CdnUrlHelper: Invalid RuCdnBaseUrl: https://nml-client-update.drillerservices.com");
			return url;
		}
		return new UriBuilder(result)
		{
			Scheme = result2.Scheme,
			Host = result2.Host,
			Port = (result2.IsDefaultPort ? (-1) : result2.Port)
		}.Uri.ToString();
	}
}
