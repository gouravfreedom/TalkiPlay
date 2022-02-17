using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using ChilliSource.Mobile.Api;
using Splat;

namespace TalkiPlay.Shared
{
    public interface IUserRepository
    {
        Task<IUser> Login(LoginRequest req);
        Task Logout();
        Task<IUser> Register(RegistrationRequest req);
        Task<IUser> Update(IUpdateUserRequest req);
        Task<IUser> GetUser();
        Task<IUser> GetUser(TokenRequest req);
        Task RequestForgotPasswordToken(ForgotPasswordTokenRequest req);

        Task<CompanyResponseDto> GetCompany();
        Task<CompanyResponseDto> UpdateCompany(CompanyPatchRequest req);
        Task<bool> VerifyAppleSubscription(AppleReceipt receipt);
        Task<GoogleVerificationResponse> VerifyGoogleSubscription(GoogleReceipt receipt);
        Task<bool> VerifyAppleReceipt(AppleReceipt receipt);

        void ClearSession();

    }

    public class UserRepository : IUserRepository
    {
        private readonly IUserSettings _settings;
        private readonly IApi<ITalkiPlayApi> _api;

        public UserRepository(IUserSettings settings = null,     
            IApi<ITalkiPlayApi> api = null)
        {
            _settings = settings ?? Locator.Current.GetService<IUserSettings>();
             _api = api ?? Locator.Current.GetService<IApi<ITalkiPlayApi>>();
        }

        public async Task<IUser> Login(LoginRequest req)
        {
            var result = await _api.Client.Login(req).ToResult();

            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }
            
            var user = result.Result;
            
            _api.SetUserKey(user.UserKey);
            await SecureSettingsService.SaveUserKey(user.UserKey);
            await SecureSettingsService.SaveUser(user);

            Task.Run(() =>
            {
                var cache = Locator.Current.GetService<IBlobCache>();
                cache.InvalidateAll();
                
            }).Forget();

           
            
            return user;           
        }

        public async Task Logout()
        {
            var result = await _api.Client.Logout().ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            ClearSession();

        }

        public void ClearSession()
        {
            _settings.ClearSession();
            SecureSettingsService.ClearUser();
            _api.SetUserKey("");
        }
        
        public async Task<IUser> Register(RegistrationRequest req)
        {
            var result = await _api.Client.Register(req).ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            var loginRequest = new LoginRequest
            {
                Email = req.Email,
                Password = req.Password,
                DeviceId = _settings.UniqueId
            };

            return await Login(loginRequest);
        }
        
        public async Task<IUser> Update(IUpdateUserRequest req)
        {
            IObservable<UserDto> observable;
            
            if (req is UpdateUserRequest updateReq)
            {
                observable = _api.Client.UpdateUser(updateReq);
            }
            else if (req is PatchUserRequest patchReq)
            {
                observable = _api.Client.PatchUser(patchReq);
            }
            else if (req is PatchByTokenUserRequest patchTokenReq)
            {
                observable = _api.Client.PatchUserByToken(patchTokenReq.Token, patchTokenReq);
            }
            else
            {
                observable = Observable.Return((UserDto) null);
            }

            var result = await observable.ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            await SecureSettingsService.SaveUser(result.Result);
            //_settings.User = result.Result;
            
            return result.Result;          
        }

        public async Task<IUser> GetUser()
        {
            var result = await _api.Client.GetCurrentUser().ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }
            await SecureSettingsService.SaveUser(result.Result);
            //_settings.User = result.Result;
            return result.Result;          
        }

        public async Task<IUser> GetUser(TokenRequest req)
        {
            var result = await _api.Client.GetUserByToken(req.Token, req.Email).ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            await SecureSettingsService.SaveUser(result.Result);
            //_settings.User = result.Result;
            return result.Result;
           
        }

        public async Task RequestForgotPasswordToken(ForgotPasswordTokenRequest req)
        {
            var result = await _api.Client.RequestForgotPasswordToken(req).ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }          
        }
        
        public async Task<CompanyResponseDto> GetCompany()
        {
            var result = await _api.Client.GetCompany().ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            return result.Result;
        }
        
        public async Task<CompanyResponseDto> UpdateCompany(CompanyPatchRequest req)
        {
            var result = await _api.Client.PatchCompany(req).ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            return result.Result;
        }

        public async Task<bool> VerifyAppleSubscription(AppleReceipt receipt)
        {
            var result = await _api.Client.VerifyAppleSubscription(receipt).ToResult();

            var success = false;
            if (result.IsSuccessful)
            {
                success = result.Result;
            }
            
            Debug.WriteLine("VerifyAppleSubscription: " + success);
            return success;
        }
        
        public async Task<GoogleVerificationResponse> VerifyGoogleSubscription(GoogleReceipt receipt)
        {
            var result = await _api.Client.VerifyGoogleSubscription(receipt).ToResult();
            return result.Result;
            
        }
        
        public async Task<bool> VerifyAppleReceipt(AppleReceipt receipt)
        {
            var result = await _api.Client.VerifyAppleReceipt(receipt).ToResult();
            
            var success = false;
            if (result.IsSuccessful)
            {
                success = result.Result;
            }
            
            Debug.WriteLine("VerifyAppleReceipt: " + success);
            return success;
        }
    }
}