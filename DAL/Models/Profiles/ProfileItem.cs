using DAL.Models;
using DAL.Models.Items;
using DAL.Models.Profiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Profiles
{
    public class ProfileItem : Entity
    {
        public int ProfileID { get; set; }
        [ForeignKey("ProfileID")]
        public Profile Profile { get; set; }

        public int ItemID { get; set; }

        [ForeignKey("ItemID")]
        public Item Item { get; set; }
    }
}
