using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.InteractionNamingPolicies;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using TDLXUtils.Commands;

namespace TDLXUtils.Tools
{
    public class DiscordEngine
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public static DiscordClient Client { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public static async Task CreateDiscordConnectionAsync(string token, LogLevel minimumLogLevel)
        {
            var builder = DiscordClientBuilder.CreateDefault(token, DiscordIntents.All);
            builder.UseInteractivity();

            builder.ConfigureExtraFeatures(c => {
                c.LogUnknownEvents = false;
                c.LogUnknownAuditlogs = false;
            });
            /*builder.ConfigureLogging(log =>
            {
               log.SetMinimumLevel(minimumLogLevel);
            });*/
            builder.UseCommands((provider, commands) =>
            {
                commands.AddCommands(typeof(NoteCommands));

                SlashCommandProcessor slashCommandProcessor = new(new SlashCommandConfiguration()
                {
                    NamingPolicy = new SnakeCaseNamingPolicy()
                });
                commands.AddProcessor(slashCommandProcessor);
            }, new CommandsConfiguration()
            {
                UseDefaultCommandErrorHandler = false,
            });
            Client = builder.Build();
            Interactivity =  (Client.ServiceProvider.GetService(typeof(InteractivityExtension)) as InteractivityExtension)!;
            await Client.ConnectAsync();

        }
    }
}