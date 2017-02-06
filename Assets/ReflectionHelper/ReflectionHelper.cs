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

    #region SetMemberValue

    /// <summary>
    /// 设置静态值
    /// </summary>
    /// <param name="type"></param>
    /// <param name="memberName"></param>
    internal static void SetStaticMemberValue(Type type, string memberName, params object[] values)
    {
        BindingFlags flag = GetFlag();
        MemberInfo member = type.GetMember(memberName, flag).FirstOrDefault();
        if (member == null)
        {
            return;
        }

        SetMemberValue(member, null, values);
    }

    internal static void SetNonStaticMemberValue(object instance, string memberName, params object[] values)
    {
        BindingFlags flag = GetFlag();
        MemberInfo member = instance.GetType().GetMember(memberName, flag).FirstOrDefault();
        if (member == null)
        {
            return;
        }

        SetMemberValue(member, instance, values);
    }

    /// <summary>
    /// Set the member value
    /// </summary>
    /// <returns></returns>
    internal static void SetMemberValue(this MemberInfo member, object instance = null, params object[] values)
    {
        if (member is MethodInfo)
        {
            MethodInfo method = (MethodInfo)member;

            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            if (method.GetParameters().Any())
            {
                method.Invoke(instance, flag, Type.DefaultBinder, values, null);
            }
            else
            {
                method.Invoke(instance, flag, Type.DefaultBinder, null, null);
            }
        }
        else if (member is PropertyInfo)
        {
            PropertyInfo property = (PropertyInfo)member;
            if (values == null)
            {
                property.SetValue(instance, null, null);
            }
            else
            {
                property.SetValue(instance, values[0], null);
            }
        }
        else if (member is FieldInfo)
        {
            FieldInfo field = (FieldInfo)member;
            if (values == null)
            {
                field.SetValue(instance, null);
            }
            else
            {
                field.SetValue(instance, values[0]);
            }
        }
    }

    #endregion SetMemberValue

    #region GetMemberValue

    /// <summary>
    /// 获取静态值
    /// </summary>
    /// <param name="type"></param>
    /// <param name="memberName"></param>
    /// <param name="values"></param>
    internal static object GetStaticMemberValue(Type type, string memberName)
    {
        if (type == null)
        {
            return null;
        }

        BindingFlags flag = GetFlag();
        MemberInfo member = type.GetMember(memberName, flag).FirstOrDefault();
        if (member == null)
        {
            return null;
        }

        return GetMemberValue(member, null);
    }

    /// <summary>
    /// Get member value
    /// </summary>
    /// <returns></returns>
    internal static object GetNonStaticMemberValue(object instance, string memberName)
    {
        if (instance == null)
        {
            return null;
        }
        BindingFlags flag = GetFlag();
        MemberInfo member = instance.GetType().GetMember(memberName, flag).FirstOrDefault();
        return GetMemberValue(member, instance);
    }

    /// <summary>
    /// Get member value
    /// </summary>
    /// <returns></returns>
    internal static object GetMemberValue(MemberInfo member, object instance)
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

    #endregion GetMemberValue

    #region 辅助

    private static BindingFlags GetFlag()
    {
        return BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
    }

    #endregion 辅助
}