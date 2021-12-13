using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public static class TreeWalker
    {
        public static IEnumerable<T> Walk<T>(T root, Func<T, IEnumerable<T>> next)
        {
            var q = next(root).SelectMany(n => Walk(n, next));
            return Enumerable.Repeat(root, 1).Concat(q);
        }
    }
}
