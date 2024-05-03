using Microsoft.Extensions.Logging;
using OMItemRestrictions.Managers;
using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Inventory.Events;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace OMItemRestrictions.Events
{
    public class UnturnedPlayerTakingItemListener : IEventListener<UnturnedPlayerTakingItemEvent>
    {
        private readonly IBlacklistManager _blacklistManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IUserManager _userManager;
        private readonly ILogger<OMItemRestrictions> _Logger;
        public UnturnedPlayerTakingItemListener(
            IBlacklistManager blacklistManager,
            IPermissionChecker permissionChecker,
            IUserManager userManager,
            ILogger<OMItemRestrictions> logger)
        {
            _blacklistManager = blacklistManager;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
            _Logger = logger;
        }
        public async Task HandleEventAsync(object sender, UnturnedPlayerTakingItemEvent @event)
        {
            var pickupItem = @event.ItemData.item;
            var user = await _userManager.FindUserAsync(KnownActorTypes.Player, @event.Player.SteamId.ToString(), UserSearchMode.FindById);

            if (!_blacklistManager.IsItemBlacklisted(pickupItem.id, out var group))
            {
                return;
            }

            _Logger.LogDebug(group);
            _Logger.LogDebug(_permissionChecker.CheckPermissionAsync(user, $"blacklist.group.{group}").Result.ToString());

            if (await _permissionChecker.CheckPermissionAsync(user, $"blacklist.group.{group}") != PermissionGrantResult.Grant)
            {
                @event.IsCancelled = true;
                await @event.Player.PrintMessageAsync($"This item is blacklisted to group {group}", Color.Red);
                return;
            }
        }
    }
}
