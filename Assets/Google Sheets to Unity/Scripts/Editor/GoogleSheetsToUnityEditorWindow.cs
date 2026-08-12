using System.Net;
using UnityEditor;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


namespace GoogleSheetsToUnity.Editor
{
    public class GoogleSheetsToUnityEditorWindow : EditorWindow
    {

        const float DarkGray = 0.4f;
        const float LightGray = 0.9f;

        GoogleSheetsToUnityConfig config;
        private bool showSecret = false;

        int tabID = 0;

        [MenuItem("Window/GSTU/Open Config")]
        static void Open()
        {
            GoogleSheetsToUnityEditorWindow win = EditorWindow.GetWindow<GoogleSheetsToUnityEditorWindow>("Google Sheets To Unity");
            ServicePointManager.ServerCertificateValidationCallback = Validator;

            win.Init();
        }

        public static bool Validator(object in_sender, X509Certificate in_certificate, X509Chain in_chain, SslPolicyErrors in_sslPolicyErrors)
        {
            return true;
        }

        public void Init()
        {
            config = (GoogleSheetsToUnityConfig)Resources.Load("GSTU_Config");
        }

        void OnGUI()
        {
            tabID = GUILayout.Toolbar(tabID, new string[] {"Private", "Private (Legacy)", "Public"});

            if (config == null)
            {
                DebugTWD.LogError("Error: no config file");
                return;
            }

            switch (tabID)
            {
                case 0:
                    {
                        config.CLIENT_ID = EditorGUILayout.TextField("Client ID", config.CLIENT_ID);

                        GUILayout.BeginHorizontal();
                        if (showSecret)
                        {
                            config.CLIENT_SECRET = EditorGUILayout.TextField("Client Secret Code", config.CLIENT_SECRET);
                        }
                        else
                        {
                            config.CLIENT_SECRET = EditorGUILayout.PasswordField("Client Secret Code", config.CLIENT_SECRET);

                        }
                        showSecret = GUILayout.Toggle(showSecret, "Show");
                        GUILayout.EndHorizontal();

                        config.PORT = EditorGUILayout.IntField("Port number", config.PORT);

                        if (GUILayout.Button("Build Connection"))
                        {
                            GoogleAuthrisationHelper.BuildHttpListener();
                        }

                        break;
                    }
                case 1:
                    {
                        GUILayout.Label("This is the legacy version of GSTU and will be removed at a future date, if you wish to use it please press the button below");
                        if(GUILayout.Button("Use Legacy Version"))
                        {
                            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (defines + ";" + "GSTU_Legacy"));
                        }
                        break;
                    }

                case 2:
                    {
                        config.API_Key = EditorGUILayout.TextField("API Key", config.API_Key);
                        break;
                    }
            }

            EditorUtility.SetDirty(config);
        }
    }
}