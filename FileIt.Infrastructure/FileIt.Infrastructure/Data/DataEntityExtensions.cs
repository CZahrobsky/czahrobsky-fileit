using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace FileIt.Infrastructure.Data;

    public static class DataEntityExtensions
    {
        public static T Hydrate<T>(this DataRow row) where T : class, new()
        {
            T obj = new T();
            foreach (DataColumn col in row.Table.Columns)
            {
                var val = row[col];
                if (val != DBNull.Value)
                {
                    var pi = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals(col.ColumnName, StringComparison.OrdinalIgnoreCase));
                    if (pi != null && pi.CanWrite)
                    {
                        pi.SetValue(obj, val);
                    }
                }
            }
            return obj;
        }

        public static void HydrateKeyValue<T>(this T obj, string key, string value) where T : class, new()
        {
            var pi = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (pi != null && pi.CanWrite)
            {
                object propertyValue = Convert.ChangeType(value, pi.PropertyType);
                pi.SetValue(obj, propertyValue);
            }
        }

        /// <summary>
        /// Gets an existing entity from the DbSet that matches all properties except the primary key (*Id).
        /// Useful for finding duplicates based on business logic rather than identity.
        /// </summary>      
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="dbSet">The DbSet to search</param>
        /// <param name="entity">The entity to match against</param>
        /// <returns>The existing matching entity, or null if not found</returns>
        public static T GetExisting<T>(this DbSet<T> dbSet, T entity) where T : class
        {
            if (entity == null)
                return null;

            var entityType = typeof(T);
            var allProperties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Get all properties except those ending with "Id" (primary/foreign keys)
            var matchingProperties = allProperties
                .Where(p => 
                    p.CanRead && 
                    !p.Name.EndsWith("Id") && 
                    !p.Name.EndsWith("_Id") &&
                    p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                .ToList();

            if (matchingProperties.Count == 0)
            {
                // If no non-Id properties found, fall back to returning null
                return null;
            }

            // Build a predicate that matches all non-Id properties
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "x");
            System.Linq.Expressions.Expression predicateBody = null;

            foreach (var prop in matchingProperties)
            {
                var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, prop.Name);
                var entityValue = prop.GetValue(entity);
                var constant = System.Linq.Expressions.Expression.Constant(entityValue, prop.PropertyType);
                var equality = System.Linq.Expressions.Expression.Equal(propertyAccess, constant);

                predicateBody = predicateBody == null
                    ? equality
                    : System.Linq.Expressions.Expression.AndAlso(predicateBody, equality);
            }

            if (predicateBody == null)
                return null;

            var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(predicateBody, parameter);
            return dbSet.FirstOrDefault(lambda);
        }
    }

