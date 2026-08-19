#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using PlayId.Scripts.Enums;
using UnityEngine;

namespace PlayId.Scripts.Data
{
    [CreateAssetMenu(fileName = "AppSettings", menuName = "Play ID/App Settings")]
    public class AppSettings : ScriptableObject
    {
        public string InvoiceNumber;
        [Password] public string SecretKey;
        public string ClientId;
        public string Name;
        public Texture2D Icon;
        public List<string> RedirectUriWhitelist;
        public List<string> Leaderboards;
        public string RemoteConfig;
        [ReadOnly] public PricingPlan PricingPlan;
        [ReadOnly] public int UserCount;

        public string Validate()
        {
            foreach (var uri in RedirectUriWhitelist)
            {
                if (uri != "*" && !Uri.TryCreate(uri, UriKind.Absolute, out _)) return $"Invalid redirect URI: {uri}. Example: [scheme]://[path].";
            }

            if (Leaderboards.Distinct().Count() != Leaderboards.Count) return "Duplicated leaderboards.";
            
            return null;
        }
    }

    public class PasswordAttribute : PropertyAttribute
    {
    }

    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}

#endif