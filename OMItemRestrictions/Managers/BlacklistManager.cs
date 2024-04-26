using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OMItemRestrictions.Managers
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Normal)]
    public class BlacklistManager : IBlacklistManager
    {
        private readonly IDataStore _DataStore;
        private const string DataKey = "itemBlacklist";
        public Dictionary<string, List<int>> _Blacklist;
        private readonly ILogger<OMItemRestrictions> _Logger;
        public BlacklistManager(IDataStore dataStore, ILogger<OMItemRestrictions> logger) 
        { 
            _DataStore = dataStore;
            _Blacklist = new Dictionary<string, List<int>>();
            _Logger = logger;
        }

        public async void AddBlacklist(string group, int item)
        {
            var blacklist = await _DataStore.LoadAsync<BlacklistData>(DataKey);
            List<int> list;

            if (blacklist.Blacklist.ContainsKey(group))
            {
                list = blacklist.Blacklist[group];
                list.Add(item);
                blacklist.Blacklist[group] = list;
            }
            else
            {
                list = new List<int>();
                list.Add(item);
                blacklist.Blacklist.Add(group, list);
            }
            await _DataStore.SaveAsync(DataKey, blacklist);
            LoadBlacklistToMemory();
        }

        public async void RemoveBlacklist(string group, int item)
        {
            var blacklist = await _DataStore.LoadAsync<BlacklistData>(DataKey);

            if (!blacklist.Blacklist.ContainsKey(group))
            {
                return;
            }
            if (!blacklist.Blacklist[group].Contains(item))
            {
                return;
            }
            var list = blacklist.Blacklist[group];
            list.Remove(item);
            blacklist.Blacklist[group] = list;

            await _DataStore.SaveAsync(DataKey, blacklist);
            LoadBlacklistToMemory();
        }

        public bool IsItemBlacklisted(int item)
        {
            foreach ( var dicts in _Blacklist )
            {
                var group = dicts.Key;
                var list = dicts.Value;

                if (list.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public async void LoadBlacklistToMemory()
        {
            var blacklist = await _DataStore.LoadAsync<BlacklistData>(DataKey);
            _Blacklist = blacklist.Blacklist;
        }
    }
}
