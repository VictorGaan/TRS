using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TrueSkills.APIs;

namespace TrueSkills
{
    public static class ExtensionMethods
    {

        public static (bool isValid, ErrorAPI response) IsValidJson(this string response)
        {
            var valid = response.Contains("error");
            if (valid)
            {
                return (valid, JsonConvert.DeserializeObject<ErrorAPI>(response));
            }
            return (valid, null);
        }

        public static T GetValueFromJson<T>(this string response)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(response);

            }
            catch (Exception ex)
            {

                Debug.WriteLine("ERROR " + ex.Message);
            }
            return default(T);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
