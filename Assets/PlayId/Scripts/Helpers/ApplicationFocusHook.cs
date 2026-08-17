using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PlayId.Scripts.Helpers
{
    public class ApplicationFocusHook : MonoBehaviour
    {
        private static ApplicationFocusHook _instance;
        private readonly List<Action> _callbacks = new();

        public static void Create(Action callback)
        {
            if (_instance == null)
            {
                _instance = new GameObject(nameof(ApplicationFocusHook)).AddComponent<ApplicationFocusHook>();
            }

            _instance._callbacks.Add(callback);
        }

        public static void Cancel()
        {
            if (_instance == null) return;

            Destroy(_instance.gameObject);
            _instance = null;
        }

        public void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                _callbacks.ForEach(i => i.Invoke());
                _callbacks.Clear();
            }
        }
    }
}