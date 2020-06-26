using Firebase.Database;
using PrivateMessenger.JavaHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivateMessenger.Extensions
{
    public static class FirebaseExtensions
    {
        public static async Task PutAsync<T>(this DatabaseReference reference, T obj) => await reference.Push().SetValueAsync(JavaConvert.ToJavaObject(obj));

        public static void ForEachChild<T>(this DataSnapshot snapshot, Action<DataSnapshot> action)
        {
            if (snapshot.HasChildren)
            {
                foreach (DataSnapshot child in snapshot.Children.ToEnumerable())
                {
                    if (child.GetValue(true) != null)
                    {
                        action(child);
                    }
                }
            }
        }

        public static IEnumerable<T> MapChildren<T>(this DataSnapshot snapshot, Func<DataSnapshot, T> mapping)
        {
            if (snapshot.HasChildren)
            {
                foreach (DataSnapshot child in snapshot.Children.ToEnumerable())
                {
                    if (child.GetValue(true) != null)
                    {
                        yield return mapping(child);
                    }
                }
            }
        }

        public static string GetChildValueString(this DataSnapshot snapshot, string refPath) => snapshot.Child(refPath)?.GetValue(true)?.ToString();
    }
}