namespace TalkiPlay.Shared
{
    public class AppleReceipt
    {
        public AppleReceipt()
        {
            
        }

        public AppleReceipt(string productId, string bundleId, string transactionId, string data)
        {
            ProductId = productId;
            BundleId = bundleId;
            TransactionId = transactionId;
            Data = data;
        }
        
        public string ProductId { get; set; }
       
        public string BundleId { get; set; }

        public string TransactionId { get; set; }

        public string Data { get; set; }       
    }
    
    public class GoogleReceipt
    {
        public GoogleReceipt(string productId, string bundleId, string developerPayload, string purchaseToken)
        {
            ProductId = productId;
            BundleId = bundleId;
            DeveloperPayload = developerPayload;
            PurchaseToken = purchaseToken;
        }
        
        public string ProductId { get; set; }

        public string BundleId { get; set; }
        
        public string DeveloperPayload { get; set; }

        public string PurchaseToken { get; set; }
    }
    
    public class GoogleVerificationResponse
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }        
    }
}