using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Goftino.Models
{
    public record ConfigurationModel : BaseNopModel
    {
	// able to set configuration for each store
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Goftino.Enable")]
        public bool Enable { get; set; }
        public bool Enable_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Goftino.TrackingScript")]
        public string TrackingScript { get; set; }
        public bool TrackingScript_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Goftino.GoftinoCode")]
        public string GoftinoCode { get; set; }
        public bool GoftinoCode_OverrideForStore { get; set; }
    }
}
