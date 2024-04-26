﻿using OMItemRestrictions.Models;
using OMItemRestrictions.Services;
using OpenMod.API.Commands;
using OpenMod.API.Persistence;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OMItemRestrictions.Commands
{
    [Command("blacklist")]
    [CommandAlias("bl")]
    [CommandDescription("Stuff")]
    [CommandSyntax("<group> [item]")]
    public class BlacklistCommand : Command
    {
        private readonly IBlacklistManager _blacklistManager;
        public BlacklistCommand(IServiceProvider serviceProvider, IBlacklistManager blacklistManager) : base(serviceProvider)
        {
            _blacklistManager = blacklistManager;
        }

        protected override async Task OnExecuteAsync()
        {
            var uPlayer = Context.Actor as UnturnedUser;

            var group = await Context.Parameters.GetAsync<string>(0);
            var item = await Context.Parameters.GetAsync<int>(1);

            _blacklistManager.AddBlacklist(group, item);
        }
    }
}