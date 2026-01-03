using System.Data;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace TDLXUtils.Tools
{
    public static class NoteEngine
    {
        public static string? GetNote(ulong userid, string title)
        {
            if (title == "createNewNote") return null;

            var data = DatabaseEngine.SelectEntrys(DatabaseEngine.DBTable.Notes, ["content"], [new ("userid", "=", userid), new ("title", "=", title)]);
            if (data.Rows.Count == 0) return null;

            return (string)data.Rows[0]["content"];
        }
        public class ChoiceProvider : IAutoCompleteProvider
        {
            public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
            {
                string? input = ctx.UserInput;
                var data = DatabaseEngine.SelectEntrys(DatabaseEngine.DBTable.Notes, ["title"], [new("userid", "=", ctx.User.Id), new("title", "LIKE", $"%{input}%")]);

                List<DiscordAutoCompleteChoice> list = new();
                list.Add(new DiscordAutoCompleteChoice("Create New...", "createNewNote"));
                foreach(DataRow row in data.Rows)
                {
                    string? title = row.ItemArray[0]!.ToString();
                    title ??= "Invalid Name! (null value)";
                    list.Add(new DiscordAutoCompleteChoice(title, title));
                }
                return list;
            }
        }
    }   
}