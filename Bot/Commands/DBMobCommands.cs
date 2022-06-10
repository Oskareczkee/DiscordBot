using Core.Services.Mobs;
using DAL.Models.Items;
using DAL.Models.Mobs;
using DiscordBot.Handlers.Dialogue;
using DiscordBot.Handlers.Dialogue.Steps;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Commands
{

    public class DBMobCommands: BaseCommandModule
    {
        private readonly IMobService _mobService;

        public DBMobCommands(IMobService mobService)
        {
            _mobService = mobService;
        }


        [Command("addmob")]
        [RequireRoles(RoleCheckMode.Any, "catboys paradise")]
        //[RequireOwner]
        public async Task AddMob(CommandContext ctx)
        {
            Mob mob = new Mob();
            string statsString = string.Empty;

            var mobXPStep = new IntStep("Enter base XP reward for mob", null);
            var mobGoldStep = new IntStep("Enter base gold reward for mob", mobXPStep);
            var mobResistanceStep = new IntStep("Enter mob's resistance to attacks", mobGoldStep, 0, 75);

            var mobStatsStep = new TextStep(@"Enter the mob base stats:
                                               Strength Agility Intelligence Endurance Luck HP BaseDMG
                                               Example: 1 1 5 5 5 25 2"
                                               , mobResistanceStep);


            var mobDescriptionStep = new TextStep("Enter the mob description or lore", mobStatsStep);
            var mobNameStep = new TextStep("Enter the mob name", mobDescriptionStep);


            mobNameStep.OnValidResult += (result) => mob.Name = result;
            mobDescriptionStep.OnValidResult += (result) => mob.Description = result;
            mobStatsStep.OnValidResult += (result) => statsString = result;
            mobGoldStep.OnValidResult += (result) => mob.GoldAward = result;
            mobXPStep.OnValidResult += (result) => mob.XPAward = result;
            mobResistanceStep.OnValidResult += (result) => mob.Resistance = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                mobNameStep
                );

            bool succeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeded)
            {
                await ctx.Channel.SendMessageAsync("Something went wrong with the AddMob dialogue, Sorry Pal!");
                return;
            }

            string[] statsSplit = statsString.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (statsSplit.Length != 7)
            {
                await ctx.Channel.SendMessageAsync("Addmob: wrong stats string (splitted string is too long or too short)").ConfigureAwait(false);
                return;
            }

            List<int> StatsInt = new List<int>();

            foreach (string stat in statsSplit)
            {
                int value = 0;
                if (int.TryParse(stat, out value))
                    StatsInt.Add(value);
                else
                {
                    await ctx.Channel.SendMessageAsync("AddMob: stats could not be parsed to an integer type, please try to add mob once more").ConfigureAwait(false);
                    return;
                }
            }

            mob.Strength = StatsInt[0];
            mob.Agility = StatsInt[1];
            mob.Intelligence = StatsInt[2];
            mob.Endurance = StatsInt[3];
            mob.Luck = StatsInt[4];
            mob.HP = StatsInt[5];
            mob.BaseDMG = StatsInt[6];


            await _mobService.CreateMob(mob);
            await ctx.Channel.SendMessageAsync($"Mob {mob.Name} has been successfully added!").ConfigureAwait(false);
        }

        [Command("getmob")]
        [Description("Shows mob and all its stats")]
        public async Task GetMob(CommandContext ctx)
        {
            var mobStep = new TextStep("What mob are you looking for?", null);

            string mobName = string.Empty;

            mobStep.OnValidResult += (result) => mobName = result;

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                mobStep
                );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded)
                return;

            var mob = await _mobService.GetMobByName(mobName).ConfigureAwait(false);

            if (mob == null)
            {
                await ctx.Channel.SendMessageAsync($"item: \"{mobName}\" does not exist ");
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = mob.Name,
                Description = mob.Description,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = DiscordBot.Bot.Configuration.mamonPhotoURL },
                Color = DiscordColor.PhthaloBlue
            };

            embed.AddField("ID: ", mob.ID.ToString());
            embed.AddField("Base XP Award:", mob.XPAward.ToString(), true);
            embed.AddField("Base Gold Award: ", mob.GoldAward.ToString(), true);
            embed.AddField("Base HP: ", mob.HP.ToString(), true);
            embed.AddField("Base DMG: ", mob.BaseDMG.ToString(), true);
            embed.AddField("Base Resistance: ", mob.Resistance.ToString() + "%", true);
            embed.AddField("Base Strength: ", mob.Strength.ToString(), true);
            embed.AddField("Base Agility: ", mob.Agility.ToString(), true);
            embed.AddField("Base Intelligence: ", mob.Intelligence.ToString(), true);
            embed.AddField("Base Endurance: ", mob.Endurance.ToString(), true);
            embed.AddField("Base Luck: ", mob.Luck.ToString(), true);

            await ctx.Channel.SendMessageAsync(embed: embed);
            return;
        }

    }
}
