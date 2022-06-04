using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Interactivity.Extensions;

namespace DiscordBot.Handlers.Dialogue.Steps
{
    public class TextStep : DialogueStepBase
    {
        private IDialogueStep _nextStep;
        private readonly int? _minLength;
        private readonly int? _maxLength;

        public TextStep(string content, IDialogueStep nextStep, int? minLength=null, int? maxLength = null) : base(content)
        {
            _nextStep = nextStep;
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public Action<string> OnValidResult { get; set; } = delegate { };

        public override IDialogueStep NextStep => _nextStep;

        public void SetNextStep(IDialogueStep step)
        {
            _nextStep = step;
        }

        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Please respond below",
                Description = $"{user.Mention}, {_content}"
            };

            embedBuilder.AddField("To stop the dialogue", $"use {DiscordBot.Bot.Configuration.prefix}cancel command");

            if(_minLength.HasValue)
                embedBuilder.AddField("Min Length: ", $"{_minLength} characters");
            if (_maxLength.HasValue)
                embedBuilder.AddField("Max Length: ", $"{_maxLength} characters");

            var interactivity = client.GetInteractivity();

            while(true)
            {
                var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);
                OnMessageAdded(embed);

                var MessageResult = await interactivity.WaitForMessageAsync
                    (x => x.ChannelId == channel.Id && x.Author.Id == user.Id).ConfigureAwait(false);

                OnMessageAdded(MessageResult.Result);

                if (MessageResult.Result.Content.Equals($"{DiscordBot.Bot.Configuration.prefix}cancel", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (_minLength.HasValue)
                    if (MessageResult.Result.Content.Length < _minLength.Value)
                    {
                        await TryAgain(channel, $"Your input is {_minLength.Value - MessageResult.Result.Content.Length} characters too short").ConfigureAwait(false);
                        continue;
                    }

                if (_maxLength.HasValue)
                    if (MessageResult.Result.Content.Length > _maxLength.Value)
                    {
                        await TryAgain(channel, $"Your input is {MessageResult.Result.Content.Length - _maxLength.Value} characters too long").ConfigureAwait(false);
                        continue;
                    }

                OnValidResult(MessageResult.Result.Content);

                return false;
            }
        }
    }
}
