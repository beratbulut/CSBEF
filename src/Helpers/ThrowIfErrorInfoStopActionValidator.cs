using System;
using System.Collections.Generic;
using CSBEF.Interfaces;

namespace CSBEF.Helpers
{
    public static class ThrowIfErrorInfoStopActionValidator
    {
        public static IReturnModel<T> ThrowIfErrorInfoStopAction<T>([ValidatedErrorInfoStopAction] this IReturnModel<T> value)
        {
            if (EqualityComparer<IReturnModel<T>>.Default.Equals(value, default))
            {
                value.ThrowIfNull();
                throw new Exception(value.ErrorInfo.Message + " (" + value.ErrorInfo.Code + ")");
            }

            return value;
        }

        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class ValidatedErrorInfoStopActionAttribute : Attribute
        {

        }
    }
}