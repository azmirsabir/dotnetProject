using System.Linq.Expressions;
using learning.Application.Model.DTOs;
using learning.Application.Model.Response;
using Microsoft.EntityFrameworkCore;

namespace learning.SharedKernel.Extensions;

public static class QueryableExtension
{
    public static async Task<PaginatedResult<T>> PaginateAsync<T>(this IQueryable<T> queryable, int perPage=10, int page=1)
    {
        // Apply pagination
        int skip = (page - 1) * perPage;
        queryable = queryable.Skip(skip).Take(perPage);
        
        var totalCount = queryable.Count();
        
        var items = await queryable.ToListAsync();
        
        return new PaginatedResult<T>(
            items,
            totalCount,
            page,
            perPage
        );
    }
    public static IQueryable<T> ApplySearchAndSort<T>(this IQueryable<T> query, QueryParameters parameters)
    {
        if (parameters.Filters is not null)
        {
            foreach (var filter in parameters.Filters)
            {
                string key = filter.Key;
                string value = filter.Value;

                var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .ToArray();

                if (values.Length == 2 && IsDate(values[0]) && IsDate(values[1]))
                {
                    // Between filter for date range
                    var start = DateTime.Parse(values[0]);
                    var end = DateTime.Parse(values[1]);
                    query = query.WhereDynamicBetweenDates(key, start, end);
                }
                else if (values.Length > 1)
                {
                    // OR filter for multiple values
                    query = query.WhereDynamicIn(key, values);
                }
                else
                {
                    // Single value filter: contains or equals
                    if (value.StartsWith("*") && value.EndsWith("*"))
                    {
                        var keyword = value.Trim('*');
                        query = query.WhereDynamicContains(key, keyword);
                    }
                    else
                    {
                        query = query.WhereDynamicEquals(key, value);
                    }
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(parameters.Sort))
        {
            var sortField = parameters.Sort.TrimStart('-');
            var isDesc = parameters.Sort.StartsWith("-");
            query = query.OrderByDynamic(sortField, isDesc);
        }
    
        return query;
    }
    
    public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> query, QueryParameters parameters) where T : class
    {
        if (string.IsNullOrWhiteSpace(parameters.Include)) return query;

        var includePaths = parameters.Include.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var includePath in includePaths)
        {
            query = query.Include(includePath.Trim());
        }

        return query;
    }
    //Helper for In
    public static IQueryable<T> WhereDynamicIn<T>(this IQueryable<T> source, string propertyName, string[] values)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, propertyName);

        // Create expressions for values
        var constants = values.Select(v => Expression.Constant(Convert.ChangeType(v, property.Type), property.Type)).ToArray();

        // Build OR expression: x.Prop == values[0] || x.Prop == values[1] || ...
        Expression? combined = null;

        foreach (var constant in constants)
        {
            var equals = Expression.Equal(property, constant);
            combined = combined == null ? equals : Expression.OrElse(combined, equals);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combined!, parameter);
        return source.Where(lambda);
    }

    // Helper for equals
    private static IQueryable<T> WhereDynamicEquals<T>(this IQueryable<T> source, string property, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var member = Expression.PropertyOrField(parameter, property);
        var constant = Expression.Constant(Convert.ChangeType(value, member.Type));
        var body = Expression.Equal(member, constant);
        var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
        return source.Where(predicate);
    }

    // Helper for contains (like)
    private static IQueryable<T> WhereDynamicContains<T>(this IQueryable<T> source, string property, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var member = Expression.PropertyOrField(parameter, property);
        var method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        var constant = Expression.Constant(value);
        var body = Expression.Call(member, method, constant);
        var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
        return source.Where(predicate);
    }

    // Helper for dynamic sorting
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName, bool descending)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);
        string methodName = descending ? "OrderByDescending" : "OrderBy";
        var result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type)
            .Invoke(null, new object[] { source, lambda });
        return (IQueryable<T>)result!;
    }
    
    public static IQueryable<T> WhereDynamicBetweenDates<T>(this IQueryable<T> source, string propertyName, DateTime start, DateTime end)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, propertyName);

        if (property.Type != typeof(DateTime) && property.Type != typeof(DateTime?))
            throw new ArgumentException("Property must be a DateTime or Nullable<DateTime>");

        var startConstant = Expression.Constant(start, property.Type);
        var endConstant = Expression.Constant(end, property.Type);

        var greaterOrEqual = Expression.GreaterThanOrEqual(property, startConstant);
        var lessOrEqual = Expression.LessThanOrEqual(property, endConstant);
        var between = Expression.AndAlso(greaterOrEqual, lessOrEqual);

        var lambda = Expression.Lambda<Func<T, bool>>(between, parameter);
        return source.Where(lambda);
    }

    public static IQueryable<T> WhereDynamicComparison<T>(this IQueryable<T> source, string propertyName, string op, double value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, propertyName);
        var constant = Expression.Constant(Convert.ChangeType(value, property.Type));

        Expression comparison = op switch
        {
            ">=" => Expression.GreaterThanOrEqual(property, constant),
            "<=" => Expression.LessThanOrEqual(property, constant),
            ">"  => Expression.GreaterThan(property, constant),
            "<"  => Expression.LessThan(property, constant),
            _    => throw new ArgumentException($"Invalid operator {op}")
        };

        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
        return source.Where(lambda);
    }
    private static bool IsDate(string s)
    {
        return DateTime.TryParse(s, out _);
    }
}