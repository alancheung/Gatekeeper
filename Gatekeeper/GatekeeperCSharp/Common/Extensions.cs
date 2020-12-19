using System;
using System.Collections.Generic;
using System.Linq;

namespace GatekeeperCSharp.Common
{
    public static class IEnumerableExtensions
    {
        public static string Stringify<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
        {
            return string.Join(", ", enumerable.Select(selector));
        }
    }
}
