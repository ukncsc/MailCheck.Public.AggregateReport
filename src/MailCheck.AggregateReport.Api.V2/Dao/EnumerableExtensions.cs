using System;
using System.Collections.Generic;
using System.Linq;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Performs a left join on two forward-only enumerables each having distinct
        /// occurances of the comparand being matched on.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TComparand"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="leftSelector"></param>
        /// <param name="rightSelector"></param>
        /// <param name="resultSelector"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> DistinctForwardOnlyLeftJoin<TLeft, TRight, TComparand, TResult>(
            this IEnumerable<TLeft> left,
            IEnumerable<TRight> right,
            Func<TLeft, TComparand> leftSelector,
            Func<TRight, TComparand> rightSelector,
            Func<TLeft, TRight, TResult> resultSelector,
            EqualityComparer<TComparand> equalityComparer = null
        )
        {
            TLeft leftCurrent;
            TRight rightCurrent;
            TComparand leftComparand, rightComparand;

            equalityComparer = equalityComparer ?? EqualityComparer<TComparand>.Default;
            
            var leftEnum = left.GetEnumerator();
            var rightEnum = right.GetEnumerator();

            bool rightDone = false;
            Func<TRight> getNextRight = () =>
            {
                if (!rightDone && rightEnum.MoveNext())
                {
                    return rightEnum.Current;
                }
                else
                {
                    rightDone = true;
                    return default(TRight);
                }
            };
            
            rightCurrent = getNextRight();
            rightComparand = rightSelector(rightCurrent);

            while (leftEnum.MoveNext())
            {
                leftCurrent = leftEnum.Current;
                leftComparand = leftSelector(leftCurrent);
                if (equalityComparer.Equals(leftComparand, rightComparand))
                {
                    yield return resultSelector(leftCurrent, rightCurrent);
                    rightCurrent = getNextRight();
                    rightComparand = rightSelector(rightCurrent);
                }
                else
                {
                    yield return resultSelector(leftCurrent, default(TRight));
                }
            }
        }

        public static IEnumerable<TRecord> FillRange<TRecord, TRange>(
            this IEnumerable<TRecord> records,
            IEnumerable<TRange> range,
            Func<TRecord, TRange> selector,
            Func<TRange, TRecord> emptyRecordFactory
        ) where TRecord : class
        {
            return range.DistinctForwardOnlyLeftJoin(records, d => d, selector, (rangeItem, record) => record ?? emptyRecordFactory(rangeItem));
        }

        public static IEnumerable<T> FillDateRange<T>(
            this IEnumerable<T> records,
            DateTime startDate,
            DateTime endDate,
            Func<T, DateTime> selector,
            Func<DateTime, T> emptyRecordFactory
        ) where T : class
        {
            IEnumerable<DateTime> dates = CreateDateRange(startDate, endDate);
            return records.FillRange(dates, selector, emptyRecordFactory);
        }

        private static IEnumerable<DateTime> CreateDateRange(DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                .Select(offset => startDate.AddDays(offset));
        }
    }
}
