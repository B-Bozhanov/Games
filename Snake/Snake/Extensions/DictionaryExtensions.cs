namespace SnakeGame.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> dict)
        {
            var isAdded = false;
            foreach (var item in dict)
            {
                dictionary.Add(item);
                isAdded = true;
            }

            return isAdded;
        }

        public static bool RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, ICollection<TKey> keys)
        {
            var isRemoved = false;
            foreach (var key in keys)
            {
                dictionary.Remove(key);
                isRemoved = true;
            }

            return isRemoved;
        }
    }
}