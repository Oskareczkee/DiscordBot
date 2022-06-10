using Core.Services.Profiles;
using DAL.Models.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Mvc;
using System;


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Attribures
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AddXP : CheckBaseAttribute
    {
        public int _xpToAdd { get; private set; }
        public readonly IExperienceService _expService;

        public AddXP(int xpToAdd)
        {
            _xpToAdd = xpToAdd;
            _expService = (IExperienceService) DiscordBot.Bot.Services.GetService(typeof(IExperienceService));
        }

        public async override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            var vievModel = await _expService.GrantXPAsync(ctx.Member.Id, ctx.Guild.Id, _xpToAdd);

            if(vievModel.LevelledUp==true)
                await ShowLevelUpEmbed(ctx, vievModel);

            return true;       
        }

        private async Task ShowLevelUpEmbed(CommandContext ctx, GrantXPVievModel model)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{ctx.Member.DisplayName} has just levelled up to level {model.Profile.Level} !",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl },
            };

            embed.AddField("XP:", $"{model.Profile.XP}", true);
            embed.AddField("Next Level:", $"{model.Profile.NextLevel}", true);
            await ctx.Channel.SendMessageAsync(embed: embed);
        }
    }
}
