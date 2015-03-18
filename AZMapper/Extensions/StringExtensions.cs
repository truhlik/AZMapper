using System;
using System.Linq;

namespace AZMapper.Extensions
{
    public static class StringExtensions
    {
        public static bool IsInArray(this string[] array, string target, bool caseSensitive = true)
        {
            var option = caseSensitive ? StringComparison.InvariantCulture
                                       : StringComparison.InvariantCultureIgnoreCase;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(target, option))
                    return true;
            }

            return false;
        }
    }
}
