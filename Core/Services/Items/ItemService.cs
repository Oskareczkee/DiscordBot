using Core.Services.Profiles;
using DAL;
using DAL.Models.Items;
using DAL.Models.Profiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Items
{
    public enum ItemPurchaseState {ItemNotFound, NotEnoughGold, EverythingWentGood }
    public interface IItemService
    {
        Task CreateNewItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task<Item> GetItemByName(string name);

        Task<ItemPurchaseState> PurchaseItemAsync(ulong discordId, ulong guildID, string itemName);

        Task AddItemAsync(ulong discordID, ulong guildID, Item item);
        Task AddProfileItemAsync(ulong discordID, ulong guildID, ProfileItem item);
        Task<List<Item>> GetItemsListAsync();
        Task<bool> EquipItemAsync(ulong discordID, ulong guildID, ProfileItem item);
    }


    public class ItemService: IItemService
    {

        private readonly DbContextOptions<Context> _options;
        private readonly IProfileService _profileService;

        public ItemService(DbContextOptions<Context> options, IProfileService profileService)
        {
            _options = options;
            _profileService = profileService;
        }
         
        public async Task<Item> GetItemByName(string name)
        {
            using var _context = new Context(_options);

            name = name.ToLower();
            return await _context.Items.FirstOrDefaultAsync(x => x.Name.ToLower()==name.ToLower()).ConfigureAwait(false);
        }

        public async Task CreateNewItemAsync(Item item)
        {
            using var _context = new Context(_options);

           await _context.AddAsync(item).ConfigureAwait(false);
           await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateItemAsync(Item item)
        {
            using var _context = new Context(_options);

            _context.Items.Update(item);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ItemPurchaseState> PurchaseItemAsync(ulong discordId, ulong guildID, string itemName)
        {
            using var context = new Context(_options);

            Item item = await GetItemByName(itemName).ConfigureAwait(false);

            if (item == null)
                return ItemPurchaseState.ItemNotFound;

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordId, guildID).ConfigureAwait(false);

            if (profile.Gold < item.Price)
                return ItemPurchaseState.NotEnoughGold;

            profile.Gold -= item.Price;

            //if something wrong will happen, just take a look here
            //I don't know if I should do something like this
            //item gets copied with its unique id...
            profile.Items.Add(new ProfileItem
            {
                ProfileID = profile.ID,
                Item = item
            });

            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);

            return ItemPurchaseState.EverythingWentGood;

        }


        public async Task<List<Item>> GetItemsListAsync()
        {
            using var context = new Context(_options);
            var Items = await context.Items.ToListAsync().ConfigureAwait(false);

            return Items;
        }

        public async Task AddItemAsync(ulong discordID, ulong guildID, Item item)
        {
            //this function basically works like purchase item, but does not require gold, takes actual item as an argument

            using var context = new Context(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordID, guildID).ConfigureAwait(false);

            //if something wrong will happen, just take a look here
            //I don't know if I should do something like this
            //item gets copied with its unique id...
            profile.Items.Add(new ProfileItem
            {
                ProfileID=profile.ID,
                Item=item
            });

            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddProfileItemAsync(ulong discordID, ulong guildID, ProfileItem item)
        { 
            //XD it is like previous function but takes profileItem as an argument
            //I should have made some kind of hierarchy to cast Item into ProfileItem
            //this function basically works like purchase item, but does not require gold, takes actual item as an argument

            using var context = new Context(_options);

            Profile profile = await _profileService.GetOrCreateProfileAsync(discordID, guildID).ConfigureAwait(false);

            //if something wrong will happen, just take a look here
            //I don't know if I should do something like this
            //item gets copied with its unique id...
            profile.Items.Add(new ProfileItem { ProfileID = profile.ID, Item =item.Item});

            context.Profiles.Update(profile);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<bool> EquipItemAsync(ulong discordID, ulong guildID, ProfileItem item)
        {
            using var context = new Context(_options);

            bool succeded = await ChangeItem(discordID, guildID, item).ConfigureAwait(false);

            if (succeded)
                return true;
            return false;
        }

        //------------------------------------\\
        //helpers
        //-------------------------------------\\

        //returns true if everything went ok
        //this function checks whether profile has something equipped on the slot
        //if not it deletes item from inventory and adds it to the equipment
        //else adds item from equipment to inventory, adds item to equipment and then deletes it from inventory
        //i hope it will work
        //and oh god so much switches aaaaa, should have done it better
        private async Task<bool> ChangeItem(ulong discordID, ulong guildID, ProfileItem item)
        {
            using var context = new Context(_options);
            Profile profile = await _profileService.GetOrCreateProfileAsync(discordID, guildID);
 
            await context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

    }
}
