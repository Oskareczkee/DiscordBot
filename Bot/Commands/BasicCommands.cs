using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

using System;
using System.Linq;
using System.Threading.Tasks;

using DiscordBot.Handlers.Dialogue.Steps;
using DiscordBot.Handlers.Dialogue;
using Bot.Attribures;
using Bot.Commands.Helpers;

namespace DiscordBot.Commands
{
    public class BasicCommands : BaseCommandModule
    {
        [Command("ping")]
        [RequireItem("Brak tatuażów Pawła")]
        [Description("Paweł gdzie są tatuaże?")]
        public async Task Ping(CommandContext ctx)
        {
            var Pawel = await ctx.Guild.GetMemberAsync(539867125704294400).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"{Pawel.Mention}zamknij mordę pls").ConfigureAwait(false);
        }

        [Command("add")]
        [Description("Dodaje 2 liczby do siebie")]
        public async Task Add(CommandContext ctx, 
                             [Description("Pierwsza liczba do dodania")]int numberOne, 
                             [Description("Druga liczba do dodania")]int numberTwo)
        {
            await ctx.Channel.SendMessageAsync((numberOne + numberTwo).ToString()).ConfigureAwait(false);
        }

        [Command("respondmessage")]
        [Description("responds to a message!")]
        public async Task Respond(CommandContext ctx)
        {
            var Interactivity = ctx.Client.GetInteractivity();

            var message = await Interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author==ctx.User).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }

        [Command("respondemoji")]
        [Description("responds to a emoji!")]
        public async Task RespondEmoji(CommandContext ctx)
        {
            var Interactivity = ctx.Client.GetInteractivity();

            var message = await Interactivity.WaitForReactionAsync(x => x.Channel == ctx.Channel && x.User == ctx.User).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(message.Result.Emoji);
        }

        [Command("ilepączkówzjadłeś")]
        [RequireItem("Nadwaga")]
        [Description("Wszystkie!!")]
        public async Task IlePaczkowZjadles(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("6");
            await Task.Delay(2000);
            await ctx.Channel.SendMessageAsync("dobre były");
        }

        [Command("poll")]
        [Description("Przeprowadza głosowanie, zlicza głosy na koniec")]
        public async Task Poll(CommandContext ctx,
            [Description("Nazwa głosowania")]string PollTitle,
            [Description("Opis głosowania")]string PollDescription,
            [Description("Długość trwania głosowania (ms/s/m/h/d)")]TimeSpan Duration ,
            [Description("Emoji użyte do głosowania")]params DiscordEmoji[] emojiOptions)
        {
            var interacitivty = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var embed = new DiscordEmbedBuilder
            {
                Title = PollTitle,
                Description = PollDescription,
                Color = DiscordColor.Aquamarine,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                     Url = Bot.Configuration.mamonPhotoURL,
                     Width = 128,
                     Height = 128
                }
            };

            var pollMess = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
                await pollMess.CreateReactionAsync(option).ConfigureAwait(false);

            var result = await interacitivty.CollectReactionsAsync(pollMess, Duration).ConfigureAwait(false);

            var resultCount =
                from Emoji in result
                group Emoji.Emoji by Emoji.Emoji into e
                select new { Emoji = e.Key, EmojiCount = e.Count() };

            foreach (var res in resultCount)
                await ctx.Channel.SendMessageAsync($"{res.Emoji}: {res.EmojiCount}\n");
        }

        [Command("huj")]
        [RequireItem("Metr budowniczy z Białegostoku")]
        [Description("Pokazuje rozmiar twojego huja")]

        public async Task Huj(CommandContext ctx, 
               [Description("(Opcjonalny) Wzmianka do użytkownika")]DiscordMember mention = null )
        {
            //set seed, seed changes every hour
            Random r;
            if(mention!=null)
                r= new Random((int)DateTime.Now.TimeOfDay.TotalHours + (int)mention.Id);
            else
                r = new Random((int)DateTime.Now.TimeOfDay.TotalHours + (int)ctx.User.Id);

            //generate size
            var dickSize = r.Next(0, 30);

            if (dickSize < 10)
                await ctx.Channel.SendMessageAsync("XDD ale mały huj").ConfigureAwait(false);
            if (dickSize >= 25)
                await ctx.Channel.SendMessageAsync("YOO niezły kutas!").ConfigureAwait(false);

            //upload proper message
            if (mention != null)
                await ctx.Channel.SendMessageAsync($"rozmiar huja {mention.Mention} wynosi {dickSize} cm");
            else
                await ctx.Channel.SendMessageAsync($"rozmiar huja {ctx.User.Mention} wynosi {dickSize} cm");
        }

        [Command("github")]
        [Description("Come and check my github!")]

        public async Task Github(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("There you go, catch my github!").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(@"https://github.com/JacexDowozIrytacji/DiscordBot").ConfigureAwait(false);
        }

        [Command("random")]
        [Description("generates random integer")]
        public async Task Random(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(BotMath.RandomNumberGenerator.Next().ToString());
        }

        [Command("combathelp")]
        [Description("Shows the basics of the combat")]
        public async Task CombatHelp(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Combat Basics",
                Description = "Learn how to fight efficiently",
                Color = DiscordColor.Aquamarine,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = Bot.Configuration.mamonPhotoURL,
                    Width = 128,
                    Height = 128
                }
            };

            embed.AddField("Strength", "Increases your damage, makes your attacks harder to dodge");
            embed.AddField("Agility", "Allows you to dodge enemies attacks, you can have maximally 50% to dodge an attack. Calculations: (defenderAgility * 1.25) - attackerStrength) * 0.01");
            embed.AddField("Intelligence", @"Increases your ability to cast a spell instead of normal attack and increases your spell damage, you can have maximally 50% chance to cast a spell. 
                                             Calculations: 
                                             Magick Attack Chance (max 50%): (attackerLuck + attackerIntelligence) * 0.01
                                             Magick Attack Damage Bonus (max 50%): (4 * attackerLevel) / defenderIntelligence");
            embed.AddField("Endurance", "Increases your health. HP = Endurance * 2 * (Level + 1)");
            embed.AddField("Luck", @"Increases your chance to deal critical hit, increases chance for magic attack and magic critical attack, Highers your chance for better XP and Gold rewards
                                     Calculations:
                                     Critical hit chance (max 50%): (attackerLuck * 5 / (defenderLevel * 2)) * 0.01
                                     Gold reward:  (RandomNumberGenerator.NextDouble() + 1) * attackerLuck / (attackerLevel * 10)*(defenderLevel*10)
                                     XP reward: (RandomNumberGenerator.NextDouble() + 1) * attackerLuck / (attackerLevel * 10)*(defenderLevel*20)");
            embed.AddField("Armor", "Not implemented yet");

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
    }
}