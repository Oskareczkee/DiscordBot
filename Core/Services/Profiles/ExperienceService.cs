using DAL;
using DAL.Models.Profiles;
using Microsoft.EntityFrameworkCore;
using static System.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Profiles
{
    public class GrantXPVievModel
    {
        public Profile Profile;
        public bool LevelledUp;
    }

    public interface IExperienceService
    {
        public Task<GrantXPVievModel> GrantXPAsync(ulong discordID, ulong guildID, int xpAmount);
    }

    public class ExperienceService : IExperienceService
    {
        private readonly DbContextOptions<Context> _options;
        private readonly IProfileService _profileService;

        public ExperienceService(DbContextOptions<Context> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }

        public async Task<GrantXPVievModel> GrantXPAsync(ulong discordID, ulong guildID, int xpAmount)
        {
            using var _context = new Context(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordID, guildID).ConfigureAwait(false);
            profile.XP += xpAmount;

            bool levelledUp = false;

            if(profile.XP>=profile.NextLevel)
            {
                profile.Level++;
                profile.NextLevel = 10 * ((int)Pow(profile.Level, 3));
                levelledUp = true;
            }

            _context.Profiles.Update(profile);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return new GrantXPVievModel
            {
                Profile = profile,
                LevelledUp = levelledUp
            };
        }
    }
}
