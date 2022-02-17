﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Akavache;
using ChilliSource.Mobile.Api;
 using Newtonsoft.Json;
 using Plugin.DownloadManager.Abstractions;
using ReactiveUI;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public interface IAssetRepository
    {
        IObservable<IList<AssetDto>> GetAssets(AssetType? type = null, Category? category = null);
        IObservable<IList<AssetDto>> GetAssetsByIds(IList<int> assets);

        IObservable<AssetDto> GetAssetById(int assetId);
        
        IObservable<AssetDto> SaveAsset(AssetDto asset);
        
        Task<IEnumerable<AssetDto>> GetAllPdfAssets();


        IObservable<IList<ITag>> GetTags();
        
        Task<ITag> AddTag(int itemId, string serialNumber);


        IObservable<IRoom> GetRoom(int id);
        IObservable<IList<IRoom>> GetRooms();

        Task<IRoom> AddOrUpdateRoom(IRoom room);
        
       
        Task<IRoom> UpdateRoomItemTags(IRoom room);

        IObservable<IList<PackDto>> GetPacks(params int[] packIds);
        IObservable<PackDto> GetPack(int packId);
        IObservable<IList<IItem>> GetItems(ItemType type);
        
    }

    public class AssetRepository : IAssetRepository
    {
        
        private readonly IBlobCache _cache;
        private readonly IApi<ITalkiPlayApi> _api;

        public AssetRepository(
            IApi<ITalkiPlayApi> api = null,
            IBlobCache cache = null)
        {
            _cache = cache ?? Locator.Current.GetService<IBlobCache>();
            _api = api ?? Locator.Current.GetService<IApi<ITalkiPlayApi>>();
            
        }

        #region Assets
       
        private string GetAssetsKey(AssetType? type, Category? category)
        {
            if (type != null && category != null)
            {
                return $"Assets_type_{(int)type.Value}_category_{(int)category.Value}";
            }

            if (type != null)
            {
                return $"Assets_type_{(int)type.Value}";
            }

            if (category != null)
            {
                return $"Assets_category_{(int)type.Value}";
            }

            return "Assets";
        }

      

        public async Task<IEnumerable<AssetDto>> GetAllPdfAssets()
        {
            return await _api.Client.GetPdfs();
        }

        //note: dodgy, avoid using
        public IObservable<IList<AssetDto>> GetAssets(AssetType? type, Category? category)
        {
            return _cache.GetOrFetchObject(GetAssetsKey(type, category), () => GetAssetList(type, category), DateTimeOffset.Now.AddHours(3))
                .SelectMany(m => m.Count > 0 ? Observable.Return(m) : _cache.InvalidateObject<List<AssetDto>>("Tags").Select(_ => m))
                .Select(m => m);
        }

        IObservable<IList<AssetDto>> GetAssetList(AssetType? type, Category? category)
        {
            return _api.Client.GetAssets(type, category)
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful)
                    {
                        throw m.Exception;
                    }
                })
                .Select(r => r.Result);
        }

        public IObservable<IList<AssetDto>> GetAssetsByIds(IList<int> assets)
        {
            return _cache.GetObjects<AssetDto>(assets.Select(GetAssetKey))
                .ObserveOn(RxApp.TaskpoolScheduler)
                .SelectMany(m => _api.Client.GetAssets(null, null))
                .Select(m => m.Where(a => assets.Contains(a.Id)))
                .SelectMany(m =>
                {
                    return Observable.FromAsync(async () =>
                    {
                        foreach (var item in m)
                        {
                            await _cache.InvalidateObject<AssetDto>(GetAssetKey(item.Id));
                            await _cache.InsertObject(GetAssetKey(item.Id), item);
                        }

                    }).Select(_ => m);
                })
                .Select(m => m.ToList());
        }

        public IObservable<AssetDto> GetAssetById(int assetId)
        {
            return GetAssetsByIds(new List<int>() {assetId}).Select(m => m.FirstOrDefault());

        }
        public IObservable<AssetDto> SaveAsset(AssetDto asset)
        {
            return _cache.InvalidateObject<AssetDto>(GetAssetKey(asset.Id))
                .SelectMany(_ => _cache.InsertObject<AssetDto>(GetAssetKey(asset.Id), asset as AssetDto))
                .Select(_ => asset);
        }
        
        private static string GetAssetKey(int id)
        {
            return $"asset_{id}";
        }
        
        // public async Task<IEnumerable<AssetDto>> GetAllAudioAssets()
        // {
        //     return (await _cache.GetAllObjects<AssetDto>());//.Where(a => a.Type == AssetType.Audio);
        // }

        // public IObservable<Unit> DownloadAudioAssets(DateTimeOffset? lastDownloaded)
        // {
        //     return _api.Client.GetAssets(AssetType.Audio, null)
        //         .RetryAfter(Constants.NumberOfRetry, TimeSpan.FromMilliseconds(10), RxApp.TaskpoolScheduler)
        //         .ToResult()
        //         .SelectMany(m => m.IsSuccessful ? m.Result.ToObservable() : new List<AssetDto>().ToObservable())
        //         .Select(asset => asset.Type == AssetType.Audio ? DownloadAudioAsset(asset) : Observable.Return(asset))
        //         .Merge(Constants.NumberOfConcurrentDownload)
        //         .ToList()
        //         .SelectMany(assetList =>
        //         {
        //             return assetList.Any() ?
        //                 _cache.InsertObjects(assetList.Distinct().ToDictionary(m => GetAssetKey(m.Id), m => m))
        //                 .OnErrorResumeNext(Observable.Return(Unit.Default))
        //                 : Observable.Return(Unit.Default);
        //         });
        // }

        // public IObservable<IAsset> GetAsset(IAsset asset)
        // {
        //     return _cache.GetObject<AssetDto>(GetAssetKey(asset.Id))
        //         .ObserveOn(RxApp.TaskpoolScheduler)
        //         .OnErrorResumeNext(Observable.Return((AssetDto)null))
        //         .Select(a =>
        //         {
        //             if (a == null)
        //             {
        //                 return asset;
        //             }
        //             else
        //             {
        //                 return a as IAsset;
        //             }
        //         });
        // }
        
        // public async Task<IAsset> GetAssetById(int assetId)
        // {
        //     return await _cache.GetObject<AssetDto>(GetAssetKey(assetId));
        // }
        
        // private IObservable<AssetDto> DownloadAudioAsset(AssetDto item)
        // {
        //     return _cache.GetObject<AssetDto>(GetAssetKey(item.Id))
        //         .ObserveOn(RxApp.TaskpoolScheduler)
        //         .OnErrorResumeNext(Observable.Return((AssetDto)null))
        //         .SelectMany(asset =>
        //         {
        //             if (asset == null || item.Filesize != asset.Filesize || item.Filename != asset.Filename ||
        //                 String.IsNullOrWhiteSpace(asset.FilePath))
        //             {
        //                 var downloadFile = _downloadManager.CreateDownloadFile(_config.GetAssetDownloadUrl(item.Id),
        //                     new Dictionary<string, string>()
        //                     {
        //                         {"apiKey", _config.ApiKey},
        //                         {"fileName",$"{item.Id}"}
        //                     });
        //
        //                 var completionSource = new TaskCompletionSource<AssetDto>();
        //                 downloadFile.PropertyChanged += (sender, args) =>
        //                 {
        //                     if (sender is IDownloadFile file && args.PropertyName.Equals(nameof(IDownloadFile.Status)))
        //                     {
        //                         if (file.Status == DownloadFileStatus.COMPLETED)
        //                         {
        //                             var filePath = _downloadManager.PathNameForDownloadedFile(file);
        //                             _logger.Information($"File downloaded with {filePath}");
        //                             item.Filename = filePath;
        //                             completionSource.TrySetResult(item);
        //                         }
        //                     }
        //                 };
        //
        //                 _downloadManager.Start(downloadFile);
        //
        //                 return completionSource.Task.ToObservable();
        //             }
        //             return Observable.Return(item);
        //
        //         });
        //
        // }
        
       
        
        #endregion

        #region Tags

        public IObservable<IList<ITag>> GetTags()
        {
            return _cache.GetOrFetchObject("Tags", GetTagsList, DateTimeOffset.Now.AddHours(5))
                .SelectMany(m => m.Count > 0 ? Observable.Return(m) : _cache.InvalidateObject<List<TagDto>>("Tags").Select(_ => m))
                .Select(m => m.ToList<ITag>());
        }

        public IObservable<ITag> GetTag(int id)
        {
            return _cache.GetOrFetchObject(GetTagKey(id), () => GetTagById(id), DateTimeOffset.Now.AddHours(5));
        }

        private IObservable<IList<TagDto>> GetTagsList()
        {
            return _api.Client.GetTags()
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful)
                    {
                        throw m.Exception;
                    }
                })
                .Select(r => r.Result);
        }

        private string GetTagKey(int id)
        {
            return $"Tag_{id}";
        }

        private IObservable<TagDto> GetTagById(int id)
        {
            return _api.Client.GetTag(id)
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful)
                    {
                        throw m.Exception;
                    }
                })
                .Select(r => r.Result);
        }

        public async Task<ITag> AddTag(int itemId, string serialNumber)
        {
            var result = await _api.Client.AddTag(new AddTagRequest()
            {
                SerialNumber = serialNumber,
                ItemId = itemId
            }).ToResult();

            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }
            else
            {
                await _cache.InvalidateObject<List<TagDto>>("Tags");
                await _cache.InvalidateObject<TagDto>(GetTagKey(result.Result.Id));
                return result.Result as ITag;
            }
        }

        #endregion

        #region Rooms

        private string GetRoomKey(int id)
        {
            return $"room_{id}";
        }

        public IObservable<IRoom> GetRoom(int id)
        {
            return _cache.GetOrFetchObject(GetRoomKey(id), () => GetRoomById(id),
                DateTimeOffset.Now.AddHours(1));
        }

        private IObservable<RoomDto> GetRoomById(int id)
        {
            return _api.Client.GetRoom(id)
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful)
                    {
                        throw m.Exception;
                    }
                })
                .Select(r => r.Result);
        }

        public IObservable<IList<IRoom>> GetRooms()
        {            

            return _cache.GetOrFetchObject("Rooms", GetRoomList, DateTimeOffset.Now.AddHours(3))
                .SelectMany(m => m.Count > 0 ? Observable.Return(m) : _cache.InvalidateObject<List<RoomDto>>("Rooms").Select(_ => m))
                .Select(m => m.ToList<IRoom>());
        }

        private IObservable<IList<RoomDto>> GetRoomList()
        {
            return _api.Client.GetRooms()
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful)
                    {
                        throw m.Exception;
                    }
                })
                .Select(r => r.Result);
        }

        public Task<IRoom> AddOrUpdateRoom(IRoom room)
        {
            return room.Id > 0 ? UpdateRoom(room) : AddRoom(room);
        }
        
        public async Task<IRoom> UpdateRoom(IRoom room)
        {
            var result = await _api.Client.UpdateRoom(room.Id, new AddUpdateRoomRequest()
            {
                Name = room.Name,
                ImageAssetId = room.AssetId ?? 0
            }).ToResult();

            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            await _cache.InvalidateObject<List<RoomDto>>("Rooms");
            await _cache.InvalidateObject<RoomDto>(GetRoomKey(room.Id));

            return result.Result;          
        }

        public async Task<IRoom> AddRoom(IRoom room)
        {
            var result = await _api.Client.AddRoom(new AddUpdateRoomRequest()
            {
                Name = room.Name,
                ImageAssetId = room.AssetId ?? 0
            }).ToResult();

            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            await _cache.InvalidateObject<List<RoomDto>>("Rooms");
            await _cache.InvalidateObject<RoomDto>(GetRoomKey(room.Id));

            return result.Result;
        }
        
        public async Task<IRoom> UpdateRoomItemTags(IRoom room)
        {
            var result = await _api.Client.UploadRoomItemTags(room.Id, new RoomItemTagsUploadRequest()
            {
                Id = room.Id,
                ItemGroupIds = room.Packs,
                TagIds = room.TagItems.Select(a => a.Id).ToList()
            }).ToResult();


            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            await _cache.InvalidateObject<List<RoomDto>>("Rooms");
            await _cache.InvalidateObject<RoomDto>(GetRoomKey(room.Id));

            return result.Result;
        }

        #endregion

        #region Packs
        
        
        public IObservable<IList<PackDto>> GetPacks(params int[] ids)
        {
            return _api.Client.GetPacks(ids)
               .ToResult()
               .Do(m =>
               {
                   if (!m.IsSuccessful)
                   {
                       throw m.Exception;
                   }
               })
               .Select(m => m.Result);
        }

        public IObservable<PackDto> GetPack(int id)
        {
            return GetPacks(id).Select(a => a.FirstOrDefault());
        }
       
        #endregion

        #region Items

        private string GetItemsKey(ItemType type)
        {
            return $"Items_{(int)type}";
        }
        
        // public IObservable<IList<IItem>> GetFullItemList()
        // {
        //     return Observable.CombineLatest(GetItems(ItemType.Home),
        //             GetItems(ItemType.Default),
        //         (homeList, itemsList) =>
        //         {
        //             var result = new List<IItem>();
        //             result.AddRange(homeList);
        //             result.AddRange(itemsList);
        //
        //             return result;
        //         })
        //         .Select(m => m);
        //
        // }
        
        public IObservable<IList<IItem>> GetItems(ItemType type)
        {
            return _cache.GetOrFetchObject(GetItemsKey(type), () => _GetItemList(type), DateTimeOffset.Now.AddHours(2))
                .SelectMany(m => m.Count > 0 ? Observable.Return(m) : _cache.InvalidateObject<List<ItemDto>>(GetItemsKey(type)).Select(_ => m))
                .Select(m => m.ToList<IItem>());
        }

        private IObservable<IList<ItemDto>> _GetItemList(ItemType type)
        {
            return _api.Client.GetItems(type.ToString())
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful)
                    {
                        throw m.Exception;
                    }
                })
                .Select(m => m.Result);
        }
      
        #endregion
               
    }
}