using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.SlashCommands.Metadata;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using TDLXUtils.Tools;
namespace TDLXUtils.Commands
{
    [Command("note"), InteractionAllowedContexts([DiscordInteractionContextType.Guild, DiscordInteractionContextType.PrivateChannel])]
    public class NoteCommands
    {
        [Command("open")]
        public static async Task openNoteAsync(SlashCommandContext ctx, [SlashAutoCompleteProvider<NoteEngine.ChoiceProvider>, Description("The note you want to modify, or create a new one.")] string title)
        {
            string? currentContent = NoteEngine.GetNote(ctx.User.Id, title);

            var modal = new DiscordModalBuilder()
                .WithTitle("TDLX-Notes")
                .WithCustomId($"NoteModal[{ctx.User.Id}]")
                .AddTextDisplay("You have 30 minutes to submit your note.")
                .AddTextInput(new DiscordTextInputComponent("noteTitle", "Title", title, false, DiscordTextInputStyle.Short, 1, 4000), "Note title")
                .AddTextDisplay("-# **Warning:** If you enter a title that already exists, the note with that title will be replaced.")
                .AddTextInput(new DiscordTextInputComponent("noteContent", "Content", currentContent, false, DiscordTextInputStyle.Paragraph, 0, 4000), "Note content")
                .AddTextDisplay("-# **Warning:** Do not enter any personal or sensitive information. TDLX-Utils does not guarantee the safety of your data.");
            
            await ctx.RespondWithModalAsync(modal);

            var modalResponse = await DiscordEngine.Interactivity.WaitForModalAsync($"NoteModal[{ctx.User.Id}]", TimeSpan.FromMinutes(30));
            await modalResponse.Result.Interaction.DeferAsync(true);

            if (modalResponse.TimedOut)
            {
                await modalResponse.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Note submission timed out."));
            }

            string inpTitle = (modalResponse.Result.Values["noteTitle"] as TextInputModalSubmission)!.Value;
            string inpContent = (modalResponse.Result.Values["noteContent"] as TextInputModalSubmission)!.Value;
            if (NoteEngine.GetNote(ctx.User.Id, inpTitle) is null)
            {
                DatabaseEngine.InsertData(DatabaseEngine.DBTable.Notes, new() { {"userid", ctx.User.Id}, {"title", inpTitle}, {"content", inpContent} });
            }
            else
            {
                if (string.IsNullOrEmpty(inpContent))
                    DatabaseEngine.DeleteData(DatabaseEngine.DBTable.Notes, [new("userid", "=", ctx.User.Id), new ("title", "=", inpTitle)]);
                else
                    DatabaseEngine.ModifyData(DatabaseEngine.DBTable.Notes, new() { {"content", inpContent} }, [new("userid", "=", ctx.User.Id), new ("title", "=", inpTitle)]);
            }
            await modalResponse.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"Created or modified note with title \"{inpTitle}\"."));
        }
        [Command("show")]
        public static async Task showNoteAsync(SlashCommandContext ctx, [SlashAutoCompleteProvider<NoteEngine.ChoiceProvider>, Description("The note you want to show publicly.")] string title )
        {
            await ctx.DeferResponseAsync();
            string? content = NoteEngine.GetNote(ctx.User.Id, title);
            if (content is null)
                await ctx.EditResponseAsync($"Error 404 - note with title \"{title}\" was not found.");
            else 
                await ctx.EditResponseAsync(content);
        }
        [Command("help")]
        public static async Task helpNoteAsync(SlashCommandContext ctx)
        {
            List<DiscordComponent> components = new();
            await ctx.DeferResponseAsync(true);

            components.Add(new DiscordTextDisplayComponent("# TDLX-Utils /note help"));
            components.Add(new DiscordSeparatorComponent(true));
            components.Add(new DiscordTextDisplayComponent(@"## /note open
With `/note open` you can create, modify, and delete your notes.
### Creating Notes
When you enter the command, you can either select `Create new Note...` or enter a title for a note that does not yet exist to create a new note.
After you enter the command with the given title, a window appears where you can enter the content of the note.
-# You can also change the title in that window. If you change the title to that of an existing note, the note will be overwritten.
### Modifying Notes
When you enter the command you can select any existing note to modify.
After you enter the command, the note will open and you can modify it. When you press Submit, the updated note will be saved with no way to undo the change.
### Deleting Notes
When modifying a note, leaving the content field empty will delete the note automatically with no way to undo the process.

-# Note: If you keep the window open for too long, the note times out and cannot be submitted."));
            components.Add(new DiscordSeparatorComponent(true));
            components.Add(new DiscordTextDisplayComponent(@"## /note show
With `/note show` you can publish your note to the current channel. It will then be sent to the channel, and everyone can see it.
-# Note: You might need the ""Use Application Commands"" permission to do that."));

            await ctx.EditResponseAsync(new DiscordMessageBuilder().EnableV2Components().AddContainerComponent(new (components, false, DiscordColor.Purple)));
        }
    }
}