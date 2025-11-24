using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html

[assembly: PluginMetadata("OMItemRestrictions", DisplayName = "OMItemRestrictions", Author = ".f.i.n.")]

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
        private readonly IPermissionRegistry _permissionRegistry;

        public OMItemRestrictions(
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            ILogger<OMItemRestrictions> logger,
            IServiceProvider serviceProvider,
            IDataStore dataStore,
            IBlacklistManager blacklistManager,
            IPermissionRegistry permissionRegistry) : base(serviceProvider)
        {
            _Configuration = configuration;
            _StringLocalizer = stringLocalizer;
            _Logger = logger;
            _DataStore = dataStore;
            _blacklistManager = blacklistManager;
            _permissionRegistry = permissionRegistry;
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
            else
            {
                _blacklistManager.LoadBlacklistToMemory();
                foreach (var item in (await _DataStore.LoadAsync<BlacklistData>(DataKey)).Blacklist)
                {
                    _Logger.LogInformation($"Added {item.Key} to the permission groups.");
                    _permissionRegistry.RegisterPermission(this, $"blacklist.group.{item.Key}", $"Permission to the {item.Key} blacklist group");
                }
            }
        }

        protected override Task OnUnloadAsync()
        {
            _Logger.LogInformation(_StringLocalizer["plugin_events:plugin_stop"]);
            return Task.CompletedTask;
        }

        public void RegisterNewPermissionGroup(string name)
        {
            _permissionRegistry.RegisterPermission(this, $"blacklist.group.{name}", $"Permission to the {name} blacklist group");
        }
    }
}
