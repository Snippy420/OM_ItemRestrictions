using Microsoft.Extensions.Logging;
using OMItemRestrictions.Services;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Unturned.Players.Crafting.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace OMItemRestrictions.Events
{
    public class UnturnedPlayerCraftingListener : IEventListener<UnturnedPlayerCraftingEvent>
    {
        private readonly IBlacklistManager _blacklistManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IUserManager _userManager;
        private readonly ILogger<OMItemRestrictions> _Logger;
        public UnturnedPlayerCraftingListener(
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
        public async Task HandleEventAsync(object sender, UnturnedPlayerCraftingEvent @event)
        {
            var craftItem = @event.ItemId;
            var user = await _userManager.FindUserAsync(KnownActorTypes.Player, @event.Player.SteamId.ToString(), UserSearchMode.FindById);

            if (!_blacklistManager.IsItemBlacklisted(craftItem, out var group))
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
