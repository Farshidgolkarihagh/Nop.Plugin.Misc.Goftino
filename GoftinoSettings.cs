using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Goftino
{
    public class GoftinoSettings : ISettings
    {
        public string TrackingScript { get; set; }
        public string GoftinoCode { get; set; }
        public bool Enable { get; set; }
    }
}


