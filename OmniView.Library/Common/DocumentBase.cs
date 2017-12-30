using System;
using System.Xml.Linq;

namespace OmniView.Library.Common
{
    internal abstract class DocumentBase
    {
        protected XDocument document;


        protected DocumentBase()
        {
            document = new XDocument();
        }

        protected string GetValue(string path, string defaultValue = null)
        {
            return document?.Root?.GetValue(path, defaultValue);
        }

        protected T GetValue<T>(string path, T defaultValue = default(T))
        {
            if (document?.Root == null) return defaultValue;
            return document.Root.GetValue(path, defaultValue);
        }

        protected void SetValue<T>(string path, T value)
        {
            if (document.Root == null)
                throw new ApplicationException("Document does not contain a root element!");

            document.Root.GetOrCreatePath(path).SetValue(value);
        }
    }
}
