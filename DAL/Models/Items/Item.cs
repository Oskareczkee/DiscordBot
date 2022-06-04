using DAL.Models.Profiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Items
{
    public enum ItemType
    {
        None = 0,
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
        Miscellaneous = 11
    }

    public enum ItemBonusType
    {
        None = 0,
        Strength = 1,
        Agility = 2,
        Intelligence = 3,
        Endurance = 4,
        Luck = 5,
        Armor=6
    }

    public class Item : Entity
    {
        public Item()
        {
            Name = "None";
            Description = "None";
            Price = 0;
            Type = ItemType.None;
            ID = 0;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public ItemType Type { get; set; }

        //skill bonuses
        public int Strength { get; set; } = 0;
        public int Agility { get; set; } = 0;
        public int Intelligence { get; set; } = 0;
        public int Endurance { get; set; } = 0;
        public int Luck { get; set; } = 0;
        public int Armor { get; set; } = 0;

    }
}
