using Core.Services;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Commands.Helpers
{
#if DATABASE_CLEAR
    public class ClearDatabase :BaseCommandModule
    {
        private readonly IDataBaseClearService _clearingService;

        public ClearDatabase(IDataBaseClearService clearingService)
        {
            _clearingService = clearingService;
        }

        [Command("cleardatabase")]
        [RequireOwner]
        public async Task ClearDataBase(CommandContext ctx)
        {
            await _clearingService.ClearDataBase().ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync("Database has been successfully cleared out of data").ConfigureAwait(false);
        }
    }
#endif
}
