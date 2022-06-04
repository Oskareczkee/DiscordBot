using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using System.IO;
using Bot.Attribures;

namespace DiscordBot.Commands
{
    public class ManageCommands : BaseCommandModule
    {
        [Command("gaytest")]
        [Description("Rzetelny test sprawdzania czy jest się gejem")]
        public async Task GayTest(CommandContext ctx)
        {
            var testEmbed = new DiscordEmbedBuilder
            {
                Title = "Czy jesteś naprawdę gejem?",
                Color = DiscordColor.Gold,
                Description = "Test na geja",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail 
                { 
                    Url = Bot.Configuration.mamonPhotoURL,
                    Width =128,
                    Height=128
                }

            };

            var testMessage = await ctx.Channel.SendMessageAsync(embed: testEmbed).ConfigureAwait(false);
            var thumbsUp = DiscordEmoji.FromName(ctx.Client,":+1:");
            var thumbsDown = DiscordEmoji.FromName(ctx.Client, ":-1:");

            await testMessage.CreateReactionAsync(thumbsUp).ConfigureAwait(false);
            await testMessage.CreateReactionAsync(thumbsDown).ConfigureAwait(false);

            var interactivity = ctx.Client.GetInteractivity();

            var reactionResult = await interactivity.WaitForReactionAsync(
                x=> x.Message == testMessage &&
                x.Emoji==thumbsUp || x.Emoji == thumbsDown &&
                x.User == ctx.User
                ).ConfigureAwait(false);

            if (reactionResult.Result.Emoji == thumbsUp)
                await ctx.Channel.SendMessageAsync("To trochę gejowo XD");
            else if (reactionResult.Result.Emoji == thumbsDown)
                await ctx.Channel.SendMessageAsync("No i to się szanuje");
            else
                await ctx.Channel.SendMessageAsync("Pojebane, nie ta emotka debilu XD");

        }

        [Command("pawel")]
        [Description("Pisze wiadomosc do Pawla")]
        [RequireItem("Klucz do serca Pawła")]

        //params allows not to use quotes to write messages
        public async Task Pawel(CommandContext ctx, 
            [Description("Wiadomosc do Pawla")] params string[] message)
        {
            if (!ctx.Guild.Members.TryGetValue(Bot.Configuration.IDPawla, out var Pawel))
            {
                await ctx.Channel.SendMessageAsync("Sadge, Pawla nie ma na tym serwerze").ConfigureAwait(false);
                return;
            }

            var pawelChannel = await Pawel.CreateDmChannelAsync().ConfigureAwait(false);
            await pawelChannel.SendMessageAsync(string.Join(" ", message)).ConfigureAwait(false);
        }

        [Command("mem")]
        [Description("Wysyla randomowego mema z folderu twórcy tego dzieła")]

        public async Task Mem(CommandContext ctx,
            [Description("(opcjonalny) nazwa mema do wysłania")] string name = null)
        {
            Random random;

            string Root = Bot.Configuration.MemeFolderRoot;

            string[] fileEntries = Directory.GetFiles(Root);
            
            if(name==null)
            {
                random = new Random((int)DateTime.Now.Ticks);
                int randomNum = random.Next(0, fileEntries.Length);

                using (var fs = new FileStream(fileEntries[randomNum], FileMode.Open, FileAccess.Read))
                {
                    await new DiscordMessageBuilder().WithContent("Dobra, coś tam znalazłem").WithFile(fs.Name, fs).SendAsync(ctx.Channel).ConfigureAwait(false);
                }
            }
            else
            {
                //await new DiscordMessageBuilder().WithContent($"{found} {fileEntries[2]} ddd").SendAsync(ctx.Channel);

                var found = Array.Find(fileEntries, x => x.Equals(Root +$@"\{name}.mp4"));

                if(found==default(string))
                {
                    await new DiscordMessageBuilder().WithContent("Ajaj, niestety nie znalazłem tego, czego szukałeś").SendAsync(ctx.Channel).ConfigureAwait(false);
                    return;
                }
                else
                    using (var fs = new FileStream(found, FileMode.Open, FileAccess.Read))
                    {
                        await new DiscordMessageBuilder().WithContent("Dobra, coś tam znalazłem").WithFile(fs.Name, fs).SendAsync(ctx.Channel).ConfigureAwait(false);
                    }
            }
        }


        [Command("pokazmemy")]
        [Description("Pokazuje wszystkie dostępne memy w folderze z memami")]
        public async Task ShowMemes(CommandContext ctx)
        {

            string Root = Bot.Configuration.MemeFolderRoot;

            var fileEntries = (from itemFile in Directory.GetFiles(Root)
                                    select Path.GetFileNameWithoutExtension(itemFile)).ToArray();

            string files = string.Join(" ", fileEntries);
            
            var Messembed = new DiscordEmbedBuilder
            {
                Title = "Memy",
                Description = "Nazwy wszystkich memów w folderze z memami",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = Bot.Configuration.mamonPhotoURL }
            };

            int start = 0;
            
            //I think there might be better solution, but i came up with this one
            //Maybe sometime i will try to upgrade it, try to make it simpler
            //field can contain max 1024 characters, thats why we make multiple fields
            //each field contain 40 values
            //There can be maximally 25 fields (25*40 memes)
            //await ctx.Channel.SendMessageAsync($"{fileEntries.Length}");
            Messembed.AddField("Memy: ", string.Join(", ", fileEntries,start,40));

            start += 40;
            while(true)
            {
                Messembed.AddField("... ", string.Join(", ", fileEntries, start, 40));
                start += 40;

                //if start will be out of range next iteration, create last field with remaining values
                if(start+40 > fileEntries.Length)
                {
                    int remaining = fileEntries.Length - start;
                    Messembed.AddField("...", string.Join(", ", fileEntries, start, remaining));
                    break;
                }
            }
           
            await ctx.Channel.SendMessageAsync(embed: Messembed).ConfigureAwait(false);
        }

      
    }
}
