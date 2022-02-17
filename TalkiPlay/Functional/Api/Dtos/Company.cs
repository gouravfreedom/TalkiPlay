using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class CompanyPatchRequest
    {
        public CompanyPatchRequest()
        {
            
        }

        public CompanyPatchRequest(bool hasSubscription, string productId)
        {
            HasAppStoreSubscription = hasSubscription;
            AppStoreSubscriptionProductId = productId;
        }
        
        [JsonProperty("hasAppStoreSubscription")]
        public bool HasAppStoreSubscription { get; set; }
        
        [JsonProperty("appStoreSubscriptionProductId")]
        public string AppStoreSubscriptionProductId { get; set; }
    }
    
    public class CompanyResponseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("hasAppStoreSubscription")]
        public bool HasAppStoreSubscription { get; set; }
        
        [JsonProperty("appStoreSubscriptionProductId")]
        public string AppStoreSubscriptionProductId { get; set; }
    }
}