using DAL.Models.Profiles;
using System;
using static System.Math;

namespace Core.Math
{
    public class BotMath
    {
        public static Random RandomNumberGenerator = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Calculates the cost of levelling up a skill depending on its actual level
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public static ulong SkillLevelUpCost(int skillLevel)
        {
            //casting should work like flooring, we like player, so we offer him always floored price
            return (ulong)(Log(skillLevel) + (Pow(skillLevel, 2) * 0.01));
        }

        /// <summary>
        /// Calculates the full cost of leveling up a skill from starting level to ending level
        /// </summary>
        /// <param name="startingLevel"></param>
        /// <param name="endingLevel"></param>
        /// <returns></returns>
        public static ulong SkillLevelUpCost(int startingLevel, int endingLevel)
        {
            ulong output = 0;

            while(startingLevel<=endingLevel)
            {
                output += SkillLevelUpCost(startingLevel);
                startingLevel++;
            }

            return output;
        }


        //i dont know if this is a good practice to have a so much dependecies in simple calculations
        //maybe i should change it, but for now i will keep it

        /// <summary>
        /// calculates a price of particular skill from a particular profile
        /// </summary>
        /// <param name="skillType"></param>
        /// <param name="profile"></param>
        /// <param name="skillAmount"></param>
        /// <returns></returns>
        public static  ulong calculatePrice(SkillType skillType, Profile profile, int skillAmount)
        {
            //subtract by one, otherwise cost will be calculated to one skill too much
            skillAmount--;

            switch (skillType)
            {
                case SkillType.Strength:
                    return BotMath.SkillLevelUpCost(profile.Strength, profile.Strength + skillAmount);
                case SkillType.Agility:
                    return BotMath.SkillLevelUpCost(profile.Agility, profile.Agility + skillAmount);
                case SkillType.Intelligence:
                    return BotMath.SkillLevelUpCost(profile.Intelligence, profile.Intelligence + skillAmount);
                case SkillType.Endurance:
                    return BotMath.SkillLevelUpCost(profile.Endurance, profile.Endurance + skillAmount);
                case SkillType.Luck:
                    return BotMath.SkillLevelUpCost(profile.Luck, profile.Luck + skillAmount);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calulates dodge chance
        /// </summary>
        /// <param name="defenderAgility"></param>
        /// <param name="attackerStrength"></param>
        /// <returns>Chance for dodge, max chance is 50%</returns>
        public static double CalculateDodgeChance(int defenderAgility, int attackerStrength)
        {
            double output =((defenderAgility * 1.25) - attackerStrength) * 0.01;
            //maximally 50% chance for dodge
            if (output > 0.5)
                output = 0.5;
            return output;
        }

        /// <summary>
        /// Calculates critical hit chance
        /// </summary>
        /// <param name="attackerLuck"></param>
        /// <param name="defenderLevel"></param>
        /// <returns>Chance for critical hit, max chance is 50%</returns>
        public static double CalculateCritChance(int attackerLuck, int defenderLevel)
        {
            double output = (attackerLuck * 5 / (defenderLevel * 2)) * 0.01;
            //maximally 50% chance for critical hit
            if (output > 0.5)
                output = 0.5;
            return output;
        }

        /// <summary>
        /// Calculates magic attack chance
        /// </summary>
        /// <param name="attackerLuck"></param>
        /// <param name="attackerIntelligence"></param>
        /// <returns>Returns chance for magick attack, max chance is 50%</returns>
        public static double CalculateMagickChance(int attackerLuck, int attackerIntelligence)
        {
            double output = (attackerLuck + attackerIntelligence) * 0.01;
            //maximally 50% chance for magic attack
            if (output > 0.5)
                output = 0.5;
            return output;
        }

        /// <summary>
        /// Calculates amount of gold awarded for fight
        /// </summary>
        /// <param name="defenderLevel"></param>
        /// <param name="attackerLuck">Gives higher chance for better drop</param>
        /// <returns>Amount of gold</returns>
        public static int CalculateGoldAmount(int defenderLevel, int attackerLuck, int attackerLevel)
        {
            int output = defenderLevel * 10;
            double bonus = (RandomNumberGenerator.NextDouble() + 1) * attackerLuck / (attackerLevel * 10);
            return (int)(output * bonus);
        }

        /// <summary>
        /// Calculates amount of XP awarded for fight
        /// </summary>
        /// <param name="defenderLevel"></param>
        /// <param name="attackerLuck">Gives higher chance for better drop</param>
        /// <param name="attackerLevel"></param>
        /// <returns></returns>
        public static int CalculateXPAmount(int defenderLevel, int attackerLuck, int attackerLevel)
        {
            int output = defenderLevel * 20;
            double bonus = (RandomNumberGenerator.NextDouble() + 1) * attackerLuck / (attackerLevel * 10);
            return (int)(output * bonus);
        }

        /// <summary>
        /// Calculates bonus damage for magick attack, lowering it if defender has high intelligence
        /// </summary>
        /// <param name="attackerLevel"></param>
        /// <param name="defenderIntelligence"></param>
        /// <returns>Attack bonus, max is 50% more damage</returns>
        public static double CalculateMagickAttackBonus(int attackerLevel, int defenderIntelligence)
        {
            double output = (4 * attackerLevel) / defenderIntelligence;
            if (output > 0.5)
                output = 0.5;

            return 1 + output;
        }

        public static int CalculateDamage(int attackerStrength, int attackerBaseDMG, double attackerDamageMultiplier, double defenderResistance=0)
        {
            int Damage =(int)System.Math.Ceiling((attackerStrength / 2) * attackerDamageMultiplier * attackerBaseDMG);
            return (int)(Damage * (1 - defenderResistance));

        }

        /// <summary>
        /// Rolls a dice
        /// </summary>
        /// <param name="chance">Chance to roll true (should be in percents [0.25]=25%)</param>
        /// <returns></returns>
        public static bool Roll(double chance)
        {
            double roll = RandomNumberGenerator.NextDouble();
            if (chance >= roll)
                return true;
            return false;
        }
    }
}
