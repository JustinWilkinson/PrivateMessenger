using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using JavaObject = Java.Lang.Object;

namespace PrivateMessenger.JavaHelpers
{
    public interface IJavaConverter
    {
        public JavaObject Convert(object obj);
    }

    public interface IJavaConverter<T> : IJavaConverter
    {
        public JavaObject Convert(T instance);
    }

    public class JavaConverter<T> : IJavaConverter<T>
    {
        private static readonly Func<T, JavaObject> _factory;

        static JavaConverter()
        {
            var propertyGetters = new Dictionary<string, Func<T, object>>();
            foreach (var property in typeof(T).GetProperties())
            {
                propertyGetters.Add(property.Name, (Func<T, object>)Delegate.CreateDelegate(typeof(Func<T, object>), property.GetMethod));
            }

            _factory = instance => new HashMap(propertyGetters.ToDictionary(x => x.Key, x => x.Value(instance)));
        }

        public JavaObject Convert(T instance) => _factory(instance);

        public JavaObject Convert(object obj)
        {
            return obj is T instance ? Convert(instance) : throw new InvalidOperationException($"Converter invalid for object of type {obj.GetType()}!");
        }
    }
}