using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


   public static class LinqTool
    {
    /// <summary>
    /// 1.HashSet<T>类，主要被设计用来存储集合，做高性能集运算，例如两个集合求交集、并集、差集等。从名称可以看出，它是基于Hash的，可以简单理解为没有Value的Dictionary。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();//创建一个高速HashSet类
            foreach (TSource element in source)//将LEnumberable中的值拿出。
            {
                if (seenKeys.Add(keySelector(element)))//如果可以填入HashSet
                {
                    yield return element;//单个返回
                }
            }
        }

    //public static IEnumerable<TSource> DistinctBy2<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    //{
    //    //source.Concat()
    //    HashSet<TKey> seenKeys = new HashSet<TKey>();//创建一个高速HashSet类
    //    foreach (TSource element in source)//将LEnumberable中的值拿出。
    //    {
    //        if (seenKeys.Add(keySelector(element)))//如果可以填入HashSet
    //        {
                
    //            yield return element;//单个返回
    //        }
    //    }
    //}


    ///yield return的例子
    //public static List<int> Unique(IEnumerable<int> nums)
    //{
    //    List<int> uniqueVals = new List<int>();

    //    foreach (int num in nums)
    //    {
    //        if (!uniqueVals.ContainsKey(num))
    //        {
    //            uniqueVals.Add(num);
    //            Console.WriteLine(num);
    //        }
    //    }
    //    return uniqueVals;
    //}
    ///yield return的例子
    //public static IEnumerable<int> Unique2(IEnumerable<int> nums)
    //{
    //    List<int> uniqueVals = new List<int>();

    //    foreach (int num in nums)
    //    {
    //        if (!uniqueVals.ContainsKey(num))
    //        {
    //            uniqueVals.Add(num);
    //            yield return num;
    //        }
    //    }
    //}
}

