using OmniView.Library.Common;
using System;
using System.Linq;
using System.Text;

namespace OmniView.Library.Network
{
    public class UrlBuilder
    {
        public string Path {get; set;}
        public string Fragment {get; set;}
        public QueryStringBuilder Query {get;}


        public UrlBuilder()
        {
            Query = new QueryStringBuilder();
        }

        public UrlBuilder(params string[] path) : this()
        {
            Path = NetPath.Combine(path);
        }

        public UrlBuilder AppendPath(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            Path = NetPath.Combine(Path ?? string.Empty, path);
            return this;
        }

        public override string ToString()
        {
            var url = new StringBuilder(Path);

            if (Query.Any())
                url.Append(Query.ToString());

            if (Fragment != null)
                url.Append(Fragment);

            return url.ToString();
        }
    }
}
