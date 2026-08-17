using System;
using System.Threading.Tasks;
using Assets.PlayId.Scripts.Data;
using Newtonsoft.Json;

namespace Assets.PlayId.Scripts.Services
{
    public partial class Auth
    {
        /// <summary>
        /// Async sign-in.
        /// </summary>
        public async Task<User> SignInAsync()
        {
            var completed = false;
            string error = null;
            User user = null;

            SignIn((success, e, result) =>
            {
                if (success)
                {
                    user = result;
                }
                else
                {
                    error = e;
                }

                completed = true;
            }, caching: true);

            while (!completed)
            {
                await Task.Yield();
            }

            if (user == null) throw new Exception(error);

            Log($"user={JsonConvert.SerializeObject(user)}");

            return user;
        }
    }
}