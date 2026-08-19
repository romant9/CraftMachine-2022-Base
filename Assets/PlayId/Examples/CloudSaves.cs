using System;
using System.Collections.Generic;
using PlayId.Scripts;
using PlayId.Scripts.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace PlayId.Examples
{
    public class CloudSaves : MonoBehaviour
    {
        public Text Output;

        public void Save()
        {
            PlayIdServices.Instance.Auth.SignIn(OnSignIn);

            void OnSignIn(bool success, string error, User user)
            {
                if (success)
                {
                    var data = new Dictionary<string, object> { { "progress", 10 }, { "timestamp", DateTime.UtcNow } };
                    var json = JsonConvert.SerializeObject(data);

                    user.CloudSaves.Save(json, OnSave);
                }
                else
                {
                    Output.text = error;
                }
            }

            void OnSave(bool success, string error)
            {
                Output.text = success ? "Saved!" : error;
            }
        }

        public void Load()
        {
            PlayIdServices.Instance.Auth.SignIn(OnSignIn);

            void OnSignIn(bool success, string error, User user)
            {
                if (success)
                {
                    user.CloudSaves.LoadString(OnLoad);
                }
                else
                {
                    Output.text = error;
                }
            }

            void OnLoad(bool success, string error, string data)
            {
                Output.text = success ? data : error;
            }
        }
    }
}