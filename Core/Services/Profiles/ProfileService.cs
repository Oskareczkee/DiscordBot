using Core.Services.Items;
using DAL;
using DAL.Models.Items;
using DAL.Models.Profiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Profiles
{
    public interface IProfileService
    {
        /// <summary>
        /// Gets discord profile if exists in database, otherwise creates new one and returns it
        /// </summary>
        /// <param name="discordID"></param>
        /// <param name="guildID"></param>
        /// <returns>Found or created profile</returns>
        Task<Profile> GetOrCreateProfileAsync(ulong discordID, ulong guildID);

        /// <summary>
        /// Uses item if player has it in his inventory
        /// </summary>
        /// <param name="discordID"></param>
        /// <param name="guildID"></param>
        /// <param name="itemName"></param>
        /// <returns>true if item was found, false if could'nt find item in inventory</returns>
        Task<bool> UseItemAsync(ulong discordID, ulong guildID, Item item);

        /// <summary>
        /// levels up profile skill by amount
        /// </summary>
        /// <param name="discordID"></param>
        /// <param name="guildID"></param>
        /// <param name="skill"></param>
        /// <param name="amount"></param>
        /// <returns>true if everything went ok, false if skill type was none or undefined</returns>
        Task<bool> LevelUpSkill(ulong discordID, ulong guildID, SkillType skill, int amount, ulong goldPrice);
        Task AddGold(ulong discordID, ulong guildID, int amount);

        Task updateProfile(Profile profile);

    }

    public class ProfileService: IProfileService
    {
        private readonly DbContextOptions<Context> _options;
        public ProfileService(DbContextOptions<Context> options)
        {
            _options = options;
        }

        public async Task AddGold(ulong discordID, ulong guildID, int amount)
        {
            using var _context = new Context(_options);

            Profile profile = await GetOrCreateProfileAsync(discordID, guildID);

            profile.Gold += amount;

            _context.Profiles.Update(profile);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Profile> GetOrCreateProfileAsync(ulong discordID, ulong guildID)
        {
            using var _context = new Context(_options);

            Profile profile = await _context.Profiles.Where(x => x.GuildID == guildID)
                    .Include(x => x.Items)
                    .Include(x => x.Items).ThenInclude(x => x.Item)
                    .FirstOrDefaultAsync(x => x.DiscordID == discordID)
                    .ConfigureAwait(false);

            if (profile != null)
                return profile;

            profile = new Profile
            {
                DiscordID = discordID,
                GuildID = guildID,
                lastFightTime = DateTime.Now,
                lastQuestTime=DateTime.Now,
                nextFightTime=DateTime.Now,
                nextQuestTime=DateTime.Now
            };

            _context.Profiles.Update(profile);
            await _context.SaveChangesAsync().ConfigureAwait(false);



            return profile;
        }

        public async Task<bool> LevelUpSkill(ulong discordID, ulong guildID, SkillType skill, int amount, ulong goldPrice)
        {
            using var _context = new Context(_options);


            Profile profile = await GetOrCreateProfileAsync(discordID, guildID).ConfigureAwait(false);

            switch (skill)
            {
                case SkillType.None:
                    return false;
                case SkillType.Strength:
                    profile.Strength += amount;
                    break;
                case SkillType.Agility:
                    profile.Agility += amount;
                    break;
                case SkillType.Intelligence:
                    profile.Intelligence += amount;
                    break;
                case SkillType.Endurance:
                    profile.Endurance += amount;
                    break;
                case SkillType.Luck:
                    profile.Luck += amount;
                    break;
                default:
                    return false;
            }

            profile.Gold -= goldPrice;

            _context.Profiles.Update(profile);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        public async Task updateProfile(Profile profile)
        {
            using var _context = new Context(_options);

            _context.Profiles.Update(profile);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<bool> UseItemAsync(ulong discordID, ulong guildID, Item item)
        {
            using var _context = new Context(_options);

            Profile profile = await GetOrCreateProfileAsync(discordID, guildID).ConfigureAwait(false);

            var Item = profile.Items.FirstOrDefault(x => x.Item.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));

            if (Item != null)
            {
                profile.Items.Remove(Item);
                _context.ProfileItems.Remove(Item);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }

            //item has not been found in inventory
            return false;
        }
    }
}
