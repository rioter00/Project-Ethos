using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(t => t.GetSetMethod() != null);
        }

        public static Type GetResponseType(this Type operationType)
        {
            return operationType.GetInterfaces().Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IOperation<>)).GetGenericArguments()[0];
        }

        public static Type GetHandlerInterfaceType(this Type concreteType)
        {
            return concreteType.GetInterfaces().Single(r => r.IsGenericType && (r.GetGenericTypeDefinition() == typeof (IOperationHandler<>) || r.GetGenericTypeDefinition() == typeof (IOperationHandler<,>)));
        }

        public static bool IsOperation(this Type type)
        {
            return type.IsClass && !type.IsAbstract && typeof (IOperation).IsAssignableFrom(type);
        }

        public static bool IsOperationResponse(this Type type)
        {
            return type.IsClass && !type.IsAbstract && typeof (IOperationResponse).IsAssignableFrom(type);
        }

        public static bool IsOperationHandler(this Type type)
        {
            return type.IsClass && !type.IsAbstract && typeof (IOperationHandler).IsAssignableFrom(type);
        }

        public static bool IsCustomSerializer(this Type type)
        {
            return typeof (ICustomSerializer).IsAssignableFrom(type);
        }
    }
}