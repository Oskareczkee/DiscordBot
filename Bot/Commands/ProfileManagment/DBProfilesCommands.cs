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
using Core.Services.Items;

namespace Bot.Commands.ProfileManagment
{
    public partial class DBProfileCommands : BaseCommandModule
    {
        //the database
        private readonly IProfileService _profileService;
        private readonly IItemService _itemService;
        private readonly IExperienceService _expService;

        public DBProfileCommands(IProfileService ProfileService, IExperienceService expService, IItemService itemService)
        {
            _profileService = ProfileService;
            _expService = expService;
            _itemService = itemService;
        }

        [Command("profile"), AddXP(10)]
        public async Task GetProfile(CommandContext ctx, DiscordMember member=null)
        {
            if (member == null)
                await DisplayProfileAsync(ctx, ctx.Member.Id).ConfigureAwait(false);
            else
                await DisplayProfileAsync(ctx, member.Id).ConfigureAwait(false);
        }

        [Command("equipment"), AddXP(10)]
        public async Task GetEquipment(CommandContext ctx, DiscordMember member = null)
        {
            if (member == null)
                await DisplayEquipmentAsync(ctx, ctx.Member.Id).ConfigureAwait(false);
            else
                await DisplayEquipmentAsync(ctx, member.Id).ConfigureAwait(false);
        }

        private async Task DisplayEquipmentAsync(CommandContext ctx, ulong memberID)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberID, ctx.Guild.Id).ConfigureAwait(false);

            DiscordMember member = ctx.Guild.Members[profile.DiscordID];


            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{ctx.Guild.Members[profile.DiscordID].DisplayName}'s equipment",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Color = DiscordColor.Blue
            };
            //profile.Equipment.Add(null);
            //profile.Equipment = Enumerable.Repeat(new EquipmentItem { ProfileID = profile.ID, Item = null }, 9).ToList();

            foreach (var item in profile.Equipment)
            {
                if (item.Item != null)
                    profileEmbed.AddField(item.Item.Type.ToString(), item.Item.Name, true);
                else
                    profileEmbed.AddField("aa", "aa", true);
            }
     

            /*
            if(profile.Equipment[0]!=null)
                profileEmbed.AddField("Helmet:", profile.Equipment[0].Item.Name, true);
            else
                profileEmbed.AddField("Helmet:", "None", true);

            if (profile.Equipment[1] != null)
                profileEmbed.AddField("Chestplate:", profile.Equipment[1].Item.Name, true);
            else
                profileEmbed.AddField("Chestplate:", "None", true);

            if (profile.Equipment[2] != null)
                profileEmbed.AddField("Gloves:", profile.Equipment[2].Item.Name, true);
            else
                profileEmbed.AddField("Gloves:", "None", true);

            if (profile.Equipment[3] != null)
                profileEmbed.AddField("Shoes:", profile.Equipment[3].Item.Name, true);
            else
                profileEmbed.AddField("Shoes:", "None", true);

            if (profile.Equipment[4] != null)
                profileEmbed.AddField("Weapon:", profile.Equipment[4].Item.Name, true);
            else
                profileEmbed.AddField("Weapon:", "None", true);

            if (profile.Equipment[5] != null)
                profileEmbed.AddField("Ring:", profile.Equipment[5].Item.Name, true);
            else
                profileEmbed.AddField("Ring:", "None", true);

            if (profile.Equipment[6] != null)
                profileEmbed.AddField("Belt:", profile.Equipment[6].Item.Name, true);
            else
                profileEmbed.AddField("Belt:", "None", true);

            if (profile.Equipment[7] != null)
                profileEmbed.AddField("Necklace:", profile.Equipment[7].Item.Name, true);
            else
                profileEmbed.AddField("Necklace:", "None", true);

            if (profile.Equipment[8] != null)
                profileEmbed.AddField("Extra:", profile.Equipment[8].Item.Name, true);
            else
                profileEmbed.AddField("Extra:", "None", true);
             */


            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }

        private async Task DisplayProfileAsync(CommandContext ctx, ulong memberID)
        {

            Profile profile = await _profileService.GetOrCreateProfileAsync(memberID, ctx.Guild.Id).ConfigureAwait(false);

            DiscordMember member = ctx.Guild.Members[profile.DiscordID];


            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{ctx.Guild.Members[profile.DiscordID].DisplayName}'s profile",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Color = DiscordColor.Aquamarine
            };

            profileEmbed.AddField("XP:", profile.XP.ToString(), true);
            profileEmbed.AddField("Level:", profile.Level.ToString(), true);
            profileEmbed.AddField("Next Level:", profile.NextLevel.ToString(), true);
            profileEmbed.AddField("Gold:", profile.Gold.ToString());
            profileEmbed.AddField("HP:", profile.HP.ToString());
            profileEmbed.AddField("Strength: ", profile.Strength.ToString(), true);
            profileEmbed.AddField("Agility: ", profile.Agility.ToString(), true);
            profileEmbed.AddField("Intelligence: ", profile.Intelligence.ToString(), true);
            profileEmbed.AddField("Endurance: ", profile.Endurance.ToString(), true);
            profileEmbed.AddField("Luck: ", profile.Luck.ToString(), true);
            profileEmbed.AddField("Armor:", profile.Armor.ToString(), true);

            if (profile.Items.Count > 0)
                profileEmbed.AddField("Items:", string.Join(", ", profile.Items.Select(x => x.Item.Name)));
            else
                profileEmbed.AddField("Items:", "None");


            await ctx.Channel.SendMessageAsync(embed: profileEmbed).ConfigureAwait(false);
        }
    }
}
