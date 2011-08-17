using System.Collections.Generic;

namespace HydrasAndHypermedia.Server.Domain
{
    public class Repository<T> where T : IIdentifiable
    {        
        private readonly IDictionary<int, T> dict;

        public Repository(params T[] items)
        {
            dict = new Dictionary<int, T>(items.Length);

            foreach (var item in items)
            {
                dict.Add(item.Id, item);
            }
        }

        public T Get(int itemId)
        {
            return dict[itemId];
        }
    }
}