using OMItemRestrictions.Services;
using OpenMod.API.Permissions;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OMItemRestrictions.Commands
{
    [Command("remove")]
    [CommandAlias("r")]
    [CommandParent(typeof(BlacklistCommand))]
    [CommandDescription("Stuff")]
    [CommandSyntax("<group> [item]")]
    public class BlacklistRemoveCommand : Command
    {
        private readonly IBlacklistManager _blacklistManager;
        public BlacklistRemoveCommand(
            IServiceProvider serviceProvider,
            IBlacklistManager blacklistManager) : base(serviceProvider)
        {
            _blacklistManager = blacklistManager;
        }

        protected override async Task OnExecuteAsync()
        {
            var uPlayer = Context.Actor as UnturnedUser;

            var group = await Context.Parameters.GetAsync<string>(0);
            var item = await Context.Parameters.GetAsync<int>(1);

            var error = _blacklistManager.RemoveBlacklist(group, item);

            if (error != null)
            {
                await uPlayer.PrintMessageAsync(error.ToString());
            }
        }
    }
}
