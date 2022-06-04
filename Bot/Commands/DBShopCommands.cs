using Core.Services.Items;
using DAL;
using Bot;
using DAL.Models.Items;
using DiscordBot.Handlers.Dialogue;
using DiscordBot.Handlers.Dialogue.Steps;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Commands
{
    public class DBShopCommands : BaseCommandModule
    {
        //the database
        private readonly IItemService _itemService;

        public DBShopCommands(IItemService ItemService)
        {
            _itemService = ItemService;
        }


        [Command("additem")]
        [RequireOwner]
        public async Task AddItem(CommandContext ctx)
        {
            Item item = new Item();
            string statsString = string.Empty;

            var itemStatsStep = new TextStep(@"Enter the item stats:
                                               Strength Agility Intelligence Endurance Luck Armor
                                               Example: 0 0 5 5 5 10"
                                               ,null);


            var itemPriceStep = new IntStep("Enter the item price", itemStatsStep);
            var itemDescriptionStep = new TextStep("Enter the item description", itemPriceStep);
            var itemNameStep = new TextStep("Enter the item name", itemDescriptionStep);

            var itemTypeStep = new IntStep(
                @"Please Enter the item type:
                None = 0 (not used)
                Helmet = 1,
                Chestplate =2,
                Gloves =3,
                Shoes =4,
                Weapon=5,
                Ring=6,
                Belt=7,
                Necklace=8,
                Extra=9,
                Potion=10,
                Miscellaneous=11", 
                itemNameStep, 1, 11);

            itemNameStep.OnValidResult += (result) => item.Name = result;
            itemDescriptionStep.OnValidResult += (result) => item.Description = result;
            itemPriceStep.OnValidResult += (result) => item.Price = result;
            itemTypeStep.OnValidResult += (result) => item.Type = (ItemType)result;
            itemStatsStep.OnValidResult += (result) => statsString = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                itemTypeStep
                );

            bool succeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if(!succeded)
            {
                await ctx.Channel.SendMessageAsync("Something went wrong with the additem dialogue, Sorry Pal!");
                return;
            }

            string[] statsSplit = statsString.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if(statsSplit.Length!=6)
            {
                await ctx.Channel.SendMessageAsync("Additem: wrong stats string (splitted string is too long or too short)").ConfigureAwait(false);
                return;
            }

            List<int> StatsInt = new List<int>();

            foreach(string stat in statsSplit)
            {
                int value = 0;
                if (int.TryParse(stat, out value))
                    StatsInt.Add(value);
                else
                {
                    await ctx.Channel.SendMessageAsync("Additem: stats could not be parsed to an integer type, please try to add item once more").ConfigureAwait(false);
                    return;
                }
            }

            item.Strength = StatsInt[0];
            item.Agility = StatsInt[1];
            item.Intelligence = StatsInt[2];
            item.Endurance = StatsInt[3];
            item.Luck = StatsInt[4];
            item.Armor = StatsInt[5];


            await _itemService.CreateNewItemAsync(item);
            await ctx.Channel.SendMessageAsync($"Item {item.Name} has been successfully added!").ConfigureAwait(false);
        }

        [Command("itemlist")]
        public async Task ItemList(CommandContext ctx)
        {
            var items = await _itemService.GetItemsListAsync().ConfigureAwait(false);

            var embed = new DiscordEmbedBuilder
            {
                Title = "All available items",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = DiscordBot.Bot.Configuration.mamonPhotoURL}, 
                Color = DiscordColor.HotPink
            };

            var itemNames = new List<string>();

            //if there are no items implemented yet
            if (items.Count==0)
            {
                embed.AddField("There are no items yet!", "Sorry");
                await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                return;
            }

            foreach(var item in items)
                itemNames.Add(item.Name);

            embed.AddField("Items: ", string.Join(", ", itemNames));

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
         

        [Command ("getitem")]
        public async Task GetItem(CommandContext ctx)
        {
            var itemStep = new TextStep("What item are you looking for?", null);

            string itemName = string.Empty;

            itemStep.OnValidResult += (result) => itemName = result;

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                itemStep
                );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if(!succeeded)
            {
                await ctx.Channel.SendMessageAsync("Something went wrong with the getitem dialogue, Sorry Pal!");
                return;
            }

            var item = await _itemService.GetItemByName(itemName).ConfigureAwait(false);

            if(item==null)
            {
                await ctx.Channel.SendMessageAsync($"item: \"{itemName}\" does not exist ");
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = item.Name,
                Description = item.Description,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = DiscordBot.Bot.Configuration.mamonPhotoURL },
                Color = DiscordColor.PhthaloBlue
            };

            embed.AddField("ID: ", item.ID.ToString(), true);
            embed.AddField("Type: ", item.Type.ToString(), true);
            embed.AddField("Price: ", item.Price.ToString(), true);
            embed.AddField("Armor: ", item.Armor.ToString(), true);
            embed.AddField("Strength: ", item.Strength.ToString(), true);
            embed.AddField("Agility: ", item.Agility.ToString(), true);
            embed.AddField("Intelligence: ", item.Intelligence.ToString(), true);
            embed.AddField("Endurance: ", item.Endurance.ToString(), true);
            embed.AddField("Luck: ", item.Luck.ToString(), true);

            await ctx.Channel.SendMessageAsync(embed: embed);
            return;
        }

        [Command("buy")]
        public async Task BuyItem(CommandContext ctx, params string[] itemName)
        {
            string fullName = string.Join(' ', itemName);

            var purchaseState = await _itemService.PurchaseItemAsync(ctx.Member.Id, ctx.Guild.Id, fullName);

            switch (purchaseState)
            {
                case ItemPurchaseState.ItemNotFound:
                    await ctx.Channel.SendMessageAsync($"Item {fullName} has not been found in shop!");
                    break;
                case ItemPurchaseState.NotEnoughGold:
                    await ctx.Channel.SendMessageAsync($"You don't have enough gold to buy {fullName}!");
                    break;
                case ItemPurchaseState.EverythingWentGood:
                    await ctx.Channel.SendMessageAsync($"item {fullName} has been successfully bought!");
                    break;
                default:
                    await ctx.Channel.SendMessageAsync("Something wrong happend in BuyItem function, please contact owner of this bot");
                    break;
            }
        }
    }
}
