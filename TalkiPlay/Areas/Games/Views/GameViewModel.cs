﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using Humanizer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class GameViewModel : ReactiveObject
    {
        readonly ReactiveCommand<IGame, Unit> _selectCommand;
        
        public GameViewModel( 
            IGame game,
            ReactiveCommand<IGame, Unit> selectCommand, IList<IChild> children, bool userHasSubscription, IUserSettings settings)
        {
            _selectCommand = selectCommand;
            Game = game;
            
            
            IsLocked = game.AccessLevel != GameAccessLevel.Free && !userHasSubscription;
            ShowGuideButton = !settings.IsGuideCompleted; 
                
            Title = game.Name;
            ShortDescription = game.ShortDescription;
            BackgroundImage = game.ImagePath.ToResizedImage(height: 250);
            Description = game.Description;
            GameType = game.Type.Humanize();

            SetRecommendationText(children, settings);
            
            SelectCommand = ReactiveCommand.CreateFromObservable(() => _selectCommand?.Execute(Game));

            GuideCommand = ReactiveCommand.Create(GuideHelper.StartGuide);

            DownloadCommand = ReactiveCommand.Create(() =>
            {

            });
        }
        
        [Reactive]
        public string ShortDescription { get; set; }
        
        [Reactive]
        public string Title { get; set; }
        
        [Reactive]
        public string BackgroundImage { get; set; }
        
        [Reactive]
        public string Description { get; set; }

        [Reactive]
        public string GameType { get; set; }

        [Reactive]
        public bool IsLocked { get; set; }
        
        [Reactive]
        public string RecommendationText { get; private set; }

        public bool HasRecommendationText => !string.IsNullOrEmpty(RecommendationText);
        public IGame Game { get; }
        
        public ReactiveCommand<Unit,Unit> SelectCommand { get;  }
        
        public ReactiveCommand<Unit,Unit> GuideCommand { get; }
        
        public ReactiveCommand<Unit,Unit> DownloadCommand { get; }
        
        public bool ShowGuideButton { get; }
        
        void SetRecommendationText(IList<IChild> children, IUserSettings settings)
        {
            var games = settings.RecommendedGames
                .Where(g => g.GameId == Game.Id)
                .ToList();

            if (games.Count == 0)
            {
                RecommendationText = "";
                return;
            }
            
            var childNames = new List<string>();
            foreach (var game in games)
            {
                var child = children.FirstOrDefault(c => c.Id == game.ChildId);
                if (child != null)
                {
                    childNames.Add(child.Name);
                }
            }

            if (childNames.Count > 0)
            {
                var result = new StringBuilder("Recommended for ");

                for (var i = 0; i < childNames.Count; ++i)
                {
                    result.Append(childNames[i]);
                    if (i < childNames.Count - 1)
                    {
                        result.Append(", ");
                    }
                }

                RecommendationText = result.ToString();
            }
            else
            {
                RecommendationText = "";
            }
        }
        
    }
}