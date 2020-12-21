using System;
using System.Collections.Generic;
using System.Linq;

namespace GatekeeperCSharp.Common
{
    public static class StringExtensions
    {
        public static string Obfuscate(this string str, char obfuscator = '*')
        {
            return new string(obfuscator, str.Length);
        }
    }

    public static class IEnumerableExtensions
    {
        public static string Stringify<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector, string separator = ", ")
        {
            return string.Join(separator, enumerable.Select(selector));
        }
    }
}
