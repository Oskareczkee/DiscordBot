using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Handlers.Dialogue.Steps
{
    public class VoidStep : DialogueStepBase
    {

        public VoidStep(string content) : base(content)
        { }

        public override IDialogueStep NextStep => null;

        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var cancelEmoji = DiscordEmoji.FromName(client, ":x:");

            var embedBuilder = new DiscordEmbedBuilder
            {
                Description = $"{user.Mention}, {_content}",
                Color = DiscordColor.HotPink
            };

            await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);

            return false;
         }
    }
}
