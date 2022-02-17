
using System;
using System.Threading.Tasks;
using ChilliSource.Mobile.Core;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
   public class ApiToken
	{
		private readonly IEnvironmentInformation _environmentInformation;
		private readonly Func<Task<string>> _getUserKey;

		[JsonConstructorAttribute]
		public ApiToken(string apiKey, IEnvironmentInformation environmentInformation, Func<Task<string>> getUserKey)
		{
			_environmentInformation = environmentInformation;
			ApiKey = apiKey;
			_getUserKey = getUserKey;
		}

		public async Task LoadUserKey()
		{
			UserKey = await _getUserKey();
		}
		public string ApiKey { get; }
		
		public string UserKey { get; set; }
		
        public string AppId => _environmentInformation.AppId;

       public string AppVersion => _environmentInformation.AppVersion;

		public string Timezone => _environmentInformation.Timezone;

		public string Platform => _environmentInformation.Platform;
		
	}
}
