using System;

namespace OmniView.Library.Devices
{
    internal class DeviceAttribute : Attribute
    {
        public string Key {get;}
        public string Name {get;}


        public DeviceAttribute(string key, string name)
        {
            this.Key = key;
            this.Name = name;
        }
    }
}
