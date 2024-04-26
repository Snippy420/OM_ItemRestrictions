using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html

[assembly: PluginMetadata("OMItemRestrictions", DisplayName = "OM.ItemRestrictions")]

namespace OMItemRestrictions
{
    public class OMItemRestrictions : OpenModUniversalPlugin
    {
        private readonly IConfiguration _Configuration;
        private readonly IStringLocalizer _StringLocalizer;
        private readonly ILogger<OMItemRestrictions> _Logger;
        private readonly IDataStore _DataStore;
        private const string DataKey = "itemBlacklist";
        private readonly IBlacklistManager _blacklistManager;

        public OMItemRestrictions(
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            ILogger<OMItemRestrictions> logger,
            IServiceProvider serviceProvider,
            IDataStore dataStore,
            IBlacklistManager blacklistManager) : base(serviceProvider)
        {
            _Configuration = configuration;
            _StringLocalizer = stringLocalizer;
            _Logger = logger;
            _DataStore = dataStore;
            _blacklistManager = blacklistManager;
        }

        protected override async Task OnLoadAsync()
        {
            _Logger.LogInformation(_StringLocalizer["plugin_events:plugin_start"]);

            if (!await _DataStore.ExistsAsync(DataKey))
            {
                await _DataStore.SaveAsync(DataKey, new BlacklistData
                {
                    Blacklist = new Dictionary<string, List<int>>()
                });
            }
            _blacklistManager.LoadBlacklistToMemory();
        }

        protected override async Task OnUnloadAsync()
        {
            _Logger.LogInformation(_StringLocalizer["plugin_events:plugin_stop"]);
        }
    }
}
