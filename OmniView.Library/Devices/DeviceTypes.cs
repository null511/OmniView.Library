using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OmniView.Library.Devices
{
    public static class DeviceTypes
    {
        public static IEnumerable<DeviceInfo> All => GetDevices();


        public static IDevice Create(IDeviceDescription description)
        {
            var info = GetDevices().FirstOrDefault(x => string.Equals(x.DeviceKey, description.Type, StringComparison.OrdinalIgnoreCase));

            if (info == null)
                throw new ApplicationException($"Device type '{description.Type}' not found!");

            var device = Activator.CreateInstance(info.DeviceType, description) as IDevice;
            if (device == null) throw new ApplicationException($"Failed to create device of type '{info.DeviceType.Name}'!");

            return device;
        }
        
        private static IEnumerable<DeviceInfo> GetDevices()
        {
            var descriptionType = typeof(IDevice);

            var deviceDescriptions = Assembly.GetExecutingAssembly().ExportedTypes
                .Where(t => t.IsClass && !t.IsAbstract && descriptionType.IsAssignableFrom(t));

            foreach (var deviceType in deviceDescriptions) {
                var attr = deviceType.GetCustomAttribute<DeviceAttribute>();
                if (attr == null) continue;

                yield return new DeviceInfo {
                    DeviceKey = attr.Key,
                    DeviceName = attr.Name,
                    DeviceType = deviceType,
                };
            }
        }
    }
}
