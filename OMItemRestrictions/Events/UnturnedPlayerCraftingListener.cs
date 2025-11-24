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
using Microsoft.Extensions.Localization;

namespace OMItemRestrictions.Events
{
    public class UnturnedPlayerCraftingListener : IEventListener<UnturnedPlayerCraftingEvent>
    {
        private readonly IBlacklistManager _blacklistManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IUserManager _userManager;
        private readonly ILogger<OMItemRestrictions> _logger;
        private readonly IStringLocalizer _localizer;
        
        public UnturnedPlayerCraftingListener(
            IBlacklistManager blacklistManager,
            IPermissionChecker permissionChecker,
            IUserManager userManager,
            ILogger<OMItemRestrictions> logger, 
            IStringLocalizer localizer)
        {
            _blacklistManager = blacklistManager;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
            _logger = logger;
            _localizer = localizer;
        }
        public async Task HandleEventAsync(object sender, UnturnedPlayerCraftingEvent @event)
        {
            var craftItem = @event.ItemId;
            var user = await _userManager.FindUserAsync(KnownActorTypes.Player, @event.Player.SteamId.ToString(), UserSearchMode.FindById);

            if (!_blacklistManager.IsItemBlacklisted(craftItem, out var group)) return;

            _logger.LogDebug(group);
            _logger.LogDebug(_permissionChecker.CheckPermissionAsync(user, $"blacklist.group.{group}").Result.ToString());

            if (await _permissionChecker.CheckPermissionAsync(user, $"blacklist.group.{group}") ==
                PermissionGrantResult.Grant) return;
            
            @event.IsCancelled = true;
            await @event.Player.PrintMessageAsync(_localizer["ItemBlacklisted", new { Group = group }], Color.Red);
        }
    }
}
