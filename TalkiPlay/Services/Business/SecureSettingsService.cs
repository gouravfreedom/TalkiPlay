using System;
using System.Threading.Tasks;
using ChilliSource.Core.Extensions;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace TalkiPlay.Shared
{
    public static class SecureSettingsService
    {
        #region User

        public static async Task<bool> IsUserLoggedIn()
        {
            var user = await GetUser();
            var key = await GetUserKey();
            return user != null && !string.IsNullOrWhiteSpace(key);
        }

        public static async Task UpdateUserSubscriptionStatus(UserSubscriptionStatus status)
        {
            var user = await GetUser();
            if (user != null)
            {
                user.SubscriptionStatus = status;
                await SaveUser(user);
            }
        }
        
        
        public static async Task<string> GetUserKey()
        {
            return await GetValue("user_key");
        }
        
        public static async Task SaveUserKey(string userKey)
        {
            await SaveValue("user_key", userKey);
        }
        
        public static async Task<UserDto> GetUser()
        {
            return await GetValue<UserDto>("user");
        }
        
        public static async Task SaveUser(UserDto user)
        {
            if (user == null)
            {
                return;
            }
            
            await SaveValue("user", JsonConvert.SerializeObject(user));
        }

        public static void ClearUser()
        {
            try
            {
                SecureStorage.Remove("user");
                SecureStorage.Remove("user_key");
            }
            catch (Exception ex)
            {                    
                Serilog.Log.Error(ex.Message, ex);
            }
        }
        
        #endregion
       
        static async Task SaveValue(string key, string value)
        {
            try
            {
                await SecureStorage.SetAsync(key, value);
            }
            catch (Exception ex)
            {                    
                Serilog.Log.Error(ex.Message, ex);
            }
        }
        
        static async Task<T> GetValue<T>(string key)
        {
            var result = await GetValue(key);
            if (!string.IsNullOrWhiteSpace(result))
            {
                return JsonConvert.DeserializeObject<T>(result);
            }

            return default(T);
        }
        
        static async Task<string> GetValue(string key)
        {
            try
            {
                return await SecureStorage.GetAsync(key);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message, ex);
            }
            return null;
        }
    }
}