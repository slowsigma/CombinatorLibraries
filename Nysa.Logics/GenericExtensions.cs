using System;
using System.Collections.Generic;

namespace Nysa.Logics
{

    public static class GenericExtensions
    {
        public static U Map<T, U>(this T? value, Func<T, U> transform, U @default)
            where T : struct
            => value == null ? @default : transform(value.Value);

        public static void Process<T>(this T value, Action<T> action) => action(value);
        public static U Map<T, U>(this T value, Func<T, U> transform) => transform(value);

        public static void Process<T>(this IEnumerable<T> set, Action<T> action)
        {
            foreach (T item in set) action(item);
        }
    }

}
