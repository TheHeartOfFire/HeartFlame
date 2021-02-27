using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.PatchNotes
{
    public class PatchNotesData
    {
        public List<Task> CompletedTasks { get; set; }
        public List<Task> UncompleteTasks { get; set; }
        public ulong ChannelId { get; set; }

        public PatchNotesData()
        {
            CompletedTasks = new List<Task>();
            UncompleteTasks = new List<Task>();
        }

        public bool CompleteTask(int TaskID, string Notes)
        {
            if (TaskID > UncompleteTasks.Count - 1)
                return false;

            UncompleteTasks[TaskID].Complete = true;
            UncompleteTasks[TaskID].Notes = Notes;

            PersistentData.SaveChangesToJson();
            return true;
        }

        public Task GetTask(string Name)
        {
            if (UncompleteTasks.Exists(x => x.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)))
                return UncompleteTasks.Find(x => x.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
            return null;
        }

        public Task GetTask(int Id)
        {
            if (Id > UncompleteTasks.Count - 1)
                return null;

            return UncompleteTasks[Id];
        }

        public int NewTask(string Name, string Details)
        {
            if (!(GetTask(Name) is null)) return -1;

            UncompleteTasks.Add(new Task() { Name = Name, Details = Details });
            PersistentData.SaveChangesToJson();

            return UncompleteTasks.IndexOf(GetTask(Name));
        }

        public List<Embed> GeneratePatchNotes() => TaskEmbed(MoveCompletedTasks(), "Patch notes", false);
        public List<Embed> CurrentTaskList() => TaskEmbed(UncompleteTasks, "Task List");
        public List<Embed> LastPatchNotes() => TaskEmbed(CompletedTasks.FindAll(Task => Task.LastBatch), "Patch notes", false);

        private List<Task> MoveCompletedTasks()
        {
            var Output = new List<Task>();

            CompletedTasks.ForEach(Task => Task.LastBatch = false);

            foreach(var Task in UncompleteTasks)
            {
                if(Task.Complete)
                {
                    Task.LastBatch = true;
                    Output.Add(Task);
                    CompletedTasks.Add(Task);
                    UncompleteTasks.Remove(Task);
                }
            }
            PersistentData.SaveChangesToJson();

            return Output;
        }

        public List<Embed> TaskEmbed(List<Task> Tasks, string Title = null, bool WithIds = true)
        {
            var Output = new List<Embed>();
            var Embed = new EmbedBuilder
            {
                Title = Title
            };

            foreach (var Task in Tasks)
            {
                Embed.AddField(
                    $"{(WithIds ? $"TaskID: {Tasks.IndexOf(Task)} | " : "")}{Task.Name} | {Task.Details}", 
                    $"Notes: {Task.Notes ?? "None"}");
                if(Embed.Fields.Count>=20)
                {
                    Output.Add(Embed.Build());
                    Embed = new EmbedBuilder();
                }
            }

            return Output;
        }
    }
}
