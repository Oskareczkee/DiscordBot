//this attribute checks if player has waited enough before getting new quest or dueling player

using Core.Services.Profiles;
using DAL.Models.Profiles;
using DiscordBot.Attributes.Enums;
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

    //checks if player waited specified time till action, action modes are defined in enums as TimeCheckMode
    public class CheckTime : CheckBaseAttribute
    {
        public readonly IProfileService _profileService;
        public TimeCheckMode checkMode { get; }
        uint _secondToWait { get; }

        //attribute does accept only simple types, thats why we get seconds as uint 
        public CheckTime(TimeCheckMode timeCheckMode, uint secondsToWait)
        {
            _profileService = (IProfileService)DiscordBot.Bot.Services.GetService(typeof(IProfileService));
            checkMode = timeCheckMode;
            _secondToWait = secondsToWait;
        }

        public async override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            switch (checkMode)
            {
                case TimeCheckMode.Fight:
                    {
                        if (profile.nextFightTime > DateTime.Now)
                        {
                            await ShowErrorEmbed(ctx, checkMode, profile.nextFightTime);
                            return false;
                        }
                        profile.lastFightTime = DateTime.Now;
                        profile.nextFightTime = profile.lastFightTime.AddSeconds(_secondToWait);
                        await _profileService.updateProfile(profile);
                        return true;
                    }
                case TimeCheckMode.Quest:
                    {
                        if (profile.nextQuestTime > DateTime.Now)
                        {
                            await ShowErrorEmbed(ctx, checkMode, profile.nextQuestTime);
                            return false;
                        }
                        profile.lastQuestTime = DateTime.Now;
                        profile.nextQuestTime = profile.lastQuestTime.AddSeconds(_secondToWait);
                        await _profileService.updateProfile(profile);
                        return true;
                    }
                default:
                    break;
            }

            return true;
        }

        //I Didn't know how to call it
        private async Task ShowErrorEmbed(CommandContext ctx, TimeCheckMode mode, DateTime time)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"You have to wait a bit till next {mode.ToString()}",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = DiscordBot.Bot.Configuration.mamonPhotoURL },
            };
            embed.AddField($"You can start next {mode.ToString()} after:", $"{time}");
            await ctx.Channel.SendMessageAsync(embed: embed);
        }
    }
}
