using Core.Math;
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
        Task<List<Item>> GetItemsListAsync();
        Task<Item> GetRandomItem();
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

        public async Task<Item> GetRandomItem()
        {
            using var _context = new Context(_options);
            int randomIndex = BotMath.RandomNumberGenerator.Next(1, _context.Items.Count());

            return await _context.Items.Where(x => x.ID == randomIndex).FirstOrDefaultAsync();
        }
    }
}
