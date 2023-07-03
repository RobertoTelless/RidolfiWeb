using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public static class ListLibrary
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source is null || !source.Any();

        public static (IEnumerable<T> True, IEnumerable<T> False) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("Lista original não informada");
            if (predicate == null) throw new ArgumentNullException("Regra de partição não informada");           

            var trueItems = new List<T>();
            var falseItems = new List<T>();

            foreach (var item in source)
            {
                if (predicate(item))
                    trueItems.Add(item);
                else
                    falseItems.Add(item);
            }
            return (trueItems, falseItems);
        }

        public static double Median<T>(this IEnumerable<T> source) where T : IConvertible
        {
            if (source == null) throw new ArgumentNullException("Lista original não informada");

            var sortedList = source.Select(x => x.ToDouble(CultureInfo.InvariantCulture)).OrderBy(x => x).ToList();
            var count = sortedList.Count;

            if (count == 0)
            {
                throw new InvalidOperationException("The source sequence is empty.");
            }

            if (count % 2 == 0)
            {
                return (sortedList[count / 2 - 1] + sortedList[count / 2]) / 2;
            }
            return sortedList[count / 2];
        }

        public static IEnumerable<T> Mode<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("Lista original não informada");

            var groups = source.GroupBy(x => x);
            var maxCount = groups.Max(g => g.Count());
            return groups.Where(g => g.Count() == maxCount).Select(g => g.Key);
        }

        public static double StandardDeviation<T>(this IEnumerable<T> source) where T : IConvertible
        {
            if (source == null) throw new ArgumentNullException("Lista original não informada");

            var values = source.Select(x => x.ToDouble(CultureInfo.InvariantCulture)).ToList();
            var count = values.Count;

            if (count == 0)
            {
                throw new InvalidOperationException("The source sequence is empty.");
            }

            var avg = values.Average();
            var sum = values.Sum(d => Math.Pow(d - avg, 2));
            return Math.Sqrt(sum / count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("Lista original não informada");

            var elements = source.ToArray();
            Random random = new Random();
            for (var i = elements.Length - 1; i > 0; i--)
            {
                var swapIndex = random.Next(i + 1);
                (elements[i], elements[swapIndex]) = (elements[swapIndex], elements[i]);
            }

            return elements;
        }



    }
}
