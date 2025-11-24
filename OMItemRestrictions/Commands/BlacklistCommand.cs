using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace OMItemRestrictions.Commands
{
    [Command("blacklist")]
    [CommandAlias("bl")]
    [CommandDescription("Adds specified item to the specified blacklist")]
    [CommandSyntax("<group> <item>")]
    public class BlacklistCommand : Command
    {
        private readonly IBlacklistManager _blacklistManager;
        public BlacklistCommand(
            IServiceProvider serviceProvider, 
            IBlacklistManager blacklistManager) : base(serviceProvider)
        {
            _blacklistManager = blacklistManager;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length != 2)
                throw new CommandWrongUsageException(Context);

            var group = await Context.Parameters.GetAsync<string>(0);
            var item = await Context.Parameters.GetAsync<int>(1);

            await _blacklistManager.AddBlacklist(group, item);
            await PrintAsync($"Added ID {item} to the group {group.ToUpper()}");
        }
    }
}
