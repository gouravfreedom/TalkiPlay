using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace TalkiPlay.Shared
{
   
    
    public class UserDto : IUser
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("userKey")]
        public string UserKey { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        
        [JsonProperty("email")]
        public string Email { get; set; }

        // [JsonProperty("fullName")]
        // public string FullName { get; set; }
        //
        // [JsonProperty("status")]
        // public UserStatus Status { get; set; }
        //
        // [JsonProperty("roles")]
        // public IList<string> Roles { get; set; }
        //
        // [JsonProperty("profilePhotoPath")]
        // public string ProfilePhotoPath { get; set; }
        
        // [Obsolete]
        // [JsonProperty("hasSubscription")]
        // public bool HasSubscription { get; set; }
        
        [JsonProperty("subscriptionStatus")]
        public UserSubscriptionStatus SubscriptionStatus { get; set; }
    }

    public class LoginRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
    }

    public enum CompanyType: int
    {
        Company = 1,
        HouseHold
    }

    public class RegistrationRequest
    {
      
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("companytype")]
        public CompanyType CompanyType { get; set; }

        [JsonProperty("acceptTermsConditions")]
        public bool AcceptTermsConditions => true;

        [JsonProperty("isAnonymous")]
        public bool IsAnonymous => false;

        [JsonProperty("timezone")]
        public string Timezone { get; set; }
    }

    public interface IUpdateUserRequest
    {
        
    }

    public class UpdateUserRequest : IUpdateUserRequest
    {
        
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
        
        // [JsonProperty("isProfileCompleted")]
        // public bool IsProfileCompleted { get; set; }
        
    }

    public class PatchByTokenUserRequest : IUpdateUserRequest
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
    
    public class PatchUserRequest : IUpdateUserRequest
    {
        [JsonProperty("currentPassword")]
        public string CurrentPassword { get; set; }

        [JsonProperty("password")]
        public string NewPassword { get; set; }

        [JsonProperty("passwordSpecified")]
        public bool IsPasswordSpecified { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("emailSpecified")]
        public bool IsEmailSpecified { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("nameSpecified")]
        public bool IsNameSpecified { get; set; }
    }

    public class ForgotPasswordTokenRequest
    {
        
       [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "Password";
    }
    
    public class TokenRequest
    {
        [JsonProperty("token")]
        public string Email { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}