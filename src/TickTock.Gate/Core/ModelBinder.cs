using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TickTock.Gate.Core
{
    public class DynamicModelBinder : IModelBinder
    {
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            IDictionary<string, object> data = GetDataFields(context);
            DynamicDictionary model = DynamicDictionary.Create(data);

            return model;
        }

        private static IDictionary<string, object> GetDataFields(NancyContext context)
        {
            return Merge(new IDictionary<string, string>[]
            {
                ConvertDynamicDictionary(context.Request.Form),
                ConvertDynamicDictionary(context.Request.Query),
                ConvertDynamicDictionary(context.Parameters)
            });
        }

        private static IDictionary<string, object> Merge(IEnumerable<IDictionary<string, string>> dictionaries)
        {
            IEqualityComparer<string> comparer = StringComparer.InvariantCultureIgnoreCase;
            Dictionary<string, object> output = new Dictionary<string, object>(comparer);

            foreach (IDictionary<string, string> dictionary in dictionaries.Where(d => d != null))
            {
                foreach (KeyValuePair<string, string> kvp in dictionary)
                {
                    if (!output.ContainsKey(kvp.Key))
                    {
                        output.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return output;
        }

        private static IDictionary<string, string> ConvertDynamicDictionary(DynamicDictionary dictionary)
        {
            return dictionary.GetDynamicMemberNames().ToDictionary(
                    memberName => memberName,
                    memberName => (string)dictionary[memberName]);
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof(DynamicDictionary);
        }
    }
}