using Core.Services.Profiles;
using DAL.Models.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Bot.Commands.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Attribures;
using DiscordBot.Handlers.Dialogue.Steps;
using DiscordBot.Handlers.Dialogue;
using DAL.Models.Items;

namespace Bot.Commands.ProfileManagment
{
    public partial class DBProfileCommands : BaseCommandModule
    {

        [Command("addskill")]
        [Description("Allows to add skills to the profile")]
        public async Task AddSkill(CommandContext ctx)
        {
            ulong price = 0;

            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);        
            var skillAmountStep = new IntStep("How much levels do you want to buy", null,1,100);

            var skillStep = new IntStep(
                $@"Which skill do you want to level up:
                Strength = 1 (Cost: {BotMath.SkillLevelUpCost(profile.Strength)})
                Agility = 2 (Cost: {BotMath.SkillLevelUpCost(profile.Agility)})
                Intelligence = 3 (Cost: {BotMath.SkillLevelUpCost(profile.Intelligence)})
                Endurance = 4 (Cost: {BotMath.SkillLevelUpCost(profile.Endurance)})
                Luck = 5 (Cost: {BotMath.SkillLevelUpCost(profile.Luck)})",
                skillAmountStep, 1, 5);

            SkillType skillType = SkillType.None;
            int skillAmount = 0;
            DiscordEmoji confirmationEmoji = null;

            skillStep.OnValidResult += (result) => skillType = (SkillType)result;
            skillAmountStep.OnValidResult += (result) => skillAmount = result;
            

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                skillStep
                );

            bool succeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);
            if(!succeded)
            {
                await ctx.Channel.SendMessageAsync("Something went wrong with AddSkill dialogue, Sorry").ConfigureAwait(false);
                return;
            }

            price = BotMath.calculatePrice(skillType, profile, skillAmount);

            var confirmationStep = new ReactionStep(
                $@"This will cost you {price} gold
                   You have: {profile.Gold} gold
                   React with {DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")} to confirm or {DiscordEmoji.FromName(ctx.Client, ":x:")} to cancel operation",
                new System.Collections.Generic.Dictionary<DiscordEmoji, ReactionStepData>
            {
                {DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"), new ReactionStepData{ Content = "", nextStep = null} },
             });

            confirmationStep.OnValidResult += (result) => confirmationEmoji = result;

            var confirmationDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                confirmationStep
                );

            await confirmationDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if(confirmationEmoji == DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"))
            {
                if(profile.Gold < price)
                {
                    await ctx.Channel.SendMessageAsync("You do not have enough gold").ConfigureAwait(false);
                    return;
                }

               await _profileService.LevelUpSkill(profile.DiscordID, profile.GuildID, skillType, skillAmount, price);
               await ctx.Channel.SendMessageAsync("Skill has been succesfully levelled up").ConfigureAwait(false);
            }

        }
    }
}
