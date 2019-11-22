using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nysa.Logics
{

    public static class Suspect
    {
        private static readonly String _BindErrorMessage = @"Bind does not accept types other than Confirmed<T> and Failed<T>.";
        private static readonly String _AffectErrorMessage = @"Affect does not accept types other than Confirmed<T> and Failed<T>.";

        public static Suspect<T> Try<T>(this Func<T> fallible, String errorMessage = "")
        {
            try { return fallible(); }
            catch (Exception except) { return String.IsNullOrWhiteSpace(errorMessage) ? except : new Exception(errorMessage, except); }
        }

        public static Suspect<T> Failed<T>(String errorMessage)
            => new Failed<T>(new Exception(errorMessage));

        public static Func<Suspect<T>> AsSuspect<T>(this Func<T> fallible, String errorMessage = "")
            => () =>
            {
                try { return fallible(); }
                catch (Exception except)
                {
                    return String.IsNullOrWhiteSpace(errorMessage)
                           ? except
                           : new Exception(errorMessage, except);
                }
            };

        public static Func<T, Suspect<U>> AsSuspect<T, U>(this Func<T, U> fallible, String errorMessage = "")
            => t =>
            {
                try { return fallible(t); }
                catch (Exception except)
                {
                    return String.IsNullOrWhiteSpace(errorMessage)
                           ? (Failed<U>)except
                           : new Exception(errorMessage, except);
                }
            };

        public static Suspect<T> Checked<T>(this T value, Func<T, Boolean> validator, String errorMessage)
            => validator(value) ? value.ToSuspect() : new Exception(errorMessage);

        public static Suspect<T> Checked<T>(this T value, params (Func<T, Boolean> validator, String errorMessage)[] checks)
            => checks.Select(p => p.validator(value)
                                  ? null
                                  : new Exception(p.errorMessage))
                     .FirstOrDefault(e => e != null)
                     ?? value.ToSuspect();

        public static Suspect<T> Checked<T>(this Suspect<T> value, Func<T, Boolean> validator, String errorMessage)
            => value is Confirmed<T> confirmed
               ? validator(confirmed.Value)
                 ? value
                 : new Exception(errorMessage)
               : value;

        public static Suspect<T> ToSuspect<T>(this T value)
            => new Confirmed<T>(value);

        public static B Bind<A, B>(this Suspect<A> a, Func<A, B> confirmedMap, Func<Exception, B> failedMap)
            =>   (a is Confirmed<A> confirmed) ? confirmedMap(confirmed.Value)
               : (a is Failed<A> failed)       ? failedMap(failed.Value)
               :                                 throw new Exception(_BindErrorMessage);

        public static async Task<B> BindAsync<A, B>(this Suspect<A> a, Func<A, Task<B>> confirmedMapAsync, Func<Exception, B> failedMap)
            =>   (a is Confirmed<A> confirmed) ? await confirmedMapAsync(confirmed.Value)
               : (a is Failed<A> failed)       ? failedMap(failed.Value)
               :                                 throw new Exception(_BindErrorMessage);

        public static Suspect<B> Bind<A, B>(this Suspect<A> a, Func<A, B> map)
            =>   (a is Confirmed<A> confirmed) ? map(confirmed.Value).ToSuspect<B>()
               : (a is Failed<A> failed)       ? failed.Value
               :                                 throw new Exception(_BindErrorMessage);

        public static async Task<Suspect<B>> BindAsync<A, B>(this Suspect<A> a, Func<A, Task<B>> mapAsync)
            =>   (a is Confirmed<A> confirmed) ? (await mapAsync(confirmed.Value)).ToSuspect<B>()
               : (a is Failed<A> failed)       ? failed.Value
               :                                 throw new Exception(_BindErrorMessage);

        public static Suspect<B> Bind<A, B>(this Suspect<A> a, Func<A, Suspect<B>> map)
            =>   (a is Confirmed<A> confirmed) ? map(confirmed)
               : (a is Failed<A> failed)       ? failed.Value
               :                                 throw new Exception(_BindErrorMessage);

        public static Suspect<C> Bind<A, B, C>(this Suspect<A> a, Suspect<B> b, Func<A, B, Suspect<C>> map)
            =>   (   a is Confirmed<A> a_confirmed
                  && b is Confirmed<B> b_confirmed) ? map(a_confirmed, b_confirmed)
               : (a is Failed<A> a_failed)          ? a_failed.Value
               : (b is Failed<B> b_failed)          ? b_failed.Value
               :                                      throw new Exception(_BindErrorMessage);

        public static Suspect<C> SelectMany<A, B, C>(this Suspect<A> a, Func<A, Suspect<B>> map, Func<A, B, C> select)
            => a.Bind(a_value => map(a_value).Bind(b_value => select(a_value, b_value).ToSuspect()));

        public static Suspect<C> Bind<A, B, C>(this Suspect<A> a, Suspect<B> b, Func<A, B, C> map)
            => a.Bind(a_value => b.Bind(b_value => map(a_value, b_value)));

        public static Suspect<D> Bind<A, B, C, D>(this Suspect<A> a, Suspect<B> b, Suspect<C> c, Func<A, B, C, D> map)
            => a.Bind(a_value => b.Bind(b_value => c.Bind(c_value => map(a_value, b_value, c_value))));

        public static Suspect<E> Bind<A, B, C, D, E>(this Suspect<A> a, Suspect<B> b, Suspect<C> c, Suspect<D> d, Func<A, B, C, D, E> map)
            => a.Bind(a_value => b.Bind(b_value => c.Bind(c_value => d.Bind(d_value => map(a_value, b_value, c_value, d_value)))));

        public static Suspect<F> Bind<A, B, C, D, E, F>(this Suspect<A> a, Suspect<B> b, Suspect<C> c, Suspect<D> d, Suspect<E> e, Func<A, B, C, D, E, F> map)
            => a.Bind(a_value => b.Bind(b_value => c.Bind(c_value => d.Bind(d_value => e.Bind(e_value => map(a_value, b_value, c_value, d_value, e_value))))));

        public static Suspect<G> Bind<A, B, C, D, E, F, G>(this Suspect<A> a, Suspect<B> b, Suspect<C> c, Suspect<D> d, Suspect<E> e, Suspect<F> f, Func<A, B, C, D, E, F, G> map)
            => a.Bind(a_value => b.Bind(b_value => c.Bind(c_value => d.Bind(d_value => e.Bind(e_value => f.Bind(f_value => map(a_value, b_value, c_value, d_value, e_value, f_value)))))));

        public static Suspect<H> Bind<A, B, C, D, E, F, G, H>(this Suspect<A> a, Suspect<B> b, Suspect<C> c, Suspect<D> d, Suspect<E> e, Suspect<F> f, Suspect<G> g, Func<A, B, C, D, E, F, G, H> map)
            => a.Bind(a_value => b.Bind(b_value => c.Bind(c_value => d.Bind(d_value => e.Bind(e_value => f.Bind(f_value => g.Bind(g_value => map(a_value, b_value, c_value, d_value, e_value, f_value, g_value))))))));

        public static Suspect<I> Bind<A, B, C, D, E, F, G, H, I>(this Suspect<A> a, Suspect<B> b, Suspect<C> c, Suspect<D> d, Suspect<E> e, Suspect<F> f, Suspect<G> g, Suspect<H> h, Func<A, B, C, D, E, F, G, H, I> map)
            => a.Bind(a_value => b.Bind(b_value => c.Bind(c_value => d.Bind(d_value => e.Bind(e_value => f.Bind(f_value => g.Bind(g_value => h.Bind(h_value => map(a_value, b_value, c_value, d_value, e_value, f_value, g_value, h_value)))))))));

        public static Suspect<D> Bind<A, B, C, D>(this Suspect<A> a, Func<Suspect<B>> getB, Func<Suspect<C>> getC, Func<A, B, C, D> map)
            => a.Bind(a_value => getB().Bind(b_value => getC().Bind(c_value => map(a_value, b_value, c_value))));

        public static void Affect<T>(this Suspect<T> suspect, Action<T> whenConfirmed, Action<Exception> whenFailed = null)
        {
            if (suspect is Confirmed<T> confirmed)
                whenConfirmed(confirmed.Value);
            else if (suspect is Failed<T> failed)
            {
                if (whenFailed != null)
                    whenFailed(failed.Value);
            }
            else
                throw new Exception(_AffectErrorMessage);
        }

        public static (IEnumerable<T> Confirmed, IEnumerable<Exception> Failed) Split<T>(this IEnumerable<Suspect<T>> suspectItems)
            => (Confirmed: suspectItems.Select(s => s as Confirmed<T>).Where(c => c != null).Select(v => v.Value),
                Failed:    suspectItems.Select(s => s as Failed<T>).Where(f => f != null).Select(v => v.Value));
    }

}
