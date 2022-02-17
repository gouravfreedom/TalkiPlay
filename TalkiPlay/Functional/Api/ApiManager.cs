using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
using TalkiPlay.Shared;

namespace ChilliSource.Mobile.Api
{
	public static class ApiConfiguration
	{
		public static readonly Func<JsonSerializerSettings> DefaultJsonSerializationSettingsFactory = () =>
		{
			var settings = new JsonSerializerSettings()
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				Converters = { new StringEnumConverter(), new IsoDateTimeConverter() },
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = DateTimeZoneHandling.Utc
			};
  
			return settings;
		};
	}
	
	public interface IApi<T>
	{
		T Client { get; }

		void SetUserKey(string key);
	}
	
	public class ApiManager<T> : IApi<T>
	{
		private readonly ApiToken _token;
		
		public ApiManager(string baseUrl, ApiToken token,  IConnectivity connectivity, HttpMessageHandler messageHandler)
        {
	        _token = token;

	        var apiHandler = new ApiAuthenticationHandler(_token, messageHandler);
	        var noNetworkHandler = new NoNetworkHandler(connectivity, apiHandler);
	        
			var client = new HttpClient(noNetworkHandler)
			{
				Timeout = TimeSpan.FromSeconds(30),
				BaseAddress = new Uri(baseUrl)
			};
			
			Client = RestService.For<T>(client, new RefitSettings()
			{
				ContentSerializer = new NewtonsoftJsonContentSerializer(ApiConfiguration.DefaultJsonSerializationSettingsFactory())
			});
        }

		public void SetUserKey(string key)
		{
			_token.UserKey = key;
		}
	
		public T Client { get; }
	}
}
