using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;

namespace OMItemRestrictions.Managers
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Normal)]
    public class BlacklistManager : IBlacklistManager
    {
        private readonly IDataStore _dataStore;
        private readonly ILogger<OMItemRestrictions> _logger;
        private readonly IPermissionRegistry _permissionRegistry;
        private readonly IPluginAccessor<OMItemRestrictions> _pluginAccessor;
        private readonly IStringLocalizer _localizer;

        private Dictionary<string, List<int>> _blacklist = new();
        
        private const string DataKey = "itemBlacklist";
        
        public BlacklistManager(IDataStore dataStore, 
            ILogger<OMItemRestrictions> logger, 
            IPermissionRegistry permissionRegistry, 
            IPluginAccessor<OMItemRestrictions> pluginAccessor, 
            IStringLocalizer localizer) 
        { 
            _dataStore = dataStore;
            _logger = logger;
            _permissionRegistry = permissionRegistry;
            _pluginAccessor = pluginAccessor;
            _localizer = localizer;
        }

        public async UniTask AddBlacklist(string group, int item)
        {
            var blacklist = await _dataStore.LoadAsync<BlacklistData>(DataKey);
            group = group.ToUpper();

            if (blacklist.Blacklist.ContainsKey(group))
            {
                blacklist.Blacklist[group].Add(item);
            }
            else
            {
                _pluginAccessor.Instance.RegisterNewPermissionGroup(group);
                blacklist.Blacklist.Add(group, new List<int> { item });
            }
            await _dataStore.SaveAsync(DataKey, blacklist);
            await LoadBlacklistToMemory();

        }

        public async UniTask RemoveBlacklist(string group, int item)
        {
            var blacklist = await _dataStore.LoadAsync<BlacklistData>(DataKey);
            group = group.ToUpper();

            if (!blacklist.Blacklist.ContainsKey(group))
                throw new UserFriendlyException(_localizer["GroupNotFound", new { Group = group }]);
            if (!blacklist.Blacklist[group].Contains(item))
                throw new UserFriendlyException(_localizer["ItemNotFound", new { Group = group }]);

            blacklist.Blacklist[group].Remove(item);
            if (blacklist.Blacklist[group].Count == 0)
            {
                blacklist.Blacklist.Remove(group);
            }

            await _dataStore.SaveAsync(DataKey, blacklist);
            await LoadBlacklistToMemory();
        }

        public bool IsItemBlacklisted(int item, out string group)
        {
            foreach ( var dicts in _blacklist )
            {
                var list = dicts.Value;

                if (!list.Contains(item)) continue;
                
                group = dicts.Key.ToUpper();
                return true;
            }
            group = null;
            return false;
        }

        public async UniTask LoadBlacklistToMemory()
        {
            var blacklist = await _dataStore.LoadAsync<BlacklistData>(DataKey);
            _blacklist = blacklist.Blacklist;
        }

        public UniTask<List<string>> BlacklistGroups()
        {
            var list = new List<string>();
            foreach (var group in _blacklist)
            {
                list.Add(group.Key);
            }
            return UniTask.FromResult(list);
        }
    }
}
