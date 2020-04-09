using System;
using System.Linq;
using System.Linq.Expressions;

namespace CSBEF.Core.Helpers {
    public class ConversionVisitor : ExpressionVisitor {
        private readonly ParameterExpression newParameter;
        private readonly ParameterExpression oldParameter;

        public ConversionVisitor (ParameterExpression newParameter, ParameterExpression oldParameter) {
            this.newParameter = newParameter;
            this.oldParameter = oldParameter;
        }

        protected override Expression VisitParameter (ParameterExpression node) {
            return newParameter;
        }

        protected override Expression VisitMember (MemberExpression node) {
            if(node == null)
                throw new ArgumentNullException(nameof(node));
            
            if (node.Expression != oldParameter)
                return base.VisitMember (node);

            var newObj = Visit (node.Expression);
            var newMember = newParameter.Type.GetMember (node.Member.Name).First ();
            return Expression.MakeMemberAccess (newObj, newMember);
        }
    }
}