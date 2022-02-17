using System;
using System.Collections.Generic;

namespace TalkiPlay.Shared
{

    // public enum UserStatus
    // {
    //     Registered,
    //     Activated,
    //     Deleted,
    //     Invited,
    //     Anonymous
    // }
    
    public enum UserSubscriptionStatus
    {
        None = 0,
        AppStore,
        Stripe
    }
    
    public interface IUser
    {
        int Id { get; }
        
        string UserKey { get; }
        string FirstName { get; }
        string LastName { get; }
        
        string Email { get; }
        
        // string FullName { get; }
        // UserStatus Status { get; }
        // IList<string> Roles { get; }
        // string ProfilePhotoPath { get; }
        //[Obsolete]
        //bool HasSubscription { get; }
        
        UserSubscriptionStatus SubscriptionStatus { get; set; }
        
    }
}