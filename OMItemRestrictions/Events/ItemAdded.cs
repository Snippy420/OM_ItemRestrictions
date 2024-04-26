using OMItemRestrictions.Managers;
using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Eventing;
using OpenMod.API.Persistence;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players.Inventory.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace OMItemRestrictions.Events
{
    public class ItemAdded : IEventListener<UnturnedPlayerTakingItemEvent>
    {
        private readonly IDataStore _DataStore;
        private const string DataKey = "itemBlacklist";
        private readonly IBlacklistManager _blacklistManager;
        public ItemAdded(IDataStore dataStore, IBlacklistManager blacklistManager)
        {
            _DataStore = dataStore;
            _blacklistManager = blacklistManager;
        }
        public async Task HandleEventAsync(object sender, UnturnedPlayerTakingItemEvent @event)
        {
            var blacklist = await _DataStore.LoadAsync<BlacklistData>(DataKey);
            var pickupItem = @event.ItemData.item;

            if (_blacklistManager.IsItemBlacklisted(pickupItem.id))
            {
                @event.IsCancelled = true;
                await @event.Player.PrintMessageAsync("This item is blacklisted", Color.Red);
                return;
            }
        }
    }
}
