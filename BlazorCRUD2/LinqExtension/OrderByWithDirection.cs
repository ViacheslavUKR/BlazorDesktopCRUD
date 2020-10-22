using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlazorCRUD2.Linq
{
    public static class LINQExtension
    {
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                              : source.OrderBy(keySelector);
        }

        public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                              : source.OrderBy(keySelector);
        }

        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource>(this IEnumerable<TSource> source, string keySelector, string descending)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(TSource)).Find(keySelector,false);

            return descending=="DESC" ? source.OrderByDescending(x => prop.GetValue(x))
                              : source.OrderBy(x => prop.GetValue(x));
        }

        public static IOrderedQueryable<TSource> OrderByWithDirection<TSource>(this IQueryable<TSource> source, string keySelector, string descending)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(TSource)).Find(keySelector, false);

            return descending=="ASC" ? source.OrderByDescending(x => prop.GetValue(x))
                              : source.OrderBy(x => prop.GetValue(x));
        }


    }
}
