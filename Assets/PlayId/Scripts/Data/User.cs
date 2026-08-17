using System;
using Assets.PlayId.Scripts.Enums;
using Assets.PlayId.Scripts.Services;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace Assets.PlayId.Scripts.Data
{
    public class User
    {
        public int Id;
        public string Name;
        public string Email;
        public Platform Platforms;
        public TokenResponse TokenResponse;

        public Internals Internals;
        public CloudSaves CloudSaves;
        public Leaderboards Leaderboards;

        [Preserve]
        protected User()
        {
            Debug.Log("protected User()");
        }

        public User(int id, string name, string email, Platform platforms, TokenResponse tokenResponse)
        {
            Id = id;
            Name = name;
            Email = email;
            Platforms = platforms;
            TokenResponse = tokenResponse;
        }

        public User(TokenResponse tokenResponse)
        {
            var jwt = new JWT(tokenResponse.IdToken);
            var payload = JObject.Parse(jwt.Payload);

            Id = (int)payload["sub"];
            Name = (string)payload["name"];
            Email = (string)payload["email"];
            Platforms = (Platform)(int)payload["plat"];
            TokenResponse = tokenResponse;
        }

        public void InitializeServices()
        {
            if (TokenResponse.Expired) throw new Exception("Access token expired.");

            Internals = new Internals(TokenResponse.AccessToken);
            CloudSaves = new CloudSaves(TokenResponse.AccessToken);
            Leaderboards = new Leaderboards(TokenResponse.AccessToken, Id);
        }
    }
}