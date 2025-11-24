using OMItemRestrictions.Services;
using OpenMod.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OMItemRestrictions.Commands
{
    [Command("groups")]
    [CommandAlias("g")]
    [CommandParent(typeof(BlacklistCommand))]
    [CommandDescription("Lists all blacklist groups")]
    public class BlacklistGroupsCommand : Command
    {
        private readonly IBlacklistManager _blacklistManager;
        public BlacklistGroupsCommand(
            IServiceProvider serviceProvider,
            IBlacklistManager blacklistManager) : base(serviceProvider)
        {
            _blacklistManager = blacklistManager;
        }

        protected async override Task OnExecuteAsync()
        {
            var groups = await _blacklistManager.BlacklistGroups();

            if (groups.Count == 0) return;

            var groupsMessage = string.Join(", ", groups);
            await PrintAsync(groupsMessage);

        }
    }
}
