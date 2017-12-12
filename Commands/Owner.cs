﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using PassiveBOT.Configuration;
using PassiveBOT.preconditions;

namespace PassiveBOT.Commands
{
    [RequireOwner]
    public class Owner : ModuleBase
    {
        public readonly CommandService Service;
        public DiscordShardedClient Client;

        public Owner(CommandService service)
        {
            Service = service;
        }

        [Command("PurgeServers+", RunMode = RunMode.Async)]
        [Summary("PurgeServers+")]
        [Remarks("Delete old server configs")]
        public async Task DeleteServerConfigs()
        {
            await ReplyAsync("Working....");
            var purged = 0;
            foreach (var config in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "setup/server/")))
            {
                var p = Path.GetFileNameWithoutExtension(config);
                //Console.WriteLine(p);
                try
                {
                    var trythis = ((DiscordShardedClient) Context.Client).GetGuild(Convert.ToUInt64(p));
                    Console.WriteLine(trythis.Name);
                }
                catch
                {
                    File.Delete(config);
                    purged++;
                }
            }
            await ReplyAsync("Guilds Purged.\n" +
                             $"Purged: {purged}");
        }


        [Command("sethome+", RunMode = RunMode.Async)]
        [Summary("sethome+")]
        [Remarks("set the owner server")]
        [Ratelimit(1, 15, Measure.Seconds)]
        public async Task HomeAsync()
        {
            var homes = new Homeserver
            {
                GuildId = Context.Guild.Id,
                GuildName = Context.Guild.Name
            };
            Homeserver.SaveHome(homes);
            await ReplyAsync("Done");
        }

        [Command("setsuggest+")]
        [Summary("setsuggest+")]
        [Remarks("set the suggestion channel")]
        public async Task Suggest()
        {
            var file = Path.Combine(AppContext.BaseDirectory, $"setup/config/home.json");
            var home = JsonConvert.DeserializeObject<Homeserver>(File.ReadAllText(file));
            home.Suggestion = Context.Channel.Id;
            Homeserver.SaveHome(home);
            await ReplyAsync("Done");
        }

        [Command("seterror+")]
        [Summary("seterror+")]
        [Remarks("set the suggestion channel")]
        public async Task Error()
        {
            var file = Path.Combine(AppContext.BaseDirectory, $"setup/config/home.json");
            var home = JsonConvert.DeserializeObject<Homeserver>(File.ReadAllText(file));
            home.Error = Context.Channel.Id;
            Homeserver.SaveHome(home);
            await ReplyAsync("Done");
        }


        [Command("help+", RunMode = RunMode.Async)]
        [Summary("help+")]
        [Remarks("Owner Commands")]
        [Ratelimit(1, 15, Measure.Seconds)]
        public async Task Help2Async()
        {
            var description = "";
            foreach (var module in Service.Modules)
                if (module.Name == "Owner")
                    description = module.Commands.Aggregate(description,
                        (current, cmd) => current + $"{Load.Pre}{cmd.Aliases.First()} - {cmd.Remarks}\n");

            var embed = new EmbedBuilder()
                .WithTitle("Owner Commands")
                .WithDescription(description);
            await ReplyAsync("", false, embed.Build());
        }

        [Command("die+")]
        [Summary("die+")]
        [Remarks("Kills the bot")]
        public async Task Die()
        {
            await ReplyAsync("Bye Bye :heart:");
            Environment.Exit(0);
        }

        [Command("LeaveServer+")]
        [Summary("LeaveServer+ <guild ID> [Optional]<reason>")]
        [Remarks("Makes the bot leave the specified guild")]
        public async Task LeaveAsync(ulong id, [Remainder] string reason = "No reason provided by the owner.")
        {
            if (id <= 0)
                await ReplyAsync("Please enter a valid Guild ID");
            var gld = await Context.Client.GetGuildAsync(id);
            var ch = await gld.GetDefaultChannelAsync();

            await ch.SendMessageAsync($"haha fuck this shit I'm out... `{reason}`");
            await Task.Delay(5000);
            await gld.LeaveAsync();
            await ReplyAsync("Message has been sent and I've left the guild!");
        }


        [Command("GetInvite+")]
        [Summary("GetInvite+ <guild ID>")]
        [Remarks("Creat an invite to the specified server")]
        public async Task GetAsync(ulong id)
        {
            if (id <= 0)
                await ReplyAsync("Please enter a valid Guild ID");

            foreach (var guild in ((DiscordShardedClient) Context.Client).Guilds)
                if (guild.Id == id)
                    foreach (var channel in guild.Channels)
                        try
                        {
                            var inv = channel.CreateInviteAsync().Result.Url;
                            await ReplyAsync(inv);
                            return;
                        }
                        catch
                        {
                            //
                        }

            await ReplyAsync("No Invites able to be created.");
        }

        /*[Command("addpremium+")]
        [Summary("addpremium+")]
        [Remarks("Bot Creator Command")]
        public async Task Addpremium(params string[] keys)
        {
            try
            {
                var i = 0;
                var duplicates = "Dupes:\n";
                if (Program.Keys == null)
                {
                    Program.Keys = keys.ToList();
                    await ReplyAsync("list replaced.");
                    var obj1 = JsonConvert.SerializeObject(Program.Keys, Formatting.Indented);
                    File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "setup/keys.json"), obj1);
                    return;
                }
                foreach (var key in keys)
                {
                    var dupe = false;
                    foreach (var k in Program.Keys)
                        if (k == key)
                            dupe = true;
                    if (!dupe)
                    {
                        i++;
                        Program.Keys.Add(key); //NO DUPES
                    }
                    else
                    {
                        duplicates += $"{key}\n";
                    }
                }
                await ReplyAsync($"{keys.Length} Supplied\n" +
                                 $"{i} Added\n" +
                                 $"{duplicates}");
                var obj = JsonConvert.SerializeObject(Program.Keys, Formatting.Indented);
                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "setup/keys.json"), obj);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }*/

        [Command("GetServer+")]
        [Summary("Getserver+ <string>")]
        [Remarks("Get servers containing the privided string")]
        public async Task GetAsync([Remainder] string s)
        {
            var s2 = "";
            foreach (var guild in ((DiscordShardedClient) Context.Client).Guilds)
                if (guild.Name.ToLower().Contains(s.ToLower()))
                    s2 += $"{guild.Name} : {guild.Id}\n";
            if (s2 != "")
                await ReplyAsync(s2);
            else
                await ReplyAsync("No Servers containing the provided string available.");
        }

        [Command("Username+")]
        [Summary("username+ <name>")]
        [Remarks("Sets the bots username")]
        public async Task UsernameAsync([Remainder] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                await ReplyAsync("Value cannot be empty");
            await Context.Client.CurrentUser.ModifyAsync(x => x.Username = value).ConfigureAwait(false);
            await ReplyAsync("Bot Username updated").ConfigureAwait(false);
        }
    }
}