using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OlympiadViewer.Services
{
    public static class DynamicQueryService
    {
        // =========================================================
        // SEARCH (для string полей)
        // =========================================================

        public static IQueryable<T> ApplySearch<T>(IQueryable<T> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            searchTerm = searchTerm.ToLower();

            var parameter = Expression.Parameter(typeof(T), "x");

            Expression finalExpression = null;

            var stringProperties = typeof(T) .GetProperties().Where(p => p.PropertyType == typeof(string));

            foreach (var property in stringProperties)
            {
                // x.Property
                var propertyExpression = Expression.Property(parameter, property);

                // x.Property != null
                var notNull = Expression.NotEqual( propertyExpression, Expression.Constant(null));

                // x.Property.ToLower()
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

                var toLowerExpression =  Expression.Call(propertyExpression, toLowerMethod);

                // x.Property.ToLower().Contains(searchTerm)
                var containsMethod = typeof(string).GetMethod( "Contains", new[] { typeof(string) });

                var containsExpression = Expression.Call( toLowerExpression, containsMethod, Expression.Constant(searchTerm));

                // x.Property != null &&
                // x.Property.ToLower().Contains(searchTerm)

                var combined = Expression.AndAlso(notNull, containsExpression);

                finalExpression = finalExpression == null ? combined : Expression.OrElse(finalExpression, combined);
            }

            if (finalExpression == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);

            return query.Where(lambda);
        }


        // =========================================================
        // SORTING
        // =========================================================

        public static IQueryable<T> ApplySorting<T>( IQueryable<T> query, string sortColumn, string sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
                return query;

            var property = typeof(T) .GetProperty(
                    sortColumn,
                    BindingFlags.IgnoreCase |
                    BindingFlags.Public |
                    BindingFlags.Instance);

            if (property == null)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");

            var propertyAccess = Expression.Property(parameter, property);

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            string methodName = sortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";

            var resultExpression = Expression.Call( typeof(Queryable),
                    methodName,
                    new Type[]
                    {
                        typeof(T),
                        property.PropertyType
                    },
                    query.Expression,
                    Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<T>(resultExpression);
        }


        // =========================================================
        // NUMERIC FILTER
        // =========================================================

        public static IQueryable<T> ApplyNumericFilter<T>(
            IQueryable<T> query,
            string propertyName,
            decimal? minValue,
            decimal? maxValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return query;

            var property = typeof(T)
                .GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase |
                    BindingFlags.Public |
                    BindingFlags.Instance);

            if (property == null)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");

            var propertyExpression =
                Expression.Property(parameter, property);

            Expression finalExpression = null;

            // >= min
            if (minValue.HasValue)
            {
                var minConstant =
                    Expression.Constant(
                        Convert.ChangeType(
                            minValue.Value,
                            property.PropertyType));

                var greaterThan =
                    Expression.GreaterThanOrEqual(
                        propertyExpression,
                        minConstant);

                finalExpression = greaterThan;
            }

            // <= max
            if (maxValue.HasValue)
            {
                var maxConstant =
                    Expression.Constant(
                        Convert.ChangeType(
                            maxValue.Value,
                            property.PropertyType));

                var lessThan =
                    Expression.LessThanOrEqual(
                        propertyExpression,
                        maxConstant);

                finalExpression = finalExpression == null
                    ? lessThan
                    : Expression.AndAlso(finalExpression, lessThan);
            }

            if (finalExpression == null)
                return query;

            var lambda =
                Expression.Lambda<Func<T, bool>>(
                    finalExpression,
                    parameter);

            return query.Where(lambda);
        }


        // =========================================================
        // DATE FILTER
        // =========================================================

        public static IQueryable<T> ApplyDateFilter<T>(
            IQueryable<T> query,
            string propertyName,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return query;

            var property = typeof(T)
                .GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase |
                    BindingFlags.Public |
                    BindingFlags.Instance);

            if (property == null)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");

            var propertyExpression = Expression.Property(parameter, property);

            Expression finalExpression = null;

            // >= fromDate
            if (fromDate.HasValue)
            {
                var fromConstant = Expression.Constant(fromDate.Value);

                var greaterThan = Expression.GreaterThanOrEqual(
                        propertyExpression,
                        fromConstant);

                finalExpression = greaterThan;
            }

            // <= toDate
            if (toDate.HasValue)
            {
                var toConstant = Expression.Constant(toDate.Value);

                var lessThan = Expression.LessThanOrEqual(
                        propertyExpression,
                        toConstant);

                finalExpression = finalExpression == null
                    ? lessThan
                    : Expression.AndAlso(finalExpression, lessThan);
            }

            if (finalExpression == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(
                    finalExpression,
                    parameter);

            return query.Where(lambda);
        }
    }
}