using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UniLinq
{
	public static class Enumerable
	{
		private enum Fallback
		{
			Default,
			Throw
		}

		public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
		{
			Check.SourceAndFunc(source, func);
			TSource result;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Enumerable.EmptySequence();
				}
				TSource tSource = enumerator.Current;
				while (enumerator.MoveNext())
				{
					tSource = func(tSource, enumerator.Current);
				}
				result = tSource;
			}
			return result;
		}

		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			Check.SourceAndFunc(source, func);
			TAccumulate tAccumulate = seed;
			foreach (TSource current in source)
			{
				tAccumulate = func(tAccumulate, current);
			}
			return tAccumulate;
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			Check.SourceAndFunc(source, func);
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			TAccumulate arg = seed;
			foreach (TSource current in source)
			{
				arg = func(arg, current);
			}
			return resultSelector(arg);
		}

		public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			foreach (TSource current in source)
			{
				if (!predicate(current))
				{
					return false;
				}
			}
			return true;
		}

		public static bool Any<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				return collection.Count > 0;
			}
			bool result;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				result = enumerator.MoveNext();
			}
			return result;
		}

		public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			foreach (TSource current in source)
			{
				if (predicate(current))
				{
					return true;
				}
			}
			return false;
		}

		public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
		{
			return source;
		}

		public static double Average(this IEnumerable<int> source)
		{
			Check.Source(source);
			long num = 0L;
			int num2 = 0;
			foreach (int current in source)
			{
				checked
				{
					num += unchecked((long)current);
				}
				num2++;
			}
			if (num2 == 0)
			{
				throw Enumerable.EmptySequence();
			}
			return (double)num / (double)num2;
		}

		public static double Average(this IEnumerable<long> source)
		{
			Check.Source(source);
			long num = 0L;
			long num2 = 0L;
			foreach (long current in source)
			{
				num += current;
				num2 += 1L;
			}
			if (num2 == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return (double)num / (double)num2;
		}

		public static double Average(this IEnumerable<double> source)
		{
			Check.Source(source);
			double num = 0.0;
			long num2 = 0L;
			foreach (double num3 in source)
			{
				num += num3;
				num2 += 1L;
			}
			if (num2 == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return num / (double)num2;
		}

		public static float Average(this IEnumerable<float> source)
		{
			Check.Source(source);
			float num = 0f;
			long num2 = 0L;
			foreach (float num3 in source)
			{
				num += num3;
				num2 += 1L;
			}
			if (num2 == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return num / (float)num2;
		}

		public static decimal Average(this IEnumerable<decimal> source)
		{
			Check.Source(source);
			decimal d = 0m;
			long num = 0L;
			foreach (decimal current in source)
			{
				d += current;
				num += 1L;
			}
			if (num == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return d / num;
		}

		private static TResult? AverageNullable<TElement, TAggregate, TResult>(this IEnumerable<TElement?> source, Func<TAggregate, TElement, TAggregate> func, Func<TAggregate, long, TResult> result) where TElement : struct where TAggregate : struct where TResult : struct
		{
			Check.Source(source);
			TAggregate arg = default(TAggregate);
			long num = 0L;
			foreach (TElement? current in source)
			{
				if (current.HasValue)
				{
					arg = func(arg, current.Value);
					num += 1L;
				}
			}
			if (num == 0L)
			{
				return null;
			}
			return new TResult?(result(arg, num));
		}

		public static double? Average(this IEnumerable<int?> source)
		{
			Check.Source(source);
			long num = 0L;
			long num2 = 0L;
			foreach (int? current in source)
			{
				if (current.HasValue)
				{
					num += (long)current.Value;
					num2 += 1L;
				}
			}
			if (num2 == 0L)
			{
				return null;
			}
			return new double?((double)num / (double)num2);
		}

		public static double? Average(this IEnumerable<long?> source)
		{
			Check.Source(source);
			long num = 0L;
			long num2 = 0L;
			foreach (long? current in source)
			{
				if (current.HasValue)
				{
					checked
					{
						num += current.Value;
					}
					num2 += 1L;
				}
			}
			if (num2 == 0L)
			{
				return null;
			}
			return new double?((double)num / (double)num2);
		}

		public static double? Average(this IEnumerable<double?> source)
		{
			Check.Source(source);
			double num = 0.0;
			long num2 = 0L;
			foreach (double? current in source)
			{
				if (current.HasValue)
				{
					num += current.Value;
					num2 += 1L;
				}
			}
			if (num2 == 0L)
			{
				return null;
			}
			return new double?(num / (double)num2);
		}

		public static decimal? Average(this IEnumerable<decimal?> source)
		{
			Check.Source(source);
			decimal d = 0m;
			long num = 0L;
			foreach (decimal? current in source)
			{
				if (current.HasValue)
				{
					d += current.Value;
					num += 1L;
				}
			}
			if (num == 0L)
			{
				return null;
			}
			return new decimal?(d / num);
		}

		public static float? Average(this IEnumerable<float?> source)
		{
			Check.Source(source);
			float num = 0f;
			long num2 = 0L;
			foreach (float? current in source)
			{
				if (current.HasValue)
				{
					num += current.Value;
					num2 += 1L;
				}
			}
			if (num2 == 0L)
			{
				return null;
			}
			return new float?(num / (float)num2);
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector(source, selector);
			long num = 0L;
			long num2 = 0L;
			foreach (TSource current in source)
			{
				num += (long)selector(current);
				num2 += 1L;
			}
			if (num2 == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return (double)num / (double)num2;
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector(source, selector);
			long num = 0L;
			long num2 = 0L;
			foreach (TSource current in source)
			{
				int? num3 = selector(current);
				if (num3.HasValue)
				{
					num += (long)num3.Value;
					num2 += 1L;
				}
			}
			if (num2 == 0L)
			{
				return null;
			}
			return new double?((double)num / (double)num2);
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector(source, selector);
			long num = 0L;
			long num2 = 0L;
			foreach (TSource current in source)
			{
				checked
				{
					num += selector(current);
				}
				num2 += 1L;
			}
			if (num2 == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return (double)num / (double)num2;
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector(source, selector);
			long num = 0L;
			long num2 = 0L;
			foreach (TSource current in source)
			{
				long? num3 = selector(current);
				if (num3.HasValue)
				{
					checked
					{
						num += num3.Value;
					}
					num2 += 1L;
				}
			}
			if (num2 == 0L)
			{
				return null;
			}
			return new double?((double)num / (double)num2);
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector(source, selector);
			double num = 0.0;
			long num2 = 0L;
			foreach (TSource current in source)
			{
				num += selector(current);
				num2 += 1L;
			}
			if (num2 == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return num / (double)num2;
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector(source, selector);
			double num = 0.0;
			long num2 = 0L;
			foreach (TSource current in source)
			{
				double? num3 = selector(current);
				if (num3.HasValue)
				{
					num += num3.Value;
					num2 += 1L;
				}
			}
			if (num2 == 0L)
			{
				return null;
			}
			return new double?(num / (double)num2);
		}

		public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector(source, selector);
			float num = 0f;
			long num2 = 0L;
			foreach (TSource current in source)
			{
				num += selector(current);
				num2 += 1L;
			}
			if (num2 == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return num / (float)num2;
		}

		public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector(source, selector);
			float num = 0f;
			long num2 = 0L;
			foreach (TSource current in source)
			{
				float? num3 = selector(current);
				if (num3.HasValue)
				{
					num += num3.Value;
					num2 += 1L;
				}
			}
			if (num2 == 0L)
			{
				return null;
			}
			return new float?(num / (float)num2);
		}

		public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector(source, selector);
			decimal d = 0m;
			long num = 0L;
			foreach (TSource current in source)
			{
				d += selector(current);
				num += 1L;
			}
			if (num == 0L)
			{
				throw Enumerable.EmptySequence();
			}
			return d / num;
		}

		public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector(source, selector);
			decimal d = 0m;
			long num = 0L;
			foreach (TSource current in source)
			{
				decimal? num2 = selector(current);
				if (num2.HasValue)
				{
					d += num2.Value;
					num += 1L;
				}
			}
			if (num == 0L)
			{
				return null;
			}
			return new decimal?(d / num);
		}

		public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
		{
			Check.Source(source);
			IEnumerable<TResult> enumerable = source as IEnumerable<TResult>;
			if (enumerable != null)
			{
				return enumerable;
			}
			return Enumerable.CreateCastIterator<TResult>(source);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateCastIterator<TResult>(IEnumerable source)
		{
			Enumerable.<CreateCastIterator>c__Iterator1E6<TResult> <CreateCastIterator>c__Iterator1E = new Enumerable.<CreateCastIterator>c__Iterator1E6<TResult>();
			<CreateCastIterator>c__Iterator1E.source = source;
			<CreateCastIterator>c__Iterator1E.<$>source = source;
			Enumerable.<CreateCastIterator>c__Iterator1E6<TResult> expr_15 = <CreateCastIterator>c__Iterator1E;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			Check.FirstAndSecond(first, second);
			return Enumerable.CreateConcatIterator<TSource>(first, second);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateConcatIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			Enumerable.<CreateConcatIterator>c__Iterator1E7<TSource> <CreateConcatIterator>c__Iterator1E = new Enumerable.<CreateConcatIterator>c__Iterator1E7<TSource>();
			<CreateConcatIterator>c__Iterator1E.first = first;
			<CreateConcatIterator>c__Iterator1E.second = second;
			<CreateConcatIterator>c__Iterator1E.<$>first = first;
			<CreateConcatIterator>c__Iterator1E.<$>second = second;
			Enumerable.<CreateConcatIterator>c__Iterator1E7<TSource> expr_23 = <CreateConcatIterator>c__Iterator1E;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
		{
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				return collection.Contains(value);
			}
			return source.Contains(value, null);
		}

		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			Check.Source(source);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			foreach (TSource current in source)
			{
				if (comparer.Equals(current, value))
				{
					return true;
				}
			}
			return false;
		}

		public static int Count<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				return collection.Count;
			}
			int num = 0;
			checked
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						num++;
					}
				}
				return num;
			}
		}

		public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndSelector(source, predicate);
			int num = 0;
			checked
			{
				foreach (TSource current in source)
				{
					if (predicate(current))
					{
						num++;
					}
				}
				return num;
			}
		}

		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
		{
			return source.DefaultIfEmpty(default(TSource));
		}

		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
		{
			Check.Source(source);
			return Enumerable.CreateDefaultIfEmptyIterator<TSource>(source, defaultValue);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateDefaultIfEmptyIterator<TSource>(IEnumerable<TSource> source, TSource defaultValue)
		{
			Enumerable.<CreateDefaultIfEmptyIterator>c__Iterator1E8<TSource> <CreateDefaultIfEmptyIterator>c__Iterator1E = new Enumerable.<CreateDefaultIfEmptyIterator>c__Iterator1E8<TSource>();
			<CreateDefaultIfEmptyIterator>c__Iterator1E.source = source;
			<CreateDefaultIfEmptyIterator>c__Iterator1E.defaultValue = defaultValue;
			<CreateDefaultIfEmptyIterator>c__Iterator1E.<$>source = source;
			<CreateDefaultIfEmptyIterator>c__Iterator1E.<$>defaultValue = defaultValue;
			Enumerable.<CreateDefaultIfEmptyIterator>c__Iterator1E8<TSource> expr_23 = <CreateDefaultIfEmptyIterator>c__Iterator1E;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
		{
			return source.Distinct(null);
		}

		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			Check.Source(source);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			return Enumerable.CreateDistinctIterator<TSource>(source, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateDistinctIterator<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			Enumerable.<CreateDistinctIterator>c__Iterator1E9<TSource> <CreateDistinctIterator>c__Iterator1E = new Enumerable.<CreateDistinctIterator>c__Iterator1E9<TSource>();
			<CreateDistinctIterator>c__Iterator1E.comparer = comparer;
			<CreateDistinctIterator>c__Iterator1E.source = source;
			<CreateDistinctIterator>c__Iterator1E.<$>comparer = comparer;
			<CreateDistinctIterator>c__Iterator1E.<$>source = source;
			Enumerable.<CreateDistinctIterator>c__Iterator1E9<TSource> expr_23 = <CreateDistinctIterator>c__Iterator1E;
			expr_23.$PC = -2;
			return expr_23;
		}

		private static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index, Enumerable.Fallback fallback)
		{
			long num = 0L;
			foreach (TSource current in source)
			{
				long arg_1E_0 = (long)index;
				long expr_19 = num;
				num = expr_19 + 1L;
				if (arg_1E_0 == expr_19)
				{
					TSource result = current;
					return result;
				}
			}
			if (fallback == Enumerable.Fallback.Throw)
			{
				throw new ArgumentOutOfRangeException();
			}
			return default(TSource);
		}

		public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
		{
			Check.Source(source);
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return list[index];
			}
			return source.ElementAt(index, Enumerable.Fallback.Throw);
		}

		public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
		{
			Check.Source(source);
			if (index < 0)
			{
				return default(TSource);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return (index >= list.Count) ? default(TSource) : list[index];
			}
			return source.ElementAt(index, Enumerable.Fallback.Default);
		}

		public static IEnumerable<TResult> Empty<TResult>()
		{
			return new TResult[0];
		}

		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Except(second, null);
		}

		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond(first, second);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			return Enumerable.CreateExceptIterator<TSource>(first, second, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateExceptIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Enumerable.<CreateExceptIterator>c__Iterator1EA<TSource> <CreateExceptIterator>c__Iterator1EA = new Enumerable.<CreateExceptIterator>c__Iterator1EA<TSource>();
			<CreateExceptIterator>c__Iterator1EA.second = second;
			<CreateExceptIterator>c__Iterator1EA.comparer = comparer;
			<CreateExceptIterator>c__Iterator1EA.first = first;
			<CreateExceptIterator>c__Iterator1EA.<$>second = second;
			<CreateExceptIterator>c__Iterator1EA.<$>comparer = comparer;
			<CreateExceptIterator>c__Iterator1EA.<$>first = first;
			Enumerable.<CreateExceptIterator>c__Iterator1EA<TSource> expr_31 = <CreateExceptIterator>c__Iterator1EA;
			expr_31.$PC = -2;
			return expr_31;
		}

		private static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Enumerable.Fallback fallback)
		{
			foreach (TSource current in source)
			{
				if (predicate(current))
				{
					TSource result = current;
					return result;
				}
			}
			if (fallback == Enumerable.Fallback.Throw)
			{
				throw Enumerable.NoMatchingElement();
			}
			return default(TSource);
		}

		public static TSource First<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				if (list.Count != 0)
				{
					return list[0];
				}
			}
			else
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
				}
			}
			throw Enumerable.EmptySequence();
		}

		public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.First(predicate, Enumerable.Fallback.Throw);
		}

		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return default(TSource);
		}

		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.First(predicate, Enumerable.Fallback.Default);
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.GroupBy(keySelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			return source.CreateGroupByIterator(keySelector, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<IGrouping<TKey, TSource>> CreateGroupByIterator<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Enumerable.<CreateGroupByIterator>c__Iterator1EB<TSource, TKey> <CreateGroupByIterator>c__Iterator1EB = new Enumerable.<CreateGroupByIterator>c__Iterator1EB<TSource, TKey>();
			<CreateGroupByIterator>c__Iterator1EB.comparer = comparer;
			<CreateGroupByIterator>c__Iterator1EB.source = source;
			<CreateGroupByIterator>c__Iterator1EB.keySelector = keySelector;
			<CreateGroupByIterator>c__Iterator1EB.<$>comparer = comparer;
			<CreateGroupByIterator>c__Iterator1EB.<$>source = source;
			<CreateGroupByIterator>c__Iterator1EB.<$>keySelector = keySelector;
			Enumerable.<CreateGroupByIterator>c__Iterator1EB<TSource, TKey> expr_31 = <CreateGroupByIterator>c__Iterator1EB;
			expr_31.$PC = -2;
			return expr_31;
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.GroupBy(keySelector, elementSelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors(source, keySelector, elementSelector);
			return source.CreateGroupByIterator(keySelector, elementSelector, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<IGrouping<TKey, TElement>> CreateGroupByIterator<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Enumerable.<CreateGroupByIterator>c__Iterator1EC<TSource, TKey, TElement> <CreateGroupByIterator>c__Iterator1EC = new Enumerable.<CreateGroupByIterator>c__Iterator1EC<TSource, TKey, TElement>();
			<CreateGroupByIterator>c__Iterator1EC.comparer = comparer;
			<CreateGroupByIterator>c__Iterator1EC.source = source;
			<CreateGroupByIterator>c__Iterator1EC.keySelector = keySelector;
			<CreateGroupByIterator>c__Iterator1EC.elementSelector = elementSelector;
			<CreateGroupByIterator>c__Iterator1EC.<$>comparer = comparer;
			<CreateGroupByIterator>c__Iterator1EC.<$>source = source;
			<CreateGroupByIterator>c__Iterator1EC.<$>keySelector = keySelector;
			<CreateGroupByIterator>c__Iterator1EC.<$>elementSelector = elementSelector;
			Enumerable.<CreateGroupByIterator>c__Iterator1EC<TSource, TKey, TElement> expr_3F = <CreateGroupByIterator>c__Iterator1EC;
			expr_3F.$PC = -2;
			return expr_3F;
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			return source.GroupBy(keySelector, elementSelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.GroupBySelectors(source, keySelector, elementSelector, resultSelector);
			return source.CreateGroupByIterator(keySelector, elementSelector, resultSelector, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Enumerable.<CreateGroupByIterator>c__Iterator1ED<TSource, TKey, TElement, TResult> <CreateGroupByIterator>c__Iterator1ED = new Enumerable.<CreateGroupByIterator>c__Iterator1ED<TSource, TKey, TElement, TResult>();
			<CreateGroupByIterator>c__Iterator1ED.source = source;
			<CreateGroupByIterator>c__Iterator1ED.keySelector = keySelector;
			<CreateGroupByIterator>c__Iterator1ED.elementSelector = elementSelector;
			<CreateGroupByIterator>c__Iterator1ED.comparer = comparer;
			<CreateGroupByIterator>c__Iterator1ED.resultSelector = resultSelector;
			<CreateGroupByIterator>c__Iterator1ED.<$>source = source;
			<CreateGroupByIterator>c__Iterator1ED.<$>keySelector = keySelector;
			<CreateGroupByIterator>c__Iterator1ED.<$>elementSelector = elementSelector;
			<CreateGroupByIterator>c__Iterator1ED.<$>comparer = comparer;
			<CreateGroupByIterator>c__Iterator1ED.<$>resultSelector = resultSelector;
			Enumerable.<CreateGroupByIterator>c__Iterator1ED<TSource, TKey, TElement, TResult> expr_4F = <CreateGroupByIterator>c__Iterator1ED;
			expr_4F.$PC = -2;
			return expr_4F;
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
		{
			return source.GroupBy(keySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyResultSelectors(source, keySelector, resultSelector);
			return source.CreateGroupByIterator(keySelector, resultSelector, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Enumerable.<CreateGroupByIterator>c__Iterator1EE<TSource, TKey, TResult> <CreateGroupByIterator>c__Iterator1EE = new Enumerable.<CreateGroupByIterator>c__Iterator1EE<TSource, TKey, TResult>();
			<CreateGroupByIterator>c__Iterator1EE.source = source;
			<CreateGroupByIterator>c__Iterator1EE.keySelector = keySelector;
			<CreateGroupByIterator>c__Iterator1EE.comparer = comparer;
			<CreateGroupByIterator>c__Iterator1EE.resultSelector = resultSelector;
			<CreateGroupByIterator>c__Iterator1EE.<$>source = source;
			<CreateGroupByIterator>c__Iterator1EE.<$>keySelector = keySelector;
			<CreateGroupByIterator>c__Iterator1EE.<$>comparer = comparer;
			<CreateGroupByIterator>c__Iterator1EE.<$>resultSelector = resultSelector;
			Enumerable.<CreateGroupByIterator>c__Iterator1EE<TSource, TKey, TResult> expr_3F = <CreateGroupByIterator>c__Iterator1EE;
			expr_3F.$PC = -2;
			return expr_3F;
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.JoinSelectors(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			return outer.CreateGroupJoinIterator(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateGroupJoinIterator<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Enumerable.<CreateGroupJoinIterator>c__Iterator1EF<TOuter, TInner, TKey, TResult> <CreateGroupJoinIterator>c__Iterator1EF = new Enumerable.<CreateGroupJoinIterator>c__Iterator1EF<TOuter, TInner, TKey, TResult>();
			<CreateGroupJoinIterator>c__Iterator1EF.inner = inner;
			<CreateGroupJoinIterator>c__Iterator1EF.innerKeySelector = innerKeySelector;
			<CreateGroupJoinIterator>c__Iterator1EF.comparer = comparer;
			<CreateGroupJoinIterator>c__Iterator1EF.outer = outer;
			<CreateGroupJoinIterator>c__Iterator1EF.outerKeySelector = outerKeySelector;
			<CreateGroupJoinIterator>c__Iterator1EF.resultSelector = resultSelector;
			<CreateGroupJoinIterator>c__Iterator1EF.<$>inner = inner;
			<CreateGroupJoinIterator>c__Iterator1EF.<$>innerKeySelector = innerKeySelector;
			<CreateGroupJoinIterator>c__Iterator1EF.<$>comparer = comparer;
			<CreateGroupJoinIterator>c__Iterator1EF.<$>outer = outer;
			<CreateGroupJoinIterator>c__Iterator1EF.<$>outerKeySelector = outerKeySelector;
			<CreateGroupJoinIterator>c__Iterator1EF.<$>resultSelector = resultSelector;
			Enumerable.<CreateGroupJoinIterator>c__Iterator1EF<TOuter, TInner, TKey, TResult> expr_5F = <CreateGroupJoinIterator>c__Iterator1EF;
			expr_5F.$PC = -2;
			return expr_5F;
		}

		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Intersect(second, null);
		}

		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond(first, second);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			return Enumerable.CreateIntersectIterator<TSource>(first, second, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateIntersectIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Enumerable.<CreateIntersectIterator>c__Iterator1F0<TSource> <CreateIntersectIterator>c__Iterator1F = new Enumerable.<CreateIntersectIterator>c__Iterator1F0<TSource>();
			<CreateIntersectIterator>c__Iterator1F.second = second;
			<CreateIntersectIterator>c__Iterator1F.comparer = comparer;
			<CreateIntersectIterator>c__Iterator1F.first = first;
			<CreateIntersectIterator>c__Iterator1F.<$>second = second;
			<CreateIntersectIterator>c__Iterator1F.<$>comparer = comparer;
			<CreateIntersectIterator>c__Iterator1F.<$>first = first;
			Enumerable.<CreateIntersectIterator>c__Iterator1F0<TSource> expr_31 = <CreateIntersectIterator>c__Iterator1F;
			expr_31.$PC = -2;
			return expr_31;
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.JoinSelectors(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			return outer.CreateJoinIterator(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateJoinIterator<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Enumerable.<CreateJoinIterator>c__Iterator1F1<TOuter, TInner, TKey, TResult> <CreateJoinIterator>c__Iterator1F = new Enumerable.<CreateJoinIterator>c__Iterator1F1<TOuter, TInner, TKey, TResult>();
			<CreateJoinIterator>c__Iterator1F.inner = inner;
			<CreateJoinIterator>c__Iterator1F.innerKeySelector = innerKeySelector;
			<CreateJoinIterator>c__Iterator1F.comparer = comparer;
			<CreateJoinIterator>c__Iterator1F.outer = outer;
			<CreateJoinIterator>c__Iterator1F.outerKeySelector = outerKeySelector;
			<CreateJoinIterator>c__Iterator1F.resultSelector = resultSelector;
			<CreateJoinIterator>c__Iterator1F.<$>inner = inner;
			<CreateJoinIterator>c__Iterator1F.<$>innerKeySelector = innerKeySelector;
			<CreateJoinIterator>c__Iterator1F.<$>comparer = comparer;
			<CreateJoinIterator>c__Iterator1F.<$>outer = outer;
			<CreateJoinIterator>c__Iterator1F.<$>outerKeySelector = outerKeySelector;
			<CreateJoinIterator>c__Iterator1F.<$>resultSelector = resultSelector;
			Enumerable.<CreateJoinIterator>c__Iterator1F1<TOuter, TInner, TKey, TResult> expr_5F = <CreateJoinIterator>c__Iterator1F;
			expr_5F.$PC = -2;
			return expr_5F;
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		private static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Enumerable.Fallback fallback)
		{
			bool flag = true;
			TSource result = default(TSource);
			foreach (TSource current in source)
			{
				if (predicate(current))
				{
					result = current;
					flag = false;
				}
			}
			if (!flag)
			{
				return result;
			}
			if (fallback == Enumerable.Fallback.Throw)
			{
				throw Enumerable.NoMatchingElement();
			}
			return result;
		}

		public static TSource Last<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null && collection.Count == 0)
			{
				throw Enumerable.EmptySequence();
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return list[list.Count - 1];
			}
			bool flag = true;
			TSource result = default(TSource);
			foreach (TSource current in source)
			{
				result = current;
				flag = false;
			}
			if (!flag)
			{
				return result;
			}
			throw Enumerable.EmptySequence();
		}

		public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.Last(predicate, Enumerable.Fallback.Throw);
		}

		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return (list.Count <= 0) ? default(TSource) : list[list.Count - 1];
			}
			bool flag = true;
			TSource result = default(TSource);
			foreach (TSource current in source)
			{
				result = current;
				flag = false;
			}
			if (!flag)
			{
				return result;
			}
			return result;
		}

		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.Last(predicate, Enumerable.Fallback.Default);
		}

		public static long LongCount<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			TSource[] array = source as TSource[];
			if (array != null)
			{
				return array.LongLength;
			}
			long num = 0L;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num += 1L;
				}
			}
			return num;
		}

		public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndSelector(source, predicate);
			long num = 0L;
			foreach (TSource current in source)
			{
				if (predicate(current))
				{
					num += 1L;
				}
			}
			return num;
		}

		public static int Max(this IEnumerable<int> source)
		{
			Check.Source(source);
			bool flag = true;
			int num = -2147483648;
			foreach (int current in source)
			{
				num = Math.Max(current, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static long Max(this IEnumerable<long> source)
		{
			Check.Source(source);
			bool flag = true;
			long num = -9223372036854775808L;
			foreach (long current in source)
			{
				num = Math.Max(current, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static double Max(this IEnumerable<double> source)
		{
			Check.Source(source);
			bool flag = true;
			double num = -1.7976931348623157E+308;
			foreach (double val in source)
			{
				num = Math.Max(val, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static float Max(this IEnumerable<float> source)
		{
			Check.Source(source);
			bool flag = true;
			float num = -3.40282347E+38f;
			foreach (float val in source)
			{
				num = Math.Max(val, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static decimal Max(this IEnumerable<decimal> source)
		{
			Check.Source(source);
			bool flag = true;
			decimal num = -79228162514264337593543950335m;
			foreach (decimal current in source)
			{
				num = Math.Max(current, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static int? Max(this IEnumerable<int?> source)
		{
			Check.Source(source);
			bool flag = true;
			int num = -2147483648;
			foreach (int? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Max(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new int?(num);
		}

		public static long? Max(this IEnumerable<long?> source)
		{
			Check.Source(source);
			bool flag = true;
			long num = -9223372036854775808L;
			foreach (long? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Max(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new long?(num);
		}

		public static double? Max(this IEnumerable<double?> source)
		{
			Check.Source(source);
			bool flag = true;
			double num = -1.7976931348623157E+308;
			foreach (double? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Max(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new double?(num);
		}

		public static float? Max(this IEnumerable<float?> source)
		{
			Check.Source(source);
			bool flag = true;
			float num = -3.40282347E+38f;
			foreach (float? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Max(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new float?(num);
		}

		public static decimal? Max(this IEnumerable<decimal?> source)
		{
			Check.Source(source);
			bool flag = true;
			decimal num = -79228162514264337593543950335m;
			foreach (decimal? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Max(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new decimal?(num);
		}

		public static TSource Max<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			Comparer<TSource> @default = Comparer<TSource>.Default;
			TSource tSource = default(TSource);
			if (default(TSource) == null)
			{
				foreach (TSource current in source)
				{
					if (current != null)
					{
						if (tSource == null || @default.Compare(current, tSource) > 0)
						{
							tSource = current;
						}
					}
				}
			}
			else
			{
				bool flag = true;
				foreach (TSource current2 in source)
				{
					if (flag)
					{
						tSource = current2;
						flag = false;
					}
					else if (@default.Compare(current2, tSource) > 0)
					{
						tSource = current2;
					}
				}
				if (flag)
				{
					throw Enumerable.EmptySequence();
				}
			}
			return tSource;
		}

		public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			int num = -2147483648;
			foreach (TSource current in source)
			{
				num = Math.Max(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			long num = -9223372036854775808L;
			foreach (TSource current in source)
			{
				num = Math.Max(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			double num = -1.7976931348623157E+308;
			foreach (TSource current in source)
			{
				num = Math.Max(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			float num = -3.40282347E+38f;
			foreach (TSource current in source)
			{
				num = Math.Max(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			decimal num = -79228162514264337593543950335m;
			foreach (TSource current in source)
			{
				num = Math.Max(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		private static U Iterate<T, U>(IEnumerable<T> source, U initValue, Func<T, U, U> selector)
		{
			bool flag = true;
			foreach (T current in source)
			{
				initValue = selector(current, initValue);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return initValue;
		}

		public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			int? result = null;
			foreach (TSource current in source)
			{
				int? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value > result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			long? result = null;
			foreach (TSource current in source)
			{
				long? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value > result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			double? result = null;
			foreach (TSource current in source)
			{
				double? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value > result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			float? result = null;
			foreach (TSource current in source)
			{
				float? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value > result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			decimal? result = null;
			foreach (TSource current in source)
			{
				decimal? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value > result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Max<TResult>();
		}

		public static int Min(this IEnumerable<int> source)
		{
			Check.Source(source);
			bool flag = true;
			int num = 2147483647;
			foreach (int current in source)
			{
				num = Math.Min(current, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static long Min(this IEnumerable<long> source)
		{
			Check.Source(source);
			bool flag = true;
			long num = 9223372036854775807L;
			foreach (long current in source)
			{
				num = Math.Min(current, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static double Min(this IEnumerable<double> source)
		{
			Check.Source(source);
			bool flag = true;
			double num = 1.7976931348623157E+308;
			foreach (double val in source)
			{
				num = Math.Min(val, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static float Min(this IEnumerable<float> source)
		{
			Check.Source(source);
			bool flag = true;
			float num = 3.40282347E+38f;
			foreach (float val in source)
			{
				num = Math.Min(val, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static decimal Min(this IEnumerable<decimal> source)
		{
			Check.Source(source);
			bool flag = true;
			decimal num = 79228162514264337593543950335m;
			foreach (decimal current in source)
			{
				num = Math.Min(current, num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.EmptySequence();
			}
			return num;
		}

		public static int? Min(this IEnumerable<int?> source)
		{
			Check.Source(source);
			bool flag = true;
			int num = 2147483647;
			foreach (int? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Min(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new int?(num);
		}

		public static long? Min(this IEnumerable<long?> source)
		{
			Check.Source(source);
			bool flag = true;
			long num = 9223372036854775807L;
			foreach (long? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Min(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new long?(num);
		}

		public static double? Min(this IEnumerable<double?> source)
		{
			Check.Source(source);
			bool flag = true;
			double num = 1.7976931348623157E+308;
			foreach (double? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Min(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new double?(num);
		}

		public static float? Min(this IEnumerable<float?> source)
		{
			Check.Source(source);
			bool flag = true;
			float num = 3.40282347E+38f;
			foreach (float? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Min(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new float?(num);
		}

		public static decimal? Min(this IEnumerable<decimal?> source)
		{
			Check.Source(source);
			bool flag = true;
			decimal num = 79228162514264337593543950335m;
			foreach (decimal? current in source)
			{
				if (current.HasValue)
				{
					num = Math.Min(current.Value, num);
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return new decimal?(num);
		}

		public static TSource Min<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			Comparer<TSource> @default = Comparer<TSource>.Default;
			TSource tSource = default(TSource);
			if (default(TSource) == null)
			{
				foreach (TSource current in source)
				{
					if (current != null)
					{
						if (tSource == null || @default.Compare(current, tSource) < 0)
						{
							tSource = current;
						}
					}
				}
			}
			else
			{
				bool flag = true;
				foreach (TSource current2 in source)
				{
					if (flag)
					{
						tSource = current2;
						flag = false;
					}
					else if (@default.Compare(current2, tSource) < 0)
					{
						tSource = current2;
					}
				}
				if (flag)
				{
					throw Enumerable.EmptySequence();
				}
			}
			return tSource;
		}

		public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			int num = 2147483647;
			foreach (TSource current in source)
			{
				num = Math.Min(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			long num = 9223372036854775807L;
			foreach (TSource current in source)
			{
				num = Math.Min(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			double num = 1.7976931348623157E+308;
			foreach (TSource current in source)
			{
				num = Math.Min(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			float num = 3.40282347E+38f;
			foreach (TSource current in source)
			{
				num = Math.Min(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			decimal num = 79228162514264337593543950335m;
			foreach (TSource current in source)
			{
				num = Math.Min(selector(current), num);
				flag = false;
			}
			if (flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return num;
		}

		public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			int? result = null;
			foreach (TSource current in source)
			{
				int? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value < result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			long? result = null;
			foreach (TSource current in source)
			{
				long? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value < result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			float? result = null;
			foreach (TSource current in source)
			{
				float? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value < result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			double? result = null;
			foreach (TSource current in source)
			{
				double? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value < result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector(source, selector);
			bool flag = true;
			decimal? result = null;
			foreach (TSource current in source)
			{
				decimal? num = selector(current);
				if (!result.HasValue)
				{
					result = num;
				}
				else if (num.HasValue && result.HasValue && num.Value < result.Value)
				{
					result = num;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Min<TResult>();
		}

		public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
		{
			Check.Source(source);
			return Enumerable.CreateOfTypeIterator<TResult>(source);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateOfTypeIterator<TResult>(IEnumerable source)
		{
			Enumerable.<CreateOfTypeIterator>c__Iterator1F2<TResult> <CreateOfTypeIterator>c__Iterator1F = new Enumerable.<CreateOfTypeIterator>c__Iterator1F2<TResult>();
			<CreateOfTypeIterator>c__Iterator1F.source = source;
			<CreateOfTypeIterator>c__Iterator1F.<$>source = source;
			Enumerable.<CreateOfTypeIterator>c__Iterator1F2<TResult> expr_15 = <CreateOfTypeIterator>c__Iterator1F;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderBy(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			return new OrderedSequence<TSource, TKey>(source, keySelector, comparer, SortDirection.Ascending);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderByDescending(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			return new OrderedSequence<TSource, TKey>(source, keySelector, comparer, SortDirection.Descending);
		}

		public static IEnumerable<int> Range(int start, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((long)start + (long)count - 1L > 2147483647L)
			{
				throw new ArgumentOutOfRangeException();
			}
			return Enumerable.CreateRangeIterator(start, count);
		}

		[DebuggerHidden]
		private static IEnumerable<int> CreateRangeIterator(int start, int count)
		{
			Enumerable.<CreateRangeIterator>c__Iterator1F3 <CreateRangeIterator>c__Iterator1F = new Enumerable.<CreateRangeIterator>c__Iterator1F3();
			<CreateRangeIterator>c__Iterator1F.count = count;
			<CreateRangeIterator>c__Iterator1F.start = start;
			<CreateRangeIterator>c__Iterator1F.<$>count = count;
			<CreateRangeIterator>c__Iterator1F.<$>start = start;
			Enumerable.<CreateRangeIterator>c__Iterator1F3 expr_23 = <CreateRangeIterator>c__Iterator1F;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return Enumerable.CreateRepeatIterator<TResult>(element, count);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateRepeatIterator<TResult>(TResult element, int count)
		{
			Enumerable.<CreateRepeatIterator>c__Iterator1F4<TResult> <CreateRepeatIterator>c__Iterator1F = new Enumerable.<CreateRepeatIterator>c__Iterator1F4<TResult>();
			<CreateRepeatIterator>c__Iterator1F.count = count;
			<CreateRepeatIterator>c__Iterator1F.element = element;
			<CreateRepeatIterator>c__Iterator1F.<$>count = count;
			<CreateRepeatIterator>c__Iterator1F.<$>element = element;
			Enumerable.<CreateRepeatIterator>c__Iterator1F4<TResult> expr_23 = <CreateRepeatIterator>c__Iterator1F;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			return Enumerable.CreateReverseIterator<TSource>(source);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateReverseIterator<TSource>(IEnumerable<TSource> source)
		{
			Enumerable.<CreateReverseIterator>c__Iterator1F5<TSource> <CreateReverseIterator>c__Iterator1F = new Enumerable.<CreateReverseIterator>c__Iterator1F5<TSource>();
			<CreateReverseIterator>c__Iterator1F.source = source;
			<CreateReverseIterator>c__Iterator1F.<$>source = source;
			Enumerable.<CreateReverseIterator>c__Iterator1F5<TSource> expr_15 = <CreateReverseIterator>c__Iterator1F;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Enumerable.CreateSelectIterator<TSource, TResult>(source, selector);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Enumerable.<CreateSelectIterator>c__Iterator1F6<TSource, TResult> <CreateSelectIterator>c__Iterator1F = new Enumerable.<CreateSelectIterator>c__Iterator1F6<TSource, TResult>();
			<CreateSelectIterator>c__Iterator1F.source = source;
			<CreateSelectIterator>c__Iterator1F.selector = selector;
			<CreateSelectIterator>c__Iterator1F.<$>source = source;
			<CreateSelectIterator>c__Iterator1F.<$>selector = selector;
			Enumerable.<CreateSelectIterator>c__Iterator1F6<TSource, TResult> expr_23 = <CreateSelectIterator>c__Iterator1F;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Enumerable.CreateSelectIterator<TSource, TResult>(source, selector);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			Enumerable.<CreateSelectIterator>c__Iterator1F7<TSource, TResult> <CreateSelectIterator>c__Iterator1F = new Enumerable.<CreateSelectIterator>c__Iterator1F7<TSource, TResult>();
			<CreateSelectIterator>c__Iterator1F.source = source;
			<CreateSelectIterator>c__Iterator1F.selector = selector;
			<CreateSelectIterator>c__Iterator1F.<$>source = source;
			<CreateSelectIterator>c__Iterator1F.<$>selector = selector;
			Enumerable.<CreateSelectIterator>c__Iterator1F7<TSource, TResult> expr_23 = <CreateSelectIterator>c__Iterator1F;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Enumerable.CreateSelectManyIterator<TSource, TResult>(source, selector);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateSelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			Enumerable.<CreateSelectManyIterator>c__Iterator1F8<TSource, TResult> <CreateSelectManyIterator>c__Iterator1F = new Enumerable.<CreateSelectManyIterator>c__Iterator1F8<TSource, TResult>();
			<CreateSelectManyIterator>c__Iterator1F.source = source;
			<CreateSelectManyIterator>c__Iterator1F.selector = selector;
			<CreateSelectManyIterator>c__Iterator1F.<$>source = source;
			<CreateSelectManyIterator>c__Iterator1F.<$>selector = selector;
			Enumerable.<CreateSelectManyIterator>c__Iterator1F8<TSource, TResult> expr_23 = <CreateSelectManyIterator>c__Iterator1F;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Enumerable.CreateSelectManyIterator<TSource, TResult>(source, selector);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateSelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			Enumerable.<CreateSelectManyIterator>c__Iterator1F9<TSource, TResult> <CreateSelectManyIterator>c__Iterator1F = new Enumerable.<CreateSelectManyIterator>c__Iterator1F9<TSource, TResult>();
			<CreateSelectManyIterator>c__Iterator1F.source = source;
			<CreateSelectManyIterator>c__Iterator1F.selector = selector;
			<CreateSelectManyIterator>c__Iterator1F.<$>source = source;
			<CreateSelectManyIterator>c__Iterator1F.<$>selector = selector;
			Enumerable.<CreateSelectManyIterator>c__Iterator1F9<TSource, TResult> expr_23 = <CreateSelectManyIterator>c__Iterator1F;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			Check.SourceAndCollectionSelectors(source, collectionSelector, resultSelector);
			return Enumerable.CreateSelectManyIterator<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateSelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> selector)
		{
			Enumerable.<CreateSelectManyIterator>c__Iterator1FA<TSource, TCollection, TResult> <CreateSelectManyIterator>c__Iterator1FA = new Enumerable.<CreateSelectManyIterator>c__Iterator1FA<TSource, TCollection, TResult>();
			<CreateSelectManyIterator>c__Iterator1FA.source = source;
			<CreateSelectManyIterator>c__Iterator1FA.collectionSelector = collectionSelector;
			<CreateSelectManyIterator>c__Iterator1FA.selector = selector;
			<CreateSelectManyIterator>c__Iterator1FA.<$>source = source;
			<CreateSelectManyIterator>c__Iterator1FA.<$>collectionSelector = collectionSelector;
			<CreateSelectManyIterator>c__Iterator1FA.<$>selector = selector;
			Enumerable.<CreateSelectManyIterator>c__Iterator1FA<TSource, TCollection, TResult> expr_31 = <CreateSelectManyIterator>c__Iterator1FA;
			expr_31.$PC = -2;
			return expr_31;
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			Check.SourceAndCollectionSelectors(source, collectionSelector, resultSelector);
			return Enumerable.CreateSelectManyIterator<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		[DebuggerHidden]
		private static IEnumerable<TResult> CreateSelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> selector)
		{
			Enumerable.<CreateSelectManyIterator>c__Iterator1FB<TSource, TCollection, TResult> <CreateSelectManyIterator>c__Iterator1FB = new Enumerable.<CreateSelectManyIterator>c__Iterator1FB<TSource, TCollection, TResult>();
			<CreateSelectManyIterator>c__Iterator1FB.source = source;
			<CreateSelectManyIterator>c__Iterator1FB.collectionSelector = collectionSelector;
			<CreateSelectManyIterator>c__Iterator1FB.selector = selector;
			<CreateSelectManyIterator>c__Iterator1FB.<$>source = source;
			<CreateSelectManyIterator>c__Iterator1FB.<$>collectionSelector = collectionSelector;
			<CreateSelectManyIterator>c__Iterator1FB.<$>selector = selector;
			Enumerable.<CreateSelectManyIterator>c__Iterator1FB<TSource, TCollection, TResult> expr_31 = <CreateSelectManyIterator>c__Iterator1FB;
			expr_31.$PC = -2;
			return expr_31;
		}

		private static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Enumerable.Fallback fallback)
		{
			bool flag = false;
			TSource result = default(TSource);
			foreach (TSource current in source)
			{
				if (predicate(current))
				{
					if (flag)
					{
						throw Enumerable.MoreThanOneMatchingElement();
					}
					flag = true;
					result = current;
				}
			}
			if (!flag && fallback == Enumerable.Fallback.Throw)
			{
				throw Enumerable.NoMatchingElement();
			}
			return result;
		}

		public static TSource Single<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			bool flag = false;
			TSource result = default(TSource);
			foreach (TSource current in source)
			{
				if (flag)
				{
					throw Enumerable.MoreThanOneElement();
				}
				flag = true;
				result = current;
			}
			if (!flag)
			{
				throw Enumerable.NoMatchingElement();
			}
			return result;
		}

		public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.Single(predicate, Enumerable.Fallback.Throw);
		}

		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			bool flag = false;
			TSource result = default(TSource);
			foreach (TSource current in source)
			{
				if (flag)
				{
					throw Enumerable.MoreThanOneMatchingElement();
				}
				flag = true;
				result = current;
			}
			return result;
		}

		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.Single(predicate, Enumerable.Fallback.Default);
		}

		public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
		{
			Check.Source(source);
			return Enumerable.CreateSkipIterator<TSource>(source, count);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateSkipIterator<TSource>(IEnumerable<TSource> source, int count)
		{
			Enumerable.<CreateSkipIterator>c__Iterator1FC<TSource> <CreateSkipIterator>c__Iterator1FC = new Enumerable.<CreateSkipIterator>c__Iterator1FC<TSource>();
			<CreateSkipIterator>c__Iterator1FC.source = source;
			<CreateSkipIterator>c__Iterator1FC.count = count;
			<CreateSkipIterator>c__Iterator1FC.<$>source = source;
			<CreateSkipIterator>c__Iterator1FC.<$>count = count;
			Enumerable.<CreateSkipIterator>c__Iterator1FC<TSource> expr_23 = <CreateSkipIterator>c__Iterator1FC;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return Enumerable.CreateSkipWhileIterator<TSource>(source, predicate);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateSkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Enumerable.<CreateSkipWhileIterator>c__Iterator1FD<TSource> <CreateSkipWhileIterator>c__Iterator1FD = new Enumerable.<CreateSkipWhileIterator>c__Iterator1FD<TSource>();
			<CreateSkipWhileIterator>c__Iterator1FD.source = source;
			<CreateSkipWhileIterator>c__Iterator1FD.predicate = predicate;
			<CreateSkipWhileIterator>c__Iterator1FD.<$>source = source;
			<CreateSkipWhileIterator>c__Iterator1FD.<$>predicate = predicate;
			Enumerable.<CreateSkipWhileIterator>c__Iterator1FD<TSource> expr_23 = <CreateSkipWhileIterator>c__Iterator1FD;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return Enumerable.CreateSkipWhileIterator<TSource>(source, predicate);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateSkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Enumerable.<CreateSkipWhileIterator>c__Iterator1FE<TSource> <CreateSkipWhileIterator>c__Iterator1FE = new Enumerable.<CreateSkipWhileIterator>c__Iterator1FE<TSource>();
			<CreateSkipWhileIterator>c__Iterator1FE.source = source;
			<CreateSkipWhileIterator>c__Iterator1FE.predicate = predicate;
			<CreateSkipWhileIterator>c__Iterator1FE.<$>source = source;
			<CreateSkipWhileIterator>c__Iterator1FE.<$>predicate = predicate;
			Enumerable.<CreateSkipWhileIterator>c__Iterator1FE<TSource> expr_23 = <CreateSkipWhileIterator>c__Iterator1FE;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static int Sum(this IEnumerable<int> source)
		{
			Check.Source(source);
			int num = 0;
			checked
			{
				foreach (int current in source)
				{
					num += current;
				}
				return num;
			}
		}

		public static int? Sum(this IEnumerable<int?> source)
		{
			Check.Source(source);
			int num = 0;
			checked
			{
				foreach (int? current in source)
				{
					if (current.HasValue)
					{
						num += current.Value;
					}
				}
				return new int?(num);
			}
		}

		public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector(source, selector);
			int num = 0;
			checked
			{
				foreach (TSource current in source)
				{
					num += selector(current);
				}
				return num;
			}
		}

		public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector(source, selector);
			int num = 0;
			checked
			{
				foreach (TSource current in source)
				{
					int? num2 = selector(current);
					if (num2.HasValue)
					{
						num += num2.Value;
					}
				}
				return new int?(num);
			}
		}

		public static long Sum(this IEnumerable<long> source)
		{
			Check.Source(source);
			long num = 0L;
			checked
			{
				foreach (long current in source)
				{
					num += current;
				}
				return num;
			}
		}

		public static long? Sum(this IEnumerable<long?> source)
		{
			Check.Source(source);
			long num = 0L;
			checked
			{
				foreach (long? current in source)
				{
					if (current.HasValue)
					{
						num += current.Value;
					}
				}
				return new long?(num);
			}
		}

		public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector(source, selector);
			long num = 0L;
			checked
			{
				foreach (TSource current in source)
				{
					num += selector(current);
				}
				return num;
			}
		}

		public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector(source, selector);
			long num = 0L;
			checked
			{
				foreach (TSource current in source)
				{
					long? num2 = selector(current);
					if (num2.HasValue)
					{
						num += num2.Value;
					}
				}
				return new long?(num);
			}
		}

		public static double Sum(this IEnumerable<double> source)
		{
			Check.Source(source);
			double num = 0.0;
			foreach (double num2 in source)
			{
				num += num2;
			}
			return num;
		}

		public static double? Sum(this IEnumerable<double?> source)
		{
			Check.Source(source);
			double num = 0.0;
			foreach (double? current in source)
			{
				if (current.HasValue)
				{
					num += current.Value;
				}
			}
			return new double?(num);
		}

		public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector(source, selector);
			double num = 0.0;
			foreach (TSource current in source)
			{
				num += selector(current);
			}
			return num;
		}

		public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector(source, selector);
			double num = 0.0;
			foreach (TSource current in source)
			{
				double? num2 = selector(current);
				if (num2.HasValue)
				{
					num += num2.Value;
				}
			}
			return new double?(num);
		}

		public static float Sum(this IEnumerable<float> source)
		{
			Check.Source(source);
			float num = 0f;
			foreach (float num2 in source)
			{
				num += num2;
			}
			return num;
		}

		public static float? Sum(this IEnumerable<float?> source)
		{
			Check.Source(source);
			float num = 0f;
			foreach (float? current in source)
			{
				if (current.HasValue)
				{
					num += current.Value;
				}
			}
			return new float?(num);
		}

		public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector(source, selector);
			float num = 0f;
			foreach (TSource current in source)
			{
				num += selector(current);
			}
			return num;
		}

		public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector(source, selector);
			float num = 0f;
			foreach (TSource current in source)
			{
				float? num2 = selector(current);
				if (num2.HasValue)
				{
					num += num2.Value;
				}
			}
			return new float?(num);
		}

		public static decimal Sum(this IEnumerable<decimal> source)
		{
			Check.Source(source);
			decimal num = 0m;
			foreach (decimal current in source)
			{
				num += current;
			}
			return num;
		}

		public static decimal? Sum(this IEnumerable<decimal?> source)
		{
			Check.Source(source);
			decimal num = 0m;
			foreach (decimal? current in source)
			{
				if (current.HasValue)
				{
					num += current.Value;
				}
			}
			return new decimal?(num);
		}

		public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector(source, selector);
			decimal num = 0m;
			foreach (TSource current in source)
			{
				num += selector(current);
			}
			return num;
		}

		public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector(source, selector);
			decimal num = 0m;
			foreach (TSource current in source)
			{
				decimal? num2 = selector(current);
				if (num2.HasValue)
				{
					num += num2.Value;
				}
			}
			return new decimal?(num);
		}

		public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
		{
			Check.Source(source);
			return Enumerable.CreateTakeIterator<TSource>(source, count);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateTakeIterator<TSource>(IEnumerable<TSource> source, int count)
		{
			Enumerable.<CreateTakeIterator>c__Iterator1FF<TSource> <CreateTakeIterator>c__Iterator1FF = new Enumerable.<CreateTakeIterator>c__Iterator1FF<TSource>();
			<CreateTakeIterator>c__Iterator1FF.count = count;
			<CreateTakeIterator>c__Iterator1FF.source = source;
			<CreateTakeIterator>c__Iterator1FF.<$>count = count;
			<CreateTakeIterator>c__Iterator1FF.<$>source = source;
			Enumerable.<CreateTakeIterator>c__Iterator1FF<TSource> expr_23 = <CreateTakeIterator>c__Iterator1FF;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return Enumerable.CreateTakeWhileIterator<TSource>(source, predicate);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateTakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Enumerable.<CreateTakeWhileIterator>c__Iterator200<TSource> <CreateTakeWhileIterator>c__Iterator = new Enumerable.<CreateTakeWhileIterator>c__Iterator200<TSource>();
			<CreateTakeWhileIterator>c__Iterator.source = source;
			<CreateTakeWhileIterator>c__Iterator.predicate = predicate;
			<CreateTakeWhileIterator>c__Iterator.<$>source = source;
			<CreateTakeWhileIterator>c__Iterator.<$>predicate = predicate;
			Enumerable.<CreateTakeWhileIterator>c__Iterator200<TSource> expr_23 = <CreateTakeWhileIterator>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return Enumerable.CreateTakeWhileIterator<TSource>(source, predicate);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateTakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Enumerable.<CreateTakeWhileIterator>c__Iterator201<TSource> <CreateTakeWhileIterator>c__Iterator = new Enumerable.<CreateTakeWhileIterator>c__Iterator201<TSource>();
			<CreateTakeWhileIterator>c__Iterator.source = source;
			<CreateTakeWhileIterator>c__Iterator.predicate = predicate;
			<CreateTakeWhileIterator>c__Iterator.<$>source = source;
			<CreateTakeWhileIterator>c__Iterator.<$>predicate = predicate;
			Enumerable.<CreateTakeWhileIterator>c__Iterator201<TSource> expr_23 = <CreateTakeWhileIterator>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ThenBy(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			OrderedEnumerable<TSource> orderedEnumerable = source as OrderedEnumerable<TSource>;
			return orderedEnumerable.CreateOrderedEnumerable<TKey>(keySelector, comparer, false);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ThenByDescending(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			OrderedEnumerable<TSource> orderedEnumerable = source as OrderedEnumerable<TSource>;
			return orderedEnumerable.CreateOrderedEnumerable<TKey>(keySelector, comparer, true);
		}

		public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			ICollection<TSource> collection = source as ICollection<TSource>;
			TSource[] array;
			if (collection == null)
			{
				int num = 0;
				array = new TSource[0];
				foreach (TSource current in source)
				{
					if (num == array.Length)
					{
						if (num == 0)
						{
							array = new TSource[4];
						}
						else
						{
							Array.Resize<TSource>(ref array, num * 2);
						}
					}
					array[num++] = current;
				}
				if (num != array.Length)
				{
					Array.Resize<TSource>(ref array, num);
				}
				return array;
			}
			if (collection.Count == 0)
			{
				return new TSource[0];
			}
			array = new TSource[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToDictionary(keySelector, elementSelector, null);
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors(source, keySelector, elementSelector);
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);
			foreach (TSource current in source)
			{
				dictionary.Add(keySelector(current), elementSelector(current));
			}
			return dictionary;
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToDictionary(keySelector, null);
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(comparer);
			foreach (TSource current in source)
			{
				dictionary.Add(keySelector(current), current);
			}
			return dictionary;
		}

		public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			return new List<TSource>(source);
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToLookup(keySelector, null);
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			List<TSource> list = null;
			Dictionary<TKey, List<TSource>> dictionary = new Dictionary<TKey, List<TSource>>(comparer ?? EqualityComparer<TKey>.Default);
			foreach (TSource current in source)
			{
				TKey tKey = keySelector(current);
				List<TSource> list2;
				if (tKey == null)
				{
					if (list == null)
					{
						list = new List<TSource>();
					}
					list2 = list;
				}
				else if (!dictionary.TryGetValue(tKey, out list2))
				{
					list2 = new List<TSource>();
					dictionary.Add(tKey, list2);
				}
				list2.Add(current);
			}
			return new Lookup<TKey, TSource>(dictionary, list);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToLookup(keySelector, elementSelector, null);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors(source, keySelector, elementSelector);
			List<TElement> list = null;
			Dictionary<TKey, List<TElement>> dictionary = new Dictionary<TKey, List<TElement>>(comparer ?? EqualityComparer<TKey>.Default);
			foreach (TSource current in source)
			{
				TKey tKey = keySelector(current);
				List<TElement> list2;
				if (tKey == null)
				{
					if (list == null)
					{
						list = new List<TElement>();
					}
					list2 = list;
				}
				else if (!dictionary.TryGetValue(tKey, out list2))
				{
					list2 = new List<TElement>();
					dictionary.Add(tKey, list2);
				}
				list2.Add(elementSelector(current));
			}
			return new Lookup<TKey, TElement>(dictionary, list);
		}

		public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.SequenceEqual(second, null);
		}

		public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond(first, second);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			bool result;
			using (IEnumerator<TSource> enumerator = first.GetEnumerator())
			{
				using (IEnumerator<TSource> enumerator2 = second.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator2.MoveNext())
						{
							result = false;
							return result;
						}
						if (!comparer.Equals(enumerator.Current, enumerator2.Current))
						{
							result = false;
							return result;
						}
					}
					result = !enumerator2.MoveNext();
				}
			}
			return result;
		}

		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			Check.FirstAndSecond(first, second);
			return first.Union(second, null);
		}

		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond(first, second);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			return Enumerable.CreateUnionIterator<TSource>(first, second, comparer);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateUnionIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Enumerable.<CreateUnionIterator>c__Iterator202<TSource> <CreateUnionIterator>c__Iterator = new Enumerable.<CreateUnionIterator>c__Iterator202<TSource>();
			<CreateUnionIterator>c__Iterator.comparer = comparer;
			<CreateUnionIterator>c__Iterator.first = first;
			<CreateUnionIterator>c__Iterator.second = second;
			<CreateUnionIterator>c__Iterator.<$>comparer = comparer;
			<CreateUnionIterator>c__Iterator.<$>first = first;
			<CreateUnionIterator>c__Iterator.<$>second = second;
			Enumerable.<CreateUnionIterator>c__Iterator202<TSource> expr_31 = <CreateUnionIterator>c__Iterator;
			expr_31.$PC = -2;
			return expr_31;
		}

		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			TSource[] array = source as TSource[];
			if (array != null)
			{
				return Enumerable.CreateWhereIterator<TSource>(array, predicate);
			}
			return Enumerable.CreateWhereIterator<TSource>(source, predicate);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateWhereIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Enumerable.<CreateWhereIterator>c__Iterator203<TSource> <CreateWhereIterator>c__Iterator = new Enumerable.<CreateWhereIterator>c__Iterator203<TSource>();
			<CreateWhereIterator>c__Iterator.source = source;
			<CreateWhereIterator>c__Iterator.predicate = predicate;
			<CreateWhereIterator>c__Iterator.<$>source = source;
			<CreateWhereIterator>c__Iterator.<$>predicate = predicate;
			Enumerable.<CreateWhereIterator>c__Iterator203<TSource> expr_23 = <CreateWhereIterator>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateWhereIterator<TSource>(TSource[] source, Func<TSource, bool> predicate)
		{
			Enumerable.<CreateWhereIterator>c__Iterator204<TSource> <CreateWhereIterator>c__Iterator = new Enumerable.<CreateWhereIterator>c__Iterator204<TSource>();
			<CreateWhereIterator>c__Iterator.source = source;
			<CreateWhereIterator>c__Iterator.predicate = predicate;
			<CreateWhereIterator>c__Iterator.<$>source = source;
			<CreateWhereIterator>c__Iterator.<$>predicate = predicate;
			Enumerable.<CreateWhereIterator>c__Iterator204<TSource> expr_23 = <CreateWhereIterator>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			TSource[] array = source as TSource[];
			if (array != null)
			{
				return Enumerable.CreateWhereIterator<TSource>(array, predicate);
			}
			return Enumerable.CreateWhereIterator<TSource>(source, predicate);
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateWhereIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Enumerable.<CreateWhereIterator>c__Iterator205<TSource> <CreateWhereIterator>c__Iterator = new Enumerable.<CreateWhereIterator>c__Iterator205<TSource>();
			<CreateWhereIterator>c__Iterator.source = source;
			<CreateWhereIterator>c__Iterator.predicate = predicate;
			<CreateWhereIterator>c__Iterator.<$>source = source;
			<CreateWhereIterator>c__Iterator.<$>predicate = predicate;
			Enumerable.<CreateWhereIterator>c__Iterator205<TSource> expr_23 = <CreateWhereIterator>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		[DebuggerHidden]
		private static IEnumerable<TSource> CreateWhereIterator<TSource>(TSource[] source, Func<TSource, int, bool> predicate)
		{
			Enumerable.<CreateWhereIterator>c__Iterator206<TSource> <CreateWhereIterator>c__Iterator = new Enumerable.<CreateWhereIterator>c__Iterator206<TSource>();
			<CreateWhereIterator>c__Iterator.source = source;
			<CreateWhereIterator>c__Iterator.predicate = predicate;
			<CreateWhereIterator>c__Iterator.<$>source = source;
			<CreateWhereIterator>c__Iterator.<$>predicate = predicate;
			Enumerable.<CreateWhereIterator>c__Iterator206<TSource> expr_23 = <CreateWhereIterator>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T current in source)
			{
				action(current);
			}
		}

		private static Exception EmptySequence()
		{
			return new InvalidOperationException("Sequence contains no elements");
		}

		private static Exception NoMatchingElement()
		{
			return new InvalidOperationException("Sequence contains no matching element");
		}

		private static Exception MoreThanOneElement()
		{
			return new InvalidOperationException("Sequence contains more than one element");
		}

		private static Exception MoreThanOneMatchingElement()
		{
			return new InvalidOperationException("Sequence contains more than one matching element");
		}
	}
}
