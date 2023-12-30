
using System.Collections.Generic;

public static class DictUtil
{
    public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
        TKey fromKey, TKey toKey)
    {
        TValue value = dic[fromKey];
        dic.Remove(fromKey);
        dic[toKey] = value;
    }
}


