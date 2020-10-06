using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        public class DistinctForwardOnlyLeftJoin
        {
            [TestCase(new int[] { })]
            [TestCase(new int[] { 1 })]
            public void EmptyLeft(int[] right)
            {
                var left = new int[] { };

                var result = left
                    .DistinctForwardOnlyLeftJoin(right, l => l, r => r, (l, r) => r)
                    .ToArray();

                Assert.That(result, Is.EquivalentTo(new int[] { }));
            }

            [TestCase(new int[] { }, new[] { 0, 0, 0, 0, 0 })]
            [TestCase(new int[] { 1 }, new[] { 1, 0, 0, 0, 0 })]
            [TestCase(new int[] { 3 }, new[] { 0, 0, 3, 0, 0 })]
            [TestCase(new int[] { 5 }, new[] { 0, 0, 0, 0, 5 })]
            [TestCase(new[] { 1, 3, 5 }, new[] { 1, 0, 3, 0, 5 })]
            [TestCase(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 })]
            public void NonEmptyLeftJoinToVariableLengthRight(int[] right, int[] expected)
            {
                var left = new[] { 1, 2, 3, 4, 5 };

                var result = left
                    .DistinctForwardOnlyLeftJoin(right, l => l, r => r, (l, r) => Tuple.Create(l, r))
                    .ToArray();

                var leftResult = result.Select(o => o.Item1).ToArray();
                var rightResult = result.Select(o => o.Item2).ToArray();

                Assert.That(leftResult, Is.EquivalentTo(left));
                Assert.That(rightResult, Is.EquivalentTo(expected));
            }            
        }

        public class FillDateRange
        {
            [TestCase(new int[] { }, new[] { 0, 0, 0, 0, 0 })]
            [TestCase(new int[] { 22 }, new[] { 1, 0, 0, 0, 0 })]
            [TestCase(new int[] { 24 }, new[] { 0, 0, 1, 0, 0 })]
            [TestCase(new int[] { 26 }, new[] { 0, 0, 0, 0, 1 })]
            [TestCase(new int[] { 22, 25 }, new[] { 1, 0, 0, 2, 0 })]
            [TestCase(new int[] { 22, 23, 24, 25, 26 }, new[] { 1, 2, 3, 4, 5 })]
            public void VariousSources(int[] sourceDays, int[] expectedSourceIndexes)
            {
                var source = sourceDays.Select((d, i) => new Dated { Index = i + 1, Date = new DateTime(2019, 5, d) }).ToArray();

                var result = source
                    .FillDateRange(new DateTime(2019, 5, 22), new DateTime(2019, 5, 26), d => d?.Date ?? DateTime.MinValue, d => new Dated { Date = d })
                    .ToArray();

                var resultDates = result.Select(o => o.Date).ToArray();

                var resultIndexes = result.Select(o => o.Index).ToArray();

                Assert.That(resultDates, Is.EquivalentTo(new DateTime[] {
                    new DateTime(2019, 5, 22),
                    new DateTime(2019, 5, 23),
                    new DateTime(2019, 5, 24),
                    new DateTime(2019, 5, 25),
                    new DateTime(2019, 5, 26)
                }));

                Assert.That(resultIndexes, Is.EquivalentTo(expectedSourceIndexes));
            }

            class Dated
            {
                public DateTime Date { get; set; }

                public int Index { get; set; }
            }
        }
    }
}
