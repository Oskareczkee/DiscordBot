//this file will help managing level ups in commands
//and its not used XD

/*
using Core.Services.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Commands.Helpers
{
    static class LevelUpHelpers
    {
        //shows level up embed if given model has levelled up
        private static async Task ShowLevelUpEmbed(CommandContext ctx, GrantXPVievModel model)
        {
            if(model.LevelledUp)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"{ctx.Member.DisplayName} has just levelled up to level {model.Profile.Level} !",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl },
                    Color = DiscordColor.CornflowerBlue
                };

                embed.AddField("XP:", $"{model.Profile.XP}");
                embed.AddField("Next Level:", $"{model.Profile.NextLevel}");
                await ctx.Channel.SendMessageAsync(embed: embed);
            }
        }

        /// <summary>
        /// Adds experience to the member (user of the command) and shows embed if he levelled up
        /// Embed showing can be disabled by setting showEmbed to false
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="expService"></param>
        /// <param name="amount"></param>
        /// <param name="showEmbed"></param>
        /// <returns></returns>
        public static async Task AddExperience(CommandContext ctx, IExperienceService expService, int amount, bool showEmbed=true)
        {
            var vievModel = await expService.GrantXPAsync(ctx.Member.Id, ctx.Guild.Id, amount);
            if(showEmbed)
                await LevelUpHelpers.ShowLevelUpEmbed(ctx, vievModel);
        }

    }
}
 */ 
