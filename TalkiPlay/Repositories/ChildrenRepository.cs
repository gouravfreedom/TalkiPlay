using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Akavache;
using ChilliSource.Mobile.Api;
using Splat;

namespace TalkiPlay.Shared
{
    public interface IChildrenRepository
    {
        IChild ActiveChild { get; }
        IObservable<IChild> GetChild(int id);
        IObservable<IList<IChild>> GetChildren();
        IObservable<IChild> AddOrUpdateChild(IChild child);
        IObservable<IChild> UpdateChild(IChild child);
        IObservable<IChild> AddChild(IChild child);
    }

    public class ChildrenRepository : IChildrenRepository
    {
        private readonly IBlobCache _cache;
        private readonly IApi<ITalkiPlayApi> _api;
        
        public ChildrenRepository(
            IApi<ITalkiPlayApi> api = null,
            IBlobCache cache = null)
        {
            _cache = cache ?? Locator.Current.GetService<IBlobCache>();            
            _api = api ?? Locator.Current.GetService<IApi<ITalkiPlayApi>>();
        }

        public IChild ActiveChild
        {
            get
            {
                var userSettings = Locator.Current.GetService<IUserSettings>();
                if (userSettings.CurrentChild == null)
                {
                    var children = GetChildren().Wait();
                    userSettings.CurrentChild = children.FirstOrDefault();
                }

                return userSettings.CurrentChild;
            }
        }
        
        public IObservable<IChild> GetChild(int id)
        {
            return _cache.GetOrFetchObject(GetChildId(id), () => GetChildById(id),
                DateTimeOffset.Now.AddHours(1));
            
        }
        
        private static string GetChildId(int id)
        {
            return $"child_{id}";
        }
        
        private IObservable<ChildDto> GetChildById(int id)
        {
            return _api.Client.GetChild(id)
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful )
                    {
                        throw m.Exception;
                    }
                })
                .Select(r => r.Result);
        }

        public IObservable<IList<IChild>> GetChildren()
        {
            return _cache.GetOrFetchObject("Children", GetChildrenList, DateTimeOffset.Now.AddHours(3))
                .SelectMany(m => m.Count > 0 ? Observable.Return(m) : _cache.InvalidateObject<List<ChildDto>>("Children").Select(_ => m))
                .Select(m => m.ToList<IChild>());
        }
        
        IObservable<IList<ChildDto>> GetChildrenList()
        {
            return _api.Client.GetChildren()
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful )
                    {
                        throw m.Exception;
                    }
                })
                .Select(r => r.Result);
        }

        public IObservable<IChild> AddOrUpdateChild(IChild child)
        {
            return child.Id > 0 ? UpdateChild(child) : AddChild(child);
        }

        public IObservable<IChild> UpdateChild(IChild child)
        {
            return  _api.Client.UpdateChild(child.Id, new AddUpdateChildRequest(child))
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful )
                    {
                        throw m.Exception;
                    }

                    var settings = Locator.Current.GetService<IUserSettings>();
                    if (settings.CurrentChild?.Id == m.Result?.Id)
                    {
                        settings.CurrentChild = m.Result;
                    }
                })
                .SelectMany(r => _cache.InvalidateObject<List<ChildDto>>("Children").Select(_ => r))
                .SelectMany(r => _cache.InvalidateObject<ChildDto>(GetChildId(child.Id)).Select(_ => r))
                .Select(r => r.Result);
        }

        public IObservable<IChild> AddChild(IChild child)
        {
            return  _api.Client.AddChild(new AddUpdateChildRequest(child))
                .ToResult()
                .Do(m =>
                {
                    if (!m.IsSuccessful )
                    {
                        throw m.Exception;
                    }
                })
                .SelectMany(r => _cache.InvalidateObject<List<ChildDto>>("Children").Select(_ => r))
                .SelectMany(r => _cache.InvalidateObject<ChildDto>(GetChildId(child.Id)).Select(_ => r))
                .Select(r => r.Result);
        }
    }
}