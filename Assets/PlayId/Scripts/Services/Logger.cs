using UnityEngine;

namespace Assets.PlayId.Scripts.Services
{
    /// <summary>
    /// This class is used to enable/disable logs in one place.
    /// </summary>
    public class Logger
    {
        public static bool Enabled = true;

        protected void Log(string message)
        {
            if (Enabled) Debug.Log(message); // TODO: Remove in Release.
        }

        protected void LogError(string error)
        {
            if (Enabled) Debug.LogError(error); // TODO: Remove in Release.
        }
    }
}