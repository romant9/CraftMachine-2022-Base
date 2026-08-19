using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlayId.Scripts.Data;
using PlayId.Scripts.Enums;
using PlayId.Scripts.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayId.Scripts.Editor
{
    /// <summary>
    /// Adds "Rebuild" button to CharacterBuilder script.
    /// </summary>
    [CustomEditor(typeof(AppSettings))]
    public class AppSettingsEditor : UnityEditor.Editor
    {
        #if _DEBUG
        private const string AppSettingsEndpoint = "https://localhost:7297/app";
        #else
        private const string AppSettingsEndpoint = "https://playid.org/app";
        #endif

        private string _backup;

        public void OnEnable()
        {
            Backup();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var appSettings = (AppSettings)target;

            if (GUILayout.Button("Load") && ValidateSettings(appSettings))
            {
                Load(appSettings);
            }

            if (GUILayout.Button("Save") && ValidateSettings(appSettings))
            {
                Save(appSettings);
            }

            GUILayout.Label("Admin");

            if (GUILayout.Button("Developer configuration"))
            {
                Application.OpenURL($"https://playid.org/auth/dev?invoice={appSettings.InvoiceNumber}");
            }

            AuthSettingsEditor.CreateLinks();
        }

        private bool ValidateSettings(AppSettings appSettings)
        {
            if (appSettings.SecretKey == "" && appSettings.ClientId != "00000000000000000000000000000000")
            {
                EditorUtility.DisplayDialog("Error", "Please set Secret Key.", "OK");
                return false;
            }

            if (appSettings.ClientId == "")
            {
                EditorUtility.DisplayDialog("Error", "Please set Client Id.", "OK");
                return false;
            }

            return true;
        }

        private void Load(AppSettings appSettings)
        {
            var request = UnityWebRequest.Post($"{AppSettingsEndpoint}/load", new Dictionary<string, string>
            {
                { "secret_key", appSettings.SecretKey ?? "" },
                { "client_id", appSettings.ClientId ?? "" }
            });

            Debug.Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var jObject = JObject.Parse(request.downloadHandler.text);
                    
                    appSettings.Name = (string)jObject["Name"];
                    appSettings.RedirectUriWhitelist = jObject["RedirectUriWhitelist"]?.ToObject<List<string>>() ?? new List<string>();
                    appSettings.Leaderboards = jObject["Leaderboards"]?.ToObject<List<string>>() ?? new List<string>();
                    appSettings.RemoteConfig = (string)jObject["RemoteConfig"];
                    appSettings.PricingPlan = (PricingPlan)(byte)jObject["PricingPlan"];
                    appSettings.UserCount = (int)jObject["UserCount"];
                    
                    if (jObject["Icon"] == null || jObject["Icon"].Type == JTokenType.Null)
                    {
                        appSettings.Icon = null;
                    }
                    else
                    {
                        var bytes = (byte[])jObject["Icon"];
                        var texture = new Texture2D(2, 2) { name = "~Icon" };
                        
                        texture.LoadImage(bytes);

                        appSettings.Icon = texture;
                    }

                    Backup();
                    EditorUtility.DisplayDialog("Success", "App settings loaded.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", request.GetError(), "OK");
                }

                request.Dispose();
            };
        }

        private void Save(AppSettings appSettings)
        {
            var error = appSettings.Validate();

            if (error != null)
            {
                EditorUtility.DisplayDialog("ERROR", error, "OK");
                return;
            }

            var form = new Dictionary<string, string>
            {
                { "secret_key", appSettings.SecretKey ?? "" },
                { "client_id", appSettings.ClientId ?? "" },
                { "name", appSettings.Name ?? "" },
                { "redirect", string.Join(',', appSettings.RedirectUriWhitelist) },
                { "leaderboards", string.Join(',', appSettings.Leaderboards) },
                { "remote_config", appSettings.RemoteConfig ?? "" }
            };

            var removed = RestoreBackup().Leaderboards.Except(appSettings.Leaderboards).ToList();

            if (removed.Any())
            {
                if (!EditorUtility.DisplayDialog("WARNING", $"The following leaderbords will be removed (with all user scores): {string.Join(", ", removed)}", "Confirm", "Cancel")) return;
            }

            if (appSettings.Icon != null)
            {
                var path = AssetDatabase.GetAssetPath(appSettings.Icon);
                var bytes = appSettings.Icon.name == "~Icon" ? appSettings.Icon.EncodeToPNG() : File.ReadAllBytes(path);

                form.Add("icon", Convert.ToBase64String(bytes));
            }

            var request = UnityWebRequest.Post($"{AppSettingsEndpoint}/save", form);

            Debug.Log($"Downloading: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    EditorUtility.DisplayDialog("Success", "App settings saved.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", request.GetError(), "OK");
                }

                request.Dispose();
            };
        }

        private void Backup()
        {
            var appSettings = (AppSettings) target;

            _backup = JsonConvert.SerializeObject(new { appSettings.Leaderboards });
        }

        private AppSettings RestoreBackup()
        {
            return JsonConvert.DeserializeObject<AppSettings>(_backup);
        }
    }
}