using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using ChilliSource.Mobile.Api;
using Splat;

namespace TalkiPlay.Shared
{
    public interface IGameService
    {
        Task<IList<GameDto>> GetGames();
        Task<GameDto> GetGame(int id);
        
        [Obsolete]
        Task<IGame> GetGameWithAssets(int id, IList<int> tagIds);
        Task<RewardDto> RecordGameResult(RecordGameSessionResultRequest req, int packId);
        IObservable<RewardDto> RecordGameResultOld(RecordGameSessionResultRequest req, int packId);

        IObservable<ChildItemRewardsDto> GetChildGameRewards(int childId, int gameId);
        IObservable<IList<ChildPackProgressDto>> GetChildPacksProgress(int childId);
        IObservable<IList<ChildItemProgressDto>> GetChildItemsProgress(int childId, int packId);
        
        void InvalidateChildGameProgress(int childId, int packId, int gameId);
    }

    public class GameService : IGameService
    {
        private readonly IBlobCache _cache;
        private readonly IAssetRepository _assetRepository;
        private readonly IApi<ITalkiPlayApi> _api;

        public GameService(IApi<ITalkiPlayApi> api = null,
            IAssetRepository assetRepository = null)
        {
            _cache = Locator.Current.GetService<IBlobCache>();
            _api = api ?? Locator.Current.GetService<IApi<ITalkiPlayApi>>();
            _assetRepository = assetRepository ?? Locator.Current.GetService<IAssetRepository>();
        }

        public async Task<IList<GameDto>> GetGames()
        {
            var result = await _api.Client.GetGames().ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            return result.Result;
        }

        public async Task<IGame> GetGameWithAssets(int id, IList<int> tagIds)
        {
            var game = await GetGame(id, tagIds);
            return await LoadAssets(game);
            
            //return GetGame(id, tagIds).SelectMany(LoadAssets);
        }

        public async Task<GameDto> GetGame(int id)
        {
            var result = await _api.Client.GetGame(id).ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            return result.Result;
        }
        
        async Task<IGame> GetGame(int id, IList<int> tagIds)
        {
            if (tagIds == null)
            {
                tagIds = new List<int>();
            }

            if (tagIds.Count == 0)
            {
                tagIds.Add(0);
            }

            //var result = await _api.Client.GetGame(id, tagIds?.ToArray()).ToResult();
            var result = await _api.Client.GetGame(id).ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            return result.Result;
        }
        
        public async Task<RewardDto> RecordGameResult(RecordGameSessionResultRequest req, int packId)
        {
            var result = await _api.Client.RecordGameSessionResult(req.GameId, req).ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            foreach (var childId in req.Children)
            {
                InvalidateChildGameProgress(childId, packId, req.GameId);
            }

            return result.Result;
        }

        public IObservable<RewardDto> RecordGameResultOld(RecordGameSessionResultRequest req, int packId)
        {
            var result = _api.Client.RecordGameSessionResult(req.GameId, req);

            foreach(var childId in req.Children)
            {
                InvalidateChildGameProgress(childId, packId, req.GameId);
            }

            return result;
        }

        private IObservable<IGame> LoadAssets(IGame m)
        {
            //if (m?.Items == null)
            //{
            //    return Observable.Return(m);
            //}

            //if (m.Items.Count > 0)
            //{
            //    return LoadAssetsFromItems(m);
            //}

            return m.Instructions.Count > 0 ? LoadAssetsFromInstructions(m) : Observable.Return(m);
        }

        private IObservable<IGame> LoadAssetsFromInstructions(IGame m)
        {
            return m.Instructions.ToObservable()
                .Select(i => (i.Item, i.Modes.FirstOrDefault(g => g.Type == InstructionModeType.Reward)))
                .Select(i => (i.Item1, i.Item2))
                .SelectMany(g => LoadAsset(g.Item1, g.Item2))
                .ToList()
                .Select(_ => m);
        }

        // private IObservable<IGame> LoadAssetsFromItems(IGame m)
        // {
        //     return m.Items.ToObservable()
        //         .SelectMany(LoadAsset)
        //         .ToList()
        //         .Select(_ => m);
        // }

        private IObservable<IItem> LoadAsset(IItem item, IMode mode)
        {
            if (mode.AudioAssetId == null 
                && mode.ImageAssetId == null) 
                return Observable.Return(item);

            var ids = new List<int>();

            if (mode.AudioAssetId != null)
            {
                ids.Add(mode.AudioAssetId.Value);
            }

            if (mode.ImageAssetId != null)
            {
                ids.Add(mode.ImageAssetId.Value);
            }

            return _assetRepository.GetAssetsByIds(ids)
                .Do(assets => { item.Assets = assets; })
                .Select(_ => item);
        }

        // private IObservable<IItem> LoadAsset(IItem item)
        // {
        //     return _assetRepository.GetAssetsByIds(item.AssetIds)
        //         .Do(assets => { item.Assets = assets; })
        //         .Select(_ => item);
        // }

        private string GetChildGameRewardKey(int childId, int gameId)
        {
            return $"ItemReward_ByChild_{childId}_ByGame_{gameId}";
        }

        private IObservable<ChildItemRewardsDto> _GetChildGameRewards(int childId, int gameId)
        {
            return _api.Client.GetChildGameRewards(childId, gameId)
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

        public IObservable<ChildItemRewardsDto> GetChildGameRewards(int childId, int gameId)
        {
            return _cache.GetOrFetchObject(GetChildGameRewardKey(childId, gameId), () => _GetChildGameRewards(childId, gameId), DateTimeOffset.Now.AddHours(5));
        }

        private string GetChildPacksProgressKey(int childId)
        {
            return $"PacksProgress_ByChild_{childId}";
        }

        private IObservable<IList<ChildPackProgressDto>> GetChildPacksProgressApi(int childId)
        {
            return _api.Client.GetChildPacksProgress(childId)
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

        public IObservable<IList<ChildPackProgressDto>> GetChildPacksProgress(int childId)
        {
            return _cache.GetOrFetchObject(GetChildPacksProgressKey(childId), () => GetChildPacksProgressApi(childId), DateTimeOffset.Now.AddHours(5));
        }

        private string GetChildItemsProgressKey(int childId, int packId)
        {
            return $"ItemProgress_ByChild_{childId}_ByPack_{packId}";
        }

        private IObservable<IList<ChildItemProgressDto>> GetChildItemsProgressApi(int childId, int packId)
        {
            return _api.Client.GetChildItemsProgress(childId, packId)
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

        public IObservable<IList<ChildItemProgressDto>> GetChildItemsProgress(int childId, int packId)
        {
            return _cache.GetOrFetchObject(GetChildItemsProgressKey(childId, packId), () => GetChildItemsProgressApi(childId, packId), DateTimeOffset.Now.AddHours(5));
        }


        public void InvalidateChildGameProgress(int childId, int packId, int gameId)
        {
            _cache.Invalidate(GetChildPacksProgressKey(childId));
            _cache.Invalidate(GetChildItemsProgressKey(childId, packId));
            _cache.Invalidate(GetChildGameRewardKey(childId, gameId));
        }
    }
}