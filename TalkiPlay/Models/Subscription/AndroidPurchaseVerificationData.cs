namespace TalkiPlay
{
    public class AndroidPurchaseVerificationData
    {
        public string OrderId { get; set; }
        public string PackageName { get; set; }
        public string ProductId { get; set; }
        public long PurchaseTime { get; set; }
        public int PurchaseState { get; set; }
        public string DeveloperPayload { get; set; }
        public string PurchaseToken { get; set; }
        public bool AutoRenewing { get; set; }
    }
}