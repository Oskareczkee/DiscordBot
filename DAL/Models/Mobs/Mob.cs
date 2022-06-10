using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Mobs
{
    public class Mob: Entity
    {

        public string Name { get; set; }

        //to add some lore to monsters
        public string Description { get; set; }

        //skills
        public int Strength { get; set; } = 1;
        public int Agility { get; set; } = 1;
        public int Intelligence { get; set; } = 1;
        public int Endurance { get; set; } = 1;
        public int Luck { get; set; } = 1;

        //resistance is just number and should be converted into percents in math functions (max 75%)
        public int Resistance { get; set; } = 0;

        public int HP { get; set; } = 1;
        public int BaseDMG { get; set; } = 1;

        public int GoldAward { get; set; } = 1;
        public int XPAward { get; set; } = 1;
    }
}
