using Core.Services.Combat;
using Core.Services.Profiles;
using DAL.Models.Profiles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Math;
using DAL.Models.Mobs;
using Core.Services.Mobs;
using Core.Services.Items;
using DAL.Models.Items;
using Bot.Attribures;
using DiscordBot.Attributes.Enums;

namespace Bot.Commands
{
    public class DBCombatCommands : BaseCommandModule
    {
        private readonly ICombatService _combatService;
        private readonly IProfileService _profileService;
        private readonly IExperienceService _expService;
        private readonly IMobService _mobService;
        private readonly IItemService _itemService;
        public DBCombatCommands(ICombatService combatService, IProfileService profileService, IExperienceService expService, IMobService mobService, IItemService itemService)
        {
            _combatService = combatService;
            _profileService = profileService;
            _expService = expService;
            _mobService = mobService;
            _itemService = itemService;
        }

        //helper class for fighting to store information
        //since hp is readonly we have to store info about damage taken
        private class ProfileAndDamage
        {
            //profile and mob is set to null to set them manually
            //this allows use of this class in quest without much more added code
            public Profile profile = null;
            public DiscordMember member = null;
            public int DamageTaken=0;

            //set those only when fighting mob, this contains readonly things from profile and name to bypass problems with names
            public bool isMob = false;
            public string Name = String.Empty;
            public int HP = 0;
            public int BaseDMG = 0;
        }

        [Command("fight")]
        [Description("Become the master of arena!")]
        [CheckTime(TimeCheckMode.Fight, 600)]
        public async Task Fight(CommandContext ctx, DiscordMember mention=null)
        {
            if (mention == null || mention==ctx.Member)
            {
                await ctx.Channel.SendMessageAsync("Bruh");
                await ctx.Channel.SendMessageAsync($"{ctx.Member.DisplayName} wants to duel himself!");
                await ctx.Channel.SendMessageAsync("\"I'll take him off with that rope\" he thinks");
                await ctx.Channel.SendMessageAsync("He found a tree and hanged himself on a tree");
                await ctx.Channel.SendMessageAsync("What a shame !");
                return;
            }

            ProfileAndDamage attacker = new ProfileAndDamage { profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id), member=ctx.Member};
            ProfileAndDamage defender = new ProfileAndDamage { profile = await _profileService.GetOrCreateProfileAsync(mention.Id, mention.Guild.Id), member=mention };

            await ctx.Channel.SendMessageAsync($"{ctx.Member.DisplayName} will duel {mention.DisplayName} !");

            CombatInfo combatInfo;

            //since hp is readonly we have to store amount of damage dealt and compare it to the duelers HPs
            while(true)
            {
                combatInfo = _combatService.Attack(attacker.profile, defender.profile);
                switch (combatInfo.attackType)
                {
                    case AttackType.Dodge:
                        await ctx.Channel.SendMessageAsync($"{defender.member.DisplayName} dodged {attacker.member.DisplayName}'s attack !");
                        break;
                    case AttackType.Attack:
                        await ctx.Channel.SendMessageAsync($"{attacker.member.DisplayName} hits {defender.member.DisplayName} for {combatInfo.Damage} HP !");
                        break;
                    case AttackType.Critical:
                        await ctx.Channel.SendMessageAsync($"{attacker.member.DisplayName} critically stabbed {defender.member.DisplayName} knee for {combatInfo.Damage} HP !");
                        break;
                    case AttackType.Magick:
                        await ctx.Channel.SendMessageAsync($"{attacker.member.DisplayName} mumbled something stupid and casted a fireball on {defender.member.DisplayName} for {combatInfo.Damage} HP !");
                        break;
                    case AttackType.CriticalMagick:
                        await ctx.Channel.SendMessageAsync($"{attacker.member.DisplayName} got lucky and guessed powerful spell and threw it at {defender.member.DisplayName} for {combatInfo.Damage} HP !");
                        break;
                    default:
                        await ctx.Channel.SendMessageAsync("Combat Commands fight - undefined attack type");
                        return;
                }
                defender.DamageTaken += combatInfo.Damage;
                if(defender.DamageTaken>=defender.profile.HP)
                {
                    await ctx.Channel.SendMessageAsync($"{attacker.member.DisplayName} brutally finished off {defender.member.DisplayName} making him swim in pool of his own blood !");
                    int GoldAmount = BotMath.CalculateGoldAmount(defender.profile.Level, attacker.profile.Luck, attacker.profile.Level);
                    int XPAmount = BotMath.CalculateXPAmount(defender.profile.Level, attacker.profile.Luck, attacker.profile.Level);
                    await ctx.Channel.SendMessageAsync($"{attacker.member.DisplayName} was awarded with {GoldAmount} gold and {XPAmount} experience points !");

                    await _profileService.AddGold(attacker.profile.DiscordID, attacker.profile.GuildID, GoldAmount);
                    await _expService.GrantXPAsync(attacker.profile.DiscordID, attacker.profile.GuildID, XPAmount);
                    break;
                }

                //swap
                (attacker, defender) = (defender, attacker);
            }

        }

        //to make things easier we convert mob to profile
        private Profile ConvertMobToProfile(Mob mob, int level)
        {
            return new Profile { Strength = mob.Strength, Agility = mob.Agility, Intelligence = mob.Intelligence, Endurance = mob.Endurance, Luck = mob.Luck, Armor = mob.Resistance, Level=level};
        }

        [Command("quest")]
        [Description("Rip and tear, until it is done.")]
        [CheckTime(TimeCheckMode.Quest, 900)]
        public async Task Quest(CommandContext ctx)
        {
            Profile Profile = await _profileService.GetOrCreateProfileAsync(ctx.Member.Id, ctx.Guild.Id);
            //we set name to bypass problem with names in this function
            ProfileAndDamage attacker = new ProfileAndDamage { profile = Profile, Name = ctx.Member.DisplayName, HP=Profile.HP, BaseDMG = Profile.BaseDMG};

            Mob Mob = await _mobService.GetRandomMob();
            //mob should be on a player level or 1 level higher
            int mobLevel = BotMath.RandomNumberGenerator.Next(Profile.Level, Profile.Level + 1);
            Mob = _mobService.ScaleMob(Mob, mobLevel);

            Profile mobProfile = ConvertMobToProfile(Mob, mobLevel);
            ProfileAndDamage defender = new ProfileAndDamage { profile = mobProfile, Name = Mob.Name, HP = Mob.HP, BaseDMG = Mob.BaseDMG, isMob=true};

            await ctx.Channel.SendMessageAsync($"{defender.Name} is staring at {attacker.Name}! Treasure won't come as easy as you thought...");

            CombatInfo combatInfo;

            //since hp is readonly we have to store amount of damage dealt and compare it to the duelers HPs
            while (true)
            {
                combatInfo = _combatService.Attack(attacker.profile, defender.profile, attacker.BaseDMG);
                switch (combatInfo.attackType)
                {
                    case AttackType.Dodge:
                        await ctx.Channel.SendMessageAsync($"{defender.Name} dodged {attacker.Name}'s attack !");
                        break;
                    case AttackType.Attack:
                        await ctx.Channel.SendMessageAsync($"{attacker.Name} hits {defender.Name} for {combatInfo.Damage} HP !");
                        break;
                    case AttackType.Critical:
                        await ctx.Channel.SendMessageAsync($"{attacker.Name} critically stabbed {defender.Name} knee for {combatInfo.Damage} HP !");
                        break;
                    case AttackType.Magick:
                        await ctx.Channel.SendMessageAsync($"{attacker.Name} mumbled something stupid and casted a fireball on {defender.Name} for {combatInfo.Damage} HP !");
                        break;
                    case AttackType.CriticalMagick:
                        await ctx.Channel.SendMessageAsync($"{attacker.Name} got lucky and guessed powerful spell and threw it at {defender.Name} for {combatInfo.Damage} HP !");
                        break;
                    default:
                        await ctx.Channel.SendMessageAsync("Combat Commands fight - undefined attack type");
                        return;
                }
                defender.DamageTaken += combatInfo.Damage;
                if (defender.DamageTaken >= defender.HP)
                {
                    if(attacker.isMob)
                    {
                        await ctx.Channel.SendMessageAsync($"{attacker.Name} brutally chopped off {defender.Name}'s head and impaled it!");
                        await ctx.Channel.SendMessageAsync($"Your eyes are crying with red warm blood");
                        break;
                    }

                    await ctx.Channel.SendMessageAsync($"{attacker.Name} shredded {defender.Name} into little pieces! You cut the head of the beast and kick it like a ball");
                    int GoldAmount = BotMath.CalculateGoldAmount(Mob.GoldAward/5, attacker.profile.Luck, attacker.profile.Level);
                    int XPAmount = BotMath.CalculateXPAmount(Mob.XPAward/10, attacker.profile.Luck, attacker.profile.Level);
                    await ctx.Channel.SendMessageAsync($"{attacker.Name} was awarded with {GoldAmount} gold and {XPAmount} experience points !");

                    await _profileService.AddGold(attacker.profile.DiscordID, attacker.profile.GuildID, GoldAmount);
                    await _expService.GrantXPAsync(attacker.profile.DiscordID, attacker.profile.GuildID, XPAmount);
                    if (BotMath.Roll(0.25))
                    {
                        Item item =await _itemService.GetRandomItem();
                        await ctx.Channel.SendMessageAsync($"It looks like this monster was snitching something in its place! {item.Name} was added to your inventory!");
                        await _itemService.AddItemAsync(ctx.Member.Id, ctx.Guild.Id, item);
                    }

                    break;
                }

                //swap
                (attacker, defender) = (defender, attacker);
            }

        }

    }
}
