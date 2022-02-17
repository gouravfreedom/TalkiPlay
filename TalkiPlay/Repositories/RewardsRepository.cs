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
    public interface IRewardsRepository
    {
        Task<IList<RewardDto>> GetAllRewards();
        Task<IList<ChildRewardDto>> GetChildRewards(int childId);
    }
    
    public class RewardsRepository : IRewardsRepository
    {
        private readonly IBlobCache _cache;
        private readonly IApi<ITalkiPlayApi> _api;

        public RewardsRepository(IApi<ITalkiPlayApi> api = null,
            IBlobCache cache = null)
        {
            _cache = cache ?? Locator.Current.GetService<IBlobCache>();            
            _api = api ?? Locator.Current.GetService<IApi<ITalkiPlayApi>>();   
        }
        
        public async Task<IList<RewardDto>> GetAllRewards()
        {
            var results = await _cache.GetOrFetchObject("Rewards", 
                GetAllRewardsList, 
                DateTimeOffset.Now.AddHours(3));
            if (results.Count > 0)
            {
                return results;
            }
            else
            {
                await _cache.InvalidateObject<List<RewardDto>>("Rewards");
                return new List<RewardDto>();
            }
        }
        
        async Task<IList<RewardDto>> GetAllRewardsList()
        {
            var result = await _api.Client.GetRewards().ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            return result.Result;
        }
        
        public async Task<IList<ChildRewardDto>> GetChildRewards(int childId)
        {
            var result = await _api.Client.GetChildRewards(childId).ToResult();
            if (!result.IsSuccessful)
            {
                throw result.Exception;
            }

            return result.Result;
        }
    }
}