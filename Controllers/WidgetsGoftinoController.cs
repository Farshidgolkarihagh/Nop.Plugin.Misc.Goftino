using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Goftino.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Goftino.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class WidgetsGoftinoController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public WidgetsGoftinoController(
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods
        
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var goftinoSettings = await _settingService.LoadSettingAsync<GoftinoSettings>(storeScope);

            var model = new ConfigurationModel
            {
                TrackingScript = goftinoSettings.TrackingScript,
                Enable = goftinoSettings.Enable,
                GoftinoCode = goftinoSettings.GoftinoCode,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.TrackingScript_OverrideForStore = await _settingService.SettingExistsAsync(goftinoSettings, x => x.TrackingScript, storeScope);
                model.Enable_OverrideForStore = await _settingService.SettingExistsAsync(goftinoSettings, x => x.Enable, storeScope);
                model.GoftinoCode_OverrideForStore = await _settingService.SettingExistsAsync(goftinoSettings, x => x.GoftinoCode, storeScope);
            }

            return View("~/Plugins/Widgets.Goftino/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var goftinoSettings = await _settingService.LoadSettingAsync<GoftinoSettings>(storeScope);

            goftinoSettings.TrackingScript = model.TrackingScript;
            goftinoSettings.GoftinoCode = model.GoftinoCode;
            goftinoSettings.Enable = model.Enable;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(goftinoSettings, x => x.TrackingScript, model.TrackingScript_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(goftinoSettings, x => x.Enable, model.Enable_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(goftinoSettings, x => x.GoftinoCode, model.GoftinoCode_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}