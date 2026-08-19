using PlayId.Scripts.Data;
using UnityEditor;
using UnityEngine;

namespace PlayId.Scripts.Editor
{
    [CustomEditor(typeof(AuthSettings))]
    public class AuthSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CreateLinks();
        }

        public static void CreateLinks()
        {
            GUILayout.Label("Resources");

            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://github.com/hippogamesunity/PlayID/wiki");
            }

            if (GUILayout.Button("Setup steps"))
            {
                Application.OpenURL("https://github.com/hippogamesunity/PlayID/wiki/Setup-steps");
            }

            if (GUILayout.Button("API reference"))
            {
                Application.OpenURL("https://github.com/hippogamesunity/PlayID/wiki/API-reference");
            }

            if (GUILayout.Button("Troubleshooting"))
            {
                Application.OpenURL("https://github.com/hippogamesunity/PlayID/wiki/Troubleshooting");
            }

            if (GUILayout.Button("Online help"))
            {
                Application.OpenURL("https://discord.gg/4ht2AhW");
            }
        }
    }
}