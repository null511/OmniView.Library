using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OmniView.Library.Network
{
    public class QueryStringBuilder : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> items;

        public object this[string key] {
            get => items[key];
            set => items[key] = value;
        }


        public QueryStringBuilder()
        {
            items = new Dictionary<string, object>();
        }

        public QueryStringBuilder Append(string name, object value)
        {
            items[name] = value;
            return this;
        }

        public override string ToString()
        {
            if (!items.Any()) return string.Empty;

            return "?" + string.Join("&", items.Select(x => {
                var value = x.Value?.ToString() ?? string.Empty;
                return $"{x.Key}={HttpUtility.UrlEncode(value)}";
            }));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
