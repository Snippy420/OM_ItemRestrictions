using OMItemRestrictions.Services;
using OpenMod.API.Commands;
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
    [CommandDescription("Removes an item from the selected groups blacklist")]
    [CommandSyntax("<group> <item>")]
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
            if (Context.Parameters.Length != 2)
                throw new CommandWrongUsageException(Context);
            
            var group = await Context.Parameters.GetAsync<string>(0);
            var item = await Context.Parameters.GetAsync<int>(1);

            await _blacklistManager.RemoveBlacklist(group, item);

            await PrintAsync($"Removed ID {item} from group {group.ToUpper()}");
        }
    }
}
