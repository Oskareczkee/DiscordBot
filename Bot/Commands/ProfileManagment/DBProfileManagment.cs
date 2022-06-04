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
                Enudrance = 4 (Cost: {BotMath.SkillLevelUpCost(profile.Endurance)})
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

        [Command("equip")]
        public async Task EqiupItem(CommandContext ctx)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);

            ItemType itemType = ItemType.None;

            var itemTypeStep = new IntStep(
            @"What item do you want to change:
                Helmet = 1,
                Chestplate =2,
                Gloves =3,
                Shoes =4,
                Weapon=5,
                Ring=6,
                Belt=7,
                Necklace=8,
                Extra=9",
                null, 1, 9);

            itemTypeStep.OnValidResult += (result) => itemType = (ItemType)result;

            var typeDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                itemTypeStep
                );

            bool succeded = await typeDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeded)
                return;

            List<ProfileItem> availableItems = (from item in profile.Items where item.Item.Type == itemType select item).ToList();

            if(availableItems.Count==0)
            {
                await ctx.Channel.SendMessageAsync($"It looks like you don't have any item of type {itemType.ToString()} in your inventory. What a shame!");
                return;
            }

            var itemEmbed = new DiscordEmbedBuilder
            {
                ImageUrl = DiscordBot.Bot.Configuration.mamonPhotoURL,
                Title = "Inventory",
                Description = $"All items of type {itemType.ToString()} in your inventory",
            };


            //gets all item names for embed and its statistics and puts it into a string
            string[] itemNames = (from item in availableItems
                                  select item.Item.Name + " {Armor:" + item.Item.Armor
                                  + " S:" + item.Item.Strength.ToString()
                                  + " A:" + item.Item.Agility.ToString()
                                  + " I:" + item.Item.Intelligence
                                  + " E:" + item.Item.Endurance.ToString()
                                  + " L:" + item.Item.Luck.ToString() + "}").ToArray();

            //adding counters for items (I could propably do this in query, but that seemed to be problematic, this is just simpler approach)
            //Insert operation on string might be ineffective 
            //Good way to do this would be propably do the strings with counters and then concat to them string from query
            for(int x =1; x<=itemNames.Length; x++)
                itemNames[x - 1] = itemNames[x - 1].Insert(0, x.ToString() + ". ");

            itemEmbed.AddField("Items: ", string.Join("\n", itemNames));

            await ctx.Channel.SendMessageAsync(embed: itemEmbed);


            int choice = 0;
            var chooseItemStep = new IntStep("Please choose index of item you want to equip", null, 1, itemNames.Length);
            chooseItemStep.OnValidResult = (result) => choice = result;

            var chooseDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                chooseItemStep
                );

            succeded = await chooseDialogueHandler.ProcessDialogue().ConfigureAwait(false);
            if (!succeded)
                return;

            bool itemEquipped = await _itemService.EquipItemAsync(profile.DiscordID, profile.GuildID, availableItems[choice - 1]);

            if (!itemEquipped)
                await ctx.Channel.SendMessageAsync("EquipItem: item could not be equipped").ConfigureAwait(false);
            else
                await ctx.Channel.SendMessageAsync("Item has been succesfully equipped").ConfigureAwait(false);
        }

    }
}
