namespace TalkiPlay.Shared
{
    public static class ValidationMessages
    {
        public static string RequiredValidationMessage(string field) => $"{field} is required.";
        public const string EmailValidationMessage = "Email is invalid";

    };
}