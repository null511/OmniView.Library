namespace OmniView.Library.Devices
{
    public class DeviceCapabilities : IDeviceCapabilities
    {
        public bool SupportsPicture {get; set;}
        public bool SupportsVideo {get; set;}
        public bool SupportsPanTilt {get; set;}
        public bool SupportsZoom {get; set;}
    }
}
