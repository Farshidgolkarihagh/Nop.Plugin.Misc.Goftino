using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Goftino.Components
{
    public class GoftinoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public GoftinoViewComponent(
            ILogger logger,
            ISettingService settingService,
            IStoreContext storeContext
        )
        {
            _logger = logger;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            try
            {
                var currentStore = await _storeContext.GetCurrentStoreAsync();
                var goftinoSettings = await _settingService.LoadSettingAsync<GoftinoSettings>(currentStore.Id);
                if(!goftinoSettings.Enable)
                    return new HtmlContentViewComponentResult(new HtmlString(string.Empty));
                goftinoSettings.TrackingScript = goftinoSettings.TrackingScript.Replace("{GoftinoCode}", goftinoSettings.GoftinoCode);
                return View("~/Plugins/Widgets.Goftino/Views/PublicInfo.cshtml", goftinoSettings.TrackingScript);
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Error, "Error creating scripts for Google eCommerce tracking", ex.ToString());
            }
            return new HtmlContentViewComponentResult(new HtmlString(string.Empty));
        }

        #endregion
    }
}