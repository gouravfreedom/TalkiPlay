using System;
using System.Collections.Generic;
using System.Linq;

namespace TalkiPlay.Shared
{
    public static class IListExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var r = new Random();
            var shuffledList =
                list.
                    Select(x => new { Number = r.Next(), Item = x }).
                    OrderBy(x => x.Number).
                    Select(x => x.Item);                    

            return shuffledList.ToList();
        }
    }
}