using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Core.Plugins;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OMItemRestrictions.Managers
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Normal)]
    public class BlacklistManager : IBlacklistManager
    {
        private readonly IDataStore _DataStore;
        private const string DataKey = "itemBlacklist";
        public Dictionary<string, List<int>> _Blacklist;
        private readonly ILogger<OMItemRestrictions> _Logger;
        private readonly IPermissionRegistry _PermissionRegistry;
        private readonly IPluginAccessor<OMItemRestrictions> _PluginAccessor;
        public BlacklistManager(IDataStore dataStore, ILogger<OMItemRestrictions> logger, IPermissionRegistry permissionRegistry, IPluginAccessor<OMItemRestrictions> pluginAccessor) 
        { 
            _DataStore = dataStore;
            _Blacklist = new Dictionary<string, List<int>>();
            _Logger = logger;
            _PermissionRegistry = permissionRegistry;
            _PluginAccessor = pluginAccessor;
        }

        public async void AddBlacklist(string group, int item)
        {
            var blacklist = await _DataStore.LoadAsync<BlacklistData>(DataKey);
            group = group.ToUpper();

            if (blacklist.Blacklist.ContainsKey(group))
            {
                blacklist.Blacklist[group].Add(item);
            }
            else
            {
                _PluginAccessor.Instance.RegisterNewPermissionGroup(group);
                blacklist.Blacklist.Add(group, new List<int> { item });
            }
            await _DataStore.SaveAsync(DataKey, blacklist);
            LoadBlacklistToMemory();


        }

        public async Task<string> RemoveBlacklist(string group, int item)
        {
            var blacklist = await _DataStore.LoadAsync<BlacklistData>(DataKey);
            group = group.ToUpper();

            if (!blacklist.Blacklist.ContainsKey(group))
            {
                return $"Group '{group}' not found.";
            }
            if (!blacklist.Blacklist[group].Contains(item))
            {
                return $"Item not found for group '{group}'.";
            }

            var list = blacklist.Blacklist[group];
            list.Remove(item);
            blacklist.Blacklist[group] = list;

            await _DataStore.SaveAsync(DataKey, blacklist);
            LoadBlacklistToMemory();
            return string.Empty;
        }

        public bool IsItemBlacklisted(int item, out string group)
        {
            foreach ( var dicts in _Blacklist )
            {
                var list = dicts.Value;

                if (list.Contains(item))
                {
                    group = dicts.Key.ToUpper();
                    return true;
                }
            }
            group = null;
            return false;
        }

        public async void LoadBlacklistToMemory()
        {
            var blacklist = await _DataStore.LoadAsync<BlacklistData>(DataKey);
            _Blacklist = blacklist.Blacklist;
        }

        public Task<List<string>> BlacklistGroups()
        {
            var list = new List<string>();
            foreach (var group in _Blacklist)
            {
                list.Add(group.Key);
            }
            return Task.FromResult(list);
        }
    }
}
