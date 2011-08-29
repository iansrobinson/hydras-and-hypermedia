using System;

namespace HydrasAndHypermedia.Server.Hypermedia
{
    [Flags]
    public enum Not
    {
        Null = 1,
        Empty = 2,
        Whitespace = 4,
        NullOrEmptyOrWhitespace = Null | Empty | Whitespace
    }

    public static class CheckString
    {
        public static void Is(Not values, string s, string name)
        {
            if ((values & Not.Null) == Not.Null)
            {
                if (s == null)
                {
                    throw new ArgumentNullException(name);
                }
            }

            if ((values & Not.Empty) == Not.Empty)
            {
                if (s != null && s.Length.Equals(0))
                {
                    throw new ArgumentException("Value cannot be empty.", name);
                }
            }

            if ((values & Not.Whitespace) == Not.Whitespace)
            {
                if ((!string.IsNullOrEmpty(s)) && (s.Trim().Length == 0))
                {
                    throw new ArgumentException("Value cannot be whitespace.", name);
                }
            }
        }
    }
}