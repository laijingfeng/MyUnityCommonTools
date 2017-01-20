using System.Reflection;
using System;
using System.Linq;

internal static class ReflectionHelper
{
    #region HasAttribute

    /// <summary>
    /// return Attribute.IsDefined(m, typeof(T));
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="m"></param>
    /// <returns></returns>
    internal static bool HasAttribute<T>(this MemberInfo m) where T : Attribute
    {
#if UNITY_WSA && !UNITY_EDITOR
            return  m.CustomAttributes.Any(o => o.AttributeType.Equals(typeof (T)));
#else
        return Attribute.IsDefined(m, typeof(T));
#endif
    }

    /// <summary>
    /// return Attribute.IsDefined(m, typeof(T));
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="m"></param>
    /// <returns></returns>
    internal static bool HasAttribute<T>(this Type m) where T : Attribute
    {
#if UNITY_WSA && !UNITY_EDITOR
            return m.GetTypeInfo().CustomAttributes.Any(o => o.AttributeType == typeof(T));
#else
        return Attribute.IsDefined(m, typeof(T));
#endif
    }


#if UNITY_WSA && !UNITY_EDITOR
        /// <summary>
        /// return Attribute.IsDefined(m, typeof(T));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        internal static bool HasAttribute<T>(this TypeInfo m) where T : Attribute
        {
            return m.CustomAttributes.Any(o => o.AttributeType == typeof(T));
        }

#endif

    #endregion HasAttribute

    #region GetAttribute

    /// <summary>
    ///  return m.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="m"></param>
    /// <returns></returns>
    internal static T GetAttribute<T>(this MemberInfo m) where T : Attribute
    {
        return m.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
    }

    /// <summary>
    ///  return m.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="m"></param>
    /// <returns></returns>
    internal static T GetAttribute<T>(this Type m) where T : Attribute
    {
#if UNITY_WSA && !UNITY_EDITOR
        return m.GetTypeInfo().GetCustomAttribute<T>();
#else
        return (T)Attribute.GetCustomAttribute(m, typeof(T));
#endif
    }

    #endregion GetAttribute

    /// <summary>
    /// Returns the Return ValueType of the member
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    internal static Type GetMemberType(this MemberInfo member)
    {
        if (member is MethodInfo)
        {
            return ((MethodInfo)member).ReturnType;
        }
        if (member is PropertyInfo)
        {
            return ((PropertyInfo)member).PropertyType;
        }
        return ((FieldInfo)member).FieldType;
    }

    /// <summary>
    /// Returns the Return ValueType of the member
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public static Type GetParamaterType(this MemberInfo member)
    {
        if (member is MethodInfo)
        {
            var p = ((MethodInfo)member).GetParameters().FirstOrDefault();
            if (p == null)
            {
                return null;
            }
            return p.ParameterType;
        }
        if (member is PropertyInfo)
        {
            return ((PropertyInfo)member).PropertyType;
        }
        if (member is FieldInfo)
        {
            return ((FieldInfo)member).FieldType;
        }
        return null;
    }

    /// <summary>
    /// Set the member's instances value
    /// </summary>
    /// <returns></returns>
    internal static void SetMemberValue(this MemberInfo member, object instance, object value)
    {
        if (member is MethodInfo)
        {
            MethodInfo method = ((MethodInfo)member);

            if (method.GetParameters().Any())
            {
                method.Invoke(instance, new[] { value });
            }
            else
            {
                method.Invoke(instance, null);
            }
        }
        else if (member is PropertyInfo)
        {
            ((PropertyInfo)member).SetValue(instance, value, null);
        }
        else
        {
            ((FieldInfo)member).SetValue(instance, value);
        }
    }

    /// <summary>
    /// 执行静态函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <param name="pars"></param>
    internal static void ExecuteStaticMethod(Type type, string methodName, params object[] pars)
    {
        MethodInfo method = type.GetMethod(methodName);
        if (method == null)
        {
            return;
        }
        if (method.GetParameters().Any())
        {
            method.Invoke(null, pars);
        }
        else
        {
            method.Invoke(null, null);
        }
    }

    #region GetMemberValue

    /// <summary>
    /// Get the member's instances value
    /// </summary>
    /// <returns></returns>
    internal static object GetMemberValue(this MemberInfo member, object instance)
    {
        if (member is MethodInfo)
        {
            return ((MethodInfo)member).Invoke(instance, null);
        }
        if (member is PropertyInfo)
        {
            return ((PropertyInfo)member).GetValue(instance, null);
        }
        return ((FieldInfo)member).GetValue(instance);
    }

    /// <summary>
    /// Get the member's instances value
    /// </summary>
    /// <returns></returns>
    internal static object GetMemberValue(this object instance, string memberName)
    {
        MemberInfo member = instance.GetType().GetMember(memberName).FirstOrDefault();

        if (member == null)
        {
            return null;
        }
        if (member is MethodInfo)
        {
            return ((MethodInfo)member).Invoke(instance, null);
        }
        if (member is PropertyInfo)
        {
            return ((PropertyInfo)member).GetValue(instance, null);
        }
        return ((FieldInfo)member).GetValue(instance);

    }

    /// <summary>
    /// Get the member's instances value
    /// </summary>
    /// <returns></returns>
    internal static T GetMemberValue<T>(this MemberInfo member, object instance)
    {
        return (T)GetMemberValue(member, instance);
    }

    #endregion GetMemberValue
}