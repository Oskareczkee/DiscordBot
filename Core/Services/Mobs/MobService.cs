using DAL;
using DAL.Models.Mobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Math;

namespace Core.Services.Mobs
{
    public interface IMobService
    {
        Task CreateMob(Mob mob);
        Task<Mob> GetMobByName(string name);
        Task<Mob> GetRandomMob();

        //scales mob to specific level
        Mob ScaleMob(Mob mob, int level);
    }

    public class MobService : IMobService
    {
        private readonly DbContextOptions<Context> _options;
        public MobService(DbContextOptions<Context> options)
        {
            _options = options;
        }

        public async Task CreateMob(Mob mob)
        {
            using var _context = new Context(_options);

            await _context.Mobs.AddAsync(mob).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Mob> GetMobByName(string name)
        {
            using var _context = new Context(_options);
            return await _context.Mobs.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower()).ConfigureAwait(false);
        }

        public async Task<Mob> GetRandomMob()
        {
            using var _context = new Context(_options);
            int randomIndex = BotMath.RandomNumberGenerator.Next(1, _context.Mobs.Count());

            return await _context.Mobs.Where(x => x.ID == randomIndex).FirstOrDefaultAsync();
        }

        public Mob ScaleMob(Mob mob, int level)
        {
            mob.Strength +=(int)(5*level);
            mob.Agility += (int)(5 * level);
            mob.Intelligence += (int)(5 * level);
            mob.Endurance += (int)(5 * level);
            mob.Luck += level;

            mob.XPAward *= (int)(1.1 * level);
            mob.GoldAward *= (int)(1.2 * level);
            mob.HP += mob.Endurance * 2 * (level + 1);
            mob.BaseDMG += level+1;

            return mob;
        }
    }
}
