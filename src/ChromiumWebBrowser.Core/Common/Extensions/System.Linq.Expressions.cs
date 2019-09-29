using System.Reflection;

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static MemberInfo GetPropertyInfo<T>(this Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null && property.Body is UnaryExpression)
                memberExpression = (property.Body as UnaryExpression).Operand as MemberExpression;
            if (memberExpression == null)
                throw new FormatException("Lambda should be a property");
            else
                return memberExpression.Member;
        }
    }
}