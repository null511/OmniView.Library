using System.Xml.Linq;

namespace OmniView.Library.Common
{
    internal static class XmlExtensions
    {
        public static string GetValue(this XElement node, string name, string defaultValue = null)
        {
            return (name.StartsWith("@")
                ? node.Attribute(name.Substring(1))?.Value
                : node.Element(name)?.Value)
                    ?? defaultValue;
        }

        public static T GetValue<T>(this XElement node, string name, T defaultValue = default(T))
        {
            var value = name.StartsWith("@")
                ? node.Attribute(name.Substring(1))?.Value
                : node.Element(name)?.Value;

            if (value == null) return defaultValue;
            return value.To<T>();
        }

        public static XElement GetOrCreatePath(this XContainer parentNode, string path)
        {
            var segments = path.Split('/');

            //XContainer parentNode = document.Root;
            foreach (var nodeName in segments) {
                var node = parentNode.Element(nodeName);

                if (node == null) {
                    node = new XElement(nodeName);
                    parentNode.Add(node);
                }

                parentNode = node;
            }

            return (XElement)parentNode;
        }
    }
}
