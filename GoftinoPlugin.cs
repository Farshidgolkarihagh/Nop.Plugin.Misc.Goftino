using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.Goftino.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Goftino
{
    /// <summary>
    /// Google Analytics plugin
    /// </summary>
    public class GoftinoPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILanguageService _languageService;
        private readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public GoftinoPlugin(ILocalizationService localizationService,
            IWebHelper webHelper,
            IStoreContext storeContext,
            INopFileProvider fileProvider,
            ILanguageService languageService,
            ISettingService settingService)
        {
            _localizationService = localizationService;
            _languageService = languageService;
            _settingService = settingService;
            _storeContext = storeContext;
            _fileProvider = fileProvider;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { "Goftino" });
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsGoftino/Configure";
        }

        /// <summary>
        /// Gets a type of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component type</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(GoftinoViewComponent);
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var settings = new GoftinoSettings
            {
                TrackingScript = @"
		            <!---start GOFTINO code--->
		            <script type=""text/javascript"">
		              !function(){var i=""{GoftinoCode}"",a=window,d=document;function g(){var g=d.createElement(""script""),s=""https://www.goftino.com/widget/""+i,l=localStorage.getItem(""goftino_""+i);g.async=!0,g.src=l?s+""?o=""+l:s;d.getElementsByTagName(""head"")[0].appendChild(g);}""complete""===d.readyState?g():a.attachEvent?a.attachEvent(""onload"",g):a.addEventListener(""load"",g,!1);}();
		            </script>
		            <!---end GOFTINO code--->
                ",
                Enable = true,
                GoftinoCode = "kskSkV"
            };
            await _settingService.SaveSettingAsync(settings, currentStore.Id);
            
            //locales import
            ImportResourseFiles();

            await base.InstallAsync();
        }
        private async void ImportResourseFiles()
        {
            foreach (var language in await _languageService.GetAllLanguagesAsync())
            {
                var fileName = $"Resources.{language.UniqueSeoCode}.xml";
                var resourceFile = _fileProvider.Combine(_fileProvider.MapPath($"~/Plugins/{GoftinoDefaults.OuputFolder}/Resources"), fileName);
                if (File.Exists(resourceFile))
                {
                    using var streamReader = new StreamReader(resourceFile);
                    _ = _localizationService.ImportResourcesFromXmlAsync(language, streamReader);
                }
            }
        }
        private async void DeleteResourseFiles()
        {
            foreach (var language in await _languageService.GetAllLanguagesAsync())
                _ = _localizationService.DeleteLocaleResourcesAsync(GoftinoDefaults.ResourcePrefix, language.Id);
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<GoftinoSettings>();
            
            //locales delete
            DeleteResourseFiles();;

            await base.UninstallAsync();
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;
    }
}