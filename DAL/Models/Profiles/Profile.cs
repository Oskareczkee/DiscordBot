using DAL.Models;
using DAL.Models.Items;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.Profiles
{
    public enum SkillType
    {
        None =0,
        Strength =1,
        Agility=2,
        Intelligence=3,
        Endurance=4,
        Luck=5
    }

    public class Profile : Entity
    {
        public ulong DiscordID { get; set; }
        public ulong GuildID { get; set; }

        //skills
        public int Strength { get; set; } = 10;
        public int Agility { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Endurance { get; set; } = 10;
        public int Luck { get; set; } = 10;
        public int Armor { get; set; } = 0;


        public int XP { get; set; }
        public int Level { get; set; } = 1;

        public int NextLevel { get; set; }
        public int HP => Endurance*2*(Level+1);
        public int BaseDMG => Level * 2;
        public double Gold { get; set; } = 100;
        public List<ProfileItem> Items { get; set; } = new List<ProfileItem>();
        public List<EquipmentItem> Equipment { get; set; } = new List<EquipmentItem>(9);

    }
}
