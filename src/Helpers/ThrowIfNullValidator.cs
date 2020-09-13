using System;
using System.Collections.Generic;

namespace CSBEF.Helpers
{
    public static class ThrowIfNullValidator
    {
        public static T ThrowIfNull<T>([ValidatedNotNull] this T value, string name = nameof(T))
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
            {
                throw new ArgumentNullException(name);
            }

            return value;
        }

        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class ValidatedNotNullAttribute : Attribute
        {

        }
    }
}