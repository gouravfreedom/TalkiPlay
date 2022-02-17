using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.Api;
using DynamicData;
using Splat;

namespace TalkiPlay.Shared
{
    public class GuideState
    {
        public GuideState(GuideStep startStep = GuideStep.Welcome)
        {
            StartStep = startStep;
        }
        public GuideStep StartStep { get; }
        public IChild SelectedChild { get; set; }
        
        public IPack SelectedPack { get; set; }
        
        public GuideInterests SelectedInterest { get; set; }
        public bool ShouldRlaceMainPage { get; set; } = false;
        public bool IsModal { get; set; } = false;
        public bool EnableBackAtStart { get; set; } = true;


        GameLevel GetRecommendedLevel()
        {
            switch (SelectedChild.CommunicationLevel)
            {
                case null:
                case ChildCommunicationLevel.NoWords:
                case ChildCommunicationLevel.AFewWords:
                    return GameLevel.Early;
                default:
                    return GameLevel.Learning;
            }
        }

        bool HuntTypeRecommended =>
            SelectedChild.ResponseLevel != null && SelectedChild.ResponseLevel.Value != ChildResponseLevel.AllTheTime;

        public async  Task<List<GameDto>> GetGameRecommendations()
        {
            var service = Locator.Current.GetService<IGameService>();
            var games = await service.GetGames();
        
            var repository = Locator.Current.GetService<IAssetRepository>();
            var packs = await repository.GetPacks();
            //var recommendedPacks = State.GetRecommendedPacks(packs);

            var recommendedPacks = new List<IPack> { SelectedPack };
            
            var simplePackName = SelectedPack.Name
                .Replace("Explore", "").Trim(); //StringComparison.InvariantCultureIgnoreCase
            
            var smartPacks = packs
                .Where(p => p.Name.IndexOf("Smart", StringComparison.InvariantCultureIgnoreCase) >=0 &&
                            p.Name.IndexOf(simplePackName, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
            recommendedPacks.AddRange(smartPacks);
            
            var packGames = games
                .Where(g => recommendedPacks.Any(p => p.Id == g.PackId))
                .Where(g => (g.Level == GetRecommendedLevel() && g.Type == GameType.Explore) 
                            || (g.Type == GameType.Hunt && HuntTypeRecommended))
                .DistinctBy(g => g.Id)
                .ToList();

            if (packGames.Count == 0)
            {
                packGames = games.Where(g =>
                        g.Name.Equals("Room Explore",
                            StringComparison.InvariantCultureIgnoreCase) ||
                        g.Name.Equals("Room Hunt", StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            return packGames;
        }


        public void SaveRecommendedGames(List<GameDto> games)
        {
            var userSettings = Locator.Current.GetService<IUserSettings>();
            var currentGames = userSettings.RecommendedGames;
            
            currentGames.Remove(currentGames.Where(g => g.ChildId == SelectedChild.Id).ToList());

            foreach (var game in games)
            {
                currentGames.Add(new RecommendedGame(game.Id, SelectedChild.Id));
            }

            userSettings.RecommendedGames = currentGames;
        }
    }
}