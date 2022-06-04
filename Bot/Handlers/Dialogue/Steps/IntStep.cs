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
    public class IntStep : DialogueStepBase
    {
        private IDialogueStep _nextStep;
        private readonly int? _minValue;
        private readonly int? _maxValue;

        public IntStep(string content, IDialogueStep nextStep, int? minValue=null, int? maxValue = null) : base(content)
        {
            _nextStep = nextStep;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public Action<int> OnValidResult { get; set; } = delegate { };

        public override IDialogueStep NextStep => _nextStep;

        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Please respond below",
                Description = $"{user.Mention}, {_content}"
            };

            embedBuilder.AddField("To stop the dialogue", $"use {DiscordBot.Bot.Configuration.prefix}cancel command");

            if(_minValue.HasValue)
                embedBuilder.AddField("Min Value: ", $"{_minValue}");
            if (_maxValue.HasValue)
                embedBuilder.AddField("Max Value: ", $"{_maxValue}");

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

                if(!int.TryParse(MessageResult.Result.Content, out int inputValue))
                {
                    await TryAgain(channel, "Your input is not an integer").ConfigureAwait(false);
                    continue;
                }

                if (_minValue.HasValue)
                    if (inputValue < _minValue.Value)
                    {
                        await TryAgain(channel, $"Your input value {inputValue} is smaller than {_minValue}").ConfigureAwait(false);
                        continue;
                    }

                if (_maxValue.HasValue)
                    if (inputValue > _maxValue.Value)
                    {
                        await TryAgain(channel, $"Your input value {inputValue} is bigger than {_maxValue}").ConfigureAwait(false);
                        continue;
                    }

                OnValidResult(inputValue);

                return false;
            }
        }
    }
}
