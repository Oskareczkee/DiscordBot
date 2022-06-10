using Core.Services.Profiles;
using DAL;
using DAL.Models.Profiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Math;
using static System.Math;
using DAL.Models.Mobs;

namespace Core.Services.Combat
{

    public enum AttackType
    {
        Dodge=1,
        Attack=2,
        Critical=3,
        Magick=4,
        CriticalMagick=5
    }
    public class CombatInfo
    {
        public Profile attacker;
        public Profile defender;
        public int Damage = 0;
        //since hp is readonly we have to store damage
        public AttackType attackType;
        
    }

    public interface ICombatService
    {
        CombatInfo Attack(Profile attacker, Profile defender);
        CombatInfo Attack(Profile attacker, Profile defender, int attackerDamage);
    }


    public class CombatService : ICombatService
    {
        private readonly DbContextOptions<Context> _options;

        public CombatService(DbContextOptions<Context> options)
        {
            _options = options;
        }

        public CombatInfo Attack(Profile Attacker, Profile Defender)
        {
            CombatInfo outputInfo = new CombatInfo { attacker = Attacker, defender = Defender};

            double attackerDamageMultiplier = BotMath.RandomNumberGenerator.NextDouble()+0.5;
            double defenderDodgeChance = BotMath.CalculateDodgeChance(Defender.Agility, Attacker.Strength);
            //check if dodged
            double DodgeRoll = BotMath.RandomNumberGenerator.NextDouble();

            if(defenderDodgeChance>=DodgeRoll)
            {
                outputInfo.attackType = AttackType.Dodge;
                return outputInfo;
            }

            //if not dodged FIGHT
            //roll critical attack
            double criticalHitChance = BotMath.CalculateCritChance(Attacker.Luck, Defender.Level);
            double CritRoll = BotMath.RandomNumberGenerator.NextDouble();

            bool criticalHit = false;
            if (criticalHitChance >= CritRoll)
            {
                attackerDamageMultiplier *= 2;
                criticalHit = true;
            }

            double magickAttackChance = BotMath.CalculateMagickChance(Attacker.Luck, Attacker.Intelligence);
            double magicRoll = BotMath.RandomNumberGenerator.NextDouble();
            bool magick = false;
            if(magickAttackChance>=magicRoll)
            {
                attackerDamageMultiplier *= BotMath.CalculateMagickAttackBonus(Attacker.Level, Defender.Intelligence);
                magick = true;
            }

            int Damage = BotMath.CalculateDamage(Attacker.Strength, Attacker.BaseDMG, attackerDamageMultiplier);
            outputInfo.Damage = Damage;

            if (criticalHit && magick)
                outputInfo.attackType = AttackType.CriticalMagick;
            else if (criticalHit)
                outputInfo.attackType = AttackType.Critical;
            else if (magick)
                outputInfo.attackType = AttackType.Magick;
            else
                outputInfo.attackType = AttackType.Attack;

            return outputInfo;
        }

        //this one is compatibile with mobs
        public CombatInfo Attack(Profile Attacker, Profile Defender, int attackerDamage)
        {
            CombatInfo outputInfo = new CombatInfo { attacker = Attacker, defender = Defender };

            double attackerDamageMultiplier = BotMath.RandomNumberGenerator.NextDouble() + 0.5;
            double defenderDodgeChance = BotMath.CalculateDodgeChance(Defender.Agility, Attacker.Strength);
            //check if dodged
            double DodgeRoll = BotMath.RandomNumberGenerator.NextDouble();

            if (defenderDodgeChance >= DodgeRoll)
            {
                outputInfo.attackType = AttackType.Dodge;
                return outputInfo;
            }

            //if not dodged FIGHT
            //roll critical attack
            double criticalHitChance = BotMath.CalculateCritChance(Attacker.Luck, Defender.Level);
            double CritRoll = BotMath.RandomNumberGenerator.NextDouble();

            bool criticalHit = false;
            if (criticalHitChance >= CritRoll)
            {
                attackerDamageMultiplier *= 2;
                criticalHit = true;
            }

            double magickAttackChance = BotMath.CalculateMagickChance(Attacker.Luck, Attacker.Intelligence);
            double magicRoll = BotMath.RandomNumberGenerator.NextDouble();
            bool magick = false;
            if (magickAttackChance >= magicRoll)
            {
                attackerDamageMultiplier *= BotMath.CalculateMagickAttackBonus(Attacker.Level, Defender.Intelligence);
                magick = true;
            }

            int Damage = BotMath.CalculateDamage(Attacker.Strength, attackerDamage, attackerDamageMultiplier, Defender.Armor/100);
            outputInfo.Damage = Damage;


            if (criticalHit && magick)
                outputInfo.attackType = AttackType.CriticalMagick;
            else if (criticalHit)
                outputInfo.attackType = AttackType.Critical;
            else if (magick)
                outputInfo.attackType = AttackType.Magick;
            else
                outputInfo.attackType = AttackType.Attack;

            return outputInfo;
        }
    }
}
