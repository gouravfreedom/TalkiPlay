using System;
using System.Collections.Generic;
using System.IO;
using Refit;

namespace TalkiPlay.Shared
{
    public interface ITalkiPlayApi
    {
        
        [Post("/v1/apple/verifysubscription")]
        IObservable<bool> VerifyAppleSubscription([Body]  AppleReceipt req);
        
        [Post("/v1/apple/verifyreceipt")]
        IObservable<bool> VerifyAppleReceipt([Body]  AppleReceipt req);
        
        [Post("/v1/google/verifysubscription")]
        IObservable<GoogleVerificationResponse> VerifyGoogleSubscription([Body]  GoogleReceipt req);

        [Get("/v1/categories")]
        IObservable<IList<CategoryDto>> GetCategories();

        [Get("/v1/categories/{id}")]
        IObservable<CategoryDto> GetCategory(int id);


        [Get("/v1/company")]
        IObservable<CompanyResponseDto> GetCompany();
        
        [Patch("/v1/company")]
        IObservable<CompanyResponseDto> PatchCompany([Body]  CompanyPatchRequest req);
        
        [Get("/v1/games")]
        IObservable<IList<GameDto>> GetGames();
        
        [Get("/v1/games/{id}")]
        IObservable<GameDto> GetGame(int id);

        [Get("/v1/assets")]
        IObservable<IList<AssetDto>> GetAssets(AssetType? assetType, Category? category);

        [Get("/v1/assets?type=pdf")]
        IObservable<IList<AssetDto>> GetPdfs();

        [Get("/v1/assets/{id}")]
        IObservable<Stream> DownloadAsset(int id);
        
        [Get("/v1/firmware")]
        IObservable<FileDataDto> GetLatestFirmware();
        
        [Get("/v1/firmware/download")]
        IObservable<Stream> DownloadLatestFirmware();

        [Post("/v1/usersessions/byemail")]
        IObservable<UserDto> Login([Body] LoginRequest req);

        [Delete("/v1/usersessions/current")]
        IObservable<string> Logout();
        
        [Get("/v1/usersessions/current")]
        IObservable<UserDto> GetCurrentSession();
        
        [Put("/v1/users/current")]
        IObservable<UserDto> UpdateUser([Body] UpdateUserRequest req);
        
        [Patch("/v1/users/current")]
        IObservable<UserDto> PatchUser([Body]  PatchUserRequest req);

        [Get("/v1/users/current")]
        IObservable<UserDto> GetCurrentUser();

        [Get("/v1/users/bytoken/{token}")]
        IObservable<UserDto> GetUserByToken(string token, string email);
        
        [Patch("/v1/users/bytoken/{token}")]
        IObservable<UserDto> PatchUserByToken(string token, [Body] PatchByTokenUserRequest request);
        
        [Post("/v1/users/byemail")]
        IObservable<UserDto> Register([Body] RegistrationRequest req);
        
        [Post("/v1/users/tokens")]
        IObservable<string> RequestForgotPasswordToken([Body] ForgotPasswordTokenRequest req);
        
        [Get("/v1/rooms")]
        IObservable<IList<RoomDto>> GetRooms();
        
        [Get("/v1/rooms/{id}")]
        IObservable<RoomDto> GetRoom(int id);

        [Post("/v1/rooms")]
        IObservable<RoomDto> AddRoom([Body] AddUpdateRoomRequest req);
        
        [Put("/v1/rooms/{id}")]
        IObservable<RoomDto> UpdateRoom(int id, [Body] AddUpdateRoomRequest req);

        [Put("/v1/rooms/{id}/configure")]
        IObservable<RoomDto> UploadRoomItemTags(int id, [Body] RoomItemTagsUploadRequest req);
        
        [Get("/v1/packs")]
        IObservable<IList<PackDto>> GetPacks([Query(CollectionFormat.Multi)] int[] ids);
        
        [Get("/v1/items")]
        IObservable<IList<ItemDto>> GetItems(string type);
        
        [Get("/v1/tags")]
        IObservable<IList<TagDto>> GetTags();

        [Get("/v1/tags/{id}")]
        IObservable<TagDto> GetTag(int id);
        
        [Post("/v1/tags")]
        IObservable<TagDto> AddTag([Body] AddTagRequest req);
        
        [Get("/v1/children")]
        IObservable<IList<ChildDto>> GetChildren();
        [Get("/v1/children/{id}")]
        IObservable<ChildDto> GetChild(int id);
        
        [Post("/v1/children")]
        IObservable<ChildDto> AddChild([Body] AddUpdateChildRequest req);

        [Put("/v1/children/{id}")]
        IObservable<ChildDto> UpdateChild(int id, [Body] AddUpdateChildRequest req);
        
        [Post("/v2/games/{id}/sessions")]
        IObservable<RewardDto> RecordGameSessionResult(int id, [Body] RecordGameSessionResultRequest req);
        
        [Get("/v1/rewards")]
        IObservable<IList<RewardDto>> GetRewards();
        
        [Get("/v1/rewards/{id}")]
        IObservable<RewardDto> GetReward(int id);
        
        [Get("/v1/rewards/child/{id}")]
        IObservable<IList<ChildRewardDto>> GetChildRewards(int id);

        ///api/v1/children/{childId}/games/{gameId}
        [Get("/v1/children/{childId}/games/{gameId}")]
        IObservable<ChildItemRewardsDto> GetChildGameRewards(int childId, int gameId);

        ///api/v1/children/{childId}/packs
        [Get("/v1/children/{childId}/packs")]
        IObservable<IList<ChildPackProgressDto>> GetChildPacksProgress(int childId);

        ///api/v1/children/{childId}/packs/{packId}
        [Get("/v1/children/{childId}/packs/{packId}")]
        IObservable<IList<ChildItemProgressDto>> GetChildItemsProgress(int childId, int packId);
    }
}