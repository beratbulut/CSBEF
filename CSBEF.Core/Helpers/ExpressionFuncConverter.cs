using System;
using System.Linq.Expressions;

namespace CSBEF.Core.Helpers {
    public static class ExpressionFuncConverter {
        public static Expression<Func<TTo, TR>> Convert<TTo, TFrom, TR> (Expression<Func<TFrom, TR>> e) {
            if (e == null)
                throw new ArgumentNullException (nameof (e));

            var oldParameter = e.Parameters[0];
            var newParameter = Expression.Parameter (typeof (TTo), oldParameter.Name);
            var converter = new ConversionVisitor (newParameter, oldParameter);
            var newBody = converter.Visit (e.Body);
            return Expression.Lambda<Func<TTo, TR>> (newBody, newParameter);
        }
    }
}