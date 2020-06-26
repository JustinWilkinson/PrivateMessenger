using System;
using System.Collections.Generic;
using JavaObject = Java.Lang.Object;

namespace PrivateMessenger.JavaHelpers
{
    public static class JavaConvert
    {
        private static readonly Dictionary<Type, IJavaConverter> _cachedConverters = new Dictionary<Type, IJavaConverter>();

        public static JavaObject ToJavaObject<T>(this T obj)
        {
            if (obj is JavaObject javaObject)
            {
                return javaObject;
            }
            else if (obj is IJavaConvertible javaConvertible)
            {
                return javaConvertible.Convert();
            }
            else
            {
                var type = typeof(T);

                if (!_cachedConverters.TryGetValue(type, out var converter))
                {
                    converter = new JavaConverter<T>();
                    _cachedConverters.Add(type, converter);
                }

                return converter.Convert(obj);
            }
        }

        public static void RegisterCustomConverter<T>(IJavaConverter<T> customConverter)
        {
            _cachedConverters.Add(typeof(T), customConverter);
        }
    }
}