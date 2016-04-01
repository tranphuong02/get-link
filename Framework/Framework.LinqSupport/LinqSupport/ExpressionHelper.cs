using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.LinqSupport.LinqSupport
{
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression from, to;

        public ReplaceExpressionVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }
    }

    public static class ExpressionHelper
    {
        public static Type GetExpressionResultType(Type type, string propertyOrExpression)
        {
            LambdaExpression lambda = DynamicExpression.ParseLambda(type, null, propertyOrExpression, null);
            return lambda.Body.Type;
        }

        public static Type GetExpressionResultType<T>(string propertyOrExpression)
        {
            return GetExpressionResultType(typeof(T), propertyOrExpression);
        }

        public static Expression<Func<T, bool>> BuildOperation<T>(Func<Expression, Expression, Expression> bodyFunc, string propertyName, object value)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), propertyName);
            LambdaExpression lambda = DynamicExpression.ParseLambda(typeof(T), null, propertyName, null);

            var leftVisitor = new ReplaceExpressionVisitor(lambda.Parameters[0], param);
            var left = leftVisitor.Visit(lambda.Body);

            ConstantExpression constant = Expression.Constant(value, lambda.Body.Type);
            var body = bodyFunc(left, constant);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        public static Expression<Func<T, bool>> Equal<T>(string propertyName, object value)
        {
            return BuildOperation<T>(Expression.Equal, propertyName, value);
        }

        public static Expression<Func<T, bool>> NotEqual<T>(string propertyName, object value)
        {
            return BuildOperation<T>(Expression.NotEqual, propertyName, value);
        }

        public static Expression<Func<T, bool>> LessThan<T>(string propertyName, object value)
        {
            return BuildOperation<T>(Expression.LessThan, propertyName, value);
        }

        public static Expression<Func<T, bool>> LessThanOrEqual<T>(string propertyName, object value)
        {
            return BuildOperation<T>(Expression.LessThanOrEqual, propertyName, value);
        }

        public static Expression<Func<T, bool>> GreaterThan<T>(string propertyName, object value)
        {
            return BuildOperation<T>(Expression.GreaterThan, propertyName, value);
        }

        public static Expression<Func<T, bool>> GreaterThanOrEqual<T>(string propertyName, object value)
        {
            return BuildOperation<T>(Expression.GreaterThanOrEqual, propertyName, value);
        }

        public static Expression<Func<T, bool>> Contains<T>(string propertyName, string value)
        {
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            return BuildOperation<T>((property, constant) => Expression.Call(property, method, constant), propertyName, value);
        }

        public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null)
            {
                return expr2;
            }
            if (expr2 == null)
            {
                return expr1;
            }
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }

        public static Expression<Func<T, bool>> OrElse<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null)
            {
                return expr2;
            }
            if (expr2 == null)
            {
                return expr1;
            }
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(left, right), parameter);
        }
    }
}