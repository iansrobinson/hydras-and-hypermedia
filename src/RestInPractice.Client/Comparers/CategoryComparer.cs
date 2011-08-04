using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace RestInPractice.Client.Comparers
{
    public class CategoryComparer : IEqualityComparer<SyndicationCategory>
    {
        public static readonly IEqualityComparer<SyndicationCategory> Instance = new CategoryComparer();
        
        private CategoryComparer()
        {
        }

        public bool Equals(SyndicationCategory x, SyndicationCategory y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(SyndicationCategory obj)
        {
            return obj.GetHashCode();
        }
    }
}